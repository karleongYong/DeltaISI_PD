using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.Utils;
using Seagate.AAS.HGA.HST;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.Parsel.Equipment;
using Seagate.AAS.IO.Serial;
using System.IO.Ports;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Data.CumulativeCountofConforming;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Utils;
using XyratexOSC.Settings;
using XyratexOSC.Settings.UI;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Models;
using XyratexOSC.Settings;
using XyratexOSC.Settings.UI;
using XyratexOSC.UI;

namespace Seagate.AAS.HGA.HST.Settings
{
    
    public class GompertzSettings
    {

        private string GompertzSettingFilePath = "C:\\Seagate\\HGA.HST\\Setup\\GompertzSetting.config";
        private SettingsXml _xml;

        public double SSEInitial;
        public double RSQInitial;
        public double amin;
        public double amax;
        public double bmin;
        public double bmax;
        public double cmin;
        public double cmax;
        public double dmin;
        public double dmax;
        public double randomscale;
        public double weight;
        public double adaptiveSearch_A;
        public double adaptiveSearch_B;
        public double adaptiveSearch_C;
        public double adaptiveSearch_D;
        public double HardLimit_RSQ;
        public double HardLimit_SSE;
        public int MaxTest;
        public double Step;
        public int Iteration;
        public bool UseGompertz;
        public double TestTimePerHGA;
        public GompertzCalculationMethod GompertzCalMethod;
        public int Split;
        
        // static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<GompertzSettings> _instance
             = new Lazy<GompertzSettings>(() => new GompertzSettings());

        public GompertzSettings()
        {
            Load();
        }

        public static GompertzSettings Instance
        {
            get
            {
                return _instance.Value;
            }
        }
        public void Load()
        {
            if (this.GompertzSettingFilePath == null)
            {
                return;
            }

            if (!File.Exists(this.GompertzSettingFilePath))
            {
                File.Create(this.GompertzSettingFilePath).Dispose();
                return;
            }

           

            _xml = new SettingsXml(this.GompertzSettingFilePath);
            _xml.OpenSection("Settings");

            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                if (!pi.CanWrite) // Avoid putting data to read only property such as IsAutoRecipe
                    continue;

                // Loop through all properties and save them into config.
                if (pi.PropertyType == typeof(String))
                {
                    pi.SetValue(this, _xml.Read(pi.Name, (String)pi.GetValue(this, null)), null);
                }

                if (pi.PropertyType == typeof(Int32))
                {
                    pi.SetValue(this, _xml.Read(pi.Name, (Int32)pi.GetValue(this, null)), null);
                }

                if (pi.PropertyType == typeof(Double))
                {
                    pi.SetValue(this, _xml.Read(pi.Name, (Double)pi.GetValue(this, null)), null);
                }

                if (pi.PropertyType == typeof(Boolean))
                {
                    pi.SetValue(this, _xml.Read(pi.Name, (Boolean)pi.GetValue(this, null)), null);
                }

                if (pi.PropertyType.IsEnum == true)
                {
                    pi.SetValue(this, _xml.Read(pi.Name, (int)pi.GetValue(this, null)), null);
                }
            }

        //    SSEInitial = _xml.Read("SSEInitial",0.15);
        //    RSQInitial = _xml.Read("RSQInitial",96);
            amin = _xml.Read("amin", 0.01);
            amax = _xml.Read("amax", 0.4);
            bmin = _xml.Read("bmin", 2.0);
            bmax = _xml.Read("bmax",7.0);
            cmin = _xml.Read("cmin",0.3);
            cmax = _xml.Read("cmax",2.0);
            dmin = _xml.Read("dmin",9.0);
            dmax = _xml.Read("dmax",16.0);
        //    randomscale = _xml.Read("randomscale",4);
        //    weight = _xml.Read("weight",1);
        //    adaptiveSearch_A = _xml.Read("adaptiveSearch_A",1);
        //    adaptiveSearch_B = _xml.Read("adaptiveSearch_B",5);
        //    adaptiveSearch_C = _xml.Read("adaptiveSearch_C",5);
        //    adaptiveSearch_D = _xml.Read("adaptiveSearch_D",5);
        //    HardLimit_RSQ = _xml.Read("HardLimit_RSQ",99);
        //    HardLimit_SSE = _xml.Read("HardLimit_SSE",0.06);
        //    MaxTest = _xml.Read("MaxTest",200);
            Step = _xml.Read("Step",0.0001);
        //    Iteration = _xml.Read("Iteration",1500);
            UseGompertz = _xml.Read("UseGompertz",true);
        //    TestTimePerHGA = _xml.Read("TestTimePerHGA",2);
            GompertzCalMethod = _xml.Read("GompertzCalculationMethod", "Random") == "Random"? GompertzCalculationMethod.Random : GompertzCalculationMethod.Fix;
            Split = _xml.Read("Split", 10);
            // _xml.Write("GompertzCalculationMethod", GompertzCalculationMethod.ToString());
            _xml.CloseSection();

        }


        public void Save()
        {
          
            File.Create(this.GompertzSettingFilePath).Dispose();

            _xml = new SettingsXml(this.GompertzSettingFilePath);
            _xml.OpenSection("Settings");

            // Loop through all properties of this type and read from
            // config based on property's name.
            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                if (!pi.CanWrite) // Avoid saving readonly property to config file such as IsAutoRecipe
                    continue;

                if (pi.PropertyType == typeof(String))
                {
                    _xml.Write(pi.Name, (String)pi.GetValue(this, null));
                }
                if (pi.PropertyType == typeof(Int32))
                {
                    _xml.Write(pi.Name, (Int32)pi.GetValue(this, null));
                }

                if (pi.PropertyType == typeof(Double))
                {
                    _xml.Write(pi.Name, (Double)pi.GetValue(this, null));
                }

                if (pi.PropertyType == typeof(Boolean))
                {
                    _xml.Write(pi.Name, (Boolean)pi.GetValue(this, null));
                }
                if (pi.PropertyType.IsEnum == true)
                {
                    _xml.Write(pi.Name, (int)pi.GetValue(this, null));
                }
            }


         //   _xml.Write("SSEInitial", SSEInitial.ToString());
        //    _xml.Write("RSQInitial", RSQInitial.ToString());
            _xml.Write("amin", amin.ToString());
            _xml.Write("amax", amax.ToString());
            _xml.Write("bmin", bmin.ToString());
            _xml.Write("bmax", bmax.ToString());

            _xml.Write("cmin", cmin.ToString());
            _xml.Write("cmax", cmax.ToString());
            _xml.Write("dmin", dmin.ToString());
            _xml.Write("dmax", dmax.ToString());
        //    _xml.Write("randomscale", randomscale.ToString());
        //    _xml.Write("weight", weight.ToString());

        //    _xml.Write("adaptiveSearch_A", adaptiveSearch_A.ToString());
        //    _xml.Write("adaptiveSearch_B", adaptiveSearch_B.ToString());
        //    _xml.Write("adaptiveSearch_C", adaptiveSearch_C.ToString());
        //    _xml.Write("adaptiveSearch_D", adaptiveSearch_D.ToString());
        //    _xml.Write("HardLimit_RSQ", HardLimit_RSQ.ToString());
        //    _xml.Write("HardLimit_SSE", HardLimit_SSE.ToString());

         //   _xml.Write("MaxTest", MaxTest.ToString());
            _xml.Write("Step", Step.ToString());
        //    _xml.Write("Iteration", Iteration.ToString());
            _xml.Write("UseGompertz", UseGompertz.ToString());
        //    _xml.Write("TestTimePerHGA", TestTimePerHGA.ToString());
            _xml.Write("GompertzCalculationMethod", GompertzCalMethod.ToString());
            _xml.Write("Split", Split.ToString());
            _xml.CloseSection();
            _xml.Save();
        }

    }

    
}
