using FlickerBatch_AlbumRetriever.Model;
using FlickrSync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlickerBatch_AlbumRetriever
{
    public static class FilesystemHelper
    {
        public static List<BaseImageData> getFileList(String baseDir)
        {
            List<BaseImageData> lidList = new List<BaseImageData>();
            foreach (String dList in Directory.GetDirectories(baseDir))
            {
                List<BaseImageData> subDList = getFileList(dList);
                lidList.AddRange(subDList);
            }
            String[] fileList = Directory.GetFiles(baseDir);
            foreach (String fileName in fileList)
            {
                FileInfo fi = new FileInfo(fileName);
                ImageInfo ii = new ImageInfo();
                ii.Load(fileName, ImageInfo.FileTypes.FileTypeUnknown);
                LocalImageData lid = new LocalImageData(fi.Name, ii.GetDateTaken(), ii.GetDescription(), fi.FullName, fi.Length);
                lidList.Add(lid);
            }
            Console.WriteLine("Count= " + lidList.Count + "\t Processed Dir: " + baseDir );
            if (lidList.Count > 100)
            {
                Console.WriteLine("Loaded PicsCount = " + lidList.Count + " Saving Data..");
                DatabaseHelper.SaveImageData(lidList);
                lidList.Clear();
            }
            return lidList;
        }
    }
}
