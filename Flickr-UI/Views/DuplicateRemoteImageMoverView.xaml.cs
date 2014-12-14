using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Flickr_UI.Views
{
    /// <summary>
    /// Interaction logic for DuplicateRemoteImageMoverView.xaml
    /// </summary>
    public partial class DuplicateRemoteImageMoverView : UserControl
    {
        public DuplicateRemoteImageMoverView()
        {
            InitializeComponent();
        }
        private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MainGrid.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
                MainGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
            else
                MainGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        }

    }
}
