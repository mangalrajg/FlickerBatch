using baseLibrary.DBInterface;
using baseLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace baseLibrary.RemoteInterface
{
    public static class FlickerCache
    {
        #region AlbumCache
        private static List<FlickrAlbumData> _AlbumList = null;
        public static List<FlickrAlbumData> AlbumList
        {
            get
            {
                if (_AlbumList == null)
                {
                    _AlbumList = FlickerHelper.LoadAllAlbums();
                    DatabaseHelper.DeleteAllFlickerAlbums();
                    DatabaseHelper.SaveFlickrAlbums(_AlbumList);
                }
                return _AlbumList;
            }
        }

        private static Dictionary<String, FlickrAlbumData> _AlbumSearchDict = null;
        public static Dictionary<String, FlickrAlbumData> AlbumSearchDict
        {
            get
            {
                if (_AlbumSearchDict == null)
                {
                    _AlbumSearchDict = new Dictionary<string, FlickrAlbumData>();
                    foreach (FlickrAlbumData fad in AlbumList)
                    {
                        Console.WriteLine(fad.Name);
                        if (!_AlbumSearchDict.ContainsKey(fad.Name))
                            _AlbumSearchDict.Add(fad.Name, fad);
                        else
                        {
                            Console.WriteLine("===========================");
                            Console.WriteLine("Duplicate Album: " + fad.Name);
                            Console.WriteLine("===========================");
                        }
                    }
                }
                return _AlbumSearchDict;
            }
        }
        #endregion

        private static Dictionary<FlickrAlbumData, Dictionary<String, RemoteImageData>> _imageDataCache = new Dictionary<FlickrAlbumData, Dictionary<string, RemoteImageData>>();
        public static RemoteImageData getImageData(String title, String album)
        {
            RemoteImageData ret = null;
            FlickrAlbumData fad = null;
            if (AlbumSearchDict.TryGetValue(album, out fad))
            {
                Dictionary<String, RemoteImageData> imageList = null;
                if (_imageDataCache.TryGetValue(fad, out imageList) == false)
                {
                    List<BaseImageData> rlist = FlickerHelper.LoadRemotePicturesData(fad);
                    if (rlist.Count > 0)
                    {
                        imageList = new Dictionary<string, RemoteImageData>();
                        foreach (BaseImageData rd in rlist)
                        {
                            if (!imageList.ContainsKey(rd.Name))
                                imageList.Add(rd.Name, rd as RemoteImageData);
                            else
                            {
                                Console.WriteLine("Duplicate Image found in Album: " + rd.Name);
                            }
                        }
                        _imageDataCache.Add(fad, imageList);
                    }
                    else
                    {
                        Console.WriteLine("Somethign went worng? is there an exception??");
                        return null;
                    }

                }
                RemoteImageData rid = null;
                if (imageList.TryGetValue(title, out rid))
                {
                    ret = rid;
                }
            }
            else
            {
                Console.WriteLine("Album: " + album + " Not found");
            }
            return ret;
        }
        public static void SaveRemoteAlbum(string p)
        {
            if (AlbumSearchDict.ContainsKey(p))
            {
                FlickrAlbumData fad = AlbumSearchDict[p];
                List<BaseImageData> list = FlickerHelper.LoadRemotePicturesData(fad);
                DatabaseHelper.DeleteRemoteImageData(p);
                Console.WriteLine("Saving Album " + p + " to database");
                DatabaseHelper.SaveImageData(list);
            }
        }
        //public static Boolean MovePicture(string title, string oldAlbumName, string newAlbumName)
        //{
        //    Boolean ret = false;
        //    RemoteImageData rid = getImageData(title, oldAlbumName);
        //    FlickrAlbumData oldAlbumData = AlbumSearchDict[oldAlbumName];
        //    if (rid != null)
        //    {
        //        FlickrAlbumData newAlbumData = null;
        //        if (AlbumSearchDict.TryGetValue(newAlbumName, out newAlbumData))
        //        {
        //            FlickerHelper.MovePicture(rid.PhotoId, oldAlbumData.AlbumId, newAlbumData.AlbumId);
        //            newAlbumData.NumberOfPhotos++;
        //            oldAlbumData.NumberOfPhotos--;
        //        }
        //        else
        //        {
        //            newAlbumData = FlickerHelper.CreateAlbumAndMovePicture(rid.PhotoId, oldAlbumData.AlbumId, newAlbumName);
        //            AlbumSearchDict.Add(newAlbumName, newAlbumData);
        //            AlbumList.Add(newAlbumData);
        //            newAlbumData.NumberOfPhotos++;
        //            oldAlbumData.NumberOfPhotos--;
        //            //DatabaseHelper.SaveFlickerAlbum(newAlbumData);
        //        }
        //    }

        //    return ret;
        //}
        public static void MovePictures(List<string> titleList, string oldAlbumName, string newAlbumName)
        {
            RemoteImageData rid = null;
            List<String> idString = new List<String>();
            foreach (String title in titleList)
            {
                rid = getImageData(title, oldAlbumName);
                if (rid != null)
                {
                    idString.Add(rid.PhotoId);
                }
            }
            FlickrAlbumData oldAlbumData = AlbumSearchDict[oldAlbumName];
            FlickrAlbumData newAlbumData = null;
            if (AlbumSearchDict.TryGetValue(newAlbumName, out newAlbumData))
            {
                FlickerHelper.MovePictures(idString.ToArray(), oldAlbumData.AlbumId, newAlbumData.AlbumId);
                newAlbumData.NumberOfPhotos += idString.Count;
                oldAlbumData.NumberOfPhotos -= idString.Count;
            }
            else
            {
                newAlbumData = FlickerHelper.CreateAlbumAndMovePictures(idString.ToArray(), oldAlbumData.AlbumId, newAlbumName);
                AlbumSearchDict.Add(newAlbumName, newAlbumData);
                AlbumList.Add(newAlbumData);
                newAlbumData.NumberOfPhotos += titleList.Count;
                oldAlbumData.NumberOfPhotos -= titleList.Count;
            }
            return;
        }

        public static void UploadImages(List<LocalImageData> fileNames, String albumName)
        {
            String basePath = ConfigModel.LocalBasePath;
            String folder = basePath + "\\" + albumName;
            FlickrAlbumData newAlbumData = null;
            if (AlbumSearchDict.TryGetValue(albumName, out newAlbumData))
            {
                FlickerHelper.UploadImagesToAlbum(fileNames, folder, newAlbumData.AlbumId);
                newAlbumData.NumberOfPhotos += fileNames.Count;
            }
            else
            {
                newAlbumData = FlickerHelper.CreateAlbumAndUploadImages(fileNames, folder, albumName);
                if (newAlbumData != null)
                {
                    AlbumSearchDict.Add(albumName, newAlbumData);
                    AlbumList.Add(newAlbumData);
                    newAlbumData.NumberOfPhotos += fileNames.Count;
                }

            }

        }

        public static void CleanCache()
        {
            _AlbumList = null;
            _AlbumSearchDict = null;

        }
    }
}
