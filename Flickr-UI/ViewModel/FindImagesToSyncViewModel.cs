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
    public class FindImagesToSyncViewModel : ViewModelBase
    {

        private DuplicateImageGroupData _SelectedItem;
        public DuplicateImageGroupData SelectedItem
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
        private ObservableCollection<DuplicateImageGroupData> _ImagesToSyncCollection;
        public ObservableCollection<DuplicateImageGroupData> ImagesToSyncCollection
        {
            get
            {
                return _ImagesToSyncCollection;
            }
            set
            {
                _ImagesToSyncCollection = value;
                NotifyPropertyChanged("RemoteAlbumList");
            }
        }
        public FindImagesToSyncViewModel()
        {
            ImagesToSyncCollection = new ObservableCollection<DuplicateImageGroupData>();
            this.LoadImagesToSync();

            CommandBinding binding = new CommandBinding(StaticCommands.SyncCommand, SyncImages, CanSyncImages);
            CommandManager.RegisterClassCommandBinding(typeof(FindImagesToSyncView), binding);
        }

        private void LoadImagesToSync()
        {
            List<DuplicateImageGroupData> imgList = DatabaseHelper.LoadImageGroupsToSyncronise();
            ImagesToSyncCollection.Clear();
            foreach (DuplicateImageGroupData gid in imgList)
            {
                ImagesToSyncCollection.Add(new DuplicateImageGroupData(gid.SourcePath, gid.DestinationPath, gid.ImgCount, Mode.MIXED));
            }
        }

        private void CanSyncImages(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SyncImages(object sender, ExecutedRoutedEventArgs e)
        {
            FindImagesToSyncView view = (sender as FindImagesToSyncView);
            var selectedItems = view.MainGrid.SelectedItems;
            List<DuplicateImageGroupData> selectedItemList = new List<DuplicateImageGroupData>();

            foreach (DuplicateImageGroupData lad in selectedItems)
            {
                selectedItemList.Add(lad);
            }
            Action<Object> del = SyncImage;
            Task t = Task.Factory.StartNew(del, selectedItemList);
            t.ContinueWith(OnTaskComplete, TaskScheduler.FromCurrentSynchronizationContext());
            //ThreadPool.QueueUserWorkItem(new WaitCallback(LoadFromFlicker), selectedItemList);

        }

        private void OnTaskComplete(Task t)
        {
            this.LoadImagesToSync();
        }

        public void SyncImage(Object sender)
        {
            int count = 0;
            List<DuplicateImageGroupData> selectedItems = (sender as List<DuplicateImageGroupData>);
            foreach (DuplicateImageGroupData lad in selectedItems)
            {
                List<String> imagesToSync = new List<String>();
                foreach (DuplicateImageData lid in lad.ImageDetails)
                {
                    imagesToSync.Add(lid.FileName);
                }
                Console.WriteLine("Sync Album: " + lad.SourcePath + " To :" + lad.DestinationPath + " Count=" + imagesToSync.Count);
                _StatusBarContext.CurrentJob = lad.SourcePath;
                _StatusBarContext.ProgressValue = (count*100 / selectedItems.Count);
                _StatusBarContext.StatusText = "Syncing " + count + "/" + selectedItems.Count;
                try
                {
                    FlickerCache.MovePictures(imagesToSync, lad.SourcePath, lad.DestinationPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    throw ex;
                }
                Console.WriteLine("Save Images of Album :" + lad.SourcePath);
                try
                {
                    FlickerCache.SaveRemoteAlbum(lad.SourcePath);
                    FlickerCache.SaveRemoteAlbum(lad.DestinationPath);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                count++;
            }

        }

        internal void SetStatusBarViewModel(StatusBarViewModel StatusBarContext)
        {
            _StatusBarContext = StatusBarContext;
        }
    }
}
