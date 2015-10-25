using baseLibrary.DBInterface;
using baseLibrary.Model;
using FlickrSync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace baseLibrary.LocalInterface
{
    public static class FilesystemHelper
    {
        private static List<BaseImageData> getFileList(String baseDir)
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
                if (fileName.EndsWith(".jpg", System.StringComparison.CurrentCultureIgnoreCase) ||
                    fileName.EndsWith(".avi", System.StringComparison.CurrentCultureIgnoreCase) ||
                    fileName.EndsWith(".bmp", System.StringComparison.CurrentCultureIgnoreCase) ||
                    fileName.EndsWith(".mov", System.StringComparison.CurrentCultureIgnoreCase) ||
                    fileName.EndsWith(".tif", System.StringComparison.CurrentCultureIgnoreCase) ||
                    fileName.EndsWith(".3gp", System.StringComparison.CurrentCultureIgnoreCase) ||
                    fileName.EndsWith(".mpg", System.StringComparison.CurrentCultureIgnoreCase) ||
                    fileName.EndsWith(".mp4", System.StringComparison.CurrentCultureIgnoreCase) ||
                    fileName.EndsWith(".png", System.StringComparison.CurrentCultureIgnoreCase) ||
                    fileName.EndsWith(".wmv", System.StringComparison.CurrentCultureIgnoreCase)
                )
                {
                    ImageInfo ii = new ImageInfo();
                    ii.Load(fileName, ImageInfo.FileTypes.FileTypeUnknown);
                    LocalImageData lid = new LocalImageData(fi.Name, ii.GetDateTaken(), ii.GetDescription(), fi.DirectoryName, fi.Length);
                    lidList.Add(lid);
                }
            }
            Console.WriteLine("Count= " + lidList.Count + "\t Processed Dir: " + baseDir);
            if (lidList.Count > 1000)
            {
                Console.WriteLine("Loaded PicsCount = " + lidList.Count + " Saving Data..");
                DatabaseHelper.SaveImageData(lidList);
                lidList.Clear();
            }

            return lidList;
        }
        public static void SaveLocalImageData(String baseDir)
        {
            DatabaseHelper.DeleteLocalImageData(baseDir.Substring(ConfigModel.LocalBasePath.Length));
            List<BaseImageData> lidList = getFileList(baseDir);
            DatabaseHelper.SaveImageData(lidList);
            DatabaseHelper.RemovePrefix(ConfigModel.LocalBasePath);
            lidList.Clear();


        }
    }

}
