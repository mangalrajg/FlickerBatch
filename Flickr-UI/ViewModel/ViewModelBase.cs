using Flickr_UI.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flickr_UI
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected StatusBarViewModel _StatusBarContext ;
        internal void SetStatusBarViewModel(StatusBarViewModel StatusBarContext)
        {
            _StatusBarContext = StatusBarContext;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}