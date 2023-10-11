//using GSpcIOLib;
//using fiberattach.Station;
//using MCPNet.IO.Config.Card;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using UserAccess;

namespace PaeoniaTechSpectroMeter.Model
{
    // using Commands;
    using COMPorts;
    using FSM;
    // using whiteshadow.Data;
    using MCPNet.MachineMonitor;
    using MessageHandler;
    using PaeoniaTechSpectroMeter.Commands;
    using PaeoniaTechSpectroMeter.Data;
    //using Newtonsoft.Json;
    using PaeoniaTechSpectroMeter.Station;
    //using AnalogOutputDevices;
    using StorageMonitorService;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Threading;
    using Utilities;

    public class MainManager : DispatcherObject, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion


        
        
        string loadingStatus = "Loading...";
        public string LoadingStatus
        {
            get { return loadingStatus; }
            set
            {
                loadingStatus = value;
                UpdateMessage(value);
            }
        }

        void UpdateMessage(string msg)
        {
            if (Splasher.Splash == null) return;

            Action ac = new Action(() => { MessageListener.Instance.ReceiveMessage(msg); });
            if (Dispatcher.CheckAccess())
                ac();
            else
                Application.Current.Dispatcher.BeginInvoke(ac);
        }

      

        public enum AccessLevel
        {
            [Description("Operator")]
            OPERATOR = 0,
            [Description("Technician")]
            TECHNICIAN = 1,
            [Description("Engineer")]
            ENGINEER = 2,
            [Description("Superuser")]
            SUPERUSER = 3,
            [Description("Developer")]
            DEVELOPER = 4,
        }

        StateMachineController mainSMC = null;
        public StateMachineController MainSMC
        {
            get
            {

                return mainSMC;
            }
        }
     
       
        MachineStatusMonitor mcStatusMonitor = null;
        public MachineStatusMonitor McStatusMonitor
        {
            get { return mcStatusMonitor; }
        }

#pragma warning disable CS0649 // Field 'MainManager.mcEvents' is never assigned to, and will always have its default value null
          MachineEvents mcEvents;
#pragma warning restore CS0649 // Field 'MainManager.mcEvents' is never assigned to, and will always have its default value null
           public MachineEvents McEvents
           {
               get { return mcEvents; }
           }
          




        AppConfig appConfig = null;
        public AppConfig AppConfig
        {
            get { return appConfig; }
        }

        DetectorConfigurationData detectorConfigurationData = null;
        public DetectorConfigurationData DetectorConfigurationData
        {
            get { return detectorConfigurationData; }
        }

        COMPortsManager comMgr = null;
        public COMPortsManager ComMgr
        {
            get { return comMgr; }
        }

        

        SystemPath systemPath = null;
        public SystemPath SystemPath
        {
            get { return systemPath; }
        }

        RecipeManager recipeMngr = null;
        public RecipeManager RecipeMngr
        {
            get { return recipeMngr; }
        }
     
      MainWindow mainWinInts = null;
        public MainWindow MainWinInts
        {
            get { return mainWinInts; }
        }

        ErrorEventManager errorEventMngr = null;
        public ErrorEventManager ErrorEventMngr
        {
            get { return errorEventMngr; }
        }

        UserAccessControl accessControl = null;
        public UserAccessControl AccessControl
        {
            get { return accessControl; }
        }
       
        UserLogging userLogin = null;
        public UserLogging UserLogin
        {
            get { return userLogin; }
        }

        TowerLight towerLight = null;
        public TowerLight TowerLight
        {
            get { return towerLight; }
        }

        RunDataInfo runDataInfo = null;
        public RunDataInfo RunDataInfo
        {
            get { return runDataInfo; }
        }

      
       


       
          SPC spc_convertion = null;
        public SPC Spc_convertion
        {
            get { return spc_convertion; }
        }



        LogWritter.LogWriter referencedata = new LogWritter.LogWriter();
        public LogWritter.LogWriter Referencedatalog
        {
            get { return referencedata; }
        }


      





        StorageMonitor cleaner = null;
        public StorageMonitor Cleaner { get { return cleaner; } }

        public ICommand VisionRecipeSaveCommand { get; set; }
        public ICommand ControlRecipeSaveCommand { get; set; }
        public ICommand ShowStationStateDebugInfoCommand { get; set; }
        public ICommand ShowStateListWindowsCommand { get; set; }
        public ICommand ShowStationRunStatusCommand { get; set; }

        public ICommand ShowLaserCOMSettingCommand { get; set; }
        public ICommand ShowLaserCOMTerminalCommand { get; set; }
        public ICommand ShowMotionProfileTestCommand { get; set; }
        public ICommand ShowCompensationConfigEditorCommand { get; set; }
        public ICommand ShowProcessCongrolConfigEditorCommand { get; set; }

        public ICommand ShowMCPDllVersionCommand { get; set; }
        public ICommand ShowHelperDllVersionCommand { get; set; }
        public ICommand ShowMVPDllVersionCommand { get; set; }
        public ICommand ShowSyncManagerCommand { get; set; }
        public ICommand ShowVisionRunTimeDataCommand { get; set; }
        public ICommand ShowVisionInspectionDataCommand { get; set; }
        public ICommand ShowZoneEditorCommand { get; set; }
        public ICommand ShowZonesMonitorCommand { get; set; }

        public ICommand ShowPowerMapWindowCommand { get; set; }
        public ICommand ShowActiveAlignDataCommand { get; set; }
        public ICommand ShowPowerScannerCommand { get; set; }
        public ICommand ShowPowerScaleGraphCommand { get; set; }

        public ICommand ShowMusashiCOMSettingCommand { get; set; }
        public ICommand ShowPowerMeterCOMSettingCommand { get; set; }
        public ICommand ShowPowerMeterCOMTerminalCommand { get; set; }

        public ICommand ShowPowerSourceDevCOMSettingCommand { get; set; }
        public ICommand ShowPowerSourceDevCOMTerminalCommand { get; set; }

        public ICommand ShowSecondPowerMeterDevCOMSettingCommand { get; set; }
        public ICommand ShowSecondPowerMeterDevCOMTerminalCommand { get; set; }

        public ICommand OpenDispenserToolCalibrationCommand { get; set; }
        public ICommand ShowFiberPowerSourceDeviceTestCommand { get; set; }
        public ICommand ShowHillClimbDataChartWindowCommand { get; set; }

        public ICommand ShowOfflineDebuggingFlagsCommand { get; set; }
        public ICommand ShowAdvancedOptionsCommand { get; set; }

        public ICommand ShowProductionOptionsCommand { get; set; }
        public ICommand ShowStorageMonitorCommand { get; set; }

        public String SDir = SystemPath.RootDirectory;
        bool isBusy = false;
        public bool IsBusy { get { return isBusy; } private set { isBusy = value; OnPropertyChanged("IsBusy"); } }

        void RegisterCustomMotionProfiles()
        {
            

        }


        public string LoadMotorConfig()
        {
            string serr = "";
            try
            {
              
            }
            catch (Exception ex)
            {
                serr = ex.Message;
            }
            return serr;
        }


        void RegisterStationMachineEvents()
        {
            mainSMC.OnStartInitalRun -= mainSMC_OnStartInitalRun;
            mainSMC.OnStartInitalRun += mainSMC_OnStartInitalRun;

            mainSMC.OnStoppedInitalRun -= mainSMC_OnStoppedInitalRun;
            mainSMC.OnStoppedInitalRun += mainSMC_OnStoppedInitalRun;

            mainSMC.OnStartEveryAutoRun -= mainSMC_OnStartEveryAutoRun;
            mainSMC.OnStartEveryAutoRun += mainSMC_OnStartEveryAutoRun;

            mainSMC.OnStartAutoRun -= mainSMC_OnStartAutoRun;
            mainSMC.OnStartAutoRun += mainSMC_OnStartAutoRun;

            mainSMC.OnStoppedAutoRun -= mainSMC_OnStoppedAutoRun;
            mainSMC.OnStoppedAutoRun += mainSMC_OnStoppedAutoRun;

            mainSMC.OnStoppedResetRun -= mainSMC_OnStoppedResetRun;
            mainSMC.OnStoppedResetRun += mainSMC_OnStoppedResetRun;
        }
      
     
        public void InitStations()
         {

        
 }


         bool mainSMC_OnStartEveryAutoRun(SMCEventArgs args)
         {
             bool success = false;
             do
             {



#pragma warning disable CS0219 // The variable 'stopping' is assigned but its value is never used
                 bool stopping = false;
#pragma warning restore CS0219 // The variable 'stopping' is assigned but its value is never used
                 //stop bh btm camera... 
                 //if (visionMngr.ImgProcessors.Length >= 4)
                 //    visionMngr.ImgProcessors[3].TryStopContinousGrab(ref stopping);

                 CheckBasicSupplyAndSafetyDoors();

                 EnableInterlockForAllMotors();

                 string runmode = mainSMC.IsSingleCycleMode ? "Single Cycle" : "Auto Run";
                 errorEventMngr.ProcessEvent(runmode + " Resumed");

                 success = true;
             }



             while (false);
             return success;
         }

         bool mainSMC_OnStoppedAutoRun(SMCEventArgs args)
         {
             try
             {

                 EnableInterlockForAllMotors();
                 //               output.ReleaseFrontDoorLock();

                 if (mainSMC.HasSingleCycleComplete ||
                     mainSMC.HasAutoCycleComplete)
                 {
                     mainSMC.ResetCurrentStatePointers();
                     if (mainSMC.HasSingleCycleComplete)
                         mcStatusMonitor.SetOverallStatus("Single Cycle Completed!");
                     else if (mainSMC.HasAutoCycleComplete)
                         mcStatusMonitor.SetOverallStatus("Auto Cycle Completed!");
                 }

                 string runmode = mainSMC.IsSingleCycleMode ? "Single Cycle" : "Auto Run";
                 errorEventMngr.ProcessEvent(runmode + " Stopped!");
             }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
             catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
             {

             }
             //
             return true;
         }

         bool mainSMC_OnStoppedResetRun(SMCEventArgs args)
         {
            /* try
             {

                 EnableInterlockForAllMotors();

                 for (int i = 0; i < mainSMC.StateMachinesStations.Count; i++)
                 {
                     StateMachineStation st = mainSMC.StateMachinesStations[i];
                  
                     int errorCode = st.CheckForAllClear();
                     if (errorCode != FSMInnerErrorCode.NO_ERROR)
                     {
                         string serr = StationCustomErrorCodes.GetErrorDescription(errorCode, st.Name);
                         mcStatusMonitor.SetMachineStatus(MachineStatus.Error);
                         ShowMessageBox(serr, StatusInfoType.Warning);
                     }
                 }
             }
             catch (Exception ex)
             {
                 ShowMessageBox(ex.Message, StatusInfoType.Error);
             }
             */
             return true;
         }

         bool mainSMC_OnStoppedInitalRun(SMCEventArgs args)
         {
             /*try
             {
                 EnableInterlockForAllMotors();
                 for (int i = 0; i < mainSMC.StateMachinesStations.Count; i++)
                 {
                     StateMachineStation st = mainSMC.StateMachinesStations[i];
                     int errorCode = st.CheckForAllClear();
                    if (errorCode != FSMInnerErrorCode.NO_ERROR)
                     {
                         string serr = StationCustomErrorCodes.GetErrorDescription(errorCode, st.Name);
                         mcStatusMonitor.SetMachineStatus(MachineStatus.Error);
                         ShowMessageBox(serr, StatusInfoType.Warning);
                     }
                 }
             }
             catch (Exception ex)
             {
                 ShowMessageBox(ex.Message, StatusInfoType.Error);
             }
             */
             return true;
         }

         MessageBoxImage GetMessageIconByInfoType(StatusInfoType infoType)
         {
             switch (infoType)
             {
                 case StatusInfoType.Error:
                     return MessageBoxImage.Error;
                 case StatusInfoType.Statement:
                     return MessageBoxImage.Information;
                 case StatusInfoType.Warning:
                     return MessageBoxImage.Warning;
                 default:
                     break;
             }
             return MessageBoxImage.Information;
         }

         string GetTitleMessageByInfoType(StatusInfoType infoType)
         {
             switch (infoType)
             {
                 case StatusInfoType.Error:
                     return "Error";
                 case StatusInfoType.Statement:
                     return "Information";
                 case StatusInfoType.Warning:
                     return "Warning";
                 default:
                     break;
             }
             return "Unknwon";
         }

         AutoResetEvent waitMainMessageBoxClose = new AutoResetEvent(false);
         public void ShowMessageBox(string message, StatusInfoType infoType)
         {
             Action ac = new Action(() =>
             {
                 string title = GetTitleMessageByInfoType(infoType);
                 MessageBoxImage img = GetMessageIconByInfoType(infoType);
                 MessageBox.Show(mainWinInts, message, title, MessageBoxButton.OK,
                                 img);
                 waitMainMessageBoxClose.Set();

                 mcStatusMonitor.SetOverallStatus(message, infoType);
             });

             if (Dispatcher.CheckAccess())
             {
                 ac();
             }
             else
             {
                 Dispatcher.BeginInvoke(ac);
                 bool ok = false;
                 while (!ok)
                 {
                     ok = waitMainMessageBoxClose.WaitOne(10);
                     Thread.Sleep(10);
                     if (ok) break;
                 }
             }

         }
         bool mainSMC_OnStartAutoRun(SMCEventArgs args)
         {
             bool success = false;
             do
             {
                 if (args.Handled)
                     break;


                 CheckBasicSupplyAndSafetyDoors();

                 //processControlCfg.ElevatorIndexingTest = false;
                 if (mainSMC.IsSingleCycleMode)
                 {
                     if (!PromptSingleCycleSelection())
                     {
                         mcStatusMonitor.SetOverallStatus("User cancelled Single Cycle Run Mode!");
                         args.Handled = true;
                         break;
                     }
                 }

                 EnableInterlockForAllMotors();

                 string runmode = mainSMC.IsSingleCycleMode ? "Single Cycle" : "Auto Run";
                 errorEventMngr.ProcessEvent(runmode + " Start");


                 success = true;
             }
             while (false);
             return success;
         }
         private void CheckBasicSupplyAndSafetyDoors()
         {
            
         }

         bool PromptSingleCycleSelection()
         {
             /* try
              {
                  WinSingleCycleSelection win = new WinSingleCycleSelection();

                  if (!(bool)win.ShowDialog()) return false;

                  //                runDataInfo.AutoProcessingLens = runDataInfo.ManualSelectedLens;
                  processControlCfg.SingleCyclePanel = win.Panel;
                  processControlCfg.SingleCyclePcb = win.LF;
                  //runDataInfo.ManualSelectedLens          = processControlCfg.SingleCycleLens;
                  //runDataInfo.CurrentPCBUnit              = win.Pcb;
                  //runDataInfo.CurrentPanel                = win.Panel;

                  return true;
              }
              catch (Exception ex)
              {
                  MessageBox.Show(ex.Message);
                  return false;
              }
              */

            return true;
        }

        bool mainSMC_OnStartInitalRun(SMCEventArgs args)
        {
            bool success = false;
            do
            {

             /*   MessageBoxResult r = App.UIDispatcher.ShowMessageBox(StationCustomMessages.SystemHomingWarming,
                                                                     "Homing???", MessageBoxButton.YesNo,
                                                                    MessageBoxImage.Warning);

                if (r != MessageBoxResult.Yes)
                {
                    mcStatusMonitor.SetOverallStatus("User Canceled");
                    args.Handled = true;
                   break;
                }
                */

               // if (!input.Simulation && input.IpAirPressureSw.IsOff())
                   // throw new Exception("Main air Pressure is not enough!");

              
                EnableInterlockForAllMotors();
               // motors.DisableAll();
                Thread.Sleep(100);
                string serr = "";
              //  string serr = output.SetRelayOutputOn();
                if (serr != "")
                    throw new Exception(serr);

                Thread.Sleep(100);
           /*     {
                    if (!output.HasRelayOn)
                        throw new Exception("Relays failed to ON");
                }
                */

                EnableInterlockForAllMotors();
                success = true;
            }
            while (false);
            return success;
        }
        void EnableInterlockForAllMotors()
        {
          //  for (int i = 0; i < Motors.AllMotors.Count; i++)
            //    Motors.AllMotors[i].DisableMotionInterlock = false;
        }


        void LoadCameraOffsetConfig()
        {
            LoadingStatus = "Loading Camera Offset Config";
            //toolCameraCalibration = ToolCameraCalibrations.LoadConfig();
            //if (toolCameraCalibration == null) 
            //{
            //    toolCameraCalibration = new ToolCameraCalibrations();
            //    ToolCameraCalibrations.SaveConfig(toolCameraCalibration);
            //}
            errorEventMngr.ProcessEvent("Camera Offset Loaded");
        }

        void LoadAppConfig()
        {
            LoadingStatus = "Loading Application Config";
            appConfig = AppConfig.LoadConfig();
            if (appConfig == null)
            {
                appConfig = new AppConfig();
                AppConfig.SaveConfig(appConfig);
            }
            hasAppConfigLoaded = true;
            errorEventMngr.ProcessEvent("App Config Loaded");
        }

        void LoadDataConfig()
        {
            LoadingStatus = "Loading Data Config";
           
            hasAppConfigLoaded = true;
            errorEventMngr.ProcessEvent("Data Config Loaded");
        }

        void LoadProductionConfig()
        {
          
        }
        void LoadProcessControlConfig()
        {
           
        }

        bool hasAppConfigLoaded = false;
        public bool HasAppConfigLoaded { get { return hasAppConfigLoaded; } }

        bool isLoginFailed = false;
        public bool IsStartUpLoginFailed { get { return isLoginFailed; } }
        public string InitialTryLogIn()
        {

            if (Debugger.IsAttached)
            {
                userLogin.SetDefaultUser();
                if (userLogin.CurrentUser != null) return "";
            }

            WinUserLogin login = new WinUserLogin(userLogin);
            login.ShowDialog();
            if (userLogin.CurrentUser == null)
            {
                isLoginFailed = true;
                return "Log in Required!";
            }

            return "";
        }

        private string InitMotorModule()
        {
            string serr = "";
            /*
            string serr = "";
            do
            {
                LoadingStatus = "Loading Motor Configuration!";
                serr = LoadMotorConfig();
                if (serr != "")
                    break;

                errorEventMngr.ProcessEvent("Motors Config Loaded");

                LoadingStatus = "Initializing Motors";

                //int i = output.ACSChannel.GetDefaultTimeout();
                if (output.ACSChannel != null && appConfig.UseACSSingleChannel)

                    SPiiPlus660.AssignChannel(output.ACSChannel);

                serr = motors.Init(appConfig);
                if (serr != "")
                    break;

                errorEventMngr.ProcessEvent("Motors Initialized");
                interlocks.Setup(input, output, motors, appConfig);//, appPosZones
                interlocks.CreateCustomDelegates();
                LoadingStatus = "Motors OK";

                //Motion Profiles
                LoadingStatus = "Initializing Motion Profiles";
                mtrProfileMgr.Init(SystemPath.GetSystemPath, motors.AllMotors);
                //RegisterCustomMotionProfiles();
                mtrProfileMgr.Load();
                UpdateOptionsForMotionProfileRequrement();
                errorEventMngr.ProcessEvent("Motion Profiles Initialized and Loaded");
                LoadingStatus = "Motion Profiles OK";

                //Moiton Zone
                LoadingStatus = "Initializing Motion Zones";
               // appPosZones.InitAllZone(motors);
                errorEventMngr.ProcessEvent("Motion Zone Initialized and Loaded");
                LoadingStatus = "Motion Zones OK";
            }
            while (false);
            */
            return serr;

        }

        private string InitIOModule()
        {
            string serr = "";
           
            return serr;
        }
        void mcStatusMonitor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IsBusy = mcStatusMonitor.IsBusy;
        }

        private void AssignTowerLightOutputs()
        {
           
        }
        public void UpdateCustomTowerLightSetting()
        {
          
        }
        void input_OnEMOSwitchOnDetected(object sender, EventArgs e)
        {
            OnEMODetected();
        }
        void UpdateOptionsForMotionProfileRequrement()
        {
          

        }

        void InitUserAccess()
        {
            accessControl.Init(appConfig.AppExeName);
            userLogin.LoadUserInfo();
        }

        public void LoadLastUsedControlRecipes()
        {
            recipeMngr.CollectAvailableControlRecipes();
            recipeMngr.LoadLastUsedControlRecipe();
            if (recipeMngr.IsControlRecipeLoaded)
            {
                errorEventMngr.ProcessEvent("Control Recipe Loaded");
            }
            recipeMngr.ControlRecipe.InitialOnLoaded();

        }

        public void InitCommands()
        {
            ControlRecipeSaveCommand = new MainManagerBasicCmd(this, SaveControlRecipe, mainWinInts);
        }


        public void InitControlInstances()
        {

         
            systemPath = new SystemPath();
            //ioCard = new IOCard();
            recipeMngr = new RecipeManager();
            comMgr = new COMPortsManager();
            appConfig = new AppConfig();

            mainSMC = new StateMachineController();
            mcStatusMonitor = new MachineStatusMonitor();
            accessControl = new UserAccessControl();
            userLogin = new UserLogging(SystemPath.RootDirectory + "\\");
          
            towerLight = new TowerLight();
            runDataInfo = new RunDataInfo();
            cleaner = new StorageMonitor();
            //serialtest = new Serialtest(this);
           // lisa = new Lisa_(this);
          //  modelData = new ModelData();
         //   pmut_inst = new pMUT_(this);
            spc_convertion = new SPC();
            referencedata = new LogWritter.LogWriter();
    
        }
        public void AssignMainWinInts(MainWindow mainWinInts)
        {
            this.mainWinInts = mainWinInts;
        }

        private void ShowStorageMonitorWindow()
        {
            mainWinInts.OpenStorageMonitorStatus();
        }

        private void ShowProductionOptions()
        {
         //   mainWinInts.ShowProductionOptionWindow();
        }

        private void ShowOfflineDebuggingFlagWindow()
        {
          //  mainWinInts.ShowOfflineDebugFlagWindow();
        }
        private void ShowAdvancedOptionsWindow()
        {
            //mainWinInts.ShowAdvancedOptionsWindow();
        }
        private void ShowPowerScaleGraphWindow()
        {
            // mainWinInts.ShowPowerScaleGraph();
        }
        private void ShowPowerMapWindow()
        {
          //  mainWinInts.ShowPowerMapVisualizerWindow();
        }
        private void ShowSecondPowerMeterCOMTerminal()
                {
               /*   if (txPowerMeterDevice is PFSeriesPowerMeter)
                    {
                       powerScanner.StopReading();
                    }

                    COMPort cp = comMgr.GetCOMPortObjectByID(appConfig.PowerMeterDeviceCOMID);
                    if (cp != null)
                    {
                        mainWinInts.ShowCOMTerminal(cp);
                    }
                    */
                }
      

        public void ShowPowerMeterCOMSetting()
        {
            /*
            powerScanner.StopReading();
            mainWinInts.ShowCOMSetting(txPowerMeterDevice.Comport);
            txPowerMeterDevice.Init(appConfig.SimulatePowerMeter, txPowerMeterDevice.Comport);
            if (!appConfig.SimulatePowerMeter && !txPowerMeterDevice.CheckOnLine())
            {
                throw new Exception("Power Meter Deivce is not on line!");
            }
            powerScanner.StartReading(readingLens: (ProcessLens)runDataInfo.ManualSelectedLens); */
        }

        private void ShowFiberPowerSourceDevieTestWindow()
                {
             //       mainWinInts.ShowFiberPowerSourceDevieTester(fiberPowerSourceDevice);
                }

                private void ShowPowerSourceCOMTerminal()
                {
                //    mainWinInts.ShowCOMTerminal(fiberPowerSourceDevice.ComPort);
                }

                private void ShowPowerSourceCOMSetting()
                {
                  // mainWinInts.ShowCOMSetting(fiberPowerSourceDevice.ComPort);
                }

                private void ShowMusashiCOMSettingWindow()
                {
                  //  mainWinInts.ShowCOMSetting(musashihDispenser.Comport);
                }

                private void ShowPowerMeterCOMTerminal()
                {
                // powerScanner.StopReading();
                // mainWinInts.ShowCOMTerminal(txPowerMeterDevice.Comport);
                }
        private void ShowPowerScannerWindow()
        {
            //mainWinInts.ShowPowerScannerWindow();
        }

        private void ShowStationRunStatusWindows()
        {
          //  mainWinInts.ShowStationsRunStatusWindow();
        }
        /*
                private void ShowActiveAlignDataWindow()
                {
                 //   mainWinInts.ShowActiveAlignmentDataWindow();
                }

                private void ShowHillClimbChartDataWindow()
                {
                  //  mainWinInts.ShowActiveHillClimbChartDataWindow();
                }
                private void ShowPowerMapWindow()
                {
                 //   mainWinInts.ShowPowerMapVisualizerWindow();
                }
                private void ShowDispensingToolCalibrationWindow()
                {
                 //   mainWinInts.ShowToolCameraCalibrationWindow(toolCameraCalibration.DispensingTool, motors.BondDispenserX, motors.BondDispenserY, motors.BondDispenserZ, null);
                }
                private void ShowPowerScannerWindow()
                {
                   // mainWinInts.ShowPowerScannerWindow();
                }*/
        private void ShowVisionInspecitonData()
        {
            //    mainWinInts.ShowAllVisionInspectionData();
        }

        private void ShowVisionRunData()
        {
            //   mainWinInts.ShowAllVisionRunData();
        }

        private void ShowProcessControlConfigEditor()
        {
            //visionKitManager.UpdateDrawDrawingEnableToProcessControl();
            //mainWinInts.ShowProcessControlObjectEditor();
            //visionKitManager.UpdateDrawingEnableOptionsFromProcessControl();
        }

        private void ShowPosZoneMonitor()
        {
            //mainWinInts.ShowZonesMonitor();
        }
        private void ShowPosZoneEditor()
        {
          // mainWinInts.ShowZoneManager();
        }

        private void ShowMCPDllVersions()
        {
            //mainWinInts.ShowMcpDllVersionInfo();
        }
        private void ShowHelperDllVersions()
        {
            //mainWinInts.ShowHelperDllVersionInfo();
        }
        private void ShowMVPDllVersions()
        {
          //  mainWinInts.ShowMVPDllVersionInfo();
        }
        private void ShowEventSycnManagers()
        {
            //mainWinInts.ShowSyncManagerStatus(leftBHStn.SyncEventMgr);
            //mainWinInts.ShowSyncManagerStatus(rightBHStn.SyncEventMgr);
        }


      
        public void LoadInputConfiguration()
        {
            

        }

        public void LoadOutputConfiguration()
        {
           
        }

        void CloseIO()
        {
          
        }

        void DisableAllMotors()
        {
           
        }

        private void ReleaseUnitLocking()
        {
        }
       
        public void LoadControlRecipe(string recipeName)
        {
            LoadingStatus = "Control Recipe Loading...[" + recipeName + "]";
            bool loadOK = recipeMngr.LoadControlRecipe(recipeName);
            LoadingStatus = loadOK ? string.Format("Control recipe [{0}] laoded", recipeName) : string.Format("Control recipe [{0}] Load Error", recipeName);
            //ReasLoadingStatussign Profiles for pos

        }
        public void AssignStationsRecipeData()
        {
            ControlRecipe ctrlRecipe = recipeMngr.ControlRecipe;
            AssignData(ctrlRecipe.DetectorConfigurationData);
            //AssignFTIRData(ctrlRecipe.FTIR_Param);
           // AssignanalysisData(ctrlRecipe.Analysis_data);
           // AssignPLSData(ctrlRecipe.pls_data);
         
        }
        public void AssignData(DetectorConfigurationData detectorConfigurationData)
        {
           this.detectorConfigurationData = detectorConfigurationData;
           //lisa.Stepdata = cameradata.Stepvalue.HoldValueString;
        }

        public void AssignFTIRData()//FTIR_Param fTIR_Param)
        {
          //  this.FTIR_Param_ = fTIR_Param;
           // lisa.Stepdata = fTIR_Param.Stepvalue.HoldValueString;
            // lisa.Stepdata = cameradata.Stepvalue.HoldValueString;
        }

       
        public void SaveControlRecipe()
        {
            recipeMngr.SaveControlRecipe();
        }
       
        public void ShowMotionProfileTest()
        {
          //  mainWinInts.ShowMotionProfileTest();
        }
       
        public void ShowStateListWindows()
        {
            mainWinInts.ShowStationStateListWindows();
        }

        public void ShowStationStateDebugWindow()
        {
            mainWinInts.ShowStationStateDebugWindows();
        }

        public void OnEMODetected()
        {

        }
        internal void UpdateErrorEventLogFlags()
        {
        }



        public string InitAll(ErrorEventManager eeMgr)
        {
            string serr = "";
            do
            {

                //StationCustomErrorCodes.InitErrorMap();

                errorEventMngr = eeMgr;
                systemPath.CreateDefaultDirectory(true);
                //
                LoadingStatus = "Loading User Access Configurations";
                InitUserAccess();
                serr = InitialTryLogIn();
                if (serr != "")
                    break;

                LoadingStatus = "Log in OK";
                mcStatusMonitor.Init(mainSMC, recipeMngr, towerLight);
                mcStatusMonitor.PropertyChanged -= mcStatusMonitor_PropertyChanged;
                mcStatusMonitor.PropertyChanged += mcStatusMonitor_PropertyChanged;

                LoadAppConfig();
               // LoadDataConfig();
                if (appConfig.AppLogDirectory != "" &&
                    Directory.Exists(appConfig.AppLogDirectory))
                {
                    SystemPath.RootLogDirectory = appConfig.AppLogDirectory;
                }
                eeMgr.Init("", SystemPath.GetLogPath);
                systemPath.CreateDefaultDirectory();

                //cleaner
                cleaner.Init(SystemPath.SystemDirectory);
              //  cleaner.OnCleaningDone += Cleaner_OnCleaningDone;

                LoadProcessControlConfig();
                UpdateErrorEventLogFlags();

                LoadProductionConfig();
                //motors.Simulation = appConfig.SimulateMotors;
                runDataInfo.Init(appConfig);
               serr = InitIOModule();
                if (serr != "")
                    break;

                serr = InitMotorModule();
                if (serr != "")
                    break;

               /* LoadingStatus = "COM Ports Initialization";
                comMgr.Init(SystemPath.RootDirectory + "\\", 3);
                LoadingStatus = "COM Ports OK";

                // LoadingStatus = "Vision Initialization";
                //       InitVision();
                // errorEventMngr.ProcessEvent("Vision Initialized");
                //  LoadingStatus = "Initializing Camera Offset Calibration Data";
                //LoadCameraOffsetConfig();
                //LoadingStatus = "Vision Calibration Data OK";

                //Init Power Meters
                LoadingStatus = "Init Power Meter";
                string simu = appConfig.SimulatePowerMeter ? " [Simulation]" : "";
                string tserr = InitPowerMeterDevices();
                if (tserr != "")
                    MessageBox.Show(tserr);
                else
                    LoadingStatus = "Power Meter OK" + simu;


    */
                //Init Stations
                LoadingStatus = "Init Stations";
                InitStations();
                LoadingStatus = "Stations OK";

              
  

                //recipeMngr.Init(visionMngr);
                LoadLastUsedControlRecipes();
                // stations.AssignStationsRecipeData();
                AssignStationsRecipeData();
               //  tcpServer.CreateHandle(appConfig.PlcDefultNetid);

                //  motors.DisableAll();
                Thread.Sleep(500);
                errorEventMngr.ProcessEvent("Ready");
                InitCommands();
               // serialtest.cratedatalog();
                Create_referencedataLog();
                //lisa.AssignData(cameradata);
               // lisa.AssignFTIRData(FTIR_Param_);
                //lisa.AssignanalysisData(Analysis_data_);
               // lisa.AssignPl
                
               // lisa.Refdatalog_Read();

            }
            while (false);

            return serr;
        }
        
        public void Create_referencedataLog()
        {

            try
            {
              String SDir = SystemPath.RootDirectory;
              
                referencedata.IsAppendFileCount = false;
                referencedata.IsMonthlyBasedFolder = false;
                referencedata.IsTimeStampPrefixOnFileName = false;
                referencedata.InitLogFile(SDir, "Baseline.csv");
                if (!File.Exists("C:\\PaeoniaTechSpectroMeter\\Baseline.csv"))
                {
                    for (int i = 0; i < 128; i++)
                    {
                        //Referencedatalog.WriteToLog((1).ToString("F3"), true);
                    }
                    //  referencedata.HeaderString = " Output(v)";
                }

              //  lisa.Refdatalog_Read();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }
        public void CloseAll()
        {
            //if (powerScanner != null)
            //    powerScanner.StopReading();

            ReleaseUnitLocking();

            CloseIO();
            DisableAllMotors();
           // CloseVisions();

        }
       
      
    }
}
