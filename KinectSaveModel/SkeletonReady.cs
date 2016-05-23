using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using System.IO;
using System.Diagnostics;
using System.Windows.Media.Media3D;

namespace KinectSaveModel
{
    class SkeletonReady
    {
        private int frameNumber = 0;
        Skeleton[] skeleton;
        private string path = @"C:\KinectSavedMovements\FirstMovement.txt";

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

                    writeToFile(vectors);
                }
            }
        }

        public void writeToFile(List<Vector3D> vectorList)
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

        public String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyy:MM:dd HH/mm/ss/ffff");
        }
    }
}
