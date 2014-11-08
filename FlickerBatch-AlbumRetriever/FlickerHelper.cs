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


            PhotosetCollection c = flickr.PhotosetsGetList();

        }
        public static PhotosetCollection getCollection()
        {
            return flickr.PhotosetsGetList();
        }

        public static List<Photoset> getAllAlbums()
        {
            PhotosetCollection psc = getCollection();
            List<Photoset> retPs = new List<Photoset>();
            foreach (Photoset ps in psc)
            {
                retPs.Add(ps);
            }
            return retPs;
        }

        public static void savePictures(Photoset ps)
        {
            PhotosetPhotoCollection pspc = flickr.PhotosetsGetPhotos(ps.PhotosetId);
            var threadFinishEvents = new List<EventWaitHandle>();

 
            List<BaseImageData> retList = new List<BaseImageData>();
            int i = 0;
            foreach (Photo p in pspc)
            {
                i++;
                var threadFinish = new EventWaitHandle(false, EventResetMode.ManualReset);
                threadFinishEvents.Add(threadFinish);

                flickr.PhotosGetInfoAsync(p.PhotoId, p.Secret, delegate(FlickrResult<PhotoInfo> result)
                {
                    #region Delegate
                    if (result.HasError == false)
                    {
                        RemoteImageData rid = new RemoteImageData(ps.Title, p.PhotoId, p.Title, result.Result.DateTaken, p.Description);
                        DatabaseHelper.SaveImageData(rid);
                        Console.WriteLine("-> " + p.Title + " \t " + ps.Title);
                    }
                    else
                    {
                        RemoteImageData rid = new RemoteImageData(ps.Title, p.PhotoId, p.Title, DateTime.Today, result.ErrorMessage);
                        DatabaseHelper.SaveImageData(rid);

                        Console.WriteLine("Error: " + result.ErrorMessage);
                    }
                    #endregion
                    threadFinish.Set();
                }
                );

                if (i == 60)
                {
                    Console.WriteLine("\n\n");
                    Mutex.WaitAll(threadFinishEvents.ToArray());
                    threadFinishEvents.Clear();
                    i = 0;
                }
            }
            Mutex.WaitAll(threadFinishEvents.ToArray());
            threadFinishEvents.Clear();
            return;
        }
    }
}
