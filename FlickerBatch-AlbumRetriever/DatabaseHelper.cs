using FlickerBatch_AlbumRetriever.ImageData;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlickerBatch_AlbumRetriever
{
    public static class DatabaseHelper
    {
        public static String MASTER_CONFIG = "MASTER_CONFIG";
        public static String REMOTE_DATA = "REMOTE_DATA";
        public static String LOCAL_DATA = "LOCAL_DATA";
        public static String FLICKR_ALBUMS = "FLICKR_ALBUMS";
        public static String JOIN = "JOIN_DATA";

        private static Object concurrencyObj = new object();
        public static void InitiallizeDataStructure()
        {
            ExecuteNonQuery("CREATE TABLE if not exists " + MASTER_CONFIG +
                "(config_type TEXT, param TEXT, value TEXT);");

            ExecuteNonQuery("CREATE TABLE if not exists " + REMOTE_DATA +
                "(TITLE TEXT, DATE_TAKEN TEXT, DESCRIPTION TEXT, ALBUM TEXT, ID TEXT, PROCESSED TEXT);");

            ExecuteNonQuery("CREATE TABLE if not exists " + LOCAL_DATA +
                "(FILENAME TEXT, DATE_TAKEN TEXT, DESCRIPTION TEXT, PATH TEXT, SIZE INTEGER, PROCESSED TEXT);");

            ExecuteNonQuery("CREATE TABLE if not exists " + FLICKR_ALBUMS +
                "(ID TEXT, NAME TEXT, DATE_CREATED TEXT, DESCRIPTION TEXT);");

            ExecuteNonQuery("CREATE TABLE if not exists " + JOIN +
                "(NAME TEXT, DATE_TAKEN TEXT, FLICKER_PATH TEXT, LOCAL_PATH TEXT, COUNT INTEGER);");

            return;
        }

        public static void ExecuteNonQuery(String sql)
        {
            DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = "Data Source=FlickerConfig.sqllite";
                cnn.Open();
                DbCommand cmd = cnn.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                cnn.Close();
            }
            return;

        }

        public static Dictionary<string, string> loadConfigData(string config_type)
        {
            Dictionary<string, string> retConfigData = new Dictionary<string, string>();
            DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = "Data Source=FlickerConfig.sqllite";
                cnn.Open();
                using (SQLiteCommand mycommand = new SQLiteCommand((SQLiteConnection)cnn))
                {

                    mycommand.CommandText = "select * from MASTER_CONFIG where config_type ='" + config_type + "';";
                    SQLiteDataReader reader = mycommand.ExecuteReader();
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


        internal static void SaveImageData(List<BaseImageData> pList)
        {
            DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = "Data Source=FlickerConfig.sqllite";
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
        }
        internal static void SaveImageData(BaseImageData bid)
        {
            lock (concurrencyObj)
            {
                ExecuteNonQuery(bid.getInsertStatement());
            }
        }

        internal static bool IsImageInDB(String photoId)
        {
            bool isImageInDb = false;
            lock (concurrencyObj)
            {
                isImageInDb = ExecuteCheckSQL(String.Format(RemoteImageData.CheckSQL, photoId));
            }
            return isImageInDb;
        }


        public static bool ExecuteCheckSQL(string sql)
        {
            Boolean ret = false;
            DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = "Data Source=FlickerConfig.sqllite";
                cnn.Open();
                using (SQLiteCommand mycommand = new SQLiteCommand((SQLiteConnection)cnn))
                {

                    mycommand.CommandText = sql;
                    SQLiteDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        ret = (Int64)reader["COUNT"] > 0 ? true : false;
                    }

                }
                cnn.Close();
            }
            return ret;
        }


        internal static List<FlickrAlbumData> LoadFlickerAlbums()
        {
            List<FlickrAlbumData> fadList = new List<FlickrAlbumData>();
            DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = "Data Source=FlickerConfig.sqllite";
                cnn.Open();
                using (SQLiteCommand mycommand = new SQLiteCommand((SQLiteConnection)cnn))
                {

                    mycommand.CommandText = "select * from " + FLICKR_ALBUMS + ";";
                    SQLiteDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        var albumid = reader.GetString(0);
                        var name = reader.GetString(1);
                        var date_created = reader.GetString(2);
                        var desc = reader.GetString(3);
                        DateTime dt = DateTime.Parse(date_created);

                        fadList.Add(new FlickrAlbumData(albumid, name, dt, desc));
                    }

                }
                cnn.Close();
            }
            return fadList;

        }

        internal static void SaveFlickerAlbums(List<FlickrAlbumData> fadList)
        {
            DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = "Data Source=FlickerConfig.sqllite";
                cnn.Open();
                DbCommand cmd = cnn.CreateCommand();
                foreach (FlickrAlbumData fad in fadList)
                {
                    cmd.CommandText = fad.getInsertStatement();
                    cmd.ExecuteNonQuery();

                }
                cnn.Close();
            }
        }

        internal static void Join()
        {
            DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");

            String getLocalDataSQL = "select * from " + LOCAL_DATA + " where FILENAME like '{0}%' and DATE_TAKEN='{1}';";
            String UpdateRemoteDataSQL = "UPDATE " + REMOTE_DATA + " SET PROCESSED='Y' where TITLE = '{0}' and DATE_TAKEN='{1}';";
            String UpdateLocalDataSQL = "UPDATE " + LOCAL_DATA + "  SET PROCESSED='Y' where FILENAME = '{0}' and DATE_TAKEN='{1}';";
            String InsertSQL = "Insert into " + DatabaseHelper.JOIN + "(NAME,DATE_TAKEN,FLICKER_PATH ,LOCAL_PATH,COUNT ) VALUES('{0}','{1}','{2}','{3}','{4}');";

            //String checkRemoteSQL = "select count(*) COUNT from " + LOCAL_DATA + " where FILENAME = '{0}' and DATE_TAKEN='{1}';";
            //String getLocalPathSQL = "select PATH from " + LOCAL_DATA + " where FILENAME = '{0}' and DATE_TAKEN='{1}';";
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = "Data Source=FlickerConfig.sqllite";
                cnn.Open();
                
                DbCommand cmd = cnn.CreateCommand();
                var transaction = cnn.BeginTransaction();
                {

                    cmd.CommandText = "select * from " + REMOTE_DATA + " WHERE PROCESSED='N';";
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
                        cmd3.CommandText = String.Format(UpdateRemoteDataSQL, title.Replace("'", "''"), dateTaken);
                        cmd3.ExecuteNonQuery();
                        cmd3.CommandText = String.Format(UpdateLocalDataSQL, title.Replace("'", "''"), dateTaken);
                        cmd3.ExecuteNonQuery();
                        cmd3.CommandText = String.Format(InsertSQL, title.Replace("'", "''"), dateTaken, flickerPath.Replace("'", "''"), localPath.Replace("'", "''"), count);
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
          
                }
                cnn.Close();
            }

        }
    }
}
