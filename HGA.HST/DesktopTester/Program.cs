using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using XyratexOSC.Logging;
using DesktopTester.UI;

namespace DesktopTester
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TraceSource trace = XyratexOSC.Logging.Log.Trace();
            trace.Switch.Level = SourceLevels.All;

            string logFileDirectory = "C:\\Seagate\\HGA.HST\\Log\\";
            string logFilePath = string.Format("{0}{1}.DesktopTester.log", logFileDirectory, Environment.MachineName);
            if (!Directory.Exists(logFileDirectory))
            {
                Directory.CreateDirectory(logFilePath);
            }
            DailyTextWriterListener dailyListener = new DailyTextWriterListener(logFilePath);
            
            trace.Listeners.Add(dailyListener);

            Application.Run(new frmMain());            
        }
    }
}
