using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();

            string path1 = @"C:\Sources\NIBRS\branches\02.03.00";
            string path2 = @"C:\repos\NIBRS\NIBRS23";

            CopyFiles(path1, path2, sb);

            string[] paths1 = Directory.GetFiles(path1, "*.csproj", SearchOption.AllDirectories);
            string[] paths2 = Directory.GetFiles(path2, "*.csproj", SearchOption.AllDirectories);

            foreach(string sp in paths1)
            {
                var fn = Path.GetFileName(sp);
                var dps = paths2.Where(r => r.Contains(fn));

                foreach(var dp in dps)
                {
                    var s = Path.GetDirectoryName(sp);
                    var d = Path.GetDirectoryName(dp);

                    CopyFiles(s, d, sb, true);
                }
            }


            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Log_{DateTime.Now.ToString("yyyyMMddHHmmssff")}.log");
            File.WriteAllText(logPath, sb.ToString());

            Process.Start(logPath);
        }

        private static void CopyFiles(string path1, string path2, StringBuilder sb, bool copy = false)
        {

            StringBuilder sbMissing = new StringBuilder();

            string[] path1Files = Directory.GetFiles(path1, "*.*", SearchOption.AllDirectories).Where(r => !r.Contains("\\bin\\") && !r.Contains("\\obj\\") && !r.Contains("\\InputData\\") && !r.Contains("\\NIBRS23_Automation\\")).ToArray();
            string[] path2Files = Directory.GetFiles(path2, "*.*", SearchOption.AllDirectories).Where(r => !r.Contains("\\bin\\") && !r.Contains("\\obj\\") && !r.Contains("\\.git\\")).ToArray();

            sb.AppendLine($"Total Path 1 files : {path1Files.Length} ({path1})");
            sb.AppendLine($"Total Path 2 files : {path2Files.Length} ({path2})");

            foreach (string f1 in path1Files)
            {
                //sb.AppendLine($"\t{f1}");

                var f1_FileName = Path.GetFileName(f1);
                var f2_Files = path2Files.Where(r => r.EndsWith(f1_FileName));

                if (f2_Files != null && f2_Files.Count() > 0)
                {
                    foreach (string f2 in f2_Files)
                    {
                        sb.AppendLine($"\t\t REPLACING - \"{f1}\" \"{f2}\"");

                        File.Copy(f1, f2, true);
                    }
                }
                else
                {
                    if (copy)
                    {
                        var f2 = f1.Replace(path1, path2);
                        sbMissing.AppendLine($"\t\t COPYING - \"{f1}\" \"{f2}\"");

                        File.Copy(f1, f2, true);
                    }
                    else
                        sbMissing.AppendLine($"\t\tMISSING - \"{f1}\"");
                }
            }

            sb.AppendLine().AppendLine().AppendLine().AppendLine(sbMissing.ToString());

        }
    }

}
