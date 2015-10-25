using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace baseLibrary.Model
{
    public class DuplicateAlbumData
    {
        public String AlbumName { get; set; }
        public List<FlickrAlbumData> Albums { get; set;}

        public DuplicateAlbumData(string albumName)
        {
            this.AlbumName = albumName;
            Albums = new List<FlickrAlbumData>();
        }
    }
}
