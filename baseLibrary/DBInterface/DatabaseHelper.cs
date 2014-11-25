using baseLibrary.Generic;
using FlickerBatch_AlbumRetriever.Model;
using Flickr_UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace baseLibrary.DBInterface
{
    public static class DatabaseHelper
    {
        private static Object concurrencyObj = new object();
        private static String dbProvider = "System.Data.SQLite";
        private static String connectionString = @"Data Source=DBInterface\FlickerConfig.sqllite";
        private static String _loadLocalDuplicatesSQL = @"select  path src,path2 dest, count(1) numPics from 
                                                        (select l1.filename, l1.date_taken, l1.path path, l2.path path2, l1.size from local_data l1, local_data l2 where
                                                        l1.filename = l2.filename and
                                                        l1.date_taken = l2.date_taken and
                                                        l1.size = l2.size and 
                                                        l1.path != l2.path
                                                        order by l1.filename desc)
                                                    group by path,path2;";
        private static String _loadLocalDuplicateImagesSQL = @"select l1.filename, l1.date_taken, l1.path path, l2.path path2, l1.size from local_data l1, local_data l2 where
                                                        l1.filename = l2.filename and
                                                        l1.date_taken = l2.date_taken and
                                                        l1.size = l2.size and 
                                                        l1.path != l2.path and
                                                        l1.path = '{0}' and
                                                        l2.path = '{1}'
                                                        order by l1.filename desc;";

        public static void InitiallizeDataStructure()
        {
            
            ExecuteNonQuery("CREATE TABLE if not exists " + TableNames.MASTER_CONFIG +
                "(config_type TEXT, param TEXT, value TEXT);");

            ExecuteNonQuery("CREATE TABLE if not exists " + TableNames.REMOTE_DATA +
                "(TITLE TEXT, DATE_TAKEN TEXT, DESCRIPTION TEXT, ALBUM TEXT, ID TEXT, PROCESSED TEXT, SYNC_DATE TEXT);");

            ExecuteNonQuery("CREATE TABLE if not exists " + TableNames.LOCAL_DATA +
                "(FILENAME TEXT, DATE_TAKEN TEXT, DESCRIPTION TEXT, PATH TEXT, SIZE INTEGER, PROCESSED TEXT, SYNC_DATE TEXT);");

            ExecuteNonQuery("CREATE TABLE if not exists " + TableNames.FLICKR_ALBUMS +
                "(ID TEXT, NAME TEXT, DATE_CREATED TEXT,NUM_PICS INTEGER, DESCRIPTION TEXT, SYNC_DATE TEXT);");

            ExecuteNonQuery("CREATE TABLE if not exists " + TableNames.JOIN_DATA +
                "(NAME TEXT, DATE_TAKEN TEXT, FLICKER_PATH TEXT, LOCAL_PATH TEXT, COUNT INTEGER);");

            return;
        }

        public static void ExecuteNonQuery(String sql)
        {
            DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                DbCommand cmd = cnn.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                cnn.Close();
            }
            return;

        }

        public static Dictionary<string, string> loadMasterConfigData(string config_type)
        {
            Dictionary<string, string> retConfigData = new Dictionary<string, string>();
            DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                using (DbCommand mycommand = cnn.CreateCommand())
                {

                    mycommand.CommandText = "select * from MASTER_CONFIG where config_type ='" + config_type + "';";
                    DbDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("Parameter: " + reader["param"] + "\tValue: " + reader["value"]);
                        retConfigData.Add(reader["param"].ToString(), reader["value"].ToString());
                    }

                }
                cnn.Close();
            }
            return retConfigData;
        }

        public static void saveConfigData(string config_type, Dictionary<string, string> configData)
        {

            ExecuteNonQuery("delete from MASTER_CONFIG where config_type ='" + config_type + "';");
            foreach (KeyValuePair<string, string> config in configData)
            {
                ExecuteNonQuery("insert into MASTER_CONFIG (config_type, param, value) values ('" + config_type + "','" + config.Key + "','" + config.Value + "');");
            }
            return;
        }


        public static void SaveImageData(List<BaseImageData> pList)
        {
            lock (concurrencyObj)
            {
                //        Console.WriteLine("Got lock: SaveImageData");
                DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);
                using (DbConnection cnn = fact.CreateConnection())
                {
                    cnn.ConnectionString = connectionString;
                    cnn.Open();
                    DbCommand cmd = cnn.CreateCommand();
                    using (var transaction = cnn.BeginTransaction())
                    {
                        foreach (BaseImageData bid in pList)
                        {
                            cmd.CommandText = bid.getInsertStatement();
                            cmd.ExecuteNonQuery();
                            Console.Write(".");

                        }
                        transaction.Commit();
                    }
                    Console.WriteLine("");
                    cnn.Close();
                }
                //      Console.WriteLine("Releasing lock: SaveImageData");

            }
        }
        public static void SaveImageData(BaseImageData bid)
        {
            lock (concurrencyObj)
            {
                ExecuteNonQuery(bid.getInsertStatement());
            }
        }
        public static List<BaseImageData> getPhotosToSave(List<BaseImageData> imageList)
        {
            List<BaseImageData> retList = new List<BaseImageData>();
            DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                using (DbCommand mycommand = cnn.CreateCommand())
                {
                    foreach (BaseImageData image in imageList)
                    {
                        mycommand.CommandText = image.getCheckStatement();
                        DbDataReader reader = mycommand.ExecuteReader();
                        while (reader.Read())
                        {
                            if ((Int64)reader["COUNT"] > 0)
                            {
                            }
                            else
                            {
                                retList.Add(image);
                            }
                        }
                        reader.Close();
                    }

                }
                cnn.Close();
            }
            return retList;
        }

        public static bool ExecuteCheckSQL(string sql)
        {
            Boolean ret = false;
            DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                using (DbCommand mycommand = cnn.CreateCommand())
                {

                    mycommand.CommandText = sql;
                    DbDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        ret = (Int64)reader["COUNT"] > 0 ? true : false;
                    }

                }
                cnn.Close();
            }
            return ret;
        }


        public static List<FlickrAlbumData> LoadFlickerAlbums()
        {
            List<FlickrAlbumData> fadList = new List<FlickrAlbumData>();
            DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                using (DbCommand mycommand = cnn.CreateCommand())
                {

                    mycommand.CommandText = "select * from " + TableNames.FLICKR_ALBUMS + ";";
                    DbDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        String albumid = (String)reader["ID"];
                        String name = (String)reader["NAME"];
                        String date_created = (String)reader["DATE_CREATED"];
                        int num_pics = Convert.ToInt32((Int64)reader["NUM_PICS"]);
                        String desc = (String)reader["DESCRIPTION"];
                        String sync_date = (String)reader["SYNC_DATE"];
                        fadList.Add(new FlickrAlbumData(albumid, name, GenericHelper.DateTimeSQLite(date_created), num_pics, desc, GenericHelper.DateTimeSQLite(sync_date)));
                    }

                }
                cnn.Close();
            }
            return fadList;

        }

        public static void SaveFlickerAlbums(List<FlickrAlbumData> fadList)
        {
            DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    DbCommand cmd = cnn.CreateCommand();
                    foreach (FlickrAlbumData fad in fadList)
                    {
                        cmd.CommandText = fad.getInsertStatement();
                        cmd.ExecuteNonQuery();

                    }
                    transaction.Commit();
                }

                cnn.Close();
            }
        }

        public static void Join()
        {

            DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);

            String getLocalDataSQL = "select * from " + TableNames.LOCAL_DATA + " where FILENAME like '{0}%' and DATE_TAKEN='{1}';";
            String UpdateRemoteDataSQL = "UPDATE " + TableNames.REMOTE_DATA + " SET PROCESSED='Y' where TITLE = '{0}' and DATE_TAKEN='{1}';";
            String UpdateLocalDataSQL = "UPDATE " + TableNames.LOCAL_DATA + "  SET PROCESSED='Y' where FILENAME = '{0}' and DATE_TAKEN='{1}';";
            String InsertSQL = "Insert into " + TableNames.JOIN_DATA + "(NAME,DATE_TAKEN,FLICKER_PATH ,LOCAL_PATH,COUNT ) VALUES('{0}','{1}','{2}','{3}','{4}');";

            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                DbCommand cmd = cnn.CreateCommand();
                var transaction = cnn.BeginTransaction();
                cmd.CommandText = "select * from " + TableNames.REMOTE_DATA + " WHERE PROCESSED='N';";
                int imageCount = 0;
                DbDataReader remoteDataReader = cmd.ExecuteReader();
                while (remoteDataReader.Read())
                {
                    imageCount++;
                    String title = (String)remoteDataReader["TITLE"];
                    String dateTaken = (String)remoteDataReader["DATE_TAKEN"];
                    String flickerPath = (String)remoteDataReader["ALBUM"];
                    String localPath = "";
                    DbCommand cmd2 = cnn.CreateCommand();
                    cmd2.CommandText = String.Format(getLocalDataSQL, title, dateTaken);
                    DbDataReader localDataReader = cmd2.ExecuteReader();
                    int count = 0;
                    while (localDataReader.Read())
                    {
                        count++;
                        localPath = (String)localDataReader["PATH"];
                    }

                    DbCommand cmd3 = cnn.CreateCommand();
                    cmd3.CommandText = String.Format(UpdateRemoteDataSQL, GenericHelper.StringSQLite(title), dateTaken);
                    cmd3.ExecuteNonQuery();
                    cmd3.CommandText = String.Format(UpdateLocalDataSQL, GenericHelper.StringSQLite(title), dateTaken);
                    cmd3.ExecuteNonQuery();
                    cmd3.CommandText = String.Format(InsertSQL, title.Replace("'", "''"), dateTaken, GenericHelper.StringSQLite(flickerPath), GenericHelper.StringSQLite(localPath), count);
                    cmd3.ExecuteNonQuery();
                    Console.WriteLine(imageCount + ". Title: " + title + "\t Count=" + count);
                    if (imageCount % 100 == 0)
                    {
                        Console.WriteLine("Saving....");
                        transaction.Commit();
                        transaction = cnn.BeginTransaction();
                    }
                }
                transaction.Commit();
                Console.WriteLine("Done....");

                cnn.Close();
            }

        }

        public static void removePrefix(string p)
        {
            ExecuteNonQuery(String.Format("update " + TableNames.LOCAL_DATA + " set PATH=substr(PATH,{0}) where PATH like '{1}%'", p.Length + 2, p));
            //ExecuteNonQuery(String.Format("update " + TableNames.REMOTE_DATA + " set ALBUM=substr(ALBUM,{0}) where ALBUM like '{1}%'", p.Length + 2, p));
        }
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

                    mycommand.CommandText = _loadLocalDuplicatesSQL;
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
        public static List<DuplicateImages> loadDuplicateImages(string SourcePath, string DestinationPath)
        {
            List<DuplicateImages> duplicateImgList = new List<DuplicateImages>();
            DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                using (DbCommand mycommand = cnn.CreateCommand())
                {

                    mycommand.CommandText = String.Format(_loadLocalDuplicateImagesSQL, SourcePath, DestinationPath);

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
