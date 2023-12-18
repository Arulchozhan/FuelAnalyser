using LiveCharts.Wpf;
using LiveCharts;
using PaeoniaTechSpectroMeter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
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
using System.ComponentModel;
using Path = System.IO.Path;
using iText.Layout.Element;
//using System.Windows.Forms;

namespace PaeoniaTechSpectroMeter.Views
{
    /// <summary>
    /// Interaction logic for CtrlSelfDiagnostics.xaml
    /// </summary>
    public partial class CtrlSelfDiagnostics : UserControl
    {
        DateTime currentTime = DateTime.Now;
        private LineSeries currentLineSeries;

        private LineSeries factoryBackgroundLineSeries;

        private LineSeries newBackgroundLineSeries;

        private SolidColorBrush _BackgroundProperty;

        List<double> ftyAir = new List<double>();
        List<double> firstAir = new List<double>();
        List<double> currentAir = new List<double>();
        List<double> ftyOff = new List<double>();
        List<double> currentOff = new List<double>();
        List<double> ftyDiff = new List<double>();

        private double[] currentBackgroundData;

        private double[] factoryBackgroundData;

        private double[] newBackgroundData;
        private double[] newBackgroundSpectrumData;


        public SolidColorBrush backgroundProperty
        {
            get { return _BackgroundProperty; }
            set
            {
                if (_BackgroundProperty != value)
                {
                    _BackgroundProperty = value;
                    OnPropertyChanged(nameof(backgroundProperty));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        MainManager mmgr = null;
        public CtrlSelfDiagnostics(MainManager mmgr)
        {
            this.mmgr = mmgr;
            InitializeComponent();
            this.DataContext = mmgr.ReadDetector;
            
            GetCurrentBackground();
            if (mmgr.AppConfig.Perfchk == "PASS")
                PerformanceWarningImage.Source = new BitmapImage(new Uri("../Icon/Performance-GreenSign_Icon.png", UriKind.Relative)); //Performance-GreenSign_Icon
           else
                PerformanceWarningImage.Source = new BitmapImage(new Uri("../Icon/Performance-WarningSign_Icon.png", UriKind.Relative));

            this.LastTestedOn.Text ="Last tested on";
            this.LastTestedTime.Text = mmgr.AppConfig.PerfchkTime;
            this.LastScannedTextAndTime.Text =$"Last scanned / reset on " + mmgr.AppConfig.BgchkTime;
            GetFactoryBackground();

        }

        private void GetCurrentBackground()  
        {
            currentBackgroundData = GetCurrentBackgroundData();  //git
            //currentBackgroundData = mmgr.SelfDiagnostics.GetCurrentBackgroundData();

            currentLineSeries = new LineSeries
            {
                Title = "Current Background",
                Values = new ChartValues<double>(currentBackgroundData),
                StrokeThickness = 1,
                Fill = Brushes.Transparent
            };

            currentLineSeries.PointGeometry = DefaultGeometries.Circle;
            currentLineSeries.PointGeometrySize = 3;
            backgroundProperty = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005fb8"));
            currentLineSeries.SetBinding(LineSeries.StrokeProperty, new Binding("backgroundProperty") { Source = this });

            chart.Series.Add(currentLineSeries);
        }

        private double[] GetCurrentBackgroundData()
        {
            string currentairPath = "C:\\FuelAnalyzer\\Currentair" + ".csv";
            string ftytairPath = "C:\\FuelAnalyzer\\Ftyair" + ".csv";


            ReadCsv(currentairPath, out currentAir);
            ReadCsv(ftytairPath, out ftyAir);

            currentBackgroundData = currentAir.Zip(ftyAir, (x, y) => x - y).ToArray(); //using Linq method to 
            return currentBackgroundData;
        }

        private void GetFactoryBackground()
        {
            factoryBackgroundData = GetFactoryBackgroundData();
            //factoryBackgroundData = mmgr.SelfDiagnostics.GetFactoryBackgroundData();
            factoryBackgroundLineSeries = new LineSeries
            {
                Title = "Factory Background",
                Values = new ChartValues<double>(factoryBackgroundData),
                StrokeThickness = 1,
                Fill = Brushes.Transparent
            };

            factoryBackgroundLineSeries.PointGeometry = DefaultGeometries.Circle;
            factoryBackgroundLineSeries.PointGeometrySize = 3;
            backgroundProperty = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1a1a1a"));
            factoryBackgroundLineSeries.SetBinding(LineSeries.StrokeProperty, new Binding("backgroundProperty") { Source = this });
            chart.Series.Add(factoryBackgroundLineSeries);
        }

        private double[] GetFactoryBackgroundData()
        {
            string firstairPath = "C:\\FuelAnalyzer\\Firstair" + ".csv";
            string ftytairPath = "C:\\FuelAnalyzer\\Ftyair" + ".csv";


            ReadCsv(firstairPath, out firstAir);
            ReadCsv(ftytairPath, out ftyAir);

            factoryBackgroundData = firstAir.Zip(ftyAir, (x, y) => x - y).ToArray(); //using Linq method to 
            return factoryBackgroundData;
        }

        private string ReadCsv(string filepath, out List<double> result)
        {
            string serr = "";

            result = new List<double>();
            result.Clear();
            StreamReader ControlPageReader = null;
            try
            {
                ControlPageReader = new StreamReader(filepath);
                string sDataLine = "";
                string[] sControlData = { "" };

                while (true)
                {
                    sDataLine = ControlPageReader.ReadLine();

                    if (sDataLine == null)
                        break;

                    sControlData = sDataLine.Split(',');

                    result.Add((Convert.ToDouble(sControlData[0])));

                }
                ControlPageReader.Dispose();
                ControlPageReader.Close();
                return serr;
            }
            catch (Exception ex)
            {
                serr = ex.Message;
                ControlPageReader.Dispose();
                ControlPageReader.Close();
                return serr;
            }
        }


        private void BtnResetToFactoryBackground_Click(object sender, RoutedEventArgs e)
        {
            string currentairPath = "C:\\FuelAnalyzer\\Currentair" + ".csv";
            string firstairPath = "C:\\FuelAnalyzer\\Firstair" + ".csv";
            factoryBackgroundData = GetFactoryBackgroundData();

            ReadCsv(firstairPath, out firstAir);
            ftyAir.ToArray();
            currentLineSeries.Values.Clear();
            foreach (var value in factoryBackgroundData)
            {
                currentLineSeries.Values.Add(value);
            }

            currentBackgroundData = factoryBackgroundData;
            mmgr.SelfDiagnostics.WriteCsv(currentairPath, firstAir.ToArray(), false);
            backgroundProperty = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005fb8"));
            currentLineSeries.SetBinding(LineSeries.StrokeProperty, new Binding("backgroundProperty") { Source = this });
            factoryBackgroundLineSeries.Visibility = Visibility.Collapsed;
            //factoryBackgroundLineSeries.SetBinding(LineSeries.StrokeProperty, new Binding("backgroundProperty") { Source = this });
            //currentLineSeries.Visibility = Visibility.Collapsed;
            currentTime = DateTime.Now;
            LastScannedTextAndTime.Text = "Last scanned / reset on " + currentTime.ToString("dd/MM/yyyy hh:mm");
            mmgr.AppConfig.BgchkTime = currentTime.ToString("dd/MM/yyyy hh:mm");

            ÏnfoMessageImage.Source = new BitmapImage(new Uri("../Images/Info-GreenSign_Icon.png", UriKind.Relative));
            InfoMessageTextBlock.Text = "Background reset to factory stored background.";
            InfoMessageTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0f7b0f"));
            SDborder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0f7b0f"));
        }

        private void BtnTestInstrument_Click(object sender, RoutedEventArgs e)    
        {
            //IsInstrumentUpToStandard();c
            // Master testing mearge
           mmgr.ReadDetector.AnalysisSelectionEnable = false;

            if (IsInstrumentUpToStandard())
            {
                currentTime = DateTime.Now;
                LastTestedOn.Text = "Last tested on";
                LastTestedTime.Text = currentTime.ToString("dd/MM/yyyy hh:mm");
                mmgr.AppConfig.Perfchk = "PASS";
                mmgr.AppConfig.PerfchkTime= currentTime.ToString("dd/MM/yyyy hh:mm");
                PerformanceWarningImage.Source = new BitmapImage(new Uri("../Icon/Performance-GreenSign_Icon.png", UriKind.Relative)); //Performance-GreenSign_Icon

                ÏnfoMessageImage.Source = new BitmapImage(new Uri("../Images/Info-GreenSign_Icon.png", UriKind.Relative));
                InfoMessageTextBlock.Text = "Ensure no fuel inside before testing instrument or scanning new background.";
                InfoMessageTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0f7b0f"));
                
            }
            else
            {
                mmgr.AppConfig.Perfchk = "Failed";
                mmgr.AppConfig.PerfchkTime = currentTime.ToString("dd/MM/yyyy hh:mm");
                ÏnfoMessageImage.Source = new BitmapImage(new Uri("../Icon/InfoWarning_Icon.png", UriKind.Relative)); //"C:\\FuelAnalyzer\\bin\\Icon\\Performance-WarningSign_Icon.png"
                InfoMessageTextBlock.Text = "Instrument is not up to standard. Perform cleaning and run “test instrument”. If this message remains, please contact WI.";
                InfoMessageTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c6891e"));

                PerformanceWarningImage.Source = new BitmapImage(new Uri("../Icon/Performance-WarningSign_Icon.png", UriKind.Relative));
                SDborder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c6891e"));

                BtnScanNewBackground.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
                BtnScanNewBackground.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));

                BtnResetToFactoryBackground.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
                BtnResetToFactoryBackground.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));

                LastTestedOn.Text = "Last tested on";
                LastTestedTime.Text = currentTime.ToString("dd/MM/yyyy hh:mm");


            }
            mmgr.ReadDetector.AnalysisSelectionEnable = true;
        }

        private bool IsInstrumentUpToStandard()
        {
            double[] spectrum;
            double[] ftydiff;
            double average=0;
            double testvalue = 0;
           
            string ftytairPath = "C:\\FuelAnalyzer\\Ftyair" + ".csv";
            string ftyoffPath = "C:\\FuelAnalyzer\\Ftyoff" + ".csv";

            string curroffPath = "C:\\FuelAnalyzer\\Currentoff" + ".csv";

            ReadCsv(ftytairPath, out ftyAir);
            ReadCsv(ftyoffPath, out ftyOff);
            ReadCsv(curroffPath, out currentOff);
            ftydiff = ftyOff.Zip(ftyAir, (x, y) => x - y).ToArray(); //ftydiff
            spectrum = mmgr.SelfDiagnostics.GetNewBackgroundData(); // current spectrum
            
         

            for (int k = 0; k < spectrum.Length; k++)
            {
                testvalue = Math.Abs((currentOff[k]- spectrum[k] -ftydiff[k]) / ftydiff[k]);
                average += testvalue;
                if (testvalue >= 0.3)
                {
                    //LastTestedOn.Text = "Last tested on";
                    //LastTestedTime.Text = currentTime.ToString("dd/MM/yyyy hh:mm");
                    //mmgr.AppConfig.Perfchk = "Failed";
                    //mmgr.AppConfig.PerfchkTime = currentTime.ToString("dd/MM/yyyy hh:mm");
                    
                    return false;
                }
            }
            average /= spectrum.Length;
            if (average >= 0.2)
            {
                return false;
            
            }

            return true;
        }

        private void BtnScanNewBackground_Click(object sender, RoutedEventArgs e)
        {

            BtnResetToFactoryBackground.IsEnabled = false;
            BtnTestInstrument.IsEnabled = false;

            BtnResetToFactoryBackground.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
            BtnResetToFactoryBackground.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9f9f9f"));

            BtnTestInstrument.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffffff"));
            BtnTestInstrument.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9f9f9f"));

            currentLineSeries.Values.Clear(); //update data later
            factoryBackgroundLineSeries.Values.Clear(); //update data later

            if (newBackgroundLineSeries != null)
            {
                newBackgroundLineSeries.Values.Clear();
            }
            if (mmgr.ReadDetector.MeasuremantBtnContent == "Start Measurement")
            {
                if (mmgr.AppConfig.Perfchk == "PASS")
                {

                    InfoMessageTextBlock.Text = "Scanning new background...";

                    BtnScanNewBackground.Content = "Cancel Scan";
                    BtnScanNewBackground.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF"));
                    BtnScanNewBackground.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005FB8"));

                    GetNewBackground();

                    ÏnfoMessageImage.Source = new BitmapImage(new Uri("../Icon/Info-GreenSign_Icon.png", UriKind.Relative));
                    InfoMessageTextBlock.Text = "New background scan completed.";
                    InfoMessageTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0f7b0f"));
                    SDborder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0f7b0f"));
                }
                else
                {
                    ÏnfoMessageImage.Source = new BitmapImage(new Uri("../Icon/InfoWarning_Icon.png", UriKind.Relative));
                    InfoMessageTextBlock.Text = "Instrument is not up to standard. Scanning new background...";
                    InfoMessageTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c6891e"));

                    BtnScanNewBackground.Content = "Cancel Scan";
                    BtnScanNewBackground.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
                    BtnScanNewBackground.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF"));

                    GetNewBackground();

                    ÏnfoMessageImage.Source = new BitmapImage(new Uri("../Icon/InfoWarning_Icon.png", UriKind.Relative));
                    InfoMessageTextBlock.Text = "Instrument is not up to standard. New background scan completed.";
                    InfoMessageTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c6891e"));
                }

            }
            else
            {
                ÏnfoMessageImage.Source = new BitmapImage(new Uri("../Icon/InfoWarning_Icon.png", UriKind.Relative));
                InfoMessageTextBlock.Text = "The instrument is set to Measuring. Please stop Measurement and scan a new background...";
                InfoMessageTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c6891e"));


            }

            /*  if (IsInstrumentUpToStandard())
              {
                  InfoMessageTextBlock.Text = "Scanning new background...";

                  BtnScanNewBackground.Content = "Cancel Scan";
                  BtnScanNewBackground.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF"));
                  BtnScanNewBackground.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005FB8"));

                  GetNewBackground();

                  ÏnfoMessageImage.Source = new BitmapImage(new Uri("../Images/Info-GreenSign_Icon.png", UriKind.Relative));
                  InfoMessageTextBlock.Text = "New background scan completed.";
                  InfoMessageTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0f7b0f"));
                  SDborder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0f7b0f"));
              }
              else
              {
                  ÏnfoMessageImage.Source = new BitmapImage(new Uri("../Images/InfoWarning_Icon.png", UriKind.Relative));
                  InfoMessageTextBlock.Text = "Instrument is not up to standard. Scanning new background...";
                  InfoMessageTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c6891e"));

                  BtnScanNewBackground.Content = "Cancel Scan";
                  BtnScanNewBackground.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
                  BtnScanNewBackground.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF"));

                  GetNewBackground();

                  ÏnfoMessageImage.Source = new BitmapImage(new Uri("../Images/InfoWarning_Icon.png", UriKind.Relative));
                  InfoMessageTextBlock.Text = "Instrument is not up to standard. New background scan completed.";
                  InfoMessageTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c6891e"));
              }
            */

        }

        private void GetNewBackground()
        {
            //have to write logic to get New Background and all three lines together
            string ftytairPath = "C:\\FuelAnalyzer\\Ftyair" + ".csv";
            ReadCsv(ftytairPath, out ftyAir);
            newBackgroundSpectrumData = mmgr.SelfDiagnostics.GetNewBackgroundData();

            //
            newBackgroundData = newBackgroundSpectrumData.Zip(ftyAir, (x, y) => x - y).ToArray(); //using Linq method to 
            newBackgroundLineSeries = new LineSeries
            {
                Title = "New Background",
                Values = new ChartValues<double>(newBackgroundData),
                StrokeThickness = 1,
                Fill = Brushes.Transparent
            };

            newBackgroundLineSeries.PointGeometry = DefaultGeometries.Circle;
            newBackgroundLineSeries.PointGeometrySize = 3;
            backgroundProperty = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0f7b0f"));
            newBackgroundLineSeries.SetBinding(LineSeries.StrokeProperty, new Binding("backgroundProperty") { Source = this });
            chart.Series.Add(newBackgroundLineSeries);
           GetCurrentBackground();
           GetFactoryBackground();

            BtnSaveNewBackground.Visibility = Visibility.Visible;
            SaveNewBorder.Visibility = Visibility.Visible;

            BtnScanNewBackground.Content = "Scan new Background";
            BtnScanNewBackground.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
            BtnScanNewBackground.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF"));

            BtnResetToFactoryBackground.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
            BtnResetToFactoryBackground.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF"));

            BtnResetToFactoryBackground.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
            BtnResetToFactoryBackground.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF"));

            BtnTestInstrument.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000"));
            BtnTestInstrument.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF"));
            InfoMessageTextBlock.Text = "New background scan completed.";
           // LastScannedTextAndTime.Text = "Last scanned / reset on " + currentTime.ToString("dd/MM/yyyy hh:mm");
           //mmgr.AppConfig.BgchkTime= currentTime.ToString("dd/MM/yyyy hh:mm");
        }

        private double[] GetNewBackgroundData()
        {
            Random random = new Random();
            return Enumerable.Range(0, 128).Select(_ => (random.NextDouble() * 0.2) + 2.6 - 2.5).ToArray();
        }

        private void BtnSaveNewBackground_Click(object sender, RoutedEventArgs e)
        {
            string currentairPath = "C:\\FuelAnalyzer\\Currentair" + ".csv";
            BtnSaveNewBackground.Visibility = Visibility.Collapsed;
            SaveNewBorder.Visibility = Visibility.Collapsed;

            BtnResetToFactoryBackground.IsEnabled = true;
            BtnTestInstrument.IsEnabled = true;



            currentLineSeries.Values.Clear();
            foreach (var value in newBackgroundData)
            {
                currentLineSeries.Values.Add(value);
            }

            currentBackgroundData = newBackgroundData;
            currentTime = DateTime.Now;
            LastScannedTextAndTime.Text = "Last scanned / reset on " + currentTime.ToString("dd/MM/yyyy hh:mm");
            mmgr.AppConfig.BgchkTime = currentTime.ToString("dd/MM/yyyy hh:mm");
            //mmgr.SaveControlRecipe();
            backgroundProperty = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005fb8"));
            currentLineSeries.SetBinding(LineSeries.StrokeProperty, new Binding("backgroundProperty") { Source = this });
            newBackgroundLineSeries.Visibility = Visibility.Collapsed;



            //backgroundProperty = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005fb8"));
            //newBackgroundLineSeries.SetBinding(LineSeries.StrokeProperty, new Binding("backgroundProperty") { Source = this });
            //currentLineSeries.Visibility = Visibility.Collapsed;

            if (mmgr.AppConfig.Perfchk == "PASS")
            {
                ÏnfoMessageImage.Source = new BitmapImage(new Uri("../Images/Info_Icon.png", UriKind.Relative));
                InfoMessageTextBlock.Text = "Background updated to new background.";
                InfoMessageTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005fb8"));
                SDborder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005fb8"));

            }
            else
            {
                ÏnfoMessageImage.Source = new BitmapImage(new Uri("../Images/InfoWarning_Icon.png", UriKind.Relative));
                InfoMessageTextBlock.Text = "Instrument is not up to standard. Background updated to new background.";
                InfoMessageTextBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c6891e"));
            }
           
            BtnScanNewBackground.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF"));
            BtnScanNewBackground.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005FB8"));

            BtnResetToFactoryBackground.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF"));
            BtnResetToFactoryBackground.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005FB8"));

            BtnTestInstrument.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF"));
            BtnTestInstrument.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005FB8"));

            // currentBackgroundData = GetCurrentBackgroundData();
            // factoryBackgroundData = GetFactoryBackgroundData();

            //SaveDataToCsv(currentBackgroundData, "CurrentBackgroundData");
            mmgr.SelfDiagnostics.WriteCsv(currentairPath, newBackgroundSpectrumData, false);
            MessageBox.Show($"Data saved to CSV file", "Save Successful", MessageBoxButton.OK, MessageBoxImage.Information);

            // SaveDataToCsv(factoryBackgroundData, "FactoryBackgroundData");
        }

        private void SaveDataToCsv(double[] data, string fileNamePrefix)
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SecureDataFolder");

            // Check if the folder exists, create it if not.
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);

                DirectorySecurity directorySecurity = new DirectorySecurity();
                directorySecurity.AddAccessRule(new FileSystemAccessRule(
                    new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow
                ));
                Directory.SetAccessControl(folderPath, directorySecurity);
            }

            string filePath = Path.Combine(folderPath, $"{fileNamePrefix}_{DateTime.Now:yyyyMMddHHmmss}.csv");

            string csvContent = string.Join(Environment.NewLine, data.Select(value => value.ToString()));

            File.WriteAllText(filePath, csvContent);

            MessageBox.Show($"Data saved to CSV file", "Save Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}

