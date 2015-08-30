using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace baseLibrary.Model
{
    public class GenericAlbumData
    {
        public String Name { get; set; }
        public long NumberOfPhotos { get; set; }
        public long NumberOfVideos { get; set; }
        public GenericAlbumData(String name, long numberOfPhotos, long numberOfVideos)
        {
            this.Name = name;
            this.NumberOfPhotos = numberOfPhotos;
            this.NumberOfVideos = numberOfVideos;
        }

    }
}
