using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlickerBatch_AlbumRetriever.ImageData
{
    public class LocalImageData : BaseImageData
    {
        public static String DbTable = "LOCAL_DATA";
        public static String InsertSQL = "Insert into " + DatabaseHelper.LOCAL_DATA
            + "(FILENAME,DATE_TAKEN,DESCRIPTION,PATH,SIZE) VALUES('{0}','{1}','{2}','{3}','{4}')";
        public static String CheckSQL = "Select count(1) from " + DatabaseHelper.LOCAL_DATA
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
            return String.Format(InsertSQL, Name.Replace("'", "''"), DateTaken, Description.Replace("'", "''"), Path.Replace("'", "''"), Size);
        }


        public override String getCheckStatement()
        {
            return String.Format(CheckSQL, Name.Replace("'", "''"), Path.Replace("'", "''"), Size);
        }
    }
}
