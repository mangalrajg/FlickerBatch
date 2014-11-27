﻿using baseLibrary.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace baseLibrary.Model
{
    public class RemoteImageData : BaseImageData
    {
        public static String sCheckSQL = "Select count(1) COUNT from " + TableNames.REMOTE_DATA + " where ID='{0}';";
        public static String sInsertSQL = "Insert into " + TableNames.REMOTE_DATA + " (TITLE,DATE_TAKEN,DESCRIPTION,ALBUM,ID,PROCESSED,SYNC_DATE) "
            + " VALUES('{0}','{1}','{2}','{3}', '{4}','{5}', '{6}')";

        public override string CheckSQL
        {
            get { return String.Format(sCheckSQL, PhotoId); }
        }
        public override string InsertSQL
        {
            get
            {
                return String.Format(sInsertSQL, GenericHelper.StringSQLite(Name), GenericHelper.DateTimeSQLite(DateTaken), GenericHelper.StringSQLite(Description),
                    GenericHelper.StringSQLite(Album), PhotoId, 'N', GenericHelper.DateTimeSQLite(DateTime.Now));
            }
        }
        public String Album { get; set; }
        public String PhotoId { get; set; }
        public RemoteImageData(String album, String photoId, String title, DateTime dateTaken, String desc)
            : base(title, dateTaken, desc)
        {
            Album = album;
            PhotoId = photoId;
        }

    }
}
