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
        String Album { get; set; }
        String PhotoId { get; set; }
        public RemoteImageData(String album, String photoId, String title, DateTime dateTaken, String desc)
            : base(title, dateTaken, desc)
        {
            Album = album;
            PhotoId = photoId;
        }

        override
        public String getInsertStatement()
        {
            String insertSQL;
            insertSQL = String.Format("Insert into {0} (TITLE,DATE_TAKEN,DESCRIPTION,ALBUM,ID) VALUES('{1}','{2}','{3}','{4}', '{5}')",DbTable, Name, DateTaken, Description, Album, PhotoId);
            return insertSQL;
        }

    }
}
