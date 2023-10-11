using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;
using Utilities;
//using fiberattach.Data;
//using System.Drawing;
using DefinationAndConveter;
using MCPNet.Data.ValueData;
using MCPNet.UI;

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


        string appName = "PaeoniaTechSpectroMeter";
        [Category("Application")]
        [Description("AppConfig")]
        public string AppName { get { return appName; } set { appName = value; OnPropertyChanged("AppName"); } }

        bool simulateMotors = true;
        [Category("HardWare Simulation")]
        [Description("Motor Simulation")]
        [TypeConverter(typeof(YesNoPropertyType))]
        public bool SimulateMotors { get { return simulateMotors; } set { simulateMotors = value; OnPropertyChanged("SimulateMotors"); } }

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
            return (AppConfig)Serializer.LoadXml(typeof(AppConfig),  fullPath);
         
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
