using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace KinectSaveModel
{
    public class readFile
    {
        private List<Row> fileList = new List<Row>();
        private static String[] joints = new String[]{"ShoulderCenter", "ShoulderRight", "ShoulderLeft", "ElbowRight", "ElbowLeft", "WristRight", "WristLeft"};
        public calculatePreviewMovement averages;

        public readFile(String path)
        {
            int counter = 0;
            string line;

            // Get the file and read line by line
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            // While there is a next line, read it
            while ((line = file.ReadLine()) != null)
            {
                // Split the line by the '@' sign, and than the part in front of the @ split is by the ','
                string[] splittedLine = line.Split('@');
                string[] splittedLinePart1 = splittedLine[0].Split(',');

                // Get the frame number and parse it into a Integer. Also Parses the datetime to DateTime
                int frameNumber = Int32.Parse(splittedLinePart1[0]);
                String dateTime = splittedLinePart1[1].Trim(new Char[] { '[', ']' });
                DateTime dateTime2 = DateTime.ParseExact(splittedLinePart1[1].Trim(new Char[] { '[', ']' }), "yyyy:MM:dd HH/mm/ss/FFFF", null);
                
                // Get the vectorlist of the coordinates of all the joints
                List<Vector3D> vectorlist = new List<Vector3D>();
                String replaceVectors = splittedLine[1].Replace("],[","]:[");
                String[] vectors = replaceVectors.Split(':');
                for (int i = 0; i < vectors.Length; i++)
                {
                    String[] vector = vectors[i].Trim(new Char[] {'[', ']'}).Split(',');
                    Vector3D vector3D = new Vector3D(Double.Parse(vector[0]), Double.Parse(vector[1]), Double.Parse(vector[2]));
                    vectorlist.Add(vector3D);
                }

                // Place the framenumber, datetime and vectorlist in a new Class, add this class to the vectorlist
                fileList.Add(new Row(frameNumber, dateTime2, vectorlist));
                counter++;
            }
            file.Close();
            averages = new calculatePreviewMovement(fileList, joints);
        }

        // Gets the average movement per joint
        public List<Double[]> getAverages()
        {
            return averages.maxMinJointTotal;
        }

        // Gets all the saved joints
        public String[] getJoints()
        {
            return joints;
        }

        
    }
}
