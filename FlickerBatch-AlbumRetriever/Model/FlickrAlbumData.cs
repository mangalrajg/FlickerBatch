using FlickerBatch_AlbumRetriever.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlickerBatch_AlbumRetriever.Model
{
    public class FlickrAlbumData
    {
        public static String InsertSQL = "Insert into " + TableNames.FLICKR_ALBUMS + " (ID, NAME,DATE_CREATED,DESCRIPTION, SYNC_DATE) VALUES('{0}','{1}','{2}','{3}','{4}')";
        public static String CheckSQL = "";
        

        public String AlbumId { get; set; }
        public String Name { get; set; }
        public DateTime DateCreated { get; set; }
        public String Description { get; set; }

        public FlickrAlbumData(String albumId, String name, DateTime dateTaken, String desc)
        {
            this.AlbumId = albumId;
            this.Name = name;
            this.DateCreated = dateTaken;
            this.Description = desc;
        }

        public String getInsertStatement()
        {
            return String.Format(InsertSQL, AlbumId, GenericHelper.StringSQLite(Name), GenericHelper.DateTimeSQLite(DateCreated), 
                GenericHelper.StringSQLite(Description), GenericHelper.DateTimeSQLite(DateTime.Now));
        }

        //public override string getCheckStatement()
        //{
        //    return String.Format(CheckSQL, AlbumId);
        //}




    }
}
