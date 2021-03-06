﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using System.IO;

namespace KinectPersonalTrainer
{
    public class AmazonUploader
    {
        static string bucketName = "smartgym-kinect";
        static string keyName;
        static string filePath;
        static IAmazonS3 client;

        public void sendToS3(string localFilePath, string fileNameInS3)
        {
            DateTime date = DateTime.Now;
            String dateString = "" + date;
            dateString = dateString.Replace('/', '-');
            dateString = dateString.Replace(' ', '_');
            dateString = dateString.Replace(':', '-');
            keyName = "users/"+Environment.MachineName+"/"+dateString + "/"+fileNameInS3;
            filePath = localFilePath;
            using (client = new AmazonS3Client(Amazon.RegionEndpoint.EUCentral1))
            {
                writeFile();
            }
        }

        static void writeFile()
        {
            try
            {
                PutObjectRequest putRequest2 = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    FilePath = filePath,
                    ContentType = "text/plain"
                };
                PutObjectResponse response2 = client.PutObject(putRequest2);

            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                System.Console.WriteLine("Error occurred. Message:'{0}' when writing an object",amazonS3Exception.Message);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Error: "+e);
            }
        }
    }
}
