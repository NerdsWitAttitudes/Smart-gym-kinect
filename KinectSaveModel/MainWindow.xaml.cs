using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Linq;
using System.IO;

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
                    Ellipse head = null;
                    SetEllipsePosition(head, currentSkeleton.Joints[JointType.Head]);
                    Ellipse leftHand = null;
                    SetEllipsePosition(leftHand, currentSkeleton.Joints[JointType.HandLeft]);
                    Ellipse leftElbow = null;
                    SetEllipsePosition(leftElbow, currentSkeleton.Joints[JointType.ElbowLeft]);
                    Ellipse leftShoulder = null;
                    SetEllipsePosition(leftShoulder, currentSkeleton.Joints[JointType.ShoulderLeft]);
                    Ellipse rightHand = null;
                    SetEllipsePosition(rightHand, currentSkeleton.Joints[JointType.HandRight]);
                    Ellipse rightElbow = null;
                    SetEllipsePosition(rightElbow, currentSkeleton.Joints[JointType.ElbowRight]);
                    Ellipse rightShoulder = null;
                    SetEllipsePosition(rightShoulder, currentSkeleton.Joints[JointType.ShoulderRight]);
                    Ellipse centerShoulders = null;
                    SetEllipsePosition(centerShoulders, currentSkeleton.Joints[JointType.ShoulderCenter]);

                }
            }
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
            sensor.ColorFrameReady += videoFrameReady;
            sensor.Start();
        }


        void videoFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            bool dataReceived = false;
            using (ColorImageFrame ColorImageFrame = e.OpenColorImageFrame())
            {
                if (ColorImageFrame == null)
                {
                    // Moet nog foutmelding voor geschreven worden
                }
                else
                {
                    pixelData = new byte[ColorImageFrame.PixelDataLength];
                    ColorImageFrame.CopyPixelDataTo(pixelData);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                BitmapSource source = BitmapSource.Create(640, 480, 96, 96,
                        PixelFormats.Bgr32, null, pixelData, 640 * 4);
                
            }
        }
    }
}
