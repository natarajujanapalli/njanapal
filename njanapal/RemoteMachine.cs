using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace njanapal
{
    public class RemoteMachine
    {
        //set HKLM\SYSTEM\CurrentControlSet\Control\Terminal Server\AllowRemoteRPC to 1

        public Log Log { get; set; }

        public string NumberOfLogicalProcessors { get; set; }
        public string Caption { get; set; }
        public string DNSHostName { get; set; }

        public RemoteMachine(Log log)
        {
            this.Log = log;
        }

        public List<RemoteMachineUser> LoggedUers { get; set; } = new List<RemoteMachineUser>();

        public List<(string User, string Message, List<RemoteMachineUser> RemoteMachines)> GetLoggedInUsersByUserName(List<string> machines = null)
        {
            List<(string User, string Message, List<RemoteMachineUser> RemoteMachines)> result = new List<(string, string, List<RemoteMachineUser>)>();

            List<RemoteMachineUser> info = this.LoggedUers;

            if (machines != null) // && this.LoggedUers != null && this.LoggedUers.Count > 0)
                info = GetLoggedInUsers(machines);

            var users = info.Select(r => r.UserName).Distinct();

            foreach (string user in users)
            {
                if (user == null || string.IsNullOrWhiteSpace(user))
                    continue;

                string message = string.Empty; //info.Where(r => r.User.Equals(user)).Select(r => r.Message).Distinct().FirstOrDefault().ToString();
                List<RemoteMachineUser> nodenames = info.Where(r => r.UserName != null && r.UserName.Equals(user)).Select(r => r).Distinct().ToList();
                result.Add((user, message, nodenames));
            }

            return result;
        }

        public List<(string RemoteMachine, string Message, List<string> Users)> GetLoggedInUsersByMachine(List<string> machines = null)
        {
            List<(string RemoteMachine, string Message, List<string> Users)> result = new List<(string, string, List<string>)>();

            List<RemoteMachineUser> info = this.LoggedUers;

            if (machines != null)
                info = GetLoggedInUsers(machines);

            var machinesTemp = info.Select(r => r.RemoteMachine).Distinct();

            foreach (string machine in machinesTemp)
            {
                string message = info.Where(r => r.RemoteMachine.Equals(machine)).Select(r => r.Message).Distinct().FirstOrDefault().ToString();
                List<string> users = info.Where(r => r.RemoteMachine.Equals(machine)).Select(r => r.UserName).Distinct().ToList();
                result.Add((machine, message, users));
            }

            return result;
        }

        public List<RemoteMachineUser> GetLoggedInUsers(List<string> machines)
        {
            int i = 1;
            foreach (string machineNodeName in machines)
            {
                if (string.IsNullOrWhiteSpace(machineNodeName))
                    continue;

                this.Log.Status = $"Processing Machine : {i++,3} / {machines.Count} - {machineNodeName}";

                var (message, users, usersDetails) = GetLoggedInUsers(machineNodeName);

                if (users == null || users.Count == 0)
                    LoggedUers.Add(new RemoteMachineUser { RemoteMachine = machineNodeName, Message = message, UserName = string.Empty, SessionName = string.Empty, Id = string.Empty, State = string.Empty, IdleTime = string.Empty, LogonTime = string.Empty });
                else
                {
                    foreach (string user in users)
                    {
                        var userDetails = usersDetails?.Where(r => r.UserName.ToLower().Equals(user.ToLower())).FirstOrDefault();
                        if (userDetails == null)
                            LoggedUers.Add(new RemoteMachineUser { RemoteMachine = machineNodeName, Message = message, UserName = user, SessionName = string.Empty, Id = string.Empty, State = string.Empty, IdleTime = string.Empty, LogonTime = string.Empty });
                        else
                        {
                            LoggedUers.Add(new RemoteMachineUser { RemoteMachine = machineNodeName, Message = message, UserName = userDetails.UserName, SessionName = userDetails.SessionName, Id = userDetails.Id, State = userDetails.State, IdleTime = userDetails.IdleTime, LogonTime = userDetails.LogonTime });
                        }
                    }
                }

            }

            return this.LoggedUers;
        }

        public (string, List<string>, List<QUser>) GetLoggedInUsers(string remoteComputerNodeName)
        {
            List<string> users = new List<string>();
            List<QUser> usersDetails = new List<QUser>();
            string message = string.Empty;

            try
            {
                ConnectionOptions connection = new ConnectionOptions();
                ManagementScope scope = new ManagementScope("\\\\" + remoteComputerNodeName + "\\root\\CIMV2", connection);
                scope.Connect();

                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Process WHERE Name = 'explorer.exe'");

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    ManagementPath path = new ManagementPath("Win32_Process.Handle='" + queryObj["Handle"] + "'");
                    ManagementObject classInstance = new ManagementObject(scope, path, null);
                    ManagementBaseObject outParams = classInstance.InvokeMethod("GetOwner", null, null);

                    users.Add(outParams["User"].ToString());
                }

                usersDetails = GetQUserInfo(remoteComputerNodeName);

            }
            catch (ManagementException err)
            {
                message = $"ManagementException. \r\nDetails: {err.ToString()}";
            }
            catch (System.UnauthorizedAccessException unauthorizedErr)
            {
                message = $"Doesn't have permissions to access the machine, \r\nDetails : {unauthorizedErr.ToString()}";
            }
            catch (Exception ex)
            {
                //message = $"Exception Details: {ex.ToString()}";
                message = $"The RPC server is unavailable. \r\nDetails: {ex.ToString()}";
            }

            return (message, users, usersDetails);
        }


        public List<QUser> GetQUserInfo(string serverName)
        {
            string output = GetQUserInfo32(serverName);
            if (string.IsNullOrWhiteSpace(output))
                output = GetQUserInfo64(serverName);

            if (string.IsNullOrWhiteSpace(output))
                return null;

            List<QUser> users = new List<QUser>();
            string columnsString = output.Split(new string[] { "\r\n" }, StringSplitOptions.None).Where(r => r.Contains("USERNAME")).FirstOrDefault();
            string[] lines = output.Split(new string[] { "\r\n" }, StringSplitOptions.None).Where(r => !r.Contains("USERNAME")).ToArray();

            int i = 0;
            foreach (string line in lines)
            {
                if (line.Contains("USERNAME") || string.IsNullOrWhiteSpace(line))
                    continue;

                users.Add(new QUser(line, columnsString));
            }

            return users;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Wow64DisableWow64FsRedirection(ref IntPtr ptr);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Wow64EnableWow64FsRedirection(ref IntPtr ptr);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Wow64RevertWow64FsRedirection(ref IntPtr ptr);

        public string GetQUserInfo32(string serverName)
        {
            IntPtr val = IntPtr.Zero;
            Wow64DisableWow64FsRedirection(ref val);
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = $"/c quser /SERVER:{serverName}";
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            p.WaitForExit();

            string output = p.StandardOutput.ReadToEnd();

            return output;
        }


        public string GetQUserInfo64(string serverName)
        {
            IntPtr val = IntPtr.Zero;
            Wow64RevertWow64FsRedirection(ref val);
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = $"/c quser /SERVER:{serverName}";
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            List<QUser> users = new List<QUser>();
            string output = p.StandardOutput.ReadToEnd();

            return output;


        }

        public (string, string, string, string, string, string) GetOSFriendlyName(string remoteComputerNodeName)
        {
            string Caption = string.Empty;
            string Version = string.Empty;
            string OSArchitecture = string.Empty;
            string LastBootUpTime = string.Empty;
            string Organization = string.Empty;
            string NumberOfUsers = string.Empty;

            ConnectionOptions connection = new ConnectionOptions();
            ManagementScope scope = new ManagementScope("\\\\" + remoteComputerNodeName + "\\root\\CIMV2", connection);
            scope.Connect();
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            foreach (ManagementObject os in searcher.Get())
            {
                Caption = os["Caption"].ToString();
                Version = os["Version"].ToString();
                OSArchitecture = os["OSArchitecture"].ToString();
                LastBootUpTime = os["LastBootUpTime"].ToString();
                Organization = os["Organization"].ToString();
                NumberOfUsers = os["NumberOfUsers"].ToString();
                break;
            }
            return (Caption, Version, OSArchitecture, LastBootUpTime, Organization, NumberOfUsers);
        }

        public ComputerSystem GetComputerSystemInfo(string remoteComputerNodeName)
        {
            ComputerSystem _computerSystem = new ComputerSystem();

            ConnectionOptions connection = new ConnectionOptions();
            ManagementScope scope = new ManagementScope("\\\\" + remoteComputerNodeName + "\\root\\CIMV2", connection);
            scope.Connect();
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_ComputerSystem");

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            foreach (ManagementObject os in searcher.Get())
            {
                _computerSystem.PrimaryOwnerName = os["PrimaryOwnerName"].ToString();
                _computerSystem.TotalPhysicalMemory = $"{(int)Math.Round(Convert.ToDouble(os["TotalPhysicalMemory"].ToString()) / (1024 * 1024 * 1024))} GB";
                _computerSystem.NumberOfLogicalProcessors = os["NumberOfLogicalProcessors"].ToString();
                _computerSystem.NumberOfProcessors = os["NumberOfProcessors"].ToString();
                _computerSystem.Caption = os["Caption"].ToString();
                _computerSystem.DNSHostName = os["DNSHostName"].ToString();


                _computerSystem.Domain = os["Domain"].ToString();
                _computerSystem.Model = os["Model"].ToString();
                _computerSystem.Name = os["Name"].ToString();
                //_computerSystem.DNSHostName = os["DNSHostName"].ToString();


                break;
            }

            return _computerSystem;
        }

        public DiskDrive GetDiskDriveInfo(string remoteComputerNodeName)
        {
            DiskDrive _diskDrive = new DiskDrive();

            ConnectionOptions connection = new ConnectionOptions();
            ManagementScope scope = new ManagementScope("\\\\" + remoteComputerNodeName + "\\root\\CIMV2", connection);
            scope.Connect();
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_DiskDrive ");

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            foreach (ManagementObject os in searcher.Get())
            {
                _diskDrive.Size = $"{(int)Math.Round(Convert.ToDouble(os["Size"].ToString()) / (1024 * 1024 * 1024))} GB";
                _diskDrive.Partitions = Convert.ToInt32(os["Partitions"].ToString());

                break;
            }

            return _diskDrive;
        }

    }



    public class RemoteMachineUser
    {
        public string RemoteMachine { get; set; }
        public string Model { get; set; }
        public string TotalPhysicalMemory { get; set; }
        public string HardDiskSize { get; set; }
        public string NumberOfLogicalProcessors { get; set; }
        public string NumberOfProcessors { get; set; }
        public int HardDiskPartitions { get; set; }

        public string UserName { get; set; }
        public string SessionName { get; set; }
        public string Id { get; set; }
        public string State { get; set; }
        public string IdleTime { get; set; }
        public string LogonTime { get; set; }

        public string Caption { get; set; }
        public string Version { get; set; }
        public string OSArchitecture { get; set; }
        public string LastBootUpTime { get; set; }
        public string Organization { get; set; }
        public string NumberOfUsers { get; set; }

        public string PrimaryOwnerName { get; set; }
        public string DNSHostName { get; set; }
        //public string CaptionTemp { get; set; }
        public string PrimaryOwnerContact { get; set; }
        public string Domain { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }

    }


    public class QUser
    {
        public string UserName { get; set; }
        public string SessionName { get; set; }
        public string Id { get; set; }
        public string State { get; set; }
        public string IdleTime { get; set; }
        public string LogonTime { get; set; }

        public QUser(string line, string columnsString)
        {
            UserName = line.Substring(columnsString.IndexOf("USERNAME"), columnsString.IndexOf("SESSIONNAME") - 1);
            SessionName = line.Substring(columnsString.IndexOf("SESSIONNAME"), columnsString.IndexOf("ID") - columnsString.IndexOf("SESSIONNAME") - 1);
            Id = line.Substring(columnsString.IndexOf("ID"), columnsString.IndexOf("STATE") - columnsString.IndexOf("ID") - 1);
            State = line.Substring(columnsString.IndexOf("STATE"), columnsString.IndexOf("IDLE TIME") - columnsString.IndexOf("STATE") - 1);
            IdleTime = line.Substring(columnsString.IndexOf("IDLE TIME"), columnsString.IndexOf("LOGON TIME") - columnsString.IndexOf("IDLE TIME") - 1);
            LogonTime = line.Substring(columnsString.IndexOf("LOGON TIME"), line.Length - columnsString.IndexOf("LOGON TIME"));

            LogonTime = GetDateString(LogonTime);

            UserName = UserName.Trim();
            SessionName = SessionName.Trim();
            Id = Id.Trim();
            State = State.Trim().ToUpper().Equals("DISC") ? "Disconnected" : State.Trim();
            IdleTime = IdleTime.Trim();
            LogonTime = LogonTime.Trim();
        }

        public string GetDateString(string datetime)
        {
            string result = datetime;
            try
            {
                result = Convert.ToDateTime(datetime).ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                result = datetime;
            }

            return result;
        }
    }

    public class Log
    {
        private StringBuilder LogMessages = new StringBuilder();

        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                LogMessages.AppendLine(_status);
                Console.WriteLine($"[{DateTime.Now}] {_status}");
            }
        }

        //public string StatusInSameLine
        //{
        //    get { return _status; }
        //    set
        //    {
        //        _status = value;
        //        LogMessages.AppendLine(_status);
        //        Console.WriteLine($"{DateTime.Now } {_status}");
        //    }
        //}

        public string GetLogMessages()
        {
            return LogMessages.ToString();
        }

        public string WriteLog()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, $"Log_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}");

            File.WriteAllText(path, LogMessages.ToString());

            return path;
        }

    }

    public class ComputerSystem
    {
        public string PrimaryOwnerName { get; set; }
        public string TotalPhysicalMemory { get; set; }
        public string NumberOfLogicalProcessors { get; set; }
        public string NumberOfProcessors { get; set; }
        public string DNSHostName { get; set; }
        public string Caption { get; set; }
        public string PrimaryOwnerContact { get; set; }
        public string Domain { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
    }

    public class DiskDrive
    {
        public string Availability { get; set; }
        public string BytesPerSector { get; set; }
        public string Capabilities { get; set; }
        public string CapabilityDescriptions { get; set; }
        public string Caption { get; set; }
        public string CompressionMethod { get; set; }
        public string ConfigManagerErrorCode { get; set; }
        public Boolean ConfigManagerUserConfig { get; set; }
        public string CreationClassName { get; set; }
        public string DefaultBlockSize { get; set; }
        public string Description { get; set; }
        public string DeviceID { get; set; }
        public Boolean ErrorCleared { get; set; }
        public string ErrorDescription { get; set; }
        public string ErrorMethodology { get; set; }
        public string FirmwareRevision { get; set; }
        public string Index { get; set; }
        public DateTime InstallDate { get; set; }
        public string InterfaceType { get; set; }
        public string LastErrorCode { get; set; }
        public string Manufacturer { get; set; }
        public string MaxBlockSize { get; set; }
        public string MaxMediaSize { get; set; }
        public Boolean MediaLoaded { get; set; }
        public string MediaType { get; set; }
        public string MinBlockSize { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public Boolean NeedsCleaning { get; set; }
        public string NumberOfMediaSupported { get; set; }
        public int Partitions { get; set; }
        public string PNPDeviceID { get; set; }
        public string PowerManagementCapabilities { get; set; }
        public Boolean PowerManagementSupported { get; set; }
        public string SCSIBus { get; set; }
        public string SCSILogicalUnit { get; set; }
        public string SCSIPort { get; set; }
        public string SCSITargetId { get; set; }
        public string SectorsPerTrack { get; set; }
        public string SerialNumber { get; set; }
        public string Signature { get; set; }
        public String Size { get; set; }
        public string Status { get; set; }
        public string StatusInfo { get; set; }
        public string SystemCreationClassName { get; set; }
        public string SystemName { get; set; }
        public string TotalCylinders { get; set; }
        public string TotalHeads { get; set; }
        public string TotalSectors { get; set; }
        public string TotalTracks { get; set; }
        public string TracksPerCylinder { get; set; }

    }
}
