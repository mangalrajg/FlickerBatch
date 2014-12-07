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
    public class FindImagesToUploadViewModel : ViewModelBase
    {

        private LocalAlbumUploadData _SelectedItem;
        public LocalAlbumUploadData SelectedItem
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

        private StatusBarViewModel _StatusBarContext;
        private ObservableCollection<LocalAlbumUploadData> _ImagesToUploadCollection;
        public ObservableCollection<LocalAlbumUploadData> ImagesToUploadCollection
        {
            get
            {
                return _ImagesToUploadCollection;
            }
            set
            {
                _ImagesToUploadCollection = value;
                NotifyPropertyChanged("RemoteAlbumList");
            }
        }
        public FindImagesToUploadViewModel()
        {
            ImagesToUploadCollection = new ObservableCollection<LocalAlbumUploadData>();
            this.LoadImagesToUpload();

            CommandBinding binding = new CommandBinding(StaticCommands.UploadCommand, UploadImages, CanUploadImages);
            CommandManager.RegisterClassCommandBinding(typeof(FindImagesToUploadView), binding);
        }

        private void LoadImagesToUpload()
        {
            List<GenericAlbumData> imgList = DatabaseHelper.LoadAlbumsToUpload();
            ImagesToUploadCollection.Clear();
            foreach (GenericAlbumData gid in imgList)
            {
                ImagesToUploadCollection.Add(new LocalAlbumUploadData(gid));
            }
        }

        private void CanUploadImages(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void UploadImages(object sender, ExecutedRoutedEventArgs e)
        {
            FindImagesToUploadView view = (sender as FindImagesToUploadView);
            var selectedItems = view.MainGrid.SelectedItems;
            List<LocalAlbumUploadData> selectedItemList = new List<LocalAlbumUploadData>();

            foreach (LocalAlbumUploadData lad in selectedItems)
            {
                selectedItemList.Add(lad);
            }
            Action<Object> del = UploadImage;
            Task t = Task.Factory.StartNew(del, selectedItemList);
            t.ContinueWith(OnTaskComplete, TaskScheduler.FromCurrentSynchronizationContext());
            //ThreadPool.QueueUserWorkItem(new WaitCallback(LoadFromFlicker), selectedItemList);

        }

        private void OnTaskComplete(Task t)
        {
            this.LoadImagesToUpload();
        }

        public void UploadImage(Object sender)
        {
            int count = 0;
            List<LocalAlbumUploadData> selectedItems = (sender as List<LocalAlbumUploadData>);
            foreach (LocalAlbumUploadData lad in selectedItems)
            {
                String albumName = lad.Name;
                List<LocalImageData> imagesToUpload = new List<LocalImageData>();
                foreach (LocalImageData lid in lad.ImageDetails)
                {
                    imagesToUpload.Add(lid);
                }
                Console.WriteLine("Upload Album: " + albumName + " Count=" + imagesToUpload.Count);
                _StatusBarContext.CurrentJob = albumName;
                _StatusBarContext.ProgressValue = (count*100 / selectedItems.Count);
                _StatusBarContext.StatusText = "Uploading " + count + "/" + selectedItems.Count;
                FlickerCache.UploadImages(imagesToUpload, albumName);
                Console.WriteLine("Save Images of Album :" + albumName);
                FlickerCache.SaveRemoteAlbum(albumName);
                count++;

            }

        }

        internal void SetStatusBarViewModel(StatusBarViewModel StatusBarContext)
        {
            _StatusBarContext = StatusBarContext;
        }
    }
}
