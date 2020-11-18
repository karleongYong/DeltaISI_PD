
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Xml.Serialization;
using System.Xml;
using System.Data;
using Seagate.AAS.HGA.HST.Process;
using Seagate.AAS.HGA.HST.Machine;
//using Seagate.AAS.Utils;
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
    public class SamplingData
    {


        // Nested declarations -------------------------------------------------
        public const string CurrentPerformaceConfigName = "SamplingData.config";


        // Member variables ----------------------------------------------------        

        protected bool _isTrackingStarted = false;
        protected int _alarmCount;
        protected DateTime _startTrackingEvent = new DateTime();
        protected string _datastring;
        protected List<double> sampledatalist = new List<double>();
        protected double Z_MAX = 6;
        protected double countPass = 0;
        protected double countFail = 0;
        protected double countTotal = 0;
        protected double CurrentZ = 0;
        protected double CurrentZvalue = 0;
        protected double Mean = 0;
        protected double Stddev = 0;
        protected double Highest = 0;
        protected double Lowest = 0;
        protected string LatestWOName = "";
        protected double LowestAcceptVal = 0.5;
        protected double HighestAcceptVal = 7;
        protected double SampleSize = 1000;
        protected double TotalTest = 0;
        protected double TotalWRBridge = 0;
        protected double TotalWRFail = 0;
        // Constructors & Finalizers -------------------------------------------
        public List<double> DataList()
        {
            return sampledatalist;
        }

        public double critz(double p)
        {

            double Z_EPSILON = 0.000001;     /* Accuracy of z approximation */
            double minz = -Z_MAX;
            double maxz = Z_MAX;
            double zval = 0.0;
            double pval = 0;

            if (p < 0.0 || p > 1.0)
            {
                return -1;
            }

            while ((maxz - minz) > Z_EPSILON)
            {
                pval = poz(zval);
                if (pval > p)
                {
                    maxz = zval;
                }
                else
                {
                    minz = zval;
                }
                zval = (maxz + minz) * 0.5;
            }
            return (zval);
        }

        public double poz(double z)
        {
            double y = 0;
            double x = 0;
            double w = 0;

            if (z == 0.0)
            {
                x = 0.0;
            }
            else
            {
                y = 0.5 * Math.Abs(z);
                if (y > (Z_MAX * 0.5))
                {
                    x = 1.0;
                }
                else if (y < 1.0)
                {
                    w = y * y;
                    x = ((((((((0.000124818987 * w
                             - 0.001075204047) * w + 0.005198775019) * w
                             - 0.019198292004) * w + 0.059054035642) * w
                             - 0.151968751364) * w + 0.319152932694) * w
                             - 0.531923007300) * w + 0.797884560593) * y * 2.0;
                }
                else
                {
                    y -= 2.0;
                    x = (((((((((((((-0.000045255659 * y
                                   + 0.000152529290) * y - 0.000019538132) * y
                                   - 0.000676904986) * y + 0.001390604284) * y
                                   - 0.000794620820) * y - 0.002034254874) * y
                                   + 0.006549791214) * y - 0.010557625006) * y
                                   + 0.011630447319) * y - 0.009279453341) * y
                                   + 0.005353579108) * y - 0.002141268741) * y
                                   + 0.000535310849) * y + 0.999936657524;
                }
            }
            return z > 0.0 ? ((x + 1.0) * 0.5) : ((1.0 - x) * 0.5);
        }
        public double getStandardDeviation(List<double> doubleList)
        {
            double average = doubleList.Average();
            double sumOfDerivation = 0;
            foreach (double value in doubleList)
            {
                sumOfDerivation += (value - average) * (value - average);
            }
            // double sumOfDerivationAverage = sumOfDerivation / (doubleList.Count - 1);
            double sumOfDerivationAverage = sumOfDerivation / (doubleList.Count);
            //return Math.Sqrt(sumOfDerivationAverage - (average * average));
            double stddv = Math.Sqrt(sumOfDerivationAverage);
            return stddv;
        }

        public void Dispose()
        {

        }

        // Properties ----------------------------------------------------------   
        public string dataLoad { get { return _datastring; } set { _datastring = value; } }
        public double getMean { get { return Mean; } set { Mean = value; } }
        public double getStdDev { get { return Stddev; } set { Mean = Stddev; } }
        public double getCurrentZvalue { get { return CurrentZvalue; } set { CurrentZvalue = value; } }
        public double getCurrentZ { get { return CurrentZ; } set { CurrentZ = value; } }
        public string getLastestWoName { get { return LatestWOName; } set { LatestWOName = value; } }
        public double getTotalTest { get { return TotalTest; } set { TotalTest = value; } }
        public double getTotalWRBridge { get { return TotalWRBridge; } set { TotalWRBridge = value; } }
        public double getTotalWRFail { get { return TotalWRFail; } set { TotalWRFail = value; } }
        public double getSampleSize { get { return SampleSize; } set { SampleSize = value; } }

        public double getHeightest { get { return HighestAcceptVal; } set { HighestAcceptVal = value; } }

        public double getLowest { get { return LowestAcceptVal; } set { LowestAcceptVal = value; } }
        // Methods -------------------------------------------------------------
        public void Initialize()
        {


        }
        public int getCount { get { return sampledatalist.Count(); } }
        public void setWOName(string WOname)
        {
            LatestWOName = WOname;
        }
        public void Append(double newitem)
        {
            if ((newitem >= LowestAcceptVal) && (newitem < HighestAcceptVal))
            {
                if (sampledatalist.Count >= SampleSize)
                {

                    sampledatalist.RemoveRange(0, 1);
                    sampledatalist.Add(newitem);
                }
                else
                {
                    sampledatalist.Add(newitem);
                }
            }

        }
        public void Calculate()
        {
            Mean = sampledatalist.Average();
            //CurrentZ = 0.01;
            Stddev = getStandardDeviation(sampledatalist);
            Highest = sampledatalist.Max();
            Lowest = sampledatalist.Min();
            CurrentZvalue = (Math.Floor(((critz(CurrentZ) * (getStandardDeviation(sampledatalist))) + sampledatalist.Average()) * 100) / 100);


        }

        public virtual void StopTracking()
        {

            _isTrackingStarted = false;
        }

        // Internal methods ----------------------------------------------------       
        public void reset()
        {
            sampledatalist.RemoveRange(0, sampledatalist.Count());
            TotalTest = 0;
            TotalWRBridge = 0;
            TotalWRFail = 0;
            Save();

        }

        public void Save()
        {
            XmlDocument xmlString = new XmlDocument();
            string sampletxt = "";
            List<double> sample = sampledatalist;
            xmlString.LoadXml("<?xml version='1.0' encoding='utf-8' standalone='yes'?><data></data>");

            // Add a price element.
            XmlElement newElem = xmlString.CreateElement("sampledata");
            if (sample.Count > 0)
            {
                for (int i = 0; i < sample.Count() - 1; i++)
                {
                    sampletxt += sample[i].ToString() + ",";
                }
                sampletxt += sample[sample.Count() - 1].ToString();
            }
           
            newElem.InnerText = sampletxt;

            xmlString.DocumentElement.AppendChild(newElem);

            newElem = xmlString.CreateElement("LastWO");
            newElem.InnerText = LatestWOName;
            xmlString.DocumentElement.AppendChild(newElem);

            newElem = xmlString.CreateElement("z_pct");
            newElem.InnerText = CurrentZ.ToString();
            xmlString.DocumentElement.AppendChild(newElem);


            newElem = xmlString.CreateElement("LowestAcceptVal");
            newElem.InnerText = LowestAcceptVal.ToString();
            xmlString.DocumentElement.AppendChild(newElem);

            newElem = xmlString.CreateElement("HighestAcceptVal");
            newElem.InnerText = HighestAcceptVal.ToString();
            xmlString.DocumentElement.AppendChild(newElem);

            newElem = xmlString.CreateElement("SampleSize");
            newElem.InnerText = SampleSize.ToString();
            xmlString.DocumentElement.AppendChild(newElem);

            newElem = xmlString.CreateElement("TotalTest");
            newElem.InnerText = TotalTest.ToString();
            xmlString.DocumentElement.AppendChild(newElem);

            newElem = xmlString.CreateElement("TotalWRBridge");
            newElem.InnerText = TotalWRBridge.ToString();
            xmlString.DocumentElement.AppendChild(newElem);

            newElem = xmlString.CreateElement("TotalWRFAIL");
            newElem.InnerText = TotalWRFail.ToString();
            xmlString.DocumentElement.AppendChild(newElem);

            // Save the document to a file. White space is
            xmlString.PreserveWhitespace = true;
            xmlString.Save("C:\\seagate\\HGA.HST\\Setup\\Sampling.config");
        }
        public void Load(string section, Seagate.AAS.Utils.SettingsXml xml)
        {
            string sampletxt;
            XmlDocument xmlString = new XmlDocument();
            try
            {
                xmlString.Load("C:\\seagate\\HGA.HST\\Setup\\Sampling.config");
            }
            catch
            {
                CurrentZ = 3;
                LowestAcceptVal = 0.5;
                HighestAcceptVal = 7;
                SampleSize = 1000;
                LatestWOName = "";
                TotalTest = 0;
                TotalWRBridge = 0;
                TotalWRFail = 0;
                Save();
                xmlString.Load("C:\\seagate\\HGA.HST\\Setup\\Sampling.config");
            }
            if (xmlString.ChildNodes.Count > 0)
            {
                sampletxt = xmlString.ChildNodes[1].ChildNodes[0].InnerText;
            }
            else
            {
                sampletxt = "";
            }
            try
            {
                LatestWOName = xmlString.ChildNodes[1].ChildNodes[1].InnerText;
            }
            catch
            {
                LatestWOName = "";
                CurrentZ = 3;
                LowestAcceptVal = 0.5;
                HighestAcceptVal = 7;
                SampleSize = 1000;
                TotalTest = 0;
                TotalWRBridge = 0;
                TotalWRFail = 0;
                Save();

                xmlString.Load("C:\\seagate\\HGA.HST\\Setup\\Sampling.config");


            }
            try
            {
                CurrentZ = double.Parse(xmlString.ChildNodes[1].ChildNodes[2].InnerText);
            }
            catch
            {
                TotalTest = 0;
                TotalWRBridge = 0;
                TotalWRFail = 0;
                CurrentZ = 0.03;
            }


            LatestWOName = xmlString.ChildNodes[1].ChildNodes[1].InnerText;

            _datastring = sampletxt;
            char[] delimiterChars = { ',', ':', '\t' };
            string[] words = _datastring.Split(delimiterChars);
            sampledatalist = new List<double>();

            if (_datastring != "")
            {
                foreach (var word in words)
                {
                    if (word.Length > 0)
                    {
                        if (sampledatalist.Count >= SampleSize)
                        {
                            sampledatalist.RemoveRange(0, 1);
                            sampledatalist.Add(double.Parse(word.Trim()));
                        }
                        else
                        {
                            sampledatalist.Add(double.Parse(word.Trim()));
                        }
                    }
                }
                try
                {
                    TotalTest = double.Parse(xmlString.ChildNodes[1].ChildNodes[6].InnerText);
                    TotalWRBridge = double.Parse(xmlString.ChildNodes[1].ChildNodes[7].InnerText);
                    TotalWRFail = double.Parse(xmlString.ChildNodes[1].ChildNodes[8].InnerText);
                }
                catch
                {
                    TotalTest = 0;
                    TotalWRBridge = 0;
                    TotalWRFail = 0;
                    Save();

                    xmlString.Load("C:\\seagate\\HGA.HST\\Setup\\Sampling.config");
                }

                Mean = sampledatalist.Average();

                Stddev = getStandardDeviation(sampledatalist);
                Highest = sampledatalist.Max();
                Lowest = sampledatalist.Min();
                CurrentZvalue = (Math.Floor(((critz(CurrentZ) * (getStandardDeviation(sampledatalist))) + sampledatalist.Average()) * 100) / 100);
            }
        }



    }
}
