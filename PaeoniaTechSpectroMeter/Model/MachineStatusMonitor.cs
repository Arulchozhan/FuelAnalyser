using FSM;
using MCPNet.MachineMonitor;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;


namespace PaeoniaTechSpectroMeter.Model
{
    public class MachineStatusMonitor : DispatcherObject, INotifyPropertyChanged

    {
        MachineStatus machineStatus = MachineStatus.UnInitialised;
        public MachineStatus MachineStatus
        {
            get { return machineStatus; }
            private set
            {
                if (machineStatus == value) return;

                machineStatus = value;
                Debug.WriteLine("MachineStatus [1]=" + MachineStatus + " <" + DateTime.Now.ToString("mm.ss.f") + ">");
                OnPropertyChanged("MachineStatus");

                // int i = 0;

                Debug.WriteLine("MachineStatus [2]=" + MachineStatus + " <" + DateTime.Now.ToString("mm.ss.f") + ">");
                // Action ac = new Action(())
                //Debug.WriteLine("Tower Light Update " + DateTime.Now.ToString("HH:mm.yy.ss.f"));
                if (towerLight != null)
                    towerLight.UpdateStatus(value);

                //Debug.WriteLine("Tower Light Update end " + DateTime.Now.ToString("HH:mm.yy.ss.f"));
                Debug.WriteLine("MachineStatus [3]=" + MachineStatus + " <" + DateTime.Now.ToString("mm.ss.f") + ">");
            }
        }

        StateMachineController mainSMC = null;
        RecipeManager recMngr = null;
        TowerLight towerLight = null;

        public void Init(StateMachineController mainSMC, RecipeManager recMngr, TowerLight towerLight)
        {
            if (mainSMC != null)
            {
                mainSMC.PropertyChanged -= mainSMC_PropertyChanged;
                mainSMC.PropertyChanged += mainSMC_PropertyChanged;


                recMngr.PropertyChanged -= recMngr_PropertyChanged;
                recMngr.PropertyChanged += recMngr_PropertyChanged;
            }
            this.mainSMC = mainSMC;
            this.recMngr = recMngr;
            this.towerLight = towerLight;

            if (towerLight != null)
                towerLight.UpdateStatus(machineStatus);



        }


        void recMngr_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsRecipeSavingAndLoading")
            {
                UpdateStatusOnSMCStatusChanges();
            }
        }
        StatusInfoType overalStatusType = StatusInfoType.Statement;
        public StatusInfoType OveralStatusType
        {
            get { return overalStatusType; }
            private set
            {
                overalStatusType = value;
                OnPropertyChanged("OveralStatusType");
            }
        }
        string overallStatusStr = "UnInitialised";
        public string OverallStatusStr
        {
            get { return overallStatusStr; }
            private set
            {
                overallStatusStr = value;
                OnPropertyChanged("OverallStatusStr");
                Debug.WriteLine("OverallStatusStr=" + value);
            }
        }

        object statusObj = new object();
        public void SetOverallStatus(string msg, StatusInfoType type = StatusInfoType.Statement)
        {
            OverallStatusStr = msg;
            OveralStatusType = type;
        }


        ObservableCollection<string> detailStatusList = new ObservableCollection<string>();
        public ObservableCollection<string> DetailStatusList
        {
            get { return detailStatusList; }
        }

        public void AddDetailStatus(string msg, StatusInfoType info = StatusInfoType.Statement)
        {
            Action ac = new Action(() => { });
            if (Dispatcher.CheckAccess())
                ac();
            else
                Application.Current.Dispatcher.BeginInvoke(ac);
        }

        void mainSMC_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Status")
                UpdateStatusOnSMCStatusChanges();
        }

        SMCStatus lastStatus = SMCStatus.UNKNOWN;



        bool IsInitialising
        {
            get { return mainSMC == null ? false : mainSMC.IsInitialising; }
        }
        bool IsRunning
        {
            get { return mainSMC == null ? false : mainSMC.IsAutoRunning && mainSMC.IsBusy; }
        }

        public bool IsBusy
        {
            get { return (mainSMC == null || recMngr == null) ? false : mainSMC.IsBusy || recMngr.IsRecipeSavingAndLoading; }
        }

        bool HasInitialisingJustDone
        {
            get
            {
                return mainSMC == null ? false : mainSMC.HasSystemInitializationDone &&
                                                 machineStatus == MachineStatus.Initialising;
            }
        }

        bool IsResetting
        {
            get { return mainSMC == null ? false : mainSMC.IsResetRunning; }
        }

        bool HasResettingJustDone
        {
            get
            {
                return mainSMC == null ? false : mainSMC.Status == SMCStatus.IDLE &&
                                                 machineStatus == MachineStatus.Resetting;
            }
        }
        bool HasUserAborted
        {
            get
            {
                return mainSMC == null ? false : !mainSMC.IsBusy &&
                                                  mainSMC.IsUserAborted;
            }
        }



        public void SetMachineStatus(MachineStatus st)
        {
            MachineStatus = st;
        }
        void UpdateStatusOnSMCStatusChanges()
        {
            //Debug.WriteLine("{3} SMC={0} Reset={1} Initial={2}", 
            //                mainSMC.Status, 
            //                mainSMC.IsResetRunning,
            //                mainSMC.IsInitialising,
            //                DateTime.Now.ToLongTimeString());

            if (mainSMC.Status == SMCStatus.ERROR ||
                    mainSMC.Status == SMCStatus.HANDLING_ERROR)
            {
                MachineStatus = MachineStatus.Error;
            }
            else if (IsInitialising)
            {
                SetOverallStatus("System Initialising..");
                MachineStatus = MachineStatus.Initialising;
            }
            else if (HasInitialisingJustDone)
            {
                MachineStatus = MachineStatus.Idle;
                SetOverallStatus("System Initialising Done");
            }
            else if (IsResetting)
            {
                MachineStatus = MachineStatus.Resetting;
            }
            else if (HasResettingJustDone)
            {
                MachineStatus = MachineStatus.Idle;
                SetOverallStatus("System Reset Done");
            }

            else if (IsBusy)
            {
                MachineStatus = MachineStatus.Busy;
            }

            else if (HasUserAborted)
            {
                //if (machineStatus == MCPNet.MachineMonitor.MachineStatus.Running ||
                //    machineStatus == MCPNet.MachineMonitor.MachineStatus.Initialising ||
                //    machineStatus == MCPNet.MachineMonitor.MachineStatus.Resetting) 
                //    SetOverallStatus("User Stopped!");
                MachineStatus = MachineStatus.Idle;

            }
            else if (mainSMC.Status == SMCStatus.IDLE &&
                     !mainSMC.IsBusy)
            {
                MachineStatus = MachineStatus.Idle;
            }
            else if (IsRunning)
            {
                MachineStatus = MachineStatus.Running;
            }

            OnPropertyChanged("IsBusy");
            OnPropertyChanged("IsRunning");

            lastStatus = mainSMC.Status;
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
