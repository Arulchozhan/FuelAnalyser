using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace PaeoniaTechSpectroMeter.Model
{
    public class ReadDetector : UserControl, INotifyPropertyChanged

    {



        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion

        private float detectorTemperature;

        public float DetectorTemperature
        {
            get => detectorTemperature;
            set => detectorTemperature = value;
        }


    }
}
