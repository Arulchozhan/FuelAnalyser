using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaeoniaTechSpectroMeter.Model
{
    public class BrosweLocationViewModel
    {
        private string userChooseDir = "";

        public string UserChooseDir
        {
            get => userChooseDir;
            set
            {
                userChooseDir = value;
                OnPropertyChanged(nameof(UserChooseDir));
            }
        }

        // Implement INotifyPropertyChanged (if not done already)
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
