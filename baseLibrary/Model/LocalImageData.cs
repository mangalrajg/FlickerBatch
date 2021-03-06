﻿using baseLibrary.Generic;
using System;

namespace baseLibrary.Model
{
    public class LocalImageData : BaseImageData
    {
        #region SQLs
        public override String InsertSQL
        {
            get
            {
                return String.Format(sInsertSQL, GenericHelper.StringSQLite(Name), GenericHelper.DateTimeSQLite(DateTaken),
                      GenericHelper.StringSQLite(Description), GenericHelper.StringSQLite(Path), Size, 'N', GenericHelper.DateTimeSQLite(DateTime.Now));
            }
        }

        public override string CheckSQL
        {
            get { return String.Format(sCheckSQL, GenericHelper.StringSQLite(Name), GenericHelper.StringSQLite(Path), Size); }
        }

        public override string DeleteSQL
        {
            get { return String.Format("DELETE From {0} where FILENAME='{1}' and PATH='{2}'", SQLRepository.LOCAL_DATA, Name, Path); }
        }

        public static String sInsertSQL = "Insert into " + SQLRepository.LOCAL_DATA
            + "(FILENAME,DATE_TAKEN,DESCRIPTION,PATH,SIZE,PROCESSED, SYNC_DATE) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";

        public static String sCheckSQL = "Select count(1) from " + SQLRepository.LOCAL_DATA
            + " where FILENAME='{0}' and PATH='{1}' and SIZE={2}";
        #endregion

        public String Path { get; set; }
        public long Size { get; set; }

        public LocalImageData(String fileName, DateTime dateTaken, String desc, String path, long size)
            : base(fileName, dateTaken, desc)
        {
            Path = path;
            Size = size;
        }

    }
}
