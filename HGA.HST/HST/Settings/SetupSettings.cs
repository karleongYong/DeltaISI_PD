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
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Utils;
using XyratexOSC.Settings;
using XyratexOSC.Settings.UI;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Models;
using XyratexOSC.Settings;
using XyratexOSC.Settings.UI;

namespace Seagate.AAS.HGA.HST.Settings
{
    public class SetupSettings
    {        
        private const string UNKNOWN = "Unknown";                
        public SettingsEditor _settingsEditor;
        protected SettingsDocument _settingsDoc;                 
       
        public EventHandler OnSettingsChanged;
           

        #region Singleton

        // static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<SetupSettings> _instance
             = new Lazy<SetupSettings>(() => new SetupSettings());

        // accessor for instance
        public static SetupSettings Instance
        {
            get
            {
                return _instance.Value;
            }
        }
        
        public SetupSettings()
        {
            MoveProfile = new MoveProfileSettings();
            Delay = new DelaySettings();
            Load();            
        }        

        #endregion       

        [ReadOnly(false)]
        [Browsable(true)]
        [DisplayName("MoveProfile")]
        public MoveProfileSettings MoveProfile
        {
            get;
            private set;
        }

        [ReadOnly(false)]
        [Browsable(true)]
        [DisplayName("Delay")]
        public DelaySettings Delay
        {
            get;
            private set;
        }

        private SettingsXml _xml;                
        
        public void Load()
        {
            if (HSTMachine.Workcell.HSTSettings.SetupSettingsFilePath == null)
            {
                return;
            }

            if (!File.Exists(HSTMachine.Workcell.HSTSettings.SetupSettingsFilePath))
            {
                File.Create(HSTMachine.Workcell.HSTSettings.SetupSettingsFilePath).Dispose();
                return;
            }

            _xml = new SettingsXml(HSTMachine.Workcell.HSTSettings.SetupSettingsFilePath);
            _xml.OpenSection("Config");

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

            //EquipmentType = _xml.Read("EquipmentType", "ZCU");            
            _xml.CloseSection();

            _xml.OpenSection("MoveProfile/InputEEZ/");
            MoveProfile.InputEEZ.Acceleration = int.Parse(_xml.Read("Acceleration", "0"));
            MoveProfile.InputEEZ.Deceleration = int.Parse(_xml.Read("Deceleration", "0"));
            MoveProfile.InputEEZ.Velocity = int.Parse(_xml.Read("Velocity", "0"));
            _xml.CloseSection();

            _xml.OpenSection("MoveProfile/PrecisorX/");
            MoveProfile.PrecisorX.Acceleration = int.Parse(_xml.Read("Acceleration", "0"));
            MoveProfile.PrecisorX.Deceleration = int.Parse(_xml.Read("Deceleration", "0"));
            MoveProfile.PrecisorX.Velocity = int.Parse(_xml.Read("Velocity", "0"));
            _xml.CloseSection();

            _xml.OpenSection("MoveProfile/PrecisorY/");
            MoveProfile.PrecisorY.Acceleration = int.Parse(_xml.Read("Acceleration", "0"));
            MoveProfile.PrecisorY.Deceleration = int.Parse(_xml.Read("Deceleration", "0"));
            MoveProfile.PrecisorY.Velocity = int.Parse(_xml.Read("Velocity", "0"));
            _xml.CloseSection();

            _xml.OpenSection("MoveProfile/PrecisorTheta/");
            MoveProfile.PrecisorTheta.Acceleration = int.Parse(_xml.Read("Acceleration", "0"));
            MoveProfile.PrecisorTheta.Deceleration = int.Parse(_xml.Read("Deceleration", "0"));
            MoveProfile.PrecisorTheta.Velocity = int.Parse(_xml.Read("Velocity", "0"));
            _xml.CloseSection();

            _xml.OpenSection("MoveProfile/TestProbeZ/");
            MoveProfile.TestProbeZ.Acceleration = int.Parse(_xml.Read("Acceleration", "0"));
            MoveProfile.TestProbeZ.Deceleration = int.Parse(_xml.Read("Deceleration", "0"));
            MoveProfile.TestProbeZ.Velocity = int.Parse(_xml.Read("Velocity", "0"));
            _xml.CloseSection();

            _xml.OpenSection("MoveProfile/OutputEEZ/");
            MoveProfile.OutputEEZ.Acceleration = int.Parse(_xml.Read("Acceleration", "0"));
            MoveProfile.OutputEEZ.Deceleration = int.Parse(_xml.Read("Deceleration", "0"));
            MoveProfile.OutputEEZ.Velocity = int.Parse(_xml.Read("Velocity", "0"));
            _xml.CloseSection();

            _xml.OpenSection("DelayTimer");
            Delay.VacuumOnDelay = int.Parse(_xml.Read("VacuumOnDelay", "0"));
            Delay.VacuumOffDelay = int.Parse(_xml.Read("VacuumOffDelay", "0"));
            _xml.CloseSection();

            if (OnSettingsChanged != null)
                OnSettingsChanged(this, null);
        }

        public void Save()
        {            
            if (OnSettingsChanged != null)
                OnSettingsChanged(this, null);            

            _xml.OpenSection("Config");

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


            _xml.OpenSection("MoveProfile/InputEEZ/");
            _xml.Write("Acceleration", MoveProfile.InputEEZ.Acceleration.ToString());
            _xml.Write("Deceleration", MoveProfile.InputEEZ.Deceleration.ToString());
            _xml.Write("Velocity", MoveProfile.InputEEZ.Velocity.ToString());
            _xml.CloseSection();

            _xml.OpenSection("MoveProfile/PrecisorX/");
            _xml.Write("Acceleration", MoveProfile.PrecisorX.Acceleration.ToString());
            _xml.Write("Deceleration", MoveProfile.PrecisorX.Deceleration.ToString());
            _xml.Write("Velocity", MoveProfile.PrecisorX.Velocity.ToString());
            _xml.CloseSection();

            _xml.OpenSection("MoveProfile/PrecisorY/");
            _xml.Write("Acceleration", MoveProfile.PrecisorY.Acceleration.ToString());
            _xml.Write("Deceleration", MoveProfile.PrecisorY.Deceleration.ToString());
            _xml.Write("Velocity", MoveProfile.PrecisorY.Velocity.ToString());
            _xml.CloseSection();

            _xml.OpenSection("MoveProfile/PrecisorTheta/");
            _xml.Write("Acceleration", MoveProfile.PrecisorTheta.Acceleration.ToString());
            _xml.Write("Deceleration", MoveProfile.PrecisorTheta.Deceleration.ToString());
            _xml.Write("Velocity", MoveProfile.PrecisorTheta.Velocity.ToString());
            _xml.CloseSection();

            _xml.OpenSection("MoveProfile/TestProbeZ/");
            _xml.Write("Acceleration", MoveProfile.TestProbeZ.Acceleration.ToString());
            _xml.Write("Deceleration", MoveProfile.TestProbeZ.Deceleration.ToString());
            _xml.Write("Velocity", MoveProfile.TestProbeZ.Velocity.ToString());
            _xml.CloseSection();

            _xml.OpenSection("MoveProfile/OutputEEZ/");
            _xml.Write("Acceleration", MoveProfile.OutputEEZ.Acceleration.ToString());
            _xml.Write("Deceleration", MoveProfile.OutputEEZ.Deceleration.ToString());
            _xml.Write("Velocity", MoveProfile.OutputEEZ.Velocity.ToString());
            _xml.CloseSection();

            _xml.OpenSection("DelayTimer");
            _xml.Write("VacuumOnDelay", Delay.VacuumOnDelay.ToString());
            _xml.Write("VacuumOffDelay", Delay.VacuumOffDelay.ToString());
            _xml.CloseSection();


            _xml.Save();

            HSTMachine.Instance.MainForm.getPanelCommand().DebugButtonsVisibility();
        }

        public void SaveSettingsToFile(object sender, EventArgs e)
        {            
            Save();
        }        

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[Config]");
            foreach (PropertyInfo pi in this.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(String) ||
                    pi.PropertyType == typeof(Boolean) ||
                    pi.PropertyType == typeof(Double) ||
                    pi.PropertyType == typeof(Int32))
                {
                    sb.AppendLine(pi.Name + "," + pi.GetValue(this, null));
                }
            }
            return sb.ToString();
        }        
    }

}
