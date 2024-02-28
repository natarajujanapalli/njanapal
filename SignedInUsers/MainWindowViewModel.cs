using Microsoft.Win32;
using njanapal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.Reflection;

namespace SignedInUsers
{
    public class MainWindowViewModel : ViewModelBase
    {

        private bool _isEnable;
        public bool IsEnable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                RaisePropertyChanged("IsEnable");

                if (!string.IsNullOrWhiteSpace(_file) && File.Exists(_file) && IsEnable)
                    EnableLoadBtn = true;
                else
                    EnableLoadBtn = false;
            }
        }

        private bool _enableLoad;
        public bool EnableLoadBtn
        {
            get { return _enableLoad; }
            set
            {
                _enableLoad = value;
                RaisePropertyChanged("EnableLoadBtn");
            }
        }


        private string _status;
        public string Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged("Status"); }
        }

        private string _file;
        public string FilePath
        {
            get { return _file; }
            set
            {
                _file = value;
                RaisePropertyChanged("FilePath");

                if (!string.IsNullOrWhiteSpace(_file) && File.Exists(_file) && IsEnable)
                    EnableLoadBtn = true;
                else
                    EnableLoadBtn = false;
            }
        }

        private ObservableCollection<string> _machines;
        public ObservableCollection<string> Machines
        {
            get { return _machines; }
            set { _machines = value; RaisePropertyChanged("Machines"); }
        }

        private string _selectedMachine;
        public string SelectedMachine
        {
            get { return _selectedMachine; }
            set { _selectedMachine = value; RaisePropertyChanged("SelectedMachine"); }
        }


        private ObservableCollection<RemoteMachineUser> _userDetails;
        public ObservableCollection<RemoteMachineUser> UserDetails
        {
            get { return _userDetails; }
            set { _userDetails = value; RaisePropertyChanged("UserDetails"); }
        }

        private RemoteMachineUser _selectedUserDetails;
        public RemoteMachineUser SelectedUserDetails
        {
            get { return _selectedUserDetails; }
            set { _selectedUserDetails = value; RaisePropertyChanged("SelectedUserDetails"); }
        }


        public RelayCommand BrowseCommand { get; set; }
        public RelayCommand LoadCommand { get; set; }
        public RelayCommand GoCommand { get; set; }
        public RelayCommand GoAllCommand { get; set; }
        public RelayCommand RemoveCommand { get; set; }
        public RelayCommand RemoveAllCommand { get; set; }
        public RelayCommand SignOffCommand { get; set; }
        public RelayCommand ExportExcelCommand { get; set; }



        //bool isDisplayState = Convert.ToBoolean(ConfigurationManager.AppSettings["DisplayState"].ToString());
        //bool isDisplayIdleTime = Convert.ToBoolean(ConfigurationManager.AppSettings["DisplayIdleTime"].ToString());
        //bool isDisplayLogonTime = Convert.ToBoolean(ConfigurationManager.AppSettings["DisplayLogonTime"].ToString());
        //bool isDisplayID = Convert.ToBoolean(ConfigurationManager.AppSettings["DisplayID"].ToString());
        //bool isDisplaySessionName = Convert.ToBoolean(ConfigurationManager.AppSettings["DisplaySessionName"].ToString());

        Log log = null;
        RemoteMachine remote = null;


        public MainWindowViewModel()
        {
            this.IsEnable = true;

            BrowseCommand = new RelayCommand(Browse);
            LoadCommand = new RelayCommand(Load);
            GoCommand = new RelayCommand(Go);
            GoAllCommand = new RelayCommand(GoAll);
            RemoveCommand = new RelayCommand(Remove);
            RemoveAllCommand = new RelayCommand(RemoveAll);
            SignOffCommand = new RelayCommand(SignOff);
            ExportExcelCommand = new RelayCommand(ExportExcel);

            FilePath = $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RemoteMachines.txt")}";

            log = new Log();
            UserDetails = new ObservableCollection<RemoteMachineUser>();
            remote = new RemoteMachine(log);
        }

        public void Browse()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (openFileDialog.ShowDialog() == true)
                    FilePath = openFileDialog.FileName;

                Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to Browse File.");
                FilePath = string.Empty;
            }
        }

        public void Load()
        {
            if (this.Machines == null)
                this.Machines = new ObservableCollection<string>();

            try
            {
                this.UserDetails = new ObservableCollection<RemoteMachineUser>();
                var machines = File.ReadLines(this.FilePath).Where(r => !string.IsNullOrWhiteSpace(r) && !r.Trim().StartsWith("--")).Distinct().OrderBy(r => r).ToList();

                foreach (var vm in machines)
                {
                    if (!string.IsNullOrWhiteSpace(vm.ToUpper().Trim()) && !this.Machines.Contains(vm.ToUpper().Trim()))
                        this.Machines.Add(vm.ToUpper().Trim());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to Load File.");
            }

        }

        public StringBuilder Statuses = new StringBuilder();

        public async void GoAll()
        {
            Stopwatch timer = new Stopwatch();

            this.IsEnable = false;

            int i = 0;
            try
            {
                Statuses.Clear();
                timer.Start();

                this.UserDetails = new ObservableCollection<RemoteMachineUser>();

                foreach (string machine in this.Machines)
                {
                    this.Status = $"Processing : {++i}/{this.Machines.Count} - '{machine}'";
                    //Process(machine);
                    var result = await Task.Run(() => Process(machine));

                    if (result != null && result.Count() > 0)
                    {
                        foreach (var r in result)
                            this.UserDetails.Add(r);
                    }
                }

                timer.Stop();
                TimeSpan timeTaken = timer.Elapsed;
                string hrs = timeTaken.Hours.ToString().PadLeft(2, '0');
                string mins = timeTaken.Minutes.ToString().PadLeft(2, '0');
                string secs = timeTaken.Seconds.ToString().PadLeft(2, '0');
                string millisecs = timeTaken.Milliseconds.ToString().PadLeft(3, '0');

                this.Status = $"Time Taken: {hrs} : {mins} : {secs} : {millisecs} to process all machines.";

                Statuses.AppendLine(this.Status);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception Occurred");
            }


            this.IsEnable = true;
        }
       
        public async void Go()
        {
            if (string.IsNullOrWhiteSpace(this.SelectedMachine) || IsEnable == false)
                return;

            Stopwatch timer = new Stopwatch();
            this.IsEnable = false;

            try
            {
                timer.Start();

                this.Status = $"Processing : '{SelectedMachine}'";
                var temp = UserDetails.ToList();
                temp.RemoveAll(r => r.RemoteMachine.Equals(SelectedMachine));

                this.UserDetails = new ObservableCollection<RemoteMachineUser>(temp);

                //Process(SelectedMachine);
                var result = await Task.Run(() => Process(SelectedMachine));

                if (result != null && result.Count() > 0)
                {
                    foreach (var r in result)
                        this.UserDetails.Add(r);
                }

                timer.Stop();
                TimeSpan timeTaken = timer.Elapsed;
                string hrs = timeTaken.Hours.ToString().PadLeft(2, '0');
                string mins = timeTaken.Minutes.ToString().PadLeft(2, '0');
                string secs = timeTaken.Seconds.ToString().PadLeft(2, '0');
                string millisecs = timeTaken.Milliseconds.ToString().PadLeft(3, '0');

                this.Status = $"Time Taken: {hrs} : {mins} : {secs} : {millisecs} to process: '{SelectedMachine}'.";
                Statuses.AppendLine(this.Status);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception Occurred");
            }

            this.IsEnable = true;
        }

        public ObservableCollection<RemoteMachineUser> Process(string machine)
        {
            if (string.IsNullOrWhiteSpace(machine))
                return null;

            ObservableCollection<RemoteMachineUser> result = new ObservableCollection<RemoteMachineUser>();
            string caption = string.Empty;
            string version = string.Empty;
            string oSArchitecture = string.Empty;
            string lastBootUpTime = string.Empty;
            string organization = string.Empty;
            string numberOfUsers = string.Empty;
            ComputerSystem _computerSystem = null;
            DiskDrive _diskDrive = null;

            if (remote.PingHost(machine) == false)
            {
                result.Add(new RemoteMachineUser { RemoteMachine = machine, Message = $"No such host is known. Host : '{machine}'." });
                return result;
            }
            

            try
            {
                (caption, version, oSArchitecture, lastBootUpTime, organization, numberOfUsers) = remote.GetOSFriendlyName(machine);
                _computerSystem = remote.GetComputerSystemInfo(machine);
                _diskDrive = remote.GetDiskDriveInfo(machine);

                var (message, users, usersDetails) = remote.GetLoggedInUsers(machine);

                if (users == null || users.Count == 0)
                {
                    RemoteMachineUser rmu = new RemoteMachineUser();
                    rmu.RemoteMachine = machine;
                    rmu.Message = message;
                    rmu.Caption = caption;
                    rmu.Version = version;
                    rmu.OSArchitecture = oSArchitecture;
                    rmu.LastBootUpTime = lastBootUpTime;
                    rmu.Organization = organization;
                    rmu.NumberOfUsers = numberOfUsers;

                    //result.Add(new RemoteMachineUser { RemoteMachine = machine, Message = message, UserName = string.Empty, SessionName = string.Empty, Id = string.Empty, State = string.Empty, IdleTime = string.Empty, LogonTime = string.Empty });
                    rmu.UserName = string.Empty;
                    rmu.SessionName = string.Empty;
                    rmu.Id = string.Empty;
                    rmu.State = string.Empty;
                    rmu.IdleTime = string.Empty;
                    rmu.LogonTime = string.Empty;

                    if (_computerSystem != null)
                    {
                        //rmu.CaptionTemp = _computerSystem.Caption;
                        rmu.DNSHostName = _computerSystem.DNSHostName;
                        rmu.Domain = _computerSystem.Domain;
                        rmu.Model = _computerSystem.Model;
                        rmu.Name = _computerSystem.Name;
                        rmu.NumberOfLogicalProcessors = _computerSystem.NumberOfLogicalProcessors;
                        rmu.NumberOfProcessors = _computerSystem.NumberOfProcessors;
                        rmu.TotalPhysicalMemory = _computerSystem.TotalPhysicalMemory;
                    }


                    if (_diskDrive != null)
                    {
                        rmu.HardDiskPartitions = _diskDrive.Partitions;
                        rmu.HardDiskSize = _diskDrive.Size;
                    }

                    result.Add(rmu);
                }
                else
                {
                    foreach (string user in users)
                    {
                        RemoteMachineUser rmu = new RemoteMachineUser();
                        rmu.RemoteMachine = machine;
                        rmu.Message = message;
                        rmu.Caption = caption;
                        rmu.Version = version;
                        rmu.OSArchitecture = oSArchitecture;
                        rmu.LastBootUpTime = lastBootUpTime;
                        rmu.Organization = organization;
                        rmu.NumberOfUsers = numberOfUsers;

                        var userDetails = usersDetails?.Where(r => r.UserName.ToLower().Equals(user.ToLower())).FirstOrDefault();
                        if (userDetails == null)
                        {
                            rmu.UserName = user;
                            rmu.SessionName = string.Empty;
                            rmu.Id = string.Empty;
                            rmu.State = string.Empty;
                            rmu.IdleTime = string.Empty;
                            rmu.LogonTime = string.Empty;
                        }
                        else
                        {
                            rmu.UserName = userDetails.UserName;
                            rmu.SessionName = userDetails.SessionName;
                            rmu.Id = userDetails.Id;
                            rmu.State = userDetails.State;
                            rmu.IdleTime = userDetails.IdleTime;
                            rmu.LogonTime = userDetails.LogonTime;
                        }

                        if(_computerSystem != null)
                        {
                            //rmu.CaptionTemp = _computerSystem.Caption;
                            rmu.DNSHostName = _computerSystem.DNSHostName;
                            rmu.Domain = _computerSystem.Domain;
                            rmu.Model = _computerSystem.Model;
                            rmu.Name = _computerSystem.Name;
                            rmu.NumberOfLogicalProcessors = _computerSystem.NumberOfLogicalProcessors;
                            rmu.NumberOfProcessors = _computerSystem.NumberOfProcessors;
                            rmu.TotalPhysicalMemory = _computerSystem.TotalPhysicalMemory;
                        }


                        if(_diskDrive != null)
                        {
                            rmu.HardDiskPartitions = _diskDrive.Partitions;
                            rmu.HardDiskSize = _diskDrive.Size;
                        }

                        result.Add(rmu);
                    }
                }
            }
            catch
            {

            }

            return result;
        }

        public void Remove()
        {
            try
            {
                if (this.Machines != null && this.Machines.Count() > 0)
                    this.Machines.Remove(SelectedMachine);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception Occurred");
            }

        }

        public void RemoveAll()
        {
            try
            {
                if (this.Machines != null && this.Machines.Count() > 0)
                    this.Machines.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception Occurred");
            }

        }

        private void SignOff()
        {
            if (SelectedUserDetails == null)
            {
                MessageBox.Show("Selected user cannot be empty.");
                return;
            }

            string SessionID = SelectedUserDetails.Id;
            string ServerName = SelectedUserDetails.RemoteMachine;

            string command = $"logoff {SessionID} /server:{ServerName}";

            //MessageBox.Show(command, "command", MessageBoxButton.OK, MessageBoxImage.Information);

            ////cmd c = new cmd();
            ////c.RunCommandAsAdmin(AppDomain.CurrentDomain.BaseDirectory, command);
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SignOff.bat");
            File.WriteAllText(path, command);

            if(File.Exists(path))
            {
                System.Diagnostics.Process.Start(AppDomain.CurrentDomain.BaseDirectory);
                if(MessageBoxResult.Yes == MessageBox.Show("Do you want to reload users?", "Reload logged in Users", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes))
                {
                    this.SelectedMachine = ServerName;
                    Go();
                }
            }

            //ExecuteBatFile(AppDomain.CurrentDomain.BaseDirectory, "Test.bat");
            //File.Delete(path);
            //Go();
        }

        private void ExportExcel()
        {
            DataTable dt = ConvertToDataTable<RemoteMachineUser>(UserDetails);

            string location = $"{AppDomain.CurrentDomain.BaseDirectory}\\ExportData";

            try
            {
                if (Directory.Exists(location) == false)
                    Directory.CreateDirectory(location);

                string sheetName = DateTime.Now.ToString("yyyyMMddHHmmss");
                var filename = Path.Combine(location, $"{sheetName}.csv");
                //var filename = Path.Combine(location, $"{sheetName}.xlsx");

                // Code example for Method 1
                StringBuilder csvContent = new StringBuilder();

                foreach(DataColumn col in dt.Columns)
                {
                    csvContent.Append(col.ColumnName + ",");
                }

                csvContent.AppendLine();

                foreach (DataRow row in dt.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        csvContent.Append(item.ToString().Replace("\r\n", " ").Replace("\n", " ") + ",");
                    }
                    csvContent.AppendLine();
                }

                File.WriteAllText(filename, csvContent.ToString());
                System.Diagnostics.Process.Start(location);

                //ExcelUtlity eu = new ExcelUtlity();
                //var res = eu.ExportDataTableToExcel(dt, $"{sheetName}", filename);
                //
                //if (res == true)
                //    System.Diagnostics.Process.Start(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error in Export Excel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        static DataTable ConvertToDataTable<T>(ObservableCollection<T> models)
        {
            // creating a data table instance and typed it as our incoming model   
            // as I make it generic, if you want, you can make it the model typed you want.  
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties of that model  
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Loop through all the properties              
            // Adding Column name to our datatable  
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names    
                dataTable.Columns.Add(prop.Name);
            }
            // Adding Row and its value to our dataTable  
            foreach (T item in models)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows    
                    values[i] = Props[i].GetValue(item, null);
                }
                // Finally add value to datatable    
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }


        //public void ExecuteBatFile(string _batDir, string file)
        //{
        //    Process proc = null;
        //
        //    //string _batDir = string.Format(@"C:\");
        //    proc = new Process();
        //    proc.StartInfo.WorkingDirectory = _batDir;
        //    proc.StartInfo.FileName = file;
        //    proc.StartInfo.CreateNoWindow = false;
        //    proc.Start();
        //    proc.WaitForExit();
        //    int ExitCode = proc.ExitCode;
        //    proc.Close();
        //}
    }
}
