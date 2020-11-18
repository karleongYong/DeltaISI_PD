using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace Seagate.AAS.Utils
{
    public class WindowsRegistry
    {
        private RegistryKey _baseRegistryKey;
        private string _subKey = "";

        public WindowsRegistry()
        {
            _baseRegistryKey = Microsoft.Win32.Registry.LocalMachine;
        }

        public WindowsRegistry(RegistryKey baseRegistryKey)
        {
            _baseRegistryKey = baseRegistryKey;
        }

        #region Properties

        public RegistryKey BaseRegistryKey
        {
            get { return _baseRegistryKey; }
            set { _baseRegistryKey = value; }
        }

        public string SubKey
        {
            get { return _subKey; }
            set { _subKey = value; }
        }

        #endregion

        #region Methods

        public String ReadValue(string valueName)
        {
            RegistryKey sk1 = _baseRegistryKey.OpenSubKey(_subKey);
            if (sk1 != null)
            {
                try
                {
                    string retVal = "";
                    RegistryValueKind rvk = sk1.GetValueKind(valueName);
                    switch (rvk.ToString().ToUpper())
                    {
                        case "BINARY": retVal = "binary data"; break;
                        default: retVal = sk1.GetValue(valueName).ToString(); break;
                    }
                    return retVal;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else return null;
        }

        public void WriteValue(string valueName, object data, RegistryValueKind RegistryKind)
        {
            try
            {
                RegistryKey sk1 = _baseRegistryKey.CreateSubKey(_subKey);
                sk1.SetValue(valueName, data, RegistryKind);
                sk1.Close(); // To confirm flushing
                return;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void WriteValueString(string valueName, String strData)
        {
            try
            {
                RegistryKey sk1 = _baseRegistryKey.CreateSubKey(_subKey);
                sk1.SetValue(valueName, strData);
                sk1.Close(); // To confirm flushing
                return;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteValue(string valueName, bool throwOnMissingValue)
        {
            try
            {
                RegistryKey sk1 = _baseRegistryKey.OpenSubKey(_subKey, true);
                if (sk1 != null)
                {
                    sk1.DeleteValue(valueName, throwOnMissingValue);
                    sk1.Close(); // To confirm flushing
                }
                return;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteSubKeyTree()
        {
            if (_subKey != "")
            {
                int lastPos = _subKey.LastIndexOf("\\");
                if (lastPos > 0)
                {
                    string baseKey = _subKey.Substring(0, lastPos);
                    string delKey = _subKey.Substring(lastPos + 1);
                    try
                    {
                        RegistryKey sk1 = _baseRegistryKey.OpenSubKey(baseKey, true);
                        if (sk1 != null)
                        {
                            sk1.DeleteSubKeyTree(delKey);
                            sk1.Close(); // To confirm flushing
                        }
                        return;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                else throw new Exception("Don't allow to delete first level of Registry Key.");
            }
            else throw new Exception("Couldn't delete blank sub key.");
        }



        #endregion

    }
}