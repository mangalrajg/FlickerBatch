using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Flickr_UI.ViewModel
{
    public class StatusBarViewModel : ViewModelBase
    {
        public String StatusText
        {
            get { return _curJobCount + "/" + _JobCount; }
            set { }
        }
        public String StatusBarVisiblity
        {
            get { return _curJobCount == 0 ? "Hidden" : "Visible"; }
            set { }
        }
        private String _CurrentJob;
        public String CurrentJob
        {
            get { return _CurrentJob; }
            set
            {
                _CurrentJob = value;
                NotifyPropertyChanged("CurrentJob");
            }
        }
        public int ProgressValue
        {
            get { return (100 * _curJobCount) / _JobCount; }
            set { }
        }
        int _JobCount = 1;
        int _curJobCount = 0;
        public void Initiallize(int jobCount)
        {
            _curJobCount = 0;
            if (jobCount != 0)
                _JobCount = jobCount;
            OneJobCompleted();
        }
        public void Reset()
        {
            Initiallize(1);
        }

        public void OneJobCompleted()
        {
            _curJobCount++;
            NotifyPropertyChanged("ProgressValue");
            NotifyPropertyChanged("StatusText");
        }
    }
}

