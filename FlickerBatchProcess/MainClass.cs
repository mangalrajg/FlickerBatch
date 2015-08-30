using baseLibrary.DBInterface;
using baseLibrary.LocalInterface;
using baseLibrary.Model;
using baseLibrary.RemoteInterface;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FlickerBatch_AlbumRetriever
{
    class MainClass
    {
        static String saveFlickrData = "FALSE";
        static String saveLocalData = "FALSE";
        static String join = "FALSE";
        static void Main(string[] args)
        {
            List<GenericAlbumData> lfad = DatabaseHelper.AlbumsWithFilesWithoutExtention();
            foreach (GenericAlbumData gad in lfad)
            {
                UpdateBatch(gad);
            }
            //List<RemoteImageData> rlist = DatabaseHelper.LoadImagesToFixDate();
            //FixDates(rlist);
        }

        private static void FixDates(List<RemoteImageData> rlist)
        {
            FlickerHelper.FixDates(rlist);
        }

        public static void UpdateBatch(Object albumObj)
        {
            GenericAlbumData album = (albumObj as GenericAlbumData);
            Console.WriteLine("Processing: " + album.Name + "Count :" + album.NumberOfPhotos);
            List<RemoteImageData> radList = DatabaseHelper.LoadFilesWithoutExtention(album.Name);
            foreach(RemoteImageData rad in radList)
            {
                rad.Name = rad.Name + ".JPG";
            }
            FlickerHelper.UpdateImage(radList);
            //FlickerHelper.WaitForAllThreads();
            DatabaseHelper.DeleteRemoteImageData(album.Name);
            FlickerCache.SaveRemoteAlbum(album.Name);
            
        }
        //   DatabaseHelper.InitiallizeDataStructure();
        //   Dictionary<string, string> config = DatabaseHelper.LoadMasterConfigData("BATCH_CONFIG");

        //   if(config.TryGetValue("SaveFlickerData", out saveFlickrData) && saveFlickrData == "TRUE" )
        //   {

        //       Console.WriteLine("Download Photo Info from Flicker");
        //       List<FlickrAlbumData> psList = FlickerHelper.LoadAllAlbums();
        //       foreach (FlickrAlbumData ps in psList)
        //       {
        //           FlickerHelper.SaveRemotePicturesData(ps);                    
        //       }
        //       FlickerHelper.WaitForAllThreads();


        //   }
        //   if (config.TryGetValue("SaveLocalData", out saveLocalData) && saveLocalData == "TRUE")
        //   {
        //       Dictionary<string, string> local_data = DatabaseHelper.LoadMasterConfigData("LOCAL");
        //       FilesystemHelper.SaveLocalImageData(ConfigModel._LocalData["LocalBasePath"]);
        //       //DatabaseHelper.RemovePrefix(local_data["LocalBasePath"]);
        //   }

        //   if (config.TryGetValue("SaveJoinData", out join) && join == "TRUE")
        //   {
        //       DatabaseHelper.SaveJoinData();
        //   }
        //   Console.WriteLine("=====================       DONE       =====================");
        //   Console.ReadLine();

        //}



    }
}
