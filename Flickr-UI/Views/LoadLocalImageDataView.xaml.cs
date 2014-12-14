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
    /// Interaction logic for LoadLocalImageDataView.xaml
    /// </summary>
    public partial class LoadLocalImageDataView : UserControl
    {
        public LoadLocalImageDataView()
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();



            // Display OpenFileDialog by calling ShowDialog method 
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();

            
            // Get the selected file name and display in a TextBox 
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Open document 
                string filename = dlg.SelectedPath;
                textBox1.Text = filename;
            }
        }


    }
}
