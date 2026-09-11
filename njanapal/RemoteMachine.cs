using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
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

                    if (!users.Contains(outParams["User"].ToString()))
                    {
                        users.Add($"{outParams["User"].ToString()}");

                        //var temp = GetUserDisplayName(outParams["User"].ToString());
                        //if (string.IsNullOrWhiteSpace(temp))
                        //    users.Add($"{outParams["User"].ToString()}");
                        //else
                        //    users.Add($"{outParams["User"].ToString()} ({temp})");
                    }
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


        // Remote Sign Off User
        public (bool Success, string Output, string Error) SignOffUser(string serverName, string sessionId)
        {
            if (string.IsNullOrWhiteSpace(serverName) || string.IsNullOrWhiteSpace(sessionId))
                return (false, string.Empty, "Server name and session ID are required.");

            try
            {
                IntPtr val = IntPtr.Zero;
                Wow64DisableWow64FsRedirection(ref val);

                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = $"/c logoff {sessionId} /server:{serverName}";
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                string error = p.StandardError.ReadToEnd();
                p.WaitForExit();

                Wow64RevertWow64FsRedirection(ref val);

                return (p.ExitCode == 0, output, error);
            }
            catch (Exception ex)
            {
                return (false, string.Empty, ex.Message);
            }
        }

        public (bool Success, string Message) TurnOnVirtualMachine(string hostName, string vmName)
        {
            if (string.IsNullOrWhiteSpace(hostName) || string.IsNullOrWhiteSpace(vmName))
                return (false, "Host name and VM name are required.");

            try
            {
                ConnectionOptions op = new ConnectionOptions();
                ManagementScope scope = new ManagementScope("\\\\" + hostName + "\\root\\virtualization\\v2", op);
                scope.Connect();

                ObjectQuery oq = new ObjectQuery($"SELECT * FROM Msvm_ComputerSystem WHERE ElementName = '{vmName}'");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, oq);

                foreach (ManagementObject vm in searcher.Get())
                {
                    // EnabledState: 2 = Running, 3 = Off, 32768 = Paused
                    if (Convert.ToUInt16(vm["EnabledState"]) == 2)
                        return (true, "Already running.");

                    // RequestStateChange: 2 = Turn On
                    ManagementBaseObject inParams = vm.GetMethodParameters("RequestStateChange");
                    inParams["RequestedState"] = 2;
                    ManagementBaseObject outParams = vm.InvokeMethod("RequestStateChange", inParams, null);

                    // ReturnValue: 0 = Completed, 4096 = Job started
                    uint returnValue = Convert.ToUInt32(outParams["ReturnValue"]);
                    if (returnValue == 0 || returnValue == 4096)
                        return (true, "VM is turning on.");

                    return (false, $"RequestStateChange failed with return value: {returnValue}.");
                }

                return (false, $"VM '{vmName}' not found on host '{hostName}'.");
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(ex.Message) && ex.Message.Trim().Equals("Access is denied.", StringComparison.OrdinalIgnoreCase))
                    return (false, $"Access denied. Your account don’t have sufficient permissions on '{hostName}' to perform this action.");

                return (false, ex.Message);
            }
        }

        // Remote Restart Machine
        public bool Reboot(string machineNodeName, string userName = null, string password = null)
        {
            return InvokeOSMethod(machineNodeName, "Reboot", userName, password);
        }

        // Remote Shutdown Machine
        public bool Shutdown(string machineNodeName, string userName = null, string password = null)
        {
            return InvokeOSMethod(machineNodeName, "ShutDown", userName, password);
        }

        // Remote Log Off Machine
        private bool InvokeOSMethod(string machineName, string methodName, string userName = null, string password = null)
        {
            try
            {
                ConnectionOptions op = new ConnectionOptions();
                if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
                {
                    op.Username = userName;
                    op.Password = password;
                }

                ManagementScope scope = new ManagementScope("\\\\" + machineName + "\\root\\cimv2", op);
                scope.Connect();

                ObjectQuery oq = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                ManagementObjectSearcher query = new ManagementObjectSearcher(scope, oq);
                ManagementObjectCollection queryCollection = query.Get();
                foreach (ManagementObject obj in queryCollection)
                {
                    obj.InvokeMethod(methodName, null);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public LoggedInUser CurrentLoggedInAccount()
        {
            using (var context = new PrincipalContext(ContextType.Machine))
            {
                // Find the current user
                var user = UserPrincipal.Current;

                return new LoggedInUser
                {
                    DisplayName = user.DisplayName,
                    EmailAddress = user.EmailAddress,
                    Guid = user.Guid,
                    LastLogon = user.LastLogon.ToString()
                };
                //Console.WriteLine("Full Name: " + user.DisplayName);
                //Console.WriteLine("Email: " + user.EmailAddress);
                //Console.WriteLine("GUID: " + user.Guid);
                //Console.WriteLine("Last Login: " + user.LastLogon);
            }

            return null;
        }

    }

    public class LoggedInUser
    {
        public string UserName { get; set; }
        public string Domain { get; set; }
        public string LogonId { get; set; }
        public string LogonType { get; set; }  // e.g., Interactive, Network
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public Guid? Guid { get; set; }
        public string LastLogon { get; set; }
    }
}