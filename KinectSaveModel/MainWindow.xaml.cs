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
        private byte[] colorPixels;
        public KinectSensor sensor = KinectSensor.KinectSensors[0];
        byte[] pixelData;
        private SkeletonReady sr;

        public MainWindow()
        {
            InitializeComponent();
            sr = new SkeletonReady();

            main = this;

            this.Loaded += new RoutedEventHandler(Window_Loaded);
            this.Unloaded += new RoutedEventHandler(Window_Closed);
            sensor.ColorStream.Enable();
            sensor.SkeletonStream.Enable();
        }

        internal static MainWindow main;
        internal string Status
        {
            get { return statusBarText.Text; }
            set { Dispatcher.Invoke(new Action(() => { statusBarText.Text = value; })); }
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
            //sensor.ColorFrameReady += SensorColorFrameReady;
            //sensor.SkeletonFrameReady += sr.skeletonFrameReady;
            detectMovement();
            sensor.Start();
        }

        private void detectMovement()
        {
            string text = System.IO.File.ReadAllText(@"C:\KinectSavedMovements\FirstMovement.txt");

            // Display the file contents to the console. Variable text is a string.
            System.Console.WriteLine("Contents of WriteText.txt = {0}", text);

            // Example #2
            // Read each line of the file into a string array. Each element
            // of the array is one line of the file.
            string[] lines = System.IO.File.ReadAllLines(@"C:\KinectSavedMovements\FirstMovementReaded.txt");

            // Display the file contents by using a foreach loop.
            System.Console.WriteLine("Contents of WriteLines2.txt = ");
            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                Debug.WriteLine("\t" + line);
            }

            // Keep the console window open in debug mode.
            Debug.WriteLine("Press any key to exit.");
        }

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
