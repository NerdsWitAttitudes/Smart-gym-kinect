using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectSaveModel
{
    class CalculateLiveMovement
    {
        // Calculates the average movement per joint in the queue with joints
        public List<Double[]> calculateAverageMovement(List<Double[]> AverageMovementJoint, List<Queue<Double[]>> lastMovements)
        {
            AverageMovementJoint = new List<Double[]>();
            List<List<Double[]>> doublesList2 = new List<List<Double[]>>();
            List<Double[]> doublesList = new List<Double[]>();
            Double[] minMaxJointCoordinate = new Double[2];
            Double coordinate;

            // For every joints
            for (int i = 0; i < lastMovements.Count; i++)
            {
                // For every saved queue list item. For example when you want to know the average
                // movement per joint in the past 10 frames
                for (int j = 0; j < lastMovements[i].Count; j++)
                {
                    // For every position, for example x, y and z position
                    doublesList = new List<Double[]>();
                    for (int k = 0; k < lastMovements[i].ElementAt(j).Length; k++)
                    {
                        // If it is the first frame for the joint, the position k doesn't exist yet, so to create it
                        if (j == 0)
                        {
                            minMaxJointCoordinate = new Double[2] { 0, 0 };
                            minMaxJointCoordinate[1] = lastMovements[i].ElementAt(j)[k];
                            doublesList.Add(minMaxJointCoordinate);
                        }
                        // If it is the third or higher frame for the joint
                        else if (j > 0)
                        {
                            coordinate = lastMovements[i].ElementAt(j)[k];
                            // If the first item in the list is 0, it compares if the max is lower than the min,
                            // if it is so, it switches the max and the mix, this way the min is always the first,
                            // and the max is always the second
                            if (doublesList2[i][k][0] == 0)
                            {
                                if (coordinate > doublesList2[i][k][1])
                                {
                                    doublesList2[i][k][0] = doublesList2[i][k][1];
                                    doublesList2[i][k][1] = coordinate;
                                }
                                else
                                {
                                    doublesList2[i][k][0] = coordinate;
                                }
                            }
                            // if the first item in the list isn't 0.
                            else if (doublesList2[i][k][0] != 0)
                            {
                                // Check if this coordinate is bigger than the current maximum
                                if (coordinate > doublesList2[i][k][1])
                                {
                                    doublesList2[i][k][1] = coordinate;
                                }
                                // Check if this coordinate is smaller than the current minimum
                                else if (coordinate < doublesList2[i][k][0])
                                {
                                    doublesList2[i][k][0] = coordinate;
                                }
                            }
                        }
                    }
                    if (j == 0)
                    {
                        doublesList2.Add(doublesList);
                    }
                }
            }


            doublesList = new List<Double[]>();
            // Now for each list item, calculate for every position (x, y, z) the average movement. You do this by
            // subtracting the minimum value from the maximum value. This way you get the difference.
            for (int i = 0; i < doublesList2.Count; i++)
            {
                Double[] xyzAverage = new Double[3];
                for (int j = 0; j < doublesList2[i].Count; j++)
                {
                    xyzAverage[j] = doublesList2[i][j][1] - doublesList2[i][j][0];
                }
                AverageMovementJoint.Add(xyzAverage);
            }
            return AverageMovementJoint;
        }
    }
}
