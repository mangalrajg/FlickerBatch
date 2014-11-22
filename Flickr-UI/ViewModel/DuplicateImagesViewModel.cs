using baseLibrary.DBInterface;
using FlickerBatch_AlbumRetriever;
using Flickr_UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Flickr_UI
{
    class DuplicateImagesViewModel : ViewModelBase
    {
        private DuplicateImageData _SelectedItem;
        public DuplicateImageData SelectedItem
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

        private ObservableCollection<DuplicateImageData> _DuplicateImageCollection;
        public ObservableCollection<DuplicateImageData> DuplicateImageCollection
        {
            get
            {
                return _DuplicateImageCollection;
            }
            set
            {
                _DuplicateImageCollection = value;
                NotifyPropertyChanged("DuplicateImageCollection");
            }
        }

        public DuplicateImagesViewModel()
        {
            DuplicateImageCollection = new ObservableCollection<DuplicateImageData>();
            DuplicateImageCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Students_CollectionChanged);
            this.LoadDuplicateImages();

            CommandBinding binding = new CommandBinding(StaticCommands.MoveImagesCommand, MoveImages, MoveImagesSomething);
            CommandManager.RegisterClassCommandBinding(typeof(DuplicateImageMoverView), binding);


        }

        private void MoveImages(object sender, ExecutedRoutedEventArgs e)
        {
//            DuplicateImageData id= (sender as DuplicateImageMoverView)
            foreach(DuplicateImages di in SelectedItem.ImageDetails)
            {
                Console.WriteLine("Move " + di.SourcePath + " to " + di.DestinationPath);
            }
        }

        private void MoveImagesSomething(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
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
