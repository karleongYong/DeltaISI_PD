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
//  [2007/01/09] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Seagate.AAS.Utils;

namespace Seagate.AAS.Parsel.Services
{
    public class FormLayout : IService
    {
        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------
        
        private Dictionary<string, FormPosition> _fromLayoutTalbe;
        private SettingsXml _xml;

        // Constructors & Finalizers -------------------------------------------
        
        internal FormLayout()
        {
            _fromLayoutTalbe = new Dictionary<string, FormPosition>();
            
            string filePath = Path.GetDirectoryName(this.GetType().Assembly.Location) + @"\FormLayout.xml";
            _xml = new SettingsXml(filePath);
        }

        // Properties ----------------------------------------------------------

        public Folder SetupFolder
        { 
            set 
            { 
                string filePath = ServiceManager.DirectoryLocator.GetPath(value) + @"\FormLayout.xml";
                _xml = new SettingsXml(filePath);
            } 
        }

        // Methods -------------------------------------------------------------

        public void InitializeService()
        {
        }

        public void UnloadService()
        {
            if (_xml != null)
                Save();
        }

        public void RegisterForm(string name, Form form)
        {
            if (!_fromLayoutTalbe.ContainsKey(name))
            {
                FormPosition pos = new FormPosition(form);
                Load(name, pos);
                _fromLayoutTalbe.Add(name, pos);
            }
            else
            {
                FormPosition pos = _fromLayoutTalbe[name];
                pos.Parent = form;
            }
        }

        public Form GetForm(string name)
        {
            if (_fromLayoutTalbe.ContainsKey(name))
                return _fromLayoutTalbe[name].Parent;
            else
                return null;
        }

        public void Save()
        {
            foreach (KeyValuePair<string, FormPosition> kvp in _fromLayoutTalbe)
            {
                _xml.OpenSection(kvp.Key);
                _xml.Write("X", kvp.Value.X);
                _xml.Write("Y", kvp.Value.Y);
                _xml.Write("Width", kvp.Value.Width);
                _xml.Write("Height", kvp.Value.Height);
                _xml.Write("WindowState", kvp.Value.WindowState);
                _xml.CloseSection();
            }
            _xml.Save();
        }

        // Event handlers ------------------------------------------------------
        
        // Internal methods ----------------------------------------------------

        private void Load(string name, FormPosition pos)
        {
            _xml.OpenSection(name);
            pos.X = _xml.Read("X", FormPosition.Undefined);
            pos.Y = _xml.Read("Y", FormPosition.Undefined);
            pos.Width = _xml.Read("Width", FormPosition.Undefined);
            pos.Height = _xml.Read("Height", FormPosition.Undefined);
            pos.WindowState = _xml.Read("WindowState", FormPosition.Undefined);
            _xml.CloseSection();
        }
    }
}
