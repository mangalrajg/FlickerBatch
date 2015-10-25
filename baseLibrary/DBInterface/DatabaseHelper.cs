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


        static DatabaseHelper()
        {
            InitiallizeDataStructure();
        }

        private static void InitiallizeDataStructure()
        {
            ExecuteNonQuery(SQLRepository._CreateMasterConfigTable);
            ExecuteNonQuery(SQLRepository._CreateRemoteDataTable);
            ExecuteNonQuery(SQLRepository._CreateLocalDataTable);
            ExecuteNonQuery(SQLRepository._CreateFlickrAlbumsTable);
            ExecuteNonQuery(SQLRepository._CreateJoinDataTable);
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
        public static void SaveConfigData(string config_type, Dictionary<string, string> configData)
        {

            ExecuteNonQuery("delete from MASTER_CONFIG where config_type ='" + config_type + "';");
            foreach (KeyValuePair<string, string> config in configData)
            {
                ExecuteNonQuery("insert into MASTER_CONFIG (config_type, param, value) values ('" + config_type + "','" + config.Key + "','" + config.Value + "');");
            }
            return;
        }

        public static List<DuplicateImageGroupData> LoadRemoteDuplicateImageData()
        {
            return _LoadDuplicateImageGroupData(SQLRepository._loadRemoteDuplicatesSQL, Mode.REMOTE);
        }
        public static List<DuplicateImageGroupData> LoadLocalDuplicateImageData()
        {
            return _LoadDuplicateImageGroupData(SQLRepository._loadLocalDuplicatesSQL, Mode.LOCAL);
        }
        public static List<DuplicateImageData> LoadLocalDuplicateImages(string SourcePath, string DestinationPath)
        {
            String sql = String.Format(SQLRepository._loadLocalDuplicateImagesSQL, GenericHelper.StringSQLite(SourcePath), GenericHelper.StringSQLite(DestinationPath));
            return _LoadDuplicateImages(sql);
        }
        public static List<DuplicateImageData> LoadRemoteDuplicateImages(string SourcePath, string DestinationPath)
        {
            String sql = String.Format(SQLRepository._loadRemoteDuplicateImagesSQL, GenericHelper.StringSQLite(SourcePath), GenericHelper.StringSQLite(DestinationPath));
            return _LoadDuplicateImages(sql);
        }

        #region Flicker Albums table
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

        public static void UpdateFlickerAlbum(FlickrAlbumData fad)
        {
            lock (DatabaseHelper.concurrencyObj)
            {
                ExecuteNonQuery(fad.DeleteSQL);
                ExecuteNonQuery(fad.InsertSQL);
            }
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

                    mycommand.CommandText = "select * from " + SQLRepository.FLICKR_ALBUMS + ";";
                    DbDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        FlickrAlbumData fad = GetFlickrAlbumDataFromReader(reader);
                        fadList.Add(fad);
                    }

                }
                cnn.Close();
            }
            return fadList;

        }
        private static FlickrAlbumData GetFlickrAlbumDataFromReader(DbDataReader reader)
        {
            String albumid = (String)reader["ID"];
            String name = (String)reader["NAME"];
            String date_created = (String)reader["DATE_CREATED"];
            int num_pics = 0;
            if (reader["NUM_PICS"] != DBNull.Value)
            {
                num_pics = Convert.ToInt32((Int64)reader["NUM_PICS"]);
            }
            int num_vid = 0;

            if (reader["NUM_VID"] != DBNull.Value)
            {
                num_vid = Convert.ToInt32((Int64)reader["NUM_VID"]);
            }
            int actual_num_pics = 0;
            int actual_num_vid = 0;

            object o = reader["ACTUAL_NUM_PICS"];
            if (o != DBNull.Value)
            {
                actual_num_pics = Convert.ToInt32((Int64)o);
            }

            object o2 = reader["ACTUAL_NUM_VID"];
            if (o2 != DBNull.Value)
            {
                actual_num_vid = Convert.ToInt32((Int64)o2);
            }

            String desc = (String)reader["DESCRIPTION"];
            String sync_date = (String)reader["SYNC_DATE"];
            return new FlickrAlbumData(albumid, name, GenericHelper.DateTimeSQLite(date_created), desc, GenericHelper.DateTimeSQLite(sync_date), num_pics, actual_num_pics, num_vid, actual_num_vid);
        }
        public static void SaveFlickrAlbums(List<FlickrAlbumData> fadList)
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
        public static void DeleteFlickerAlbums(List<FlickrAlbumData> fadList)
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
        public static void DeleteAllFlickrAlbums()
        {
            ExecuteNonQuery(FlickrAlbumData.DeleteAllSQL);
        }

        #endregion

        public static List<GenericAlbumData> LoadAlbumsToUpload()
        {
            String sql = @"SELECT PATH, COUNT(1) COUNT FROM LOCAL_DATA  WHERE UPPER(FILENAME|| DATE_TAKEN) NOT IN (SELECT UPPER(TITLE|| DATE_TAKEN) FROM REMOTE_DATA ) AND DATE_TAKEN != 20000101000000 group by PATH ORDER BY COUNT(1) ;";
            return _LoadLocalAlbums(sql);
        }
        public static List<GenericAlbumData> LoadLocalAlbums()
        {
            String sql = @"SELECT PATH, count(1) COUNT FROM LOCAL_DATA  group by PATH;";
            return _LoadLocalAlbums(sql);

        }
        public static List<GenericAlbumData> LoadRemoteAlbums()
        {
            String sql = @"SELECT ALBUM PATH, count(1) COUNT FROM REMOTE_DATA  group by ALBUM;";
            return _LoadLocalAlbums(sql);

        }

        public static List<LocalImageData> LoadImagesToUpload(String path)
        {
            String sql = String.Format(@"SELECT * FROM LOCAL_DATA WHERE UPPER(FILENAME|| DATE_TAKEN) NOT IN (SELECT UPPER(TITLE|| DATE_TAKEN) FROM REMOTE_DATA ) AND PATH='{0}' AND DATE_TAKEN != 20000101000000 ;", GenericHelper.StringSQLite(path));
            return _LoadLocalImageData(sql);
        }
        public static List<LocalImageData> LoadLocalImageData(string baseDir)
        {
            String sql = String.Format("SELECT * FROM " + SQLRepository.LOCAL_DATA + @" WHERE PATH='{0}';", GenericHelper.StringSQLite(baseDir));
            return _LoadLocalImageData(sql);
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
        public static void SaveJoinData()
        {

            DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);

            String getLocalDataSQL = "select * from " + SQLRepository.LOCAL_DATA + " where FILENAME like '{0}%' and DATE_TAKEN='{1}';";
            String UpdateRemoteDataSQL = "UPDATE " + SQLRepository.REMOTE_DATA + " SET PROCESSED='Y' where TITLE = '{0}' and DATE_TAKEN='{1}';";
            String UpdateLocalDataSQL = "UPDATE " + SQLRepository.LOCAL_DATA + "  SET PROCESSED='Y' where FILENAME = '{0}' and DATE_TAKEN='{1}';";
            String InsertSQL = "Insert into " + SQLRepository.JOIN_DATA + "(NAME,DATE_TAKEN,FLICKER_PATH ,LOCAL_PATH,COUNT ) VALUES('{0}','{1}','{2}','{3}','{4}');";

            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                DbCommand cmd = cnn.CreateCommand();
                var transaction = cnn.BeginTransaction();
                cmd.CommandText = "select * from " + SQLRepository.REMOTE_DATA + " WHERE PROCESSED='N';";
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
            ExecuteNonQuery(String.Format("update " + SQLRepository.LOCAL_DATA + " set PATH=substr(PATH,{0}) where PATH like '{1}%'", albumName.Length + 2, albumName));
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

                    mycommand.CommandText = String.Format("SELECT ID FROM " + SQLRepository.FLICKR_ALBUMS + " WHERE NAME='{0}';", GenericHelper.StringSQLite(albumName));
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
            ExecuteNonQuery(String.Format("DELETE from LOCAL_DATA where PATH like'{0}%';", GenericHelper.StringSQLite(baseDir)));
        }

        public static void DeleteRemoteImageData(string albumName)
        {
            Console.WriteLine("Delete Album: " + albumName + " from database");
            ExecuteNonQuery(String.Format("DELETE from REMOTE_DATA where ALBUM='{0}';", GenericHelper.StringSQLite(albumName)));
        }

        public static void DeleteAllRemoteImageData()
        {
            Console.WriteLine("Delete Table: " + SQLRepository.REMOTE_DATA + " from database");
            ExecuteNonQuery(String.Format("DELETE from REMOTE_DATA ;"));
        }

        private static List<RemoteImageData> _LoadRemoteImageData(String sql)
        {
            List<RemoteImageData> remoteImageList = new List<RemoteImageData>();
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
                        RemoteImageData rid = GetRemoteImageDataFromReader(reader);
                        remoteImageList.Add(rid);
                    }

                }
                cnn.Close();
            }
            return remoteImageList;
        }

        private static RemoteImageData GetRemoteImageDataFromReader(DbDataReader reader)
        {
            String fileName = (String)reader["TITLE"];
            String date_taken = (String)reader["DATE_TAKEN"];
            String description = (String)reader["DESCRIPTION"];
            String id = (String)reader["ID"];
            String sync_date = (String)reader["SYNC_DATE"];
            String album = (String)reader["ALBUM"];
            String media = (String)reader["MEDIA"];
            return new RemoteImageData(album, id, fileName, GenericHelper.DateTimeSQLite(date_taken), description, media);
        }
        private static LocalImageData GetLocalImageDataFromReader(DbDataReader reader)
        {
            String fileName = (String)reader["FILENAME"];
            String date_taken = (String)reader["DATE_TAKEN"];
            String description = (String)reader["DESCRIPTION"];
            long size = (Int64)reader["SIZE"];
            String sync_date = (String)reader["SYNC_DATE"];
            String path = (String)reader["PATH"];
            return new LocalImageData(fileName, GenericHelper.DateTimeSQLite(date_taken), description, path, size);
        }
        private static List<LocalImageData> _LoadLocalImageData(String sql)
        {
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
                        LocalImageData lid = GetLocalImageDataFromReader(reader);
                        localImageList.Add(lid);
                    }

                }
                cnn.Close();
            }
            return localImageList;
        }
        private static List<GenericAlbumData> _LoadLocalAlbums(String sql)
        {

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
                        //TODO Video count
                        localImageList.Add(new GenericAlbumData(path, count, 0));
                    }

                }
                cnn.Close();
            }
            return localImageList;

        }
        public static List<DuplicateImageGroupData> _LoadDuplicateImageGroupData(String sql, Mode mode)
        {
            List<DuplicateImageGroupData> duplicateImgList = new List<DuplicateImageGroupData>();
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
                        String src = reader.GetString(0);
                        String dest = reader.GetString(1);
                        Int64 count = reader.GetInt64(2);
                        duplicateImgList.Add(new DuplicateImageGroupData(src, dest, Convert.ToInt32(count), mode));
                    }

                }
                cnn.Close();
            }
            return duplicateImgList;
        }
        public static List<DuplicateImageData> _LoadDuplicateImages(String sql)
        {
            List<DuplicateImageData> duplicateImgList = new List<DuplicateImageData>();
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

        public static List<DuplicateAlbumData> LoadDuplicateAlbums()
        {
            List<DuplicateAlbumData> remoteImageList = new List<DuplicateAlbumData>();
            DbProviderFactory fact = DbProviderFactories.GetFactory(dbProvider);
            using (DbConnection cnn = fact.CreateConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                using (DbCommand mycommand = cnn.CreateCommand())
                {
                    mycommand.CommandText = SQLRepository._loadDuplicateAlbumsSQL;
                    DbDataReader reader = mycommand.ExecuteReader();
                    while (reader.Read())
                    {
                        String albumName = reader.GetString(0);
                        DuplicateAlbumData rid = new DuplicateAlbumData(albumName);
                        remoteImageList.Add(rid);
                    }

                }
                foreach (DuplicateAlbumData rid in remoteImageList)
                {
                    using (DbCommand mycommand = cnn.CreateCommand())
                    {
                        mycommand.CommandText = mycommand.CommandText = String.Format("SELECT * FROM " + SQLRepository.FLICKR_ALBUMS + " WHERE NAME='{0}';", GenericHelper.StringSQLite(rid.AlbumName));
                        DbDataReader reader = mycommand.ExecuteReader();
                        while (reader.Read())
                        {
                            FlickrAlbumData fad = GetFlickrAlbumDataFromReader(reader);
                            rid.Albums.Add(fad);
                        }
                    }
                }
                cnn.Close();
            }
            return remoteImageList;
        }
  
        public static List<RemoteImageData> LoadFilesWithoutExtention(String albumName)
        {
            String sql = String.Format(@"select * from REMOTE_DATA r where album='{0}' and title not like '%.%' 
order by substr(title, 0,4);", GenericHelper.StringSQLite(albumName));
            return _LoadRemoteImageData(sql);
        }

        public static void DeleteRemoteImageData(List<RemoteImageData> rlist)
        {
            throw new NotImplementedException();
        }

        public static List<GenericAlbumData> AlbumsWithFilesWithoutExtention()
        {
            String sql =
@"select album PATH,count(1) COUNT from REMOTE_DATA r where title not like '%.%' group by album;";
            return _LoadLocalAlbums(sql);
        }

        public static List<RemoteImageData> LoadImagesToFixDate()
        {
            String sql = @"select * from REMOTE_DATA where date_taken > '20141101000000' order by date_taken";
            return _LoadRemoteImageData(sql);
        }

        public static List<RemoteImageData> LoadRemoteImageData(String albumName)
        {
            String sql = String.Format(@"select * from REMOTE_DATA where ALBUM='{0}'", GenericHelper.StringSQLite( albumName)); ;
            return _LoadRemoteImageData(sql);
        }
        public static void DeleteImageData(List<BaseImageData> rlist)
        {
            foreach (BaseImageData rid in rlist)
            {
                ExecuteNonQuery(rid.DeleteSQL);
            }
        }

        public static List<DuplicateImageData> LoadImagesToSyncronise(String sourcePath, String destinationPath)
        {
            //TODO
            String sql = @"select rd.title,rd.date_taken, rd.album, ld.path 
                        from local_DATA ld, remoTE_DATA rd
                        where
                        upper(ld.filename) = upper(rd.title) and
                        ld.date_taken = rd.date_taken and
                        ld.path != rd.album and
                        rd.album = '{0}' and
                        ld.path = '{1}' and
                        ld.date_taken != '20000101000000';";
            return _LoadDuplicateImages(string.Format(sql, GenericHelper.StringSQLite(sourcePath), GenericHelper.StringSQLite(destinationPath)));
        }
        public static List<DuplicateImageGroupData> LoadImageGroupsToSyncronise()
        {
            String sql = @"
                        select rd.album, ld.path, count(1) from local_DATA ld, remoTE_DATA rd
                        where
                        upper(ld.filename) = upper(rd.title) and
                        ld.date_taken = rd.date_taken and
                        ld.path != rd.album
                        and ld.date_taken != '20000101000000'
                        group by rd.album, ld.path;";
            return _LoadDuplicateImageGroupData(sql, Mode.MIXED);
        }
    }

}
