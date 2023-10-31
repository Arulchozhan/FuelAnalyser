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
    
    public partial class CtrlMeasurementSetting : UserControl, IAccessConfigurableControl,IUIRefreshable
    {
        MainManager meassurementSetting;
        public CtrlMeasurementSetting()
        {
            InitializeComponent();
        }
        public void Setup(MainManager meassurementSetting)
        {
            this.meassurementSetting = meassurementSetting;
            ReSetupUI();

        }



        public void ReSetupUI()
        {

            SetupUIPanels();

        }
        private void SetupUIPanels() //

        {
            // Dias_Lisa.Lisa.Action();
            ControlRecipe ctrlRecipe = meassurementSetting.RecipeMngr.ControlRecipe;
            SearchableItemList.Setup(ctrlRecipe.MeasurementConfigurationData);

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
