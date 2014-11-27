using baseLibrary.DBInterface;
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
    class DuplicateRemoteImageMoverViewModel : ViewModelBase
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

        private ObservableCollection<DuplicateImageGroupData> _DuplicateImageCollection;
        public ObservableCollection<DuplicateImageGroupData> DuplicateImageCollection
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

        public DuplicateRemoteImageMoverViewModel()
        {
            DuplicateImageCollection = new ObservableCollection<DuplicateImageGroupData>();
           // DuplicateImageCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Students_CollectionChanged);
            this.LoadDuplicateImages();

            CommandBinding binding = new CommandBinding(StaticCommands.MoveImagesRemoteCommand, MoveImages, MoveImagesSomething);
            CommandManager.RegisterClassCommandBinding(typeof(DuplicateRemoteImageMoverView), binding);


        }

        private void MoveImages(object sender, ExecutedRoutedEventArgs e)
        {
            foreach(DuplicateImageData di in SelectedItem.ImageDetails)
            {
                Console.WriteLine("Move " + di.SourcePath + " to " + di.DestinationPath);
                di.IsProcessed = true;
            }
        }

        private void MoveImagesSomething(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        //void Students_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    NotifyPropertyChanged("DuplicateImageObjs");
        //}
        private void LoadDuplicateImages()
        {
            List<DuplicateImageGroupData> imgData = DatabaseHelper.LoadRemoteDuplicateImageData();
            foreach (DuplicateImageGroupData did in imgData)
            {
                DuplicateImageCollection.Add(did);
            }
        }

    }
}
