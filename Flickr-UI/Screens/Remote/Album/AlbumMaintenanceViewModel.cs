using baseLibrary.DBInterface;
using baseLibrary.Model;
using baseLibrary.RemoteInterface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Flickr_UI
{
    public class AlbumMaintenanceViewModel : ViewModelBase
    {
        public AlbumMaintenanceViewModel()
        {
            AlbumDataList = new ObservableCollection<DuplicateAlbumData>(DatabaseHelper.LoadDuplicateAlbums());
            AlbumDataList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Students_CollectionChanged);
            CommandBinding binding = new CommandBinding(StaticCommands.MergeCommand, MergeAlbums, (s, e) => { e.CanExecute = SelectedItem != null; });
            CommandManager.RegisterClassCommandBinding(typeof(AlbumMaintenanceViewModel), binding);

        }

        private void MergeAlbums(object sender, ExecutedRoutedEventArgs e)
        {
            FlickrAlbumData root = SelectedItem.Albums[0];
            foreach (FlickrAlbumData fad in SelectedItem.Albums)
            {
                if (fad == root)
                    continue;
                List<BaseImageData> lbid = FlickerHelper.LoadRemotePicturesData(fad);
                List<String> idString = new List<string>();
                foreach (BaseImageData bid in lbid)
                {
                    idString.Add((bid as RemoteImageData).PhotoId);
                }
                FlickerHelper.MovePictures(idString.ToArray(), fad.AlbumId, root.AlbumId);
            }
        }
        private ObservableCollection<DuplicateAlbumData> _AlbumDataList;
        public ObservableCollection<DuplicateAlbumData> AlbumDataList
        {
            get { return _AlbumDataList; }
            set { _AlbumDataList = value; NotifyPropertyChanged("AlbumDataList"); }
        }
        private DuplicateAlbumData _SelectedItem;
        public DuplicateAlbumData SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                _SelectedItem = value;
                NotifyPropertyChanged("SelectedItem");
            }
        }

        void Students_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("AlbumDataList");
        }

    }
}
