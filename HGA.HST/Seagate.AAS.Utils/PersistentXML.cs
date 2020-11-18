//
//
//  © Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [9/2/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows.Forms;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Data;
using Seagate.AAS.Utils;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace Seagate.AAS.Utils
{
    public class PersistentXML
    {
        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------
        
        protected string fileName;
        
        // Constructors & Finalizers -------------------------------------------
        
        public PersistentXML()
        {
            fileName = "PersistentXML.config";
        }

        public PersistentXML(string fileName)
        {
            this.fileName = fileName;
        }
        
        // Properties ----------------------------------------------------------
        
        //[XmlIgnore]
        [BrowsableAttribute(false)]         // hide when using PropertyGrid to display properties in derived classes
        [ReadOnlyAttribute(true)]
        public string FileName 
        {
            get{return fileName;}
            set
            {
                if(value != fileName)
                {
                    fileName = value;
                }
            }
        }
        
        // Methods -------------------------------------------------------------
        
        // Serializes the class to the config file
        // if any of the settings have changed.
        public void Save()
        {
            StreamWriter myWriter = null;
            XmlSerializer mySerializer = null;
            try
            {
                // Create an XmlSerializer for the 
                // PersistentXML type.
                mySerializer = new XmlSerializer(this.GetType());
                myWriter = new StreamWriter(fileName, false);
                // Serialize this instance of the PersistentXML 
                // class to the config file.
                mySerializer.Serialize(myWriter, this);
            }
            catch(Exception ex)
            {
				MessageBox.Show(ex.Message + ex.InnerException);
				
            }
            finally
            {
                // If the FileStream is open, close it.
                if(myWriter != null)
                {
                    myWriter.Close();
                }
            }
        }

        // Deserializes the class from the config file.
        public bool Load()
        {
            XmlSerializer mySerializer = null;
            FileStream myFileStream = null;
            bool fileExists = false;

            try
            {
                // Create an XmlSerializer for the PersistentXML type.
                mySerializer = new XmlSerializer(this.GetType());
                FileInfo fi = new FileInfo(fileName);
                
                // If the config file exists, open it.
                if(fi.Exists)
                {
                    myFileStream = fi.OpenRead();
                    // Create a new instance of the PersistentXML by
                    // deserializing the config file.
                    PersistentXML myAppSettings = (PersistentXML) mySerializer.Deserialize(myFileStream);

                    UpdateMemberVariables(myAppSettings);
                    fileExists = true;
                }
            }
//            catch(Exception ex)
//            {
//                MessageBox.Show(ex.Message);
//            }
            finally
            {
                // If the FileStream is open, close it.
                if(myFileStream != null)
                {
                    myFileStream.Close();
                }
            }
            return fileExists;
        }

        // Internal methods ----------------------------------------------------

        protected virtual void UpdateMemberVariables(PersistentXML persistentXML)
        {
            foreach (PropertyInfo toProperty in this.GetType().GetProperties())
            {
                // do not replace special "FileName" property with persisted value
                if (toProperty.Name != "FileName")
                {
                    toProperty.SetValue(this, persistentXML.GetType().GetProperty(toProperty.Name).GetValue(persistentXML, null), null);
                }
            }
        }
    }
}
