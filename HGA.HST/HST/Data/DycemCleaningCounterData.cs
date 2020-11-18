using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Utils;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.Data
{
    public class DycemCleaningCounterData
    {
        private int inputEEDycemCleaningCount = 0;
        private int outputEEDycemCleaningCount = 0;
        private DateTime startDateTime = DateTime.Now;

        private string fileName = "";
        private SettingsXml xml;

        public int InputEEDycemCleaningCount { get { return inputEEDycemCleaningCount; } set { inputEEDycemCleaningCount = value; } }
        public int OutputEEDycemCleaningCount { get { return outputEEDycemCleaningCount; } set { outputEEDycemCleaningCount = value; } }

        public DateTime StartDateTime { get { return startDateTime; } }

        public string InputEEDycemCleaningCountString { get { return inputEEDycemCleaningCount.ToString("#,##0"); } }
        public string OutputEEDycemCleaningCountString { get { return outputEEDycemCleaningCount.ToString("#,##0"); } }
        public string StartDateTimeString { get { return startDateTime.ToString(); } }

        public DycemCleaningCounterData(string fileName)
        {
            this.fileName = fileName;

            if (!File.Exists(fileName))
            {
                File.Create(fileName).Dispose();
            }

            xml = new SettingsXml(fileName);
            Load();
        }

        public void Reset()
        {
            startDateTime = DateTime.Now;
            StoreOldDataToFile();
            inputEEDycemCleaningCount = 0;
            outputEEDycemCleaningCount = 0;
            Save();
        }

        public void Load()
        {
            xml.OpenSection("Counter");
            if (!int.TryParse(xml.Read("InputEEDycemCleaningCount", "0"), out inputEEDycemCleaningCount))
                inputEEDycemCleaningCount = 0;
            if (!int.TryParse(xml.Read("OutputEEDycemCleaningCount", "0"), out outputEEDycemCleaningCount))
                outputEEDycemCleaningCount = 0;
            if (!DateTime.TryParse(xml.Read("StartDateTime", DateTime.Now.ToString()), out startDateTime))
                startDateTime = DateTime.Now;
            xml.CloseSection();
        }

        public void Save()
        {
            xml.OpenSection("Counter");
            xml.Write("InputEEDycemCleaningCount", inputEEDycemCleaningCount.ToString());
            xml.Write("OutputEEDycemCleaningCount", outputEEDycemCleaningCount.ToString());
            xml.Write("StartDateTime", startDateTime.ToString());
            xml.CloseSection();
            xml.Save();
        }

        private void StoreOldDataToFile()
        {
            string dycemCleaningRecordFileName = HSTSettings.Instance.Directory.DataPath + @"\DycemCleaningRecord.csv";

            if (!File.Exists(dycemCleaningRecordFileName))
            {
                File.Create(dycemCleaningRecordFileName).Dispose();
                File.AppendAllText(dycemCleaningRecordFileName, "Input EE,Output EE, Reset Time" + Environment.NewLine); 

            }

            File.AppendAllText(dycemCleaningRecordFileName,
                               inputEEDycemCleaningCount.ToString() + "," +
                               outputEEDycemCleaningCount.ToString() + "," +
                               startDateTime.ToString() + Environment.NewLine); 
        }
    }
}
