using Accord.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace VCD_Demo
{
    class Config
    {
        private static string xmlFile = @"Config.xml";
        private static Config _instance = null;
        public static Config getInstance()
        {
            if (_instance == null)
            {
                _instance = new Config();
            }
            return _instance;
        }


        Config()
        {
            LoadOBSConfig();
        }


        // Face Detect
        private string _cameraName = "";
        private int _detectSpeed = 800;
        private int _ignoreTop = 25;
        private int _ignoreBottom = 25;
        private Size _detectMainArea = new Size(400, 400);
        private int _detectOutsideSec = 60;
        private Size _inputResolution = new Size(1920, 1080);
        private Size _outputResolution = new Size(854, 480);
        private double _zoomTopPadding = 0.5;
        private double _zoomTotalHeight = 3.5;
        private int _smoothXOffset = 100;
        private int _smoothXSpeed = 5;
        private int _smoothYOffset = 70;
        private int _smoothYSpeed = 5;
        private int _smoothZOffset = 20;
        private int _smoothZSpeed = 2;
        private Size _minFaceSize = new Size(100, 100);

        private void LoadOBSConfig()
        {
            var xml = XDocument.Load(xmlFile);
            var query = from c in xml.Root.Descendants("Config")
                        select c;
            foreach (var item in query)
            {
                switch ((string)item.Attribute("Key"))
                {

                    case "CameraName":
                        _cameraName = (string)item.Attribute("Value");
                        break;
                    case "DetectSpeed":
                        _detectSpeed = (int)item.Attribute("Value");
                        break;
                    case "IgnoreRegion":
                        _ignoreTop = (int)item.Attribute("Top");
                        _ignoreBottom = (int)item.Attribute("Bottom");
                        break;
                    case "MainArea":
                        _detectMainArea = new Size((int)item.Attribute("Width"), (int)item.Attribute("Height"));
                        _detectOutsideSec = (int)item.Attribute("Outside");
                        break;
                    case "InputResolution":
                        _inputResolution = new Size((int)item.Attribute("Width"), (int)item.Attribute("Height"));
                        break;
                    case "OutputResolution":
                        _outputResolution = new Size((int)item.Attribute("Width"), (int)item.Attribute("Height"));
                        break;
                    case "Zoom":
                        _zoomTopPadding = (double)item.Attribute("TopPadding");
                        _zoomTotalHeight = (double)item.Attribute("TotalHeight");
                        break;
                    case "Smoothing":
                        _smoothXOffset = (int)item.Attribute("XOffset");
                        _smoothXSpeed = (int)item.Attribute("XSpeed");
                        _smoothYOffset = (int)item.Attribute("YOffset");
                        _smoothYSpeed = (int)item.Attribute("YSpeed");
                        _smoothZOffset = (int)item.Attribute("ZOffset");
                        _smoothZSpeed = (int)item.Attribute("ZSpeed");
                        break;
                    case "MinFaceSize":
                        _minFaceSize = new Size((int)item.Attribute("Width"), (int)item.Attribute("Height"));
                        break;
                }
            }
        }

        public string getCameraName()
        {
            return _cameraName;
        }
        public int getDetectSpeed()
        {
            return _detectSpeed;
        }
        public int getTopIgnorePercent()
        {
            return _ignoreTop;
        }
        public int getBottomIgnorePercent()
        {
            return _ignoreBottom;
        }
        public Size getDetectMainArea()
        {
            return _detectMainArea;
        }
        public int getDetectOutsideWaitSeconds()
        {
            return _detectOutsideSec;
        }
        public Size getCameraInputResolution()
        {
            return _inputResolution;
        }
        public Size getOutputOutputResolution()
        {
            return _outputResolution;
        }
        public double getZoomTopPadding()
        {
            return _zoomTopPadding;
        }
        public double getZoomTotalHeight()
        {
            return _zoomTotalHeight;
        }
        public int getSmoothingXOffset()
        {
            return _smoothXOffset;
        }
        public int getSmoothingXSpeed()
        {
            return _smoothXSpeed;
        }
        public int getSmoothingYOffset()
        {
            return _smoothYOffset;
        }
        public int getSmoothingYSpeed()
        {
            return _smoothYSpeed;
        }
        public int getSmoothingZOffset()
        {
            return _smoothZOffset;
        }
        public int getSmoothingZSpeed()
        {
            return _smoothZSpeed;
        }
        public int getMinFaceWidth()
        {
            return _minFaceSize.Width;
        }
        public int getMinFaceHeight()
        {
            return _minFaceSize.Height;
        }
    }
}
