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
        private bool m_cancel = false;
        private FlickrAlbumData _SelectedItem;
        public FlickrAlbumData SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                _SelectedItem = value;
                NotifyPropertyChanged("SelectedItem");
            }
        }

        private ObservableCollection<FlickrAlbumData> _RemoteAlbumList;
        public ObservableCollection<FlickrAlbumData> RemoteAlbumList
        {
            get { return _RemoteAlbumList; }
            set
            {
                _RemoteAlbumList = value;
                NotifyPropertyChanged("RemoteAlbumList");
            }
        }

        public LoadRemoteImageDataViewModel()
        {
            RemoteAlbumList = new ObservableCollection<FlickrAlbumData>();
            this.RefreshView(null);

            CommandBinding binding = new CommandBinding(StaticCommands.LoadFromDBCommand, LoadFromDB, CanAlwaysDo);
            CommandManager.RegisterClassCommandBinding(typeof(LoadRemoteImageDataView), binding);

            binding = new CommandBinding(StaticCommands.LoadAlbumFromFlicker, LoadAlbumFromFlicker, CanLoadAlbum);
            CommandManager.RegisterClassCommandBinding(typeof(LoadRemoteImageDataView), binding);

            binding = new CommandBinding(StaticCommands.LoadAllAlbumsFromFlicker, LoadAllAlbumFromFlicker, CanAlwaysDo);
            CommandManager.RegisterClassCommandBinding(typeof(LoadRemoteImageDataView), binding);

            binding = new CommandBinding(StaticCommands.ClearDataGrid, ClearDataGrid, CanAlwaysDo);
            CommandManager.RegisterClassCommandBinding(typeof(LoadRemoteImageDataView), binding);

            binding = new CommandBinding(StaticCommands.CancelCommand, CancelAction, CanAlwaysDo);
            CommandManager.RegisterClassCommandBinding(typeof(LoadRemoteImageDataView), binding);
        }


        #region oneLiners
        private void CanLoadAlbum(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedItem != null;
        }


        private void CanAlwaysDo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void LoadFromDB(object sender, ExecutedRoutedEventArgs e)
        {
            RefreshView(null);
        }

        private void OnTaskComplete(Task t, Object obj)
        {
            RefreshView(obj as FlickrAlbumData);
            _StatusBarContext.OneJobCompleted();
        }
        private void CancelAction(object sender, ExecutedRoutedEventArgs e)
        {
            m_cancel = true;
            _StatusBarContext.Reset();
        }

        #endregion

        private async void ClearDataGrid(object sender, ExecutedRoutedEventArgs e)
        {
            await Task.Factory.StartNew(() => DatabaseHelper.DeleteAllRemoteImageData());
            RefreshView(null);
        }

        private async void LoadAllAlbumFromFlicker(object sender, ExecutedRoutedEventArgs e)
        {
            m_cancel = false; 
            await Task.Factory.StartNew(() => DatabaseHelper.DeleteAllFlickerAlbums());
            await Task.Factory.StartNew(() => DatabaseHelper.DeleteAllRemoteImageData());
            List<FlickrAlbumData> fadList = await FlickerHelper.LoadAllAlbumsAsync();
            await Task.Factory.StartNew(() => DatabaseHelper.SaveFlickrAlbums(fadList));
            LoadFromFlicker(fadList);

        }
        private  void LoadAlbumFromFlicker(object sender, ExecutedRoutedEventArgs e)
        {
            m_cancel = false;
            LoadRemoteImageDataView view = (sender as LoadRemoteImageDataView);
            List<FlickrAlbumData> selectedItems = view.MainGrid.SelectedItems as List<FlickrAlbumData>;
            LoadFromFlicker(selectedItems);
        }

        public void LoadFromFlicker(List<FlickrAlbumData> fadList)
        {
            _StatusBarContext.Initiallize(fadList.Count);
            foreach (FlickrAlbumData fad in fadList)
            {
                Task t = Task.Factory.StartNew(() => LoadFromFlicker(fad));
                t.ContinueWith(OnTaskComplete, fad, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
        public void LoadFromFlicker(FlickrAlbumData fad)
        {
            if (fad != null && m_cancel != true)
            {
                _StatusBarContext.CurrentJob = fad.Name;
                DatabaseHelper.DeleteRemoteImageData(fad.Name);
                List<BaseImageData> imageList = FlickerHelper.LoadRemotePicturesData(fad);
                fad.ActualPhotoCount = imageList.Count;
                DatabaseHelper.UpdateFlickerAlbum(fad);
                DatabaseHelper.SaveImageData(imageList);
            }
        }

        private void RefreshView(FlickrAlbumData obj)
        {
            if (m_cancel != true)
            {
                List<FlickrAlbumData> imgList = DatabaseHelper.LoadFlickerAlbums();
                Console.WriteLine("We have " + imgList.Count + " Albums");
                RemoteAlbumList.Clear();
                foreach (FlickrAlbumData gid in imgList)
                {
                    RemoteAlbumList.Add(gid);
                }

            }
        }

    }

}
