using FlickerBatch_AlbumRetriever.ImageData;
using FlickrNet;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlickerBatch_AlbumRetriever
{
    class MainClass
    {
        static bool saveFlickrData = false;
        static bool saveLocalData = true;
        static void Main(string[] args)
        {

            DatabaseHelper.InitiallizeDataStructure();
            if (saveFlickrData)
            {

                Console.WriteLine("Authenticating With Flicker");
                Dictionary<string, string> auth_data = DatabaseHelper.loadConfigData("AUTH");
                FlickerHelper.Flickr_Auth(auth_data);
                DatabaseHelper.saveConfigData("AUTH", auth_data);
            }
            if (saveFlickrData)
            {
                Console.WriteLine("Download Photo Info from Flicker");

                List<FlickrAlbumData> psList = FlickerHelper.getAllAlbums();
                foreach (FlickrAlbumData ps in psList)
                {
                    FlickerHelper.savePictures(ps);                    
                }
            }
            if (saveLocalData)
            {
                Dictionary<string, string> local_data = DatabaseHelper.loadConfigData("LOCAL");
                FilesystemHelper.getFileList(local_data["basePath"]);
            }
            Console.ReadLine();

         }



    }
}
