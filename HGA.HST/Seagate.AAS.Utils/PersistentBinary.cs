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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
//using System.Xml.Serialization;
using System.IO;

namespace Seagate.AAS.Utils
{
    /// <summary>
    /// PersistentBinary is a utility class that allows itself be serialized in a binary format.
    /// You must mark all types with Serializable() attribute for this to work.
    /// </summary>
    [Serializable()]
    public class PersistentBinary 
    {
        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------
        protected string fileName;
        
        // Constructors & Finalizers -------------------------------------------
        public PersistentBinary()
        {
            fileName = "PersistentBinary.config";
        }
        
        // Properties ----------------------------------------------------------
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
            FileStream fs = new FileStream(fileName, FileMode.Create);

            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            try 
            {
                formatter.Serialize(fs, this);
            }
            catch (SerializationException e) 
            {
                Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                throw;
            }
            finally 
            {
                fs.Close();
            }

        }

        // Deserializes the class from the config file.
        public virtual void Load()
        {
            // Declare the hashtable reference.
            // Open the file containing the data that you want to deserialize.
            FileStream fs = new FileStream(fileName, FileMode.Open);
            try 
            {
                BinaryFormatter formatter = new BinaryFormatter();

                // Deserialize the object from the file and 
                // assign the reference to the local variable.
                PersistentBinary binObject = (PersistentBinary) formatter.Deserialize(fs);
                UpdateMemberVariables(binObject);
            }
            catch (SerializationException e) 
            {
                Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                throw;
            }
            finally 
            {
                fs.Close();
            }

        }


        // Internal methods ----------------------------------------------------
        protected virtual void UpdateMemberVariables(PersistentBinary persistentBinary)
        {
            this.fileName = persistentBinary.fileName;
        }
    }
}
