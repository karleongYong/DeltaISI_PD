using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using XyratexOSC.Logging;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.Parsel.Services;
using XyratexOSC.UI;
using XyratexOSC.Utilities;
using System.Threading.Tasks;
using Seagate.AAS.HGA.HST.UI;
using System.IO;

namespace Seagate.AAS.HGA.HST.Machine
{
    class HSTMachine: Seagate.AAS.Parsel.Equipment.Machine
    {
        private static HSTMachine _instance;
        private HSTWorkcell _workcell = null;

        private Seagate.AAS.HGA.HST.UI.FormMainE95 _mainForm; 

        public new static HSTMachine Instance
        { 
            get
            {
                if (_instance == null)
                    _instance = new HSTMachine();

                return _instance;
            }        
        }

        public static HwSystem HwSystem
        { get { return _instance.hwSystem; } }

        public static HSTWorkcell Workcell
        {
            get
            {
                if (_instance != null)
                    return _instance._workcell;
                else
                {
                    return null;
                }
            }
        }

        public FormMainE95 MainForm
        {
            get { return _mainForm; }
            /*get 
            {
                if (_instance != null)
                    return _instance._mainForm;
                else
                {
                    return null;
                }
            }*/
            //set { _instance._mainForm = value; }
        }     

        private HSTMachine()
        {
            this.hwSystem = HSTHwSystem.Instance;
        }

        public override void Launch()
        {

            //No logging here as LogDir is in workcell

            base.Launch();

            //Logging here as Startup() executes in base.Launch();

            Task task = Task.Factory.StartNew(() => _mainForm = new FormMainE95(HSTMachine.Workcell));
            task.Wait();                        

            
         System.Windows.Forms.Application.Run(_mainForm);

            HSTMachine.Workcell.Shutdown();

            base.Shutdown();
            FISManager.Instance.Dispose();
        }

        public override void Startup()
        {
                             
            try
            {
                _workcell = new HSTWorkcell();

                TraceSource trace = Log.Trace();
                trace.Switch.Level = SourceLevels.All;
                string logName = string.Format("{0}\\{1}.HST.log", HSTMachine.Workcell.HSTSettings.Directory.LogFilePath, HSTMachine.Workcell.HSTSettings.Install.EquipmentID/*Environment.MachineName*/);
                DailyTextWriterListener listener = new DailyTextWriterListener(logName);
                trace.Listeners.Add(listener);
                LogStartup();    

                RegisterWorkcell(HSTMachine.Workcell);
                HSTMachine.Workcell.instantiateWorkOrder();
                ServiceManager.Tracing.Trace("Loaded Workcell: " + HSTMachine.Workcell.SlotName);
            }
            catch (Exception ex)
            {
                string msg = ServiceManager.ErrorHandler.GetExceptionMessages(ex);
                Notify.PopUpError("Startup error", "Failed to load module: " + HSTMachine.Workcell.SlotName + "\nError: " + msg + "\r\n\r\n" + ex);                
            }
            base.Startup();          
        }


        private void LogStartup()
        {
            List<string> startupMessages = new List<string>();
            Assembly appExe = Assembly.GetExecutingAssembly();


            startupMessages.Add("******************************************************************************************");
            startupMessages.Add("******************************************************************************************");
            startupMessages.Add("");
            startupMessages.Add("**********    START-UP    ***********    START-UP    ***********    START-UP   ***********");
            startupMessages.Add("");
            startupMessages.Add("");
            startupMessages.Add(string.Format("Application version: {0}", Assembly.GetExecutingAssembly().FullName));
            startupMessages.Add("");
            startupMessages.Add("Loaded assemblies...");

            bool appExeReached = false;
            AppDomain myDomain = AppDomain.CurrentDomain;
            Assembly[] assembliesLoaded = myDomain.GetAssemblies();
            foreach (Assembly assembly in assembliesLoaded)
            {
                AssemblyName assemblyName = assembly.GetName();

                if (!appExeReached)
                {
                    if (assembly == appExe)
                        appExeReached = true;

                    continue;
                }

                if (assemblyName.Name.StartsWith("System") || assemblyName.Name.StartsWith("Microsoft"))
                    continue;

                try
                {
                    string infoVersion = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

                    if (String.IsNullOrEmpty(infoVersion))
                        infoVersion = assemblyName.Version.ToString();

                    string assemblyInfo = String.Format("{0} ({1}) [{2}]", assemblyName.Name, infoVersion, assembly.Location);
                    startupMessages.Add(assemblyInfo);
                }
                catch (Exception)
                {
                    // Ignore error since we are just logging.  In some cases where an "Anonymous Assembly" is loaded (not sure how it come about),
                    // it causes Invoke Except from the above.
                }
            }

            startupMessages.Add("");
            startupMessages.Add("******************************************************************************************");
            startupMessages.Add("******************************************************************************************");

            foreach (string message in startupMessages)
                Log.Info(this, "|| {0}", message);

        }

        public override string ToString()
        {
            return "HSTMachine";
        }

        protected override void ShowSplashScreen()
        {
            base.ShowSplashScreen();
        }

    }
}
