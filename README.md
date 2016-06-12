# Smart-gym-kinect
## Kinect SDK 1.8
This code is based on Kinect sdk v1.8. You can download kinect sdk v1.8 on: https://www.microsoft.com/en-us/download/details.aspx?id=40278

##Visual Studio
This code is developed in Visual Studio 2013.
This code is developed in the language C#

#Amazon S3
Kinect downloads the preview file from the S3 on Amazon. You can change the download and upload location in the files `AmazonUpload` and `AmazonDownload`

#Setup Amazon S3
In App.Config you have to add the following code:
`<appSettings>
    <add key="AWSProfileName" value="profile1"/>
    <add key="AWSAccessKey" value="********************"/>
    <add key="AWSSecretKey" value="****************************************"/>
  </appSettings>`
