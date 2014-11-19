using baseLibrary.DBInterface;
using FlickerBatch_AlbumRetriever;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flickr_UI
{
    class DuplicateImagesViewModel : ViewModelBase
    {
        private ObservableCollection<DuplicateImageData> _DuplicateImageObjs;
        public ObservableCollection<DuplicateImageData> DuplicateImageCollection
        {
            get
            {
                return _DuplicateImageObjs;
            }
            set
            {
                _DuplicateImageObjs = value;
                NotifyPropertyChanged("DuplicateImageObjs");
            }
        }

        public DuplicateImagesViewModel()
        {
            DuplicateImageCollection = new ObservableCollection<DuplicateImageData>();
            DuplicateImageCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Students_CollectionChanged);
            this.LoadDuplicateImages();

        }
        void Students_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("DuplicateImageObjs");
        }
        private void LoadDuplicateImages()
        {
            List<DuplicateImageData> imgData = DatabaseHelper.loadDuplicateImageData();
            foreach (DuplicateImageData did in imgData)
            {
                DuplicateImageCollection.Add(did);
            }
        }

    }
}
