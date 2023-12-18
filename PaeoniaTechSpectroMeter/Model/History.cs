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


namespace PaeoniaTechSpectroMeter.Model
{
    public class History
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public History()
        {
            //connectionstrin=
           // _connectionString=
            Initialize();
            PageButtonClickCommand = new RelayCommand<int>(PageButton_Click);
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
                string listOfMeasurements = $"SELECT * FROM Measurement ORDER BY [Time Stamp] OFFSET {(_currentPage - 1) * _pageSize} ROWS FETCH NEXT {_pageSize} ROWS ONLY";
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

        public Table CreateAdditionalInfoTable()
        {
            Table infoTable = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();

            var info = new Dictionary<string, string>
            {
                { "Company", "XYZ Lab" },
                { "Operator", "Ahmad" },
                { "Instrument Model", "Waukesha Instrument Model E" },
                { "Instrument Serial Number", "ABC1234K" },
                { "Instrument Firmware version", "V1.0.01" },
                { "Fuel Analyzer Software Application Version", "V1.0.5" },
                { "Report Date and Time", DateTime.Now.ToString() },
                { "Report Path", @"C:\Users\Public\Documents\" }
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
                { "Last Performance Test Result", "PASSED" },
                { "Last Performance Test Datae and Time", "YYYY-MM-DD, HH:MM:SS (AM) (GMT+8)" },
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

                // Add your footer content
                canvas.BeginText()
                    .SetFontAndSize(PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA), 8)
                    .MoveText(36, 20)
                    .ShowText("Page Signature : ______________________")
                    .MoveText(pageSize.GetWidth() - 100, 20)
                    .ShowText($"Page {pdfDoc.GetPageNumber(page)} of {totalPages}")
                    .EndText();
            }

            public void SetTotalPages(int totalPages)
            {
                this.totalPages = totalPages;
            }
        }
    }
}
