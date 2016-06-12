using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using System.IO;

namespace KinectSaveModel
{
    class AmazonDownloader
    {
        static string bucketName = "smartgym-kinect";
        static string keyName;
        static IAmazonS3 client;

        public void DownloadS3Object(string path)
        {
            keyName = "Preview/FirstMovement_2.txt";
            client = new AmazonS3Client(Amazon.RegionEndpoint.EUCentral1);
            GetObjectRequest request = new GetObjectRequest();
            request.BucketName = bucketName;
            request.Key = keyName;
            GetObjectResponse response = client.GetObject(request);
            response.WriteResponseStreamToFile(path);
        }
    }
}
