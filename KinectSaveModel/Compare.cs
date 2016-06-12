using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace KinectSaveModel
{
    class Compare
    {
        List<Queue<Double[]>> lastMovements = new List<Queue<Double[]>>();
        public List<Double[]> AverageMovementJoint;
        private List<DateTime> errorListDateTimeEnd = new List<DateTime>();
        private List<String> errorListJoints = new List<String>();

        public void CompareMovement(List<Vector3D> vectorList)
        {
            createList(vectorList);
            CompareLive();
        }

        private void createList(List<Vector3D> vectorList)
        {
            Queue<Double[]> movementQueue;
            // For each joint
            for (int i = 0; i < vectorList.Count; i++)
            {
                // Check if there is already a queue item. If there isn't
                if (lastMovements.ElementAtOrDefault(i) == null)
                {
                    // Create a new Queue and add the x,y and z values to the queue
                    movementQueue = new Queue<Double[]>();
                    Double[] xyzJoint = new Double[3];
                    xyzJoint[0] = vectorList[i].X;
                    xyzJoint[1] = vectorList[i].Y;
                    xyzJoint[2] = vectorList[i].Z;
                    movementQueue.Enqueue(xyzJoint);
                    lastMovements.Add(movementQueue);
                }
                // If there is
                else if (lastMovements.ElementAtOrDefault(i) != null)
                {
                    // Get the queue, add the x,y and z values to the queue. Check if the queue has more than
                    // 25 items. If so, delete one item from the queue and save the new list. 
                    // If not, just add and save the new list
                    movementQueue = lastMovements[i];
                    Double[] xyzJoint = new Double[3];
                    xyzJoint[0] = vectorList[i].X;
                    xyzJoint[1] = vectorList[i].Y;
                    xyzJoint[2] = vectorList[i].Z;
                    movementQueue.Enqueue(xyzJoint);
                    if (movementQueue.Count > 25)
                    {
                        movementQueue.Dequeue();
                    }
                    lastMovements[i] = movementQueue;
                }
            }
        }

        private void CompareLive()
        {
            calculateAverageMovement();
            ComparePreview();
        }

        private void ComparePreview()
        {
            // For every joint
            for (int joint = 0; joint < MainWindow.main.Joints.Length; joint++ )
            {
                // For every xyz position
                for (int xyz = 0; xyz < 3; xyz++)
                {
                    // if the position is not equal to z. Because the scale used for the z is different from 
                    // the scale used for x and y
                    if (xyz != 2)
                    {
                        // If user moves the joint more than supposed to
                        if ((AverageMovementJoint[joint][xyz] > (MainWindow.main.Averages[joint][xyz]) + 10))
                        {
                            addToErrorList(joint);                            
                        }
                    }
                    // If the position is equal to z.
                    else if (xyz == 2)
                    {
                        if ((AverageMovementJoint[joint][xyz] > (MainWindow.main.Averages[joint][xyz])+0.05))
                        {
                            addToErrorList(joint);   
                        }
                    }
                }
            }
            showErrorMessage();
        }

        private void showErrorMessage()
        {
            String text = "";
            // For every joint in the error list 
            for (int joint = 0; joint < errorListJoints.Count; joint++)
            {
                // If the ending time is yet to come
                if (errorListDateTimeEnd[joint] > DateTime.Now)
                {
                    // Add a text saying to keep the joint steady
                    text += "Keep " + errorListJoints[joint] + " steady, \n";
                }
                // If the ending time is past
                else if (errorListDateTimeEnd[joint] < DateTime.Now)
                {
                    // Remove the joint and datetime from the error list
                    errorListJoints.RemoveAt(joint);
                    errorListDateTimeEnd.RemoveAt(joint);
                    joint--;
                }
            }
            // Show the joints to keep steady in the message box
            MainWindow.main.statusBarText.Text = string.Format(text);
        }

        private void addToErrorList(int joint)
        {
            // If the joint is already in the errorlist
            if (errorListJoints.IndexOf(MainWindow.main.Joints[joint]) != -1)
            {
                // Set the ending time of this error message to 3 seconds in the future
                errorListDateTimeEnd[errorListJoints.IndexOf(MainWindow.main.Joints[joint])] = DateTime.Now.AddSeconds(3);
            }
            // If the joint is not in the error list already
            else
            {
                // Add this joint and the ending time (3 seconds in the future) to the lists
                errorListJoints.Add(MainWindow.main.Joints[joint]);
                errorListDateTimeEnd.Add(DateTime.Now.AddSeconds(3));
            }
        }

        // Calculates the average movement per joint in the queue with joints
        private void calculateAverageMovement()
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
                            minMaxJointCoordinate = new Double[2]{0,0};
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
                for(int j = 0 ; j < doublesList2[i].Count; j ++){
                    xyzAverage[j] = doublesList2[i][j][1] - doublesList2[i][j][0];
                }
                AverageMovementJoint.Add(xyzAverage);
            }
        }
    }
}
