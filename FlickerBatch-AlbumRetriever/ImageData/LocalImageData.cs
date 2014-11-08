using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlickerBatch_AlbumRetriever.ImageData
{
    public class LocalImageData : BaseImageData
    {
        public static String DbTable = "REMOTE_DATA";
        public String FileName { get; set; }
        public String Path { get; set; }

        public LocalImageData(String title, String fileName, DateTime dateTaken, String desc, String path)
            : base(title, dateTaken, desc)
        {
            FileName = fileName;
            Path = path;


        }
        
        override
        public String getInsertStatement()
        {
            String insertSQL;
            insertSQL = String.Format("Insert into {0} (TITLE,DATE_TAKEN,DESCRIPTION,ALBUM,ID) VALUES('{1}','{2}','{3}')", DbTable, FileName, DateTaken, Description);
            return insertSQL;
        }

    }
}
