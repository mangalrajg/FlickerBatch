using baseLibrary.DBInterface;
using baseLibrary.LocalInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace baseLibrary.Model
{
    public class LocalAlbumData : GenericAlbumData
    {

        public LocalAlbumData(GenericAlbumData gid)
            :base(gid.Name,gid.NumberOfPhotos)
        {
        }

        private List<LocalImageData> _imageDetails = null;
        public List<LocalImageData> ImageDetails
        {
            get
            {
                if (_imageDetails == null)
                {
                    _imageDetails = DatabaseHelper.LoadLocalImageData(Name);
                }
                return _imageDetails;
            }
        }
    }
}
