using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace baseLibrary.Model
{
    public abstract class BaseImageData
    {
        public abstract String InsertSQL { get; }
        public abstract String CheckSQL { get; }
        public abstract String DeleteSQL { get; }
        public String Name { get; set; }
        public DateTime DateTaken { get; set; }
        public String Description { get; set; }

        public BaseImageData(String name, DateTime dateTaken, String desc)
        {
            this.Name = name;
            this.DateTaken = dateTaken;
            this.Description = desc;
        }

    }
}
