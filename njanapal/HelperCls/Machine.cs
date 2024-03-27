using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace njanapal
{
    public class Machine
    {
        public string MachineName { get; set; }
        public string Owner { get; set; }
        public string Status { get; set; }

        public string HostName { get; set; }
        public string Domain { get; set; }
        public string Model { get; set; }
        public string Name { get; set; }
        public string NumberOfLogicalProcessors { get; set; }
        public string NumberOfProcessors { get; set; }
        public string TotalPhysicalMemory { get; set; }


        public string DiskSpaceSize { get; set; }
        public string DiskFreeSpace { get; set; }
        public string DiskFreeSpacePercentage { get; set; }


        public string Caption { get; set; }
        public string Version { get; set; }
        public string OSArchitecture { get; set; }
        public string LastBootUpTime { get; set; }
        public string Organization { get; set; }
        public string NumberOfUsers { get; set; }

        public string HardDiskPartitions { get; set; }
        public string HardDiskSize { get; set; }


        public string Message { get; set; }

        public ObservableCollection<User> Users { get; set; }

    }
}
