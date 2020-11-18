using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using Seagate.AAS.Utils;
using Seagate.AAS.Parsel.Equipment;
using Seagate.AAS.Parsel.Services;
using Seagate.AAS.Parsel.Hw;
using System.ComponentModel;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.Machine
{
    public class SetupConfigHST
    {

        private SettingsXml _xml;

        public SetupConfigHST(HSTWorkcell Workcell)
        {

            AxesProfile = new AxesProfile();
            DelayTimer = new DelayTimer();
            
            // 2: get the main xml
            string fileName = ServiceManager.DirectoryLocator.GetPath(Folders.Setup) + @"\Automation.config";
            _xml = new SettingsXml(fileName);

            ServiceManager.FormLayout.SetupFolder = HSTSettings.Directories.Setup;
            Load();
        }

        // Properties ----------------------------------------------------------
        public SettingsXml Xml
        { get { return _xml; } }

        public AxesProfile AxesProfile { get; set; }
        public DelayTimer DelayTimer { get; set; }

        public string LastRunRecipeName { get; set; }


        // Methods -------------------------------------------------------------

        public void Load()
        {
            LastRunRecipeName = _xml.Read("LastRunRecipeName", String.Empty);
            AxesProfile.Load("MoveProfile", _xml);
            DelayTimer.Load("DelayTimer", _xml);
        }
        public void Save()
        {
            _xml.Write("LastRunRecipeName", CommonFunctions.Instance.ProductRecipeName);
            AxesProfile.Save("MoveProfile", _xml);
            DelayTimer.Save("DelayTimer", _xml);
            _xml.Save();
        }
    }
    
    public class DelayTimer
    {
        public DelayTimer()
        {
        }
        
        // Methods -------------------------------------------------------------
        public void Load(string section, SettingsXml xml)
        {
            SetupConfigHelper.LoadHelper(section, xml, this);
        }
        public void Save(string section, SettingsXml xml)
        {
            SetupConfigHelper.SaveHelper(section, xml, this);
        }
    }

    public class AxesProfile
    {
        public AxesProfile()
        {
            InputEEZ = new MoveProfileBase();
            PrecisorX = new MoveProfileBase();
            PrecisorY = new MoveProfileBase();
            PrecisorTheta = new MoveProfileBase();
            TestProbeZ = new MoveProfileBase();
            OutputEEZ = new MoveProfileBase();
        }

        public MoveProfileBase InputEEZ { get; set; }
        public MoveProfileBase PrecisorX { get; set; }
        public MoveProfileBase PrecisorY { get; set; }
        public MoveProfileBase PrecisorTheta { get; set; }
        public MoveProfileBase TestProbeZ { get; set; }
        public MoveProfileBase OutputEEZ { get; set; }
        
        // Methods -------------------------------------------------------------
        public void Load(string section, SettingsXml xml)
        {
            SetupConfigHelper.LoadHelper(section, xml, this);
        }
        public void Save(string section, SettingsXml xml)
        {
            SetupConfigHelper.SaveHelper(section, xml, this);
        }

        private MoveProfileBase GetMoveProfile(MoveProfileBase mp)
        {
            MoveProfileBase newMp = new MoveProfileBase();
            {
                newMp.Acceleration = mp.Acceleration;
                newMp.Deceleration = mp.Deceleration;
                newMp.Velocity = mp.Velocity;
            }
            return newMp;
        }
    }

    public class TgaTrayTransferConfig
    {
    }

    public class EndEffector1Configure
    {
    }

    public class EndEffector2Configure
    {
    }

    public class EndEffector3Configure
    {
    }


    class SetupConfigHelper
    {
        public static void LoadHelper(string section, SettingsXml xml, object obj)
        {
            xml.OpenSection(section);
            foreach (PropertyInfo pi in obj.GetType().GetProperties())
            {
                // Loop through all properties and save them into config.
                if (pi.PropertyType == typeof(String))
                {
                    pi.SetValue(obj, xml.Read(pi.Name, (String)pi.GetValue(obj, null)), null);
                }

                if (pi.PropertyType == typeof(Int32))
                {
                    pi.SetValue(obj, xml.Read(pi.Name, (Int32)pi.GetValue(obj, null)), null);
                }

                if (pi.PropertyType == typeof(Double))
                {
                    pi.SetValue(obj, xml.Read(pi.Name, (Double)pi.GetValue(obj, null)), null);
                }

                if (pi.PropertyType == typeof(Boolean))
                {
                    pi.SetValue(obj, xml.Read(pi.Name, (Boolean)pi.GetValue(obj, null)), null);
                }

                if (pi.PropertyType == typeof(MoveProfileBase) && pi.CanWrite)
                {
                    MoveProfileBase mp = new MoveProfileBase();

                    mp.Acceleration = xml.Read(pi.Name + "/" + "Acceleration", 1000);
                    mp.Deceleration = xml.Read(pi.Name + "/" + "Deceleration", 1000);
                    mp.Velocity = xml.Read(pi.Name + "/" + "Velocity", 50);
                    pi.SetValue(obj, mp, null);
                }
            }
            xml.CloseSection();
        }

        public static void SaveHelper(string section, SettingsXml xml, object obj)
        {
            xml.OpenSection(section);
            // Loop through all properties of this type and read from
            // config based on property's name.

            foreach (PropertyInfo pi in obj.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(String))
                {
                    xml.Write(pi.Name, (String)pi.GetValue(obj, null));
                }

                if (pi.PropertyType == typeof(Int32))
                {
                    xml.Write(pi.Name, (Int32)pi.GetValue(obj, null));
                }

                if (pi.PropertyType == typeof(Double))
                {
                    xml.Write(pi.Name, (Double)pi.GetValue(obj, null));
                }

                if (pi.PropertyType == typeof(Boolean))
                {
                    xml.Write(pi.Name, (Boolean)pi.GetValue(obj, null));
                }

                if (pi.PropertyType == typeof(MoveProfileBase) && pi.CanWrite)
                {
                    MoveProfileBase mp = (MoveProfileBase)pi.GetValue(obj, null);
                    xml.Write(pi.Name + "/" + "Acceleration", (Double)mp.Acceleration);
                    xml.Write(pi.Name + "/" + "Deceleration", (Double)mp.Deceleration);
                    xml.Write(pi.Name + "/" + "Velocity", (Double)mp.Velocity);
                }
            }
            xml.CloseSection();
        }
    }



}
