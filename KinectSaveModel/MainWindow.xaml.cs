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
            for (int i = 0; i < vectorList.Count; i++)
            {
                line += vectorList.Cast<String>();
            }
            string path = @"C:\KinectSavedMovements\FirstMovement.txt";
            if (!File.Exists(path))
            {
                string createText = "" + Environment.NewLine;
                File.WriteAllText(path, createText);
            }

            File.AppendAllText(path, line);

            string readText = File.ReadAllText(path);
        }

        // Deze methode genereert de positie van elke joint (de gewrichten van een persoon)
        private void SetEllipsePosition(Ellipse ellipse, Joint joint)
        {
            SkeletonPoint vector = new SkeletonPoint();
            vector.X = ScaleVector(640, joint.Position.X);
            vector.Y = ScaleVector(480, -joint.Position.Y);
            vector.Z = joint.Position.Z;
            Joint updatedJoint = new Joint();
            updatedJoint = joint;
            updatedJoint.TrackingState = JointTrackingState.Tracked;
            updatedJoint.Position = vector;

            Canvas.SetLeft(ellipse, updatedJoint.Position.X);
            Canvas.SetTop(ellipse, updatedJoint.Position.Y);
        }


        private float ScaleVector(int length, float position)
        {
            float value = (((((float)length) / 1f) / 2f) * position) + (length / 2);
            if (value > length)
            {
                return (float)length;
            }
            if (value < 0f)
            {
                return 0f;
            }
            string line = Convert.ToString(value);
            string path = @"C:\KinectSavedMovements\FirstMovement.txt";
            if (!File.Exists(path))
            {
                string createText = "" + Environment.NewLine;
                File.WriteAllText(path, createText);
            }

            File.AppendAllText(path, line);

            string readText = File.ReadAllText(path);


            return value;
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
