using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace PaeoniaTechSpectroMeter.Model
{
    public class SystemPath
    {
        public static string exeDirectory = Directory.GetCurrentDirectory();
        public static string RootDirectory
        {
            get
            {
                if (Debugger.IsAttached) return RootInfoDirection;
                return exeDirectory;
            }

        }

        public static string RootInfoDirection = "C:\\FuelAnalyzer";
        public static string RootLogDirectory = "C:\\FuelAnalyzer Logs";

        public const string RecipeDirectory = "Recipe";
       // public const string LastCntDirectory = "LastCnt";
        public const string LogDirectory = "Log";
        //public const string LotDirectory = "Lot";
        public const string SystemDirectory = "System";
      //  public const string WorkOrderDirectory = "WorkOrder";
        //public const string VisionDirectory = "Vision";
        public const string ControlDirectory = "Control";
        public const string TimingLogDirectory = "TimingLog";
       // public const string CapturedImageDirectory = "CapturedImage";
        public const string LotExt = ".csv";
        public const string VisExt = "mvp";
        public const string ControlExt = "mcp";
        public const string BatExt = ".txt";
        public const string LogExt = ".log";
        public const string ImageExt = ".bmp";

        public const string ConfigFileName = "Config.xml";
/*
        public const string AxesConfigFileName = "AxesConfig.xml";
        public const string AxesSpeedFileName = "AxesSpeed.xml";
        public const string AxesTableFileName = "AxesTable.xml";
        public const string AxesMiscFileName = "AxesMisc.xml";

        public const string TowerLightFileName = "TowerLightConfig.xml";
        public const string OtherDataFileName = "OtherData.xml";

        public const string IoCardFileName = "IoCard.xml";
        public const string InputFileName = "Input.xml";
        public const string OutputFileName = "Output.xml";
        */

        static public string GetTimingPath
        {
            get { return RootLogDirectory + "\\" + TimingLogDirectory; }
        }

        static public string GetRecipePath
        {
            get { return RootInfoDirection + "\\" + RecipeDirectory; }
        }

        static public string GetControlRecipePath
        {
            get { return RootInfoDirection + "\\" + RecipeDirectory + "\\" + ControlDirectory; }
        }

      /*  static public string GetVisionRecipePath
        {
            get { return RootInfoDirection + "\\" + RecipeDirectory + "\\" + VisionDirectory; }
        }
        */
        static public string GetSystemPath
        {
            get { return RootDirectory + "\\" + SystemDirectory; }
        }
/*
        static public string GetLastCntPath
        {
            get { return RootInfoDirection + "\\" + LastCntDirectory; }
        }

        static public string GetWorkOrderPath
        {
            get { return RootInfoDirection + "\\" + WorkOrderDirectory; }
        }
        static public string GetCapturedImagePath
        {
            get { return RootInfoDirection + "\\" + CapturedImageDirectory; }
        }
         static public string GetLotPath
        {
            get { return RootInfoDirection + "\\" + LotDirectory; }
        }
        */


        static public string GetLogPath
        {
            get { return RootLogDirectory + "\\" + LogDirectory; }
        }

       
        public string LoadedSystemPath
        {
            get { return GetSystemPath; }
        }

        public void CreateDefaultDirectory(bool excludelogs = false)
        {
            CreateDirectoryIfDontHave(RootInfoDirection);
            CreateDirectoryIfDontHave(RootLogDirectory);
            CreateDirectoryIfDontHave(GetRecipePath);
            CreateDirectoryIfDontHave(GetSystemPath);
            CreateDirectoryIfDontHave(GetControlRecipePath);
/*
            CreateDirectoryIfDontHave(GetLastCntPath);
            CreateDirectoryIfDontHave(GetWorkOrderPath);
            CreateDirectoryIfDontHave(GetCapturedImagePath);
            CreateDirectoryIfDontHave(GetLotPath);
            CreateDirectoryIfDontHave(GetVisionRecipePath);
            */

            if (!excludelogs)
                CreateDirectoryIfDontHave(GetTimingPath);
            if (!excludelogs)
                CreateDirectoryIfDontHave(GetLogPath);
        }
        void CreateDirectoryIfDontHave(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }



    }
}
