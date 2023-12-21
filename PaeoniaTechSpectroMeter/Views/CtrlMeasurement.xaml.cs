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
        public CtrlMeasurement(MainManager mmgr)
        {
            this.mmgr = mmgr;
            InitializeComponent();
            DispatcherTimer LiveTime = new DispatcherTimer();
          //  LiveTime.Interval = TimeSpan.FromSeconds(1);
          //  LiveTime.Tick += timer_Tick;
           // LiveTime.Start();
            this.DataContext = mmgr.ReadDetector;

        }


        void timer_Tick(object sender, EventArgs e)
        {
        //   DateandTime.Content = DateTime.Now.ToString("dd MMM yyyy HH:mm tt");
        }

        private void BtnMeasurement_Click(object sender, RoutedEventArgs e)
        {
            string serr = "";

            if ((string)BtnMeasurement.Content == "Start Measurement")
            {
                
                if (mmgr.ReadDetector.checkfeildentry())
                {
                    if (!mmgr.ReadDetector.checkfeildentryexist())
                    {
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
            else if((string)BtnMeasurement.Content == "Cancel Measurement")
            {
                MessageBoxResult r = MessageBox.Show("Do You want to Cancel Measurement??", "Attention", MessageBoxButton.YesNo);
                if (r == MessageBoxResult.Yes)
                {
                    mmgr.ReadDetector.MeasurementStatusCont = 0;
                    mmgr.ReadDetector.CancelMeasurement();
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
            string serr = ""; //Testing from Arul 
            serr = mmgr.ReadDetector.BrowseLocation();
            if (serr != "")
                MessageBox.Show(serr, "Change Location");

            //CustomFolderDialog customFolderDialog = mmgr.ReadDetector.CustomFolderDialog();
            //if(customFolderDialog.ShowDialog() == true)
            //{
            //    string selectedFolderPath = customFolderDialog.SelectedFolderPath;
            //    MessageBox.Show($"Selected Folder: {selectedFolderPath}", "Change Location");
            //}
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!mmgr.ReadDetector.IsDataSavedDB)
                {
                    mmgr.ReadDetector.SaveMeasurementData();
                    if(mmgr.ReadDetector.UserChooseDir == "")
                    {
                        string serr = ""; //Testing from Arul 
                        serr = mmgr.ReadDetector.BrowseLocation();
                        mmgr.ReadDetector.SaveFilePDF();
                    }
                    else
                    {
                        mmgr.ReadDetector.SaveFilePDF();
                    }
                       
                }
                //else
                //{
                //    MessageBox.Show("Data is already saved.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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
            if (!mmgr.ReadDetector.IsDataSavedDB)
            {
                mmgr.ReadDetector.SaveMeasurementData();
                mmgr.ReadDetector.IsDataSavedDB = true;
                mmgr.ReadDetector.SaveFilePDF();
            }

            //List<DataItem> allDataItems = GetAllDataItems();

            //if (allDataItems.Count > 0)
            //{
            //    SaveFileDialog saveFileDialog = new SaveFileDialog
            //    {
            //        Filter = "PDF files (*.pdf)|*.pdf",
            //        Title = "Save PDF file"
            //    };

            //    if (saveFileDialog.ShowDialog() == true)
            //    {
            //        using (var writer = new PdfWriter(Path.Combine(saveFileDialog.FileName)))
            //        using (var pdf = new PdfDocument(writer))
            //        {
            //            CustomPdfPageEvent pageEvent = new CustomPdfPageEvent();
            //            //pageEvent.SetTotalPages(totalPages);
            //            pdf.AddEventHandler(iText.Kernel.Events.PdfDocumentEvent.END_PAGE, pageEvent);

            //            using (var document = new Document(pdf))
            //            {
            //                int totalPages = allDataItems.Count;
            //                Paragraph title = new Paragraph("FUEL ANALYZER MEASUREMENT REPORT")
            //                    .SetFontColor(ColorConstants.WHITE)
            //                    .SetFontSize(16)
            //                    .SetBold();
            //                string logoPath = @"C:\FuelAnalyzer\bin\Icon\Company_Logo.png"; // Replace with the actual path to your logo
            //                ImageData imageData = ImageDataFactory.Create(logoPath);
            //                Image logoImage = new Image(imageData).ScaleAbsolute(30, 30).SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.RIGHT);


            //                Table headerTable = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
            //                headerTable.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
            //                headerTable.SetBackgroundColor(ColorConstants.BLUE);


            //                Cell titleCell = new Cell(1, 2).Add(title).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT);
            //                headerTable.AddCell(titleCell);


            //                Cell logoCell = new Cell().Add(logoImage).SetBorder(iText.Layout.Borders.Border.NO_BORDER);
            //                headerTable.AddCell(logoCell);


            //                document.Add(headerTable);

            //                document.Add(new Paragraph("\n"));

            //                Paragraph reportTitle = new Paragraph("REPORT").SetFontColor(ColorConstants.BLACK)
            //                    .SetFontSize(16)
            //                    .SetBold();
            //                document.Add(reportTitle);
            //                document.Add(new Paragraph("\n"));

            //                History history = new History();
            //                Table additionalInfoTable = history.CreateAdditionalInfoTable();// Add iTextSharp table with additional information
            //                document.Add(additionalInfoTable);

            //                document.Add(new Paragraph("\n"));
            //                Paragraph equipmentInfo = new Paragraph("EQUIPMENT INFORMATION").SetFontColor(ColorConstants.BLACK)
            //                    .SetFontSize(16)
            //                    .SetBold();
            //                document.Add(equipmentInfo);
            //                document.Add(new Paragraph("\n"));

            //                Table equipmentInfoTable = history.CreateEquipmentInfoTable();// Add iTextSharp table with additional information
            //                document.Add(equipmentInfoTable);



            //                // Add a new page
            //                //document.Add(new AreaBreak());




            //                allDataItems.Sort((x, y) => x.Batch.GetValueOrDefault() - y.Batch.GetValueOrDefault());

            //                // Initialize batch number for the first item
            //                int currentBatchNumber = allDataItems[0].Batch.GetValueOrDefault();
            //                int pageNumber = 1;

            //                foreach (var itemGroup in allDataItems.GroupBy(item => item.Batch.GetValueOrDefault()))
            //                {
            //                    // Start a new page for a new batch
            //                    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

            //                    Paragraph sampleReport = new Paragraph($"SAMPLE MEASUREMENT REPORT {pageNumber}").SetFontColor(ColorConstants.WHITE).SetFontSize(16).SetBold().SetBackgroundColor(ColorConstants.BLUE);
            //                    document.Add(sampleReport);
            //                    document.Add(new Paragraph("\n"));



            //                    var operatorNames = itemGroup.Select(item => item.Operator).Distinct();
            //                    string concatenatedOperatorNames = string.Join(", ", operatorNames);

            //                    // Add summary information for the batch
            //                    Table summarySelectedItemsTable = history.CreateSummarySelectedItemsTable(itemGroup.First(), itemGroup.Count(), concatenatedOperatorNames);
            //                    document.Add(summarySelectedItemsTable);

            //                    document.Add(new Paragraph("\n"));
            //                    Paragraph avgResultInfo = new Paragraph("AVERAGE RESULT").SetFontColor(ColorConstants.BLACK)
            //                        .SetFontSize(16)
            //                        .SetBold();
            //                    document.Add(avgResultInfo);
            //                    document.Add(new Paragraph("\n"));

            //                    // Calculate average values for the batch
            //                    //double avgEthanol = itemGroup.Average(item => item.Ethanol ?? 0);
            //                    //double avgDenaturant = itemGroup.Average(item => item.Denaturant ?? 0);
            //                    //double avgMethanol = itemGroup.Average(item => item.Methanol ?? 0);
            //                    //double avgWater = itemGroup.Average(item => item.Water ?? 0);

            //                    double? avgEthanol = itemGroup.Select(item => item.Ethanol).Where(value => value.HasValue).Select(value => value.Value).DefaultIfEmpty().Average();

            //                    double? avgDenaturant = itemGroup.Select(item => item.Denaturant).Where(value => value.HasValue).Select(value => value.Value).DefaultIfEmpty().Average();

            //                    double? avgMethanol = itemGroup.Select(item => item.Methanol).Where(value => value.HasValue).Select(value => value.Value).DefaultIfEmpty().Average();

            //                    double? avgWater = itemGroup.Select(item => item.Water).Where(value => value.HasValue).Select(value => value.Value).DefaultIfEmpty().Average();



            //                    // Create a single row table for average results
            //                    Table avgResultItemsTable = history.CreateAverageResultItemsTable(
            //                        new DataItem
            //                        {
            //                            Ethanol = (double)avgEthanol,
            //                            Denaturant = (double)avgDenaturant,
            //                            Methanol = (double)avgMethanol,
            //                            Water = (double)avgWater
            //                        }
            //                    );
            //                    document.Add(avgResultItemsTable);

            //                    document.Add(new Paragraph("\n"));
            //                    Paragraph passesResultInfo = new Paragraph("PASSES RESULTS").SetFontColor(ColorConstants.BLACK)
            //                        .SetFontSize(16)
            //                        .SetBold();
            //                    document.Add(passesResultInfo);
            //                    document.Add(new Paragraph("\n"));

            //                    // Create a table for all items in the batch
            //                    Table selectedItemsTable = new Table(UnitValue.CreatePercentArray(6)).UseAllAvailableWidth();
            //                    selectedItemsTable.AddCell(new Cell().Add(new Paragraph("Pass No.")).SetTextAlignment(TextAlignment.CENTER));
            //                    selectedItemsTable.AddCell(new Cell().Add(new Paragraph("Time Stamp")).SetTextAlignment(TextAlignment.CENTER));
            //                    selectedItemsTable.AddCell(new Cell().Add(new Paragraph("Ethanol (Vol%)")).SetTextAlignment(TextAlignment.CENTER));
            //                    selectedItemsTable.AddCell(new Cell().Add(new Paragraph("Denaturant (Vol%)")).SetTextAlignment(TextAlignment.CENTER));
            //                    selectedItemsTable.AddCell(new Cell().Add(new Paragraph("Methanol (Vol%)")).SetTextAlignment(TextAlignment.CENTER));
            //                    selectedItemsTable.AddCell(new Cell().Add(new Paragraph("Water (Vol%)")).SetTextAlignment(TextAlignment.CENTER));

            //                    foreach (var item in itemGroup)
            //                    {
            //                        selectedItemsTable.AddCell(new Cell().Add(new Paragraph(item.PassNo.ToString())));
            //                        selectedItemsTable.AddCell(new Cell().Add(new Paragraph(item.Timestamp.ToString())));
            //                        selectedItemsTable.AddCell(new Cell().Add(new Paragraph(item.Ethanol != null ? item.Ethanol.ToString() : "-")));
            //                        selectedItemsTable.AddCell(new Cell().Add(new Paragraph(item.Denaturant != null ? item.Denaturant.ToString() : "-")));
            //                        selectedItemsTable.AddCell(new Cell().Add(new Paragraph(item.Methanol != null ? item.Methanol.ToString() : "-")));
            //                        selectedItemsTable.AddCell(new Cell().Add(new Paragraph(item.Water != null ? item.Water.ToString() : "-")));
            //                    }



            //                    document.Add(selectedItemsTable);
            //                    pageNumber++;
            //                    //MessageBox.Show($"hello{ itemGroup.Count()}");
            //                }
            //                //pageEvent.SetTotalPages(pdf.GetNumberOfPages());
            //                pageEvent.SetTotalPages(totalPages);
            //                //pdf.AddEventHandler(iText.Kernel.Events.PdfDocumentEvent.END_PAGE, pageEvent);
            //                //pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new History.FooterEventHandler()); // Add a footer to each page
            //            }


            //            MessageBox.Show("PDF file exported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            //        }
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("No items selected for export.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}

            //BtnMeasurement.Click(sender);
            mmgr.ReadDetector.PassNo++;
            mmgr.ReadDetector.MeasuremantBtnContent = "Start Measurement";
            mmgr.ReadDetector.PyExceptionCount = 0;
            mmgr.ReadDetector.StartMeasurement(32, 16);
            mmgr.ReadDetector.IsRepeatmeasure = false;

        }
    }
}
