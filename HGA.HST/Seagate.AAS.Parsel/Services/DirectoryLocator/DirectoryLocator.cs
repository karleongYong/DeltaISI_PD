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
//  [9/26/2005] Sabrina Murray
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using Seagate.AAS.Utils;

namespace Seagate.AAS.Parsel.Services
{

    public class Folder : EnumItem
    {
        public Folder(string name) : base(name) { }
    }

    /// <summary>
    /// Predefined application level directories. Need to be registered before use.
    /// </summary>
    public class Folders
    {
        public static readonly Folder App = new Folder("App"); // application install root directory
        public static readonly Folder Bin = new Folder("Binaries");
        public static readonly Folder Log = new Folder("Logging");
        public static readonly Folder Setup = new Folder("Setup");
        public static readonly Folder Data = new Folder("Data");
        public static readonly Folder Doc = new Folder("Documentation");
    }


    /// <summary>
    /// DirectoryLocator
    ///		Used by workcells and such to get/store directory paths
    /// </summary>
    public class DirectoryLocator : IService
    {
        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------

        private Dictionary<Folder, string> _directories = new Dictionary<Folder, string>();

        // Constructors & Finalizers -------------------------------------------

        internal DirectoryLocator()
        {
        }

        // Properties ----------------------------------------------------------

        // Methods -------------------------------------------------------------

        public string GetPath(Folder directory)
        {
            return _directories[directory];
        }

        public void RegisterPath(Folder directory, string path)
        {
            if (!_directories.ContainsKey(directory))
            {
                _directories.Add(directory, path);
            }
            else
            {
                _directories[directory] = path;
            }

            if (!System.IO.Directory.Exists(path))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(path);
                    //System.Windows.Forms.MessageBox.Show("Application directory \" " + path + "\" created.");
                }
                catch
                {
                    throw;
                }
                //catch (Exception ex)
                //{
                //    System.Windows.Forms.MessageBox.Show(ex.Message + ": " + string.Format("'{0}' is not a valid directory.", path), "Not a directory");
                //    throw;
                //}
            }
        }

        // 		public void Save(SettingsXml config)
        // 		{
        //             config.Write("Directories/Log", AppDirectories.Log.ToString());
        //             config.Write("Directories/Setup", AppDirectories.Setup.ToString());
        //             config.Save();
        // 
        // 		}
        // 
        //         public bool Load(SettingsXml config)
        // 		{
        // 
        //             AppDirectories.Bin.Load();
        //             AppDirectories.Log.Load();
        //             AppDirectories.Setup.Load();
        // 		}

        #region IService Members

        void IService.InitializeService()
        {
            // TODO:  Add DirectoryLocator.InitializeService implementation
        }

        void IService.UnloadService()
        {
            // TODO:  Add DirectoryLocator.UnloadService implementation
        }

        #endregion

        // Internal methods ---------------------------------------------------- 

    }
}

