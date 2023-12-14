
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
using PaeoniaTechSpectroMeter.Model;
namespace PaeoniaTechSpectroMeter.Views
{
    /// <summary>
    /// Interaction logic for CtrlAppConfig.xaml
    /// </summary>
    public partial class CtrlAppConfig : UserControl
    {
        public CtrlAppConfig()
        {
            InitializeComponent();
        }
        AppConfig cfg = null;
        public void Setup(AppConfig cfg) 
        {
            ProGrdConfig.SelectedObject = cfg;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cfg == null) return;
            AppConfig.SaveConfig(cfg);
        }
    }
}
