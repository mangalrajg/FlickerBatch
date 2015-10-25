using baseLibrary.DBInterface;
using baseLibrary.LocalInterface;
using baseLibrary.Model;
using Flickr_UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Flickr_UI.ViewModel
{
    public class LoadLocalImageDataViewModel : ViewModelBase
    {
        //private String _BasePath;
        public String BasePath
        {
            get { return ConfigModel.LocalBasePath; }
            set
            {
                ConfigModel.LocalBasePath = value;
                NotifyPropertyChanged("BasePath");
            }
        }
        private LocalAlbumData _SelectedItem;
        public LocalAlbumData SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                _SelectedItem = value;
                NotifyPropertyChanged("SelectedItem");
            }
        }

        private ObservableCollection<LocalAlbumData> _LocalAlbumList;
        public ObservableCollection<LocalAlbumData> LocalAlbumList
        {
            get { return _LocalAlbumList; }
            set
            {
                _LocalAlbumList = value;
                NotifyPropertyChanged("RemoteAlbumList");
            }
        }

        public LoadLocalImageDataViewModel()
        {
            LocalAlbumList = new ObservableCollection<LocalAlbumData>();
            //BasePath = ConfigModel.LocalBasePath;
            this.RefreshView();

            CommandBinding binding = new CommandBinding(StaticCommands.LoadFromDBCommand, LoadFromDB, CanLoadFromDB);
            CommandManager.RegisterClassCommandBinding(typeof(LoadLocalImageDataView), binding);

            binding = new CommandBinding(StaticCommands.LoadFromFileCommand, LoadFromFile, CanLoadFromFile);
            CommandManager.RegisterClassCommandBinding(typeof(LoadLocalImageDataView), binding);
            binding = new CommandBinding(StaticCommands.LoadBaseDirFromFileCommand, LoadBaseDirFromFile, CanLoadBaseDirFromFile);
            CommandManager.RegisterClassCommandBinding(typeof(LoadLocalImageDataView), binding);
        }

        private void CanLoadBaseDirFromFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void CanLoadFromFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void CanLoadFromDB(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void LoadBaseDirFromFile(object sender, ExecutedRoutedEventArgs e)
        {
            Action<Object> del = LoadFromFileSystem;
            Task t = Task.Factory.StartNew(del, BasePath);
            t.ContinueWith(OnTaskComplete, TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void LoadFromFile(object sender, ExecutedRoutedEventArgs e)
        {
            LoadLocalImageDataView view = (sender as LoadLocalImageDataView);
            var selectedItems = view.MainGrid.SelectedItems;

            foreach (LocalAlbumData lad in selectedItems)
            {
                Action<Object> del = LoadFromFileSystem;
                Task t = Task.Factory.StartNew(del, lad.Name);
                t.ContinueWith(OnTaskComplete, TaskScheduler.FromCurrentSynchronizationContext());
            }

        }
        private void LoadFromDB(object sender, ExecutedRoutedEventArgs e)
        {
            RefreshView();
        }

        private void OnTaskComplete(Task t)
        {
            this.RefreshView();
        }

        public void LoadFromFileSystem(Object baseDir)
        {
            FilesystemHelper.SaveLocalImageData(baseDir.ToString());
        }


        private void RefreshView()
        {
            List<GenericAlbumData> imgList = DatabaseHelper.LoadLocalAlbums();
            LocalAlbumList.Clear();
            foreach (GenericAlbumData gid in imgList)
            {
                LocalAlbumList.Add(new LocalAlbumData(gid));
            }
        }

    }
}
