using HistoryTest.Database;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using Microsoft.Win32;
using System.ComponentModel;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using HistoryTest.Model;
using System.Data;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Properties;
using iText.Layout.Element;
using iText.Kernel.Exceptions;
using Paragraph = iText.Layout.Element.Paragraph;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;

namespace PaeoniaTechSpectroMeter.Views
{
    /// <summary>
    /// Interaction logic for HistoryPage.xaml
    /// </summary>
    public partial class HistoryPage : UserControl
    {
        public bool IsSelected { get; set; }
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["HistoryPage"].ConnectionString;
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalRecords;
        private int totalPages;
        private DataAccess dataAccess;
        private DataTable dataTable;
        private List<DataItem> currentPageItems;
        private List<DataItem> allDataItems;
        private ObservableCollection<DataItem> _dataItems;

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


        public HistoryPage()
        {
            InitializeComponent();         
            LoadData();
            DataContext = new HistoryPageViewModel();
        }

     

        private void LoadData()
        {
            try
            {
                string listOfMeasurements = $"SELECT * FROM Measurement ORDER BY [Pass No.] OFFSET {(currentPage - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";

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
                        Ethanol = row["Ethanol"] is int ethanol ? (int?)ethanol : null,
                        Denaturant = row["Denaturant"] is int denaturant ? (int?)denaturant : null,
                        Methanol = row["Methanol"] is int methanol ? (int?)methanol : null,
                        Water = row["Water"] is int water ? (int?)water : null,

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
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                LoadData();
            }
        }

        private void btnPreviousPage_Click(object sender, RoutedEventArgs e)
        {
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
            ExportAsPDFBtn.Visibility = isCheckBoxChecked ? Visibility.Visible : Visibility.Collapsed;
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

        private List<DataItem> GetAllDataItems()
        {
            string listOfMeasurements = "select * from Measurement";

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
                dataItem.Ethanol = row["Ethanol"] is int ethanol ? (int?)ethanol : null;
                dataItem.Denaturant = row["Denaturant"] is int denaturant ? (int?)denaturant : null;
                dataItem.Methanol = row["Methanol"] is int methanol ? (int?)methanol : null;
                dataItem.Water = row["Water"] is int water ? (int?)water : null;

                allItems.Add(dataItem);
            }

            return allItems;
        }

        private void ExportAsPDFBtn_Click(object sender, RoutedEventArgs e)
        {
            List<DataItem> selectedItems = GetSelectedItems();

            if (selectedItems.Count > 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    Title = "Save PDF file"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var writer = new PdfWriter(System.IO.Path.Combine(saveFileDialog.FileName)))
                    using (var pdf = new iText.Kernel.Pdf.PdfDocument(writer))
                    {
                        using (var document = new Document(pdf))
                        {
                            iText.Layout.Element.Table table = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(10)).UseAllAvailableWidth();//Add a table to the document

                            foreach (var property in typeof(DataItem).GetProperties())// Add header row
                            {
                                if (property.Name != "IsSelected")
                                {
                                    table.AddHeaderCell(new Cell().Add(new Paragraph(property.Name)));
                                }
                            }

                            foreach (var item in selectedItems)//Add data rows
                            {
                                foreach (var property in typeof(DataItem).GetProperties())
                                {
                                    if (property.Name != "IsSelected")
                                    {
                                        object value = property.GetValue(item);
                                        string cellValue = value != null ? value.ToString() : "N/A";

                                        table.AddCell(new Cell().Add(new Paragraph(cellValue)));
                                    }
                                }
                            }
                            document.Add(table);
                        }
                        MessageBox.Show("PDF file exported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("No items selected for export.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        //private List<DataItem> GetSelectedItems()
        //{
        //    List<DataItem> selectedItems = new List<DataItem>();

        //    foreach (DataRow row in dataTable.Rows)
        //    {
        //        DataItem dataItem = new DataItem();

        //        dataItem.Timestamp = row["Time Stamp"] is DateTime timestamp ? timestamp : default;
        //        dataItem.Name = row["Name"]?.ToString();
        //        dataItem.PassNo = row["Pass No."]?.ToString();
        //        dataItem.Operator = row["Operator"]?.ToString();
        //        dataItem.AnalysisType = row["Analysis Type"]?.ToString();
        //        dataItem.SampleType = row["Sample Type"]?.ToString();
        //        dataItem.Ethanol = row["Ethanol"] is int ethanol ? (int?)ethanol : null;
        //        dataItem.Denaturant = row["Denaturant"] is int denaturant ? (int?)denaturant : null;
        //        dataItem.Methanol = row["Methanol"] is int methanol ? (int?)methanol : null;
        //        dataItem.Water = row["Water"] is int water ? (int?)water : null;

        //        selectedItems.Add(dataItem);
        //    }

        //    return selectedItems;
        //}

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

       
    }

}
