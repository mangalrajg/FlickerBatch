using FlickerBatch_AlbumRetriever.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlickerBatch_AlbumRetriever.Model
{
    public class LocalImageData : BaseImageData
    {
        public static String InsertSQL = "Insert into " + TableNames.LOCAL_DATA
            + "(FILENAME,DATE_TAKEN,DESCRIPTION,PATH,SIZE,PROCESSED, SYNC_DATE) VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}')";

        public static String CheckSQL = "Select count(1) from " + TableNames.LOCAL_DATA
            + " where FILENAME='{0}' and PATH='{1}' and SIZE={2}";

        public String Path { get; set; }
        public long Size { get; set; }

        public LocalImageData(String fileName, DateTime dateTaken, String desc, String path, long size)
            : base(fileName, dateTaken, desc)
        {
            Path = path;
            Size = size;
        }


        public override String getInsertStatement()
        {
            return String.Format(InsertSQL, GenericHelper.StringSQLite(Name), GenericHelper.DateTimeSQLite(DateTaken),
                GenericHelper.StringSQLite(Description), GenericHelper.StringSQLite(Path), Size, 'N', GenericHelper.DateTimeSQLite(DateTime.Now));
        }


        public override String getCheckStatement()
        {
            return String.Format(CheckSQL, GenericHelper.StringSQLite(Name), GenericHelper.StringSQLite(Path), Size);
        }
    }
}
