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
        public String Path { get; set; }
        public int Size { get; set; }

        public LocalImageData(String fileName, DateTime dateTaken, String desc, String path, int size)
            : base(fileName, dateTaken, desc)
        {
            Path = path;
            Size = size;
        }


        public override String getInsertStatement()
        {
            return String.Format("Insert into {0} (FILENAME,DATE_TAKEN,DESCRIPTION,PATH,SIZE) VALUES('{1}','{2}','{3}')", DbTable, Name, DateTaken, Description);
        }


        public override String getCheckStatement()
        {
            return String.Format("Select count(1) from {0} where FILENAME='{2}' and PATH='{3}' and SIZE={4};", DbTable, Name, Path, Size );
        }
    }
}
