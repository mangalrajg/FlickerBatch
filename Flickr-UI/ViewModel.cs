using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Flickr_UI
{
    public class ViewModel : ViewModelBase
    {
        private ICommand _DuplicatesCommand;
        private ICommand _MoveCommand;
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
        public ICommand DuplicatesCommand
        {
            get
            {
                if (_DuplicatesCommand == null)
                {
                    _DuplicatesCommand = new DuplicatesCommand(param => this.LoadDuplicateImages(), null);
                }
                return _DuplicatesCommand;
            }
        }



        public ICommand MoveCommand
        {
            get
            {
                if (_MoveCommand == null)
                {
                    _MoveCommand = new DuplicatesCommand(param => this.MoveImages(), null);
                }
                return _MoveCommand;
            }
        }

        private object MoveImages()
        {
            throw new NotImplementedException();
        }
        public ViewModel()
        {
            DuplicateImageCollection = new ObservableCollection<DuplicateImageData>();
            DuplicateImageCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Students_CollectionChanged);
        }
        //Whenever new item is added to the collection, am explicitly calling notify property changed
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

