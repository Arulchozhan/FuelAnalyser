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

        MeasurementConfigurationData measurementConfigurationData = null;
        public MeasurementConfigurationData MeasurementConfigurationData
        {
            get { return measurementConfigurationData; }
            set
            {
                measurementConfigurationData = value;
                OnPropertyChanged("MeasurementConfigurationData");
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
            if (measurementConfigurationData == null)
                measurementConfigurationData = new MeasurementConfigurationData();



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
