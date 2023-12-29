using System.Diagnostics;
using System.IO;

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
                return RootInfoDirection; //exeDirectory 
            }

        }

        public static string RootInfoDirection = "C:\\FuelAnalyzer";
        public static string RootLogDirectory = "C:\\FuelAnalyzer Logs";

        public const string RecipeDirectory = "Recipe";
        public const string LogDirectory = "Log";
        public const string SystemDirectory = "System";
        public const string ControlDirectory = "Control";
        public const string TimingLogDirectory = "TimingLog";
        public const string LotExt = ".csv";
        public const string VisExt = "mvp";
        public const string ControlExt = "mcp";
        public const string BatExt = ".txt";
        public const string LogExt = ".log";
        public const string ImageExt = ".bmp";

        public const string ConfigFileName = "Config.xml";

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


        static public string GetSystemPath
        {
            get { return RootDirectory + "\\" + SystemDirectory; }
        }



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

            if (!excludelogs)
                CreateDirectoryIfDontHave(GetTimingPath);
            if (!excludelogs)
                CreateDirectoryIfDontHave(GetLogPath);
        }


        public void CreateDirectoryIfDontHave(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }



    }
}
