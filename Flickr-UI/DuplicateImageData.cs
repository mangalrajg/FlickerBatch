using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Flickr_UI
{
    public class DuplicateImageData
    {
        public String SourcePath { get; set; }
        public String DestinationPath { get; set; }
        public int ImgCount { get; set; }
        private List<DuplicateImages> _imageDetails = null;
        public List<DuplicateImages> ImageDetails
        {
            get
            {
                if (_imageDetails == null)
                    _imageDetails = DatabaseHelper.loadDuplicateImages(SourcePath, DestinationPath);
                return _imageDetails;
            }
        }


        public DuplicateImageData(String src, String dest, int count)
        {
            SourcePath = src;
            DestinationPath = dest;
            ImgCount = count;
        }
    }
}
