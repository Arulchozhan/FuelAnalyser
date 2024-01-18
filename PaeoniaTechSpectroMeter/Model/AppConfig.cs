using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Utilities;


namespace PaeoniaTechSpectroMeter.Model
{
    public class AppConfig : FilterablePropertyBase, INotifyPropertyChanged
    {
        static string configName = "PaeoniaTechSpectroMeter.cfg";

        [XmlIgnore]
        public string AppExeName = Assembly.GetExecutingAssembly().GetName().Name;

        [XmlIgnore]
        public string AppVersion = "V1.002";// Assembly.GetExecutingAssembly().GetName().Version.ToString(); 

        [XmlIgnore]
        public string BuiltDate = File.GetCreationTime(Assembly.GetExecutingAssembly().Location).ToString("dd-MMM-yy [HH:mm]"); //  DateTime.Now.ToString("dd-MMM-yy [HH:mm]")  Assembly.GetExecutingAssembly().GetLinkerTime().ToString("dd-MMM-yy [HH:mm]");


        string appName = "FuelAnalyser";
        [Category("Application")]
        [Description("AppConfig")]
        public string AppName { get { return appName; } set { appName = value; OnPropertyChanged("AppName"); } }

       
        string acsEtherNetMotionIP = "192.168.157.20";
        [Category("Hardware Setting")]
        [Description(" IP")]
        public string AcsEtherNetMotionIP
        {
            get => acsEtherNetMotionIP;
            set
            {
                acsEtherNetMotionIP = value;
                OnPropertyChanged("IP");
            }
        }

        string perfchk = "OK";
        [Category("Hardware Setting")]
        [Description(" Performance")]
        public string Perfchk
        {
            get => perfchk;
            set
            {
                perfchk = value;
                OnPropertyChanged("Performance");
            }
        }
        string perfchktime = "dd-MMM-yy [HH:mm]";
        [Category("Hardware Setting")]
        [Description(" Performance Checked On")]
        public string PerfchkTime
        {
            get => perfchktime;
            set
            {
                perfchktime = value;
                OnPropertyChanged("PerformanceCheckedOn");
            }
        }

        string connectionString = "";
        [Category("Hardware Setting")]
        [Description("Sql Connection String")]
        public string ConnectionString
        {
            get => connectionString;
            set
            {
                connectionString = value;
                OnPropertyChanged("ConnectionString");
            }
        }
        string bgchktime = "dd-MMM-yy [HH:mm]";
        [Category("Hardware Setting")]
        [Description(" Last Background Saved On")]
        public string BgchkTime
        {
            get => bgchktime;
            set
            {
                bgchktime = value;
                OnPropertyChanged("PerformanceCheckedOn");
            }
        }

        string calib1Name = "Fuel Ethanol V1.0.01";
        [Category("EQUIPMENT INFORMATION")]
        [Description(" Calibration Module 1 Name and Version")]
        public string Calib1Name
        {
            get => calib1Name;
            set
            {
                calib1Name = value;
                OnPropertyChanged("Calib1Name");
            }
        }
        string calib1Instdate = "21-12-2023";
        [Category("EQUIPMENT INFORMATION")]
        [Description(" Calibration Module 1 Installation Date")]
        public string Calib1Instdate
        {
            get => calib1Instdate;
            set
            {
                calib1Instdate = value;
                OnPropertyChanged("Calib1Instdate");
            }
        }
        string calib2Name = "Fuel Methanol V1.0.01";
        [Category("EQUIPMENT INFORMATION")]
        [Description(" Calibration Module 2 Name and Version")]
        public string Calib2Name
        {
            get => calib2Name;
            set
            {
                calib2Name = value;
                OnPropertyChanged("Calib2Name");
            }
        }

        string calib2Instdate = "21-12-2023";
        [Category("EQUIPMENT INFORMATION")]
        [Description(" Calibration Module 2 Installation Date")]
        public string Calib2Instdate
        {
            get => calib2Instdate;
            set
            {
                calib2Instdate = value;
                OnPropertyChanged("Calib2Instdate");
            }
        }
        string instrumentinstdate = "21-12-2023";
        [Category("EQUIPMENT INFORMATION")]
        [Description(" Instrument Installation Date")]
        public string Instrumentinstdate
        {
            get => instrumentinstdate;
            set
            {
                instrumentinstdate = value;
                OnPropertyChanged("Instrumentinstdate");
            }
        }


        string customerInd = "XYZ Lab";
        [Category("COMPANY INFORMATION")]
        [Description("Company")]
        public string CustomerInd
        {
            get => customerInd;
            set
            {
                customerInd = value;
                OnPropertyChanged("CustomerInd");
            }
        }
        string instrumentSN = "ABC1234K";
        [Category("COMPANY INFORMATION")]
        [Description("Instrument Serial Number")]
        public string InstrumentSN
        {
            get => instrumentSN;
            set
            {
                instrumentSN = value;
                OnPropertyChanged("InstrumentSN");
            }
        }
        string folderPath = "C:\\\\Users\\\\MuruganArulchozhan\\.conda\\envs\\Novel_PAT\\Lib\\ite-packages\\";
        [Category("COMPANY INFORMATION")]
        [Description("Folder Path")]
        public string FolderPath
        {
            get => folderPath;
            set
            {
                folderPath = value;
                OnPropertyChanged("FolderPath");
            }
        }

        string instrumentFwv = "V1.0.01";
        [Category("COMPANY INFORMATION")]
        [Description("Instrument Firmware version")]
        public string InstrumentFwv
        {
            get => instrumentFwv;
            set
            {
                instrumentFwv = value;
                OnPropertyChanged("InstrumentFwv");
            }
        }



        string appLogDirectory = "";
        [Category("Application")]
        [Description("AppLogDirectory")]
        public string AppLogDirectory { get { return appLogDirectory; } set { appLogDirectory = value; OnPropertyChanged("AppLogDirectory"); } }

        bool simulateIO = true;


        string lastControlRecipeName = "";
        [Category("Last Recipe")]
        [Description("Last Control Recipe")]
        public string LastControlRecipeName { get { return lastControlRecipeName; } set { lastControlRecipeName = value; OnPropertyChanged("LastControlRecipeName"); } }




        public static void SaveConfig(AppConfig cfg)
        {
            string fullPath = Path.Combine(SystemPath.RootDirectory, configName);
            Serializer.SaveXml(typeof(AppConfig), cfg, fullPath);
        }
        public static AppConfig LoadConfig()
        {
            // string fullPath=null;
            string fullPath = Path.Combine(SystemPath.RootDirectory, configName);
            return (AppConfig)Serializer.LoadXml(typeof(AppConfig), fullPath);

        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion

    }
}
