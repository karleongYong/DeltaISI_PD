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
//  [2006/12/13] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Seagate.AAS.UI
{
    public partial class FileListBox : ListBox
    {
        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------

        private string _path = @"c:\";
        private string _extension = @"*";

        // Constructors & Finalizers -------------------------------------------

        public FileListBox()
        {
            InitializeComponent();
        }

        // Properties ----------------------------------------------------------

        [Browsable(true)]
        public string Extension
        {
            get { return _extension; }
        }

        [Browsable(true)]
        public string Path
        {
            get { return _path; }
        }

        // Methods -------------------------------------------------------------

        public void SetPath(string path, string extension)
        {
            _path = path;
            _extension = extension;
            if (_extension[0] != '.')
                _extension = "." + _extension;

            try
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(_path);
                System.IO.FileInfo[] files = dir.GetFiles();

                this.DisplayMember = "Name";
                //this.ValueMember = "Length";
                foreach (System.IO.FileInfo fi in files)
                {
                    if (fi.Extension == _extension || _extension == "*")
                    {
                        this.Items.Add(System.IO.Path.GetFileNameWithoutExtension(fi.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        // Event handlers ------------------------------------------------------

        // Internal methods ----------------------------------------------------

    }
}
