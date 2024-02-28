using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace nataraju
{
    internal class ComputerSystem
    {
        public string? PrimaryOwnerName { get; set; }
        public string? TotalPhysicalMemory { get; set; }
        public string? NumberOfLogicalProcessors { get; set; }
        public string? NumberOfProcessors { get; set; }
        public string? DNSHostName { get; set; }
        public string? Caption { get; set; }
        public string? PrimaryOwnerContact { get; set; }
        public string? Domain { get; set; } 
        public string? Model { get; set; }
        public string? Name { get; set; }

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

                break;
            }

            return _computerSystem;
        }
    }
}
