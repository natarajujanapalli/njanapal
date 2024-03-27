using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace njanapal
{

    public class UserMachines
    {
        public string Owner { get; set; }
        public List<string> MachineNames { get; set; }
        public UserMachines()
        {
            MachineNames = new List<string>();
        }
    }

}
