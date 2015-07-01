using baseLibrary.DBInterface;
using baseLibrary.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace baseLibrary.Model
{
    public class FlickrAlbumData : GenericAlbumData
    {
        #region SQLs
        private static String _InsertSQL = "Insert into " + TableNames.FLICKR_ALBUMS + " (ID, NAME,DATE_CREATED,NUM_PICS,ACTUAL_NUM_PICS,DESCRIPTION, SYNC_DATE) VALUES('{0}','{1}','{2}','{3}','{4}','{5}',{6})";
        public String InsertSQL
        {
            get
            {
                return String.Format(_InsertSQL, AlbumId, GenericHelper.StringSQLite(Name), GenericHelper.DateTimeSQLite(DateCreated), NumberOfPhotos, ActualPhotoCount,
                GenericHelper.StringSQLite(Description), GenericHelper.DateTimeSQLite(SyncDate));
            }
        }
        public static String DeleteAllSQL
        {
            get
            {
                return String.Format("DELETE FROM " + TableNames.FLICKR_ALBUMS + ";");
            }
        }
        public String DeleteSQL
        {
            get
            {
                return String.Format("DELETE FROM " + TableNames.FLICKR_ALBUMS + " WHERE ID='{0}';", AlbumId);
            }
        }

        #endregion

        private List<RemoteImageData> _imageDetails = null;
        public List<RemoteImageData> ImageDetails
        {
            get
            {
                if (_imageDetails == null)
                {
                    _imageDetails = DatabaseHelper.LoadRemoteImageData(Name);
                }
                return _imageDetails;
            }
        }

        public String AlbumId { get; set; }
        public DateTime DateCreated { get; set; }
        public String Description { get; set; }
        public DateTime SyncDate { get; set; }
        public int ActualPhotoCount { get; set; }

        public FlickrAlbumData(String albumId, String name, DateTime dateTaken, int NumberOfPhotos, int actualPhotoCount, String desc, DateTime syncDate)
            : base(name, NumberOfPhotos)
        {
            this.AlbumId = albumId;
            this.DateCreated = dateTaken;
            this.Description = desc;
            this.SyncDate = syncDate;
            this.ActualPhotoCount = actualPhotoCount;
        }
        public FlickrAlbumData(String albumId, String name, DateTime dateTaken, int NumberOfPhotos, String desc, DateTime syncDate)
            : base(name, NumberOfPhotos)
        {
            this.AlbumId = albumId;
            this.DateCreated = dateTaken;
            this.Description = desc;
            this.SyncDate = syncDate;
            this.ActualPhotoCount = 0;
        }

        public override string ToString()
        {
            return "[" + NumberOfPhotos + "][" + ActualPhotoCount + "] Name=" + Name;
        }

    }
}
