//
//  (c) Copyright 2007 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2007/09/28] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Device.RFID.Hga;

namespace Seagate.AAS.HGA.HST.UI.Utils
{
    public partial class FolaTagDataPanel : UserControl
    {
        // Nested declarations -------------------------------------------------
        
        // Member variables ----------------------------------------------------

        private bool _readOnly = true;

        // Constructors & Finalizers -------------------------------------------

        public FolaTagDataPanel()
        {
            InitializeComponent();
            SetControls();
        }

        // Properties ----------------------------------------------------------
        
	    public bool ReadOnly
	    {
		    get { return _readOnly; }
		    set { _readOnly = value; SetControls(); }
	    }

        // Methods -------------------------------------------------------------

        public void DisplayTagData(FolaTagData tag)
        {
            textBoxVersion.Text = tag.WorkOrderVersion.ToString();
            txtWritecount.Text = tag.WriteCount.ToString();
            lblChecksum.Text = tag.CheckSum.ToString();
            //int lastStep = tag.LastStep + 1;
            //txtLaststep.Text = lastStep.ToString();
            txtLaststep.Text = tag.LastStep.ToString();
            txtCarrierId.Text = tag.CarrierID;
            txtWorkorder.Text = tag.WorkOrder;

            // serial numbers
            txtHgaSerialNum1.Text = tag[0].HgaSN;
            txtHgaSerialNum2.Text = tag[1].HgaSN;
            txtHgaSerialNum3.Text = tag[2].HgaSN;
            txtHgaSerialNum4.Text = tag[3].HgaSN;
            txtHgaSerialNum5.Text = tag[4].HgaSN;
            txtHgaSerialNum6.Text = tag[5].HgaSN;
            txtHgaSerialNum7.Text = tag[6].HgaSN;
            txtHgaSerialNum8.Text = tag[7].HgaSN;
            txtHgaSerialNum9.Text = tag[8].HgaSN;
            txtHgaSerialNum10.Text = tag[9].HgaSN;

            txtHeadNum1.Text = tag[0].HgaSN;
            txtHeadNum2.Text = tag[1].HgaSN;
            txtHeadNum3.Text = tag[2].HgaSN;
            txtHeadNum4.Text = tag[3].HgaSN;
            txtHeadNum5.Text = tag[4].HgaSN;
            txtHeadNum6.Text = tag[5].HgaSN;
            txtHeadNum7.Text = tag[6].HgaSN;
            txtHeadNum8.Text = tag[7].HgaSN;
            txtHeadNum9.Text = tag[8].HgaSN;
            txtHeadNum10.Text = tag[9].HgaSN;

            txtHeadstatus1.Text = tag[0].Status.ToString();
            txtHeadstatus2.Text = tag[1].Status.ToString();
            txtHeadstatus3.Text = tag[2].Status.ToString();
            txtHeadstatus4.Text = tag[3].Status.ToString();
            txtHeadstatus5.Text = tag[4].Status.ToString();
            txtHeadstatus6.Text = tag[5].Status.ToString();
            txtHeadstatus7.Text = tag[6].Status.ToString();
            txtHeadstatus8.Text = tag[7].Status.ToString();
            txtHeadstatus9.Text = tag[8].Status.ToString();
            txtHeadstatus10.Text = tag[9].Status.ToString();

            // process info
            txtStep1.Text = tag.ProcStep[0].StationCode.ToString();
            txtStep2.Text = tag.ProcStep[1].StationCode.ToString();
            txtStep3.Text = tag.ProcStep[2].StationCode.ToString();
            txtStep4.Text = tag.ProcStep[3].StationCode.ToString();
            txtStep5.Text = tag.ProcStep[4].StationCode.ToString();
            txtStep6.Text = tag.ProcStep[5].StationCode.ToString();
            txtStep7.Text = tag.ProcStep[6].StationCode.ToString();
            txtStep8.Text = tag.ProcStep[7].StationCode.ToString();
            txtStep9.Text = tag.ProcStep[8].StationCode.ToString();
            txtStep10.Text = tag.ProcStep[9].StationCode.ToString();
            txtStep11.Text = tag.ProcStep[10].StationCode.ToString();
            txtStep12.Text = tag.ProcStep[11].StationCode.ToString();
            txtStep13.Text = tag.ProcStep[12].StationCode.ToString();
            txtStep14.Text = tag.ProcStep[13].StationCode.ToString();

            txtFilename1.Text = tag.ProcStep[0].ProcessRecipe;
            txtFilename2.Text = tag.ProcStep[1].ProcessRecipe;
            txtFilename3.Text = tag.ProcStep[2].ProcessRecipe;
            txtFilename4.Text = tag.ProcStep[3].ProcessRecipe;
            txtFilename5.Text = tag.ProcStep[4].ProcessRecipe;
            txtFilename6.Text = tag.ProcStep[5].ProcessRecipe;
            txtFilename7.Text = tag.ProcStep[6].ProcessRecipe;
            txtFilename8.Text = tag.ProcStep[7].ProcessRecipe;
            txtFilename9.Text = tag.ProcStep[8].ProcessRecipe;
            txtFilename10.Text = tag.ProcStep[9].ProcessRecipe;
            txtFilename11.Text = tag.ProcStep[10].ProcessRecipe;
            txtFilename12.Text = tag.ProcStep[11].ProcessRecipe;
            txtFilename13.Text = tag.ProcStep[12].ProcessRecipe;
            txtFilename14.Text = tag.ProcStep[13].ProcessRecipe;

            txtProcessRcp.Text = tag.CurrentProcessStep.ProcessRecipe;
            txtStationCode.Text = tag.CurrentProcessStep.StationCode.ToString();
            txtProcessStep.Text = tag.CurrentProcessStep.StationCode.ToString();//ZJ marked
        }

        public void UpdateTag(FolaTagData tag)
        {
            tag.WorkOrderVersion = Convert.ToChar(textBoxVersion.Text);
            //int lastStep = int.Parse(txtLaststep.Text) - 1; 
            //tag.LastStep         = lastStep ;       
            tag.LastStep = int.Parse(txtLaststep.Text);
            tag.CarrierID        = txtCarrierId.Text;      
            tag.WorkOrder        = txtWorkorder.Text;      

            tag[0].HgaSN = txtHeadNum1.Text ;
            tag[1].HgaSN = txtHeadNum2.Text;
            tag[2].HgaSN = txtHeadNum3.Text;
            tag[3].HgaSN = txtHeadNum4.Text;
            tag[4].HgaSN = txtHeadNum5.Text;
            tag[5].HgaSN = txtHeadNum6.Text;
            tag[6].HgaSN = txtHeadNum7.Text;
            tag[7].HgaSN = txtHeadNum8.Text;
            tag[8].HgaSN = txtHeadNum9.Text;
            tag[9].HgaSN = txtHeadNum10.Text;


            //ZJ Marked
            //tag[0].out = txtHeadstatus1.Text[0] ;
            //tag[1].OutgoingStatus = txtHeadstatus2.Text[0] ;
            //tag[2].OutgoingStatus = txtHeadstatus3.Text[0] ;
            //tag[3].OutgoingStatus = txtHeadstatus4.Text[0] ;
            //tag[4].OutgoingStatus = txtHeadstatus5.Text[0] ;
            //tag[5].OutgoingStatus = txtHeadstatus6.Text[0] ;
            //tag[6].OutgoingStatus = txtHeadstatus7.Text[0] ;
            //tag[7].OutgoingStatus = txtHeadstatus8.Text[0] ;
            //tag[8].OutgoingStatus = txtHeadstatus9.Text[0] ;
            //tag[9].OutgoingStatus = txtHeadstatus10.Text[0];
        }

        /// <summary>
        /// Clear tag data and update UI panel
        /// </summary>
        /// <param name="tag"></param>
        public void ClearTag(FolaTagData tag)
        {
            tag.Clear();
            DisplayTagData(tag);
        }

        // Event handlers ------------------------------------------------------
        
        // Internal methods ----------------------------------------------------
        
        private void SetControls()
        {
            txtLaststep.ReadOnly     = _readOnly;
            txtWorkorder.ReadOnly    = _readOnly;
            textBoxVersion.ReadOnly  = _readOnly;
           

            txtHeadNum1.ReadOnly = _readOnly;
            txtHeadNum2.ReadOnly = _readOnly;
            txtHeadNum3.ReadOnly = _readOnly;
            txtHeadNum4.ReadOnly = _readOnly;
            txtHeadNum5.ReadOnly = _readOnly;
            txtHeadNum6.ReadOnly = _readOnly;
            txtHeadNum7.ReadOnly = _readOnly;
            txtHeadNum8.ReadOnly = _readOnly;
            txtHeadNum9.ReadOnly = _readOnly;
            txtHeadNum10.ReadOnly =_readOnly;

            txtHeadstatus1.ReadOnly = _readOnly;
            txtHeadstatus2.ReadOnly = _readOnly;
            txtHeadstatus3.ReadOnly = _readOnly;
            txtHeadstatus4.ReadOnly = _readOnly;
            txtHeadstatus5.ReadOnly = _readOnly;
            txtHeadstatus6.ReadOnly = _readOnly;
            txtHeadstatus7.ReadOnly = _readOnly;
            txtHeadstatus8.ReadOnly = _readOnly;
            txtHeadstatus9.ReadOnly = _readOnly;
            txtHeadstatus10.ReadOnly =_readOnly;

        }
    }
}
