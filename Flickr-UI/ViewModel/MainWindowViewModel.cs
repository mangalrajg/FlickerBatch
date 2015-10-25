
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
                    _FindImagesToUploadCommand = new GenericCommand(param => SetViewAndViewModel(param as Grid, new FindImagesToUploadView(), new FindImagesToUploadViewModel()), null);
                }
                return _FindImagesToUploadCommand;
            }
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
                    _SyncCommand = new GenericCommand(param => SetViewAndViewModel(param as Grid, new FindImagesToSyncView(), new FindImagesToSyncViewModel()), null);
                }
                return _SyncCommand;
            }
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

        #region AlbumMaintenanceCommand
        private ICommand _AlbumMaintenanceCommand;
        public ICommand AlbumMaintenanceCommand
        {
            get
            {
                if (_AlbumMaintenanceCommand == null)
                {
                    _AlbumMaintenanceCommand = new GenericCommand(param => this.SetAlbumMaintenanceViewModel(param), null);
                }
                return _AlbumMaintenanceCommand;
            }
        }

        private object SetAlbumMaintenanceViewModel(object param)
        {
            (param as Grid).Children.Clear();
            (param as Grid).Children.Add(new AlbumMaintenanceView());
            (param as Grid).DataContext = new AlbumMaintenanceViewModel();
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
                    _LoadLocalImageDataCommand = new GenericCommand(param => this.SetViewAndViewModel(param as Panel, new LoadLocalImageDataView(), new LoadLocalImageDataViewModel()), null);
                }
                return _LoadLocalImageDataCommand;
            }
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
                    _LoadRemoteImageDataCommand = new GenericCommand(param => SetViewAndViewModel(param as Grid, new LoadRemoteImageDataView(), new LoadRemoteImageDataViewModel()), null);
                }
                return _LoadRemoteImageDataCommand;
            }
        }

        #endregion

        private void SetViewAndViewModel(Panel panel, ContentControl view, ViewModelBase viewModel)
        {
            panel.Children.Clear();
            panel.Children.Add(view);
            view.DataContext = viewModel;
            viewModel.SetStatusBarViewModel(StatusBarContext);
        }


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

