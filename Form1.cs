using Accord.Collections;
using Accord.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Face;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace VCD_Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach(var videoDevice in videoDevices)
            {
                lstSource.Items.Add(new ItemSource(videoDevice.Name, videoDevice.MonikerString));
            }
            if(lstSource.Items.Count == 1)
            {
                lstSource.SelectedIndex = 0;
                btnApply.PerformClick();
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Size inputSize = Config.getInstance().getCameraInputResolution();
            ItemSource item = (ItemSource)lstSource.SelectedItem;
            lblStatus.Text = "Not Connected";
            string id = "";
            Size approxSize = new Size(0, 0);
            if (item != null)
            {
                id = item.getID();
                var videoSource = new VideoCaptureDevice(id);
                var resolutions = videoSource.VideoCapabilities;
                foreach(var size in resolutions)
                {
                    var FrameSize = size.FrameSize;
                    if(FrameSize.Width == inputSize.Width && FrameSize.Height == inputSize.Height)
                    {
                        lblStatus.Text = "Found Best Resolution: 1920, 1080";
                        approxSize = FrameSize;
                        break;
                    }
                    if(approxSize.Width * approxSize.Height < FrameSize.Width * FrameSize.Height)
                    {
                        approxSize = FrameSize;
                    }
                }
                if(approxSize.Width != 1920 || approxSize.Height != 1080)
                {

                    lblStatus.Text = $"Found Max Resolution: {approxSize.Width}, {approxSize.Height}";
                }
            }
            if(id != "")
            {
                var track = new TrackForm();
                track.ShowDialog();
            }
        }
    }
    public class ItemSource
    {
        private string name;
        private string id;
        public ItemSource(string nn, string dd)
        {
            name = nn;
            id = dd;
        }
        public override string ToString()
        {
            return name;
        }
        public string getID()
        {
            return id;
        }
    }
}
