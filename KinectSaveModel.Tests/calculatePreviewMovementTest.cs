using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace KinectSaveModel.Tests
{
    [TestClass]
    public class calculatePreviewMovementTest
    {
        [TestMethod]
        public void getCount()
        {
            MainWindow main = new MainWindow();
            ReadFile rf = new ReadFile(main.previewPath);
            CalculatePreviewMovement cm = rf.averages;
            List<Double[]> list = cm.maxMinJointTotal;
            Assert.AreEqual(7,list.Count);
        }

        [TestMethod]
        public void getOutput()
        {
            MainWindow main = new MainWindow();
            ReadFile rf = new ReadFile(main.previewPath);
            CalculatePreviewMovement cm = rf.averages;
            Double[] maxMinX = { 350, 550 };
            Double[] maxMinY = { 400, 450 };
            Double[] maxMinZ = { 0.9, 1.35 };
            Vector3D vector = new Vector3D(355, 390, 1.6);
            List<Double[]> maxMinJoint = new List<Double[]>();
            maxMinJoint.Add(maxMinX);
            maxMinJoint.Add(maxMinY);
            maxMinJoint.Add(maxMinZ);
            List<Double[]> maxMinJoint2 = new List<Double[]>();
            maxMinJoint2.Add(new Double[] { 350, 550 });
            maxMinJoint2.Add(new Double[] { 390, 450 });
            maxMinJoint2.Add(new Double[] { 0.9, 1.6 });
            maxMinJoint = cm.getMaxMinJoint(vector, maxMinJoint);
            for (int i = 0; i < maxMinJoint.Count; i++)
            {
                for (int j = 0; j < maxMinJoint[i].Length; j++)
                {
                    Assert.AreEqual(maxMinJoint[i][j], maxMinJoint2[i][j]);
                }
            }
                
        }

        [TestMethod]
        public void listLength()
        {
            MainWindow main = new MainWindow();
            ReadFile rf = new ReadFile(main.previewPath);
            CalculatePreviewMovement cm = rf.averages;
            int jointMovementListLength = cm.maxMinJointTotal.Count;
            Assert.AreEqual(7, jointMovementListLength);
        }
        
    }
}
