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
                return ConfigModel.LocalBasePath;
            }
            set
            {
                ConfigModel.LocalBasePath = value;
                NotifyPropertyChanged("LocalBasePath");
            }
        }


        public String APIKey
        {
            get
            {
                return ConfigModel.APIKey;
            }
            set
            {
                ConfigModel.APIKey = value;
                NotifyPropertyChanged("APIKey");
            }
        }

        public String SharedSecret
        {
            get
            {
                return ConfigModel.SharedSecret;
            }
            set
            {
                ConfigModel.SharedSecret = value;
                NotifyPropertyChanged("SharedSecret");
            }
        }

        public String AccessTokenStr
        {
            get
            {
                return ConfigModel.AccessTokenStr;
            }
            set
            {
                ConfigModel.AccessTokenStr = value;
                NotifyPropertyChanged("AccessTokenStr");
            }
        }

        public String AuthToken
        {
            get
            {
                return ConfigModel.AuthToken;
            }
            set
            {
                ConfigModel.AuthToken = value;
                NotifyPropertyChanged("AuthToken");
            }
        }


    }
}
