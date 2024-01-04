using PaeoniaTechSpectroMeter.Database;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PaeoniaTechSpectroMeter.Model;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;
using CsvHelper;
using System.Globalization;
using System.Data;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Properties;
using iText.Layout.Element;
using Paragraph = iText.Layout.Element.Paragraph;
using System.Collections.ObjectModel;
using iText.Kernel.Colors;
using iText.IO.Image;
using Image = iText.Layout.Element.Image;
using TextAlignment = iText.Layout.Properties.TextAlignment;
using Table = iText.Layout.Element.Table;
using static PaeoniaTechSpectroMeter.Model.History;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace PaeoniaTechSpectroMeter.Views
{
    /// <summary>
    /// Interaction logic for CtrlHistory.xaml
    /// </summary>
    public partial class CtrlHistory : UserControl
    {
        public bool IsSelected { get; set; }
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["FuelAnalyser"].ConnectionString;
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalRecords;
        private int totalPages;
        private DataAccess dataAccess;
        private DataTable dataTable;
        private List<DataItem> currentPageItems;
        private List<DataItem> allDataItems;
        private ObservableCollection<DataItem> _dataItems;
        private Dictionary<int, List<DataItem>> selectedItemsPerPage = new Dictionary<int, List<DataItem>>();

        public static BrosweLocationViewModel brosweLocationViewModel;


        bool exportPDFEnable = false;

        History history;

        //public ObservableCollection<DataItem> DataItems { get; set; } = new ObservableCollection<DataItem>();
        public ObservableCollection<DataItem> DataItems
        {
            get { return _dataItems; }
            set
            {
                _dataItems = value;
                OnPropertyChanged(nameof(DataItems));
            }
        }

        private int _selectedButtonIndex;

        public int SelectedButtonIndex
        {
            get { return _selectedButtonIndex; }
            set
            {
                if (_selectedButtonIndex != value)
                {
                    _selectedButtonIndex = value;
                    OnPropertyChanged(nameof(SelectedButtonIndex));
                }
            }
        }

        public bool ExportPDFEnable
        {
            get => exportPDFEnable;
            set
            {
                exportPDFEnable = value;
                OnPropertyChanged("ExportPDFEnable");
            }
        }




        MainManager mmgr = null;
        public CtrlHistory(MainManager mmgr)
        {
            this.mmgr = mmgr;
            InitializeComponent();
            // connectionString=mmgr.AppConfig.ConnectionString;
            //   connectionString = $"Data Source={connectionString};Initial Catalog=fuelanalyser;Integrated Security=True";

            //DataContext = new History(mmgr);
            //DataContext = new History(mmgr);
            this.history = new History(mmgr);
            this.DataContext = history;

            LoadData();
            brosweLocationViewModel = new BrosweLocationViewModel();

        }



        private void LoadData()
        {
            try
            {
                string listOfMeasurements = $"SELECT * FROM Measurement ORDER BY [Time Stamp] desc OFFSET {(currentPage - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";

                dataAccess = new DataAccess(connectionString);
                dataTable = dataAccess.GetData(listOfMeasurements);
                List<DataItem> dataItemList = new List<DataItem>();


                foreach (DataRow row in dataTable.Rows)
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
                    //DataItems.Add(item);
                }

                //history_dataGrid.ItemsSource = dataItemList;

                DataItems = new ObservableCollection<DataItem>(dataItemList);
                history_dataGrid.ItemsSource = DataItems;

                // Update PageNumbers collection
                totalRecords = GetTotalRecords(); // Implement GetTotalRecords() method
                totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                if (selectedItemsPerPage.ContainsKey(currentPage))
                {
                    // Set the selected items in the DataGrid
                    foreach (var selectedItem in selectedItemsPerPage[currentPage])
                    {
                        history_dataGrid.SelectedItems.Add(selectedItem);
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            selectedItemsPerPage[currentPage] = history_dataGrid.SelectedItems.Cast<DataItem>().ToList();
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadData();
            }
        }

        private void btnPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            selectedItemsPerPage[currentPage] = history_dataGrid.SelectedItems.Cast<DataItem>().ToList();
            if (currentPage > 1)
            {
                currentPage--;
                LoadData();
                //UpdateDataGrid();
            }
        }

        public int CurrentPage
        {
            get { return currentPage; }
            set
            {
                if (currentPage != value)
                {
                    currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void PageButton_Click(int pageNumber)
        {
            CurrentPage = pageNumber;
            LoadData();
        }

        private void Chk_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            DataItem dataItem = (DataItem)checkBox.DataContext;
            dataItem.IsSelected = true;
            UpateExportButtonVisibility();
        }

        private void Chk_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            DataItem dataItem = (DataItem)checkBox.DataContext;
            dataItem.IsSelected = false;
            UpateExportButtonVisibility();
        }

        private void UpateExportButtonVisibility()
        {
            bool isCheckBoxChecked = history_dataGrid.Items.OfType<DataItem>().Any(item => item.IsSelected);
            //ExportAsPDFBtn.Visibility = isCheckBoxChecked ? Visibility.Visible : Visibility.Collapsed;
            ExportAsPDFBtn.IsEnabled = isCheckBoxChecked ? ExportPDFEnable = true : ExportPDFEnable = false;
            //ExportPDFEnable = isCheckBoxChecked;
        }

        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);

                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }

            return null;
        }
        private void ExportTableBtn_Click(object sender, RoutedEventArgs e)
        {
            string userChooseDirFromMainWindow = brosweLocationViewModel.UserChooseDir;
            if (!string.IsNullOrEmpty(userChooseDirFromMainWindow))
            {
                SaveCSVFile(userChooseDirFromMainWindow);
            }

            else
            {
                string serr = "";
                serr = history.BrowseLocation();
                SaveCSVFile(serr);
            }

        }

        private void SaveCSVFile(string Dir)
        {
            List<DataItem> allDataItems = GetAllDataItems();

            if (allDataItems.Count > 0)
            {
                string folderPath = Dir;

                string filePath = Path.Combine(folderPath, $"{DateTime.Now:yyyyMMdd_HHmmss}_AllCSVDataReport.csv");
                try
                {
                    using (var writer = new StreamWriter(filePath))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.Context.RegisterClassMap<DataItemMap>();
                        csv.WriteRecords(allDataItems);
                    }

                    //Dispatcher.Invoke(() =>
                    //{
                    //    history.HistoryInfoIconSource = @"C:\FuelAnalyzer\bin\Icon\Info-GreenSign_Icon.png";
                    //    history.ExportMessageCompleted = $"File Saved Successfully At {DateTime.Now.ToString("HH:mm:ss")}";

                    //});

                    HistoryInfoIcon.Source = new BitmapImage(new Uri(@"C:\FuelAnalyzer\bin\Icon\Info-GreenSign_Icon.png"));
                    ExportMessage.Text = $"CSV File Saved Successfully At {DateTime.Now.ToString("HH:mm:ss")}";
                    ExportMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0f7b0f"));
                    ExportBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0f7b0f"));

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting CSV file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private List<DataItem> GetAllDataItems()
        {
            string listOfMeasurements = "select * from Measurement ORDER BY [Time Stamp] desc";

            dataAccess = new DataAccess(connectionString);
            dataTable = dataAccess.GetData(listOfMeasurements);
            List<DataItem> allItems = new List<DataItem>();

            foreach (DataRow row in dataTable.Rows)
            {
                DataItem dataItem = new DataItem();

                dataItem.Timestamp = row["Time Stamp"] is DateTime timestamp ? timestamp : default;
                dataItem.Name = row["Name"]?.ToString();
                dataItem.PassNo = row["Pass No."]?.ToString();
                dataItem.Operator = row["Operator"]?.ToString();
                dataItem.AnalysisType = row["Analysis Type"]?.ToString();
                dataItem.SampleType = row["Sample Type"]?.ToString();
                dataItem.Ethanol = row["Ethanol"] is double ethanol ? (double?)ethanol : null;
                dataItem.Denaturant = row["Denaturant"] is double denaturant ? (double?)denaturant : null;
                dataItem.Methanol = row["Methanol"] is double methanol ? (double?)methanol : null;
                dataItem.Water = row["Water"] is double water ? (double?)water : null;
                dataItem.Batch = row["Batch"] is int batch ? (int?)batch : null;

                allItems.Add(dataItem);
            }

            return allItems;
        }

        private void ExportAsPDFBtn_Click(object sender, RoutedEventArgs e)
        {
            string userChooseDirFromMainWindow = brosweLocationViewModel.UserChooseDir;
            if (!string.IsNullOrEmpty(userChooseDirFromMainWindow))
            {
                SavePDFFile(userChooseDirFromMainWindow);
            }

            else
            {
                string serr = "";
                serr = history.BrowseLocation();
                SavePDFFile(serr);
            }
            
        }

        private void SavePDFFile(string Dir)
        {
            List<DataItem> selectedItems = GetSelectedItems();
            if (selectedItems.Count > 0)
            {
                string folderPath = Dir;

                string filePath = $"{DateTime.Now:yyyyMMdd_HHmmss}_PDFReport.pdf";

                using (var writer = new PdfWriter(Path.Combine(folderPath, filePath)))
                using (var pdf = new PdfDocument(writer))
                {
                    CustomPdfPageEvent pageEvent = new CustomPdfPageEvent();
                    //pageEvent.SetTotalPages(totalPages);
                    pdf.AddEventHandler(iText.Kernel.Events.PdfDocumentEvent.END_PAGE, pageEvent);

                    using (var document = new Document(pdf))
                    {
                        int totalPages = selectedItems.Count;
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

                        //History history = new History(mmgr);
                        Table additionalInfoTable = history.CreateAdditionalInfoTable(Dir);// Add iTextSharp table with additional information
                        document.Add(additionalInfoTable);

                        document.Add(new Paragraph("\n"));
                        Paragraph equipmentInfo = new Paragraph("EQUIPMENT INFORMATION").SetFontColor(ColorConstants.BLACK)
                            .SetFontSize(16)
                            .SetBold();
                        document.Add(equipmentInfo);
                        document.Add(new Paragraph("\n"));

                        Table equipmentInfoTable = history.CreateEquipmentInfoTable();// Add iTextSharp table with additional information
                        document.Add(equipmentInfoTable);



                        // Add a new page
                        //document.Add(new AreaBreak());




                        selectedItems.Sort((x, y) => x.Batch.GetValueOrDefault() - y.Batch.GetValueOrDefault());

                        // Initialize batch number for the first item
                        int currentBatchNumber = selectedItems[0].Batch.GetValueOrDefault();
                        int pageNumber = 1;

                        foreach (var itemGroup in selectedItems.GroupBy(item => item.Batch.GetValueOrDefault()))
                        {
                            // Start a new page for a new batch
                            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

                            Paragraph sampleReport = new Paragraph($"SAMPLE MEASUREMENT REPORT {pageNumber}").SetFontColor(ColorConstants.WHITE).SetFontSize(16).SetBold().SetBackgroundColor(ColorConstants.BLUE);
                            document.Add(sampleReport);
                            document.Add(new Paragraph("\n"));



                            var operatorNames = itemGroup.Select(item => item.Operator).Distinct();
                            string concatenatedOperatorNames = string.Join(", ", operatorNames);

                            // Add summary information for the batch
                            Table summarySelectedItemsTable = history.CreateSummarySelectedItemsTable(itemGroup.First(), itemGroup.Count(), concatenatedOperatorNames);
                            document.Add(summarySelectedItemsTable);

                            document.Add(new Paragraph("\n"));
                            Paragraph avgResultInfo = new Paragraph("AVERAGE RESULT").SetFontColor(ColorConstants.BLACK)
                                .SetFontSize(16)
                                .SetBold();
                            document.Add(avgResultInfo);
                            document.Add(new Paragraph("\n"));

                            // Calculate average values for the batch
                            //double avgEthanol = itemGroup.Average(item => item.Ethanol ?? 0);
                            //double avgDenaturant = itemGroup.Average(item => item.Denaturant ?? 0);
                            //double avgMethanol = itemGroup.Average(item => item.Methanol ?? 0);
                            //double avgWater = itemGroup.Average(item => item.Water ?? 0);

                            double? avgEthanol = itemGroup.Select(item => item.Ethanol).Where(value => value.HasValue).Select(value => value.Value).DefaultIfEmpty().Average();

                            double? avgDenaturant = itemGroup.Select(item => item.Denaturant).Where(value => value.HasValue).Select(value => value.Value).DefaultIfEmpty().Average();

                            double? avgMethanol = itemGroup.Select(item => item.Methanol).Where(value => value.HasValue).Select(value => value.Value).DefaultIfEmpty().Average();

                            double? avgWater = itemGroup.Select(item => item.Water).Where(value => value.HasValue).Select(value => value.Value).DefaultIfEmpty().Average();



                            // Create a single row table for average results
                            Table avgResultItemsTable = history.CreateAverageResultItemsTable(
                                new DataItem
                                {
                                    Ethanol = (double)avgEthanol,
                                    Denaturant = (double)avgDenaturant,
                                    Methanol = (double)avgMethanol,
                                    Water = (double)avgWater
                                }
                            );
                            document.Add(avgResultItemsTable);

                            document.Add(new Paragraph("\n"));
                            Paragraph passesResultInfo = new Paragraph("PASSES RESULTS").SetFontColor(ColorConstants.BLACK)
                                .SetFontSize(16)
                                .SetBold();
                            document.Add(passesResultInfo);
                            document.Add(new Paragraph("\n"));

                            // Create a table for all items in the batch
                            Table selectedItemsTable = new Table(UnitValue.CreatePercentArray(6)).UseAllAvailableWidth();
                            selectedItemsTable.AddCell(new Cell().Add(new Paragraph("Pass No.")).SetTextAlignment(TextAlignment.CENTER));
                            selectedItemsTable.AddCell(new Cell().Add(new Paragraph("Time Stamp")).SetTextAlignment(TextAlignment.CENTER));
                            selectedItemsTable.AddCell(new Cell().Add(new Paragraph("Ethanol (Vol%)")).SetTextAlignment(TextAlignment.CENTER));
                            selectedItemsTable.AddCell(new Cell().Add(new Paragraph("Denaturant (Vol%)")).SetTextAlignment(TextAlignment.CENTER));
                            selectedItemsTable.AddCell(new Cell().Add(new Paragraph("Methanol (Vol%)")).SetTextAlignment(TextAlignment.CENTER));
                            selectedItemsTable.AddCell(new Cell().Add(new Paragraph("Water (Vol%)")).SetTextAlignment(TextAlignment.CENTER));

                            foreach (var item in itemGroup)
                            {
                                selectedItemsTable.AddCell(new Cell().Add(new Paragraph(item.PassNo.ToString())));
                                selectedItemsTable.AddCell(new Cell().Add(new Paragraph(item.Timestamp.ToString())));
                                selectedItemsTable.AddCell(new Cell().Add(new Paragraph(item.Ethanol != null ? item.Ethanol.ToString() : "-")));
                                selectedItemsTable.AddCell(new Cell().Add(new Paragraph(item.Denaturant != null ? item.Denaturant.ToString() : "-")));
                                selectedItemsTable.AddCell(new Cell().Add(new Paragraph(item.Methanol != null ? item.Methanol.ToString() : "-")));
                                selectedItemsTable.AddCell(new Cell().Add(new Paragraph(item.Water != null ? item.Water.ToString() : "-")));
                            }



                            document.Add(selectedItemsTable);
                            pageNumber++;
                            //MessageBox.Show($"hello{ itemGroup.Count()}");
                        }
                        //pageEvent.SetTotalPages(pdf.GetNumberOfPages());
                        pageEvent.SetTotalPages(totalPages);
                        //pdf.AddEventHandler(iText.Kernel.Events.PdfDocumentEvent.END_PAGE, pageEvent);
                        //pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new History.FooterEventHandler()); // Add a footer to each page
                    }


                    HistoryInfoIcon.Source = new BitmapImage(new Uri(@"C:\FuelAnalyzer\bin\Icon\Info-GreenSign_Icon.png"));
                    ExportMessage.Text = $"PDF File Saved Successfully At {DateTime.Now.ToString("HH:mm:ss")}";
                    ExportMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0f7b0f"));
                    ExportBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0f7b0f"));
                }

            }
            else
            {
                MessageBox.Show("No items selected for export.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private List<DataItem> GetSelectedItems()
        {
            return history_dataGrid.Items.OfType<DataItem>().Where(item => item.IsSelected).ToList();
        }

        private void UpdateDataGrid()
        {
            // Assuming you have a DataGrid named history_dataGrid
            history_dataGrid.ItemsSource = dataTable.DefaultView;

            // Update pagination information
            UpdatePaginationInfo();
        }

        private void UpdatePaginationInfo()
        {
            // Assuming you have TextBox controls for displaying page information
            //pageInfoTextBox.Text = $"Page {currentPage}";
        }
        private void UpdatePageButtom(int totalPages)
        {
            // You may update UI elements here based on the total pages
            // For example, update a label displaying the current page and total pages
            //pageLabel.Content = $"Page {currentPage} of {totalPages}";

            // Enable or disable Next/Previous buttons based on the current page
            NextBtn.IsEnabled = currentPage < totalPages;
            PreviousBtn.IsEnabled = currentPage > 1;
        }

        private int GetTotalRecords()
        {
            try
            {
                string countQuery = "SELECT COUNT(*) FROM Measurement";

                //DataAccess dataAccess = new DataAccess(connectionString);
                int totalCount = dataAccess.ExecuteScalar<int>(countQuery);

                return totalCount;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while getting total records: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0; // Default to 0 if an error occurs
            }
        }

        private int GetTotalPages()
        {
            return (int)Math.Ceiling((double)totalRecords / pageSize);
        }

        private void PageNumbersBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                CtrlHistory vm = DataContext as CtrlHistory;

                if (vm != null)
                {
                    vm.SelectedButtonIndex = Convert.ToInt32(button.Content);
                }
            }
        }

        private void BtnChangeLocationForBothCSV_PDF_Click(object sender, RoutedEventArgs e)
        {
            string serr = "";
            serr = history.BrowseLocation();
        }
    }
}
