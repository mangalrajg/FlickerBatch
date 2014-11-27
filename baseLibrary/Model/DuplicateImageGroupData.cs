using baseLibrary.DBInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Flickr_UI
{
    public enum Mode { REMOTE, LOCAL };
    public class DuplicateImageGroupData
    {
        private Mode _mode = Mode.LOCAL;
        public String SourcePath { get; set; }
        public String DestinationPath { get; set; }
        public int ImgCount { get; set; }
        private List<DuplicateImageData> _imageDetails = null;
        public List<DuplicateImageData> ImageDetails
        {
            get
            {
                if (_imageDetails == null)
                {
                    if (_mode == Mode.REMOTE)
                    {
                        _imageDetails = DatabaseHelper.LoadRemoteDuplicateImages(SourcePath, DestinationPath);
                    }
                    else
                    {
                        _imageDetails = DatabaseHelper.LoadLocalDuplicateImages(SourcePath, DestinationPath);
                    }
                }
                return _imageDetails;
            }
        }


        public DuplicateImageGroupData(String src, String dest, int count, Mode mode)
        {
            SourcePath = src;
            DestinationPath = dest;
            ImgCount = count;
            _mode = mode;
        }
    }
}
