//
//  (c) Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2006/12/18] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;
using System.Xml;
using System.ComponentModel;
using System.Globalization;

namespace Seagate.AAS.Utils
{
    public class SettingsXml
    {
        // Nested declarations -------------------------------------------------

        public class Item<T>
        {
            private string _itemValue;
            private T _defaultValue;

            public Item(string item, T defaultValue)
            {
                _itemValue = item;
                _defaultValue = defaultValue;
            }

            public T ItemValue
            {
                get
                {
                    if (string.IsNullOrEmpty(_itemValue))
                        return _defaultValue;
                    else
                        return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(_itemValue);
                }
            }
        }

        // Member variables ----------------------------------------------------

        private XmlDocument _xmlDocument = new XmlDocument();
        private string _documentPath = ""; //Application.StartupPath + "\\settings.xml";
        private string _root = "settings/";
        private string _section = "";

        public delegate void SettingsChangedHandler();
        public event SettingsChangedHandler OnSettingsChanged;

        // Constructors & Finalizers -------------------------------------------

        public SettingsXml()
        {
        }

        public SettingsXml(string _documentPath)
        {
            Load(_documentPath);
        }

        // Properties ----------------------------------------------------------
        
        public string DocumentPath
        { get { return _documentPath; } }

        public string Section
        {
            get { return _section; }
        }
        
        // Methods -------------------------------------------------------------

        public void OpenSection(string section)
        {
            _section = section; 
            if (_section.Length > 0 && _section[_section.Length-1] != '/')
                _section += '/';
        }

        public void CloseSection()
        {
            _section = "";
        }

        public int Read(string xPath, int defaultValue)
        {
            Item<int> item = new Item<int>(GetSetting(xPath), defaultValue);
            return item.ItemValue;
        }

        public void Write(string xPath, int value)
        {
            PutSetting(xPath, TypeDescriptor.GetConverter(typeof(int)).ConvertToString(value));
        }

        public double Read(string xPath, double defaultValue)
        {
            Item<double> item = new Item<double>(GetSetting(xPath), defaultValue);
            return item.ItemValue;
        }
        public void Write(string xPath, double value)
        {
            PutSetting(xPath, TypeDescriptor.GetConverter(typeof(double)).ConvertToString(value));
        }

        public string Read(string xPath, string defaultValue)
        {
            Item<string> item = new Item<string>(GetSetting(xPath), defaultValue);
            return item.ItemValue;
        }
        public void Write(string xPath, string value)
        {
            PutSetting(xPath, value);
        }

        public bool Read(string xPath, bool defaultValue)
        {
            Item<bool> item = new Item<bool>(GetSetting(xPath), defaultValue);
            return item.ItemValue;
        }
        public void Write(string xPath, bool value)
        {
            PutSetting(xPath, TypeDescriptor.GetConverter(typeof(bool)).ConvertToString(value));
        }




//         public DateTime Read(string xPath, DateTime defaultValue)
//         {
//             CultureInfo cultureInfo = new CultureInfo("en-US");
//             return Convert.ToDateTime(GetSetting(xPath,
//             Convert.ToString(defaultValue, cultureInfo)), cultureInfo);
//         }
// 
//         public void Write(string xPath, DateTime value)
//         {
//             CultureInfo cultureInfo = new CultureInfo("en-US");
//             PutSetting(xPath, Convert.ToString(value, cultureInfo));
//         }

        public void Load(string _documentPath)
        {
            this._documentPath = _documentPath;
            try
            {
                _xmlDocument.Load(_documentPath);
            }
            catch
            {
                _xmlDocument.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?><settings></settings>");
            }
        }

        public void Save()
        {
            _xmlDocument.Save(_documentPath);

            if (OnSettingsChanged != null)
                OnSettingsChanged();
        }

        public bool ItemExist(string xPath)
        {
            XmlNode xmlNode = _xmlDocument.SelectSingleNode(_root + _section + xPath);
            return (xmlNode != null);

        }

        // Internal methods ----------------------------------------------------
        
        private string GetSetting(string xPath)
        {
            XmlNode xmlNode = _xmlDocument.SelectSingleNode(_root +_section + xPath);
            return xmlNode!=null?xmlNode.InnerText:null; 
        }

        private void PutSetting(string xPath, string value)
        {
            XmlNode xmlNode = _xmlDocument.SelectSingleNode(_root + _section + xPath);
            if (xmlNode == null) 
            { 
                xmlNode = CreateMissingNode(_root + _section + xPath); 
            }
            xmlNode.InnerText = value;
        }

        private XmlNode CreateMissingNode(string xPath)
        {
            string[] xPathSections = xPath.Split('/');
            string currentXPath = "";
            XmlNode testNode = null;
            XmlNode currentNode = _xmlDocument.SelectSingleNode("settings");
            foreach (string xPathSection in xPathSections)
            {
                currentXPath += xPathSection;
                testNode = _xmlDocument.SelectSingleNode(currentXPath);
                if (testNode == null) { currentNode.InnerXml += "<" + xPathSection + "></" + xPathSection + ">"; }
                currentNode = _xmlDocument.SelectSingleNode(currentXPath);
                currentXPath += "/";
            }
            return currentNode;
        }
    }
}
