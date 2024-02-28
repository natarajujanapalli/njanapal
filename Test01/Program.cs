using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using njanapal;
using WindowsUpdates;

namespace Test01
{
    class Program
    {
        public static void Main()
        {

            //RegEdit rg = new RegEdit();
            //rg.GetRegistryKey("", "");

            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send("37INTSQL-WS2019");
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException ex)
            {
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

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
