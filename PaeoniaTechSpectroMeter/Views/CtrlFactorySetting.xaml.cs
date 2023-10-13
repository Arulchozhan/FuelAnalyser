using PaeoniaTechSpectroMeter.Data;
using PaeoniaTechSpectroMeter.Interface;
using PaeoniaTechSpectroMeter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UserAccess;

namespace PaeoniaTechSpectroMeter.Views
{
    /// <summary>
    /// Interaction logic for CtrlFactorySetting.xaml
    /// </summary>
    public partial class CtrlFactorySetting : UserControl, IAccessConfigurableControl, IUIRefreshable
    {
        MainManager factorySetting;
        public CtrlFactorySetting()
        {
            InitializeComponent();
        }
        public void Setup(MainManager factorySetting)
        {
            this.factorySetting = factorySetting;
            ReSetupUI();

        }



        public void ReSetupUI()
        {

            SetupUIPanels();

        }
        private void SetupUIPanels() //

        {
            // Dias_Lisa.Lisa.Action();
            ControlRecipe ctrlRecipe = factorySetting.RecipeMngr.ControlRecipe;
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
