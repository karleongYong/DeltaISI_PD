using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Threading;

namespace Seagate.AAS.HGA.HST.Machine
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Thread.CurrentThread.Name = "AppMain";
            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.High;
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                if (System.Diagnostics.Process.GetProcessesByName("HGA.HST").Length > 1)
                {
                    MessageBox.Show("Another instance of this application is already running!", "HST Machine Startup Error");
                    return;
                }

                Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                HSTMachine.Instance.Launch();

            }

            catch (Exception ex)
            {
                string msg = "Unhandled exception caught at the top-most application level. " + Environment.NewLine +
                                    "Source: " + ex.Source + Environment.NewLine +
                                    "Error: " + ex.Message + Environment.NewLine +
                                    "Stack trace: " + ex.StackTrace;
                MessageBox.Show(msg, "HST Unhandled Exception");
            }
        }


        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            // TY: I am just try to resolve a bug in parcel when
            // error form is clicked rapidly.
            if (e.Exception.StackTrace.Contains("FormErrorList"))
            {
                Console.Beep();
                return;
            }

            MessageBox.Show(e.Exception.ToString());
        }
    }
}
