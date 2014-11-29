using baseLibrary.Generic;
using baseLibrary.Model;
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
        //        private static String connectionString = @"Data Source=DBInterface\FlickerConfig.sqllite";
        private static String connectionString = @"Data Source=..\FlickerConfig.sqllite";

        #region Static SQLs
        private static String _loadLocalDuplicatesSQL = @"select  folder src,path2 dest, count(1) numPics from 
                                                        (select l1.filename, l1.date_taken, l1.folder folder, l2.folder path2, l1.size from local_data l1, local_data l2 where
                                                        l1.filename = l2.filename and
                                                        l1.date_taken = l2.date_taken and
                                                        l1.size = l2.size and 
                                                        l1.folder != l2.folder
                                                        order by l1.filename desc)
                                                    group by folder,path2;";
        private static String _loadRemoteDuplicatesSQL = @"select  folder src,path2 dest, count(1) numPics from 
                                                        (select l1.TITLE, l1.date_taken, l1.ALBUM folder, l2.ALBUM path2, l1.id from remote_data l1, remote_data l2 where
                                                        l1.TITLE = l2.TITLE and
                                                        l1.date_taken = l2.date_taken and
                                                        l1.ALBUM != l2.ALBUM
                                                        order by l1.ALBUM desc)
                                                    group by folder,path2;";

        private static String _loadRemoteDuplicateImagesSQL = @"select l1.TITLE, l1.date_taken, l1.ALBUM folder, l2.ALBUM path2, l1.ID from remote_data l1, remote_data l2 where
                                                        l1.TITLE = l2.TITLE and
                                                        l1.date_taken = l2.date_taken and
                                                        l1.ALBUM != l2.ALBUM and
                                                        l1.ALBUM = '{0}' and
                                                        l2.ALBUM = '{1}'
                                                        order by l1.TITLE desc;";

        private static String _loadLocalDuplicateImagesSQL = @"select l1.filename, l1.date_taken, l1.folder folder, l2.folder path2, l1.size from local_data l1, local_data l2 where
                                                        l1.filename = l2.filename and
                                                        l1.date_taken = l2.date_taken and
                                                        l1.size = l2.size and 
                                                        l1.folder != l2.folder and
                                                        l1.folder = '{0}' and
                                                        l2.folder = '{1}'
                                                        order by l1.filename desc;";

        private static String _createTable = "CREATE TABLE if not exists ";
        private static String _CreateMasterConfigTable = _createTable + TableNames.MASTER_CONFIG + "(CONFIG_TYPE TEXT, PARAM TEXT, VALUE TEXT);";
        private static String _CreateFlickrAlbumsTable = _createTable + TableNames.FLICKR_ALBUMS + "(ID TEXT, NAME TEXT, DATE_CREATED TEXT,NUM_PICS INTEGER, DESCRIPTION TEXT, SYNC_DATE TEXT);";
        private static String _CreateRemoteDataTable = _createTable + TableNames.REMOTE_DATA + "(TITLE TEXT, DATE_TAKEN TEXT, DESCRIPTION TEXT, ALBUM TEXT, ID TEXT, PROCESSED TEXT, SYNC_DATE TEXT);";
        private static String _CreateLocalDataTable = _createTable + TableNames.LOCAL_DATA + "(FILENAME TEXT, DATE_TAKEN TEXT, DESCRIPTION TEXT, PATH TEXT, SIZE INTEGER, PROCESSED TEXT, SYNC_DATE TEXT);";
        private static String _CreateJoinDataTable = _createTable + TableNames.JOIN_DATA + "(NAME TEXT, DATE_TAKEN TEXT, FLICKER_PATH TEXT, LOCAL_PATH TEXT, COUNT INTEGER);";
        #endregion

        public static void InitiallizeDataStructure()
        {
            ExecuteNonQuery(_CreateMasterConfigTable);
            ExecuteNonQuery(_CreateRemoteDataTable);
            ExecuteNonQuery(_CreateLocalDataTable);
            ExecuteNonQuery(_CreateFlickrAlbumsTable);
            ExecuteNonQuery(_CreateJoinDataTable);
            return;
        }

        private static void ExecuteNonQuery(String sql)
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
        private static bool ExecuteCheckSQL(string sql)
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

        public static Dictionary<string, string> LoadMasterConfigData(string config_type)
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
        public static List<DuplicateImageGroupData> LoadRemoteDuplicateImageData()
        {
            List<DuplicateImageGroupData> duplicateImgList = new List<DuplicateImageGroupData>();
            DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                using (DbCommand mycommand = cnn.CreateCommand())
                {

                    mycommand.CommandText = _loadRemoteDuplicatesSQL;
                    DbDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        String src = reader.GetString(0);
                        String dest = reader.GetString(1);
                        Int64 count = reader.GetInt64(2);
                        duplicateImgList.Add(new DuplicateImageGroupData(src, dest, Convert.ToInt32(count), Mode.REMOTE));
                    }

                }
                cnn.Close();
            }
            return duplicateImgList;
        }
        public static List<DuplicateImageGroupData> LoadLocalDuplicateImageData()
        {
            List<DuplicateImageGroupData> duplicateImgList = new List<DuplicateImageGroupData>();
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
                        duplicateImgList.Add(new DuplicateImageGroupData(src, dest, Convert.ToInt32(count), Mode.LOCAL));
                    }

                }
                cnn.Close();
            }
            return duplicateImgList;
        }
        public static List<DuplicateImageData> LoadLocalDuplicateImages(string SourcePath, string DestinationPath)
        {
            List<DuplicateImageData> duplicateImgList = new List<DuplicateImageData>();
            DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                using (DbCommand mycommand = cnn.CreateCommand())
                {

                    mycommand.CommandText = String.Format(_loadLocalDuplicateImagesSQL, GenericHelper.StringSQLite(SourcePath), GenericHelper.StringSQLite(DestinationPath));

                    DbDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        String filename = reader.GetString(0);
                        String date_taken = reader.GetString(1);
                        String src = reader.GetString(2);
                        String dest = reader.GetString(3);
                        duplicateImgList.Add(new DuplicateImageData(filename, date_taken, src, dest));
                    }
                }
                cnn.Close();
            }
            return duplicateImgList;
        }
        public static List<DuplicateImageData> LoadRemoteDuplicateImages(string SourcePath, string DestinationPath)
        {
            List<DuplicateImageData> duplicateImgList = new List<DuplicateImageData>();
            DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                using (DbCommand mycommand = cnn.CreateCommand())
                {

                    mycommand.CommandText = String.Format(_loadRemoteDuplicateImagesSQL, GenericHelper.StringSQLite(SourcePath), GenericHelper.StringSQLite(DestinationPath));

                    DbDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        String filename = reader.GetString(0);
                        String date_taken = reader.GetString(1);
                        String src = reader.GetString(2);
                        String dest = reader.GetString(3);
                        duplicateImgList.Add(new DuplicateImageData(filename, date_taken, src, dest));
                    }

                }
                cnn.Close();
            }
            return duplicateImgList;
        }
        public static List<BaseImageData> GetPhotosToSave(List<BaseImageData> imageList)
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
                        mycommand.CommandText = image.CheckSQL;
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

        public static List<GenericAlbumData> LoadAlbumsToUpload()
        {
            String sql = "SELECT PATH, count(1) COUNT FROM LOCAL_DATA  WHERE UPPER(FILENAME|| DATE_TAKEN) NOT IN (SELECT UPPER(TITLE||'.jpg'|| DATE_TAKEN) FROM REMOTE_DATA ) group by PATH;";

            List<GenericAlbumData> localImageList = new List<GenericAlbumData>();
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
                        String path = (String)reader["PATH"];
                        long count = (long)reader["COUNT"];
                        localImageList.Add(new GenericAlbumData(path, count));
                    }

                }
                cnn.Close();
            }
            return localImageList;

        }

        public static List<LocalImageData> LoadImagesToUpload(String path)
        {
            String sql = String.Format("SELECT * FROM " + TableNames.LOCAL_DATA + " WHERE UPPER(FILENAME|| DATE_TAKEN) NOT IN (SELECT UPPER(TITLE||'.jpg'|| DATE_TAKEN) FROM " + TableNames.REMOTE_DATA + ") AND PATH='{0}';",GenericHelper.StringSQLite(path));
            List<LocalImageData> localImageList = new List<LocalImageData>();
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
                        String fileName = (String)reader["FILENAME"];
                        String date_taken = (String)reader["DATE_TAKEN"];
                        String description = (String)reader["DESCRIPTION"];
                        long size = (Int64)reader["SIZE"];
                        String sync_date = (String)reader["SYNC_DATE"];
                        localImageList.Add(new LocalImageData(fileName, GenericHelper.DateTimeSQLite(date_taken), description, path,size));
                    }

                }
                cnn.Close();
            }
            return localImageList;

        }

        public static void SaveConfigData(string config_type, Dictionary<string, string> configData)
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
                            cmd.CommandText = bid.InsertSQL;
                            cmd.ExecuteNonQuery();
                            Console.Write(".");

                        }
                        transaction.Commit();
                    }
                    Console.WriteLine("");
                    cnn.Close();
                }
            }
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
                        cmd.CommandText = fad.InsertSQL;
                        cmd.ExecuteNonQuery();

                    }
                    transaction.Commit();
                }
                cnn.Close();
            }
        }
        public static void SaveFlickerAlbum(FlickrAlbumData fad)
        {
            ExecuteNonQuery(fad.InsertSQL);
        }
        public static void SaveJoinData()
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

        public static void RemovePrefix(string albumName)
        {
            ExecuteNonQuery(String.Format("update " + TableNames.LOCAL_DATA + " set PATH=substr(PATH,{0}) where PATH like '{1}%'", albumName.Length + 2, albumName));
            //ExecuteNonQuery(String.Format("update " + TableNames.REMOTE_DATA + " set ALBUM=substr(ALBUM,{0}) where ALBUM like '{1}%'", albumName.Length + 2, albumName));
        }


        public static string GetAlbumID(string albumName)
        {
            String ret = null;
            DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                using (DbCommand mycommand = cnn.CreateCommand())
                {

                    mycommand.CommandText = String.Format("SELECT ID FROM " + TableNames.FLICKR_ALBUMS + " WHERE NAME='{0}';", GenericHelper.StringSQLite(albumName));
                    DbDataReader reader = mycommand.ExecuteReader();
                    reader.Read();
                    ret = reader.GetString(0);
                }
                cnn.Close();
            }
            return ret;

        }

        public static void DeleteLocalImageData(string baseDir)
        {
            ExecuteNonQuery(String.Format("DELETE from LOCAL_DATA where PATH='{0}';", GenericHelper.StringSQLite(baseDir)));
        }

        public static void DeleteRemoteImageData(string albumName)
        {
            Console.WriteLine("Delete Album: " + albumName + " from database");
            ExecuteNonQuery(String.Format("DELETE from REMOTE_DATA where ALBUM='{0}';", GenericHelper.StringSQLite(albumName)));
        }

        internal static void DeleteFlickerAlbums(List<FlickrAlbumData> fadList)
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
                        cmd.CommandText = fad.DeleteSQL;
                        cmd.ExecuteNonQuery();

                    }
                    transaction.Commit();
                }
                cnn.Close();
            }
        }
        public static void DeleteAllFlickerAlbums()
        {
            ExecuteNonQuery(FlickrAlbumData.DeleteAllSQL);
        }
    }

}
