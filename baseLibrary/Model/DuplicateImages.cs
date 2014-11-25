using baseLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Flickr_UI
{
    public class DuplicateImages
    {
        public string FileName { get; set; }
        public string DateTaken { get; set; }
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public int Size { get; set; }
        private ImageSource _SrcImageData;
        public ImageSource SrcImageData
        {
            get
            {
                if (_SrcImageData == null)
                {
                    BitmapImage bi= new BitmapImage();
                    try
                    {
                        bi.BeginInit();
                        bi.UriSource = new Uri(ConfigModel.LocalData["LocalBasePath"] + "\\" + SourcePath + "\\" + FileName);
                        bi.DecodePixelWidth = 200;
                        bi.EndInit();
                    }
                    catch(Exception )
                    {
                        Console.WriteLine("Not able to load: " + SourcePath + "\\" + FileName);
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
                    bi.BeginInit();
                    bi.UriSource = new Uri(ConfigModel.LocalData["LocalBasePath"] + "\\" + DestinationPath + "\\" + FileName);
                    bi.DecodePixelWidth = 200;
                    bi.EndInit();
                    _DestImageData = bi;

                }
                return _DestImageData;
            }
        }

        public DuplicateImages(string filename, string date_taken, string src, string dest, int size)
        {
            this.FileName = filename;
            this.DateTaken = date_taken;
            this.SourcePath = src;
            this.DestinationPath = dest;
            this.Size = size;
        }
    }
}
