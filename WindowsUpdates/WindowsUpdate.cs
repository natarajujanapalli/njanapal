using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WUApiLib;

namespace WindowsUpdates
{

    public class WindowsUpdate
    {

        public String GetUpdates()
        {

            UpdateSession uSession = new UpdateSession();
            IUpdateSearcher uSearcher = uSession.CreateUpdateSearcher();

            StringBuilder sb = new StringBuilder();

            uSearcher.Online = false;
            try
            {
                ISearchResult sResult = uSearcher.Search("IsInstalled=1 And IsHidden=0");
                sb.AppendLine("Found " + sResult.Updates.Count + " updates").AppendLine();
                int lineNumber = 0;
                foreach (IUpdate update in sResult.Updates)
                {
                    sb.AppendLine("\t" + $"{++lineNumber:000}. " + update.Title);
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine().AppendLine("Something went wrong: " + ex.Message);
            }

            return sb.ToString();
        }

    }
}
