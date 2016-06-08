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

        public void CompareMovement(List<Vector3D> vectorList)
        {
            createList(vectorList);
            CompareLive();
        }

        private void createList(List<Vector3D> vectorList)
        {
            Queue<Double[]> movementQueue;
            for (int i = 0; i < vectorList.Count; i++)
            {
                if (lastMovements.ElementAtOrDefault(i) == null)
                {
                    movementQueue = new Queue<Double[]>();
                    Double[] xyzJoint = new Double[3];
                    xyzJoint[0] = vectorList[i].X;
                    xyzJoint[1] = vectorList[i].Y;
                    xyzJoint[2] = vectorList[i].Z;
                    movementQueue.Enqueue(xyzJoint);
                    lastMovements.Add(movementQueue);
                }
                else if (lastMovements.ElementAtOrDefault(i) != null)
                {
                    movementQueue = lastMovements[i];
                    Double[] xyzJoint = new Double[3];
                    xyzJoint[0] = vectorList[i].X;
                    xyzJoint[1] = vectorList[i].Y;
                    xyzJoint[2] = vectorList[i].Z;
                    movementQueue.Enqueue(xyzJoint);
                    if (movementQueue.Count > 10)
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
            for (int i = 0; i < MainWindow.main.Joints.Length; i++ )
            {
                for (int j = 0; j < 3; j++)
                {
                    if ((AverageMovementJoint[i][j] > (MainWindow.main.Averages[i][j]+100)) || (AverageMovementJoint[i][j] < (MainWindow.main.Averages[i][j] -100)))
                    {
                        MainWindow.main.statusBarText.Text = string.Format("Keep Joint steady");
                    }
                }
            }
        }

        private void calculateAverageMovement()
        {
            AverageMovementJoint = new List<Double[]>();
            List<List<Double[]>> doublesList2 = new List<List<Double[]>>();
            List<Double[]> doublesList = new List<Double[]>();
            Double[] minMaxJointCoordinate = new Double[2];
            Double coordinate;
            for (int i = 0; i < lastMovements.Count; i++)
            {
                for (int j = 0; j < lastMovements[i].Count; j++)
                {
                    doublesList = new List<Double[]>();
                    for (int k = 0; k < lastMovements[i].ElementAt(j).Length; k++)
                    {
                        if (j == 0)
                        {
                            minMaxJointCoordinate = new Double[2]{0,0};
                            minMaxJointCoordinate[1] = lastMovements[i].ElementAt(j)[k];
                            doublesList.Add(minMaxJointCoordinate);
                        }
                        else if (j == 1)
                        {
                            minMaxJointCoordinate = doublesList2[i][k];
                            if (lastMovements[i].ElementAt(j)[k] > minMaxJointCoordinate[1])
                            {
                                minMaxJointCoordinate[0] = minMaxJointCoordinate[1];
                                minMaxJointCoordinate[1] = lastMovements[i].ElementAt(j)[k];
                                doublesList2[i][k] = minMaxJointCoordinate;
                            }
                        }
                        else if (j > 1)
                        {
                            coordinate = lastMovements[i].ElementAt(j)[k];
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
                            else if (doublesList2[i][k][0] != 0)
                            {
                                if (coordinate > doublesList2[i][k][1])
                                {
                                    doublesList2[i][k][1] = coordinate;
                                }
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
            for (int i = 0; i < doublesList2.Count; i++)
            {
                Double[] xyzAverage = new Double[3];
                for(int j = 0 ; j < doublesList2[i].Count; j ++){
                    xyzAverage[j] = doublesList2[i][j][1] - doublesList2[i][j][0];
                }
                AverageMovementJoint.Add(xyzAverage);
            }

            for (int i = 0; i < AverageMovementJoint.Count; i++)
            {
                //System.Console.WriteLine(doublesList.Count);
                //System.Console.WriteLine(MainWindow.main.Joints.Length);
                System.Console.WriteLine("---------------------------//////// Joint: "+MainWindow.main.Joints[i]+" ////////----------------------------");
                System.Console.WriteLine(AverageMovementJoint[i][0]);
                System.Console.WriteLine(AverageMovementJoint[i][1]);
                System.Console.WriteLine(AverageMovementJoint[i][2]);
            }
        }
    }
}
