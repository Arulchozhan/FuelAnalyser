using PaeoniaTechSpectroMeter.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace PaeoniaTechSpectroMeter.Data
{
    public class RunDataInfo : FilterablePropertyBase, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion
        AppConfig appCfg = null;

        public void Init(AppConfig appCfg)
        {
            this.appCfg = appCfg;
        }
    }
}
