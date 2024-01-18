using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PaeoniaTechSpectroMeter
{
    using COMPorts;
    using CustomWindow;
    using FSM;
    using FSM.UI;
    using MessageHandler;
    using PaeoniaTechSpectroMeter.Interface;
    using PaeoniaTechSpectroMeter.Model;
    using PaeoniaTechSpectroMeter.Views;
    using StorageMonitorService.UI;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using UserAccess;
    using Utilities;
    using UserControl = System.Windows.Controls.UserControl;
    public partial class MainWindow : Window, IDispatcherUIDisplay
    {
        MainManager mmgr = null;
        CtrlAppConfig ctrlAppConfig = null;
        CtrlFactorySetting ctrlFactorySetting = null;
        CtrlMeasurement ctrlMeasurement = null;
        CtrlSelfDiagnostics ctrlSelfDiagnostics = null;
        CtrlHistory ctrlHistory = null;
        CtrlMeasurementSetting ctrlMeasurementSetting = null;
        RecipeManager reMgr = null;
        List<UserControl> customControls = new List<UserControl>();
        List<StandardWindow> imageViewWindowList = new List<StandardWindow>();


        CtrlMeasurement measurementPage;
        CtrlSelfDiagnostics selfDiagnosticsPage;
        CtrlHistory historyPage;
        CtrlFactorySetting factoryPage;
        CtrlMeasurementSetting measurementSettingPage;
        CtrlAppConfig advancedConfigPage;

        public MainWindow(MainManager mmgr)
        {
            InitializeComponent();
            this.mmgr = mmgr;
            this.reMgr = mmgr.RecipeMngr;
            this.Title = mmgr.AppConfig.AppName; //+ " [" + mmgr.AppConfig.AppVersion + "] Built < " + mmgr.AppConfig.BuiltDate + " >"

        }

        bool forceClose = true;
        int controlrecipeIndex = -1;
        private void CbControlRecipe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            try
            {

                bool runninghalfway = !mmgr.MainSMC.IsStartingFromDefaultState();

                string msg = string.Format("Recipe  cannot change duing running half way."
                                                        + Environment.NewLine +
                                                        "Need to Reset or Initialise Fist");


                if (runninghalfway)
                {
                    MessageBox.Show(msg, "Cannot Change in middle of running");
                    if (controlrecipeIndex >= 0)
                    {
                        // skiprecipeSelection = true;
                        CbControlRecipeList.SelectedIndex = controlrecipeIndex;
                        // skiprecipeSelection = false;
                    }
                    return;
                }

                string selectedName = (string)CbControlRecipeList.SelectedItem;
                mmgr.LoadControlRecipe(selectedName);
                controlrecipeIndex = CbControlRecipeList.SelectedIndex;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SetupPannels();

                //TabControl_SelectionChanged(TabCtrlSetup, null);
             //   LblAppName.Content = mmgr.AppConfig.AppName;
             //  LblVersion.Content = mmgr.AppConfig.AppVersion + " Built < " + mmgr.AppConfig.BuiltDate + " >";

                reMgr.PropertyChanged -= ReMgr_PropertyChanged;
                reMgr.PropertyChanged += ReMgr_PropertyChanged;

                mmgr.MainSMC.StartMonitor();

                mmgr.MainSMC.OnStartAutoRun -= MainSMC_OnStatAutoRun;
                mmgr.MainSMC.OnStartAutoRun += MainSMC_OnStatAutoRun;

                mmgr.MainSMC.OnStartEveryAutoRun -= MainSMC_OnEveryStatAutoRun;
                mmgr.MainSMC.OnStartEveryAutoRun += MainSMC_OnEveryStatAutoRun;

                this.DataContext = mmgr;


              //  LstBoxMsg.Setup(mmgr.ErrorEventMngr, this);


                RefershUISetup();
                mmgr.UserLogin.OnLoginLevelUpdated -= UserLogin_OnLoginLevelUpdated;
                mmgr.UserLogin.OnLoginLevelUpdated += UserLogin_OnLoginLevelUpdated;

                TabCtrlSetup.SelectionChanged -= Tabpage_updated;
                TabCtrlSetup.SelectionChanged += Tabpage_updated;
                UpdateLayout();
                 DispatcherTimer LiveTime = new DispatcherTimer();
                LiveTime.Interval = TimeSpan.FromSeconds(1);
                LiveTime.Tick += timer_Tick;
                LiveTime.Start();
            


            forceClose = false;
                if (!App.HasPauseStartUpKeyPressed)
                    MessageListener.Instance.ClearList();
                else
                    App.WelcomeScreen.Owner = this;
                Action ac = new Action(() =>
                {
                    //UIUtility.DispatcherHelper.DoEvents();
                    //Thread.Sleep(20);
                    UpdateAllControlsAccess();
                    DetermineAccess();
                });

                Dispatcher.BeginInvoke(ac);

                mmgr.ErrorEventMngr.ProcessEvent("Main Application Window Loaded");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                Debug.WriteLine("Main win Loaded" + Environment.NewLine +
                                ex.ToString());

                forceClose = true;
                Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// 
        void timer_Tick(object sender, EventArgs e)
        {
            CurrentDateandTime.Text = DateTime.Now.ToString("dd MMM yyyy HH:mm");
        }
     
        void SetupPannels()
        {
            /// <Factoryseeting>
            /// in this pannel we set the detctor/emitter pulse setting 
            /// </Factoryseeting>

            measurementPage = new CtrlMeasurement(mmgr);
            selfDiagnosticsPage = new CtrlSelfDiagnostics(mmgr);
            historyPage = new CtrlHistory(mmgr);
            factoryPage = new CtrlFactorySetting();
            measurementSettingPage = new CtrlMeasurementSetting();
            advancedConfigPage = new CtrlAppConfig();

            //ctrlMeasurement = new CtrlMeasurement(mmgr);
            //MakeControlStrech(ctrlMeasurement);
            //GrdMeasurement.Children.Add(ctrlMeasurement);
            //RegisterCustomControl(ctrlMeasurement);

            //ctrlSelfDiagnostics = new CtrlSelfDiagnostics(mmgr);
            //MakeControlStrech(ctrlSelfDiagnostics);
            //GrdSelfDiagnostics.Children.Add(ctrlSelfDiagnostics);
            //RegisterCustomControl(ctrlSelfDiagnostics);


            //ctrlHistory = new CtrlHistory();
            //MakeControlStrech(ctrlHistory);
            //GrdHistory.Children.Add(ctrlHistory);
            //RegisterCustomControl(ctrlHistory);

            //ctrlMeasurementSetting = new CtrlMeasurementSetting();
            //MakeControlStrech(ctrlMeasurementSetting);
            //ctrlMeasurementSetting.Setup(mmgr); //.Cameradata
            //GrdMeasurementConfig.Children.Add(ctrlMeasurementSetting);
            //RegisterCustomControl(ctrlMeasurementSetting);


            //ctrlFactorySetting = new CtrlFactorySetting();
            //MakeControlStrech(ctrlFactorySetting);
            //ctrlFactorySetting.Setup(mmgr); //.Cameradata
            //GrdSystemConfig.Children.Add(ctrlFactorySetting);
            //RegisterCustomControl(ctrlFactorySetting);

            //ctrlAppConfig = new CtrlAppConfig();
            //ctrlAppConfig.Setup(mmgr.AppConfig);
            //MakeControlStrech(ctrlAppConfig);
            //GrdAppConfig.Children.Add(ctrlAppConfig);
            //RegisterCustomControl(ctrlAppConfig);

        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (TabItem tabItem in TabCtrlSetup.Items)
            {
                Image measurementIconImage = FindChild<Image>(tabItem, "MeasurementIconImage");
                TextBlock measurementTextBlock = FindChild<TextBlock>(tabItem, "MeasurementTextBlockTabItem");
                if (measurementTextBlock != null)
                {
                    measurementIconImage.Source = new BitmapImage(new Uri("../Icon/MeasurementB_Icon.png", UriKind.Relative));
                    measurementTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1A1A1A"));
                }

                Image selfDiagnosticsIconImage = FindChild<Image>(tabItem, "SelfDiagnosticsIconImage");
                TextBlock selfDiagnosticsTextBlockTabItem = FindChild<TextBlock>(tabItem, "SelfDiagnosticsTextBlockTabItem");
                if (selfDiagnosticsTextBlockTabItem != null)
                {
                    selfDiagnosticsIconImage.Source = new BitmapImage(new Uri("../Icon/Self-diagnosticsB_Icon.png", UriKind.Relative));
                    selfDiagnosticsTextBlockTabItem.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1A1A1A"));
                }

                Image historyIconImage = FindChild<Image>(tabItem, "HistoryIconImage");
                TextBlock historyTextBlockTabItem = FindChild<TextBlock>(tabItem, "HistoryTextBlockTabItem");
                if (historyTextBlockTabItem != null)
                {
                    historyIconImage.Source = new BitmapImage(new Uri("../Icon/HistoryB_Icon.png", UriKind.Relative));
                    historyTextBlockTabItem.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1A1A1A"));
                }
            }

            TabItem selectedTabItem = TabCtrlSetup.SelectedItem as TabItem; //Get the selected TabItem

            if (selectedTabItem != null)
            {
                this.Title = "Page - " + selectedTabItem.Header.ToString();
            }
            else
            {
                this.Title = "Home Page";
            }

            if (selectedTabItem.Header.ToString() == "Meausrement")
            {
                if (measurementPage ==null)
                {
                    measurementPage = new CtrlMeasurement(mmgr);
                }

                Image measurementIconImage = FindChild<Image>(selectedTabItem, "MeasurementIconImage");
                TextBlock measurementTextBlock = FindChild<TextBlock>(selectedTabItem, "MeasurementTextBlockTabItem");
                if (measurementTextBlock != null)
                {
                    measurementIconImage.Source = new BitmapImage(new Uri("../Icon/Measurement_Icon.png", UriKind.Relative));
                    measurementTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005fb8"));
                }

                page_Content.Content = measurementPage;
            }
            else if (selectedTabItem.Header.ToString() == "Self-diagnostics")
            {
                if (selfDiagnosticsPage == null)
                 selfDiagnosticsPage = new CtrlSelfDiagnostics(mmgr);
                Image selfDiagnosticsIconImage = FindChild<Image>(selectedTabItem, "SelfDiagnosticsIconImage");
                TextBlock selfDiagnosticsTextBlockTabItem = FindChild<TextBlock>(selectedTabItem, "SelfDiagnosticsTextBlockTabItem");
                if (selfDiagnosticsTextBlockTabItem != null)
                {
                    selfDiagnosticsIconImage.Source = new BitmapImage(new Uri("../Icon/Self-diagnostics_Icon.png", UriKind.Relative));
                    selfDiagnosticsTextBlockTabItem.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005fb8"));
                }
                page_Content.Content = selfDiagnosticsPage;
            }
            else if (selectedTabItem.Header.ToString() == "History")
            {
                //if (historyPage == null)
                     historyPage = new CtrlHistory(mmgr);
                Image historyIconImage = FindChild<Image>(selectedTabItem, "HistoryIconImage");
                TextBlock historyTextBlockTabItem = FindChild<TextBlock>(selectedTabItem, "HistoryTextBlockTabItem");
                if (historyTextBlockTabItem != null)
                {
                    historyIconImage.Source = new BitmapImage(new Uri("../Icon/History_Icon.png", UriKind.Relative));
                    historyTextBlockTabItem.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005fb8"));
                }
                page_Content.Content = historyPage;
            }
            else if (selectedTabItem.Header.ToString() == "Factory Setting")
            {
                if (factoryPage == null)
                     factoryPage = new CtrlFactorySetting();
                factoryPage.Setup(mmgr);
                page_Content.Content = factoryPage;
            }
            else if (selectedTabItem.Header.ToString() == "MeasurementSetting")
            {
                if (measurementSettingPage == null)
                 measurementSettingPage = new CtrlMeasurementSetting();
                measurementSettingPage.Setup(mmgr);
                page_Content.Content = measurementSettingPage;
            }
            else if (selectedTabItem.Header.ToString() == "AdvancedConfig")
            {
                if (factoryPage == null)
                   advancedConfigPage = new CtrlAppConfig();
                advancedConfigPage.Setup(mmgr.AppConfig);
                page_Content.Content = advancedConfigPage;
            }
        }

        private T FindChild<T>(DependencyObject parent, string childName) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T frameworkElement && frameworkElement.Name == childName)
                {
                    return frameworkElement;
                }

                T childOfChild = FindChild<T>(child, childName);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }

            return null;
        }

        private void TabItem_Unloaded(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = sender as TabItem;
            TextBlock measurementTextBlock = FindChild<TextBlock>(tabItem, "MeasurementTextBlockTabItem");
            if (measurementTextBlock != null)
            {
                measurementTextBlock.Foreground = Brushes.Black;
            }
        }

        void MakeControlStrech(UserControl uc)
        {
            uc.HorizontalAlignment =HorizontalAlignment.Stretch;
            uc.VerticalAlignment = VerticalAlignment.Stretch;
        }
        void RegisterCustomControl(UserControl uc)
        {
            if (!customControls.Contains(uc))
                customControls.Add(uc);
        }

        private void ReMgr_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ControlRecipe")
            {
                mmgr.AssignStationsRecipeData();
                RefershUISetup();
                //mmgr.AssignVisionLandMarkData();
                DetermineAccess();
            }


        }
        void DetermineAccess()
        {
            bool isAccessGranted = mmgr.UserLogin.CurrentAccessLevel >= AccessLevel.ENGINEER;
            mmgr.DetectorConfigurationData.Samplingfreq.IsVisible = isAccessGranted ? true : false;
            TabMeasurementSetting.Visibility = isAccessGranted ? Visibility.Visible : Visibility.Collapsed;
            GrdRightPannel.Visibility= isAccessGranted ? Visibility.Visible : Visibility.Collapsed;
            TabSystemConfig.Visibility = isAccessGranted ? Visibility.Visible : Visibility.Collapsed;
            TabAdvancedSystemConfig.Visibility = isAccessGranted ? Visibility.Visible : Visibility.Collapsed;
            isAccessGranted = mmgr.UserLogin.CurrentAccessLevel >= AccessLevel.DEVELOPER;
            mmgr.DetectorConfigurationData.PixelSize.AllowEdit = isAccessGranted ? true : false;
        }
        private bool MainSMC_OnStatAutoRun(SMCEventArgs args)
        {
            if (args.Handled) return false;

            Action ac = new Action(() =>
            {
                ReLayoutUIChangesForAutoRun();
            });

            if (CheckAccess())
            {
                ac();
            }
            else
            {
                Dispatcher.BeginInvoke(ac);
            }

            return true;

        }
        private bool MainSMC_OnEveryStatAutoRun(SMCEventArgs args)
        {
            if (args.Handled) return false;

            Action ac = new Action(() =>
            {
                ReLayoutUIChangesForAutoRun();
            });

            if (CheckAccess())
            {
                ac();
            }
            else
            {
                Dispatcher.BeginInvoke(ac);
            }

            return true;

        }
        void ReLayoutUIChangesForAutoRun()
        {

            TabCtrlSetup.SelectedIndex = 0;
        }
        void Tabpage_updated(object sender, SelectionChangedEventArgs e)
        {

            RefershUISetup();

        }
        void UserLogin_OnLoginLevelUpdated(UserAccess.AccessLevel lvl)
        {
            UpdateAllControlsAccess();
        }
        void UpdateAllControlsAccess()
        {
            //reset safety interlocked by super user level
            if (mmgr.UserLogin.CurrentAccessLevel < UserAccess.AccessLevel.ENGINEER)
                //  mmgr.ProcessControlCfg.ByPassSafetyDoorLocks = false;
                mmgr.DetectorConfigurationData.Integrtime.AllowEdit = false;

            for (int i = 0; i < OwnedWindows.Count; i++)
            {
                IAccessConfigurableControl ctr = OwnedWindows[i] as IAccessConfigurableControl;
                if (ctr == null) continue;
                ctr.DetermineAccess();

            }

            for (int i = 0; i < customControls.Count; i++)
            {
                IAccessConfigurableControl ctr = customControls[i] as IAccessConfigurableControl;
                if (ctr == null) continue;
                ctr.DetermineAccess();
            }
            DetermineAccess();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            try
            {
                if (mmgr.IsStartUpLoginFailed)
                    return;

                if (!CheckForMachineStatusAndComfirmation(e))
                    return;



                mmgr.TowerLight.Quiet();

                Splasher.Splash = new WinWelcomeScreen(true);
                mmgr.LoadingStatus = "Saving Config...";
                Splasher.ShowSplash();
                Thread.Sleep(200);
                try
                {
                    Thread.Sleep(100);

                    if (mmgr.RecipeMngr.IsControlRecipeLoaded)
                        mmgr.AppConfig.LastControlRecipeName = mmgr.RecipeMngr.LoadedControlRecipe;



                    if (mmgr.HasAppConfigLoaded)
                    {
                        AppConfig.SaveConfig(mmgr.AppConfig);

                    }

                    mmgr.LoadingStatus = "Closing...All";
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.ToString());
                    if (mmgr.ErrorEventMngr != null)
                        mmgr.ErrorEventMngr.ProcessError(ex.Message, ex.ToString());
                }

                Thread.Sleep(100);
                mmgr.CloseAll();
                Environment.Exit(0);

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                if (mmgr.ErrorEventMngr != null)
                    mmgr.ErrorEventMngr.ProcessError(ex.Message, ex.ToString());
            }
            finally
            {
                if (mmgr.ErrorEventMngr != null)
                    mmgr.ErrorEventMngr.ProcessEvent("Application Closed");
                Splasher.CloseSplash();
            }

        }
        bool CheckForMachineStatusAndComfirmation(System.ComponentModel.CancelEventArgs e)
        {
            if (forceClose) return true;

            if (IsLoaded && mmgr.MainSMC.IsBusy)
            {
                System.Windows.MessageBox.Show("Machine is Busy");
                e.Cancel = true;
                return false;
            }

            string msg = string.Format("{0} is will be closed!\nExit?", mmgr.AppConfig.AppName);
            if (System.Windows.MessageBox.Show(this, msg, "Closing", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
                return false;
            }
            return true;
        }

        public void OpenStorageMonitorStatus()
        {
            Window win = HasWindowWithType(typeof(WinStorageMonitoring));
            if (win != null) win.Activate();
            else
            {
                win = new WinStorageMonitoring(mmgr.Cleaner, App.MainMngr.UserLogin.CurrentAccessLevel >= AccessLevel.ENGINEER);
                win.Owner = this;
                win.ShowDialog();
            }
        }


        Window HasWindowWithType(Type tp)
        {

            foreach (Window item in OwnedWindows)
            {
                if (item.GetType() == tp)
                {
                    return item;
                }
            }
            return null;
        }

        Window HasDebugWindowWithStation(StateMachineStation ax)
        {
            for (int i = 0; i < OwnedWindows.Count; i++)
            {
                WinStationStateDebug win = OwnedWindows[i] as WinStationStateDebug;
                if (win == null) continue;
                if (win.Station != ax) continue;

                return win;
            }
            return null;
        }
        Window HasStateListWindowWithStation(StateMachineStation ax)
        {
            for (int i = 0; i < OwnedWindows.Count; i++)
            {
                WinStateListView win = OwnedWindows[i] as WinStateListView;
                if (win == null) continue;
                if (win.FSMStation != ax) continue;

                return win;
            }
            return null;
        }

        Window HasComportTerminalWindow(COMPort selectedCOMPort)
        {
            for (int i = 0; i < OwnedWindows.Count; i++)
            {
                WinRS232COMTerminal winRS232Ter = OwnedWindows[i] as WinRS232COMTerminal;
                if (winRS232Ter == null) continue;
                if (winRS232Ter.SelectedCOMPort != selectedCOMPort) continue;
                return winRS232Ter;
            }
            return null;
        }
        public void ShowStationStateDebugWindows()
        {
            for (int i = 0; i < App.MainMngr.MainSMC.StateMachinesStations.Count; i++)
            {
                StateMachineStation sms = App.MainMngr.MainSMC.StateMachinesStations[i];
                ShowStationRunDebugWindow(sms);
            }
        }
        public void RefershUISetup()
        {


            //on windows
            for (int i = 0; i < OwnedWindows.Count; i++)
            {
                IUIRefreshable ui = OwnedWindows[i] as IUIRefreshable;
                if (ui == null) continue;
                ui.ReSetupUI();
            }

            //on UI
            for (int i = 0; i < customControls.Count; i++)
            {
                IUIRefreshable ui = customControls[i] as IUIRefreshable;
                if (ui == null) continue;
                ui.ReSetupUI();
            }

        }
        public void ShowWindow(Window win, bool showDialog = false)
        {
            win.Owner = this;
            if (showDialog) win.ShowDialog();
            else win.Show();
        }
        volatile bool closingChildWindows = false;
        Type[] skipcloseWinType ={typeof(WinStationStateDebug),
                                  typeof(WinStateListView),

        };
        object closeObj = new object();
        public void CloseAllChildWindows()
        {
            //return;
            if (closingChildWindows) return;
            Thread.Sleep(10);
            if (closingChildWindows) return;

            AutoResetEvent closedAllWindows = new AutoResetEvent(false);
            //lock (closeObj)
            {
                closingChildWindows = true;
                Action ac = new Action(() =>
                {
                    try
                    {
                        for (int i = 0; i < OwnedWindows.Count; i++)
                        {
                            if (i > OwnedWindows.Count - 1) break;
                            //if (skipcloseWinType.Contains(OwnedWindows[i].GetType())) continue;
                            #region old style
                            if (OwnedWindows[i] is WinStationStateDebug) continue;
                            if (OwnedWindows[i] is WinStateListView) continue;
                            if (OwnedWindows[i] is WinMessageBoxView) continue;
                            #endregion
                            OwnedWindows[i].Close();
                        }
                    }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                    catch (Exception ex) { }
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                    finally
                    {
                        closedAllWindows.Set();
                    }
                });

                closedAllWindows.Reset();

                if (Dispatcher.CheckAccess())
                    ac();
                else
                    Dispatcher.BeginInvoke(ac);

                bool ok = false;
                while (!ok)
                {
                    ok = closedAllWindows.WaitOne(10);
                    Thread.Sleep(10);
                    //UIUtility.DispatcherHelper.DoEvents();
                    if (ok) break;
                }
                closingChildWindows = false;
            }


        }
        public void ShowStationRunDebugWindow(StateMachineStation sms)
        {
            Window win = HasDebugWindowWithStation(sms);
            if (win != null)
                win.Activate();
            else
            {
                win = new WinStationStateDebug(sms);
                ShowWindow(win, false);
            }
        }

        public void ShowStationStateListWindows()
        {
            for (int i = 0; i < App.MainMngr.MainSMC.StateMachinesStations.Count; i++)
            {
                StateMachineStation sms = App.MainMngr.MainSMC.StateMachinesStations[i];
                Window win = HasStateListWindowWithStation(sms);
                if (win != null)
                    win.Activate();
                else
                {
                    win = new WinStateListView(sms);
                    ShowWindow(win, false);
                }
            }
        }

        AutoResetEvent dispatcherWindowDisplayEvent = new AutoResetEvent(false);
        public void Reset()
        {
            dispatcherWindowDisplayEvent.Reset();
        }

        public int DisplayChildWindowDialog(Window win)
        {
            int result = 0;
            Action ac = new Action(() =>
            {
                win.Owner = this;
                win.ShowDialog();

                if (win is IUserActionResult)
                {
                    result = (win as IUserActionResult).ActionResult;
                }
                dispatcherWindowDisplayEvent.Set();
            });

            if (Dispatcher.CheckAccess())
            {
                ac();
            }
            else
            {
                Dispatcher.BeginInvoke(ac);
            }
            bool ok = dispatcherWindowDisplayEvent.WaitOne(10);
            while (!ok)
            {
                ok = dispatcherWindowDisplayEvent.WaitOne(10);
                Thread.Sleep(100);
                if (ok) break;
            }
            return result;
        }

        public int WaitForDisplayWindowClose(IUserActionResult win)
        {
            bool ok = false;
            while (!ok)
            {
                ok = dispatcherWindowDisplayEvent.WaitOne(10);
                Thread.Sleep(10);
                if (ok) break;
            }

            return win.ActionResult;
        }

        public int WaitForDisplayWindowClose(Window win)
        {
            bool ok = false;
            while (!ok)
            {
                ok = dispatcherWindowDisplayEvent.WaitOne(10);
                Thread.Sleep(10);
                if (ok) break;
            }
            return (bool)win.DialogResult ? 1 : 0;
        }

        private void BtnSaveControlRecipe_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
