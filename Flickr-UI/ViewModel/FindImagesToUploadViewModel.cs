using baseLibrary.DBInterface;
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

        private ObservableCollection<LocalImageData> _ImagesToUploadCollection;
        public ObservableCollection<LocalImageData> ImagesToUploadCollection
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
            ImagesToUploadCollection = new ObservableCollection<LocalImageData>();
            this.LoadDuplicateImages();

            CommandBinding binding = new CommandBinding(StaticCommands.FindImagesToUploadCommand, FindImagesToUpload, FindImagesToUploadSomething);
            CommandManager.RegisterClassCommandBinding(typeof(FindImagesToUploadView), binding);
        }

        private void LoadDuplicateImages()
        {
            List<LocalImageData> imgList = DatabaseHelper.LoadImagesToUpload();
            foreach (LocalImageData did in imgList)
            {
                ImagesToUploadCollection.Add(did);
            }
        }

        private void FindImagesToUploadSomething(object sender, CanExecuteRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void FindImagesToUpload(object sender, ExecutedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
