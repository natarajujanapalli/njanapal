using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;

namespace njanapal
{
    public class RemoteMachine
    {
        //set HKLM\SYSTEM\CurrentControlSet\Control\Terminal Server\AllowRemoteRPC to 1

        public Log Log { get; set; }

        public string NumberOfLogicalProcessors { get; set; }
        public string Caption { get; set; }
        public string DNSHostName { get; set; }

        public RemoteMachine(Log log)
        {
            this.Log = log;
        }

        public List<RemoteMachineUser> LoggedUers { get; set; } = new List<RemoteMachineUser>();

        public List<(string User, string Message, List<RemoteMachineUser> RemoteMachines)> GetLoggedInUsersByUserName(List<string> machines = null)
        {
            List<(string User, string Message, List<RemoteMachineUser> RemoteMachines)> result = new List<(string, string, List<RemoteMachineUser>)>();

            List<RemoteMachineUser> info = this.LoggedUers;

            if (machines != null) // && this.LoggedUers != null && this.LoggedUers.Count > 0)
                info = GetLoggedInUsers(machines);

            var users = info.Select(r => r.UserName).Distinct();

            foreach (string user in users)
            {
                if (user == null || string.IsNullOrWhiteSpace(user))
                    continue;

                string message = string.Empty; //info.Where(r => r.User.Equals(user)).Select(r => r.Message).Distinct().FirstOrDefault().ToString();
                List<RemoteMachineUser> nodenames = info.Where(r => r.UserName != null && r.UserName.Equals(user)).Select(r => r).Distinct().ToList();
                result.Add((user, message, nodenames));
            }

            return result;
        }

        public List<(string RemoteMachine, string Message, List<string> Users)> GetLoggedInUsersByMachine(List<string> machines = null)
        {
            List<(string RemoteMachine, string Message, List<string> Users)> result = new List<(string, string, List<string>)>();

            List<RemoteMachineUser> info = this.LoggedUers;

            if (machines != null)
                info = GetLoggedInUsers(machines);

            var machinesTemp = info.Select(r => r.RemoteMachine).Distinct();

            foreach (string machine in machinesTemp)
            {
                string message = info.Where(r => r.RemoteMachine.Equals(machine)).Select(r => r.Message).Distinct().FirstOrDefault().ToString();
                List<string> users = info.Where(r => r.RemoteMachine.Equals(machine)).Select(r => r.UserName).Distinct().ToList();
                result.Add((machine, message, users));
            }

            return result;
        }

        public List<RemoteMachineUser> GetLoggedInUsers(List<string> machines)
        {
            int i = 1;
            foreach (string machineNodeName in machines)
            {
                if (string.IsNullOrWhiteSpace(machineNodeName))
                    continue;

                this.Log.Status = $"Processing Machine : {i++,3} / {machines.Count} - {machineNodeName}";

                var (message, users, usersDetails) = GetLoggedInUsers(machineNodeName);

                if (users == null || users.Count == 0)
                    LoggedUers.Add(new RemoteMachineUser { RemoteMachine = machineNodeName, Message = message, UserName = string.Empty, SessionName = string.Empty, Id = string.Empty, State = string.Empty, IdleTime = string.Empty, LogonTime = string.Empty });
                else
                {
                    foreach (string user in users)
                    {
                        var userDetails = usersDetails?.Where(r => r.UserName.ToLower().Equals(user.ToLower())).FirstOrDefault();
                        if (userDetails == null)
                            LoggedUers.Add(new RemoteMachineUser { RemoteMachine = machineNodeName, Message = message, UserName = user, SessionName = string.Empty, Id = string.Empty, State = string.Empty, IdleTime = string.Empty, LogonTime = string.Empty });
                        else
                        {
                            LoggedUers.Add(new RemoteMachineUser { RemoteMachine = machineNodeName, Message = message, UserName = userDetails.UserName, SessionName = userDetails.SessionName, Id = userDetails.Id, State = userDetails.State, IdleTime = userDetails.IdleTime, LogonTime = userDetails.LogonTime });
                        }
                    }
                }

            }

            return this.LoggedUers;
        }

        public (string, List<string>, List<User>) GetLoggedInUsers(string remoteComputerNodeName)
        {
            List<string> users = new List<string>();
            List<User> usersDetails = new List<User>();
            string message = string.Empty;

            try
            {
                ConnectionOptions connection = new ConnectionOptions();
                ManagementScope scope = new ManagementScope("\\\\" + remoteComputerNodeName + "\\root\\CIMV2", connection);
                scope.Connect();

                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Process WHERE Name = 'explorer.exe'");

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    ManagementPath path = new ManagementPath("Win32_Process.Handle='" + queryObj["Handle"] + "'");
                    ManagementObject classInstance = new ManagementObject(scope, path, null);
                    ManagementBaseObject outParams = classInstance.InvokeMethod("GetOwner", null, null);

                    users.Add(outParams["User"].ToString());
                }

                usersDetails = GetQUserInfo(remoteComputerNodeName);

            }
            catch (ManagementException err)
            {
                message = $"ManagementException. \r\nDetails: {err.ToString()}";
            }
            catch (System.UnauthorizedAccessException unauthorizedErr)
            {
                message = $"Doesn't have permissions to access the machine, \r\nDetails : {unauthorizedErr.ToString()}";
            }
            catch (Exception ex)
            {
                //message = $"Exception Details: {ex.ToString()}";
                message = $"The RPC server is unavailable. \r\nDetails: {ex.ToString()}";
            }

            return (message, users, usersDetails);
        }


        public List<User> GetQUserInfo(string serverName)
        {
            string output = GetQUserInfo32(serverName);
            if (string.IsNullOrWhiteSpace(output))
                output = GetQUserInfo64(serverName);

            if (string.IsNullOrWhiteSpace(output))
                return null;

            List<User> users = new List<User>();
            string columnsString = output.Split(new string[] { "\r\n" }, StringSplitOptions.None).Where(r => r.Contains("USERNAME")).FirstOrDefault();
            string[] lines = output.Split(new string[] { "\r\n" }, StringSplitOptions.None).Where(r => !r.Contains("USERNAME")).ToArray();

            int i = 0;
            foreach (string line in lines)
            {
                if (line.Contains("USERNAME") || string.IsNullOrWhiteSpace(line))
                    continue;

                users.Add(new User(line, columnsString));
            }

            return users;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Wow64DisableWow64FsRedirection(ref IntPtr ptr);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Wow64EnableWow64FsRedirection(ref IntPtr ptr);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int Wow64RevertWow64FsRedirection(ref IntPtr ptr);

        public string GetQUserInfo32(string serverName)
        {
            IntPtr val = IntPtr.Zero;
            Wow64DisableWow64FsRedirection(ref val);
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = $"/c quser /SERVER:{serverName}";
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            p.WaitForExit();

            string output = p.StandardOutput.ReadToEnd();

            return output;
        }


        public string GetQUserInfo64(string serverName)
        {
            IntPtr val = IntPtr.Zero;
            Wow64RevertWow64FsRedirection(ref val);
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = $"/c quser /SERVER:{serverName}";
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            List<User> users = new List<User>();
            string output = p.StandardOutput.ReadToEnd();

            return output;


        }




  

    }


}
