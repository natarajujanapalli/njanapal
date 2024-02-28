using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace njanapal
{
    public class cmd
    {

        public void RunCommandAsAdmin(string path, string command)
        {
            //Process p = new Process();
            //p.StartInfo.UseShellExecute = false;
            //p.StartInfo.RedirectStandardOutput = true;
            //p.StartInfo.FileName = "cmd.exe";
            //
            //if (!string.IsNullOrWhiteSpace(path))
            //    p.StartInfo.WorkingDirectory = path;
            //
            //p.StartInfo.Arguments = $"/c {command}";
            //p.StartInfo.CreateNoWindow = true;
            //p.Start();
            //p.WaitForExit();

            //var p = new ProcessStartInfo();
            //p.UseShellExecute = true;
            //
            //if (!string.IsNullOrWhiteSpace(path))
            //    p.WorkingDirectory = path;
            //else
            //    p.WorkingDirectory = @"C:\Windows\System32";
            //
            //p.FileName = @"C:\Windows\System32\cmd.exe";
            //p.Verb = "runas";
            //p.Arguments = "/c " + command;
            //p.WindowStyle = ProcessWindowStyle.Hidden;
            //Process.Start(p);

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.Verb = "runas";
            cmd.Start();

            cmd.StandardInput.WriteLine("echo Oscar");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
        }
    }
}
