﻿using baseLibrary.DBInterface;
using FlickerBatch_AlbumRetriever.Model;
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
        private static List<EventWaitHandle> threadFinishEvents = new List<EventWaitHandle>();

        public static void Flickr_Auth(Dictionary<string, string> auth_data)
        {
            flickr = new Flickr(ApiKey, SharedSecret);
            String accessTokenStr = "";
            OAuthAccessToken accessToken = null;
            if (auth_data.ContainsKey("AccessTokenStr"))
            {
                accessTokenStr = auth_data["AccessTokenStr"];
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
                auth_data["AuthToken"] = AuthToken;
                auth_data["AccessTokenStr"] = sw.ToString();

            }
            flickr.OAuthAccessToken = accessToken.Token;
            flickr.OAuthAccessTokenSecret = accessToken.TokenSecret;
        }

        public static List<FlickrAlbumData> getAllAlbums()
        {
            List<FlickrAlbumData> retPs = DatabaseHelper.LoadFlickerAlbums();
            if (retPs.Count == 0)
            {
                Console.WriteLine("Loading Albums from Flicker..... ");
                PhotosetCollection psc = flickr.PhotosetsGetList(); 
                foreach (Photoset ps in psc)
                {
                    FlickrAlbumData fad = new FlickrAlbumData(ps.PhotosetId, ps.Title, ps.DateCreated, ps.NumberOfPhotos,ps.Description, DateTime.Now);
                    retPs.Add(fad);
                }
                Console.WriteLine("Saving Albums in local DB ..... ");
                DatabaseHelper.SaveFlickerAlbums(retPs);
            }
            return retPs;
        }

        public static void savePictures(FlickrAlbumData ps)
        {
            int numPage = 0;
            while (ps.NumberOfPhotos > numPage * 500  )
            {
                numPage++;
                Console.WriteLine("Loading Album Info(Async): " + ps.Name);
                var threadFinish = new EventWaitHandle(false, EventResetMode.ManualReset);
                threadFinishEvents.Add(threadFinish);
                flickr.PhotosetsGetPhotosAsync(ps.AlbumId, PhotoSearchExtras.All, numPage, 500, delegate(FlickrResult<PhotosetPhotoCollection> result)
                {
                    if (result.HasError == false)
                    {
                        List<BaseImageData> retList; 
                        List<BaseImageData> allPics = new List<BaseImageData>();
                        foreach (Photo p in result.Result)
                        {
                            RemoteImageData rid = new RemoteImageData(ps.Name, p.PhotoId, p.Title, p.DateTaken, p.Description);
                            allPics.Add(rid);
                        }

                        retList = DatabaseHelper.getPhotosToSave(allPics);
                        if (retList.Count > 0)
                        {
                            DatabaseHelper.SaveImageData(retList);
                            Console.WriteLine("Saving Album Info(Async): " + ps.Name + " Count = " + retList.Count + "/" + ps.NumberOfPhotos);
                        }
                        else
                        {
                            Console.WriteLine("Nothing to save in Album : " + ps.Name + " Count = " + retList.Count + "/" + ps.NumberOfPhotos);
                        }
                        retList.Clear();
                    }
                    else
                    {
                        Console.WriteLine("==============================================================");
                        Console.WriteLine("Error Retrieving Album Info(Async): " + ps.Name + " Count = " + ps.NumberOfPhotos);
                        Console.WriteLine("==============================================================");

                    }
                    threadFinish.Set();

                }
                );

                if (threadFinishEvents.Count > 60)
                {
                    Mutex.WaitAll(threadFinishEvents.ToArray(),150000);
                    threadFinishEvents.Clear();
                }
            }

            return;
        }



        internal static void WaitForAllThreads()
        {
            if (threadFinishEvents.Count > 0)
            {
                Mutex.WaitAll(threadFinishEvents.ToArray());
                threadFinishEvents.Clear();
            }
        }
    }
}
