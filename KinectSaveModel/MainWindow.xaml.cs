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
        private WriteableBitmap colorBitmap;
        private byte[] colorPixels;
        KinectSensor sensor = KinectSensor.KinectSensors[0];
        byte[] pixelData;
        SkeletonReady sr = new SkeletonReady();
        SensorColorFrame scf = new SensorColorFrame();

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(Window_Loaded);
            this.Unloaded += new RoutedEventHandler(Window_Closed);
            sensor.ColorStream.Enable();
            sensor.SkeletonStream.Enable();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            sensor.Stop();
        }

        // Methode wanneer de window wordt geladen
        // Deze methode wordt aangeroepen wanneer het scherm is geladen
        private void Window_Loaded(object sender, EventArgs e)
        {
            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            colorPixels = new byte[sensor.ColorStream.FramePixelDataLength];
            colorBitmap = new WriteableBitmap(sensor.ColorStream.FrameWidth, sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
            Image.Source = colorBitmap;
            sensor.ColorFrameReady += scf.SensorColorFrameReady;
            sensor.SkeletonFrameReady += sr.skeletonFrameReady;
            sensor.Start();
        }
    }
}
