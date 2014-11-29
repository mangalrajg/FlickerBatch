using baseLibrary.DBInterface;
using baseLibrary.LocalInterface;
using baseLibrary.Model;
using Flickr_UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Flickr_UI
{
    class DuplicateLocalImageMoverViewModel : ViewModelBase
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
                NotifyPropertyChanged("ImagesToUploadCollection");
            }
        }

        public DuplicateLocalImageMoverViewModel()
        {
            DuplicateImageCollection = new ObservableCollection<DuplicateImageGroupData>();
            this.LoadDuplicateImages();

            CommandBinding binding = new CommandBinding(StaticCommands.MoveImagesLocalCommand, MoveImages, MoveImagesSomething);
            CommandManager.RegisterClassCommandBinding(typeof(DuplicateLocalImageMoverView), binding);


        }

        private void MoveImages(object sender, ExecutedRoutedEventArgs e)
        {
            foreach(DuplicateImageData di in SelectedItem.ImageDetails)
            {

                Console.WriteLine("Move " + di.SrcFileName + " to " + di.DestFileName);
                if (File.Exists(di.SrcFileName))
                {
                    if (File.Exists(di.DestFileName))
                    {
                        String tmpFolder = ConfigModel.LocalData["LocalTempPath"] + "\\" + di.SourcePath;
                        Directory.CreateDirectory(tmpFolder);
                        File.Move(di.DestFileName, tmpFolder  +"\\"+ di.FileName);
                    }
                    File.Move(di.SrcFileName, di.DestFileName);
                    di.IsProcessed = true;
                }
            }
            FilesystemHelper.SaveLocalImageData(SelectedItem.SourcePath);
            FilesystemHelper.SaveLocalImageData(SelectedItem.DestinationPath);
            this.LoadDuplicateImages();
        }

        private void MoveImagesSomething(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedItem != null;
        }

        private void LoadDuplicateImages()
        {
            DuplicateImageCollection.Clear();
            List<DuplicateImageGroupData> imgData = DatabaseHelper.LoadLocalDuplicateImageData();
            foreach (DuplicateImageGroupData did in imgData)
            {
                DuplicateImageCollection.Add(did);
            }
        }

    }
}
