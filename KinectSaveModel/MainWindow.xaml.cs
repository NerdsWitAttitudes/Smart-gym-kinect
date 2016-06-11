using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Linq;
using System.IO;
using System.Windows.Media.Media3D;
using System.Collections.Generic;
using System.Diagnostics;

namespace KinectSaveModel
{
    public partial class MainWindow : Window
    {
        public static WriteableBitmap colorBitmap;
        public byte[] colorPixels;
        public KinectSensor sensor = KinectSensor.KinectSensors[0];
        private SkeletonReady sr;
        private readFile read;
        public String previewPath = @"C:\Users\Dave\Documents\FirstMovement\FirstMovement_1.txt";

        public MainWindow()
        {
            InitializeComponent();
            sr = new SkeletonReady();

            main = this;

            this.Loaded += new RoutedEventHandler(Window_Loaded);
            this.Unloaded += new RoutedEventHandler(Window_Closed);
            sensor.ColorStream.Enable();
            sensor.SkeletonStream.Enable();
            read = new readFile(previewPath);
        }

        // To get and set the statusbar text, this can be used for messages
        internal static MainWindow main;
        internal string Status
        {
            get { return statusBarText.Text; }
            set { Dispatcher.Invoke(new Action(() => { statusBarText.Text = value; })); }
        }
        internal List<Double[]> Averages
        {
            get { return read.getAverages(); }
        }
        internal String[] Joints
        {
            get { return read.getJoints(); }
        }
        internal String getPreviewPath
        {
            get { return previewPath; }
        }

        // Method called when the window is closed
        private void Window_Closed(object sender, EventArgs e)
        {
            sensor.Stop();
        }

        // This method will be called when the window is loaded
        private void Window_Loaded(object sender, EventArgs e)
        {
            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            colorPixels = new byte[sensor.ColorStream.FramePixelDataLength];
            colorBitmap = new WriteableBitmap(sensor.ColorStream.FrameWidth, sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
            Image.Source = colorBitmap;
            sensor.ColorFrameReady += SensorColorFrameReady;
            sensor.SkeletonFrameReady += sr.skeletonFrameReady;
            sensor.Start();
        }

        // The method too show what the kinect sees
        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    colorFrame.CopyPixelDataTo(colorPixels);
                    MainWindow.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, MainWindow.colorBitmap.PixelWidth, colorBitmap.PixelHeight),
                        colorPixels,
                        colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }
        }
    }
}
