﻿//using Dias_Lisa;
using LogWritter;
using Microsoft.VisualBasic.FileIO;
using PaeoniaTechSpectroMeter.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Controls;
using Python.Runtime;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using PaeoniaTechSpectroMeter.Database;
using System.Configuration;
using iText.Kernel.Pdf;
using static PaeoniaTechSpectroMeter.Model.History;
using iText.Layout;
using System.Windows.Documents;
using iText.Kernel.Colors;
using iText.IO.Image;
using System.Windows.Controls.DataVisualization;
using iText.Layout.Element;
using Image = iText.Layout.Element.Image;
using TextAlignment = iText.Layout.Properties.TextAlignment;
using Paragraph = iText.Layout.Element.Paragraph;
using Table = iText.Layout.Element.Table;
using iText.Layout.Properties;
using UnitValue = iText.Layout.Properties.UnitValue;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;
using Microsoft.Win32;

namespace PaeoniaTechSpectroMeter.Model
{
    public class Pixelresult_data
    {
        public List<double> output1 = new List<double>();
        public List<List<string>> myList = new List<List<string>>();
    }

    public class ReadDetector : UserControl, INotifyPropertyChanged

    {


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion

        private string detectorTemperature;
        private int selectedAnalysistype;
        bool analysisSelection;
        bool analysisSelectionEnable;
        bool saveBckVisibility;
        bool measurementEnable;
        bool newBckScanEnable;
        private string sampleFileName = "";
        private int selectedSampleType;
        private int filecnt = 0;
        private int measurementStatusCont = 0;
        private int measurementMaxCont = 100;
        bool readingfinished = false;
        private double ethanolConcentration;
        private double methanolConcentration;
        private double denaturantConcentration;
        private double waterConcentration;
        private int pyExceptionCount = 0;
        private int passNo = 1;
        private bool isDataSavedDB = false;
        private string opearatorName = "";

        object obj = new object();
        private string measurementContent;
        private bool isReading = false;
        public bool stopReq = false;
        private DateTime starttime;
        private string measurementCompletedat = "";
        private bool isMeasurementCompleted;
        private bool isMeasurementNotInRange;
        private string userChooseDir = "";
        private bool isRepeatmeasure = false;
        private bool isReadytoSave = false;
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["FuelAnalyser"].ConnectionString;
        private DataAccess _dataAccess;

        Thread MeasureSpectram = null;
        //private Lisa.LISA_Status ls;

        private string infoIconSource;

        private History history;


        public string InfoIconSource
        {
            get => infoIconSource;
            set
            {
                infoIconSource = value;
                OnPropertyChanged(nameof(InfoIconSource));
            }
        }

        public string DetectorTemperature
        {
            get => detectorTemperature;
            set
            {
                detectorTemperature = value;
                OnPropertyChanged("DetectorTemperature");
            }
        }


        public int SelectedAnalysistype
        {
            get => selectedAnalysistype;
            set
            {
                selectedAnalysistype = value;
                OnPropertyChanged("SelectedAnalysistype");
            }
        }



        public string SampleFileName
        {
            get => sampleFileName;

            set
            {
                sampleFileName = value;
                OnPropertyChanged("SampleFileName");
            }
        }

        public int PassNo
        {
            get => passNo;
            set
            {
                passNo = value;
                OnPropertyChanged("PassNo");
            }
        }

        public String OpearatorName
        {
            get => opearatorName;
            set
            {
                opearatorName = value;
                OnPropertyChanged("OpearatorName");
            }
        }
        public int SelectedSampleType
        {
            get => selectedSampleType;
            set
            {
                selectedSampleType = value;
                OnPropertyChanged("SelectedSampleType");
            }
        }


        public bool IsReading
        {
            get
            {
                return isReading;
            }
            set
            {
                isReading = value;
                OnPropertyChanged("IsReading");
            }
        }
        public bool IsDataSavedDB
        {
            get
            {
                return isDataSavedDB;
            }
            set
            {
                isDataSavedDB = value;
                OnPropertyChanged("IsDataSavedDB");
            }
        }

        public bool IsRepeatmeasure
        {
            get
            {
                return isRepeatmeasure;
            }
            set
            {
                isRepeatmeasure = value;
                OnPropertyChanged("IsRepeatmeasure");
            }
        }
        public bool IsReadytoSave
        {
            get
            {
                return isReadytoSave;
            }
            set
            {
                isReadytoSave = value;
                OnPropertyChanged("IsReadytoSave");
            }
        }

        public bool Readingfinished
        {
            get
            {
                return readingfinished;
            }
            set
            {
                readingfinished = value;
                OnPropertyChanged("Readingfinished");
            }
        }

        public DateTime Starttime
        {
            get { return starttime; }
            set
            {
                starttime = value;
                OnPropertyChanged("Starttime");
            }
        }


        public double EthanolConcentration
        {
            get => ethanolConcentration;
            set
            {
                ethanolConcentration = value;
                OnPropertyChanged("Ethanolconcentration");
            }
        }

        public double MethanolConcentration
        {
            get => methanolConcentration;
            set
            {
                methanolConcentration = value;
                OnPropertyChanged("MethanolConcentration");
            }
        }
        public double DenaturantConcentration
        {
            get => denaturantConcentration;

            set
            {
                denaturantConcentration = value;
                OnPropertyChanged("DenaturantConcentration");
            }
        }
        public double WaterConcentration
        {
            get => waterConcentration;
            set
            {
                waterConcentration = value;
                OnPropertyChanged("WaterConcentration");
            }
        }


        public string MeasuremantBtnContent
        {
            get { return measurementContent; } //?? (measurementContent = "Start Measurement");
            set
            {
                measurementContent = value;
                OnPropertyChanged("MeasuremantBtnContent");

            }
        }

        public int MeasurementStatusCont
        {
            get => measurementStatusCont;
            set
            {
                measurementStatusCont = value;
                OnPropertyChanged("MeasurementStatusCont");
            }
        }

        public int MeasurementMaxCont
        {
            get => measurementMaxCont;
            set
            {
                measurementMaxCont = value;
                OnPropertyChanged("MeasurementMaxCont");
            }
        }

        public string MeasurementCompletedat
        {
            get => measurementCompletedat;
            set
            {
                measurementCompletedat = value;
                OnPropertyChanged("MeasurementCompletedat");
            }
        }

        public bool IsMeasurementCompleted
        {
            get => isMeasurementCompleted;
            set
            {
                isMeasurementCompleted = value;
                OnPropertyChanged(nameof(IsMeasurementCompleted));
            }
        }

        public bool IsMeasurementNotInRange
        {
            get => isMeasurementNotInRange;
            set
            {
                isMeasurementNotInRange = value;
                OnPropertyChanged(nameof(IsMeasurementNotInRange));
            }
        }
        public bool AnalysisSelection
        {
            get => analysisSelection;
            set
            {
                analysisSelection = value;
                OnPropertyChanged("AnalysisSelection");

            }

        }

        public bool AnalysisSelectionEnable
        {
            get => analysisSelectionEnable;
            set
            {
                analysisSelectionEnable = value;
                OnPropertyChanged("AnalysisSelectionEnable");
            }
        }

        public bool SaveBckVisibility
        {
            get => saveBckVisibility;
            set
            {
                saveBckVisibility = value;
                OnPropertyChanged("SaveBckVisibility");
            }
        }

        public bool MeasurementEnable
        {
            get => measurementEnable;
            set
            {
                measurementEnable = value;
                OnPropertyChanged("MeasurementEnable");
            }
        }

        public bool NewBckScanEnable
        {
            get => newBckScanEnable;
            set
            {
                newBckScanEnable = value;
                OnPropertyChanged("NewBckScanEnable");
            }
        }

        public string UserChooseDir
        {
            get => userChooseDir;
            set
            {
                userChooseDir = value;
                OnPropertyChanged("UserChooseDir");
            }
        }
        public int PyExceptionCount
        {
            get => pyExceptionCount;
            set
            {
                pyExceptionCount = value;
                OnPropertyChanged("PyExceptionCount");
            }
        }








        /// <summary>
        /// instance creation  
        /// </summary>

        CtrlMeasurement ctrlMeasurement;
        MainManager mmgr;
        SPC sPCcreation;



        /// <summary>
        /// global variable declaration 
        /// </summary>

        public string[] currentProductBaselineInfo = new string[128];
        public string[] currentProductPixeltoWavelenthInfo = new string[128];
        public LogWriter RawAbsorbanceDataLogger = new LogWriter();
        public LogWriter UserDataLogger = new LogWriter();

        public Pixelresult_data pixelresult_data = new Pixelresult_data();
        private Dias_Lisa.Lisa.LISA_Status ls;
        List<double> backgroundData = new List<double>();
        List<String> currentBaselineInfo = new List<String>();


        public ReadDetector(MainManager mmgr)
        {

            this.mmgr = mmgr;
            ctrlMeasurement = new CtrlMeasurement(mmgr);
            sPCcreation = mmgr.Spc_convertion;
            MeasuremantBtnContent = "Start Measurement";
            MeasurementCompletedat = $"Ready to measure";
            AnalysisSelectionEnable = true;
            InfoIconSource = @"C:\FuelAnalyzer\bin\Icon\Info_Icon.png";
            _dataAccess = new DataAccess(_connectionString);
            history = new History(mmgr);
            MeasurementEnable = true;
            NewBckScanEnable=true;  
        }


        public string BrowseLocation()
        {
            string serr = "";

            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (UserChooseDir != "" && Directory.Exists(UserChooseDir))
                dialog.SelectedPath = UserChooseDir;
            var result = dialog.ShowDialog();
            // string path= dialog.SelectedPath;
            if (result.ToString() != string.Empty)
            {
                UserChooseDir = dialog.SelectedPath;

                if (UserChooseDir != "" && Directory.Exists(UserChooseDir))
                {
                    Directory.CreateDirectory(UserChooseDir);
                    //SystemPath.RootLogDirectory = mmgr.AppConfig.AppLogDirectory;
                    // mmgr.ErrorEventMngr.Init("", SystemPath.GetLogPath);
                    // if (!Directory.Exists(SystemPath.GetLogPath))
                    // Directory.CreateDirectory(SystemPath.GetLogPath);

                }


            }

            return serr;
        }
        public string LogData()
        {

            string serr = "";
            UserDataLogger.IsTimeStampPrefixOnFileName = false;
            UserDataLogger.IsAppendFileCount = false;
            if (UserChooseDir == "")
            {
                UserDataLogger.InitLogFile(SystemPath.GetLogPath, SampleFileName + ".csv"); //
                UserDataLogger.HeaderString = "Time Stamp,Name,Pass No, Operator,Analysis Type,Sample Type,Ethanol,Denaturant,Methanol,Water,";//Temperature 
            }
            else
            {
                UserDataLogger.InitLogFile(UserChooseDir, SampleFileName + ".csv"); //
                UserDataLogger.HeaderString = "Time Stamp,Name,Pass No, Operator,Analysis Type,Sample Type,Ethanol,Denaturant,Methanol,Water,";//Temperature 
            }

            UserDataLogger.WriteToLog(DateTime.Now.ToString("dd:mm:yyyy HH:mm tt ") + "," + SampleFileName + "," + "PassNo" + "," + "Operator" + ", " + "AnalyType" + "," + "SampleType" + "," + EthanolConcentration + "," + "DenaturantConcentration" + "," + "Methanolconcentration" + "," + "WaterConcentration" + "", true); //



            return serr;


        }


        //this.Dispatcher.BeginInvoke(new Action(() =>
        //   {

        // }), System.Windows.Threading.DispatcherPriority.Background);

        /// <summary>
        /// This function help to read the baseline information (emitter off data)its used for absobance calculation .
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public string ReadBaselineInfo(string filepath)
        {

            bool cfg_skipheader = false;
            string[] baselineData;
            int cfg_row_index = 0;
            int cfg_col_index = 0;
            int cfg_col_size = 0;
            string testfilepath = "C:\\FuelAnalyzer\\Baseline" + ".csv";
            string serr = "";
            /*   List<List<string>> baselinePixelDataLst = new List<List<string>>();
               try
               {
                   using (TextFieldParser baselineReadData = new TextFieldParser(testfilepath))
                   {
                       baselineReadData.HasFieldsEnclosedInQuotes = true;
                       baselineReadData.SetDelimiters(",");
                       while (!baselineReadData.EndOfData)
                       {

                           baselineData = baselineReadData.ReadFields();
                           foreach (string field in baselineData)
                           {
                               if (!cfg_skipheader)
                               {
                                   baselinePixelDataLst.Add(new List<string>());
                                   cfg_col_size++;
                               }
                           }
                           cfg_skipheader = true; //init completed
                           cfg_col_index = 0;

                           foreach (string field in baselineData)
                           {
                               if (cfg_col_index < cfg_col_size)
                               {
                                   //ignore 1st  row
                                   if (cfg_row_index >= 1)
                                       baselinePixelDataLst[cfg_col_index].Add(field);
                               }
                               cfg_col_index++;
                           }
                           cfg_row_index++;
                       }
                       baselineReadData.Close();
                   }
                   currentProductBaselineInfo = baselinePixelDataLst[5].ToArray();


                   return serr;
               }
               catch (Exception ex)
               {
                   serr = ex.Message;
                   return serr;
               }
            */


            StreamReader ControlPageReader = new StreamReader(testfilepath);
            string sDataLine = "";
            string[] sControlData = { "" };
            backgroundData.Clear();
            while (true)
            {
                sDataLine = ControlPageReader.ReadLine();

                if (sDataLine == null)
                    break;

                sControlData = sDataLine.Split(',');

                currentBaselineInfo.Add(sControlData[0]);
                // currentProductBaselineInfo
            }
            // bReadrefData1 = backgroundData;
            ControlPageReader.Dispose();
            ControlPageReader.Close();
            return serr;

        }


        public string ReadPixelWavelength(string filepath)
        {

            bool cfg_skipheader = false;
            string[] pixeltoWavelenthDataAry;
            int cfg_row_index = 0;
            int cfg_col_index = 0;
            int cfg_col_size = 0;
            string testfilepath = "C:\\FuelAnalyzer\\Linear_Array_Pixel_To_Wavenum" + ".csv";
            string serr = "";
            List<List<string>> pixeltoWavelenthDataLst = new List<List<string>>();
            try
            {
                using (TextFieldParser PixeltoWavelenthReadData = new TextFieldParser(testfilepath))
                {
                    PixeltoWavelenthReadData.HasFieldsEnclosedInQuotes = true;
                    PixeltoWavelenthReadData.SetDelimiters(",");
                    while (!PixeltoWavelenthReadData.EndOfData)
                    {

                        pixeltoWavelenthDataAry = PixeltoWavelenthReadData.ReadFields();
                        foreach (string field in pixeltoWavelenthDataAry)
                        {
                            if (!cfg_skipheader)
                            {
                                pixeltoWavelenthDataLst.Add(new List<string>());
                                cfg_col_size++;
                            }
                        }
                        cfg_skipheader = true; //init completed
                        cfg_col_index = 0;

                        foreach (string field in pixeltoWavelenthDataAry)
                        {
                            if (cfg_col_index < cfg_col_size)
                            {
                                //ignore 1st  row
                                if (cfg_row_index >= 1)
                                    pixeltoWavelenthDataLst[cfg_col_index].Add(field);
                            }
                            cfg_col_index++;
                        }
                        cfg_row_index++;
                    }
                    PixeltoWavelenthReadData.Close();
                }
                currentProductPixeltoWavelenthInfo = pixeltoWavelenthDataLst[5].ToArray();


                return serr;
            }
            catch (Exception ex)
            {
                serr = ex.Message;
                return serr;
            }



        }

        public string ReadBackground(string path)
        {
            string serr = "";
            string filename = "C:\\FuelAnalyzer\\BackgroundP6_5Hz" + ".csv";
            StreamReader ControlPageReader = new StreamReader(filename);
            string sDataLine = "";
            string[] sControlData = { "" };
            backgroundData.Clear();
            while (true)
            {
                sDataLine = ControlPageReader.ReadLine();

                if (sDataLine == null)
                    break;

                sControlData = sDataLine.Split(',');

                backgroundData.Add((Convert.ToDouble(sControlData[0])));

            }
            // bReadrefData1 = backgroundData;
            ControlPageReader.Dispose();
            ControlPageReader.Close();
            return serr;
        }

        public unsafe string LisaConnect()
        {
            //string ls = "";
            string serr = "";

            int size = 0;
            int freq = (int)mmgr.DetectorConfigurationData.Samplingfreq.HoldValue;//3;
            int delay = (int)mmgr.DetectorConfigurationData.Delaytime.HoldValue;
            int vdr = (int)mmgr.DetectorConfigurationData.Vdrtime.HoldValue;
            int vvr = (int)mmgr.DetectorConfigurationData.Vvrtime.HoldValue;
            int pulseWidth = (int)mmgr.DetectorConfigurationData.Pulsewidth.HoldValue;
            try
            {
                ls = Dias_Lisa.Lisa.LISA_LogIn();
                if (ls != Dias_Lisa.Lisa.LISA_Status.LISA_OK) return ls.ToString();
                ls = Dias_Lisa.Lisa.LISA_SetSyncDirection(false);   //Sync. puls as output
                if (ls != Dias_Lisa.Lisa.LISA_Status.LISA_OK) return ls.ToString();

                ls = Dias_Lisa.Lisa.LISA_SetArraySize(size); // Set the size of the array
                if (ls != Dias_Lisa.Lisa.LISA_Status.LISA_OK) return ls.ToString();

                ls = Dias_Lisa.Lisa.LISA_SetSampleFreq(freq);  // Set the freq. of sample cycle
                if (ls != Dias_Lisa.Lisa.LISA_Status.LISA_OK) return ls.ToString();


                ls = Dias_Lisa.Lisa.LISA_SetSyncDelay(delay);    // Set the freq. of sample cycle
                if (ls != Dias_Lisa.Lisa.LISA_Status.LISA_OK) return ls.ToString();

                ls = Dias_Lisa.Lisa.LISA_SetSyncLowActive(false);   // Set the freq. of sample cycle
                if (ls != Dias_Lisa.Lisa.LISA_Status.LISA_OK) return ls.ToString();

                ls = Dias_Lisa.Lisa.LISA_SetVDR(vdr);    // Set the freq. of sample cycle
                if (ls != Dias_Lisa.Lisa.LISA_Status.LISA_OK) return ls.ToString();


                ls = Dias_Lisa.Lisa.LISA_SetVVR(vvr);    // Set the freq. of sample cycle
                if (ls != Dias_Lisa.Lisa.LISA_Status.LISA_OK) return ls.ToString();

                ls = Dias_Lisa.Lisa.LISA_SetSyncWidth(pulseWidth);    // Set the freq. of sample cycle
                if (ls != Dias_Lisa.Lisa.LISA_Status.LISA_OK) return ls.ToString();

                ls = Dias_Lisa.Lisa.LISA_SetStart();   // Starts the sample cycle and                   
                if (ls != Dias_Lisa.Lisa.LISA_Status.LISA_OK) return ls.ToString();


                Dias_Lisa.Lisa.Action();


                return serr;

            }
            catch (Exception ex)
            {
                return "Detector Login Exception" + ex.ToString();
            }



        }
        public string UpdatePLSResultVisibility()
        {
            string ser = "";
            if (SelectedAnalysistype == 0)
            {
                AnalysisSelection = false;

            }

            else
            {
                AnalysisSelection = true;

            }



            return ser;
        }
        public bool checkfeildentry()
        {
            if (string.IsNullOrWhiteSpace(OpearatorName) || string.IsNullOrWhiteSpace(PassNo.ToString()) || string.IsNullOrWhiteSpace(SampleFileName))
                return false;

            return true;
        }
        public bool checkfeildentryexist()
        {
            //if (string.IsNullOrWhiteSpace(OpearatorName) || string.IsNullOrWhiteSpace(PassNo.ToString()) || string.IsNullOrWhiteSpace(SampleFileName)) //implement query logic if the combination exists 
            //    return true;
            if (string.IsNullOrWhiteSpace(OpearatorName) || PassNo == 0 || string.IsNullOrWhiteSpace(SampleFileName))
                return false;
            if (SelectedAnalysistype == 0)
            {
                string analysisType = "Methanol";
                string checkFieldEntry = "SELECT COUNT(*) FROM Measurement WHERE [Pass No.] = @PassNo AND [Name] = @SampleFileName AND [Analysis type] = @analysisType AND [Time Stamp] >= @Starttime";

                try
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {

                        { "@PassNo", PassNo },
                        { "@SampleFileName", SampleFileName },
                        { "@analysisType", analysisType },
                        { "@Starttime", DateTime.Today }
                    };

                    foreach (var parameter in parameters.ToList())
                    {
                        if (parameter.Value == null)
                        {
                            parameters[parameter.Key] = DBNull.Value;
                        }
                    }
                    return _dataAccess.CheckData(checkFieldEntry, parameters);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                string analysisType = "Ethanol";
                string checkFieldEntry = "SELECT COUNT(*) FROM Measurement WHERE [Pass No.] = @PassNo AND [Name] = @SampleFileName AND [Analysis type] = @analysisType AND [Time Stamp] >= @Starttime";

                try
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {

                        { "@PassNo", PassNo },
                        { "@SampleFileName", SampleFileName },
                        { "@analysisType", analysisType },
                        { "@Starttime", DateTime.Today }
                    };

                    foreach (var parameter in parameters.ToList())
                    {
                        if (parameter.Value == null)
                        {
                            parameters[parameter.Key] = DBNull.Value;
                        }
                    }
                    return _dataAccess.CheckData(checkFieldEntry, parameters);
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

        }
        public bool SaveMeasurementData()
        {
            if (!checkfeildentryexist())
            {
                if (_dataAccess == null)
                {
                    return false;
                }

                string getLatestBatchQuery = "SELECT MAX(Batch) AS LatestBatch FROM Measurement";

                try
                {
                    int latestBatch = _dataAccess.GetScalarValue<int>(getLatestBatchQuery);
                    int nextBatch = (PassNo == 1) ? latestBatch + 1 : latestBatch;
                    string pass_No = PassNo.ToString("D3");

                    if (SelectedAnalysistype == 1)
                    {
                        string analysisType = "Ethanol";
                        string sampleType = "Fuel Ethanol";
                        string savedMeasurementQuery = @"INSERT INTO Measurement ([Time Stamp], [Name], [Pass No.], [Operator], [Analysis type], [Sample Type], [Ethanol], [Denaturant], [Methanol], [Water], [Batch]) VALUES (GETDATE(), @SampleFileName, @pass_No, @OpearatorName, @analysisType, @sampleType, @EthanolConcentration,@DenaturantConcentration , @MethanolConcentration, @WaterConcentration, @nextBatch)";

                        Dictionary<string, object> parameters = new Dictionary<string, object>
                        {
                            { "@SampleFileName", SampleFileName },
                            { "@pass_No", pass_No },
                            { "@OpearatorName", OpearatorName },
                            { "@analysisType", analysisType },
                            { "@sampleType", sampleType },
                            { "@EthanolConcentration", EthanolConcentration},
                            { "@DenaturantConcentration", DenaturantConcentration},
                            { "@MethanolConcentration",MethanolConcentration},
                            { "@WaterConcentration", WaterConcentration },
                            { "@nextBatch", nextBatch}
                        };

                        // Replace null values with DBNull.Value
                        foreach (var parameter in parameters.ToList())
                        {
                            if (parameter.Value == null)
                            {
                                parameters[parameter.Key] = DBNull.Value;
                            }
                        }

                        return _dataAccess.InsertData(savedMeasurementQuery, parameters);
                    }
                    else
                    {
                        string analysisType = "Methanol";
                        string sampleType = "Fuel Methanol";
                        string savedMeasurementQuery = @"INSERT INTO Measurement ([Time Stamp], [Name], [Pass No.], [Operator], [Analysis type], [Sample Type], [Methanol], [Batch]) VALUES (GETDATE(), @SampleFileName, @pass_No, @OpearatorName, @analysisType, @sampleType, @MethanolConcentration, @nextBatch)";

                        Dictionary<string, object> parameters = new Dictionary<string, object>
                        {
                            { "@SampleFileName", SampleFileName },
                            { "@pass_No", pass_No },
                            { "@OpearatorName", OpearatorName },
                            { "@analysisType", analysisType },
                            { "@sampleType", sampleType },
                            { "@MethanolConcentration",MethanolConcentration},
                            { "@nextBatch", nextBatch}
                        };

                        // Replace null values with DBNull.Value
                        foreach (var parameter in parameters.ToList())
                        {
                            if (parameter.Value == null)
                            {
                                parameters[parameter.Key] = DBNull.Value;
                            }
                        }

                        return _dataAccess.InsertData(savedMeasurementQuery, parameters);
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception details (you can use a logging framework or print to console)
                    Console.WriteLine($"Exception: {ex.Message}");
                    return false; // Or rethrow the exception if needed
                }
            }

            return false;
        }

        public void SaveFilePDF()
        {
            string analysisType;
            if (SelectedAnalysistype == 0)
            {
                analysisType = "Methanol";
            }
            else
            {
                analysisType = "Ethanol";
            }
            string filePath = $"{SampleFileName}_{PassNo.ToString("D3")}_{DateTime.Now:yyyyMMdd}_{analysisType}.pdf";

            using (var writer = new PdfWriter(Path.Combine(UserChooseDir, filePath)))
            using (var pdf = new PdfDocument(writer))
            {
                FooterEventHandler pageEvent = new FooterEventHandler();
                pdf.AddEventHandler(iText.Kernel.Events.PdfDocumentEvent.END_PAGE, pageEvent);

                using (var document = new Document(pdf))
                {
                    //int totalPages = allDataItems.Count;
                    Paragraph title = new Paragraph("FUEL ANALYZER MEASUREMENT REPORT")
                        .SetFontColor(ColorConstants.WHITE)
                        .SetFontSize(16)
                        .SetBold();
                    string logoPath = @"C:\FuelAnalyzer\bin\Icon\Company_Logo.png"; // Replace with the actual path to your logo
                    ImageData imageData = ImageDataFactory.Create(logoPath);
                    Image logoImage = new Image(imageData).ScaleAbsolute(30, 30).SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.RIGHT);


                    Table headerTable = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
                    headerTable.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    headerTable.SetBackgroundColor(ColorConstants.BLUE);


                    Cell titleCell = new Cell(1, 2).Add(title).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT);
                    headerTable.AddCell(titleCell);


                    Cell logoCell = new Cell().Add(logoImage).SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    headerTable.AddCell(logoCell);


                    document.Add(headerTable);

                    document.Add(new Paragraph("\n"));

                    Paragraph reportTitle = new Paragraph("REPORT").SetFontColor(ColorConstants.BLACK)
                        .SetFontSize(16)
                        .SetBold();
                    document.Add(reportTitle);
                    document.Add(new Paragraph("\n"));

                    Table additionalInfoTable = history.CreateAdditionalInfoTable();// Add iTextSharp table with additional information
                    document.Add(additionalInfoTable);

                    document.Add(new Paragraph("\n"));
                    Paragraph equipmentInfo = new Paragraph("EQUIPMENT INFORMATION").SetFontColor(ColorConstants.BLACK)
                        .SetFontSize(16)
                        .SetBold();
                    document.Add(equipmentInfo);
                    document.Add(new Paragraph("\n"));

                    Table equipmentInfoTable = history.CreateEquipmentInfoTable();// Add iTextSharp table with additional information
                    document.Add(equipmentInfoTable);

                    int pageNumber = 1;

                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

                    Paragraph sampleReport = new Paragraph($"SAMPLE MEASUREMENT REPORT {pageNumber}").SetFontColor(ColorConstants.WHITE).SetFontSize(16).SetBold().SetBackgroundColor(ColorConstants.BLUE);
                    document.Add(sampleReport);
                    document.Add(new Paragraph("\n"));

                    Table summarySelectedItemsTable = history.CreateMeasurementReportTable();
                    document.Add(summarySelectedItemsTable);

                    document.Add(new Paragraph("\n"));

                    Paragraph passesResultInfo = new Paragraph("PASS RESULTS").SetFontColor(ColorConstants.BLACK)
                        .SetFontSize(16)
                        .SetBold();
                    document.Add(passesResultInfo);
                    document.Add(new Paragraph("\n"));

                    Table selectedItemsTable = history.PassResultTable();
                    document.Add(selectedItemsTable);


                }

                IsMeasurementCompleted = true;
                InfoIconSource = @"C:\FuelAnalyzer\bin\Icon\Info-GreenSign_Icon.png";
                MeasurementCompletedat = $"File Saved Successfully At {DateTime.Now.ToString("HH:mm:ss")}";
            }

        }

        public bool StartMeasurement(int avraragecount, int dataSet)
        {
            MeasuremantBtnContent = "Cancel Measurement";
            AnalysisSelectionEnable = false;
            IsReadytoSave = false;
            NewBckScanEnable = false;
            MeasurementEnable =true;
            //  if (PropertyChanged != null)
            // PropertyChanged(this, new PropertyChangedEventArgs("MeasuremantBtnContent"));
            Starttime = DateTime.Now;
            //  RawAbsorbanceDataLogger.WriteToLog(Starttime.ToString("HH:mm:ss fff") + ", " + "" + "," + "" + "," + "" + "," + "," + "", true); //DateTime.Now.ToString("h:mm:ss tt") + "," +

            bool success = false;

            do
            {

                if (isReading) { success = true; MeasurementCompletedat = "Previous Measuring in Progress..."; break; }
                stopReq = false;
                filecnt = 0;
                MeasurementCompletedat = "Measuring...";


                //  MeasureSpectram = new Thread(PAT_Sensor_Read);
                // MeasureSpectram.Start();

                var t = new Thread(() => PAT_Sensor_Read(false));
                t.Start();

                // MeasureSpectram.Join();

            }
            while (false);

            return success;
        }

        public void CancelMeasurement()
        { 
            

            if (!isReading)
            {
                AnalysisSelectionEnable = true;
                MeasuremantBtnContent = "Start Measurement";
                return;

            }
            Readingfinished = false; //flag changes 
            IsRepeatmeasure = false;
            AnalysisSelectionEnable = true;
            MeasurementEnable = true;
            NewBckScanEnable = true;
            saveBckVisibility = false;
            stopReq = true;
            //  MeasurementCompletedat = $"Measurement Was Cancelled";
            MeasuremantBtnContent = "Start Measurement";
        }
        public void CancelRepeatMeasurement()
        {

            if (!isReading)
            {
                AnalysisSelectionEnable = false;
                MeasuremantBtnContent = "New Measurement";
                return;

            }
            Readingfinished = false; //flag changes 
            AnalysisSelectionEnable = false;
            NewBckScanEnable = false;
            IsRepeatmeasure = true;
            MeasurementEnable = true;
            stopReq = true;
            //  MeasurementCompletedat = $"Measurement Was Cancelled";
            MeasuremantBtnContent = "New Measurement";
        }



        void PAT_Sensor_Read(bool isBackgroundRead)
        {
            // List<Pixelresult_data1> Collected_result = new List<Pixelresult_data1>();
            // List<Pixelresult_data1> Collected_result_Raw = new List<Pixelresult_data1>();
            // myList = new List<List<string>>();


            int page = 0;
            var temporalCv = new double[1000]; // new double[1000]  SeriesCollection[0].Values
                                               //  var serialdat = new ChartValues<double>();
                                               //  Collected_result = new List<myList>();
            float OutputData;
            double[] lisa_values = new double[128];
            int ms = mmgr.MeasurementConfigurationData.DelaybtwnMeasurement.HoldValue;
            int Sec = mmgr.MeasurementConfigurationData.DelaybtwnMeasurement1.HoldValue;
            int measurementCount = mmgr.MeasurementConfigurationData.RepeatMeasurement.HoldValue;
            MeasurementMaxCont = measurementCount - 1;
            int delay = (Sec * 1000) + ms;
            IsReading = true;

            int i = 0;
            try
            {

                while (i < measurementCount)
                {
                    MeasurementStatusCont = i;

                    if (ls != Dias_Lisa.Lisa.LISA_Status.LISA_OK)
                    {
                        IsReading = false;
                        return;
                    }

                    lock (obj)
                    {
                        if (!Readingfinished)
                        {
                            Readingfinished = true;
                            lisa_values = ReadPixelVolt(isBackgroundRead);
                            page++;

                            Thread.Sleep(delay);
                            // Collected_result.Add(pixelresult_data1);
                            // Collected_result_Raw.Add(Raw_pixelresult_data1);

                            //  Collected_result[i].Add(myList);
                            filecnt++;
                            i++;

                        }
                        else
                            Thread.Sleep(3);
                    }


                    if (stopReq) break;

                }
                DateTime cycleCompletedAt = DateTime.Now;
                // AnalysisSelectionEnable = true;
                if (i == measurementCount)
                {
                    IsDataSavedDB = false;
                    IsRepeatmeasure = true;
                    IsReadytoSave = true;
                    IsMeasurementCompleted = true;
                    InfoIconSource = @"C:\FuelAnalyzer\bin\Icon\Info-GreenSign_Icon.png";

                    MeasuremantBtnContent = "New Measurement";
                    MeasurementCompletedat = $"Ready to measure";
                    MeasurementCompletedat = $"Measurement Completed At {cycleCompletedAt.ToString("HH:mm:ss")}";

                }


            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Measurement Error");
            }

            IsReading = false;
            //AnalysisSelectionEnable = true;
            //MeasuremantBtnContent = "Start Measurement";
            // MeasuremantBtnContent = "New Measurement"; 



        }

        public void ResetforStartmeasurement()
        {
            // IsDataSavedDB = false;
            PassNo = 1;
            MeasurementStatusCont = 0;
            AnalysisSelectionEnable = true;
            IsRepeatmeasure = false;
            IsReadytoSave = false;
            IsMeasurementCompleted = false;
            InfoIconSource = @"C:\FuelAnalyzer\bin\Icon\Info_Icon.png";
            MeasuremantBtnContent = "Start Measurement";
            MeasurementCompletedat = $"Ready to measure";
           NewBckScanEnable = true;

        }
        public string PLSCalibration()
        {

            if (SelectedAnalysistype == 0)
            {

            }
            else
            {

            }
            return "";


        }

        public double[] ReadPixelVolt(bool isBackgroundRead)
        {
            string backgroundPath = "C:\\FuelAnalyzer\\BackgroundP6_5Hz" + ".csv";
            string baselinePath = "C:\\FuelAnalyzer\\Baseline" + ".csv";
            string logtime = DateTime.Now.ToString("HH-mm-ss");
            string rawDataPath = SystemPath.GetLogPath + "\\" + DateTime.Now.ToString("yyyy-MMM") + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + "-Raw_Absorbance" + filecnt.ToString() + "_" + logtime + ".csv";
            //string rawDataPath = "C:\\FuelAnalyzer Logs\\Log\\2023-Nov\\2023-10-26-Raw_AbsorbanceSample2931" + ".csv";
            string prototypeName = "P6";
            string pLSModelPath = @"C:\FuelAnalyzer\P6_PLS_model.pkl";
            List<List<string>> rawSPCList = new List<List<string>>();
            List<List<string>> myList = new List<List<string>>();
            // List<Pixelresult_data> Sensorpixelresult = new List<Pixelresult_data>();

            int lisaPixelSize = mmgr.DetectorConfigurationData.PixelSize.HoldValue;
            int lisaAvrgCount = mmgr.MeasurementConfigurationData.Avaragecount.HoldValue;
            int scanBetweenInterval = (int)mmgr.MeasurementConfigurationData.DelaybtwnScans.HoldValue;
            int Min_wavenumber = (int)mmgr.MeasurementConfigurationData.Minwavenumber.HoldValue;
            int Max_wavenumber = (int)mmgr.MeasurementConfigurationData.Maxwavenumber.HoldValue;
            // int Stepdata = mmgr.MeasurementConfigurationData.Stepvalue.HoldValueString;
            double rawAdcToOutput;
            //double[] Reconstruct_Absorbance = new double[256];
            //double[] Rawavolt = new double[256];
            int temperature;
            double[] Rawabsorbance = new double[128];
            double[] Rawavolt = new double[128]; //256
            int cfg_read_index = 0;
            //int cfg_print_index = 0;
            bool cfg_dataview_set = false;
            int cfg_row_index = 0;
            int cfg_col_index = 0;
            int cfg_col_size = 0;

            int cfg_row_index1 = 0;
            int cfg_read_index1 = 0;
            int cfg_col_index1 = 0;
            int cfg_col_size1 = 0;


            RawAbsorbanceDataLogger.InitLogFile(SystemPath.GetLogPath, "Raw_Absorbance" + filecnt.ToString() + "_" + logtime + ".csv"); //mmgr.DetectorConfigurationData
            RawAbsorbanceDataLogger.IsAppendFileCount = false;  //file count number after file name   
                                                                // LisaDataLogger.SetLogFileName("LisaData");
            RawAbsorbanceDataLogger.HeaderString = "Time Stamp(ms),Pixel No,Wavenumber(Cm^-1), Output(v),RawAbsorbance,Temperature";//Temperature 


            string[] fields = { "Time Stamp(ms)", "Pixel No", "Wavelength(Cm ^ -1)", "Output(v)", "Absorbance" };
            string[] Rawfields = { "Time Stamp(ms)", "Pixel No", "Wavelength(Cm ^ -1)", "Output(v)", "Absorbance" };
            int rawADCVal;
            double avgout = 0;//current sensor Raw voltage 
                              // double pixel_linearfit = 5.432722;   // 5.565646; old value
                              // double pixel_linerfitdiff = 0.043489; //0.040664; 
            double pixel_um = 0;
            double pixel_cm = 0;
            int[] pixelvolt = new int[lisaPixelSize];
            rawSPCList = new List<List<string>>();
            myList = new List<List<string>>();
            List<List<double>> adcDatalstoflst = new List<List<double>>();
            adcDatalstoflst.Clear();
            //pixelresult_data1 = new Pixelresult_data1();
            //Raw_pixelresult_data1 = new Pixelresult_data1();
            // ArrayList pixelvoltlist = new ArrayList();
            //pixelvolt = new int[Lisapixelsize];
            // List<Pixelresult_data> Sensorpixelresult = new List<Pixelresult_data>();
            string lsStatus = "";

            try
            {
                foreach (string field in fields)
                {
                    //setup dataview col
                    if (!cfg_dataview_set)
                    {
                        // pixelresult_data.myList
                        myList.Add(new List<string>());
                        // pixelresult_data1.myList.Add(new List<string>());
                        cfg_col_size++;
                    }
                }
                // cfg_dataview_set = true; //init completed


                cfg_col_index = 0;

                foreach (string field in Rawfields)
                {
                    //setup dataview col
                    if (!cfg_dataview_set)
                    {
                        // pixelresult_data.myList
                        rawSPCList.Add(new List<string>());
                        // Raw_pixelresult_data1.myList.Add(new List<string>());
                        cfg_col_size1++;
                    }
                }
                cfg_dataview_set = true; //init completed

                cfg_col_index = 0;
                ///

                for (int count = 0; count < lisaAvrgCount; count++)
                {
                    avgout = 0;

                    lsStatus = GetPixelVoltage(ref pixelvolt, lisaPixelSize);
                    Dias_Lisa.Lisa.Action();
                    List<double> adcData = new List<double>();
                    adcData.Clear();
                    // adcData.Clear();
                    // pixelresult_data = new Pixelresult_data();
                    // if (ls == Dias_Lisa.Lisa.LISA_Status.LISA_OK)
                    if (lsStatus == "LISA_OK" || lsStatus == "LISA_NotNewValues")
                    {


                        for (int i = 0; i < lisaPixelSize; i++)
                        {
                            rawADCVal = pixelvolt[i];
                            rawAdcToOutput = (double)(rawADCVal * 0.001220703125);
                            // pixelresult_data.output1.Add(rawAdcToOutput);
                            adcData.Add(rawAdcToOutput);
                            //myList.Add(output[i].ToString());

                        }
                        adcDatalstoflst.Add(adcData);
                        // Sensorpixelresult.Add(pixelresult_data);
                        // pixelvoltlist.Add(Values1);
                        temperature = 0;
                        Dias_Lisa.Lisa.LISA_GetSensorTemp(out temperature);
                        DetectorTemperature = ((double)(temperature * 0.1) - 273.15).ToString("f3");
                        // GetTemp(ref temperature);
                        // Temperature_data = ((temperature[0] * 0.1) - 273.2).ToString();

                    }
                    else
                    {
                        Dias_Lisa.Lisa.Action();
                        //lsStatus = "";
                        //ls = Dias_Lisa.Lisa.LISA_SetStop();
                        //ls = Dias_Lisa.Lisa.LISA_LogOut();
                        //if (ls == Dias_Lisa.Lisa.LISA_Status.LISA_OK)
                        //{
                        //     LisaConnect(); 
                        //}


                    }
                    Thread.Sleep(scanBetweenInterval);
                    Thread.Sleep(1);
                }
                Dias_Lisa.Lisa.Action();
                Thread.Sleep(1);
                TimeSpan loggedtime = (DateTime.Now - Starttime);
                for (int i = 0; i < 128; i++)
                {
                    avgout = 0;
                    cfg_col_index1 = 0;
                    Int64 totaltime = loggedtime.Milliseconds + (loggedtime.Seconds * 1000) + loggedtime.Minutes * 60000 + loggedtime.Hours * 3600000;

                    for (int k = 0; k < adcDatalstoflst.Count; k++)
                    {
                        // avgout += Sensorpixelresult[k].output1[i];
                        avgout += adcDatalstoflst[k][i];


                    }

                    avgout = avgout / adcDatalstoflst.Count;
                    pixel_um = Convert.ToDouble(currentProductPixeltoWavelenthInfo[i]);
                    pixel_cm = 10000 / pixel_um;
                    Rawavolt[i] = avgout;
                    Rawabsorbance[i] = -1 * Math.Log10(((Convert.ToDouble(currentBaselineInfo[i]) + 1.0) - avgout) / ((Convert.ToDouble(currentBaselineInfo[i]) + 1.0) - backgroundData[i]));
                    //Rawabsorbance[i] = (-1 * Math.Log10(((Convert.ToDouble(currentProductBaselineInfo[i]) + 1.0) - avgout) / ((Convert.ToDouble(currentProductBaselineInfo[i]) + 1.0) - backgroundData[i])));

                    // Samplevalue_raw[i] = Rawabsorbance[i];


                    //if (Iscsv_)
                    // DateTime.Now.ToString(" h:mm:ss fff")//totaltime.ToString()
                    if (!isBackgroundRead)
                    {

                        RawAbsorbanceDataLogger.WriteToLog(DateTime.Now.ToString("HH:mm:ss fff") + "," + i.ToString() + "," + pixel_cm.ToString() + "," + Rawavolt[i].ToString("F6") + "," + Rawabsorbance[i].ToString("F6") + "," + DetectorTemperature + "", true); //DateTime.Now.ToString("h:mm:ss tt") + "," +  Math.Log10((Eoff_ref_data1[i] - avgout)/(Eoff_ref_data1[i] - bReadrefData1[i]))


                        Rawfields[0] = totaltime.ToString();
                        Rawfields[1] = i.ToString();
                        Rawfields[2] = pixel_cm.ToString();
                        Rawfields[3] = Rawavolt[i].ToString("F3");
                        Rawfields[4] = Rawabsorbance[i].ToString("F6");

                        foreach (string field in Rawfields)
                        {
                            if (cfg_col_index1 < cfg_col_size1)
                            {

                                rawSPCList[cfg_col_index1].Add(field);
                                //Raw_pixelresult_data1.myList[cfg_col_index1].Add(field);
                            }

                            //increment col index
                            cfg_col_index1++;
                        }

                        //increment cfg read index
                        cfg_read_index1++;

                        cfg_row_index1++;

                        ////
                    }
                }
                if (!isBackgroundRead)
                {

                    string concentrationResultArray = ImportPythonTest(backgroundPath, baselinePath, rawDataPath, pLSModelPath);
                    if (concentrationResultArray != null)
                    {
                        // Remove the brackets and split the string by space
                        string[] concentrationStringArray = concentrationResultArray.Trim('[', ']').Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                        //Remove the null or empty elemnrt in the string array using Linq method  
                        concentrationStringArray = concentrationStringArray.Where(x => !string.IsNullOrEmpty(x)).ToArray();

                        double[] concentrationDoubleArray = new double[concentrationStringArray.Length];

                        for (int i = 0; i < concentrationDoubleArray.Length; i++)
                        {

                            concentrationDoubleArray[i] = double.Parse(concentrationStringArray[i]);
                        }

                        EthanolConcentration = Math.Round(concentrationDoubleArray[0], 2); 
                        MethanolConcentration = Math.Round(concentrationDoubleArray[1], 2);
                        WaterConcentration = Math.Round(concentrationDoubleArray[2], 2);
                        DenaturantConcentration = Math.Round(concentrationDoubleArray[3], 2);
                    }
                    else
                    {
                        PyExceptionCount++;

                        if (PyExceptionCount > (int)(MeasurementMaxCont / 100))
                        {
                            IsReadytoSave = false;
                            MeasurementCompletedat = $"Measurement Was not Successful exceeds the cont {PyExceptionCount}";
                            IsMeasurementNotInRange = true;
                            InfoIconSource = @"C:\FuelAnalyzer\bin\Icon\Info_RedSign_Icon.png";
                            this.CancelMeasurement();
                        }
                    }
                }

                string exe = "";
                string out_exe;
                // if (Isspc) ;
                // out_exe = RawConvertSPC(out exe, RawList);

                //write_jdx_single("test", RawList);



                exe = "";
                // string out_exe;
                //if (Isspc)
                //     out_exe = ConvertSPC(out exe);


            }

            catch (Exception ex)

            {
                System.Windows.MessageBox.Show(ex.Message, "Read Detector Error!");
            }
            Thread.Sleep(1);
            Readingfinished = false;
            return Rawavolt;
        }

        public unsafe string GetPixelVoltage(ref int[] outArray, int count)
        {
            Dias_Lisa.Lisa.LISA_Status ls;
            IntPtr dataParam;

            fixed (int* p = outArray)
            {
                dataParam = (IntPtr)p;
            }


            ls = Dias_Lisa.Lisa.LISA_GetPixelValues(dataParam, count);
            Marshal.Copy(dataParam, outArray, 0, count);
            Thread.Sleep(1);
            return ls.ToString();
        }



        public string ImportPythonTest(string Emitter_On_Background_Path, string Emitter_Off_Background_Path, string Raw_Data_Path, string PLS_Model_Path)
        {
            List<object> myList = new List<object>();
            PythonEngine.Initialize();
            var Concentration_threadState = PythonEngine.BeginAllowThreads();// allow to threed 
            string pythonConcentrationArrayString;
            try
            {

                using (Py.GIL())
                {


                    dynamic PaeoniaPLS = Py.Import("PaeoniaModel");
                    dynamic result = PaeoniaPLS.Predict_Y(Emitter_On_Background_Path, Emitter_Off_Background_Path, Raw_Data_Path, PLS_Model_Path);
                    myList = ((object[])result).ToList<object>();
                    pythonConcentrationArrayString = $"{myList[0].ToString()} {myList[1].ToString()} {myList[2].ToString()} {myList[3].ToString()}";
                }
                PythonEngine.EndAllowThreads(Concentration_threadState);
                Thread.Sleep(2);
                return pythonConcentrationArrayString;
                // return null;


            }
            catch (Exception e)
            {

                PythonEngine.EndAllowThreads(Concentration_threadState);
                return null;
            }


        }



    }
}
