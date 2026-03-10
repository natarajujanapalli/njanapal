using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using Newtonsoft.Json;

namespace njanapal
{
    public class Utlity
    {
        public Dictionary<string, string> Owners { get; set; }
        public Utlity()
        {

        }
        public void ProcessExcel(string excelPath)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(excelPath))
                {
                    Console.WriteLine("Excel file path is not provided. Please provide a valid path.");
                    return;
                }
                string outputDirectory = Path.GetDirectoryName(excelPath);
                string sheetName = "VMs";

                Console.WriteLine("Starting VM data processing...");

                // Read Excel file
                var vms = LoadVMsFromExcel(excelPath, sheetName);
                Console.WriteLine($"Loaded {vms.Count} VMs from Excel.");

                // Group and transform data
                var vmModels = GroupVMsByOwner(vms);
                Console.WriteLine($"Grouped into {vmModels.Count} owner groups.");

                // Export to JSON
                string jsonFilePath = Path.Combine(outputDirectory, $"RemoteMachines_{DateTime.Now:yyyyMMdd_HHmmss}.json");
                ExportToJson(vmModels, jsonFilePath);
                Console.WriteLine($"JSON file created successfully at: {jsonFilePath}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Loads VM data from an Excel file
        /// </summary>
        public List<VM> LoadVMsFromExcel(string filePath, string sheetName)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Excel file not found at: {filePath}");
            }
            
            var dataSet = ReadExcelFile(filePath);

            if (!dataSet.Tables.Contains(sheetName))
            {
                throw new ArgumentException($"Sheet '{sheetName}' not found in the workbook. Available sheets: {string.Join(", ", dataSet.Tables.Cast<DataTable>().Select(t => t.TableName))}");
            }

            DataTable dt = dataSet.Tables[sheetName];
            ValidateDataTableStructure(dt);

            var vms = new List<VM>();
            foreach (DataRow row in dt.Rows)
            {
                // Skip rows with empty or null Name
                if (row["Name"] == DBNull.Value || string.IsNullOrWhiteSpace(row.Field<string>("Name")))
                    continue;

                vms.Add(new VM
                {
                    NodeName = row.Field<string>("Name").Trim().ToUpper(),
                    HostName = GetStringValueOrDefault(row, "HostName", "UNKNOWN").ToUpper(),
                    Owner = GetStringValueOrDefault(row, "Owner", "UNKNOWN"),
                    Purpose = GetStringValueOrDefault(row, "Purpose", string.Empty)
                });
            }

            return vms;
        }

        /// <summary>
        /// Validates that the DataTable has required columns
        /// </summary>
        public void ValidateDataTableStructure(DataTable dt)
        {
            var requiredColumns = new[] { "Name", "HostName", "Owner", "Purpose" };
            var missingColumns = requiredColumns.Where(col => !dt.Columns.Contains(col)).ToList();

            if (missingColumns.Any())
            {
                throw new InvalidOperationException($"Missing required columns: {string.Join(", ", missingColumns)}");
            }
        }

        /// <summary>
        /// Safely gets a string value from a DataRow or returns a default value
        /// </summary>
        public string GetStringValueOrDefault(DataRow row, string columnName, string defaultValue)
        {
            if (row[columnName] == DBNull.Value)
                return defaultValue;

            var value = row.Field<string>(columnName);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
        }

        /// <summary>
        /// Groups VMs by owner and sorts them
        /// </summary>
        public List<VMModel> GroupVMsByOwner(List<VM> vms)
        {
            var vmModels = new List<VMModel>();

            // Fix: Use ThenBy for secondary sorting instead of chaining OrderBy
            var groupedVMs = vms
                .OrderBy(x => x.Owner)
                .ThenBy(x => x.NodeName)
                .GroupBy(x => x.Owner);

            foreach (var group in groupedVMs)
            {
                var model = new VMModel
                {
                    Owner = GetOwner(group.Key),
                    MachineNames = group.Select(x => new VMInfo
                    {
                        NodeName = x.NodeName,
                        HostName = x.HostName,
                        Purpose = x.Purpose
                    }).ToList()
                };
                vmModels.Add(model);
            }

            return vmModels;
        }

        /// <summary>
        /// Exports VM models to a JSON file
        /// </summary>
        public void ExportToJson(List<VMModel> vmModels, string filePath)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(vmModels, settings);
            File.WriteAllText(filePath, json);
        }

        public DataSet ReadExcelFile(string filePath)
        {
            DataSet result = new DataSet();
            try
            {
                //using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                //{
                //    using (var reader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                //    {
                //        result = reader.AsDataSet(); // Process the DataSet as needed
                //    }
                //}
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                    {
                        var config = new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true  // Use first row as column headers
                            }
                        };

                        result = reader.AsDataSet(config); // Process the DataSet as needed
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading Excel file: " + ex.Message);
            }

            return result;
        }

        public string GetOwner(string owner)
        {
            if (Owners.TryGetValue(owner, out string mappedOwner) && !string.IsNullOrWhiteSpace(mappedOwner))
                return mappedOwner;

            return owner.Trim();
        }

        //public void ExportToExcel(DataTable dataTable, string filePath)
        //{
        //    try
        //    {
        //        using (var stream = File.Open(filePath, FileMode.Create, FileAccess.Write))
        //        {
        //            using (var writer = ExcelWriterFactory.CreateOpenXmlWriter(stream))
        //            {
        //                writer.Write(dataTable);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error exporting to Excel: " + ex.Message);
        //    }
        //}

    }

    /// <summary>
    /// Represents a Virtual Machine with full details
    /// </summary>
    public class VM
    {
        public string NodeName { get; set; }
        public string HostName { get; set; }
        public string Owner { get; set; }
        public string Purpose { get; set; }
    }

    /// <summary>
    /// Represents VM information for JSON export (without Owner property)
    /// </summary>
    public class VMInfo
    {
        public string NodeName { get; set; }
        public string HostName { get; set; }
        public string Purpose { get; set; }
    }

    /// <summary>
    /// Represents a group of VMs owned by a single owner
    /// </summary>
    public class VMModel
    {
        public string Owner { get; set; }
        public List<VMInfo> MachineNames { get; set; }
    }
}
