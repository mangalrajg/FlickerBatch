using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Flickr_UI
{
    public class ViewModel : ViewModelBase
    {
        private ICommand _DuplicatesCommand;
        public ICommand DuplicatesCommand
        {
            get
            {
                if (_DuplicatesCommand == null)
                {
                    _DuplicatesCommand = new DuplicatesCommand(param => this.SetDuplicateImageViewModel(param), null);
                }
                return _DuplicatesCommand;
            }
        }

        private object SetDuplicateImageViewModel(Object param)
        {
            (param as Grid).Children.Clear();
            (param as Grid).Children.Add(new DuplicateImageView());
            (param as Grid).DataContext = new DuplicateImagesViewModel();
            return null;
        }

        private ICommand _AlbumRenameCommand;
        public ICommand AlbumRenameCommand
        {
            get
            {
                if (_AlbumRenameCommand == null)
                {
                    _AlbumRenameCommand = new DuplicatesCommand(param => this.SetAlbumRenameViewModel(param), null);
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



        public ViewModel()
        {
        }
        //Whenever new item is added to the collection, am explicitly calling notify property changed
    }
}

