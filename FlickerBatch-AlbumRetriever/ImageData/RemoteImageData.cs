using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlickerBatch_AlbumRetriever.ImageData
{
    public class RemoteImageData : BaseImageData
    {
        public static String DbTable = "REMOTE_DATA";
        public static String CheckSQL = "Select count(1) COUNT from REMOTE_DATA where ID='{0}';";
        public static String InsertSQL = "Insert into REMOTE_DATA (TITLE,DATE_TAKEN,DESCRIPTION,ALBUM,ID) VALUES('{0}','{1}','{2}','{3}', '{4}')";
        String Album { get; set; }
        String PhotoId { get; set; }
        public RemoteImageData(String album, String photoId, String title, DateTime dateTaken, String desc)
            : base(title, dateTaken, desc)
        {
            Album = album;
            PhotoId = photoId;
        }
        
        public override String getInsertStatement()
        {
            return String.Format(InsertSQL, Name, DateTaken, Description, Album.Replace("'","''"), PhotoId);
        }

        public override string getCheckStatement()
        {
            return String.Format(CheckSQL, PhotoId);
        }
    }
}
