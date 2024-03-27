using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace njanapal
{
    public class MachineHandler
    {
        public ObservableCollection<Machine> GetRemoteMachineNames(string filePath)
        {
            ObservableCollection<Machine> data = new ObservableCollection<Machine>();
            string text = string.Empty;

            if (string.IsNullOrWhiteSpace(filePath))
                text = File.ReadAllText(@"./RemoteMachines.json");
            else
                text = File.ReadAllText(filePath);

            //var machine = JsonSerializer.Deserialize<ObservableCollection<Machine>>(text); //.Deserialize<Machine>(text);
            var machines = JsonSerializer.Deserialize<ObservableCollection<UserMachines>>(text); //.Deserialize<Machine>(text);

            foreach(var u in machines)
            {
                foreach (var m in u.MachineNames)
                    data.Add(new Machine { MachineName = m, Owner = u.Owner });
            }

            return data;
        }

        public bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                    pinger.Dispose();
            }

            return pingable;
        }


        //public List<LogicalDisk> GetRemoteDiskSpace(List<string> remoteMachines)
        //{
        //    List<LogicalDisk> _logicalDisk = new List<LogicalDisk>();
        //
        //    ConnectionOptions opt = new ConnectionOptions();
        //    ObjectQuery oQuery = new ObjectQuery("SELECT Size, FreeSpace, Name, FileSystem FROM Win32_LogicalDisk WHERE DriveType = 3");
        //
        //    foreach (string sLine in remoteMachines)
        //    {
        //        if (string.IsNullOrWhiteSpace(sLine))
        //            continue;
        //
        //        ManagementScope scope = new ManagementScope("\\\\" + sLine + "\\root\\cimv2", opt);
        //        ManagementObjectSearcher moSearcher = new ManagementObjectSearcher(scope, oQuery);
        //        ManagementObjectCollection collection = moSearcher.Get();
        //
        //        decimal size = 0;
        //        decimal freeSpace = 0;
        //
        //        foreach (ManagementObject res in collection)
        //        {
        //            size += Convert.ToDecimal(res["Size"]) / 1024 / 1024 / 1024;
        //            freeSpace += Convert.ToDecimal(res["FreeSpace"]) / 1024 / 1024 / 1024;
        //        }
        //
        //        _logicalDisk.Add(new LogicalDisk
        //        {
        //            Name = sLine,
        //            Size = $"{Decimal.Round(size, 2)} GB",
        //            FreeSpace = $"{Decimal.Round(freeSpace, 2)} GB",
        //            FreeSpacePercentage = $"{Decimal.Round(freeSpace / size, 2) * 100} %"
        //        });
        //    }
        //
        //    return _logicalDisk;
        //}

        public LogicalDisk GetRemoteDiskSpace(string remoteMachine)
        {
            LogicalDisk _logicalDisk = new LogicalDisk();

            if (string.IsNullOrWhiteSpace(remoteMachine))
                return _logicalDisk;

            ConnectionOptions opt = new ConnectionOptions();
            ObjectQuery oQuery = new ObjectQuery("SELECT * FROM Win32_LogicalDisk WHERE DriveType = 3");


            ManagementScope scope = new ManagementScope("\\\\" + remoteMachine + "\\root\\cimv2", opt);
            ManagementObjectSearcher moSearcher = new ManagementObjectSearcher(scope, oQuery);
            ManagementObjectCollection collection = moSearcher.Get();

            decimal size = 0;
            decimal freeSpace = 0;

            foreach (ManagementObject res in collection)
            {
                size += Convert.ToDecimal(res["Size"]) / 1024 / 1024 / 1024;
                freeSpace += Convert.ToDecimal(res["FreeSpace"]) / 1024 / 1024 / 1024;
            }

            _logicalDisk.Name = remoteMachine;
            _logicalDisk.Size = $"{Decimal.Round(size, 2)} GB".PadLeft(9, '0');
            _logicalDisk.FreeSpace = $"{Decimal.Round(freeSpace, 2)} GB".PadLeft(9, '0');
            _logicalDisk.FreeSpacePercentage = $"{Decimal.Round(freeSpace / size, 2) * 100} %".PadLeft(7, '0'); 


            return _logicalDisk;
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
                _computerSystem.TotalPhysicalMemory = $"{(int)Math.Round(Convert.ToDouble(os["TotalPhysicalMemory"].ToString()) / (1024 * 1024 * 1024), 2)} GB".PadLeft(5, '0');
                _computerSystem.NumberOfLogicalProcessors = os["NumberOfLogicalProcessors"].ToString().PadLeft(2, '0');
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
                LastBootUpTime = $"{LastBootUpTime.Substring(0, 4)}-{LastBootUpTime.Substring(4, 2)}-{LastBootUpTime.Substring(6, 2)} {LastBootUpTime.Substring(8, 2)}:{LastBootUpTime.Substring(10, 2)}:{LastBootUpTime.Substring(12, 2)}";

                Organization = os["Organization"].ToString();
                NumberOfUsers = os["NumberOfUsers"].ToString();
                break;
            }
            return (Caption, Version, OSArchitecture, LastBootUpTime, Organization, NumberOfUsers);
        }

    }
}
