using PaeoniaTechSpectroMeter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
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
    /// Interaction logic for BrowseLocationDialog.xaml
    /// </summary>
    public partial class BrowseLocationDialog : Window
    {
        private FolderItem _selectedFolder;

        public string SelectedPath { get; private set; }

        public BrosweLocationViewModel brosweLocationViewModel { get; set; } = new BrosweLocationViewModel();

        public BrowseLocationDialog()
        {
            InitializeComponent();
            LoadDrives();
            DataContext = brosweLocationViewModel;
        }

        private void LoadDrives()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var documentsFolder = new FolderItem(documentsPath, "Documents", null);
            FolderWatcher myDocumentsWatcher = new FolderWatcher(documentsFolder, this);
            //folderTreeView.Items.Add(documentsFolder);
            AddFilteredSubfolders(documentsFolder);

            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable)
                {
                    string label = string.IsNullOrEmpty(drive.VolumeLabel) ? drive.Name : drive.VolumeLabel;
                    var rootFolder = new FolderItem(drive.RootDirectory.FullName, label, drive);
                    Task.Delay(2000).Wait();


                    FolderWatcher myUsbWatcher = new FolderWatcher(rootFolder, this);
                    folderTreeView.Items.Add(rootFolder);
                    InitializeFolderItem(rootFolder);
                }
            }

        }

        private void OnLocationSelected(string selectedPath)
        {
            SelectedPath = selectedPath;
            // You can perform additional actions if needed
        }
        private void OnUsbDeviceChange(object sender, EventArrivedEventArgs e)
        {
            // Handle USB device change event
            // Refresh your UI or remove the corresponding item
            //RefreshDrives();
        }

        private void RefreshDrives()
        {
            // Refresh your UI or remove the corresponding item
            // Iterate through existing items and remove those that are no longer available
            foreach (FolderItem item in folderTreeView.Items.OfType<FolderItem>().ToList())
            {
                if (item.DriveInfo != null && !item.DriveInfo.IsReady)
                {
                    //RemoveDriveFromUI(item);
                    RemoveDriveFromUI(item);
                }
            }

            // Add new drives
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable && drive.IsReady)
                {
                    string label = string.IsNullOrEmpty(drive.VolumeLabel) ? drive.Name : drive.VolumeLabel;
                    var rootFolder = new FolderItem(drive.RootDirectory.FullName, label, drive);

                    FolderWatcher myUsbWatcher = new FolderWatcher(rootFolder, this);
                    folderTreeView.Items.Add(rootFolder);
                    InitializeFolderItem(rootFolder);

                }
            }
        }

        private void RemoveDriveFromUI(FolderItem rootFolder)
        {
            // Find and remove the corresponding item from the UI
            var driveItem = folderTreeView.Items.Cast<FolderItem>().FirstOrDefault(item => item.Path == rootFolder.Path);
            if (driveItem != null)
            {
                folderTreeView.Items.Remove(driveItem);
            }
        }


        public void InitializeFolderItem(FolderItem folderItem)
        {
            folderItem.FolderDeleted += OnFolderDeleted;
        }

        private void OnFolderDeleted(object sender, string fullPath)
        {
            var deletedFolder = (FolderItem)sender;

            var parentFolder = FindParentFolder(fullPath);

            // If the parent folder is found, find and remove the TreeViewItem
            if (parentFolder != null)
            {
                parentFolder.SubFolders.Remove(deletedFolder);
                var parentTreeViewItem = FindTreeViewItem(folderTreeView, parentFolder);
                if (parentTreeViewItem != null)
                {
                    var deletedTreeViewItem = FindTreeViewItem(parentTreeViewItem, deletedFolder);
                    if (deletedTreeViewItem != null)
                    {
                        parentTreeViewItem.Items.Remove(deletedTreeViewItem);
                        return;
                    }
                }
                RefreshFolderItem(parentFolder);
            }

            else
            {
                // If the deleted folder is a root folder, refresh all root items
                foreach (var rootItem in folderTreeView.Items.OfType<FolderItem>())
                {
                    RefreshFolderItem(rootItem);
                }
            }
        }

        private void RefreshFolderItem(FolderItem folder)
        {
            var treeViewItem = FindTreeViewItem(folderTreeView, folder);
            if (treeViewItem != null)
            {
                treeViewItem.Items.Refresh();
            }
        }

        private FolderItem FindParentFolder(string deletedFolderPath)
        {
            // Iterate through your folder items to find the parent folder
            foreach (var folderItem in folderTreeView.Items.OfType<FolderItem>())
            {
                if (deletedFolderPath.StartsWith(folderItem.Path, StringComparison.OrdinalIgnoreCase))
                {
                    return folderItem;
                }
            }

            return null;
        }

        private void AddFilteredSubfolders(FolderItem folder)
        {
            if (folder.Name.Equals("Documents", StringComparison.OrdinalIgnoreCase))
            {
                folderTreeView.Items.Add(folder);
            }
            // Add your condition to exclude specific subfolders
            var filteredSubfolders = folder.SubFolders.Where(subfolder => !FolderItem.IsExcludedFolder(subfolder.Name)).ToList();

            // Clear existing subfolders and add the filtered ones
            folder.SubFolders.Clear();
            foreach (var subfolder in filteredSubfolders)
            {
                folder.SubFolders.Add(subfolder);
                InitializeFolderItem(subfolder); //added
            }
        }





        private void FolderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            _selectedFolder = e.NewValue as FolderItem;

        }

       
        private void BtnNewFolder_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFolder != null)
            {
                CreateFolder();
                NewFolderNameTextBox.Focus();
                NewFolderNameTextBox.ToolTip = "New Folder";
            }
            else
            {
                MessageBoxInfoDialog customDialog = new MessageBoxInfoDialog("Please select a folder before creating a new folder.");
                customDialog.Topmost = true;
                customDialog.ShowDialog();
                NewFolderNameTextBox.Focus();
            }
        }



        private TreeViewItem GetTreeViewItem(ItemsControl parent, object dataItem)
        {
            foreach (var item in parent.Items)
            {
                TreeViewItem treeViewItem = parent.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;

                if (treeViewItem != null)
                {
                    if (treeViewItem.Header == dataItem)
                    {
                        return treeViewItem;
                    }

                    // Recursively search in the tree view item's children
                    TreeViewItem childTreeViewItem = GetTreeViewItem(treeViewItem, dataItem);
                    if (childTreeViewItem != null)
                    {
                        return childTreeViewItem;
                    }
                }
            }

            return null;
        }


        public void RefreshTreeView(FolderItem selectedFolder)
        {
            selectedFolder.LoadSubFolders(); // Reload the subfolders for the selected folder

            var treeViewItem = FindTreeViewItem(folderTreeView, selectedFolder);

            if (treeViewItem != null)
            {
                treeViewItem.IsExpanded = true; // Expand the selected folder to show the new subfolder
                treeViewItem.IsSelected = true; // Select the new subfolder
            }
        }

        private TreeViewItem FindTreeViewItem(ItemsControl container, object itemToFind)
        {
            if (container != null)
            {
                foreach (var item in container.Items)
                {
                    if (item is TreeViewItem treeViewItem)
                    {
                        if (treeViewItem.DataContext == itemToFind)
                        {
                            return treeViewItem;
                        }

                        var resultContainer = FindTreeViewItem(treeViewItem, itemToFind);
                        if (resultContainer != null)
                        {
                            return resultContainer;
                        }
                    }
                }
            }

            return null;
        }

        //private void NewFolderNameTextBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (_selectedFolder != null)
        //    {

        //        if (e.Key == Key.Enter)
        //        {
        //            CreateFolder();
        //        }
        //    }
        //    else
        //    {

        //        MessageBoxInfoDialog customDialog = new MessageBoxInfoDialog("Please select a folder before creating a new folder.");
        //        customDialog.ShowDialog();
        //        NewFolderNameTextBox.Text = "";
        //    }
        //}

        private void CreateFolder()
        {
            if (_selectedFolder != null)
            {
                string newFolderName = NewFolderNameTextBox.Text.Trim();

                if (!string.IsNullOrEmpty(newFolderName))
                {
                    string newFolderPath = System.IO.Path.Combine(_selectedFolder.Path, newFolderName);

                    try
                    {
                        Directory.CreateDirectory(newFolderPath);
                        //await Task.Delay(500);
                        RefreshTreeView(_selectedFolder);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"Error creating new folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    NewFolderNameTextBox.Text = "";
                }
            }

            //NewFolderPanel.Visibility = Visibility.Collapsed;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            
            CreateFolder();
            var selectedFolder = (FolderItem)folderTreeView.SelectedItem;
            if (selectedFolder != null)
            {
                brosweLocationViewModel.UserChooseDir = selectedFolder.Path;
            }
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
