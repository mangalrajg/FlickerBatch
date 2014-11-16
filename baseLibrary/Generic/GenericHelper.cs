using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace baseLibrary.Generic
{
    public static class GenericHelper
    {
        public static string DateTimeSQLite(DateTime datetime)
        {
            string dateTimeFormat = "yyyyMMddHHmmss";
            return datetime.ToString(dateTimeFormat);
        }
        public static string StringSQLite(String str)
        {
            return str.Replace("'", "''");
        }
        public static DateTime DateTimeSQLite(String datetime)
        {
            return DateTime.ParseExact(datetime, "yyyyMMddHHmmss",null);
        }

    }
}
