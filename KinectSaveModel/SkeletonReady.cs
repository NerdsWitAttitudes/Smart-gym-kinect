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

namespace KinectPersonalTrainer
{
    class SkeletonReady
    {
        private int frameNumber = 0;
        Skeleton[] skeleton;
        public string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), previewPath;
        public String movementName = "FirstMovement", previewName = "FirstMovement-2", folder;
        private Compare compare = new Compare();

        // The method for getting the skeleton frame
        public void skeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            bool dataReceived = false;
            using (SkeletonFrame SkeletonFrame = e.OpenSkeletonFrame())
            {
                if (SkeletonFrame == null)
                {
                    Debug.WriteLine("frame == null");
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
                // When there actually is a person (skeleton frame) in front of the kinect, too prevent null values in the file
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
                    frameNumber = frameNumber + 1;
                    compare.CompareMovement(vectors);
                    writeToFile(vectors);
                }
            }
        }

        // To set the paths to save the screenshots & the coordinates
        // This also creates the directory if it doesn't exists
        public void setPaths()
        {
            path = Path.Combine(path, movementName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            folder = path;
            previewPath = Path.Combine(path, previewName + ".txt");
            path = Path.Combine(path, movementName + ".txt");
        }

        // The method to write the coordinates to the text file
        private void writeToFile(List<Vector3D> vectorList)
        {
            string line = "";
            String timeStamp = GetTimestamp(DateTime.Now);
            line += "" + frameNumber + ",[" + timeStamp + "]@";
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
                File.AppendAllText(path, Environment.NewLine + line);
            }
            frameNumber = frameNumber + 1;
        }

        // Get the coordinates on the screen
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

        // Method to get the current date and time
        private String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyy:MM:dd HH/mm/ss/ffff");
        }
    }
}
