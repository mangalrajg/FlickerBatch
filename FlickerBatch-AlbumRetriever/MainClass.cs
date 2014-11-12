using FlickerBatch_AlbumRetriever.Model;
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
        static bool saveLocalData = false;
        static bool join = true;
        static void Main(string[] args)
        {

            DatabaseHelper.InitiallizeDataStructure();
            if (saveFlickrData)
            {

                Dictionary<string, string> auth_data = DatabaseHelper.loadMasterConfigData("AUTH");
                Console.WriteLine("Authenticating With Flicker");
                FlickerHelper.Flickr_Auth(auth_data);
                Console.WriteLine("Saving Auth Info..");
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
                FlickerHelper.WaitForAllThreads();


            }
            if (saveLocalData)
            {
                Dictionary<string, string> local_data = DatabaseHelper.loadMasterConfigData("LOCAL");
                FilesystemHelper.getFileList(local_data["basePath"]);
            }

            if(join)
            {
                DatabaseHelper.Join();
            }
            Console.WriteLine("=====================       DONE       =====================");
            Console.ReadLine();

         }



    }
}
