using PaeoniaTechSpectroMeter.Model;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace PaeoniaTechSpectroMeter.Views
{
    /// <summary>
    /// Interaction logic for CustomFolderDialog.xaml
    /// </summary>
    public partial class CustomFolderDialog : Window
    {
        public string SelectedFolderPath { get; private set; }
        public CustomFolderDialog()
        {
            InitializeComponent();
        }
        private void LoadDrives()
        {
            foreach (var drive in DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed))
            {
                var rootFolder = new FolderItem(drive.RootDirectory.FullName);
                folderTreeView.Items.Add(rootFolder);
            }
        }

        private void FolderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // Handle selection change if needed
        }

        private void SelectFolder_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected folder
            var selectedFolder = (FolderItem)folderTreeView.SelectedItem;

            if (selectedFolder != null)
            {
                MessageBox.Show($"Selected Folder: {selectedFolder.Path}");
                // Close the dialog or perform other actions
            }
            else
            {
                MessageBox.Show("Please select a folder.");
            }
        }
    }
}
