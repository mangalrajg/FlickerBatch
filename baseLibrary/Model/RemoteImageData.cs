using baseLibrary.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace baseLibrary.Model
{
    public class RemoteImageData : BaseImageData
    {
        #region SQLs
        public static String sCheckSQL = "Select count(1) COUNT from " + SQLRepository.REMOTE_DATA + " where ID='{0}';";
        public static String sInsertSQL = "Insert into " + SQLRepository.REMOTE_DATA + " (TITLE,DATE_TAKEN,DESCRIPTION,ALBUM,ID,PROCESSED,SYNC_DATE, MEDIA) "
            + " VALUES('{0}','{1}','{2}','{3}', '{4}','{5}', '{6}','{7}')";

        public override string CheckSQL
        {
            get { return String.Format(sCheckSQL, PhotoId); }
        }
        public override string InsertSQL
        {
            get
            {
                return String.Format(sInsertSQL, GenericHelper.StringSQLite(Name), GenericHelper.DateTimeSQLite(DateTaken), GenericHelper.StringSQLite(Description),
                    GenericHelper.StringSQLite(Album), PhotoId, 'N', GenericHelper.DateTimeSQLite(DateTime.Now), Media);
            }
        }
        public override string DeleteSQL
        {
            get { return String.Format("DELETE From {0} where ID='{1}' ", SQLRepository.REMOTE_DATA, PhotoId); }
        }
        #endregion

        public String Album { get; set; }
        public String PhotoId { get; set; }
        public String Media { get; set; }
        public RemoteImageData(String album, String photoId, String title, DateTime dateTaken, String desc, String media)
            : base(title, dateTaken, desc)
        {
            Album = album;
            PhotoId = photoId;
            Media = media;
        }

    }
}
