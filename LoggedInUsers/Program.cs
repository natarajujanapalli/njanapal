using njanapal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LoggedInUsers
{
    class Program
    {
        static void Main(string[] args)
        {

            bool isDisplayState = Convert.ToBoolean(ConfigurationManager.AppSettings["DisplayState"].ToString());
            bool isDisplayIdleTime = Convert.ToBoolean(ConfigurationManager.AppSettings["DisplayIdleTime"].ToString());
            bool isDisplayLogonTime = Convert.ToBoolean(ConfigurationManager.AppSettings["DisplayLogonTime"].ToString());
            bool isDisplayID = Convert.ToBoolean(ConfigurationManager.AppSettings["DisplayID"].ToString());
            bool isDisplaySessionName = Convert.ToBoolean(ConfigurationManager.AppSettings["DisplaySessionName"].ToString());

            Console.WriteLine();
            //Console.WriteLine($"Version : {Assembly.GetExecutingAssembly().GetName().Version}");
            Console.WriteLine($"Product Version : {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion}");
            Console.WriteLine("Note : Run 'AllowRemoteRPC.reg' registry file on the remote machine when remote machine state field is blank.");
            string option = "Y";
            Log log = new Log();

            do
            {
                string fileFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RemoteMachines.txt");

                if (!File.Exists(fileFullPath))
                {
                    Console.WriteLine($" File doesnot exist at {fileFullPath} location.");
                    return;
                }

                List<string> machines = new List<string>();
                machines = File.ReadLines(fileFullPath).Where(r => !string.IsNullOrWhiteSpace(r) && !r.Trim().StartsWith("--")).Distinct().OrderBy(r => r).ToList();

                if (machines == null || machines.Count == 0)
                {
                    Console.WriteLine($"No remote machine's configured in the {fileFullPath} file.");
                    return;
                }

                RemoteMachine remote = new RemoteMachine(log);

                //var items1 = remote.GetLoggedInUsersByMachine(machines);
                //foreach (var item in items1)
                //{
                //    Console.WriteLine($"{item.RemoteMachine.ToUpper()} : ");
                //    foreach (var user in item.Users)
                //        Console.WriteLine($"\t{user}");
                //}
                //
                //Console.WriteLine();
                //Console.WriteLine();


                //var items2 = remote.GetLoggedInUsersByUserName(machines);
                //foreach (var item in items2)
                //{
                //    Console.WriteLine();
                //    Console.WriteLine($"{item.User.ToUpper()} : ");
                //
                //    foreach (var remoteMachine in item.RemoteMachines)
                //    {
                //        Console.WriteLine($"\t{remoteMachine.RemoteMachine}");
                //
                //        Console.WriteLine($"\t\t{remoteMachine.}");
                //        Console.WriteLine($"\t\t{remoteMachine}");
                //
                //    }
                //    Console.WriteLine();
                //    Console.WriteLine();
                //}
                //
                //Console.WriteLine();
                //Console.WriteLine();

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("\t1. Display details by User");
                Console.WriteLine("\t2. Display details by Remote Mchine");
                Console.WriteLine();
                Console.Write("\tEnter your option : ");
                var ch = Console.ReadLine();
                ch = ch.Trim();

                if (!ch.Equals("1") && !ch.Equals("2"))
                    return;

                Console.WriteLine();
                log.Status = "Started";
                var userDetails = remote.GetLoggedInUsers(machines);
                log.Status = "Completed";

                if (ch.Equals("1"))
                {
                    var users = userDetails.Select(r => r.UserName).Distinct().OrderBy(r => r).ToList();
                    foreach (string user in users)
                    {
                        if (user == null || string.IsNullOrWhiteSpace(user))
                            continue;

                        Console.WriteLine();
                        Console.WriteLine($"\t\t {user.ToUpper()} : ");

                        var details = userDetails.Where(r => r.UserName.ToLower().Equals(user.ToLower())).OrderBy(r => r.RemoteMachine).ToList();
                        if (details != null)
                        {
                            foreach (var d in details)
                            {
                                Console.WriteLine($"\t\t\t {d.RemoteMachine.ToUpper()}");

                                if (isDisplayState)
                                    Console.WriteLine($"\t\t\t\t State          : {d.State}");

                                if (isDisplayIdleTime)
                                    Console.WriteLine($"\t\t\t\t Idle Time      : {d.IdleTime}");

                                if (isDisplayLogonTime)
                                    Console.WriteLine($"\t\t\t\t Logon Time     : {d.LogonTime}");

                                if (isDisplayID)
                                    Console.WriteLine($"\t\t\t\t ID             : {d.Id}");

                                if (isDisplaySessionName)
                                    Console.WriteLine($"\t\t\t\t Session Name   : {d.SessionName}");

                                //Console.WriteLine();
                            }
                        }
                    }
                }
                else if (ch.Equals("2"))
                {
                    var remoteMachines = userDetails.Select(r => r.RemoteMachine).Distinct().OrderBy(r => r).ToList();
                    foreach (string remoteMachine in remoteMachines)
                    {
                        if (remoteMachine == null || string.IsNullOrWhiteSpace(remoteMachine))
                            continue;

                        Console.WriteLine();
                        Console.WriteLine($"\t\t {remoteMachine.ToUpper()} : ");

                        var details = userDetails.Where(r => r.RemoteMachine.ToLower().Equals(remoteMachine.ToLower())).OrderBy(r => r.UserName).ToList();
                        if (details != null)
                        {
                            foreach (var d in details)
                            {
                                if (string.IsNullOrWhiteSpace(d.UserName) && string.IsNullOrWhiteSpace(d.State) && string.IsNullOrWhiteSpace(d.IdleTime) && string.IsNullOrWhiteSpace(d.LogonTime) && string.IsNullOrWhiteSpace(d.Id) && string.IsNullOrWhiteSpace(d.SessionName))
                                    continue;

                                Console.WriteLine($"\t\t\t {d.UserName.ToUpper()}");
                                if (isDisplayState)
                                    Console.WriteLine($"\t\t\t\t State          : {d.State}");

                                if (isDisplayIdleTime)
                                    Console.WriteLine($"\t\t\t\t Idle Time      : {d.IdleTime}");

                                if (isDisplayLogonTime)
                                    Console.WriteLine($"\t\t\t\t Logon Time     : {d.LogonTime}");

                                if (isDisplayID)
                                    Console.WriteLine($"\t\t\t\t ID             : {d.Id}");

                                if (isDisplaySessionName)
                                    Console.WriteLine($"\t\t\t\t Session Name   : {d.SessionName}");

                                //Console.WriteLine();
                            }
                        }
                    }


                }


                Console.WriteLine();
                Console.WriteLine();

                Console.Write("Do you re-scan it again (Y/N)? : ");
                option = Console.ReadLine();
            } while (option.ToString().ToUpper().Equals("Y"));

            Console.WriteLine();
            //Console.ReadLine();
        }

        private static List<string> GetRemoteMachines()
        {
            string fileFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RemoteMachines.txt");

            return File.ReadLines(fileFullPath).Distinct().Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
        }

        private static void Checkuser(string remoteMachineName)
        {
            if (string.IsNullOrWhiteSpace(remoteMachineName))
            {
                Console.WriteLine("usage: checkuser <machinename>");
                return;
            }

            string strStatus = "There is no user logged into " + remoteMachineName;

            Process[] ps = null;

            try
            {
                ps = Process.GetProcesses(remoteMachineName);
            }
            catch
            {
                //GetProcesses will fail if the machine is off
                strStatus = "Could not get process list from " + remoteMachineName;
            }

            if (ps != null)
            {
                foreach (Process p in ps)
                {

                    if (p.ProcessName.Equals("explorer"))
                        strStatus = "There is a user logged into " + remoteMachineName;

                }
            }

            Console.WriteLine(strStatus);

        }


        //public static string whoisLoggedIn(string strComputer)
        //{
        //
        //    try
        //    {
        //
        //
        //
        //        ConnectionOptions connection = new ConnectionOptions();
        //
        //
        //        ManagementScope scope = new ManagementScope("\\\\" + strComputer + "\\root\\CIMV2", connection);
        //        scope.Connect();
        //
        //
        //
        //        ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Process WHERE Name = 'explorer.exe'");
        //
        //
        //        ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
        //
        //
        //        foreach (ManagementObject queryObj in searcher.Get())
        //        {
        //
        //
        //
        //            ManagementPath path = new ManagementPath("Win32_Process.Handle='" + queryObj["Handle"] + "'");
        //
        //
        //            ManagementObject classInstance = new ManagementObject(scope, path, null);
        //
        //
        //            ManagementBaseObject outParams = classInstance.InvokeMethod("GetOwner", null, null);
        //
        //
        //            return (outParams["User"].ToString());
        //        }
        //
        //    }
        //
        //
        //
        //    catch (ManagementException err)
        //    {
        //
        //
        //
        //        return ("NA");
        //    }
        //
        //
        //
        //    catch (System.UnauthorizedAccessException unauthorizedErr)
        //    {
        //
        //
        //
        //        return ("NA");
        //    }
        //
        //
        //
        //    catch (Exception ex)
        //    {
        //
        //
        //
        //        return ("NA");
        //    }
        //
        //
        //
        //    return "NA";
        //}

    }
}
