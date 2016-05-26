using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.IO;
using System.Diagnostics;
using System.Windows.Media.Media3D;
using System.Globalization;
using System.Windows;
using System.Windows.Media.Imaging;

namespace KinectSaveModel
{
    class SkeletonReady
    {
        private int frameNumber = 0;
        Skeleton[] skeleton;
        private string path = @"C:\KinectSavedMovements\FirstMovement.txt";
        private String ssPath = @"C:\KinectSavedMovements\screenshots\FirstMovement";

        public void skeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
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
                    vectors.Add(new Vector3D(coordinatePos(640, currentSkeleton.Joints[JointType.ShoulderCenter].Position.X), coordinatePos(480, currentSkeleton.Joints[JointType.ShoulderCenter].Position.Y), currentSkeleton.Joints[JointType.ShoulderCenter].Position.Z));
                    vectors.Add(new Vector3D(coordinatePos(640, currentSkeleton.Joints[JointType.ShoulderRight].Position.X), coordinatePos(480, currentSkeleton.Joints[JointType.ShoulderRight].Position.Y), currentSkeleton.Joints[JointType.ShoulderRight].Position.Z));
                    vectors.Add(new Vector3D(coordinatePos(640, currentSkeleton.Joints[JointType.ShoulderLeft].Position.X), coordinatePos(480, currentSkeleton.Joints[JointType.ShoulderLeft].Position.Y), currentSkeleton.Joints[JointType.ShoulderLeft].Position.Z));
                    vectors.Add(new Vector3D(coordinatePos(640, currentSkeleton.Joints[JointType.ElbowRight].Position.X), coordinatePos(480, currentSkeleton.Joints[JointType.ElbowRight].Position.Y), currentSkeleton.Joints[JointType.ElbowRight].Position.Z));
                    vectors.Add(new Vector3D(coordinatePos(640, currentSkeleton.Joints[JointType.ElbowLeft].Position.X), coordinatePos(480, currentSkeleton.Joints[JointType.ElbowLeft].Position.Y), currentSkeleton.Joints[JointType.ElbowLeft].Position.Z));
                    vectors.Add(new Vector3D(coordinatePos(640, currentSkeleton.Joints[JointType.WristRight].Position.X), coordinatePos(480, currentSkeleton.Joints[JointType.WristRight].Position.Y), currentSkeleton.Joints[JointType.WristRight].Position.Z));
                    vectors.Add(new Vector3D(coordinatePos(640, currentSkeleton.Joints[JointType.WristLeft].Position.X), coordinatePos(480, currentSkeleton.Joints[JointType.WristLeft].Position.Y), currentSkeleton.Joints[JointType.WristLeft].Position.Z));

                    screenshotSave();
                    writeToFile(vectors);
                    calculate(currentSkeleton.Joints[JointType.ShoulderRight], currentSkeleton.Joints[JointType.ElbowRight], currentSkeleton.Joints[JointType.WristRight]);
                }
            }
        }

        private void screenshotSave()
        {
            // create a png bitmap encoder which knows how to save a .png file
            BitmapEncoder encoder = new PngBitmapEncoder();

            // create frame from the writable bitmap and add to encoder
            encoder.Frames.Add(BitmapFrame.Create(MainWindow.colorBitmap));

            string time = System.DateTime.Now.ToString("hh'-'mm'-'ss", CultureInfo.CurrentUICulture.DateTimeFormat);
            if (frameNumber == 0)
            {
                if (!Directory.Exists(@"C:\KinectSavedMovements\screenshots\FirstMovement"))
                {
                    Directory.CreateDirectory(@"C:\KinectSavedMovements\screenshots\FirstMovement");
                    ssPath = @"C:\KinectSavedMovements\screenshots\FirstMovement";
                }
                if (Directory.Exists(@"C:\KinectSavedMovements\screenshots\FirstMovement"))
                {
                    Directory.CreateDirectory(@"C:\KinectSavedMovements\screenshots\FirstMovement-"+time);
                    ssPath = @"C:\KinectSavedMovements\screenshots\FirstMovement-" + time;
                }
            }

            string ssFullPath = Path.Combine(ssPath, "frame-" + frameNumber + ".png");

            // write the new file to disk
            try
            {
                using (FileStream fs = new FileStream(ssFullPath, FileMode.Create))
                {
                    encoder.Save(fs);
                }

                //this.statusBarText.Text = string.Format(CultureInfo.InvariantCulture, "{0} {1}", Properties.Resources.ScreenshotWriteSuccess, path);
            }
            catch (IOException)
            {
                //this.statusBarText.Text = string.Format(CultureInfo.InvariantCulture, "{0} {1}", Properties.Resources.ScreenshotWriteFailed, path);
            }
        }

        private void calculate(Joint rightShoulder, Joint rightElbow, Joint rightWrist)
        {
            float rightShoulderCoordx = coordinatePos(640, rightShoulder.Position.X);
            float rightElbowCoordx = coordinatePos(640, rightElbow.Position.X);
            float rightShoulderCoordy = coordinatePos(480, rightShoulder.Position.X);
            float rightElbowCoordy = coordinatePos(480, rightElbow.Position.X);
            double lengthX = Math.Pow((rightElbowCoordx - rightShoulderCoordx),2);
            double lengthY = Math.Pow((rightElbowCoordy-rightShoulderCoordy),2);
            double lengthUpper = Math.Sqrt((lengthX+lengthY));
            double asin = Math.Sqrt(lengthX) / lengthUpper;
            //asin = Math.Acos(asin);
            //MainWindow.main.statusBarText.Text = ""+lengthY;
            MainWindow.main.statusBarText.Text = ""+Math.Sqrt(lengthX)+"/"+lengthUpper+"="+asin+" = asin("+(Math.Acos(asin)*(180/Math.PI))+"";
        }

        private void writeToFile(List<Vector3D> vectorList)
        {
            string line = "";
            String timeStamp = GetTimestamp(DateTime.Now);
            String frame = "" + frameNumber;
            line += frame + ",[" + timeStamp + "]@";
            for (int i = 0; i < vectorList.Count; i++)
            {
                if (i != (vectorList.Count - 1))
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

        private float coordinatePos(int length, float xpos)
        {
            float value = (((((float)length) / 1f) / 2f) * xpos) + (length / 2);
            if (value > length)
            {
                return (float)length;
            }
            if (value < 0f)
            {
                return 0f;
            }
            return value;
        }

        private String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyy:MM:dd HH/mm/ss/ffff");
        }
    }
}
