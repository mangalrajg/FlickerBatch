using baseLibrary.DBInterface;
using FlickerBatch_AlbumRetriever.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Flickr_UI
{
    public class AlbumRenameViewModel : ViewModelBase
    {
        public AlbumRenameViewModel()
        {
            AlbumDataList = new ObservableCollection<FlickrAlbumData>(DatabaseHelper.LoadFlickerAlbums());
            AlbumDataList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Students_CollectionChanged);
        }
        private String _AddPrefix;
        public String AddPrefix
        {
            get
            {
                return _AddPrefix;
            }
            set
            {
                _AddPrefix = value;
                NotifyPropertyChanged("AddPrefix");
            }
        }

        private String _RemovePrefix;
        public String RemovePrefix
        {
            get
            {
                return _RemovePrefix;
            }
            set
            {
                _RemovePrefix = value;
                NotifyPropertyChanged("LocalBasePath");
            }
        }

        private ObservableCollection<FlickrAlbumData> _AlbumDataList;
        public ObservableCollection<FlickrAlbumData> AlbumDataList
        {
            get
            {
                return _AlbumDataList;
            }
            set
            {
                _AlbumDataList = value;
                NotifyPropertyChanged("AlbumDataList");
            }
        }
        void Students_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("AlbumDataList");
        }

    }
}
