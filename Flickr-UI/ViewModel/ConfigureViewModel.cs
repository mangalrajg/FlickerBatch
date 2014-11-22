using baseLibrary.DBInterface;
using baseLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flickr_UI.ViewModel
{
    public class ConfigureViewModel : ViewModelBase
    {
        public ConfigureViewModel()
        {

        }

        public String LocalBasePath
        {
            get
            {
                String key = "LocalBasePath";
                if (ConfigModel.LocalData.ContainsKey(key))
                {
                    return ConfigModel.LocalData[key];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                String key = "LocalBasePath";
                ConfigModel.LocalData[key] = value;
                NotifyPropertyChanged(key);
            }
        }


        public String APIKey
        {
            get { 
                String key = "APIKey";
                if (ConfigModel.AuthData.ContainsKey(key))
                {
                    return ConfigModel.AuthData[key];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                String key = "APIKey";
                ConfigModel.AuthData[key] = value;
                NotifyPropertyChanged(key);
            }
        }

        public String SharedSecret
        {
            get
            {
                String key = "SharedSecret";
                if (ConfigModel.AuthData.ContainsKey(key))
                {
                    return ConfigModel.AuthData[key];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                String key = "SharedSecret";
                ConfigModel.AuthData[key] = value;
                NotifyPropertyChanged(key);
            }
        }

        public String AccessTokenStr
        {
            get
            {
                String key = "AccessTokenStr";
                if (ConfigModel.AuthData.ContainsKey(key))
                {
                    return ConfigModel.AuthData[key];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                String key = "AccessTokenStr";
                ConfigModel.AuthData[key] = value;
                NotifyPropertyChanged(key);
            }
        }

        public String AuthToken
        {
            get
            {
                String key = "AuthToken";
                if (ConfigModel.AuthData.ContainsKey(key))
                {
                    return ConfigModel.AuthData[key];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                String key = "AuthToken";
                ConfigModel.AuthData[key] = value;
                NotifyPropertyChanged(key);
            }
        }

        
    }
}
