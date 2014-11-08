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
        static bool saveFlickrData = true;
        static bool saveLocalData = false;
        static void Main(string[] args)
        {

            DatabaseHelper.InitiallizeDataStructure();
            Dictionary<string, string> auth_data = DatabaseHelper.loadConfigData("AUTH");
            FlickerHelper.Flickr_Auth(auth_data);
            DatabaseHelper.saveConfigData("AUTH", auth_data);

            if (saveFlickrData)
            {
                List<Photoset> psList = FlickerHelper.getAllAlbums();
                foreach (Photoset ps in psList)
                {
                    FlickerHelper.savePictures(ps);                    
                }
            }
            Console.ReadLine();

         }



    }
}
