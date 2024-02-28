using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Office.Interop.Excel;
using Excel;
using System.Runtime.InteropServices;

namespace SignedInUsers
{
    public class ExcelUtlity
    {
        //public LogInformation LogInfo { get; set; }

      
        //public void WriteDataTableToExcel(System.Data.DataTable dataTable, string worksheetName, string saveAsLocation)
        //{
        //    Thread secondaryThread = new Thread(new ThreadStart(() =>
        //    {
        //        bool result = true;
        //        result = ExportDataTableToExcel(dataTable, worksheetName, saveAsLocation);
        //    }));
        //    secondaryThread.Start();
        // }

        /// <summary>
        /// FUNCTION FOR EXPORT TO EXCEL
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="worksheetName"></param>
        /// <param name="saveAsLocation"></param>
        /// <returns></returns>
        public bool ExportDataTableToExcel(System.Data.DataTable dataTable, string worksheetName, string saveAsLocation)
        {
            Microsoft.Office.Interop.Excel.Application excel = null;
            Workbook excelworkBook = null;
            Worksheet excelSheet;
            Range excelCellrange;

            try
            {
                Stopwatch _watch = new Stopwatch();

                if (!Directory.Exists(Path.GetDirectoryName(saveAsLocation)))
                    Directory.CreateDirectory(Path.GetDirectoryName(saveAsLocation));


                // Start Excel and get Application object.
                excel = new Microsoft.Office.Interop.Excel.Application();

                // for making Excel visible
                excel.Visible = false;
                excel.DisplayAlerts = false;

                // Creation a new Workbook
                excelworkBook = excel.Workbooks.Add(Type.Missing);

                // Workk sheet
                excelSheet = (Worksheet)excelworkBook.ActiveSheet;
                excelSheet.Name = worksheetName;


                //excelSheet.Cells[1, 1] = ReporType;
                //excelSheet.Cells[1, 2] = "Date : " + DateTime.Now.ToShortDateString();

                // loop through each row and add values to our sheet
                int rowcount = 1;
                _watch.Start();

                foreach (DataRow datarow in dataTable.Rows)
                {

                    rowcount += 1;
                    for (int i = 1; i <= dataTable.Columns.Count; i++)
                    {
                        // on the first iteration we add the column headers
                        if (rowcount == 2)
                        {
                            excelSheet.Cells[1, i] = dataTable.Columns[i - 1].ColumnName;
                            excelSheet.Cells.Font.Color = System.Drawing.Color.Black;
                        }

                        if (datarow[i - 1].GetType().Name.ToUpper().Contains("STRING"))
                        {
                            //excelSheet.Cells[rowcount, i].NumberFormat = "@";
                            excelSheet.Cells[rowcount, i] = datarow[i - 1].ToString();
                        }
                        else
                            excelSheet.Cells[rowcount, i] = datarow[i - 1].ToString();

                        ////for alternate rows
                        //if (rowcount > 2)
                        //{
                        //    if (i == dataTable.Columns.Count)
                        //    {
                        //        if (rowcount % 2 == 0)
                        //        {
                        //            excelCellrange = excelSheet.Range[excelSheet.Cells[rowcount, 1], excelSheet.Cells[rowcount, dataTable.Columns.Count]];
                        //            //FormattingExcelCells(excelCellrange, "#CCCCFF", System.Drawing.Color.Black, false);
                        //            //FormattingExcelCells(excelCellrange, "#D9E1F2", System.Drawing.Color.Black, false);
                        //            FormattingExcelCells(excelCellrange, "#FFFFFF", System.Drawing.Color.Black, false);
                        //            //FormattingExcelCells(excelCellrange, Brushes.Black, false);
                        //        }

                        //    }
                        //}

                    }

                }

                // now we resize the columns
                excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[rowcount, dataTable.Columns.Count]];
                excelCellrange.EntireColumn.AutoFit();
                Borders border = excelCellrange.Borders;
                border.LineStyle = XlLineStyle.xlContinuous;
                border.Weight = 2d;


                excelCellrange = excelSheet.Range[excelSheet.Cells[1, 1], excelSheet.Cells[1, dataTable.Columns.Count]];
                FormattingExcelCells(excelCellrange, "#F4B084", System.Drawing.Color.Black, true);
                //FormattingExcelCells(excelCellrange, Brushes.White, true);


                //now save the workbook and exit Excel
                _watch.Stop();


                excelworkBook.SaveAs(saveAsLocation);
                excelworkBook.Close();
                excel.Quit();

                TimeSpan ts = _watch.Elapsed;

                return true;
            }
            catch (Exception)
            {
                if (excelworkBook != null)
                    excelworkBook.Close();
                if (excel != null)
                    excel.Quit();

                return false;
            }
            finally
            {
                excelSheet = null;
                excelCellrange = null;
                excelworkBook = null;
            }

        }

        /// <summary>
        /// FUNCTION FOR FORMATTING EXCEL CELLS
        /// </summary>
        /// <param name="range"></param>
        /// <param name="HTMLcolorCode"></param>
        /// <param name="fontColor"></param>
        /// <param name="IsFontbool"></param>
        //public void FormattingExcelCells(Range range, string HTMLcolorCode, System.Drawing.Color fontColor, bool IsFontbool)
        private void FormattingExcelCells(Range range, string HTMLcolorCode, System.Drawing.Color fontColor, bool IsFontbool)
        {
            range.Interior.Color = System.Drawing.ColorTranslator.FromHtml(HTMLcolorCode);
            range.Font.Color =  System.Drawing.ColorTranslator.ToOle(fontColor);
            if (IsFontbool == true)
            {
                range.Font.Bold = IsFontbool;
            }
        }


        //public static void ExportToExcel(System.Windows.Forms.DataGridView dgView)
        //{
        //    Microsoft.Office.Interop.Excel.Application excelApp = null;
        //    try
        //    {
        //        // instantiating the excel application class
        //        excelApp = new Microsoft.Office.Interop.Excel.Application();
        //        Microsoft.Office.Interop.Excel.Workbook currentWorkbook = excelApp.Workbooks.Add(Type.Missing);
        //        Microsoft.Office.Interop.Excel.Worksheet currentWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)currentWorkbook.ActiveSheet;
        //        currentWorksheet.Columns.ColumnWidth = 18;
        //        //currentWorksheet.EnableAutoFilter = true;
        //        //currentWorksheet.Cells.AutoFilter(1); 

        //        if (dgView != null && dgView.Rows.Count > 0)
        //        {
        //            currentWorksheet.Cells[1, 1] = DateTime.Now.ToString("s");
        //            int i = 1;
        //            foreach (System.Windows.Forms.DataGridViewColumn dgviewColumn in dgView.Columns)
        //            {
        //                // Excel work sheet indexing starts with 1
        //                currentWorksheet.Cells[2, i] = dgviewColumn.Name;
        //                ++i;
        //            }
        //            Microsoft.Office.Interop.Excel.Range headerColumnRange = currentWorksheet.get_Range("A2", "G2");
        //            headerColumnRange.Font.Bold = true;
        //            headerColumnRange.Font.Color = 0xFF0000;
        //            //headerColumnRange.EntireColumn.AutoFit();
        //            int rowIndex = 0;
        //            for (rowIndex = 0; rowIndex < dgView.Rows.Count; rowIndex++)
        //            {
        //                System.Windows.Forms.DataGridViewRow dgRow = dgView.Rows[rowIndex];
        //                for (int cellIndex = 0; cellIndex < dgRow.Cells.Count; cellIndex++)
        //                {
        //                    currentWorksheet.Cells[rowIndex + 3, cellIndex + 1] = dgRow.Cells[cellIndex].Value;
        //                }
        //            }
        //            Microsoft.Office.Interop.Excel.Range fullTextRange = currentWorksheet.get_Range("A1", "G" + (rowIndex + 1).ToString());
        //            fullTextRange.WrapText = true;
        //            fullTextRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
        //        }
        //        else
        //        {
        //            string timeStamp = DateTime.Now.ToString("s");
        //            timeStamp = timeStamp.Replace(':', '-');
        //            timeStamp = timeStamp.Replace("T", "__");
        //            currentWorksheet.Cells[1, 1] = timeStamp;
        //            currentWorksheet.Cells[1, 2] = "No error occured";
        //        }
        //        using (System.Windows.Forms.SaveFileDialog exportSaveFileDialog = new System.Windows.Forms.SaveFileDialog())
        //        {
        //            exportSaveFileDialog.Title = "Select Excel File";
        //            exportSaveFileDialog.Filter = "Microsoft Office Excel Workbook(*.xlsx)|*.xlsx";

        //            if (System.Windows.Forms.DialogResult.OK == exportSaveFileDialog.ShowDialog())
        //            {
        //                string fullFileName = exportSaveFileDialog.FileName;
        //                // currentWorkbook.SaveCopyAs(fullFileName);
        //                // indicating that we already saved the workbook, otherwise call to Quit() will pop up
        //                // the save file dialogue box

        //                currentWorkbook.SaveAs(fullFileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook, System.Reflection.Missing.Value, Missing.Value, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true, Missing.Value, Missing.Value, Missing.Value);
        //                currentWorkbook.Saved = true;
        //                System.Windows.Forms.MessageBox.Show("Error memory exported successfully", "Exported to Excel", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.Forms.MessageBox.Show(ex.Message, "Exception", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        //    }
        //    finally
        //    {
        //        if (excelApp != null)
        //        {
        //            excelApp.Quit();
        //        }
        //    }



        //}



        //public bool ExportToCSV(System.Data.DataTable sourceTable, string filename)
        //{
        //    bool result = true;

        //    if (string.IsNullOrWhiteSpace(filename))
        //        return false;

        //    try
        //    {
        //        if (Path.GetExtension(filename).ToUpper() != ".CSV")
        //            filename.Replace(Path.GetExtension(filename), ".csv");

        //        if (!Directory.Exists(Path.GetDirectoryName(filename)))
        //            Directory.CreateDirectory(Path.GetDirectoryName(filename));

        //        using (StreamWriter writer = new StreamWriter(filename))
        //        {
        //            WriteDataTable(sourceTable, writer, true);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        result = false;
        //    }
        //    return result;
        //}

        //public void WriteDataTable(System.Data.DataTable sourceTable, StreamWriter writer, bool includeHeaders)
        //{
        //    if (includeHeaders)
        //    {
        //        List<string> headerValues = new List<string>();
        //        foreach (DataColumn column in sourceTable.Columns)
        //        {
        //            headerValues.Add(QuoteValue(column.ColumnName));
        //        }

        //        writer.WriteLine(string.Join(",", headerValues.ToArray()));
        //    }


        //    string[] items = null;
        //    int rowsCnt = sourceTable.Rows.Count;

        //    foreach (DataRow row in sourceTable.Rows)
        //    {
        //        items = row.ItemArray.Select(o => QuoteValue(o.ToString())).ToArray();
        //        writer.WriteLine(string.Join(",", items));
        //    }

        //    writer.Flush();
        //}

        //private static string QuoteValue(string value)
        //{
        //    return String.Concat("\"", value.Replace("\"", "\"\""), "\"");
        //}

        //Khaja
        //Read the data from excel
        public static System.Data.DataTable ExcelToDataTable(string fileName, string SheetName)
        {
            //Opn file and returns as stream
            FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read);

            //Createopenxmlreader via ExcelReaderFactory
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream); //Read the .xlsx file

            //Set the first row as column name
            excelReader.IsFirstRowAsColumnNames = true;

            //Return the DataSet
            DataSet result = excelReader.AsDataSet();

            //Get all the Tables
            DataTableCollection table = result.Tables;

            //Store it in DataTable
            System.Data.DataTable resultTable = table[SheetName];

            //return
            return resultTable;

        }

        public static void OpenAndSaveExcel(string filename)
        {
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workBook;           
            workBook = app.Workbooks.Open(filename);
            workBook.RefreshAll();
            workBook.Save();
            workBook.Close(false);
            app.Quit();

            GC.GetTotalMemory(false);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.GetTotalMemory(true);

            Marshal.ReleaseComObject(workBook);
            Marshal.ReleaseComObject(app);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);

    }

    public static class ListtoDataTableConverter
    {
        public static System.Data.DataTable ToDataTable<T>(this ObservableCollection<T> items)
        {
            System.Data.DataTable dataTable = new System.Data.DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);

            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        //public static void DataTableToExcel(WebRmsAutomationUtility.TestResults)
        //{
        //    ObservableCollection<ResultLog> TestResults = new ObservableCollection<ResultLog>();

        //    // Conert Collection to Data Table
        //    DataTable dt = TestResults.ToDataTable();

        //    //Export Result into Xlsx
        //    WebRmsAutomationUtility.ExcelUtlity objExcel = new WebRmsAutomationUtility.ExcelUtlity();
        //    objExcel.ExportDataTableToExcel(dt, "ResultsSheet", @"C:\TestResults.xlsx");

        //}

    }

}
