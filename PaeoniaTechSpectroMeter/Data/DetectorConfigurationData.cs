//using MCPNet.Motion;
//using MCPNet.Motion.PosModel;
using MCPNet.Data.ValueData;
using MCPNet.UI;
using PaeoniaTechSpectroMeter.Model;
using System;
using System.ComponentModel;
using System.IO;
using Utilities;

namespace PaeoniaTechSpectroMeter.Data
{
    public class DetectorConfigurationData : StationData
    {

        static string ValueGroupID = "Common Values";
        static string LisaParam = "General Parameters";
        static string LisaPulseParam = "Sync/Pulse";

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion
        [Category("Lisa Sync /Pulse Setting")]
        public IntValueData Resolution
        {
            get { return resolution; }
            set { resolution.CopyFrom(value); }
        }
        protected IntValueData resolution = new IntValueData("Resolution", 4, 128, 4, ValueGroupID,
                                                                        LisaParam,
                                                                        true, "Cm^-1", UserAccessControl.CommonValueEntryFeatureName);


        [Category("Lisa Sync /Pulse Setting")]
        public IntValueData PixelSize
        {
            get { return pixelSize; }
            set { pixelSize.CopyFrom(value); }
        }
        protected IntValueData pixelSize = new IntValueData("Pixel Size", 128, 512, 128, ValueGroupID,
                                                                        LisaPulseParam,
                                                                        true, "Pixels", UserAccessControl.CommonValueEntryFeatureName);

        [Category("Lisa Sync /Pulse Setting")]
        public DecimalValueData Samplingfreq
        {
            get { return samplingfreq; }
            set { samplingfreq.CopyFrom(value); }
        }
        protected DecimalValueData samplingfreq = new DecimalValueData("Sampling Freq", 1, 100, 3, ValueGroupID,
                                                                        LisaPulseParam,
                                                                        true, "Hz", UserAccessControl.CommonValueEntryFeatureName);


        [Category(" Lisa Sync /Pulse Setting")]
        public DecimalValueData Delaytime
        {
            get { return delaytime; }
            set { delaytime.CopyFrom(value); }
        }
        protected DecimalValueData delaytime = new DecimalValueData("Interval", 1000, 1000000, 20000, ValueGroupID,
                                                                        LisaPulseParam,
                                                                        true, "us", UserAccessControl.CommonValueEntryFeatureName);


        [Category(" Lisa Sync /Pulse Setting")]
        public DecimalValueData Pulsewidth
        {
            get { return pulsewidth; }
            set { pulsewidth.CopyFrom(value); }
        }
        protected DecimalValueData pulsewidth = new DecimalValueData("Pulse Width", 10000, 1000000, 166500, ValueGroupID,
                                                                        LisaPulseParam,
                                                                        true, "us", UserAccessControl.CommonValueEntryFeatureName);

        [Category("Lisa Sync /Pulse Setting")]
        public DecimalValueData Lptime
        {
            get { return lptime; }
            set { lptime.CopyFrom(value); }
        }
        protected DecimalValueData lptime = new DecimalValueData("Lp Time", 1, 200, 166, ValueGroupID,
                                                                        LisaPulseParam,
                                                                        true, "ms", UserAccessControl.CommonValueEntryFeatureName);


        [Category("Lisa Sync /Pulse Setting")]
        public DecimalValueData Integrtime
        {
            get { return integrtime; }
            set { integrtime.CopyFrom(value); }
        }
        protected DecimalValueData integrtime = new DecimalValueData("Integrl Time", 1, 200, 166, ValueGroupID,
                                                                        LisaPulseParam,
                                                                        true, "ms", UserAccessControl.CommonValueEntryFeatureName);

        [Category("Lisa Sync /Pulse Setting")]
        public DecimalValueData Vvrtime
        {
            get { return vvrtime; }
            set { vvrtime.CopyFrom(value); }
        }
        protected DecimalValueData vvrtime = new DecimalValueData("VVR Time", 0.001, 1000.000, 666, ValueGroupID,
                                                                        LisaPulseParam,
                                                                        true, "us", UserAccessControl.CommonValueEntryFeatureName);

        [Category("Lisa Sync /Pulse Setting")]
        public DecimalValueData Vdrtime
        {
            get { return vdrtime; }
            set { vdrtime.CopyFrom(value); }
        }
        protected DecimalValueData vdrtime = new DecimalValueData("VDR Time", 10000, 1000000, 166700, ValueGroupID,
                                                                        LisaPulseParam,
                                                                        true, "us", UserAccessControl.CommonValueEntryFeatureName);



        [Category(" Lisa Sync /Pulse Setting")]
        public BoolValueData Active
        {
            get { return active; }
            set { active.CopyFrom(value); }
        }
        protected BoolValueData active = new BoolValueData("Active", true, ValueGroupID, LisaPulseParam,
                                                                  UserAccessControl.SuperUserAccessFeatureName,
                                                                  "High", "Low");


        [Category(" Lisa Sync /Pulse Setting")]
        public BoolValueData Direction
        {
            get { return direction; }
            set { direction.CopyFrom(value); }
        }
        protected BoolValueData direction = new BoolValueData("Direction", true, ValueGroupID, LisaPulseParam,
                                                                  UserAccessControl.SuperUserAccessFeatureName,
                                                                  "Output", "Input");




        protected override void GeneratePosDataList()
        {

        }




        public override void InitInfo()
        {

            GenerateDataModel();
        }

        public DetectorConfigurationData()

        {

            FileName = "DetectorConfigData";


        }


        public static void SaveConfig(DetectorConfigurationData cfg)
        {
            string fullPath = Path.Combine(SystemPath.RootDirectory, "DetectorConfigData");
            Serializer.SaveXml(typeof(DetectorConfigurationData), cfg, fullPath);
        }
        public static DetectorConfigurationData LoadConfig()
        {
            // string fullPath=null;
            string fullPath = Path.Combine(SystemPath.RootDirectory, "DetectorConfigData");
            return (DetectorConfigurationData)Serializer.LoadXml(typeof(DetectorConfigurationData), fullPath);

        }
        public void RaisePropertyChanged(string propName)
        {

            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }



    }
}
