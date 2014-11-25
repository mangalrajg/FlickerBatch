using baseLibrary.DBInterface;
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
        static String saveFlickrData = "FALSE";
        static String saveLocalData = "FALSE";
        static String join = "FALSE";
        static void Main(string[] args)
        {

            DatabaseHelper.InitiallizeDataStructure();
            Dictionary<string, string> config = DatabaseHelper.loadMasterConfigData("BATCH_CONFIG");

            if(config.TryGetValue("SaveFlickerData", out saveFlickrData) && saveFlickrData == "TRUE" )
            {

                Dictionary<string, string> auth_data = DatabaseHelper.loadMasterConfigData("AUTH");
                Console.WriteLine("Authenticating With Flicker");
                FlickerHelper.Flickr_Auth(auth_data);
                Console.WriteLine("Saving Auth Info..");
                DatabaseHelper.saveConfigData("AUTH", auth_data);

                Console.WriteLine("Download Photo Info from Flicker");
                List<FlickrAlbumData> psList = FlickerHelper.getAllAlbums();
                foreach (FlickrAlbumData ps in psList)
                {
                    FlickerHelper.savePictures(ps);                    
                }
                FlickerHelper.WaitForAllThreads();


            }
            if (config.TryGetValue("SaveLocalData", out saveLocalData) && saveLocalData == "TRUE")
            {
                Dictionary<string, string> local_data = DatabaseHelper.loadMasterConfigData("LOCAL");
                FilesystemHelper.getFileList(local_data["LocalBasePath"]);
                DatabaseHelper.removePrefix(local_data["LocalBasePath"]);
            }

            if (config.TryGetValue("Join", out join) && join == "TRUE")
            {
                DatabaseHelper.Join();
            }
            Console.WriteLine("=====================       DONE       =====================");
            Console.ReadLine();

         }



    }
}
