using PaeoniaTechSpectroMeter.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
//using Microsoft.WindowsAPICodePack.Dialogs;

namespace PaeoniaTechSpectroMeter.Views
{
    public partial class CtrlMeasurement : UserControl
    {
        MainManager mmgr = null;
        // Serialtest sertest = null;
        // Lisa_ lisa = null;
        SPC spcObj = new SPC();
        public CtrlMeasurement(MainManager mmgr)
        {
            this.mmgr = mmgr;
            InitializeComponent();
            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();
            this.DataContext = mmgr.ReadDetector;

        }


        void timer_Tick(object sender, EventArgs e)
        {
            DateandTime.Content = DateTime.Now.ToString("dd MMM yyyy HH:mm tt");
        }

        private void BtnMeasurement_Click(object sender, RoutedEventArgs e)
        {
            string serr = "";

            if (BtnMeasurement.Content == "Start Measurement")
            {


                serr = "";
                //  serr = mmgr.ReadDetector.ReadBaselineInfo("testc");
                //  serr = mmgr.ReadDetector.ReadPixelWavelength("testpath");
                //  serr = mmgr.ReadDetector.ReadBackground("testpath");
                // serr = mmgr.ReadDetector.LisaConnect();
                 mmgr.ReadDetector.StartMeasurement(32, 16);
              //  mmgr.ReadDetector.ImportPythonTest();
               // if (serr != "")
                   // MessageBox.Show(serr, "Measurements read feild ");

            }
            else
            {
                MessageBoxResult r = MessageBox.Show("Do You want to Cancel Measurement??", "Attention", MessageBoxButton.YesNo);
                if (r == MessageBoxResult.Yes)
                    mmgr.ReadDetector.CancelMeasurement();

            }


            // serr = mmgr.ReadDetector.ReadBaselineInfo("testc");
            //serr = mmgr.ReadDetector.ReadPixelWavelength("testpath");
            //serr = mmgr.ReadDetector.ReadBackground("testpath");

            // serr = mmgr.ReadDetector.LisaConnect();
            // serr = "";
            //bool status = false;
            // mmgr.ReadDetector.StartMeasurement(32, 16);


            //  if (status != true)
            //  MessageBox.Show(serr, "Measurements read feild ");



        }

        private void BtnChangeLocation_Click(object sender, RoutedEventArgs e)
        {
            string serr = "";
            serr= mmgr.ReadDetector.BrowseLocation();
            if(serr!="")
                MessageBox.Show(serr, "Change Location");

        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string serr = "";
            serr = mmgr.ReadDetector.LogData();
            if (serr != "")
                MessageBox.Show(serr, "Save File");


        }

        private void Chemicaltype_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string ser = "";
            ser = mmgr.ReadDetector.UpdatePLSResultVisibility();

        }
    }
}
