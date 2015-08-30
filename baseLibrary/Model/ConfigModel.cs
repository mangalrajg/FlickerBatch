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
        private static String _ApiKeyKey = "APIKey";
        private static String _SharedSecretKey = "SharedSecret";
        private static String _AccessTokenStrKey = "AccessTokenStr";
        private static String _AuthTokenKey = "AuthToken";

        private static String _LocalTempPathKey = "LocalTempPath";
        private static String _LocalBasePathKey = "LocalBasePath";

        private static String _RemoteTempAlbumKey = "RemoteTempAlbum";

        private static Dictionary<string, string> _AuthData { get; set; }
        private static Dictionary<string, string> _LocalData { get; set; }
        private static Dictionary<string, string> _RemoteData { get; set; }

        static ConfigModel()
        {
            Reload();
        }

        public static void Reload()
        {
            _AuthData = DatabaseHelper.LoadMasterConfigData("AUTH");
            _LocalData = DatabaseHelper.LoadMasterConfigData("LOCAL");
            _RemoteData = DatabaseHelper.LoadMasterConfigData("REMOTE");
        }
        public static void Save()
        {
            DatabaseHelper.SaveConfigData("AUTH", _AuthData);
            DatabaseHelper.SaveConfigData("LOCAL", _LocalData);
            DatabaseHelper.SaveConfigData("REMOTE", _RemoteData);
        }

        public static String LocalBasePath
        {
            set { _LocalData[_LocalBasePathKey] = value; }
            get
            {
                if (_LocalData.ContainsKey(_LocalBasePathKey))
                    return _LocalData[_LocalBasePathKey];
                else
                    return "";
            }
        }


        public static String APIKey
        {
            set { _AuthData[_ApiKeyKey] = value; }
            get
            {
                if (_AuthData.ContainsKey(_ApiKeyKey))
                    return _AuthData[_ApiKeyKey];
                else
                    return "";
            }
        }

        public static String SharedSecret
        {
            set { _AuthData[_SharedSecretKey] = value; }
            get
            {
                if (_AuthData.ContainsKey(_SharedSecretKey))
                    return _AuthData[_SharedSecretKey];
                else
                    return "";
            }
        }

        public static String AccessTokenStr
        {
            set { _AuthData[_AccessTokenStrKey] = value; }
            get
            {
                if (_AuthData.ContainsKey(_AccessTokenStrKey))
                    return _AuthData[_AccessTokenStrKey];
                else
                    return "";
            }
        }

        public static String AuthToken
        {
            set { _AuthData[_AuthTokenKey] = value; }
            get
            {
                if (_AuthData.ContainsKey(_AuthTokenKey))
                    return _AuthData[_AuthTokenKey];
                else
                    return "";
            }
        }

        public static String LocalTempPath
        {
            set { _LocalData[_LocalTempPathKey] = value; }
            get
            {
                if (_LocalData.ContainsKey(_LocalTempPathKey))
                    return _LocalData[_LocalTempPathKey];
                else
                    return "";
            }
        }

        public static String RemoteTempAlbum
        {
            set { _RemoteData[_RemoteTempAlbumKey] = value; }
            get
            {
                if (_RemoteData.ContainsKey(_RemoteTempAlbumKey))
                    return _RemoteData[_RemoteTempAlbumKey];
                else
                    return "";
            }
        }
    }
}
