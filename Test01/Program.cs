using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using njanapal;
using WindowsUpdates;
using static System.Net.Mime.MediaTypeNames;

namespace Test01
{
    public class vms
    {
        public string Owner { get; set; }
        public string VM { get; set; }
    }

    class Program
    {

        public static void Main()
        {
            var data = File.ReadAllLines(@"C:\Users\njanapal\Documents\Book1.csv");

            List<UserMachines> Machines = new List<UserMachines>();
            List<vms> _vms = new List<vms>();

            foreach (string line in data)
            {
                var d = line.Split(',');

                _vms.Add(new vms { Owner = d[0], VM = d[1] });

            }

            var distinctOwners = _vms.Select(r => r.Owner).Distinct();

            foreach (var owner in distinctOwners)
            {
                var um = new UserMachines { Owner = owner };
                foreach (var m in _vms.Where(r => r.Owner == owner))
                {
                    um.MachineNames.Add(m.VM);
                }

                Machines.Add(um);
            }


            


            var res = JsonSerializer.Serialize(Machines);

            //RegEdit rg = new RegEdit();
            //rg.GetRegistryKey("", "");

            //bool pingable = false;
            //Ping pinger = null;
            //
            //try
            //{
            //    pinger = new Ping();
            //    PingReply reply = pinger.Send("37INTSQL-WS2019");
            //    pingable = reply.Status == IPStatus.Success;
            //}
            //catch (PingException ex)
            //{
            //    // Discard PingExceptions and return false;
            //}
            //finally
            //{
            //    if (pinger != null)
            //    {
            //        pinger.Dispose();
            //    }
            //}

            //foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            //{
            //    Console.WriteLine("Number Of Physical Processors: {0} ", item["NumberOfProcessors"]);
            //}
            //Console.ReadLine();



            //static void Main(string[] args)
            //{
            //    WindowsUpdate updates = new WindowsUpdate();
            //    File.WriteAllText( Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"WindowsUpdates_{DateTime.Now.ToString("yyyyMMddHHmmsss")}.txt"), updates.GetUpdates());
            //
            //    //string temp = string.Concat(@"C:\PROGRA~1\ILeads82\RMS\ALULTRA\", "~PreIBR.log");
            //    //Console.WriteLine(temp); ;
            //
            //    //temp = string.Concat(@"C:\PROGRA~1\ILeads82\RMS\ALULTRA", "~PreIBR.log");
            //    //Console.WriteLine(temp); ;
            //
            //    //temp = Path.Combine(@"C:\PROGRA~1\ILeads82\RMS\ALULTRA", "~PreIBR.log");
            //    //Console.WriteLine(temp); ;
            //
            //
            //    //RemoteMachine rm = new RemoteMachine(new Log());
            //
            //    //var result = rm.GetOSFriendlyName("in-hsi1099a");
            //    //Console.WriteLine(result.Item1);
            //    //Console.WriteLine(result.Item2);
            //    //Console.WriteLine(result.Item3);
            //    //Console.WriteLine(result.Item4);
            //    //Console.WriteLine(result.Item5);
            //    //Console.WriteLine(result.Item6);
            //}
        }
    }
}
