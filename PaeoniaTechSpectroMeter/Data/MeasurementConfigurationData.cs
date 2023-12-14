using MCPNet.Data.ValueData;
using MCPNet.UI;
using PaeoniaTechSpectroMeter.Model;
using System;
using System.ComponentModel;
using System.IO;
using Utilities;

namespace PaeoniaTechSpectroMeter.Data
{


    public class MeasurementConfigurationData : StationData
    {
        static string valueGroupID = "Common Values";
        static string messurementsetting = "General Parameters";
        static string LisaPulseParam = "Sync/Pulse";

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion

        [Category("General Lisa Setting")]
        public DecimalValueData Maxwavenumber
        {
            get { return maxwavenumber; }
            set { maxwavenumber.CopyFrom(value); }
        }
        protected DecimalValueData maxwavenumber = new DecimalValueData("Save Wavenumber From", 700, 1900, 1850, valueGroupID, messurementsetting, true, "cmˉ¹", UserAccessControl.CommonValueEntryFeatureName);

        [Category("General Lisa Setting")]
        public DecimalValueData Minwavenumber
        {
            get { return minwavenumber; }
            set { minwavenumber.CopyFrom(value); }
        }
        protected DecimalValueData minwavenumber = new DecimalValueData("Save Wavenumber To", 700, 1900,                                            800, valueGroupID, messurementsetting, true,  "cmˉ¹", UserAccessControl.CommonValueEntryFeatureName);


        [Category("General Measurement Setting")]
        public IntValueData Avaragecount
        {
            get { return avaragecount; }
            set { avaragecount.CopyFrom(value); }
        }
        protected IntValueData avaragecount = new IntValueData("Average", 1, 65000, 256, valueGroupID,
                                                               messurementsetting,true, "Scan",                                     UserAccessControl.CommonValueEntryFeatureName);

        [Category("General Measurement Setting")]
        public IntValueData RepeatMeasurement
        {
            get { return repeatMeasurement; }
            set { repeatMeasurement.CopyFrom(value); }
        }
        protected IntValueData repeatMeasurement = new IntValueData("Number of Data Set", 0, 1000000,                                           16,valueGroupID, messurementsetting, true, "Data",                                                 UserAccessControl.CommonValueEntryFeatureName);

        [Category("General Lisa Setting")]
        public IntValueData DelaybtwnMeasurement1
        {
            get { return delaybtwnMeasurement1; }
            set { delaybtwnMeasurement1.CopyFrom(value); }
        }
        protected IntValueData delaybtwnMeasurement1 = new IntValueData("Scan Interval Time", 0, 3600,                                                 1, valueGroupID, messurementsetting,true,                                            "Sec", UserAccessControl.CommonValueEntryFeatureName);



        [Category("General Lisa Setting")]
        public IntValueData DelaybtwnMeasurement
        {
            get { return delaybtwnMeasurement; }
            set { delaybtwnMeasurement.CopyFrom(value); }
        }
        protected IntValueData delaybtwnMeasurement = new IntValueData("", 0, 3600000, 100,                                                         valueGroupID,messurementsetting,true, "ms",                                                     UserAccessControl.CommonValueEntryFeatureName);
        [Category("General Lisa Setting")]
        public IntValueData DelaybtwnScans
        {
            get { return delaybtwnscans; }
            set { delaybtwnscans.CopyFrom(value); }
        }
        protected IntValueData delaybtwnscans = new IntValueData("Delay B/W Scans", 0, 1000, 1, valueGroupID, messurementsetting,true, "ms", UserAccessControl.CommonValueEntryFeatureName);

        protected override void GeneratePosDataList()
        {

        }




        public override void InitInfo()
        {

            GenerateDataModel();
        }

        public MeasurementConfigurationData()

        {

            FileName = "MeasuremnentData";


        }


        public static void SaveConfig(MeasurementConfigurationData cfg)
        {
            string fullPath = Path.Combine(SystemPath.RootDirectory, "MeasuremnentData");
            Serializer.SaveXml(typeof(MeasurementConfigurationData), cfg, fullPath);
        }
        public static MeasurementConfigurationData LoadConfig()
        {
            // string fullPath=null;
            string fullPath = Path.Combine(SystemPath.RootDirectory, "MeasuremnentData");
            return (MeasurementConfigurationData)Serializer.LoadXml(typeof(MeasurementConfigurationData), fullPath);

        }
        public void RaisePropertyChanged(string propName)
        {

            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }




    }
}
