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
                NotifyPropertyChanged("RemoteAlbumList");
            }
        }

        public DuplicateRemoteImageMoverViewModel()
        {
            DuplicateImageCollection = new ObservableCollection<DuplicateImageGroupData>();
            // RemoteAlbumList.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Students_CollectionChanged);
            this.LoadDuplicateImages();

            CommandBinding binding = new CommandBinding(StaticCommands.MoveImagesRemoteCommand, MoveImages, MoveImagesSomething);
            CommandManager.RegisterClassCommandBinding(typeof(DuplicateRemoteImageMoverView), binding);


        }

        private void MoveImages(object sender, ExecutedRoutedEventArgs e)
        {
            int i = 1;
            try
            {
                List<String> backupList = new List<string>();
                List<String> moveList = new List<string>();
                foreach (DuplicateImageData di in SelectedItem.ImageDetails)
                {
                    Console.WriteLine(i + "/" + SelectedItem.ImageDetails.Count + " : Move " + di.FileName + " From " + di.SourcePath + " to " + di.DestinationPath);
                    if (FlickerCache.AlbumSearchDict.ContainsKey(di.SourcePath))
                    {
                        RemoteImageData rid = FlickerCache.getImageData(di.FileName, di.DestinationPath);
                        if (rid != null)
                        {
                            backupList.Add(di.FileName);
                        }
                        moveList.Add(di.FileName);
                    }
                    else
                    {
                        Console.WriteLine("Album: " + di.SourcePath + " Not FOUND");
                    }
                    i++;
                }

                String tmpFolder = ConfigModel.RemoteTempAlbum + "\\" + SelectedItem.SourcePath;
                FlickerCache.MovePictures(backupList, SelectedItem.DestinationPath, tmpFolder);
                FlickerCache.MovePictures(moveList, SelectedItem.SourcePath, SelectedItem.DestinationPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An Exception has been detected : " + ex.ToString());
            }
            Console.WriteLine("");
            FlickerCache.SaveRemoteAlbum(SelectedItem.SourcePath);
            FlickerCache.SaveRemoteAlbum(SelectedItem.DestinationPath);
            this.LoadDuplicateImages();
            Console.WriteLine("---------------------------------------------");

        }

        private void MoveImagesSomething(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedItem != null ;
        }

        //void Students_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    NotifyPropertyChanged("DuplicateImageObjs");
        //}
        private void LoadDuplicateImages()
        {
            DuplicateImageCollection.Clear();
            List<DuplicateImageGroupData> imgData = DatabaseHelper.LoadRemoteDuplicateImageData();
            foreach (DuplicateImageGroupData did in imgData)
            {
                DuplicateImageCollection.Add(did);
            }
        }

    }
}
