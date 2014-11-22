﻿
using Flickr_UI.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Flickr_UI.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region DuplicatesCommand
        private ICommand _DuplicatesCommand;
        public ICommand DuplicatesCommand
        {
            get
            {
                if (_DuplicatesCommand == null)
                {
                    _DuplicatesCommand = new GenericCommand(param => this.SetDuplicateImageViewModel(param), null);
                }
                return _DuplicatesCommand;
            }
        }

        private object SetDuplicateImageViewModel(Object param)
        {
            (param as Grid).Children.Clear();
            (param as Grid).Children.Add(new DuplicateImageMoverView());
            (param as Grid).DataContext = new DuplicateImagesViewModel();
            return null;
        }
        #endregion

        #region AlbumRenameCommand
        private ICommand _AlbumRenameCommand;
        public ICommand AlbumRenameCommand
        {
            get
            {
                if (_AlbumRenameCommand == null)
                {
                    _AlbumRenameCommand = new GenericCommand(param => this.SetAlbumRenameViewModel(param), null);
                }
                return _AlbumRenameCommand;
            }
        }

        private object SetAlbumRenameViewModel(object param)
        {
            (param as Grid).Children.Clear();
            (param as Grid).Children.Add(new AlbumRenameView());
            (param as Grid).DataContext = new AlbumRenameViewModel();
            return null;
        }
        #endregion

        #region UploadCommand
        private ICommand _UploadCommand;
        public ICommand UploadCommand
        {
            get
            {
                if (_UploadCommand == null)
                {
                    _UploadCommand = new GenericCommand(param => this.SetUploadViewModel(param), null);
                }
                return _UploadCommand;
            }
        }

        private object SetUploadViewModel(object param)
        {
            (param as Grid).Children.Clear();
            (param as Grid).Children.Add(new AlbumRenameView());
            (param as Grid).DataContext = new AlbumRenameViewModel();
            return null;
        }
        #endregion

        #region ConfigureCommand
        private ICommand _ConfigureCommand;
        public ICommand ConfigureCommand
        {
            get
            {
                if (_ConfigureCommand == null)
                {
                    _ConfigureCommand = new GenericCommand(param => this.SetConfigureViewModel(param), null);
                }
                return _ConfigureCommand;
            }
        }

        private object SetConfigureViewModel(object param)
        {
            (param as Grid).Children.Clear();
            (param as Grid).Children.Add(new ConfigureView());
            (param as Grid).DataContext = new ConfigureViewModel();
            return null;
        }
        #endregion


        public MainWindowViewModel()
        {
        }
        //Whenever new item is added to the collection, am explicitly calling notify property changed
    }
}

