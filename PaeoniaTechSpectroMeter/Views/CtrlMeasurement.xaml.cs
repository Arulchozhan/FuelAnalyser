using CsvHelper;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using PaeoniaTechSpectroMeter.Database;
using PaeoniaTechSpectroMeter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using static PaeoniaTechSpectroMeter.Model.History;
using Image = iText.Layout.Element.Image;
using TextAlignment = iText.Layout.Properties.TextAlignment;
//using Microsoft.WindowsAPICodePack.Dialogs;

namespace PaeoniaTechSpectroMeter.Views
{
    public partial class CtrlMeasurement : UserControl
    {
        MainManager mmgr = null;
        // Serialtest sertest = null;
        // Lisa_ lisa = null;
        SPC spcObj = new SPC();

        

        public static BrosweLocationViewModel brosweLocationViewModel;
        public CtrlMeasurement(MainManager mmgr)
        {
            this.mmgr = mmgr;
            InitializeComponent();
            DispatcherTimer LiveTime = new DispatcherTimer();
            //  LiveTime.Interval = TimeSpan.FromSeconds(1);
            //  LiveTime.Tick += timer_Tick;
            // LiveTime.Start();
            this.DataContext = mmgr.ReadDetector;
            brosweLocationViewModel = new BrosweLocationViewModel();

        }


        void timer_Tick(object sender, EventArgs e)
        {
            //   DateandTime.Content = DateTime.Now.ToString("dd MMM yyyy HH:mm tt");
        }

        private void BtnMeasurement_Click(object sender, RoutedEventArgs e)
        {
            string serr = "";
            mmgr.ReadDetector.SaveBckVisibility = false;
            if ((string)BtnMeasurement.Content == "Start Measurement")
            {

                if (mmgr.ReadDetector.checkfeildentry())
                {
                    if (!mmgr.ReadDetector.checkfeildentryexist())
                    {
                        mmgr.ReadDetector.AnalysisSelectionEnable = false;
                        mmgr.ReadDetector.PyExceptionCount = 0;
                        mmgr.ReadDetector.StartMeasurement(32, 16);

                    }

                    else
                        mmgr.ReadDetector.MeasurementCompletedat = $"Please ensure Sample Name and Analysis Type distinct from today's measurements before starting measurements.";

                }


                else
                {
                    mmgr.ReadDetector.MeasurementCompletedat = $"Please key in and check all fields before starting measurements";

                }
                // serr = "";
                //  serr = mmgr.ReadDetector.ReadBaselineInfo("testc");
                //  serr = mmgr.ReadDetector.ReadPixelWavelength("testpath");
                //  serr = mmgr.ReadDetector.ReadBackground("testpath");
                // serr = mmgr.ReadDetector.LisaConnect();
                //       mmgr.ReadDetector.PyExceptionCount = 0;
                //     mmgr.ReadDetector.StartMeasurement(32, 16);
                //  mmgr.ReadDetector.ImportPythonTest();
                // if (serr != "")team
                // MessageBox.Show(serr, "Measurements read feild ");

            }
            else if ((string)BtnMeasurement.Content == "Cancel Measurement")
            {
                //MessageBoxResult r = MessageBox.Show("Do You want to Cancel Measurement??", "Attention", MessageBoxButton.YesNo);
                //if (r == MessageBoxResult.Yes)
                CustomMessageBox customMessageBox = new CustomMessageBox();
                customMessageBox.Owner = Window.GetWindow(this);
                bool? result = customMessageBox.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    mmgr.ReadDetector.MeasurementStatusCont = 0;
                    if (mmgr.ReadDetector.PassNo > 1)
                    {
                        mmgr.ReadDetector.PassNo = mmgr.ReadDetector.PassNo - 1;
                        mmgr.ReadDetector.CancelRepeatMeasurement();
                    }
                    else
                        mmgr.ReadDetector.CancelMeasurement();
                    //mmgr.ReadDetector.PassNo = 1;
                    mmgr.ReadDetector.MeasurementCompletedat = $"Measurement Was Cancelled";
                }

            }
            else if ((string)BtnMeasurement.Content == "New Measurement")
            {
                mmgr.ReadDetector.ResetforStartmeasurement();

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
            //browseLocationClicked = true;
            string serr = "";
            serr = mmgr.ReadDetector.BrowseLocation();
            //if (serr != "")
            //    MessageBox.Show(serr, "Change Location");

        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            //BrosweLocationViewModel browseLocationViewModel = Application.Current.MainWindow.DataContext as BrosweLocationViewModel;
            string userChooseDirFromMainWindow = brosweLocationViewModel.UserChooseDir;


            if (!mmgr.ReadDetector.IsDataSavedDB)
            {
                mmgr.ReadDetector.SaveMeasurementData();
                if (!string.IsNullOrEmpty(userChooseDirFromMainWindow))
                {
                    mmgr.ReadDetector.SaveFilePDF(userChooseDirFromMainWindow);
                }
                else
                {
                    string serr = "";
                    serr = mmgr.ReadDetector.BrowseLocation();
                    mmgr.ReadDetector.SaveFilePDF(serr);
                }
                //if (mmgr.ReadDetector.UserChooseDir == "")
                //{
                //    string serr = "";
                //    serr = mmgr.ReadDetector.BrowseLocation();
                //    mmgr.ReadDetector.SaveFilePDF();
                //}
                //else
                //{
                //    mmgr.ReadDetector.SaveFilePDF();
                //}

            }




            //catch (Exception ex)
            //{
            //    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
        private List<DataItem> GetAllDataItems()
        {

            List<DataItem> allItems = new List<DataItem>();
            allItems.Clear();

            DataItem dataItem = new DataItem();

            dataItem.Timestamp = System.DateTime.Now;
            dataItem.Name = mmgr.ReadDetector.SampleFileName; // "test";//.ToString();
            dataItem.PassNo = mmgr.ReadDetector.PassNo.ToString("D3"); //"001"; //["Pass No."]?.ToString();
            dataItem.Operator = mmgr.ReadDetector.OpearatorName; //"Arul";//row["Operator"]?.ToString();
            dataItem.AnalysisType = "eth";//row["Analysis Type"]?.ToString();
            dataItem.SampleType = "Eth"; //row["Sample Type"]?.ToString();
            dataItem.Ethanol = 0;// row["Ethanol"] is int ethanol ? (int?)ethanol : null;
            dataItem.Denaturant = 0;//row["Denaturant"] is int denaturant ? (int?)denaturant : null;
            dataItem.Methanol = 1; //row["Methanol"] is int methanol ? (int?)methanol : null;
            dataItem.Water = 2;// row["Water"] is int water ? (int?)water : null;
            dataItem.Batch = 2;// row["Batch"] is int batch ? (int?)batch : null;

            allItems.Add(dataItem);


            return allItems;
            //return null;
        }

        private void Chemicaltype_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string ser = "";
            ser = mmgr.ReadDetector.UpdatePLSResultVisibility();


            if (sender is ComboBox comboBox)
            {
                ComboBoxItem selectedComboBoxItem = comboBox.SelectedItem as ComboBoxItem;

                Sampletype.Text = "Fuel " + selectedComboBoxItem.Content.ToString();
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<DataItem> allDataItems = GetAllDataItems();

            if (allDataItems.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    Title = "Save CSV file"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var writer = new StreamWriter(saveFileDialog.FileName))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.Context.RegisterClassMap<DataItemMap>();
                        csv.WriteRecords(allDataItems);
                    }

                    MessageBox.Show("CSV file exported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private void BtnRepeatMeasurement_Click(object sender, RoutedEventArgs e)
        {
            //BrosweLocationViewModel browseLocationViewModel = Application.Current.MainWindow.DataContext as BrosweLocationViewModel;
            string userChooseDirFromMainWindow = brosweLocationViewModel.UserChooseDir;

            if (!mmgr.ReadDetector.IsDataSavedDB)
            {
                mmgr.ReadDetector.SaveMeasurementData();
                mmgr.ReadDetector.IsDataSavedDB = true;
                if (!string.IsNullOrEmpty(userChooseDirFromMainWindow))
                {
                    mmgr.ReadDetector.SaveFilePDF(userChooseDirFromMainWindow);
                }
                else
                {
                    string serr = "";
                    serr = mmgr.ReadDetector.BrowseLocation();
                    mmgr.ReadDetector.SaveFilePDF(serr);
                }

            }

            //if (browseLocationViewModel != null)
            //{
            //    string userChooseDirFromMainWindow = browseLocationViewModel.UserChooseDir;
            //    if (!mmgr.ReadDetector.IsDataSavedDB)
            //    {
            //        mmgr.ReadDetector.SaveMeasurementData();
            //        mmgr.ReadDetector.IsDataSavedDB = true;
            //        if (!string.IsNullOrEmpty(userChooseDirFromMainWindow))
            //        {
            //            mmgr.ReadDetector.SaveFilePDF(userChooseDirFromMainWindow);
            //        }
            //        else
            //        {
            //            string serr = "";
            //            serr = mmgr.ReadDetector.BrowseLocation();
            //            mmgr.ReadDetector.SaveFilePDF(userChooseDirFromMainWindow);
            //        }
            //    }
            //}

            //if (!mmgr.ReadDetector.IsDataSavedDB)
            //{
            //    mmgr.ReadDetector.SaveMeasurementData();
            //    mmgr.ReadDetector.IsDataSavedDB = true;
            //    if (mmgr.ReadDetector.UserChooseDir == "")
            //    {
            //        string serr = "";
            //        serr = mmgr.ReadDetector.BrowseLocation();
            //        mmgr.ReadDetector.SaveFilePDF();
            //    }
            //    else
            //    {
            //        mmgr.ReadDetector.SaveFilePDF();
            //    }
            //}

            mmgr.ReadDetector.PassNo++;
            mmgr.ReadDetector.MeasuremantBtnContent = "Start Measurement";
            mmgr.ReadDetector.PyExceptionCount = 0;
            mmgr.ReadDetector.StartMeasurement(32, 16);
            mmgr.ReadDetector.IsRepeatmeasure = false;
            mmgr.ReadDetector.IsMeasurementCompleted = false;
            mmgr.ReadDetector.InfoIconSource = @"C:\FuelAnalyzer\bin\Icon\Info_Icon.png";

        }
    }
}
