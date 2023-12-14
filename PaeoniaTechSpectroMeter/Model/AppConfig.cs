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
        public string AppVersion = "0.0.0.1";// Assembly.GetExecutingAssembly().GetName().Version.ToString(); 

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
