using PaeoniaTechSpectroMeter.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PaeoniaTechSpectroMeter.Model
{
    public class FolderItem
    {
        private string _name;
        private string _path;
        private string _label;
        private string _icon;
        private ObservableCollection<FolderItem> _subFolders;

        public event EventHandler<string> InitializeFolderItemRequested;


        private string _folderPath;

        public event EventHandler<string> FolderDeleted;

        public DriveInfo DriveInfo { get; private set; } 

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }

        public string Label
        {
            get { return _label; }
            set
            {
                if (_label != value)
                {
                    _label = value;
                    OnPropertyChanged(nameof(Label));
                }
            }
        }
        public string Icon
        {
            get { return _icon; }
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    OnPropertyChanged(nameof(Icon));
                }
            }
        }



        public ObservableCollection<FolderItem> SubFolders
        {
            get { return _subFolders; }
            set
            {
                if (_subFolders != value)
                {
                    _subFolders = value;
                    OnPropertyChanged(nameof(SubFolders));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //public ObservableCollection<FolderItem> SubFolders { get; }

        public FolderItem(string path, string label, DriveInfo drive)
        {
            Name = drive != null ? label : System.IO.Path.GetFileName(path);
            Path = path;
            Label = label;
            DriveInfo = drive;

            SubFolders = new ObservableCollection<FolderItem>();

            if (drive != null && drive.DriveType == DriveType.Removable)
            {
                // Icon = new BitmapImage(new Uri("pack://application:,,,/YourAssemblyName;component/Resources/ThumbDriveIcon.png"));
                Icon = @"C:\FuelAnalyzer\bin\Icon\USB_Icon.png";
            }
            else
            {
                // Icon = new BitmapImage(new Uri("pack://application:,,,/YourAssemblyName;component/Resources/FolderIcon.png"));
                Icon = @"C:\FuelAnalyzer\bin\Icon\Documents_Icon.png";
            }


            try
            {
                foreach (var subfolder in Directory.GetDirectories(path))
                {
                    //if (IsUnderDesktop(subfolder))
                    //{
                    //SubFolders.Add(new FolderItem(subfolder, null, null));
                    var subfolderItem = new FolderItem(subfolder, null, null);
                    subfolderItem.Icon = @"C:\FuelAnalyzer\bin\Icon\Folder_Icon.png";
                    SubFolders.Add(subfolderItem);
                    //}
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // Handle or log the exception as needed
                Console.WriteLine($"Access denied: {ex.Message}");
            }
            //InitializeFileSystemWatcher();
        }



        public void LoadSubFolders()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SubFolders.Clear();

                try
                {
                    string[] subfolderPaths = Directory.GetDirectories(Path);

                    foreach (string subfolderPath in subfolderPaths)
                    {
                        string subfolderName = System.IO.Path.GetFileName(subfolderPath);

                        // Add your condition to exclude specific folders
                        if (!IsExcludedFolder(subfolderName))
                        {
                            var subfolderItem = new FolderItem(subfolderPath, null, null);
                            subfolderItem.Icon = @"C:\FuelAnalyzer\bin\Icon\Folder_Icon.png";
                            SubFolders.Add(subfolderItem);
                            (Application.Current.MainWindow as BrowseLocationDialog)?.InitializeFolderItem(subfolderItem);
                        }

                    }
                    OnPropertyChanged(nameof(SubFolders));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading subfolders: {ex.Message}");
                }
            });
        }

        public static bool IsExcludedFolder(string folderName)
        {
            // Add your conditions to exclude specific folders
            return folderName.Trim().Equals("My Pictures", StringComparison.OrdinalIgnoreCase) ||
                   folderName.Trim().Equals("My Music", StringComparison.OrdinalIgnoreCase) ||
                   folderName.Trim().Equals("My Videos", StringComparison.OrdinalIgnoreCase) ||
                   folderName.Trim().Equals("Python Scripts", StringComparison.OrdinalIgnoreCase) ||
                   folderName.Trim().Equals("SQL Server Management Studio", StringComparison.OrdinalIgnoreCase) ||
                   folderName.Trim().Equals("Custom Office Templates", StringComparison.OrdinalIgnoreCase) ||
                   folderName.Trim().Equals("Visual Studio 2017", StringComparison.OrdinalIgnoreCase) ||
                   folderName.Trim().Equals("Visual Studio 2019", StringComparison.OrdinalIgnoreCase) ||
                   folderName.Trim().Equals("IISExpress", StringComparison.OrdinalIgnoreCase) ||
                   folderName.Trim().Equals("My Web Sites", StringComparison.OrdinalIgnoreCase) ||
                   folderName.Trim().Equals("Dell", StringComparison.OrdinalIgnoreCase);
        }
    }
}
