﻿using Accord.Video;
using Accord.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace VCD_Demo
{
    public partial class TrackForm : Form
    {
        private int XOffset, XSpeed, YOffset, YSpeed, ZOffset, ZSpeed;
        private Size mainDetectArea;
        private int outsideSec;
        private int topIgnore, bottomIgnore;
        private double topPadding, totalHeight;
        private Size inputResolution, outputResolution;
        private string cameraName;

        private Thread cameraStart = null;
        private CascadeClassifier faceDetector;
        private VideoCaptureDevice cameraSource = null;
        private Thread detectThread = null;
        private DateTime lastDetect = DateTime.Now;
        private Thread calcThread = null;
        private Size curSize, detectedSize;

        private Point curPoint, detectedPoint;
        private DateTime lastMidPersonDetect;

        public TrackForm()
        {
            InitializeComponent();
        }

        private void TrackForm_Load(object sender, EventArgs e)
        {
            LoadInitialValues();
            Width = outputResolution.Width;
            Height = outputResolution.Height;

            cameraStart = new Thread(StartCamera);
            cameraStart.Start();
        }
            
        private void TrackForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Terminate();
        }

        private void LoadInitialValues()
        {
            Config config = Config.getInstance();

            topIgnore = config.getTopIgnorePercent();
            bottomIgnore = config.getBottomIgnorePercent();

            mainDetectArea = config.getDetectMainArea();
            outsideSec = config.getDetectOutsideWaitSeconds();

            topPadding = config.getZoomTopPadding();
            totalHeight = config.getZoomTotalHeight();

            XOffset = config.getSmoothingXOffset();
            XSpeed = config.getSmoothingXSpeed();
            YOffset = config.getSmoothingYOffset();
            YSpeed = config.getSmoothingYSpeed();
            ZOffset = config.getSmoothingZOffset();
            ZSpeed = config.getSmoothingZSpeed();

            inputResolution = config.getCameraInputResolution();
            outputResolution = config.getOutputOutputResolution();
            cameraName = config.getCameraName();

            faceDetector = new CascadeClassifier("Detect/haarcascade_frontalface.xml");
        }

        private void StartCamera()
        {
            while (true)
            {
                if (getCamera())
                    return;
                Thread.Sleep(5000);
            }
        }

        private bool getCamera()
        {
            try
            {
                var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                foreach (var videoDevice in videoDevices)
                {
                    if (videoDevice.Name == cameraName)
                    {
                        cameraSource = new VideoCaptureDevice(videoDevice.MonikerString);
                        var resolutions = cameraSource.VideoCapabilities;
                        Size approxSize = new Size(0, 0);
                        foreach (var cap in resolutions)
                        {
                            var FrameSize = cap.FrameSize;
                            if (FrameSize.Width == inputResolution.Width && FrameSize.Height == inputResolution.Height)
                            {
                                cameraSource.VideoResolution = cap;
                                break;
                            }
                            if (approxSize.Width * approxSize.Height < FrameSize.Width * FrameSize.Height)
                            {
                                approxSize = FrameSize;
                                cameraSource.VideoResolution = cap;
                            }
                        }
                        if (cameraSource.VideoResolution != null)
                        {
                            inputResolution = cameraSource.VideoResolution.FrameSize;
                        }

                        curPoint = detectedPoint = new Point(inputResolution.Width / 2, inputResolution.Height / 2);
                        curSize = detectedSize = new Size((int)(outputResolution.Height / totalHeight), (int)(outputResolution.Height / totalHeight));

                        cameraSource.NewFrame += NewFrame_EventHandler;
                        cameraSource.Start();
                        return true;
                    }
                }
            }
            catch (Exception) { }
            return false;
        }

        private void NewFrame_EventHandler(object sender, NewFrameEventArgs e)
        {
            try
            {
                if ((DateTime.Now - lastDetect).TotalMilliseconds > Config.getInstance().getDetectSpeed() && (detectThread == null || !detectThread.IsAlive))
                {
                    Bitmap detectSource = (Bitmap)e.Frame.Clone();
                    lastDetect = DateTime.Now;
                    detectThread = new Thread(DetectWork);
                    detectThread.Start(detectSource);
                }
                if (calcThread == null || !calcThread.IsAlive)
                {
                    Bitmap calcSource = (Bitmap)e.Frame.Clone();
                    calcThread = new Thread(Calculate);
                    calcThread.Start(calcSource);
                }
            }
            catch (Exception) { }
        }
        private void DetectWork(object param)
        {
            Bitmap frame = (Bitmap)param;
            Bitmap target = new Bitmap(frame.Width, frame.Height * (100 - topIgnore - bottomIgnore) / 100);
            lastMidPersonDetect = DateTime.Now.AddDays(-1);
            using (Graphics g = Graphics.FromImage(target))
            {
                g.DrawImage(frame, new Rectangle(0, 0, target.Width, target.Height),
                                 new Rectangle(0, frame.Height * topIgnore / 100, target.Width, target.Height),
                                 GraphicsUnit.Pixel);
            }

            Rectangle[] faces = null;
            using (var userPicture = target.ToImage<Gray, byte>())
            {
                faces = faceDetector.DetectMultiScale(userPicture, 1.3, 6);
            }

            if (faces != null && faces.Length > 0)
            {
                int minD = -1;
                Rectangle approx = new Rectangle();

                int midD = -1;
                Rectangle middle = new Rectangle();
                Point center = new Point(inputResolution.Width / 2, inputResolution.Height / 2);

                foreach (var face in faces)
                {
                    face.Offset(0, inputResolution.Height * topIgnore / 100);
                    Point tmp = new Point(face.X + face.Width / 2, face.Y + face.Height / 2);
                    int dist = (tmp.X - center.X) * (tmp.X - center.X) + (tmp.Y - center.Y) * (tmp.Y - center.Y);
                    if ((Math.Abs(tmp.X - center.X) < mainDetectArea.Width / 2 && Math.Abs(tmp.Y - center.Y) < mainDetectArea.Height / 2) && (midD == -1 || midD > dist))
                    {
                        midD = dist;
                        middle = face;
                    }
                    if ((Math.Abs(tmp.X - center.X) >= mainDetectArea.Width / 2 || Math.Abs(tmp.Y - center.Y) >= mainDetectArea.Height / 2) && (minD == -1 || minD > dist))
                    {
                        minD = dist;
                        approx = face;
                    }
                }

                if (midD >= 0
                    && middle.Size.Width > Config.getInstance().getMinFaceWidth() && middle.Size.Height > Config.getInstance().getMinFaceHeight())
                {
                    lastMidPersonDetect = DateTime.Now;
                    detectedSize = middle.Size;
                    detectedPoint = new Point(middle.X + detectedSize.Width / 2, middle.Y + detectedSize.Height / 2);
                    //detected = true;
                }
                else if (minD >= 0 && (DateTime.Now - lastMidPersonDetect).TotalSeconds > outsideSec
                    && approx.Size.Width > Config.getInstance().getMinFaceWidth() && approx.Size.Height > Config.getInstance().getMinFaceHeight())
                {
                    detectedSize = approx.Size;
                    detectedPoint = new Point(approx.X + detectedSize.Width / 2, approx.Y + detectedSize.Height / 2);
                    //detected = true;
                }
                else
                {
                    //detected = false;
                }

            }
            else
            {
                //detected = false;
            }

            target.Dispose();
            frame.Dispose();
        }

        private void Calculate(object frame)
        {
            try
            {
                Bitmap srcbmp = (Bitmap)frame;
                Image<Bgr, byte> source = srcbmp.ToImage<Bgr, byte>();
                srcbmp.Dispose();
                int deltaZoom = 0;
                if (Math.Abs(detectedSize.Width - curSize.Width) > ZOffset)
                {
                    deltaZoom = (detectedSize.Width - curSize.Width) / Math.Abs(detectedSize.Width - curSize.Width) * ZSpeed;
                }
                curSize = new Size(curSize.Width + deltaZoom, curSize.Height + deltaZoom);

                int deltaX = 0, deltaY = 0;
                if (Math.Abs(detectedPoint.X - curPoint.X) > XOffset)
                {
                    deltaX = (detectedPoint.X - curPoint.X) / Math.Abs(detectedPoint.X - curPoint.X) * XSpeed;
                }
                if (Math.Abs(detectedPoint.Y - curPoint.Y) > YOffset)
                {
                    deltaY = (detectedPoint.Y - curPoint.Y) / Math.Abs(detectedPoint.Y - curPoint.Y) * YSpeed;
                }
                curPoint = new Point(curPoint.X + deltaX, curPoint.Y + deltaY);

                int realHeight = (int)(curSize.Height * totalHeight);
                int realWidth = realHeight * outputResolution.Width / outputResolution.Height;
                int x = curPoint.X - realWidth / 2;
                int y = curPoint.Y - (int)(curSize.Height * (0.5 + topPadding));

                if (x < 0) x = 0;
                if (x > inputResolution.Width - realWidth) x = inputResolution.Width - realWidth;

                if (y < 0) y = 0;
                if (y > inputResolution.Height - realHeight) y = inputResolution.Height - realHeight;

                source.ROI = new Rectangle(x, y, realWidth, realHeight);
                Image<Bgr, byte> newimg = source.Copy().Resize(outputResolution.Width, outputResolution.Height, Inter.Linear);
                resultImage.Image = newimg;
                try
                {
                    Invoke(new Action(() =>
                    {
                        Program.Broadcast(newimg.ToJpegData()); 
                    }));
                }
                catch (Exception e)
                {
                    Console.WriteLine("\n" + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " : Error happened before Broadcast");
                    Console.WriteLine(e.Message);
                    Console.WriteLine("-------------------------------");
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine("###############################\n");
                }
                source.Dispose();
            }
            catch (Exception) { }
        }

        public void Terminate()
        {
            if (cameraStart != null && cameraStart.IsAlive)
            {
                cameraStart.Abort();
            }
            if (cameraSource != null && cameraSource.IsRunning)
            {
                cameraSource.SignalToStop();
            }
            if (detectThread != null && detectThread.IsAlive)
            {
                detectThread.Abort();
            }
            if (calcThread != null && calcThread.IsAlive)
            {
                calcThread.Abort();
            }
        }
    }
}
