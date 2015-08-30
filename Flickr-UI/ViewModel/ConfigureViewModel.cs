using baseLibrary.DBInterface;
using baseLibrary.Model;
using Flickr_UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Flickr_UI.ViewModel
{
    public class ConfigureViewModel : ViewModelBase
    {
        public ConfigureViewModel()
        {
            CommandBinding binding = new CommandBinding(StaticCommands.ClearDataGrid, ClearAction, CanAlwaysDo);
            CommandManager.RegisterClassCommandBinding(typeof(ConfigureView), binding);

            binding = new CommandBinding(StaticCommands.CancelCommand, CancelAction, CanAlwaysDo);
            CommandManager.RegisterClassCommandBinding(typeof(ConfigureView), binding);

            binding = new CommandBinding(StaticCommands.SyncCommand, SaveAction, CanAlwaysDo);
            CommandManager.RegisterClassCommandBinding(typeof(ConfigureView), binding);
        }

        private void SaveAction(object sender, ExecutedRoutedEventArgs e)
        {
            ConfigModel.Save();
        }

        private void CancelAction(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void CanAlwaysDo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ClearAction(object sender, ExecutedRoutedEventArgs e)
        {
            ConfigModel.Reload();
        }

        public String LocalBasePath
        {
            get { return ConfigModel.LocalBasePath; }
            set
            {
                ConfigModel.LocalBasePath = value;
                NotifyPropertyChanged("LocalBasePath");
            }
        }


        public String APIKey
        {
            get { return ConfigModel.APIKey; }
            set
            {
                ConfigModel.APIKey = value;
                NotifyPropertyChanged("APIKey");
            }
        }

        public String SharedSecret
        {
            get { return ConfigModel.SharedSecret; }
            set
            {
                ConfigModel.SharedSecret = value;
                NotifyPropertyChanged("SharedSecret");
            }
        }

        public String AccessTokenStr
        {
            get { return ConfigModel.AccessTokenStr; }
            set
            {
                ConfigModel.AccessTokenStr = value;
                NotifyPropertyChanged("AccessTokenStr");
            }
        }

        public String AuthToken
        {
            get { return ConfigModel.AuthToken; }
            set
            {
                ConfigModel.AuthToken = value;
                NotifyPropertyChanged("AuthToken");
            }
        }


    }
}
