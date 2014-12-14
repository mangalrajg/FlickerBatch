using baseLibrary.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Flickr_UI
{
    public class DuplicateImageData
    {
        public string FileName { get; set; }
        public string DateTaken { get; set; }
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public Boolean IsProcessed { get; set; }
        private ImageSource _SrcImageData;
        public ImageSource SrcImageData
        {
            get
            {
                if (_SrcImageData == null)
                {
                    BitmapImage bi= new BitmapImage();
                    if (File.Exists(SrcFileName) && SrcFileName.EndsWith(".jpg", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        try
                        {
                            bi.BeginInit();
                            bi.UriSource = new Uri(SrcFileName);
                            bi.CacheOption = BitmapCacheOption.OnLoad;
                            bi.DecodePixelWidth = 200;
                            bi.EndInit();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Not able to load: " + SrcFileName);
                        }
                    }
                    _SrcImageData = bi;

                }
                return _SrcImageData;
            }
        }
        private ImageSource _DestImageData;
        public ImageSource DestImageData
        {
            get
            {
                if (_DestImageData == null)
                {
                    BitmapImage bi = new BitmapImage();
                    if (File.Exists(DestFileName) && DestFileName.EndsWith(".jpg", System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        try
                        {
                            bi.BeginInit();
                            bi.UriSource = new Uri(DestFileName);
                            bi.CacheOption = BitmapCacheOption.OnLoad;
                            bi.DecodePixelWidth = 200;
                            bi.EndInit();
                        }
                        catch (Exception) 
                        {
                            Console.WriteLine("Not able to load: " + SourcePath + "\\" + FileName);
                        }
                    }
                    _DestImageData = bi;

                }
                return _DestImageData;
            }
        }
        public String SrcFileName
        {
            get
            {
                return ConfigModel.LocalBasePath + "\\" + SourcePath + "\\" + FileName;
            }
        }

        public String DestFileName
        {
            get
            {
                return ConfigModel.LocalBasePath + "\\" + DestinationPath + "\\" + FileName;
            }
        }

        public DuplicateImageData(string filename, string date_taken, string src, string dest)
        {
            this.FileName = filename;
            this.DateTaken = date_taken;
            this.SourcePath = src;
            this.DestinationPath = dest;
            this.IsProcessed = false;
        }
    }
}
