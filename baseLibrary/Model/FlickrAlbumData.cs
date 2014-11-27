using baseLibrary.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace baseLibrary.Model
{
    public class FlickrAlbumData
    {
        public static String InsertSQL = "Insert into " + TableNames.FLICKR_ALBUMS + " (ID, NAME,DATE_CREATED,NUM_PICS,DESCRIPTION, SYNC_DATE) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')";
        public static String CheckSQL = "";
        

        public String AlbumId { get; set; }
        public String Name { get; set; }
        public DateTime DateCreated { get; set; }
        public int NumberOfPhotos { get; set; }
        public String Description { get; set; }
        public DateTime SyncDate{ get; set; }

        public FlickrAlbumData(String albumId, String name, DateTime dateTaken, int NumberOfPhotos, String desc, DateTime syncDate)
        {
            this.AlbumId = albumId;
            this.Name = name;
            this.DateCreated = dateTaken;
            this.NumberOfPhotos = NumberOfPhotos;
            this.Description = desc;
            this.SyncDate = syncDate;
        }

        public String getInsertStatement()
        {
            return String.Format(InsertSQL, AlbumId, GenericHelper.StringSQLite(Name), GenericHelper.DateTimeSQLite(DateCreated), NumberOfPhotos,
                GenericHelper.StringSQLite(Description), GenericHelper.DateTimeSQLite(SyncDate));
        }

        //public override string getCheckStatement()
        //{
        //    return String.Format(CheckSQL, AlbumId);
        //}




    }
}
