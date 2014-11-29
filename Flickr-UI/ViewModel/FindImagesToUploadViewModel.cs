using baseLibrary.DBInterface;
using baseLibrary.Model;
using baseLibrary.RemoteInterface;
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
    public class FindImagesToUploadViewModel : ViewModelBase
    {
        private LocalImageData _SelectedItem;
        public LocalImageData SelectedItem
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

        private ObservableCollection<LocalAlbumData> _ImagesToUploadCollection;
        public ObservableCollection<LocalAlbumData> ImagesToUploadCollection
        {
            get
            {
                return _ImagesToUploadCollection;
            }
            set
            {
                _ImagesToUploadCollection = value;
                NotifyPropertyChanged("ImagesToUploadCollection");
            }
        }
        public FindImagesToUploadViewModel()
        {
            ImagesToUploadCollection = new ObservableCollection<LocalAlbumData>();
            this.LoadImagesToUpload();

            CommandBinding binding = new CommandBinding(StaticCommands.UploadCommand, UploadImages, CanUploadImages);
            CommandManager.RegisterClassCommandBinding(typeof(FindImagesToUploadView), binding);
        }

        private void LoadImagesToUpload()
        {
            ImagesToUploadCollection.Clear();
            List<GenericAlbumData> imgList = DatabaseHelper.LoadAlbumsToUpload();
            foreach (GenericAlbumData gid in imgList)
            {
                ImagesToUploadCollection.Add(new LocalAlbumData(gid));
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
            foreach (LocalAlbumData lad in selectedItems)
            {
                String albumName = lad.Name;
                List<String> fileNames = new List<string>();

                foreach (LocalImageData lid in lad.ImageDetails)
                {
                    Console.WriteLine("Upload:" + lid.Name + " in Album: " + albumName);
                    fileNames.Add(lid.Name);
                }
                FlickerCache.UploadImages(fileNames, albumName);
                Console.WriteLine("Save Images of Album :" + albumName);
                FlickerCache.SaveRemoteAlbum(albumName);
            }

            this.LoadImagesToUpload();

        }
    }
}
