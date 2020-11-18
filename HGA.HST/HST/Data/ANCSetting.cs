
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Xml.Serialization;
using System.Xml;
using System.Data;
using Seagate.AAS.HGA.HST.Process;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Equipment;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Utils;


using System.Linq;

using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using System.Runtime.InteropServices;

using System.Diagnostics;



namespace Seagate.AAS.HGA.HST.Data
{
    /// <summary>
    /// Provide machine performance data.
    /// 
    /// </summary>
    public class ANCSetting
    {


        // Nested declarations -------------------------------------------------
        public const string CurrentPerformaceConfigName = "ANCSetting.config";


        // Member variables ----------------------------------------------------        

        protected bool _isTrackingStarted = false;
        protected int _alarmCount;
        protected DateTime _startTrackingEvent = new DateTime();
        protected string _uTICMachineName1;
        protected string _uTICMachineName2;

        protected double _All_TicFailCounter;
        protected double _All_HstFailCounter;
        protected double _All_TicGoodPartCounter;
        protected double _All_HstGoodPartCounter;
        protected double _All_AdaptivePartRunCounter;
        protected double _All_AdaptiveDefectCounter;
        protected DateTime _All_LastSaveLogTime;
        protected double _All_MCDownTriggering;
        protected double _All_TicHighPercentTriggeringCounter;
        protected double _All_HstHighPercentTriggeringCounter;
        protected double _All_InternalTriggerCounter;

        protected double _MC1_TicFailCounter;
        protected double _MC1_HstFailCounter;
        protected double _MC1_TicGoodPartCounter;
        protected double _MC1_HstGoodPartCounter;
        protected double _MC1_AdaptivePartRunCounter;
        protected double _MC1_AdaptiveDefectCounter;
        protected DateTime _MC1_LastSaveLogTime;
        protected double _MC1_MCDownTriggering;
        protected double _MC1_TicHighPercentTriggeringCounter;
        protected double _MC1_HstHighPercentTriggeringCounter;
        protected double _MC1_InternalTriggerCounter;

        protected double _MC2_TicFailCounter;
        protected double _MC2_HstFailCounter;
        protected double _MC2_TicGoodPartCounter;
        protected double _MC2_HstGoodPartCounter;
        protected double _MC2_AdaptivePartRunCounter;
        protected double _MC2_AdaptiveDefectCounter;
        protected DateTime _MC2_LastSaveLogTime;
        protected double _MC2_MCDownTriggering;
        protected double _MC2_TicHighPercentTriggeringCounter;
        protected double _MC2_HstHighPercentTriggeringCounter;
        protected double _MC2_InternalTriggerCounter;




        // Constructors & Finalizers -------------------------------------------

        public void Dispose()
        {

        }

        // Properties ----------------------------------------------------------   
        public string uTICMachineName1 { get { return _uTICMachineName1; } set { _uTICMachineName1 = value; } }
        public string uTICMachineName2 { get { return _uTICMachineName2; } set { _uTICMachineName2 = value; } }
        public double All_TicFailCounter { get { return _All_TicFailCounter; } set { _All_TicFailCounter = value; } }
        public double All_HstFailCounter { get { return _All_HstFailCounter; } set { _All_HstFailCounter = value; } }
        public double All_TicGoodPartCounter { get { return _All_TicGoodPartCounter; } set { _All_TicGoodPartCounter = value; } }
        public double All_HstGoodPartCounter { get { return _All_HstGoodPartCounter; } set { _All_HstGoodPartCounter = value; } }
        public double All_AdaptivePartRunCounter { get { return _All_AdaptivePartRunCounter; } set { _All_AdaptivePartRunCounter = value; } }
        public double All_AdaptiveDefectCounter { get { return _All_AdaptiveDefectCounter; } set { _All_AdaptiveDefectCounter = value; } }
        public DateTime All_LastSaveLogTime { get { return _All_LastSaveLogTime; } set { _All_LastSaveLogTime = value; } }
        public double All_MCDownTriggering { get { return _All_MCDownTriggering; } set { _All_MCDownTriggering = value; } }
        public double All_TicHighPercentTriggeringCounter { get { return _All_TicHighPercentTriggeringCounter; } set { _All_TicHighPercentTriggeringCounter = value; } }
        public double All_HstHighPercentTriggeringCounter { get { return _All_HstHighPercentTriggeringCounter; } set { _All_HstHighPercentTriggeringCounter = value; } }
        public double All_InternalTriggerCounter { get { return _All_InternalTriggerCounter; } set { _All_InternalTriggerCounter = value; } }
        public double MC1_TicFailCounter { get { return _MC1_TicFailCounter; } set { _MC1_TicFailCounter = value; } }
        public double MC1_HstFailCounter { get { return _MC1_HstFailCounter; } set { _MC1_HstFailCounter = value; } }
        public double MC1_TicGoodPartCounter { get { return _MC1_TicGoodPartCounter; } set { _MC1_TicGoodPartCounter = value; } }
        public double MC1_HstGoodPartCounter { get { return _MC1_HstGoodPartCounter; } set { _MC1_HstGoodPartCounter = value; } }
        public double MC1_AdaptivePartRunCounter { get { return _MC1_AdaptivePartRunCounter; } set { _MC1_AdaptivePartRunCounter = value; } }
        public double MC1_AdaptiveDefectCounter { get { return _MC1_AdaptiveDefectCounter; } set { _MC1_AdaptiveDefectCounter = value; } }
        public DateTime MC1_LastSaveLogTime { get { return _MC1_LastSaveLogTime; } set { _MC1_LastSaveLogTime = value; } }
        public double MC1_MCDownTriggering { get { return _MC1_MCDownTriggering; } set { _MC1_MCDownTriggering = value; } }
        public double MC1_TicHighPercentTriggeringCounter { get { return _MC1_TicHighPercentTriggeringCounter; } set { _MC1_TicHighPercentTriggeringCounter = value; } }
        public double MC1_HstHighPercentTriggeringCounter { get { return _MC1_HstHighPercentTriggeringCounter; } set { _MC1_HstHighPercentTriggeringCounter = value; } }
        public double MC1_InternalTriggerCounter { get { return _MC1_InternalTriggerCounter; } set { _MC1_InternalTriggerCounter = value; } }
        public double MC2_TicFailCounter { get { return _MC2_TicFailCounter; } set { _MC2_TicFailCounter = value; } }
        public double MC2_HstFailCounter { get { return _MC2_HstFailCounter; } set { _MC2_HstFailCounter = value; } }
        public double MC2_TicGoodPartCounter { get { return _MC2_TicGoodPartCounter; } set { _MC2_TicGoodPartCounter = value; } }
        public double MC2_HstGoodPartCounter { get { return _MC2_HstGoodPartCounter; } set { _MC2_HstGoodPartCounter = value; } }
        public double MC2_AdaptivePartRunCounter { get { return _MC2_AdaptivePartRunCounter; } set { _MC2_AdaptivePartRunCounter = value; } }
        public double MC2_AdaptiveDefectCounter { get { return _MC2_AdaptiveDefectCounter; } set { _MC2_AdaptiveDefectCounter = value; } }
        public DateTime MC2_LastSaveLogTime { get { return _MC2_LastSaveLogTime; } set { _MC2_LastSaveLogTime = value; } }
        public double MC2_MCDownTriggering { get { return _MC2_MCDownTriggering; } set { _MC2_MCDownTriggering = value; } }
        public double MC2_TicHighPercentTriggeringCounter { get { return _MC2_TicHighPercentTriggeringCounter; } set { _MC2_TicHighPercentTriggeringCounter = value; } }
        public double MC2_HstHighPercentTriggeringCounter { get { return _MC2_HstHighPercentTriggeringCounter; } set { _MC2_HstHighPercentTriggeringCounter = value; } }
        public double MC2_InternalTriggerCounter { get { return _MC2_InternalTriggerCounter; } set { _MC2_InternalTriggerCounter = value; } }


        // Methods -------------------------------------------------------------
        public void Initialize()
        {


        }

        // Internal methods ----------------------------------------------------       
        public void reset()
        {


            _All_TicFailCounter = 0;
            _All_HstFailCounter = 0;
            _All_TicGoodPartCounter = 0;
            _All_HstGoodPartCounter = 0;
            _All_AdaptivePartRunCounter = 0;
            _All_AdaptiveDefectCounter = 0;
            _All_LastSaveLogTime = System.DateTime.Today;
            _All_MCDownTriggering = 0;
            _All_TicHighPercentTriggeringCounter = 0;
            _All_HstHighPercentTriggeringCounter = 0;
            _All_InternalTriggerCounter = 0;

            _MC1_TicFailCounter = 0;
            _MC1_HstFailCounter = 0;
            _MC1_TicGoodPartCounter = 0;
            _MC1_HstGoodPartCounter = 0;
            _MC1_AdaptivePartRunCounter = 0;
            _MC1_AdaptiveDefectCounter = 0;
            _MC1_LastSaveLogTime = System.DateTime.Today;
            _MC1_MCDownTriggering = 0;
            _MC1_TicHighPercentTriggeringCounter = 0;
            _MC1_HstHighPercentTriggeringCounter = 0;
            _MC1_InternalTriggerCounter = 0;

            _MC2_TicFailCounter = 0;
            _MC2_HstFailCounter = 0;
            _MC2_TicGoodPartCounter = 0;
            _MC2_HstGoodPartCounter = 0;
            _MC2_AdaptivePartRunCounter = 0;
            _MC2_AdaptiveDefectCounter = 0;
            _MC2_LastSaveLogTime = System.DateTime.Today;
            _MC2_MCDownTriggering = 0;
            _MC2_TicHighPercentTriggeringCounter = 0;
            _MC2_HstHighPercentTriggeringCounter = 0;
            _MC2_InternalTriggerCounter = 0;

        }

        public void Save()
        {
            XmlDocument xmlString = new XmlDocument();
            xmlString.LoadXml("<?xml version='1.0' encoding='utf-8' standalone='yes'?><data></data>");

            // Add a price element.
            XmlElement newElem = xmlString.CreateElement("CCCParameterSetting");
            appendXML(xmlString, "uTICMachineName1", uTICMachineName1);
            appendXML(xmlString, "uTICMachineName2", uTICMachineName2);
            appendXML(xmlString, "All_TicFailCounter", _All_TicFailCounter.ToString());
            appendXML(xmlString, "All_HstFailCounter", All_HstFailCounter.ToString());
            appendXML(xmlString, "All_TicGoodPartCounter", All_TicGoodPartCounter.ToString());
            appendXML(xmlString, "All_HstGoodPartCounter", All_HstGoodPartCounter.ToString());
            appendXML(xmlString, "All_AdaptivePartRunCounter", All_AdaptivePartRunCounter.ToString());
            appendXML(xmlString, "All_AdaptiveDefectCounter", All_AdaptiveDefectCounter.ToString());
            appendXML(xmlString, "All_LastSaveLogTime", _All_LastSaveLogTime.ToString("dd-MMM-yy:HH:mm:ss"));
            appendXML(xmlString, "All_MCDownTriggering", All_MCDownTriggering.ToString());
            appendXML(xmlString, "All_TicHighPercentTriggeringCounter", All_TicHighPercentTriggeringCounter.ToString());
            appendXML(xmlString, "All_HstHighPercentTriggeringCounter", All_HstHighPercentTriggeringCounter.ToString());
            appendXML(xmlString, "All_InternalTriggerCounter", All_InternalTriggerCounter.ToString());

            appendXML(xmlString, "MC1_TicFailCounter", _MC1_TicFailCounter.ToString());
            appendXML(xmlString, "MC1_HstFailCounter", MC1_HstFailCounter.ToString());
            appendXML(xmlString, "MC1_TicGoodPartCounter", MC1_TicGoodPartCounter.ToString());
            appendXML(xmlString, "MC1_HstGoodPartCounter", MC1_HstGoodPartCounter.ToString());
            appendXML(xmlString, "MC1_AdaptivePartRunCounter", MC1_AdaptivePartRunCounter.ToString());
            appendXML(xmlString, "MC1_AdaptiveDefectCounter", MC1_AdaptiveDefectCounter.ToString());
            appendXML(xmlString, "MC1_LastSaveLogTime", _MC1_LastSaveLogTime.ToString("dd-MMM-yy:HH:mm:ss"));
            appendXML(xmlString, "MC1_MCDownTriggering", MC1_MCDownTriggering.ToString());
            appendXML(xmlString, "MC1_TicHighPercentTriggeringCounter", MC1_TicHighPercentTriggeringCounter.ToString());
            appendXML(xmlString, "MC1_HstHighPercentTriggeringCounter", MC1_HstHighPercentTriggeringCounter.ToString());
            appendXML(xmlString, "MC1_InternalTriggerCounter", MC1_InternalTriggerCounter.ToString());

            appendXML(xmlString, "MC2_TicFailCounter", _MC2_TicFailCounter.ToString());
            appendXML(xmlString, "MC2_HstFailCounter", MC2_HstFailCounter.ToString());
            appendXML(xmlString, "MC2_TicGoodPartCounter", MC2_TicGoodPartCounter.ToString());
            appendXML(xmlString, "MC2_HstGoodPartCounter", MC2_HstGoodPartCounter.ToString());
            appendXML(xmlString, "MC2_AdaptivePartRunCounter", MC2_AdaptivePartRunCounter.ToString());
            appendXML(xmlString, "MC2_AdaptiveDefectCounter", MC2_AdaptiveDefectCounter.ToString());
            appendXML(xmlString, "MC2_LastSaveLogTime", _MC2_LastSaveLogTime.ToString("dd-MMM-yy:HH:mm:ss"));
            appendXML(xmlString, "MC2_MCDownTriggering", MC2_MCDownTriggering.ToString());
            appendXML(xmlString, "MC2_TicHighPercentTriggeringCounter", MC2_TicHighPercentTriggeringCounter.ToString());
            appendXML(xmlString, "MC2_HstHighPercentTriggeringCounter", MC2_HstHighPercentTriggeringCounter.ToString());
            appendXML(xmlString, "MC2_InternalTriggerCounter", MC2_InternalTriggerCounter.ToString());

            // Save the document to a file. White space is
            // preserved (no white space).
            xmlString.PreserveWhitespace = true;
            xmlString.Save("C:\\seagate\\HGA.HST\\Setup\\ANCSetting.config");

        }

        //  public void Save(string section, Seagate.AAS.Utils.SettingsXml xml)
        public void SaveFromHSTSettings()
        {
            XmlDocument xmlString = new XmlDocument();
            xmlString.LoadXml("<?xml version='1.0' encoding='utf-8' standalone='yes'?><data></data>");

            // Add a price element.
            XmlElement newElem = xmlString.CreateElement("CCCParameterSetting");
            appendXML(xmlString, "uTICMachineName1", HSTMachine.Workcell.HSTSettings.CccParameterSetting.uTICMachineName1.ToString());
            appendXML(xmlString, "uTICMachineName2", HSTMachine.Workcell.HSTSettings.CccParameterSetting.uTICMachineName2.ToString());
            appendXML(xmlString, "All_TicFailCounter", HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.TicFailCounter.ToString());
            appendXML(xmlString, "All_HstFailCounter", HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.HstFailCounter.ToString());
            appendXML(xmlString, "All_TicGoodPartCounter", HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.TicGoodPartCounter.ToString());
            appendXML(xmlString, "All_HstGoodPartCounter", HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.HstGoodPartCounter.ToString());
            appendXML(xmlString, "All_AdaptivePartRunCounter", HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.AdaptivePartRunCounter.ToString());
            appendXML(xmlString, "All_AdaptiveDefectCounter", HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.AdaptiveDefectCounter.ToString());
            appendXML(xmlString, "All_LastSaveLogTime", HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.LastSaveLogTime.ToString("dd-MMM-yy:HH:mm:ss"));

            appendXML(xmlString, "All_MCDownTriggering", HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.MCDownTriggering.ToString());
            appendXML(xmlString, "All_TicHighPercentTriggeringCounter", HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.TicHighPercentTriggeringCounter.ToString());
            appendXML(xmlString, "All_HstHighPercentTriggeringCounter", HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.HstHighPercentTriggeringCounter.ToString());
            appendXML(xmlString, "All_InternalTriggerCounter", HSTMachine.Workcell.TICCccControl.CCCControlAllMc.CccResult.OutputCounter.InternalTriggerCounter.ToString());

            appendXML(xmlString, "MC1_TicFailCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.TicFailCounter.ToString());
            appendXML(xmlString, "MC1_HstFailCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.HstFailCounter.ToString());
            appendXML(xmlString, "MC1_TicGoodPartCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.TicGoodPartCounter.ToString());
            appendXML(xmlString, "MC1_HstGoodPartCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.HstGoodPartCounter.ToString());
            appendXML(xmlString, "MC1_AdaptivePartRunCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.AdaptivePartRunCounter.ToString());
            appendXML(xmlString, "MC1_AdaptiveDefectCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.AdaptiveDefectCounter.ToString());
            appendXML(xmlString, "MC1_LastSaveLogTime", HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.LastSaveLogTime.ToString("dd-MMM-yy:HH:mm:ss"));
            appendXML(xmlString, "MC1_MCDownTriggering", HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.MCDownTriggering.ToString());
            appendXML(xmlString, "MC1_TicHighPercentTriggeringCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.TicHighPercentTriggeringCounter.ToString());
            appendXML(xmlString, "MC1_HstHighPercentTriggeringCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.HstHighPercentTriggeringCounter.ToString());
            appendXML(xmlString, "MC1_InternalTriggerCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc1.CccResult.OutputCounter.InternalTriggerCounter.ToString());

            appendXML(xmlString, "MC2_TicFailCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.TicFailCounter.ToString());
            appendXML(xmlString, "MC2_HstFailCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.HstFailCounter.ToString());
            appendXML(xmlString, "MC2_TicGoodPartCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.TicGoodPartCounter.ToString());
            appendXML(xmlString, "MC2_HstGoodPartCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.HstGoodPartCounter.ToString());
            appendXML(xmlString, "MC2_AdaptivePartRunCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.AdaptivePartRunCounter.ToString());
            appendXML(xmlString, "MC2_AdaptiveDefectCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.AdaptiveDefectCounter.ToString());
            appendXML(xmlString, "MC2_LastSaveLogTime", HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.LastSaveLogTime.ToString("dd-MMM-yy:HH:mm:ss"));
            appendXML(xmlString, "MC2_MCDownTriggering", HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.MCDownTriggering.ToString());
            appendXML(xmlString, "MC2_TicHighPercentTriggeringCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.TicHighPercentTriggeringCounter.ToString());
            appendXML(xmlString, "MC2_HstHighPercentTriggeringCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.HstHighPercentTriggeringCounter.ToString());
            appendXML(xmlString, "MC2_InternalTriggerCounter", HSTMachine.Workcell.TICCccControl.CCCControlTicMc2.CccResult.OutputCounter.InternalTriggerCounter.ToString());

            // Save the document to a file. White space is
            // preserved (no white space).
            xmlString.PreserveWhitespace = true;
            xmlString.Save("C:\\seagate\\HGA.HST\\Setup\\ANCSetting.config");

        }

        public void appendXML(XmlDocument xmldoc, string elemname, string elemval)
        {
            XmlElement newElem = xmldoc.CreateElement(elemname);
            newElem.InnerText = elemval;
            xmldoc.DocumentElement.AppendChild(newElem);
        }
        public void Load(string section, Seagate.AAS.Utils.SettingsXml xml)
        {
            XmlDocument xmlString = new XmlDocument();
            try
            {
                xmlString.Load("C:\\seagate\\HGA.HST\\Setup\\ANCSetting.config");
            }
            catch
            {
                _uTICMachineName1 = "";
                _uTICMachineName2 = "";

                _All_TicFailCounter = 0;
                _All_HstFailCounter = 0;
                _All_TicGoodPartCounter = 0;
                _All_HstGoodPartCounter = 0;
                _All_AdaptivePartRunCounter = 0;
                _All_AdaptiveDefectCounter = 0;
                _All_LastSaveLogTime = DateTime.ParseExact((System.DateTime.Today).ToString("dd-MMM-yy:HH:mm:ss"), "dd-MMM-yy:HH:mm:ss", null);
                _All_MCDownTriggering = 0;
                _All_TicHighPercentTriggeringCounter = 0;
                _All_HstHighPercentTriggeringCounter = 0;
                _All_InternalTriggerCounter = 0;

                _MC1_TicFailCounter = 0;
                _MC1_HstFailCounter = 0;
                _MC1_TicGoodPartCounter = 0;
                _MC1_HstGoodPartCounter = 0;
                _MC1_AdaptivePartRunCounter = 0;
                _MC1_AdaptiveDefectCounter = 0;
                _MC1_LastSaveLogTime = DateTime.ParseExact((System.DateTime.Today).ToString("dd-MMM-yy:HH:mm:ss"), "dd-MMM-yy:HH:mm:ss", null);
                _MC1_MCDownTriggering = 0;
                _MC1_TicHighPercentTriggeringCounter = 0;
                _MC1_HstHighPercentTriggeringCounter = 0;
                _MC1_InternalTriggerCounter = 0;

                _MC2_TicFailCounter = 0;
                _MC2_HstFailCounter = 0;
                _MC2_TicGoodPartCounter = 0;
                _MC2_HstGoodPartCounter = 0;
                _MC2_AdaptivePartRunCounter = 0;
                _MC2_AdaptiveDefectCounter = 0;
                _MC2_LastSaveLogTime = DateTime.ParseExact((System.DateTime.Today).ToString("dd-MMM-yy:HH:mm:ss"), "dd-MMM-yy:HH:mm:ss", null);
                _MC2_MCDownTriggering = 0;
                _MC2_TicHighPercentTriggeringCounter = 0;
                _MC2_HstHighPercentTriggeringCounter = 0;
                _MC2_InternalTriggerCounter = 0;

                Save();
                xmlString.Load("C:\\seagate\\HGA.HST\\Setup\\ANCSetting.config");
            }

            _uTICMachineName1 = xmlString.ChildNodes[1].ChildNodes[0].InnerText;
            _uTICMachineName2 = xmlString.ChildNodes[1].ChildNodes[1].InnerText;

            _All_TicFailCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[2].InnerText);
            _All_HstFailCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[3].InnerText);
            _All_TicGoodPartCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[4].InnerText);
            _All_HstGoodPartCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[5].InnerText);
            _All_AdaptivePartRunCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[6].InnerText);
            _All_AdaptiveDefectCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[7].InnerText);
           
            _All_LastSaveLogTime = DateTime.ParseExact(xmlString.ChildNodes[1].ChildNodes[8].InnerText, "dd-MMM-yy:HH:mm:ss", null);
            _All_MCDownTriggering = double.Parse(xmlString.ChildNodes[1].ChildNodes[9].InnerText);
            _All_TicHighPercentTriggeringCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[10].InnerText);
            _All_HstHighPercentTriggeringCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[11].InnerText);
            _All_InternalTriggerCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[12].InnerText);

            _MC1_TicFailCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[13].InnerText);
            _MC1_HstFailCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[14].InnerText);
            _MC1_TicGoodPartCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[15].InnerText);
            _MC1_HstGoodPartCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[16].InnerText);
            _MC1_AdaptivePartRunCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[17].InnerText);
            _MC1_AdaptiveDefectCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[18].InnerText);
            _MC1_LastSaveLogTime = DateTime.ParseExact(xmlString.ChildNodes[1].ChildNodes[19].InnerText, "dd-MMM-yy:HH:mm:ss", null);
            _MC1_MCDownTriggering = double.Parse(xmlString.ChildNodes[1].ChildNodes[20].InnerText);
            _MC1_TicHighPercentTriggeringCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[21].InnerText);
            _MC1_HstHighPercentTriggeringCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[22].InnerText);
            _MC1_InternalTriggerCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[23].InnerText);

            _MC2_TicFailCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[24].InnerText);
            _MC2_HstFailCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[25].InnerText);
            _MC2_TicGoodPartCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[26].InnerText);
            _MC2_HstGoodPartCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[27].InnerText);
            _MC2_AdaptivePartRunCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[28].InnerText);
            _MC2_AdaptiveDefectCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[29].InnerText);
            _MC2_LastSaveLogTime = DateTime.ParseExact(xmlString.ChildNodes[1].ChildNodes[30].InnerText, "dd-MMM-yy:HH:mm:ss", null);
            _MC2_MCDownTriggering = double.Parse(xmlString.ChildNodes[1].ChildNodes[31].InnerText);
            _MC2_TicHighPercentTriggeringCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[32].InnerText);
            _MC2_HstHighPercentTriggeringCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[33].InnerText);
            _MC2_InternalTriggerCounter = double.Parse(xmlString.ChildNodes[1].ChildNodes[34].InnerText);

        }

    }



}

