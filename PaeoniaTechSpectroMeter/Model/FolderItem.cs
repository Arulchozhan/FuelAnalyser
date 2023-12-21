using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaeoniaTechSpectroMeter.Model
{
    public class FolderItem
    {
        public string Name { get; }
        public string Path { get; }
        public ObservableCollection<FolderItem> SubFolders { get; }

        public FolderItem(string path)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = path;
            SubFolders = new ObservableCollection<FolderItem>();

            foreach (var subfolder in Directory.GetDirectories(path))
            {
                SubFolders.Add(new FolderItem(subfolder));
            }
        }
    }
}
