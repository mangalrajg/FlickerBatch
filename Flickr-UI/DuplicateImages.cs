using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flickr_UI
{
    public class DuplicateImages
    {
        public string FileName {get; set;}
        public string Date_Taken { get; set; }
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public int Size { get; set; }

        public DuplicateImages(string filename, string date_taken, string src, string dest, int size)
        {
            this.FileName = filename;
            this.Date_Taken = date_taken;
            this.SourcePath = src;
            this.DestinationPath = dest;
            this.Size = size;
        }
    }
}
