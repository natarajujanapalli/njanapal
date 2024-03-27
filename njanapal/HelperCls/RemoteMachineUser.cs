using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace njanapal
{
    public class RemoteMachineUser
    {
        public string RemoteMachine { get; set; }
        public string Status { get; set; }
        public string HostName { get; set; }

        public string Model { get; set; }
        public string TotalPhysicalMemory { get; set; }
        public string HardDiskSize { get; set; }
        public string NumberOfLogicalProcessors { get; set; }
        public string NumberOfProcessors { get; set; }
        public int HardDiskPartitions { get; set; }
        public string DiskSpaceSize { get; set; }
        public string DiskFreeSpace { get; set; }
        public string DiskFreeSpacePercentage { get; set; }

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
        //public string CaptionTemp { get; set; }
        public string PrimaryOwnerContact { get; set; }
        public string Domain { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }

    }

}
