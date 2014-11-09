using FlickerBatch_AlbumRetriever.ImageData;
using FlickrNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlickerBatch_AlbumRetriever
{
    public static class FlickerHelper
    {
        public static Flickr flickr = null;
        static String ApiKey = "6c5f9affbbd5f9f96054300504900e92";
        static String SharedSecret = "c9fbcf157653bcd9";

        public static void Flickr_Auth(Dictionary<string, string> auth_data)
        {
            flickr = new Flickr(ApiKey, SharedSecret);
            String accessTokenStr = "";
            OAuthAccessToken accessToken = null;
            if (auth_data.ContainsKey("accessTokenStr"))
            {
                accessTokenStr = auth_data["accessTokenStr"];
                XmlSerializer writer = new XmlSerializer(typeof(OAuthAccessToken));
                StringReader sr = new StringReader(accessTokenStr);
                accessToken = (OAuthAccessToken)writer.Deserialize(sr);
            }
            else
            {
                OAuthRequestToken requestToken = flickr.OAuthGetRequestToken("oob");
                string url = flickr.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Write);
                System.Diagnostics.Process.Start(url);
                Console.WriteLine("Enter The Auth Token: ");
                string AuthToken = Console.ReadLine();
                accessToken = flickr.OAuthGetAccessToken(requestToken, AuthToken);
                XmlSerializer writer = new XmlSerializer(typeof(OAuthAccessToken));
                StringWriter sw = new StringWriter();
                writer.Serialize(sw, accessToken);
                auth_data["accessTokenStr"] = sw.ToString();

            }
            flickr.OAuthAccessToken = accessToken.Token;
            flickr.OAuthAccessTokenSecret = accessToken.TokenSecret;



        }
        public static PhotosetCollection getCollection()
        {
            return flickr.PhotosetsGetList();
        }

        public static List<FlickrAlbumData> getAllAlbums()
        {
            List<FlickrAlbumData> retPs = DatabaseHelper.LoadFlickerAlbums();
            if (retPs.Count == 0)
            {
                PhotosetCollection psc = getCollection();
                foreach (Photoset ps in psc)
                {
                    FlickrAlbumData fad = new FlickrAlbumData(ps.PhotosetId, ps.Title, ps.DateCreated, ps.Description);
                    retPs.Add(fad);
                }
                DatabaseHelper.SaveFlickerAlbums(retPs);
            }
            return retPs;
        }

        public static void savePictures(FlickrAlbumData ps)
        {
            PhotosetPhotoCollection pspc = flickr.PhotosetsGetPhotos(ps.AlbumId);
            var threadFinishEvents = new List<EventWaitHandle>();

 
            List<BaseImageData> retList = new List<BaseImageData>();
            int unsavedImageCount = 0, batchSaveCount = 0;
            foreach (Photo p in pspc)
            {
                if (DatabaseHelper.IsImageInDB(p.PhotoId))
                    continue;

                unsavedImageCount++;
                var threadFinish = new EventWaitHandle(false, EventResetMode.ManualReset);
                threadFinishEvents.Add(threadFinish);

                flickr.PhotosGetInfoAsync(p.PhotoId, p.Secret, delegate(FlickrResult<PhotoInfo> result)
                {
                    #region Delegate
                    if (result.HasError == false)
                    {
                        RemoteImageData rid = new RemoteImageData(ps.Name, p.PhotoId, p.Title, result.Result.DateTaken, p.Description);
                        lock (retList)
                        {
                            retList.Add(rid);
                            Console.WriteLine("-> " + p.Title + " \t " + ps.Name);
                        }
                        //DatabaseHelper.SaveImageData(rid);
                    }
                    else
                    {
                        RemoteImageData rid = new RemoteImageData(ps.Name, p.PhotoId, p.Title, DateTime.Today, result.ErrorMessage);
 //                       DatabaseHelper.SaveImageData(rid);
                        lock (retList)
                        {
                            retList.Add(rid);
                            Console.WriteLine("Error: " + ps.Name +" "+ result.ErrorMessage);
                        }
                    }
                    #endregion
                    threadFinish.Set();
                }
                );

                if (unsavedImageCount == 60)
                {
                    batchSaveCount++;
                    Console.WriteLine("\n\n");
                    Mutex.WaitAll(threadFinishEvents.ToArray());
                    threadFinishEvents.Clear();
                    unsavedImageCount = 0;
                    Console.WriteLine("Saving Images.. " + (retList.Count + (batchSaveCount * 60)) + "/" + pspc.Count);
                    DatabaseHelper.SaveImageData(retList);
                    retList.Clear();
                }
            }
            if (threadFinishEvents.Count > 0)
            {
                Mutex.WaitAll(threadFinishEvents.ToArray());
                Console.WriteLine("Saving Images.. " + (retList.Count + (batchSaveCount * 60)) + "/" + pspc.Count);
                batchSaveCount = 0;
                threadFinishEvents.Clear();
            }
            return;
        }
    }
}
