using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;


namespace ContraDrift
{
    public class DataManager
    {
        public class DataRecord
        {
            

            public DateTime Timestamp { get; set; }
            public string Filename { get; set; }
            public string Type { get; set; }
            public double ExpTime { get; set; }
            public string Filter { get; set; }
            public double DtSec { get; set; }
            public double PlateRa { get; set; }
            public double PlateRaArcSecBuf { get; set; }
            public double RaP { get; set; }
            public double RaI { get; set; }
            public double RaD { get; set; }
            public double NewRaRate { get; set; }
            public double PlateDec { get; set; }
            public double PlateDecArcSecBuf { get; set; }
            public double DecP { get; set; }
            public double DecI { get; set; }
            public double DecD { get; set; }
            public double NewDecRate { get; set; }
            public double ScopeRa { get; set; }
            public double ScopeDec { get; set; }
            public double FitsHeaderRa { get; set; }
            public double FitsHeaderDec { get; set; }
            public string PendingMessage { get; set; }
            public DateTime RateUpdateTimeStamp { get; set; }
        }

        private List<DataRecord> records;
        private DataGridView dataGridView;
        private Excel.Worksheet xlWorkSheet;
        private Excel.Workbook xlWorkBook;
        private Excel.Application xlApp;
        private readonly object excelLock = new object();
        private readonly object recordLock= new object();

        public DataManager(DataGridView gridView, Excel.Application excelApp)
        {
            records = new List<DataRecord>();
            dataGridView = gridView;
            
            xlApp = excelApp;
            SetupDataGridView();
            SetupExcelWriter(); // Call to setup Excel worksheet
        }

        public bool AddRecord(DataRecord record)
        {
            lock (recordLock)
            {
                if (records.Any(r => r.Filename == record.Filename))
                {
                    return false;  // Duplicate found, do not add the record
                }
                records.Add(record);
                AddToGridView(record);
                AddToExcel(record);
            }
            return true;
        }
        // Method to update an existing record
        public bool UpdateRecord(DataRecord updatedRecord)
        {
            var existingRecord = records.FirstOrDefault(r => r.Filename == updatedRecord.Filename);
            if (existingRecord == null)
            {
                AddRecord(updatedRecord);
                return true ;  // No record found to update
            }
            int recordIndex = records.IndexOf(existingRecord);
            records[recordIndex] = updatedRecord;
            UpdateGridView(recordIndex, updatedRecord);
            UpdateExcelRow(recordIndex, updatedRecord);
            return true;
        }
            
        public List<DataRecord> GetRecords()
        {
            return records;
        }
        public int Count()
        {
            return records.Count;
        }

        // Move SetupDataGridView into DataManager
        private void SetupDataGridView()
        {
            dataGridView.Columns.Clear();
            dataGridView.RowHeadersVisible = false;

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn { 
                HeaderText = "Timestamp", 
                Name = "Timestamp", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader, 
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "HH:mm:ss.fff" } } );

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn {
                HeaderText = "Filename",
                Name = "Filename",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight },
                Width = 50,
            //    AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            } ) ;

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn { 
                HeaderText = "Type", 
                Name = "Type",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft },
                Width = 100, 
            //    AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells 
            } );

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn {
                HeaderText = "ExpTime", 
                Name = "ExpTime", 
                SortMode = DataGridViewColumnSortMode.NotSortable, 
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.0" }, 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Filter",
                Name = "Filter",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "DtSec",
                Name = "DtSec",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.0" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "PlateRa",
                Name = "PlateRa",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.0000000" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "PlateRaArcSecBuf",
                Name = "PlateRaArcSecBuf",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.00" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "RaP",
                Name = "RaP",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.00000" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "RaI",
                Name = "RaI",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.00000" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "RaD",
                Name = "RaD",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.00000" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "NewRaRate",
                Name = "NewRaRate",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.00000" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "PlateDec",
                Name = "PlateDec",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.0000000" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "PlateDecArcSecBuf",
                Name = "PlateDecArcSecBuf",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.00" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "DecP",
                Name = "DecP",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.00000" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "DecI",
                Name = "DecI",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.00000" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "DecD",
                Name = "DecD",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.00000" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "NewDecRate",
                Name = "NewDecRate",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.00000" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Messages",
                Name = "Messages",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "ScopeRa",
                Name = "ScopeRa",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.00000" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "ScopeDec",
                Name = "ScopeDec",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.00000" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "FitsHeaderRa",
                Name = "FitsHeaderRa",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.00000" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "FitsHeaderDec",
                Name = "FitsHeaderDec",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "0.00000" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "RateUpdateTimeStamp",
                Name = "RateUpdateTimeStamp",
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "HH:mm:ss.fff" },
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells
            });
        }

        // Move SetupExcelWriter into DataManager
        private void SetupExcelWriter()
        {
            object misValue = System.Reflection.Missing.Value;
            xlWorkBook = xlApp.Workbooks.Add(misValue);
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            // Set up headers from DataGridView columns
            for (int j = 0; j < dataGridView.ColumnCount; j++)
            {
                xlWorkSheet.Cells[1, j + 1] = dataGridView.Columns[j].HeaderText.ToString();
            }
        }

        public void Reset()
        {
            records.Clear();
            dataGridView.Rows.Clear();
            ClearExcelSheet();
        }

        public void SaveToCSV(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Use reflection to get the property names of DataRecord for the header
                var properties = typeof(DataRecord).GetProperties();

                // Write the headers dynamically
                writer.WriteLine(string.Join(",", properties.Select(prop => prop.Name)));

                // Write the values for each record dynamically
                foreach (var record in records)
                {
                    var values = properties.Select(prop => prop.GetValue(record, null)?.ToString() ?? "");
                    writer.WriteLine(string.Join(",", values));
                }
            }
        }

        // Save to Excel using DataRecord
        public void SaveToExcel(string filePath)
        {
            xlWorkSheet.UsedRange.Columns.AutoFit();
            xlWorkBook.SaveCopyAs(filePath);
        }



        // Method to generate a timestamped filename for saving
        public string GenerateFileName(string extension)
        {
            string fileName = DateTime.Now.ToString("yyyy-MM-dd HHmmss") + " - ContraDrift" + extension;
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ContraDriftLog", fileName);
        }

        public void Shutdown()
        {
            // Same shutdown method to clean up Excel resources
            if (xlWorkBook != null)
            {
                xlWorkBook.Close(false);
                Marshal.ReleaseComObject(xlWorkBook);
            }
            if (xlApp != null)
            {
                xlApp.Quit();
                Marshal.ReleaseComObject(xlApp);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        private void ClearExcelSheet()
        {
            Excel.Range usedRange = xlWorkSheet.UsedRange;
            if (usedRange.Rows.Count > 1)
            {
                Excel.Range dataRange = xlWorkSheet.Range["A2", usedRange.SpecialCells(Excel.XlCellType.xlCellTypeLastCell)];
                dataRange.ClearContents();
            }
        }

        private void AddToGridView(DataRecord record)
        {

            dataGridView.Invoke(new Action(() =>
                {
                dataGridView.Rows.Add(
                    record.Timestamp,
                    record.Filename,
                    record.Type,
                    record.ExpTime,
                    record.Filter,
                    record.DtSec,
                    record.PlateRa,
                    record.PlateRaArcSecBuf,
                    record.RaP,
                    record.RaI,
                    record.RaD,
                    record.NewRaRate,
                    record.PlateDec,
                    record.PlateDecArcSecBuf,
                    record.DecP,
                    record.DecI,
                    record.DecD,
                    record.NewDecRate,
                    record.PendingMessage,
                    record.ScopeRa,
                    record.ScopeDec,
                    record.FitsHeaderRa,
                    record.FitsHeaderDec,
                    record.RateUpdateTimeStamp
                );

                
                        ScrollToBottom();
                }));
            
        }


        private void ScrollToBottom()
        {
            // Scroll to the last row in the DataGridView
            if (dataGridView.RowCount > 0)
            {
                dataGridView.FirstDisplayedScrollingRowIndex = dataGridView.RowCount - 1;
            }
        }

        private void AddToExcel(DataRecord record)
        {
            int rowIndex = records.Count + 1; 
            lock (excelLock)
            {
                xlWorkSheet.Cells[rowIndex, 1] = record.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
                xlWorkSheet.Cells[rowIndex, 1].NumberFormat = "m/d/yyyy h:mm:ss.000";

                xlWorkSheet.Cells[rowIndex, 2] = record.Filename;
                xlWorkSheet.Cells[rowIndex, 3] = record.Type;
                xlWorkSheet.Cells[rowIndex, 4] = record.ExpTime;
                xlWorkSheet.Cells[rowIndex, 5] = record.Filter;
                xlWorkSheet.Cells[rowIndex, 6] = record.DtSec;
                xlWorkSheet.Cells[rowIndex, 7] = record.PlateRa;
                xlWorkSheet.Cells[rowIndex, 8] = record.PlateRaArcSecBuf;
                xlWorkSheet.Cells[rowIndex, 9] = record.RaP;
                xlWorkSheet.Cells[rowIndex, 10] = record.RaI;
                xlWorkSheet.Cells[rowIndex, 11] = record.RaD;
                xlWorkSheet.Cells[rowIndex, 12] = record.NewRaRate;
                xlWorkSheet.Cells[rowIndex, 13] = record.PlateDec;
                xlWorkSheet.Cells[rowIndex, 14] = record.PlateDecArcSecBuf;
                xlWorkSheet.Cells[rowIndex, 15] = record.DecP;
                xlWorkSheet.Cells[rowIndex, 16] = record.DecI;
                xlWorkSheet.Cells[rowIndex, 17] = record.DecD;
                xlWorkSheet.Cells[rowIndex, 18] = record.NewDecRate;
                xlWorkSheet.Cells[rowIndex, 19] = record.PendingMessage;
                xlWorkSheet.Cells[rowIndex, 20] = record.ScopeRa;
                xlWorkSheet.Cells[rowIndex, 21] = record.ScopeDec;
                xlWorkSheet.Cells[rowIndex, 22] = record.FitsHeaderRa;
                xlWorkSheet.Cells[rowIndex, 23] = record.FitsHeaderDec;
                xlWorkSheet.Cells[rowIndex, 24] = record.RateUpdateTimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
                xlWorkSheet.Cells[rowIndex, 24].NumberFormat = "m/d/yyyy h:mm:ss.000";
            }
        }

        private void UpdateGridView(int rowIndex, DataRecord record)
        {
            dataGridView.Invoke(new Action(() =>
            {

                var row = dataGridView.Rows[rowIndex];
                row.SetValues(
                    record.Timestamp,
                    record.Filename,
                    record.Type,
                    record.ExpTime,
                    record.Filter,
                    record.DtSec,
                    record.PlateRa,
                    record.PlateRaArcSecBuf,
                    record.RaP,
                    record.RaI,
                    record.RaD,
                    record.NewRaRate,
                    record.PlateDec,
                    record.PlateDecArcSecBuf,
                    record.DecP,
                    record.DecI,
                    record.DecD,
                    record.NewDecRate,
                    record.PendingMessage,
                    record.ScopeRa,
                    record.ScopeDec,
                    record.FitsHeaderRa,
                    record.FitsHeaderDec,
                    record.RateUpdateTimeStamp
                );
            }));
        }

        private void UpdateExcelRow(int rowIndex, DataRecord record)
        {
            lock (excelLock)
            {
                rowIndex = rowIndex + 2; // excel starts counting at 1, plus there is a header.  So we have to offset by for all data.
                xlWorkSheet.Cells[rowIndex, 1] = record.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
                xlWorkSheet.Cells[rowIndex, 2] = record.Filename;
                xlWorkSheet.Cells[rowIndex, 3] = record.Type;
                xlWorkSheet.Cells[rowIndex, 4] = record.ExpTime;
                xlWorkSheet.Cells[rowIndex, 5] = record.Filter;
                xlWorkSheet.Cells[rowIndex, 6] = record.DtSec;
                xlWorkSheet.Cells[rowIndex, 7] = record.PlateRa;
                xlWorkSheet.Cells[rowIndex, 8] = record.PlateRaArcSecBuf;
                xlWorkSheet.Cells[rowIndex, 9] = record.RaP;
                xlWorkSheet.Cells[rowIndex, 10] = record.RaI;
                xlWorkSheet.Cells[rowIndex, 11] = record.RaD;
                xlWorkSheet.Cells[rowIndex, 12] = record.NewRaRate;
                xlWorkSheet.Cells[rowIndex, 13] = record.PlateDec;
                xlWorkSheet.Cells[rowIndex, 14] = record.PlateDecArcSecBuf;
                xlWorkSheet.Cells[rowIndex, 15] = record.DecP;
                xlWorkSheet.Cells[rowIndex, 16] = record.DecI;
                xlWorkSheet.Cells[rowIndex, 17] = record.DecD;
                xlWorkSheet.Cells[rowIndex, 18] = record.NewDecRate;
                xlWorkSheet.Cells[rowIndex, 19] = record.PendingMessage;
                xlWorkSheet.Cells[rowIndex, 20] = record.ScopeRa;
                xlWorkSheet.Cells[rowIndex, 21] = record.ScopeDec;
                xlWorkSheet.Cells[rowIndex, 22] = record.FitsHeaderRa;
                xlWorkSheet.Cells[rowIndex, 23] = record.FitsHeaderDec;
                xlWorkSheet.Cells[rowIndex, 24] = record.RateUpdateTimeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
        }
    }

}
