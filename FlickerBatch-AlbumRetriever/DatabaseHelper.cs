﻿using FlickerBatch_AlbumRetriever.ImageData;
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
        private static Object concurrencyObj = new object();
        public static void InitiallizeDataStructure()
        {
            ExecuteNonQuery("CREATE TABLE if not exists MASTER_CONFIG(config_type TEXT, param TEXT, value TEXT);");
            ExecuteNonQuery("CREATE TABLE if not exists REMOTE_DATA " +
                "(TITLE TEXT, DATE_TAKEN TEXT, DESCRIPTION TEXT, ALBUM TEXT, ID TEXT);");
            ExecuteNonQuery("CREATE TABLE if not exists LOCAL_DATA " +
                "(FILENAME TEXT, DATE_TAKEN TEXT, DESCRIPTION TEXT, PATH TEXT, SIZE INTEGER);");
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
                foreach (BaseImageData bid in pList)
                {
                    cmd.CommandText = bid.getInsertStatement();
                    cmd.ExecuteNonQuery();

                }
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
    }
}
