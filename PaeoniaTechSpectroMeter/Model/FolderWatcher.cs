using PaeoniaTechSpectroMeter.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PaeoniaTechSpectroMeter.Model
{
    public class FolderWatcher : IDisposable
    {
        private readonly FileSystemWatcher watcher;
        private readonly FolderItem folderItem;

        private readonly BrowseLocationDialog browseLocationDialog;

        public event EventHandler FolderChanged;

        private bool isHandlingEvent = false;

        private readonly ManagementEventWatcher usbWatcher;

        public FolderWatcher(FolderItem folderItem, BrowseLocationDialog browseLocationDialog)
        {
            this.folderItem = folderItem;
            this.browseLocationDialog = browseLocationDialog;

            watcher = new FileSystemWatcher();
            watcher.Path = folderItem.Path;
            watcher.IncludeSubdirectories = true;

            watcher.Created += OnFolderChanged;
            watcher.Deleted += OnFolderChanged;
            watcher.Renamed += OnFolderRenamed;

            watcher.EnableRaisingEvents = true;

            usbWatcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2 or EventType = 3");
            usbWatcher.EventArrived += OnUsbDriveChange;
            usbWatcher.Query = query;
            usbWatcher.Start();
        }



        private void InitializeFolderItem(FolderItem folderItem)
        {
            // Your initialization code here
        }


        private void OnFolderChanged(object sender, FileSystemEventArgs e)
        {
            HandleFolderChange();
        }

        private void OnFolderRenamed(object sender, RenamedEventArgs e)
        {
            HandleFolderChange();
        }

        private void OnUsbDriveChange(object sender, EventArrivedEventArgs e)
        {
            HandleUSBChange();
        }

        private void HandleFolderChange()
        {
            if (!isHandlingEvent)
            {
                isHandlingEvent = true;
                browseLocationDialog.RefreshTreeView(folderItem);
                isHandlingEvent = false;
            }

            FolderChanged?.Invoke(this, EventArgs.Empty);



        }

        private void HandleUSBChange()
        {
            foreach (FolderItem item in browseLocationDialog.folderTreeView.Items.OfType<FolderItem>().ToList())
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

                    FolderWatcher myUsbWatcher = new FolderWatcher(rootFolder, browseLocationDialog);
                    browseLocationDialog.Dispatcher.Invoke(() =>
                    {
                        var existingItem = browseLocationDialog.folderTreeView.Items.Cast<FolderItem>().FirstOrDefault(item => item.Path == rootFolder.Path);

                        if (existingItem != null)
                        {
                            existingItem.Name = rootFolder.Name;
                            existingItem.Icon = rootFolder.Icon;
                        }
                        else
                        {
                            // The item doesn't exist, add it
                            browseLocationDialog.folderTreeView.Items.Add(rootFolder);
                        }
                    });
                    InitializeFolderItem(rootFolder);

                }
            }
        }
 
        private void RemoveDriveFromUI(FolderItem rootFolder)
        {
            browseLocationDialog.Dispatcher.Invoke(() =>
            {
                var driveItem = browseLocationDialog.folderTreeView.Items.Cast<FolderItem>().FirstOrDefault(item => item.Path == rootFolder.Path);
                if (driveItem != null)
                {
                    browseLocationDialog.folderTreeView.Items.Remove(driveItem);
                }
            });
        }

        public void Dispose()
        {
            // Disable the watcher and unsubscribe from events
            watcher.EnableRaisingEvents = false;
            watcher.Created -= OnFolderChanged;
            watcher.Deleted -= OnFolderChanged;
            watcher.Renamed -= OnFolderRenamed;

            usbWatcher.Stop();
            usbWatcher.EventArrived -= OnUsbDriveChange;
            usbWatcher.Dispose();

            watcher.Dispose();
        }
    }
}
