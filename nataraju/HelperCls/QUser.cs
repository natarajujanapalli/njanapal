using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nataraju
{
    public class QUser
    {
        public string UserName { get; set; }
        public string SessionName { get; set; }
        public string Id { get; set; }
        public string State { get; set; }
        public string IdleTime { get; set; }
        public string LogonTime { get; set; }

        public QUser(string line, string columnsString)
        {
            UserName = line.Substring(columnsString.IndexOf("USERNAME"), columnsString.IndexOf("SESSIONNAME") - 1);
            SessionName = line.Substring(columnsString.IndexOf("SESSIONNAME"), columnsString.IndexOf("ID") - columnsString.IndexOf("SESSIONNAME") - 1);
            Id = line.Substring(columnsString.IndexOf("ID"), columnsString.IndexOf("STATE") - columnsString.IndexOf("ID") - 1);
            State = line.Substring(columnsString.IndexOf("STATE"), columnsString.IndexOf("IDLE TIME") - columnsString.IndexOf("STATE") - 1);
            IdleTime = line.Substring(columnsString.IndexOf("IDLE TIME"), columnsString.IndexOf("LOGON TIME") - columnsString.IndexOf("IDLE TIME") - 1);
            LogonTime = line.Substring(columnsString.IndexOf("LOGON TIME"), line.Length - columnsString.IndexOf("LOGON TIME"));

            LogonTime = GetDateString(LogonTime);

            UserName = UserName.Trim();
            SessionName = SessionName.Trim();
            Id = Id.Trim();
            State = State.Trim().ToUpper().Equals("DISC") ? "Disconnected" : State.Trim();
            IdleTime = IdleTime.Trim();
            LogonTime = LogonTime.Trim();
        }

        public string GetDateString(string datetime)
        {
            string result = datetime;
            try
            {
                result = Convert.ToDateTime(datetime).ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch
            {
                result = datetime;
            }

            return result;
        }
    }

}
