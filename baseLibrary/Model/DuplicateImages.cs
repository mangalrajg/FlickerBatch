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
        private ImageSource _ImageData;

        public ImageSource ImageData
        {
            get
            {
                if (_ImageData == null)
                {
                    BitmapImage bi= new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri(@"C:\Users\Mangalraj\Desktop\share\pics\" + SourcePath + "\\" + FileName);
                    bi.DecodePixelWidth = 200;
                    bi.EndInit();
                    _ImageData = bi;

                }
                return _ImageData;
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
