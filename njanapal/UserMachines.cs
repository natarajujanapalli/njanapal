using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace njanapal
{
 
    public class Node
    {
        public string NodeName { get; set; }
        public string HostName { get; set; }
        public string Purpose { get; set; }
    }
    
    public class UserMachines
    {
        public string Owner { get; set; }
        public List<Node> MachineNames { get; set; }

        public UserMachines()
        {
            MachineNames = new List<Node>();
        }
    }

}
