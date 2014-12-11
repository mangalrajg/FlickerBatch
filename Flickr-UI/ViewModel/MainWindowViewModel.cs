
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
        private StatusBarViewModel _StatusBarContext;
        public StatusBarViewModel StatusBarContext
        {
            get { return _StatusBarContext; }
            set
            {
                _StatusBarContext = value;
                NotifyPropertyChanged("StatusBarContext");
            }
        }
        #region DuplicateLocalImageMoverCommand
        private ICommand _DuplicateLocalImageMoverCommand;
        public ICommand DuplicateLocalImageMoverCommand
        {
            get
            {
                if (_DuplicateLocalImageMoverCommand == null)
                {
                    _DuplicateLocalImageMoverCommand = new GenericCommand(param => this.SetDuplicateLocalImageMoverViewModel(param), null);
                }
                return _DuplicateLocalImageMoverCommand;
            }
        }

        private object SetDuplicateLocalImageMoverViewModel(Object param)
        {
            (param as Grid).Children.Clear();
            (param as Grid).Children.Add(new DuplicateLocalImageMoverView());
            (param as Grid).DataContext = new DuplicateLocalImageMoverViewModel();
            return null;
        }
        #endregion

        #region DuplicateRemoteImageMoverCommand
        private ICommand _DuplicateRemoteImageMoverCommand;
        public ICommand DuplicateRemoteImageMoverCommand
        {
            get
            {
                if (_DuplicateRemoteImageMoverCommand == null)
                {
                    _DuplicateRemoteImageMoverCommand = new GenericCommand(param => this.SetDuplicateRemoteImageMoverViewModel(param), null);
                }
                return _DuplicateRemoteImageMoverCommand;
            }
        }

        private object SetDuplicateRemoteImageMoverViewModel(Object param)
        {
            (param as Grid).Children.Clear();
            (param as Grid).Children.Add(new DuplicateRemoteImageMoverView());
            (param as Grid).DataContext = new DuplicateRemoteImageMoverViewModel();
            return null;
        }
        #endregion

        #region UploadCommand
        private ICommand _FindImagesToUploadCommand;
        public ICommand FindImagesToUploadCommand
        {
            get
            {
                if (_FindImagesToUploadCommand == null)
                {
                    _FindImagesToUploadCommand = new GenericCommand(param => this.SetFindImagesToUploadViewModel(param), null);
                }
                return _FindImagesToUploadCommand;
            }
        }

        private object SetFindImagesToUploadViewModel(object param)
        {
            (param as Grid).Children.Clear();
            (param as Grid).Children.Add(new FindImagesToUploadView());
            FindImagesToUploadViewModel vm = new FindImagesToUploadViewModel();
            vm.SetStatusBarViewModel(StatusBarContext);
            (param as Grid).DataContext = vm;

            return null;
        }
        #endregion


        #region SyncCommand
        private ICommand _SyncCommand;
        public ICommand SyncCommand
        {
            get
            {
                if (_SyncCommand == null)
                {
                    _SyncCommand = new GenericCommand(param => this.SetFindImagesToSyncViewModel(param), null);
                }
                return _SyncCommand;
            }
        }

        private object SetFindImagesToSyncViewModel(object param)
        {
            (param as Grid).Children.Clear();
            (param as Grid).Children.Add(new FindImagesToSyncView());
            FindImagesToSyncViewModel vm = new FindImagesToSyncViewModel();
            vm.SetStatusBarViewModel(StatusBarContext);
            (param as Grid).DataContext = vm;

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

        #region LoadLocalImageDataCommand
        private ICommand _LoadLocalImageDataCommand;
        public ICommand LoadLocalImageDataCommand
        {
            get
            {
                if (_LoadLocalImageDataCommand == null)
                {
                    _LoadLocalImageDataCommand = new GenericCommand(param => this.SetLoadLocalImageDataViewModel(param), null);
                }
                return _LoadLocalImageDataCommand;
            }
        }

        private object SetLoadLocalImageDataViewModel(object param)
        {
            (param as Grid).Children.Clear();
            (param as Grid).Children.Add(new LoadLocalImageDataView());
            (param as Grid).DataContext = new LoadLocalImageDataViewModel();
            return null;
        }
        #endregion

        #region LoadRemoteImageDataCommand
        private ICommand _LoadRemoteImageDataCommand;
        public ICommand LoadRemoteImageDataCommand
        {
            get
            {
                if (_LoadRemoteImageDataCommand == null)
                {
                    _LoadRemoteImageDataCommand = new GenericCommand(param => this.SetLoadRemoteImageDataViewModel(param), null);
                }
                return _LoadRemoteImageDataCommand;
            }
        }

        private object SetLoadRemoteImageDataViewModel(object param)
        {
            (param as Grid).Children.Clear();
            (param as Grid).Children.Add(new LoadRemoteImageDataView());
            (param as Grid).DataContext = new LoadRemoteImageDataViewModel();
            return null;
        }
        #endregion

        public MainWindowViewModel()
        {
            StatusBarContext = new StatusBarViewModel();
            StatusBarContext.PropertyChanged += StatusBarContext_PropertyChanged;
            
        }

        void StatusBarContext_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("StatusBarContext");
        }
        //Whenever new item is added to the collection, am explicitly calling notify property changed
    }
}

