using baseLibrary.DBInterface;
using baseLibrary.LocalInterface;
using baseLibrary.Model;
using baseLibrary.RemoteInterface;
using System;
using System.Collections.Generic;

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
            Dictionary<string, string> config = DatabaseHelper.LoadMasterConfigData("BATCH_CONFIG");

            if(config.TryGetValue("SaveFlickerData", out saveFlickrData) && saveFlickrData == "TRUE" )
            {

                Dictionary<string, string> auth_data = DatabaseHelper.LoadMasterConfigData("AUTH");
                Console.WriteLine("Authenticating With Flicker");
                FlickerHelper.Flickr_Auth(auth_data);
                Console.WriteLine("Saving Auth Info..");
                DatabaseHelper.SaveConfigData("AUTH", auth_data);

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
                Dictionary<string, string> local_data = DatabaseHelper.LoadMasterConfigData("LOCAL");
                FilesystemHelper.getFileList(local_data["LocalBasePath"]);
                //DatabaseHelper.RemovePrefix(local_data["LocalBasePath"]);
            }

            if (config.TryGetValue("SaveJoinData", out join) && join == "TRUE")
            {
                DatabaseHelper.SaveJoinData();
            }
            Console.WriteLine("=====================       DONE       =====================");
            Console.ReadLine();

         }



    }
}
