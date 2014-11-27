﻿using baseLibrary.DBInterface;
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
                NotifyPropertyChanged("DuplicateImageCollection");
            }
        }

        public DuplicateLocalImageMoverViewModel()
        {
            DuplicateImageCollection = new ObservableCollection<DuplicateImageGroupData>();
           // DuplicateImageCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Students_CollectionChanged);
            this.LoadDuplicateImages();

            CommandBinding binding = new CommandBinding(StaticCommands.MoveImagesCommand, MoveImages, MoveImagesSomething);
            CommandManager.RegisterClassCommandBinding(typeof(DuplicateLocalImageMoverView), binding);


        }

        private void MoveImages(object sender, ExecutedRoutedEventArgs e)
        {
//            DuplicateImageGroupData id= (sender as DuplicateImageMoverView)
            foreach(DuplicateImageData di in SelectedItem.ImageDetails)
            {
                Console.WriteLine("Move " + di.SourcePath + " to " + di.DestinationPath);
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
            List<DuplicateImageGroupData> imgData = DatabaseHelper.LoadLocalDuplicateImageData();
            foreach (DuplicateImageGroupData did in imgData)
            {
                DuplicateImageCollection.Add(did);
            }
        }

    }
}
