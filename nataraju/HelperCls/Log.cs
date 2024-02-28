using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nataraju
{
    public class Log
    {
        private StringBuilder LogMessages = new StringBuilder();

        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                LogMessages.AppendLine(_status);
                Console.WriteLine($"[{DateTime.Now}] {_status}");
            }
        }

        //public string StatusInSameLine
        //{
        //    get { return _status; }
        //    set
        //    {
        //        _status = value;
        //        LogMessages.AppendLine(_status);
        //        Console.WriteLine($"{DateTime.Now } {_status}");
        //    }
        //}

        public string GetLogMessages()
        {
            return LogMessages.ToString();
        }

        public string WriteLog()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, $"Log_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}");

            File.WriteAllText(path, LogMessages.ToString());

            return path;
        }

    }
}
