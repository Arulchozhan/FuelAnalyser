using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace PaeoniaTechSpectroMeter.Data
{
    public class ControlRecipe : PersistentXML, INotifyPropertyChanged
    {

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion
        protected string _name = "FuelAnalyzer_ControlRecipe";
        protected string _version = "0.0";

        DetectorConfigurationData detectorConfigurationData = null;
        public DetectorConfigurationData DetectorConfigurationData
        {
            get { return detectorConfigurationData; }
            set
            {
                detectorConfigurationData = value;
                OnPropertyChanged("DetectorConfigurationData");
            }
        }



        public ControlRecipe()
        {
            CreateDataObjectIfNecessary();
        }

        void CreateDataObjectIfNecessary()
        {

            if (detectorConfigurationData == null)
                detectorConfigurationData = new DetectorConfigurationData();


        }
        public void InitialOnLoaded()
        {
            CreateDataObjectIfNecessary();

        }


        public void WriteDownObjectID(string tag)
        {
        }


    }
}
