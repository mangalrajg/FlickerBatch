using baseLibrary.DBInterface;
using baseLibrary.Model;
using baseLibrary.RemoteInterface;
using Flickr_UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Flickr_UI.ViewModel
{
    class LoadRemoteImageDataViewModel : ViewModelBase
    {
        private GenericAlbumData _SelectedItem;
        public GenericAlbumData SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;
                NotifyPropertyChanged("SelectedItem");
            }
        }

        private ObservableCollection<GenericAlbumData> _RemoteAlbumList;
        public ObservableCollection<GenericAlbumData> RemoteAlbumList
        {
            get
            {
                return _RemoteAlbumList;
            }
            set
            {
                _RemoteAlbumList = value;
                NotifyPropertyChanged("RemoteAlbumList");
            }
        }

        public LoadRemoteImageDataViewModel()
        {
            RemoteAlbumList = new ObservableCollection<GenericAlbumData>();
            this.RefreshView();

            CommandBinding binding = new CommandBinding(StaticCommands.LoadFromDBCommand, LoadFromDB, CanLoadFromDB);
            CommandManager.RegisterClassCommandBinding(typeof(LoadRemoteImageDataView), binding);

            binding = new CommandBinding(StaticCommands.LoadAlbumsFromFlicker, LoadAlbumFromFlicker, CanLoadAlbumFromFlicker);
            CommandManager.RegisterClassCommandBinding(typeof(LoadRemoteImageDataView), binding);

            binding = new CommandBinding(StaticCommands.LoadAllAlbumsFromFlicker, LoadAllAlbumFromFlicker, CanLoadAllAlbumFromFlicker);
            CommandManager.RegisterClassCommandBinding(typeof(LoadRemoteImageDataView), binding);
        }

        private void CanLoadAllAlbumFromFlicker(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void CanLoadAlbumFromFlicker(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void CanLoadFromDB(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void LoadAllAlbumFromFlicker(object sender, ExecutedRoutedEventArgs e)
        {
            DatabaseHelper.DeleteAllRemoteImageData();
            List<FlickrAlbumData> fadList = FlickerHelper.LoadAllAlbums();
            ThreadPool.SetMaxThreads(20, 20);
            Action<Object> del = LoadFromFlicker;
            foreach (FlickrAlbumData fad in fadList)
            {
                Task t = Task.Factory.StartNew(del, fad);
                t.ContinueWith(OnTaskComplete, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
        private void LoadAlbumFromFlicker(object sender, ExecutedRoutedEventArgs e)
        {
            LoadRemoteImageDataView view = (sender as LoadRemoteImageDataView);
            var selectedItems = view.MainGrid.SelectedItems;
            Action<Object> del = LoadFromFlicker;
            foreach (FlickrAlbumData fad in selectedItems)
            {
                Task t = Task.Factory.StartNew(del, fad);
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

        public void LoadFromFlicker(Object obj)
        {
            FlickrAlbumData fad = (obj as FlickrAlbumData);
            if (fad != null)
            {
                DatabaseHelper.DeleteRemoteImageData(fad.Name);
                List<BaseImageData> imageList = FlickerHelper.LoadRemotePicturesData(fad);
                DatabaseHelper.SaveImageData(imageList);
            }
        }


        private void RefreshView()
        {
            List<GenericAlbumData> imgList = DatabaseHelper.LoadRemoteAlbums();
            Console.WriteLine("We have " + imgList.Count + " Albums");
            RemoteAlbumList.Clear();
            foreach (GenericAlbumData gid in imgList)
            {
                RemoteAlbumList.Add(gid);
            }
        }
    }

}
