using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flickr_UI.ViewModel
{
    public class StatusBarViewModel : ViewModelBase
    {
        private String _StatusText;
        public String StatusText
        {
            get
            {
                return _StatusText;
            }
            set
            {
                _StatusText = value;
                NotifyPropertyChanged("StatusText");
            }
        }
        private String _CurrentJob;
        public String CurrentJob
        {
            get
            {
                return _CurrentJob;
            }
            set
            {
                _CurrentJob = value;
                NotifyPropertyChanged("CurrentJob");
            }
        }
        private int _ProgressValue;
        public int ProgressValue
        {
            get
            {
                return _ProgressValue;
            }
            set
            {
                _ProgressValue = value;
                NotifyPropertyChanged("ProgressValue");
            }
        }



    }
}
