using baseLibrary.DBInterface;
using baseLibrary.Model;
using FlickrNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace baseLibrary.RemoteInterface
{
    public static class FlickerHelper
    {
        public static Flickr flickr = null;
        private static String ApiKey = "6c5f9affbbd5f9f96054300504900e92";
        private static String SharedSecret = "c9fbcf157653bcd9";
        private static List<EventWaitHandle> threadFinishEvents = new List<EventWaitHandle>();

        static FlickerHelper()
        {
            Dictionary<string, string> auth_data = ConfigModel.AuthData;
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
                DatabaseHelper.SaveConfigData("AUTH", auth_data);
            }
            flickr.OAuthAccessToken = accessToken.Token;
            flickr.OAuthAccessTokenSecret = accessToken.TokenSecret;
        }

        public static List<FlickrAlbumData> LoadAllAlbums()
        {
            List<FlickrAlbumData> retPs = new List<FlickrAlbumData>();
            Console.WriteLine("Loading Albums from Flicker..... ");
            PhotosetCollection psc = flickr.PhotosetsGetList();
            foreach (Photoset ps in psc)
            {
                FlickrAlbumData fad = new FlickrAlbumData(ps.PhotosetId, ps.Title, ps.DateCreated, ps.NumberOfPhotos, ps.Description, DateTime.Now);
                retPs.Add(fad);
            }
            return retPs;
        }

        public static void SaveRemotePicturesData(FlickrAlbumData ps)
        {
            int numPage = 0;
            while (ps.NumberOfPhotos > numPage * 500)
            {
                numPage++;
                Console.WriteLine("Loading Album Info(Async): " + ps.Name);
                var threadFinish = new EventWaitHandle(false, EventResetMode.ManualReset);
                threadFinishEvents.Add(threadFinish);
                #region flickr.PhotosetsGetPhotosAsync
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

                        retList = DatabaseHelper.GetPhotosToSave(allPics);
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
                #endregion
                if (threadFinishEvents.Count > 60)
                {
                    Mutex.WaitAll(threadFinishEvents.ToArray(), 150000);
                    threadFinishEvents.Clear();
                }
            }

            return;
        }

        public static List<BaseImageData> LoadRemotePicturesData(FlickrAlbumData ps)
        {
            int numPage = 0;
            List<BaseImageData> allPics = new List<BaseImageData>();
            try
            {
                while (ps.NumberOfPhotos > numPage * 500)
                {
                    numPage++;
                    Console.WriteLine("Loading Album Info: " + ps.Name + "  Page:" + numPage);
                    PhotosetPhotoCollection ppc = flickr.PhotosetsGetPhotos(ps.AlbumId, PhotoSearchExtras.All, numPage, 500);
                    foreach (Photo p in ppc)
                    {
                        RemoteImageData rid = new RemoteImageData(ps.Name, p.PhotoId, p.Title, p.DateTaken, p.Description);
                        allPics.Add(rid);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return allPics;
        }

        public static void RenameImage(List<RemoteImageData> ridList, String newAlbumName)
        {
            String tmpAlbumName = "";
            String oldAblumId = "";
            String newAblumId = DatabaseHelper.GetAlbumID(newAlbumName);

            foreach (RemoteImageData rid in ridList)
            {
                if (rid.Album != tmpAlbumName)
                {
                    oldAblumId = DatabaseHelper.GetAlbumID(rid.Album);
                    tmpAlbumName = rid.Album;
                }
                flickr.PhotosetsRemovePhoto(oldAblumId, rid.PhotoId);
                flickr.PhotosetsAddPhoto(newAblumId, rid.PhotoId);
            }
        }

        public static void WaitForAllThreads()
        {
            if (threadFinishEvents.Count > 0)
            {
                Mutex.WaitAll(threadFinishEvents.ToArray());
                threadFinishEvents.Clear();
            }
        }


        public static void MovePicture(string photoID, string oldAblumId, string newAlbumId)
        {
            try
            {
                flickr.PhotosetsRemovePhoto(oldAblumId, photoID);
                flickr.PhotosetsAddPhoto(newAlbumId, photoID);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw ex;
            }
        }

        public static FlickrAlbumData CreateAlbumAndMovePicture(string photoID, string oldAblumId, string newAlbumName)
        {
            flickr.PhotosetsRemovePhoto(oldAblumId, photoID);
            Photoset ps = flickr.PhotosetsCreate(newAlbumName, photoID);

            return new FlickrAlbumData(ps.PhotosetId, newAlbumName, ps.DateCreated, ps.NumberOfPhotos, ps.Description, DateTime.Now);

        }

        internal static void MovePictures(string[] photoIDList, string oldAblumId, string newAlbumId)
        {
            try
            {
                flickr.PhotosetsRemovePhotos(oldAblumId, photoIDList);
                foreach (String photoID in photoIDList)
                {
                    Console.Write(".");
                    flickr.PhotosetsAddPhoto(newAlbumId, photoID);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw ex;
            }
            Console.WriteLine("");
        }

        internal static FlickrAlbumData CreateAlbumAndMovePictures(string[] photoIDList, string oldAblumId, string newAlbumName)
        {
            FlickrAlbumData fad = null;
            try
            {
                flickr.PhotosetsRemovePhotos(oldAblumId, photoIDList);
                if (photoIDList.Length > 0)
                {
                    Photoset ps = flickr.PhotosetsCreate(newAlbumName, photoIDList[0]);
                    fad = new FlickrAlbumData(ps.PhotosetId, newAlbumName, ps.DateCreated, ps.NumberOfPhotos, ps.Description, DateTime.Now);
                    foreach (String photoID in photoIDList)
                    {
                        Console.Write(".");
                        if (photoID != photoIDList[0])
                            flickr.PhotosetsAddPhoto(ps.PhotosetId, photoID);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw ex;
            }
            Console.WriteLine("");
            return fad;
        }

        public static List<String> UploadImage(List<String> fileNames, String folder)
        {
            List<String> photoIdList = new List<string>();
            foreach (String filename in fileNames)
            {
                FileStream fs = new FileStream(folder + "\\" + filename, FileMode.Open, FileAccess.Read);
                var threadFinish = new EventWaitHandle(false, EventResetMode.ManualReset);
                threadFinishEvents.Add(threadFinish);

                flickr.UploadPictureAsync(fs, filename, filename, "", "", false, false, false, ContentType.Photo, SafetyLevel.Safe, HiddenFromSearch.None, r1 =>
                    {
                        FlickrResult<string> result = r1;
                        if (result.HasError == false)
                        {
                            String photoId = result.Result;
                            if (photoId != null)
                                photoIdList.Add(photoId);
                        }
                        else
                        {
                            Console.WriteLine("Error uploading " + filename + " Album:" + folder);
                        }
                        threadFinish.Set();
                    });
                if (threadFinishEvents.Count > 3)
                {
                    Mutex.WaitAll(threadFinishEvents.ToArray(), 150000);
                    threadFinishEvents.Clear();
                }

            }
            WaitForAllThreads();
            return photoIdList;
        }


        internal static void AddImagesToAlbum(List<string> photoIDList, string albumId)
        {
            foreach (String photoID in photoIDList)
            {
                Console.Write(".");
                flickr.PhotosetsAddPhoto(albumId, photoID);
            }
            Console.WriteLine("");
        }

        internal static FlickrAlbumData CreateAlbumAndAddPictures(List<string> photoIDList, string albumName)
        {
            FlickrAlbumData fad = null;
            try
            {
                if (photoIDList.Count > 0)
                {
                    Photoset ps = flickr.PhotosetsCreate(albumName, photoIDList[0]);
                    fad = new FlickrAlbumData(ps.PhotosetId, albumName, ps.DateCreated, ps.NumberOfPhotos, ps.Description, DateTime.Now);
                    foreach (String photoID in photoIDList)
                    {
                        Console.Write(".");
                        if (photoID != photoIDList[0])
                            flickr.PhotosetsAddPhoto(ps.PhotosetId, photoID);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
                throw ex;
            }
            Console.WriteLine("");
            return fad;
        }
    }
}
