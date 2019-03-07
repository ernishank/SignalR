using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SignalRApp.Logs
{
    public class WriteLogFile
    {
        public static bool WriteLog(string strMessage)
        {
            try
            {
                FileStream objFilestream = new FileStream(string.Format("{0}\\{1}", @"C:\Users\n.maraiya\Downloads\SignalRApp\Logs", "SignalRLogs.txt"), FileMode.Append, FileAccess.Write);
                StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
                objStreamWriter.WriteLine(strMessage);
                objStreamWriter.Close();
                objFilestream.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}