using System;
using System.Windows;
namespace PaeoniaTechSpectroMeter
{
    using FSM;
    using MessageHandler;
    using PaeoniaTechSpectroMeter.Database;
    using PaeoniaTechSpectroMeter.Model;
    using System.Configuration;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Input;
    using Utilities;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow MainWinInts = null;
        public static MainManager MainMngr = null;
        public static bool ErrorDuringStartup = true;
        public static WinWelcomeScreen WelcomeScreen = null;
        public static ErrorEventManager ErrorEventMgr = null;
        public static DispatcherUIHelper UIDispatcher = null;
        public static MCPNet.MCPNet McpNet = null;
        public static bool HasPauseStartUpKeyPressed = false;
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["FuelAnalyser"].ConnectionString;
        


        //Added method to check Database ready to proceed
      
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {            
                DatabaseInitializer initializer = new DatabaseInitializer(_connectionString); // Initialize the database
                initializer.InitializeDatabase();
                
                ApplicationStartup(null, null);// Continue with application startup
            }
            catch (Exception ex)
            {
                // Log or display the exception message
                MessageBox.Show($"Error initializing database: {ex.Message}");
            }
        }
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            string serr = "";
            string serrDetail = "";
            do
            {

                Process thisProc = Process.GetCurrentProcess();
                if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
                {
                    serr = "Ops! " + thisProc.ProcessName + " is Running..";
                    break;
                }

                try
                {
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                    Init();

                    ErrorDuringStartup = false;
                }
                catch (Exception ex)
                {
                    serr = ex.Message;
                    serrDetail = ex.ToString();
                }

            }
            while (false);
            CheckErrorOnStartUP(serr, serrDetail);
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                object o = e.ExceptionObject;
                Trace.Assert(false, "UnhandledException: " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm:ss") + " -> " + o.ToString());
            }
        }

        void Init()
        {
            CheckSpecialKeyPressed();
            HasPauseStartUpKeyPressed = Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.F);

            McpNet = new MCPNet.MCPNet();
            //      MvpNet = new MVPNet.MVPNet();
            UIDispatcher = new DispatcherUIHelper();
            ErrorEventMgr = new ErrorEventManager("error", "event");
            MainMngr = new MainManager();
            MainMngr.InitControlInstances();
            MainWinInts = new PaeoniaTechSpectroMeter.MainWindow(MainMngr);
            MainMngr.AssignMainWinInts(MainWinInts);

          //  WelcomeScreen = new WinWelcomeScreen();
           // Splasher.Splash = WelcomeScreen;
            MainMngr.LoadingStatus = "Loading...";

            Splasher.ShowSplash();
            MainMngr.LoadingStatus = "Checking Dll info...";


            McpNet.CheckAllDllInfo(); //test
            //        MvpNet.CheckAllDllInfo();
            Thread.Sleep(100);

            string serr = MainMngr.InitAll(ErrorEventMgr);
            if (serr != "")
                throw new Exception(serr);

            CheckSpecialKeyPressed();
            UIDispatcher.Init(MainWinInts);
            MainWinInts.Show();

            Thread.Sleep(100);
            //UIUtility.DispatcherHelper.DoEvents();
            CheckSpecialKeyPressed();
            Thread.Sleep(100);
            CheckSpecialKeyPressed();

            if (!HasPauseStartUpKeyPressed)
            {
                Splasher.CloseSplash();
            }
        }
        void CheckSpecialKeyPressed()
        {
            if (!HasPauseStartUpKeyPressed)
            {
                HasPauseStartUpKeyPressed = Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.F) ||
                                            Keyboard.IsKeyDown(Key.RightCtrl) && Keyboard.IsKeyDown(Key.F);
            }
        }

        void CheckErrorOnStartUP(string serr, string serrDetail)
        {
            if (serr != "")
            {
                try
                {
                    MessageBox.Show(serr);


                }
                catch (Exception) { }
                finally
                {
                    Application.Current.Shutdown();
                }
            }
        }





    }
}
