using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flickr_UI
{
    public static class DatabaseHelper
    {
        private static Object concurrencyObj = new object();
        private static String dbProvider = "System.Data.SQLite";
        private static String connectionString = "Data Source=FlickerConfig.sqllite";

        private static String _loadDuplicatesSQL = @"select  path src,path2 dest, count(1) numPics from 
                                                        (select l1.filename, l1.date_taken, l1.path path, l2.path path2, l1.size from local_data l1, local_data l2 where
                                                        l1.filename = l2.filename and
                                                        l1.date_taken = l2.date_taken and
                                                        l1.size = l2.size and 
                                                        l1.path != l2.path
                                                        order by l1.filename desc)
                                                    group by path,path2;";
        private static String _loadDuplicateImagesSQL = @"select l1.filename, l1.date_taken, l1.path path, l2.path path2, l1.size from local_data l1, local_data l2 where
                                                        l1.filename = l2.filename and
                                                        l1.date_taken = l2.date_taken and
                                                        l1.size = l2.size and 
                                                        l1.path != l2.path and
                                                        l1.path = '{0}' and
                                                        l2.path = '{1}'
                                                        order by l1.filename desc;";

        public static List<DuplicateImageData> loadDuplicateImageData()
        {
            List<DuplicateImageData> duplicateImgList = new List<DuplicateImageData>();
            DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                using (DbCommand mycommand = cnn.CreateCommand())
                {

                    mycommand.CommandText = _loadDuplicatesSQL;
                    DbDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        String src = reader.GetString(0);
                        String dest = reader.GetString(1);
                        Int64 count = reader.GetInt64(2);
                        duplicateImgList.Add(new DuplicateImageData(src, dest, Convert.ToInt32(count)));
                    }

                }
                cnn.Close();
            }
            return duplicateImgList;
        }


        internal static List<DuplicateImages> loadDuplicateImages(string SourcePath, string DestinationPath)
        {
            List<DuplicateImages> duplicateImgList = new List<DuplicateImages>();
            DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                using (DbCommand mycommand = cnn.CreateCommand())
                {

                    mycommand.CommandText = String.Format(_loadDuplicateImagesSQL,SourcePath, DestinationPath);

                    DbDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        String filename = reader.GetString(0);
                        String date_taken = reader.GetString(1);
                        String src = reader.GetString(2);
                        String dest = reader.GetString(3);
                        Int64 size = reader.GetInt64(4);
                        duplicateImgList.Add(new DuplicateImages(filename, date_taken, src, dest, Convert.ToInt32(size)));
                    }

                }
                cnn.Close();
            }
            return duplicateImgList;
        }
    }
}
