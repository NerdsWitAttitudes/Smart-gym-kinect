using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Linq;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace KinectSaveModel
{
    public partial class MainWindow : Window
    {
        KinectSensor sensor = KinectSensor.KinectSensors[0];
        byte[] pixelData;
        Skeleton[] skeleton;

        public MainWindow()
        {
            InitializeComponent();

            // Test data benodigd voor het schrijven naar bestand te testen
            /*Vector3D ShoulderCenter = new Vector3D(10,10,30);
            Vector3D RightShoulder = new Vector3D(11,11,31);
            Vector3D LeftShoulder = new Vector3D(12,12,32);
            Vector3D RightElbow = new Vector3D(13,13,33);
            Vector3D LeftElbow = new Vector3D(14,14,34);
            Vector3D RightWrist = new Vector3D(15,15,35);
            Vector3D LeftWrist = new Vector3D(16,16,36);

            List<Vector3D> vectors = new List<Vector3D>();
            vectors.Add(ShoulderCenter);
            vectors.Add(RightShoulder);
            vectors.Add(LeftShoulder);
            vectors.Add(RightElbow);
            vectors.Add(LeftElbow);
            vectors.Add(RightWrist);
            vectors.Add(LeftWrist);

            writeToFile(vectors);*/

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
                    
                    Vector3D ShoulderCenter = new Vector3D(currentSkeleton.Joints[JointType.ShoulderCenter].Position.X, currentSkeleton.Joints[JointType.ShoulderCenter].Position.Y, currentSkeleton.Joints[JointType.ShoulderCenter].Position.Z);
                    Vector3D RightShoulder = new Vector3D(currentSkeleton.Joints[JointType.ShoulderRight].Position.X, currentSkeleton.Joints[JointType.ShoulderRight].Position.Y, currentSkeleton.Joints[JointType.ShoulderRight].Position.Z);
                    Vector3D LeftShoulder = new Vector3D(currentSkeleton.Joints[JointType.ShoulderLeft].Position.X, currentSkeleton.Joints[JointType.ShoulderLeft].Position.Y, currentSkeleton.Joints[JointType.ShoulderLeft].Position.Z);
                    Vector3D RightElbow = new Vector3D(currentSkeleton.Joints[JointType.ElbowRight].Position.X, currentSkeleton.Joints[JointType.ElbowRight].Position.Y, currentSkeleton.Joints[JointType.ElbowRight].Position.Z);
                    Vector3D LeftElbow = new Vector3D(currentSkeleton.Joints[JointType.ElbowLeft].Position.X, currentSkeleton.Joints[JointType.ElbowLeft].Position.Y, currentSkeleton.Joints[JointType.ElbowLeft].Position.Z);
                    Vector3D RightWrist = new Vector3D(currentSkeleton.Joints[JointType.WristRight].Position.X, currentSkeleton.Joints[JointType.WristRight].Position.Y, currentSkeleton.Joints[JointType.WristRight].Position.Z);
                    Vector3D LeftWrist = new Vector3D(currentSkeleton.Joints[JointType.WristLeft].Position.X, currentSkeleton.Joints[JointType.WristLeft].Position.Y, currentSkeleton.Joints[JointType.WristLeft].Position.Z);

                    List<Vector3D> vectors = new List<Vector3D>();
                    vectors.Add(ShoulderCenter);
                    vectors.Add(RightShoulder);
                    vectors.Add(LeftShoulder);
                    vectors.Add(RightElbow);
                    vectors.Add(LeftElbow);
                    vectors.Add(RightWrist);
                    vectors.Add(LeftWrist);

                    writeToFile(vectors);

                }
            }
        }

        private void writeToFile(List<Vector3D> vectorList)
        {
            string line = "";
            String timeStamp = GetTimestamp(DateTime.Now);
            line += timeStamp + "@";
            for (int i = 0; i < vectorList.Count; i++)
            {
                line += "["+vectorList[i]+"]";
            }
            string path = @"C:\KinectSavedMovements\FirstMovement.txt";
            if (!File.Exists(path))
            {
                File.WriteAllText(path, line);
            }
                else
            {
                File.AppendAllText(path, Environment.NewLine + line);
                

                // Deze moet alleen bij de eerste frame gebruikt worden. Op deze manier overscrhijft hij geen bestaande bestanden
                /*String date = GetTimestamp(DateTime.Now);
                date = date.Replace("/","-");
                date = date.Replace(" ","_");
                date = date.Replace(":","-");
                path = @"C:\KinectSavedMovements\FirstMovement_"+ date + ".txt";*/
            }
        }

        public static String GetTimestamp(DateTime value)
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
            sensor.SkeletonFrameReady += skeletonFrameReady;
            sensor.Start();
        }
    }
}
