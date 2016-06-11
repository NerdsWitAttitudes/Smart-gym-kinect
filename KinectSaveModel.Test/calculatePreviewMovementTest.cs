using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace KinectSaveModel.Test
{
    [TestClass]
    class calculatePreviewMovementTest
    {
        public void ShouldAddReturnNineWhenPassFiveAndFour()
        {
            //List<Row> fileList, String[] jointsList
            List<Vector3D> vectorlist = new List<Vector3D>();
            Row rij = new Row(DateTime.Now, );
            List<String> rowList = new List<String>();
            calculatePreviewMovement compare = new calculatePreviewMovement();
            //private List<Double[]> getAverageMovement(Double[] maxMinX, Double[] maxMinY, Double[] maxMinZ, Vector3D coordinates, List<Double[]> maxMinJoint)
        }
    }
}
