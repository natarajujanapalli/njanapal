using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nataraju.HelperCls
{
    public class RemoteMachineUser
    {
        public string RemoteMachine { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string TotalPhysicalMemory { get; set; } = string.Empty;
        public string HardDiskSize { get; set; } = string.Empty;
        public string NumberOfLogicalProcessors { get; set; } = string.Empty;
        public string NumberOfProcessors { get; set; } = string.Empty;
        public int HardDiskPartitions { get; set; } = 0;

        public string Message { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string SessionName { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string IdleTime { get; set; } = string.Empty;
        public string LogonTime { get; set; } = string.Empty;

        public string Caption { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string OSArchitecture { get; set; } = string.Empty;
        public string LastBootUpTime { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public string NumberOfUsers { get; set; } = string.Empty;

        public string PrimaryOwnerName { get; set; } = string.Empty;
        public string DNSHostName { get; set; } = string.Empty;
        //public string CaptionTemp { get; set; }
        public string PrimaryOwnerContact { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

    }

}
