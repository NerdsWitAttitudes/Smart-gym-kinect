using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace KinectSaveModel
{
    class calculatePreviewMovement
    {
        public List<Double[]> maxMinJointTotal;
        private static String[] joints;
        private List<Row> fileList;

        public calculatePreviewMovement(List<Row> fileList, String[] jointsList)
        {
            joints = jointsList;
            this.fileList = fileList;
            getMovement();
        }

        private void getMovement()
        {
            // To get the averagemovement of the wrists, first create two Arrays of double values
            double[] averageWristRightXY = { 0, 0 };
            double[] averageWristLeftXY = { 0, 0 };
            for (int i = 0; i < fileList.Count; i++)
            {
                // Place the Listitem in a new List, get the X and Y values and add the values of the x and y positions
                // to the Double list
                List<Vector3D> vectorList = new List<Vector3D>();
                vectorList = fileList[i].getVectorList();
                averageWristRightXY[0] = averageWristRightXY[0] + fileList[i].getVectorList()[Array.IndexOf(joints, "WristRight")].X;
                averageWristRightXY[1] = averageWristRightXY[1] + fileList[i].getVectorList()[Array.IndexOf(joints, "WristRight")].Y;
                averageWristLeftXY[0] = averageWristLeftXY[0] + fileList[i].getVectorList()[Array.IndexOf(joints, "WristRight")].X;
                averageWristLeftXY[1] = averageWristLeftXY[1] + fileList[i].getVectorList()[Array.IndexOf(joints, "WristRight")].Y;
            }
            // Get the average position by dividing the coordinates by the number of items added
            averageWristRightXY[0] = averageWristRightXY[0] / fileList.Count;
            averageWristRightXY[1] = averageWristRightXY[1] / fileList.Count;
            averageWristLeftXY[0] = averageWristLeftXY[0] / fileList.Count;
            averageWristLeftXY[1] = averageWristLeftXY[1] / fileList.Count;
            // Get the average Y coordinates by summing up the left and right average y coordinates and deviding it by two
            // since there are two items summed up

            double averageWristsY = ((averageWristRightXY[1] + averageWristLeftXY[1]) / 2);

            getReps(averageWristsY);
        }

        private void getReps(double averageWristsY)
        {
            bool workoutStarted = false;
            int repNum = 0;
            bool averageYPos = false;

            List<Double[]> maxMinListPerRepPerJoint = new List<Double[]>();
            List<List<Double[]>> maxMinListRep = new List<List<Double[]>>();
            List<List<List<Double[]>>> maxMinListRepTotal = new List<List<List<Double[]>>>();
            Double[] positionX = { 0, 0 };
            Double[] positionY = { 0, 0 };
            Double[] positionZ = { 0, 0 };

            // Loop through the list again, now to determine the number of reps and the average of movement of the joints
            for (int i = 0; i < fileList.Count; i++)
            {
                // Get the y coordinates of the wrists of the frame you are looking into
                double WristRightY = fileList[i].getVectorList()[Array.IndexOf(joints, "WristRight")].Y;

                // Because you don't want to start counting and calculating when the skeleton is visible but only 
                // when the workout starts, you need to determine when the workout starts
                if (workoutStarted == false)
                {

                    // If the right wrist between the average y coordinates + 3 and average y coordinates - 10, the workout starts
                    if (WristRightY <= (Math.Round(averageWristsY) + 3) && WristRightY >= (Math.Round(averageWristsY) - 10))
                    {
                        workoutStarted = true;
                        averageYPos = true;
                    }
                }

                // Part of code which only fires when the workout has started
                else if (workoutStarted == true)
                {

                    // If the right wrist is close to the average y position. Soo when the wrist is at start position
                    if (averageYPos == true)
                    {

                        // If this isn't the first rep
                        if (repNum != 0)
                        {
                            // @Loop through all the joints saved, get the vector list for each joint,
                            // and check if the x, y and z position is bigger or smaller than the current
                            // min and max value in the list
                            for (int j = 0; j < fileList[i].getVectorList().Count; j++)
                            {
                                Vector3D vector = fileList[i].getVectorList()[j];
                                maxMinListPerRepPerJoint = maxMinListRep[j];
                                maxMinListPerRepPerJoint = getAverageMovement(vector, maxMinListPerRepPerJoint);
                                maxMinListRep[j] = maxMinListPerRepPerJoint;
                            }
                        }

                        // When the right wrist goes out of the average y coordinate position, the new rep starts
                        if (WristRightY >= (Math.Round(averageWristsY) + 3))
                        {
                            averageYPos = false;
                            repNum = repNum + 1;
                            maxMinListRepTotal.Add(maxMinListRep);
                            maxMinListRep = new List<List<double[]>>();

                            // @Loop through all the joints saved, get the vector list for each joint,
                            // and add these values to the list as min value, later on it checks if the second
                            // x, y or z value is bigger than the current min value, if it is it switches them
                            for (int j = 0; j < fileList[i].getVectorList().Count; j++)
                            {
                                positionX = new Double[2];
                                positionY = new Double[2];
                                positionZ = new Double[2];
                                maxMinListPerRepPerJoint = new List<Double[]>();
                                maxMinListPerRepPerJoint.Add(positionX);
                                maxMinListPerRepPerJoint.Add(positionY);
                                maxMinListPerRepPerJoint.Add(positionZ);
                                Vector3D vector = new Vector3D(fileList[i].getVectorList()[j].X, fileList[i].getVectorList()[j].Y, fileList[i].getVectorList()[j].Z);
                                maxMinListPerRepPerJoint = getAverageMovement(positionX, positionY, positionZ, vector, maxMinListPerRepPerJoint);
                                maxMinListRep.Add(maxMinListPerRepPerJoint);
                            }
                        }
                    }

                    // If the right wrist is outside the average position of the wrist, the rep is busy
                    else if (averageYPos == false)
                    {

                        // @Loop through all the joints saved, get the vector list for each joint,
                        // and check if the x, y and z position is bigger or smaller than the current
                        // min and max value in the list
                        for (int j = 0; j < fileList[i].getVectorList().Count; j++)
                        {
                            Vector3D vector = fileList[i].getVectorList()[j];
                            maxMinListPerRepPerJoint = maxMinListRep[j];
                            maxMinListPerRepPerJoint = getAverageMovement(vector, maxMinListPerRepPerJoint);
                            maxMinListRep[j] = maxMinListPerRepPerJoint;
                        }

                        // if the right wirst gets close to the average y position
                        if (Math.Round(WristRightY) <= (Math.Round(averageWristsY) + 3) && Math.Round(WristRightY) >= (Math.Round(averageWristsY) - 10))
                        {
                            averageYPos = true;
                        }
                    }
                }
            }
            // Now all the min and max values are added to a list per rep, these
            // are needed to calculate the average movement per joint
            calculateMovementPerJoint(maxMinListRepTotal);
        }


        // To calculate the average movement in the x, y or z position per joint
        private void calculateMovementPerJoint(List<List<List<Double[]>>> maxMinListRepTotal)
        {
            List<List<Double>> maxMinTotalJoints = new List<List<Double>>();
            List<Double> maxMinJoint = new List<Double>();

            // First add all the movement in the x, y or z position to a list
            Double AverageMovementJoint;
            for (int rep = 0; rep < maxMinListRepTotal.Count; rep++)
            {
                for (int joint = 0; joint < maxMinListRepTotal[rep].Count; joint++)
                {
                    maxMinJoint = new List<Double>();
                    for (int xyz = 0; xyz < maxMinListRepTotal[rep][joint].Count; xyz++)
                    {
                        AverageMovementJoint = maxMinListRepTotal[rep][joint][xyz][1] - maxMinListRepTotal[rep][joint][xyz][0];
                        maxMinJoint.Add(AverageMovementJoint);
                    }
                    maxMinTotalJoints.Add(maxMinJoint);
                }
            }

            getAverageMovementPerJoint(maxMinTotalJoints);
        }

        private void getAverageMovementPerJoint(List<List<Double>> maxMinTotalJoints){
            // At the moment if there are 7 reps and 7 joints, there are 7*7=49 items. Now we want only 7 and the average
            // movement per joint in the x, y and z position. These average movement is added to the list 'maxMinJointTotal'
            maxMinJointTotal = new List<Double[]>();
            Double[] xyzJoint = new Double[3];
            for (int joint = 0; joint < maxMinTotalJoints.Count; joint++)
            {
                if (maxMinJointTotal.ElementAtOrDefault(joint % joints.Length) == null)
                {
                    xyzJoint = new Double[3];
                    for (int j = 0; j < maxMinTotalJoints[joint].Count; j++)
                    {
                        xyzJoint[j] = maxMinTotalJoints[joint][j];
                    }
                    maxMinJointTotal.Add(xyzJoint);
                }
                else if (maxMinJointTotal.ElementAtOrDefault(joint % joints.Length) != null)
                {
                    xyzJoint = new Double[3];
                    for (int j = 0; j < maxMinTotalJoints[joint].Count; j++)
                    {
                        xyzJoint[j] = (maxMinJointTotal[joint % joints.Length][j] + maxMinTotalJoints[joint][j]) / 2;
                    }
                    maxMinJointTotal[joint % joints.Length] = xyzJoint;
                }
            }
        }

        private List<Double[]> getAverageMovement(Double[] maxMinX, Double[] maxMinY, Double[] maxMinZ, Vector3D coordinates, List<Double[]> maxMinJoint)
        {
            Double positionX = coordinates.X;
            Double positionY = coordinates.Y;
            Double positionZ = coordinates.Z;
            if (maxMinX[0] == 0)
            {
                maxMinX[0] = positionX;
                maxMinY[0] = positionY;
                maxMinZ[0] = positionZ;
            }
            maxMinJoint[0] = maxMinX;
            maxMinJoint[1] = maxMinY;
            maxMinJoint[2] = maxMinZ;

            return maxMinJoint;
        }

        private List<Double[]> getAverageMovement(Vector3D coordinates, List<Double[]> maxMinJoint)
        {
            Double[] xyz = { coordinates.X, coordinates.Y, coordinates.Z };
            // if the array with the max and min values is not fully filled
            if (maxMinJoint[0][1] == 0)
            {
                for (int position = 0; position < maxMinJoint.Count; position++)
                {
                    // if the min value is bigger than the value to add,
                    // set the max value to the min value, and add in the place of the min value the value to add
                    if (maxMinJoint[position][0] > xyz[position])
                    {
                        maxMinJoint[position][1] = maxMinJoint[position][0];
                        maxMinJoint[position][0] = xyz[position];
                    }
                    // add the value to the max
                    else
                    {
                        maxMinJoint[position][1] = xyz[position];
                    }
                }
            }
            // if the array with the max and min values is fully filled
            else
            {
                for (int position = 0; position < maxMinJoint.Count; position++)
                {
                    if (xyz[position] < maxMinJoint[position][0])
                    {
                        maxMinJoint[position][0] = xyz[position];
                    }
                    else if (xyz[position] > maxMinJoint[position][1])
                    {
                        maxMinJoint[position][1] = xyz[position];
                    }
                }
            }
            return maxMinJoint;
        }
    }
}
