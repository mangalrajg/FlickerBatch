using baseLibrary.DBInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace baseLibrary.Model
{
    public static class ConfigModel
    {

        private static String _LocalBasePathKey = "LocalBasePath";
        private static String _ApiKeyKey = "APIKey";
        private static String _SharedSecretKey = "SharedSecret";
        private static String _AccessTokenStrKey = "AccessTokenStr";
        private static String _AuthTokenKey = "AuthToken";
        private static String _LocalTempPathKey = "LocalTempPath";
        private static String _RemoteTempAlbumKey = "RemoteTempAlbum";

        private static Dictionary<string, string> _AuthData { get; set; }
        private static Dictionary<string, string> _LocalData { get; set; }
        private static Dictionary<string, string> _RemoteData { get; set; }

        static ConfigModel()
        {
            _AuthData = DatabaseHelper.LoadMasterConfigData("AUTH");
            _LocalData = DatabaseHelper.LoadMasterConfigData("LOCAL");
            _RemoteData = DatabaseHelper.LoadMasterConfigData("REMOTE");
        }

        public static String LocalBasePath
        {
            get
            {
                if (ConfigModel._LocalData.ContainsKey(_LocalBasePathKey))
                {
                    return ConfigModel._LocalData[_LocalBasePathKey];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ConfigModel._LocalData[_LocalBasePathKey] = value;
            }
        }


        public static String APIKey
        {
            get
            {
                if (ConfigModel._AuthData.ContainsKey(_ApiKeyKey))
                {
                    return ConfigModel._AuthData[_ApiKeyKey];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ConfigModel._AuthData[_ApiKeyKey] = value;
            }
        }

        public static String SharedSecret
        {
            get
            {
                if (ConfigModel._AuthData.ContainsKey(_SharedSecretKey))
                {
                    return ConfigModel._AuthData[_SharedSecretKey];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ConfigModel._AuthData[_SharedSecretKey] = value;
            }
        }

        public static String AccessTokenStr
        {
            get
            {
                if (ConfigModel._AuthData.ContainsKey(_AccessTokenStrKey))
                {
                    return ConfigModel._AuthData[_AccessTokenStrKey];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ConfigModel._AuthData[_AccessTokenStrKey] = value;
            }
        }

        public static String AuthToken
        {
            get
            {
                if (ConfigModel._AuthData.ContainsKey(_AuthTokenKey))
                {
                    return ConfigModel._AuthData[_AuthTokenKey];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ConfigModel._AuthData[_AuthTokenKey] = value;
            }
        }

        //public static String AuthToken
        //{
        //    get
        //    {
        //        if (ConfigModel._AuthData.ContainsKey(_AuthTokenKey))
        //        {
        //            return ConfigModel._AuthData[_AuthTokenKey];
        //        }
        //        else
        //        {
        //            return "";
        //        }
        //    }
        //    set
        //    {
        //        ConfigModel._AuthData[_AuthTokenKey] = value;
        //    }
        //}

        public static String LocalTempPath
        {
            get
            {
                if (ConfigModel._LocalData.ContainsKey(_LocalTempPathKey))
                {
                    return ConfigModel._LocalData[_LocalTempPathKey];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ConfigModel._LocalData[_LocalTempPathKey] = value;
            }
        }

        public static String RemoteTempAlbum
        {
            get
            {
                if (ConfigModel._RemoteData.ContainsKey(_RemoteTempAlbumKey))
                {
                    return ConfigModel._RemoteData[_RemoteTempAlbumKey];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ConfigModel._RemoteData[_RemoteTempAlbumKey] = value;
            }
        }

        public static void Save()
        {
            DatabaseHelper.SaveConfigData("AUTH", _AuthData);
            DatabaseHelper.SaveConfigData("LOCAL", _LocalData);
            DatabaseHelper.SaveConfigData("REMOTE", _RemoteData);

        }

    }
}
