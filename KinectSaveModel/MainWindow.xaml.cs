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
        Skeleton[] skeleton;
        int frameNumber = 0;
        private string path = @"C:\KinectSavedMovements\FirstMovement.txt";

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(Window_Loaded);
            this.Unloaded += new RoutedEventHandler(Window_Closed);
            sensor.ColorStream.Enable();
            sensor.SkeletonStream.Enable();
        }

        void skeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            bool dataReceived = false;
            using (SkeletonFrame SkeletonFrame = e.OpenSkeletonFrame())
            {
                if (SkeletonFrame == null)
                {
                    Debug.WriteLine("frame == null");
                    // Moet nog een foutmelding voor aangemaakt worden
                }
                else
                {
                    skeleton = new Skeleton[SkeletonFrame.SkeletonArrayLength];
                    SkeletonFrame.CopySkeletonDataTo(skeleton);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {

                Skeleton currentSkeleton = (from s in skeleton
                                            where s.TrackingState == SkeletonTrackingState.Tracked
                                            select s).FirstOrDefault();
                if (currentSkeleton != null)
                {
                    List<Vector3D> vectors = new List<Vector3D>();
                    vectors.Add(new Vector3D(currentSkeleton.Joints[JointType.ShoulderCenter].Position.X, currentSkeleton.Joints[JointType.ShoulderCenter].Position.Y, currentSkeleton.Joints[JointType.ShoulderCenter].Position.Z));
                    vectors.Add(new Vector3D(currentSkeleton.Joints[JointType.ShoulderRight].Position.X, currentSkeleton.Joints[JointType.ShoulderRight].Position.Y, currentSkeleton.Joints[JointType.ShoulderRight].Position.Z));
                    vectors.Add(new Vector3D(currentSkeleton.Joints[JointType.ShoulderLeft].Position.X, currentSkeleton.Joints[JointType.ShoulderLeft].Position.Y, currentSkeleton.Joints[JointType.ShoulderLeft].Position.Z));
                    vectors.Add(new Vector3D(currentSkeleton.Joints[JointType.ElbowRight].Position.X, currentSkeleton.Joints[JointType.ElbowRight].Position.Y, currentSkeleton.Joints[JointType.ElbowRight].Position.Z));
                    vectors.Add(new Vector3D(currentSkeleton.Joints[JointType.ElbowLeft].Position.X, currentSkeleton.Joints[JointType.ElbowLeft].Position.Y, currentSkeleton.Joints[JointType.ElbowLeft].Position.Z));
                    vectors.Add(new Vector3D(currentSkeleton.Joints[JointType.WristRight].Position.X, currentSkeleton.Joints[JointType.WristRight].Position.Y, currentSkeleton.Joints[JointType.WristRight].Position.Z));
                    vectors.Add(new Vector3D(currentSkeleton.Joints[JointType.WristLeft].Position.X, currentSkeleton.Joints[JointType.WristLeft].Position.Y, currentSkeleton.Joints[JointType.WristLeft].Position.Z));
                    
                    writeToFile(vectors);
                }
            }
        }

        private void writeToFile(List<Vector3D> vectorList)
        {
            string line = "";
            String timeStamp = GetTimestamp(DateTime.Now);
            String frame = ""+frameNumber;
            line += frame+",["+timeStamp + "]@";
            for (int i = 0; i < vectorList.Count; i++)
            {
                if (i != (vectorList.Count-1))
                {
                    line += "[" + vectorList[i] + "],";
                }
                else
                {
                    line += "[" + vectorList[i] + "]";
                }
            }
            if (!File.Exists(path))
            {
                File.WriteAllText(path, line);
            }
                else
            {
                if (frameNumber == 0)
                {
                    String date = GetTimestamp(DateTime.Now);
                    date = date.Replace("/", "-");
                    date = date.Replace(" ", "_");
                    date = date.Replace(":", "-");
                    path = @"C:\KinectSavedMovements\FirstMovement_" + date + ".txt";
                }
                File.AppendAllText(path, Environment.NewLine + line);
            }
            frameNumber = frameNumber + 1;
        }

        public String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyy:MM:dd HH/mm/ss/ffff");
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
            colorBitmap = new WriteableBitmap(sensor.ColorStream.FrameWidth, this.sensor.ColorStream.FrameHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
            Image.Source = this.colorBitmap;
            this.sensor.ColorFrameReady += this.SensorColorFrameReady;
            sensor.SkeletonFrameReady += skeletonFrameReady;
            sensor.Start();
        }

        private void SensorColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    colorFrame.CopyPixelDataTo(this.colorPixels);
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.colorPixels,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);
                }
            }
        }
    }
}
