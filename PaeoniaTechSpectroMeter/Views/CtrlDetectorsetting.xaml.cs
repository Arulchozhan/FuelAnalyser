using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PaeoniaTechSpectroMeter.Interface;
using UserAccess;

namespace PaeoniaTechSpectroMeter.Views
{
    using PaeoniaTechSpectroMeter.Station;
   // using MCPNet.IO.UI;
    using PaeoniaTechSpectroMeter.Data;
    using PaeoniaTechSpectroMeter.Model;
    using MCPNet.Motion;
    using MCPNet.UI;
    using MCPNet.Data.ValueData;
   
    public partial class CtrlCemerasetting : UserControl, IAccessConfigurableControl , IUIRefreshable
    {
     
        //Cameradata Dat;
        MainManager Dat;
        public CtrlCemerasetting()
        {
            InitializeComponent();
          //  ReSetupUI(); // testing 
        }

        public void Setup(MainManager cam)
        {
             this.Dat = cam;
            ReSetupUI();

        }

    

        public void ReSetupUI()
        {
       
            SetupUIPanels();
           
        }
        private void SetupUIPanels() //

        {
           // Dias_Lisa.Lisa.Action();
            ControlRecipe ctrlRecipe =Dat.RecipeMngr.ControlRecipe;
            //SearchableItemList
            SearchableItemList.Setup(ctrlRecipe.DetectorConfigurationData);
           //SearchableItemList.Setup(stn.Cameradata, App.MainMngr.MtrProfileMgr, App.MainWinInts);//, App.MainMngr.MtrProfileMgr

        }
        public void DetermineAccess()
        {
            if (!IsLoaded) return;
            AccessControl.UpdateAccessConfig(this, App.MainMngr.AccessControl, App.MainMngr.UserLogin);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Background = null;
            DetermineAccess();
        }

        private void TabCtrlStation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
    }
