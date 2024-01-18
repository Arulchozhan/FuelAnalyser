using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using PaeoniaTechSpectroMeter.Database;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using PaeoniaTechSpectroMeter.Model;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Data;
using iText.Layout;
using iText.Kernel.Exceptions;
using Paragraph = iText.Layout.Element.Paragraph;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using iText.Kernel.Colors;
using iText.IO.Image;
using Image = iText.Layout.Element.Image;
using TextAlignment = iText.Layout.Properties.TextAlignment;
using Table = iText.Layout.Element.Table;
using iText.Kernel.Events;
using static PaeoniaTechSpectroMeter.Model.History;
using PaeoniaTechSpectroMeter.Views;
using Path = System.IO.Path;

namespace PaeoniaTechSpectroMeter.Model
{
    public class History : INotifyPropertyChanged
    {
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalRecords;
        private int _totalPages;
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["FuelAnalyser"].ConnectionString;
        // private readonly string _connectionString = //ConfigurationManager.ConnectionStrings["FuelAnalyser"].ConnectionString;
        private DataAccess _dataAccess;
        private DataTable _dataTable;
        private DataView _dataView;
        MainManager mmgr;

        private string historyInfoIconSource;
        private string exportMessageCompleted;
        private bool historyMessageText;
        private DataAccess dataAccess;
        private DataTable dataTable;

        private BrowseLocationDialog browseLocationDialogInstance;


        private ObservableCollection<DataItem> _dataItems = new ObservableCollection<DataItem>();
        public ObservableCollection<DataItem> DataItems
        {
            get { return _dataItems; }
            set
            {
                _dataItems = value;
                OnPropertyChanged(nameof(DataItems));
            }
        }



        public DataView DataView
        {
            get { return _dataView; }
            set
            {
                _dataView = value;
                OnPropertyChanged(nameof(DataView));
                //OnPropertyChanged(nameof(HistoryDataGridItemsSource));
            }
        }
        public RelayCommand<int> PageButtonClickCommand { get; private set; }

        public ObservableCollection<int> PageNumbers { get; set; } = new ObservableCollection<int>();

        public object DataContext { get; private set; }

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                }
            }
        }

        public string HistoryInfoIconSource
        {
            get { return historyInfoIconSource; }
            set
            {
                if (historyInfoIconSource != value)
                {
                    historyInfoIconSource = value;
                    OnPropertyChanged(nameof(HistoryInfoIconSource));
                }
            }
        }

        public string ExportMessageCompleted
        {
            get { return exportMessageCompleted; }
            set
            {
                if (exportMessageCompleted != value)
                {
                    exportMessageCompleted = value;
                    OnPropertyChanged(nameof(ExportMessageCompleted));
                }
            }
        }

        public bool HistoryMessageText
        {
            get => historyMessageText;
            set
            {
                historyMessageText = value;
                OnPropertyChanged(nameof(HistoryMessageText));
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public History(MainManager mmgr)
        {
            //connectionstrin=
            // _connectionString=
            Initialize();
            PageButtonClickCommand = new RelayCommand<int>(PageButton_Click);
            this.mmgr = mmgr;

            HistoryInfoIconSource = @"C:\FuelAnalyzer\bin\Icon\Info_Icon.png";
            ExportMessageCompleted = $"The button for exporting to PDF will be accessible when the checkbox is checked.";
            HistoryMessageText = true;
        }

        private void Initialize()
        {

            try
            {
                _dataAccess = new DataAccess(_connectionString);
                DataItems = new ObservableCollection<DataItem>();
                //LoadData();
                _totalRecords = GetTotalRecords();
                _totalPages = (int)Math.Ceiling((double)_totalRecords / _pageSize);
                PageNumbers.Clear();
                for (int i = 1; i <= _totalPages; i++)
                {
                    PageNumbers.Add(i);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during initialization: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadData()
        {
            try
            {
                string listOfMeasurements = $"SELECT * FROM Measurement ORDER BY [Time Stamp] desc OFFSET {(_currentPage - 1) * _pageSize} ROWS FETCH NEXT {_pageSize} ROWS ONLY";
                _dataTable = _dataAccess.GetData(listOfMeasurements);
                ObservableCollection<DataItem> dataItemList = new ObservableCollection<DataItem>();


                foreach (DataRow row in _dataTable.Rows)
                {
                    DataItem item = new DataItem
                    {
                        Timestamp = row["Time Stamp"] is DateTime timestamp ? timestamp : default,
                        Name = row["Name"].ToString(),
                        PassNo = row["Pass No."].ToString(),
                        Operator = row["Operator"].ToString(),
                        AnalysisType = row["Analysis Type"].ToString(),
                        SampleType = row["Sample Type"].ToString(),
                        Ethanol = row["Ethanol"] is double ethanol ? (double?)ethanol : null,
                        Denaturant = row["Denaturant"] is double denaturant ? (double?)denaturant : null,
                        Methanol = row["Methanol"] is double methanol ? (double?)methanol : null,
                        Water = row["Water"] is double water ? (double?)water : null,
                        Batch = row["Batch"] is int batch ? (int?)batch : null,
                    };

                    dataItemList.Add(item);
                }
                //DataItems = dataItemList;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    DataItems.Clear();
                    foreach (var item in dataItemList)
                    {
                        DataItems.Add(item);
                    }
                });
                OnPropertyChanged(nameof(DataItems));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private int GetTotalRecords()
        {
            try
            {
                string countQuery = "SELECT COUNT(*) FROM Measurement";
                int totalCount = _dataAccess.ExecuteScalar<int>(countQuery);

                return totalCount;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while getting total records: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }
        private void PageButton_Click(int pageNumber)
        {
            CurrentPage = pageNumber;
            LoadData();
        }

        public Table CreateAdditionalInfoTable(string UserChooseDir)
        {
            Table infoTable = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();

            var info = new Dictionary<string, string>
            {
                { "Company", "XYZ Lab" },
                { "Instrument Model", "Waukesha Instrument Model E" },
                { "Instrument Serial Number", mmgr.SelfDiagnostics.productSerialNo },
                { "Instrument Firmware version", mmgr.SelfDiagnostics.firmwareversion },
                { "Fuel Analyzer Software Application Version", mmgr.SelfDiagnostics.softwareversion },
                { "Report Date and Time", DateTime.Now.ToString() },
                { "Report Path", UserChooseDir }
            };

            foreach (var kvp in info)
            {
                infoTable.AddCell(new Cell().Add(new Paragraph(kvp.Key)));
                infoTable.AddCell(new Cell().Add(new Paragraph(kvp.Value)));

            }

            return infoTable;
        }

        public Table CreateEquipmentInfoTable()
        {
            Table infoTable = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();

            var info = new Dictionary<string, string>
            {
                { "Last Performance Test Result", mmgr.AppConfig.Perfchk },
                { "Last Performance Test Date and Time", mmgr.AppConfig.PerfchkTime.ToString() },
                { "Calibration Module 1 Name and Version", "Fuel Ethanol V1.0.01" },
                { "Calibration Module 1 Installation Date", "YYYY-MM-DD" },
                { "Calibration Module 2 Name and Version", "Fuel Methanol V1.0.01 " },
                { "Calibration Module 2 Installation Data", "YYYY-MM-DD" },
                { "Instrument Installation Date", "YYYY-MM-DD" },
                { "Instrument Service Contract End Date", "YYYY-MM-DD (+3 year)" }
            };


            foreach (var kvp in info)
            {
                infoTable.AddCell(new Cell().Add(new Paragraph(kvp.Key)));
                infoTable.AddCell(new Cell().Add(new Paragraph(kvp.Value)));
            }

            return infoTable;
        }

        public Table CreateItemsTable(DataItem item)
        {
            Table table = new Table(UnitValue.CreatePercentArray(6)).UseAllAvailableWidth();

            table.AddCell(new Cell().Add(new Paragraph("Pass No.")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph("Time Stamp")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph("Ethanol (Vol%)")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph("Denaturant (Vol%)")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph("Methanol (Vol%)")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph("Water (Vol%)")).SetTextAlignment(TextAlignment.CENTER));

            // Add PassNo and Timestamp cells for the single item
            table.AddCell(new Cell().Add(new Paragraph(item.PassNo.ToString())));
            table.AddCell(new Cell().Add(new Paragraph(item.Timestamp.ToString())));

            table.AddCell(new Cell().Add(new Paragraph(item.Ethanol != null ? item.Ethanol.ToString() : "-")));

            table.AddCell(new Cell().Add(new Paragraph(item.Denaturant != null ? item.Denaturant.ToString() : "-")));

            table.AddCell(new Cell().Add(new Paragraph(item.Methanol != null ? item.Methanol.ToString() : "-")));

            table.AddCell(new Cell().Add(new Paragraph(item.Water != null ? item.Water.ToString() : "-")));

            return table;
        }

        public Table CreateSummarySelectedItemsTable(DataItem item, int itemCount, string OperatorNames)
        {
            Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();

            var row = new Cell[2];

            row[0] = new Cell().Add(new Paragraph("Measurement Date")).SetTextAlignment(TextAlignment.LEFT);
            row[1] = new Cell().Add(new Paragraph(item.Timestamp.ToString())).SetTextAlignment(TextAlignment.LEFT);
            table.AddCell(row[0]);
            table.AddCell(row[1]);

            row[0] = new Cell().Add(new Paragraph("Operator Name")).SetTextAlignment(TextAlignment.LEFT);

            //var operatorNames = item.Operator;
            row[1] = new Cell().Add(new Paragraph(OperatorNames)).SetTextAlignment(TextAlignment.LEFT);
            table.AddCell(row[0]);
            table.AddCell(row[1]);

            row[0] = new Cell().Add(new Paragraph("Analysis Type (or Method)")).SetTextAlignment(TextAlignment.LEFT);
            row[1] = new Cell().Add(new Paragraph(item.AnalysisType)).SetTextAlignment(TextAlignment.LEFT);
            table.AddCell(row[0]);
            table.AddCell(row[1]);

            row[0] = new Cell().Add(new Paragraph("Sample Name")).SetTextAlignment(TextAlignment.LEFT);
            row[1] = new Cell().Add(new Paragraph(item.Name)).SetTextAlignment(TextAlignment.LEFT);
            table.AddCell(row[0]);
            table.AddCell(row[1]);

            row[0] = new Cell().Add(new Paragraph("Sample Type")).SetTextAlignment(TextAlignment.LEFT);
            row[1] = new Cell().Add(new Paragraph(item.SampleType)).SetTextAlignment(TextAlignment.LEFT);
            table.AddCell(row[0]);
            table.AddCell(row[1]);

            row[0] = new Cell().Add(new Paragraph("Total Pass No.")).SetTextAlignment(TextAlignment.LEFT);
            row[1] = new Cell().Add(new Paragraph(itemCount.ToString())).SetTextAlignment(TextAlignment.LEFT); // Assuming each item is considered one pass
            table.AddCell(row[0]);
            table.AddCell(row[1]);

            return table;
        }

        public Table CreateMeasurementReportTable()
        {
            Table infoTable = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            string analysisType;
            string sampleType;
            if (mmgr.ReadDetector.SelectedAnalysistype == 0)
            {
                analysisType = "Methanol";
                sampleType = "Fuel Methanol";
            }
            else
            {
                analysisType = "Ethanol";
                sampleType = "Fuel Ethanol";
            }

            var info = new Dictionary<string, string>
                {
                    { "Measurement Date", DateTime.Now.ToString() },
                    { "Operator Name", mmgr.ReadDetector.OpearatorName},
                    { "Analysis Type (or Method)", analysisType },
                    { "Sample Name", mmgr.ReadDetector.SampleFileName },
                    { "Sample Type", sampleType},
                    { "Pass No", mmgr.ReadDetector.PassNo.ToString("D3") },
                };
            foreach (var kvp in info)
            {
                infoTable.AddCell(new Cell().Add(new Paragraph(kvp.Key)));
                infoTable.AddCell(new Cell().Add(new Paragraph(kvp.Value)));
            }
            return infoTable;
        }

        public Table PassResultTable()
        {
            Table table = new Table(UnitValue.CreatePercentArray(6)).UseAllAvailableWidth();

            table.AddCell(new Cell().Add(new Paragraph("Pass No.")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph("Time Stamp")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph("Ethanol (Vol%)")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph("Denaturant (Vol%)")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph("Methanol (Vol%)")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph("Water (Vol%)")).SetTextAlignment(TextAlignment.CENTER));

            table.AddCell(new Cell().Add(new Paragraph(mmgr.ReadDetector.PassNo.ToString("D3"))));
            table.AddCell(new Cell().Add(new Paragraph(DateTime.Now.ToString())));

            if (mmgr.ReadDetector.SelectedAnalysistype == 0)
            {
                table.AddCell(new Cell().Add(new Paragraph("-")));
                table.AddCell(new Cell().Add(new Paragraph(mmgr.ReadDetector.MethanolConcentration.ToString() != null ? mmgr.ReadDetector.MethanolConcentration.ToString() : "-")));
                table.AddCell(new Cell().Add(new Paragraph("-")));
                table.AddCell(new Cell().Add(new Paragraph("-")));
            }
            else
            {
                table.AddCell(new Cell().Add(new Paragraph(mmgr.ReadDetector.EthanolConcentration.ToString() != null ? mmgr.ReadDetector.EthanolConcentration.ToString() : "-")));
                table.AddCell(new Cell().Add(new Paragraph(mmgr.ReadDetector.DenaturantConcentration.ToString() != null ? mmgr.ReadDetector.DenaturantConcentration.ToString() : "-")));
                table.AddCell(new Cell().Add(new Paragraph(mmgr.ReadDetector.MethanolConcentration.ToString() != null ? mmgr.ReadDetector.MethanolConcentration.ToString() : "-")));
                table.AddCell(new Cell().Add(new Paragraph(mmgr.ReadDetector.WaterConcentration.ToString() != null ? mmgr.ReadDetector.WaterConcentration.ToString() : "-")));
            }

            return table;
        }

        public Table CreateAverageResultItemsTable(DataItem item)
        {
            Table table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth();

            table.AddCell(new Cell().Add(new Paragraph("Ethanol (0-100) Vol%")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph("Denaturant (0-75) Vol%")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph("Methanol (0-15) Vol%")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph("Water (0-2) Vol%")).SetTextAlignment(TextAlignment.CENTER));

            double totalEthanolValue = item.Ethanol ?? 0;
            double totalDenaturantValue = item.Denaturant ?? 0;
            double totalMethanolValue = item.Methanol ?? 0;
            double totalWaterValue = item.Water ?? 0;

            double avgEthanolValue = (double)totalEthanolValue;
            double avgDenaturantValue = (double)totalDenaturantValue;
            double avgMethanolValue = (double)totalMethanolValue;
            double avgWaterValue = (double)totalWaterValue;


            table.AddCell(new Cell().Add(new Paragraph(avgEthanolValue != 0 ? avgEthanolValue.ToString("0.00") : "-")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph(avgDenaturantValue != 0 ? avgDenaturantValue.ToString("0.00") : "-")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph(avgMethanolValue != 0 ? avgMethanolValue.ToString("0.00") : "-")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph(avgWaterValue != 0 ? avgWaterValue.ToString("0.00") : "-")).SetTextAlignment(TextAlignment.CENTER));

            return table;
        }

        public class FooterEventHandler : iText.Kernel.Events.IEventHandler
        {

            public void HandleEvent(iText.Kernel.Events.Event @event)
            {
                var docEvent = (iText.Kernel.Events.PdfDocumentEvent)@event;
                var pdf = docEvent.GetDocument();
                var currentPageNumber = pdf.GetPageNumber(docEvent.GetPage());

                //totalPages = Math.Max(totalPages, currentPageNumber);

                var canvas = new PdfCanvas(docEvent.GetPage());// Create a canvas to add content to the page

                var font = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA); // Set the font for the footer
                canvas.SetFontAndSize(font, 10);

                canvas.BeginText()
                    .MoveText(36, 20)
                    .ShowText("Page Signature : ______________________")
                    .EndText();

                // Add "Page X of Y"
                canvas.BeginText()
                    .MoveText(docEvent.GetPage().GetPageSize().GetWidth() - 100, 20)
                    .ShowText($"Page {currentPageNumber} of {pdf.GetNumberOfPages()}")
                    .EndText();
            }
        }

        public class CustomPdfPageEvent : iText.Kernel.Events.IEventHandler
        {
            private int totalPages = 0;

            public void HandleEvent(iText.Kernel.Events.Event @event)
            {
                var docEvent = (iText.Kernel.Events.PdfDocumentEvent)@event;
                var pdfDoc = docEvent.GetDocument();
                var page = docEvent.GetPage();

                var canvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);
                var pageSize = page.GetPageSize();

                var font = PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA); // Set the font for the footer
                canvas.SetFontAndSize(font, 10);

                // Add your footer content
                //canvas.BeginText()
                //    .SetFontAndSize(PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA), 8)
                //    .MoveText(36, 20)
                //    .ShowText("Page Signature : ______________________")
                //    .MoveText(pageSize.GetWidth() - 100, 20)
                //    .ShowText($"Page {pdfDoc.GetPageNumber(page)}")
                //    .EndText();
                canvas.BeginText()
                   .MoveText(36, 20)
                   .ShowText("Page Signature : ______________________")
                   .EndText();

                // Add "Page X of Y"
                canvas.BeginText()
                    .MoveText(pageSize.GetWidth() - 100, 20)
                    .ShowText($"Page {pdfDoc.GetPageNumber(page)}")
                    .EndText();
            }

            public void SetTotalPages(int totalPages)
            {
                this.totalPages = totalPages;
            }
        }

        public string BrowseLocation()
        {
            string selectedDir = "";
            if (browseLocationDialogInstance == null)
            {
                browseLocationDialogInstance = new BrowseLocationDialog() { brosweLocationViewModel = CtrlHistory.brosweLocationViewModel };
                browseLocationDialogInstance.Closed += (s, args) =>
                {
                    // Check if the user made a selection
                    if (browseLocationDialogInstance.DialogResult == true)
                    {
                        // User made a selection, get the selected directory
                        selectedDir = CtrlHistory.brosweLocationViewModel.UserChooseDir;
                    }

                    // Reset the dialog instance
                    browseLocationDialogInstance = null;
                };
                browseLocationDialogInstance.Topmost = true;
                //browseLocationDialogInstance.Show();
                // Show the dialog modally
                bool? dialogResult = browseLocationDialogInstance.ShowDialog();

                // Check the dialog result
                if (dialogResult == false)
                {
                    // User clicked OK, get the selected directory
                    selectedDir = CtrlHistory.brosweLocationViewModel.UserChooseDir;
                }

                // Reset the dialog instance
                browseLocationDialogInstance = null;
            }
            else
            {
                browseLocationDialogInstance.Focus();
            }

            //var dialog = new System.Windows.Forms.FolderBrowserDialog();

            //BrowseLocationDialog browseLocationDialog = new BrowseLocationDialog();
            //browseLocationDialog.Owner = Window.GetWindow(this);
            //browseLocationDialog.Topmost = true;
            //browseLocationDialog.Show();

            return selectedDir;
        }


       
    }
}
