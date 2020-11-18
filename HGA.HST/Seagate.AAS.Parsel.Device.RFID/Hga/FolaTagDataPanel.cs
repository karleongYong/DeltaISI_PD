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

namespace Seagate.AAS.Parsel.Device.RFID.Hga
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
            txtWritecount.Text = tag.WriteCount.ToString();
            lblChecksum.Text = string.Format("0x{0:X2}", tag.CheckSum);

            txtWorkorder.Text = tag.WorkOrder;
            txtVersion.Text = tag.WorkOrderVersion.ToString();
            txtLaststep.Text = tag.LastStep.ToString();

            txtCarrierId.Text = tag.CarrierID;

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
        }

        public void UpdateTag(FolaTagData tag)
        {
            tag.WorkOrder = txtWorkorder.Text;
            tag.WorkOrderVersion = Convert.ToChar(txtVersion.Text);
            tag.LastStep = int.Parse(txtLaststep.Text);       
            tag.CarrierID = txtCarrierId.Text;      
                  
            tag[0].HgaSN = txtHgaSerialNum1.Text;
            tag[1].HgaSN = txtHgaSerialNum2.Text;
            tag[2].HgaSN = txtHgaSerialNum3.Text;
            tag[3].HgaSN = txtHgaSerialNum4.Text;
            tag[4].HgaSN = txtHgaSerialNum5.Text;
            tag[5].HgaSN = txtHgaSerialNum6.Text;
            tag[6].HgaSN = txtHgaSerialNum7.Text;
            tag[7].HgaSN = txtHgaSerialNum8.Text;
            tag[8].HgaSN = txtHgaSerialNum9.Text;
            tag[9].HgaSN = txtHgaSerialNum10.Text;

            tag[0].Status = txtHeadstatus1.Text == "" ? ' ' : txtHeadstatus1.Text[0];
            tag[1].Status = txtHeadstatus2.Text == "" ? ' ' : txtHeadstatus2.Text[0];
            tag[2].Status = txtHeadstatus3.Text == "" ? ' ' : txtHeadstatus3.Text[0];
            tag[3].Status = txtHeadstatus4.Text == "" ? ' ' : txtHeadstatus4.Text[0];
            tag[4].Status = txtHeadstatus5.Text == "" ? ' ' : txtHeadstatus5.Text[0];
            tag[5].Status = txtHeadstatus6.Text == "" ? ' ' : txtHeadstatus6.Text[0];
            tag[6].Status = txtHeadstatus7.Text == "" ? ' ' : txtHeadstatus7.Text[0];
            tag[7].Status = txtHeadstatus8.Text == "" ? ' ' : txtHeadstatus8.Text[0];
            tag[8].Status = txtHeadstatus9.Text == "" ? ' ' : txtHeadstatus9.Text[0];
            tag[9].Status = txtHeadstatus10.Text == "" ? ' ' : txtHeadstatus10.Text[0];


            //process info
            tag.ProcStep[0].StationCode = txtStep1.Text == "" ? ' ' : txtStep1.Text[0];
            tag.ProcStep[1].StationCode = txtStep2.Text == "" ? ' ' : txtStep2.Text[0];
            tag.ProcStep[2].StationCode = txtStep3.Text == "" ? ' ' : txtStep3.Text[0];
            tag.ProcStep[3].StationCode = txtStep4.Text == "" ? ' ' : txtStep4.Text[0];
            tag.ProcStep[4].StationCode = txtStep5.Text == "" ? ' ' : txtStep5.Text[0];
            tag.ProcStep[5].StationCode = txtStep6.Text == "" ? ' ' : txtStep6.Text[0];
            tag.ProcStep[6].StationCode = txtStep7.Text == "" ? ' ' : txtStep7.Text[0];
            tag.ProcStep[7].StationCode = txtStep8.Text == "" ? ' ' : txtStep8.Text[0];
            tag.ProcStep[8].StationCode = txtStep9.Text == "" ? ' ' : txtStep9.Text[0];
            tag.ProcStep[9].StationCode = txtStep10.Text == "" ? ' ' : txtStep10.Text[0];
            tag.ProcStep[10].StationCode = txtStep11.Text == "" ? ' ' : txtStep11.Text[0];
            tag.ProcStep[11].StationCode = txtStep12.Text == "" ? ' ' : txtStep12.Text[0];
            tag.ProcStep[12].StationCode = txtStep13.Text == "" ? ' ' : txtStep13.Text[0];
            tag.ProcStep[13].StationCode = txtStep14.Text == "" ? ' ' : txtStep14.Text[0];
 
            tag.ProcStep[0].ProcessRecipe = txtFilename1.Text;
            tag.ProcStep[1].ProcessRecipe = txtFilename2.Text;
            tag.ProcStep[2].ProcessRecipe = txtFilename3.Text;
            tag.ProcStep[3].ProcessRecipe = txtFilename4.Text;
            tag.ProcStep[4].ProcessRecipe = txtFilename5.Text;
            tag.ProcStep[5].ProcessRecipe = txtFilename6.Text;
            tag.ProcStep[6].ProcessRecipe = txtFilename7.Text;
            tag.ProcStep[7].ProcessRecipe = txtFilename8.Text;
            tag.ProcStep[8].ProcessRecipe = txtFilename9.Text;
            tag.ProcStep[9].ProcessRecipe = txtFilename10.Text;
            tag.ProcStep[10].ProcessRecipe= txtFilename11.Text;
            tag.ProcStep[11].ProcessRecipe= txtFilename12.Text;
            tag.ProcStep[12].ProcessRecipe= txtFilename13.Text;
            tag.ProcStep[13].ProcessRecipe= txtFilename14.Text;
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
            txtWorkorder.ReadOnly = _readOnly;
            txtVersion.ReadOnly = _readOnly;
            txtLaststep.ReadOnly = _readOnly;

            //txtCarrierId.ReadOnly = _readOnly;

            txtHgaSerialNum1.ReadOnly = _readOnly;
            txtHgaSerialNum2.ReadOnly = _readOnly;
            txtHgaSerialNum3.ReadOnly = _readOnly;
            txtHgaSerialNum4.ReadOnly = _readOnly;
            txtHgaSerialNum5.ReadOnly = _readOnly;
            txtHgaSerialNum6.ReadOnly = _readOnly;
            txtHgaSerialNum7.ReadOnly = _readOnly;
            txtHgaSerialNum8.ReadOnly = _readOnly;
            txtHgaSerialNum9.ReadOnly = _readOnly;
            txtHgaSerialNum10.ReadOnly = _readOnly;

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

            txtStep1.ReadOnly = _readOnly;
            txtStep2.ReadOnly = _readOnly;
            txtStep3.ReadOnly = _readOnly;
            txtStep4.ReadOnly = _readOnly;
            txtStep5.ReadOnly = _readOnly;
            txtStep6.ReadOnly = _readOnly;
            txtStep7.ReadOnly = _readOnly;
            txtStep8.ReadOnly = _readOnly;
            txtStep9.ReadOnly = _readOnly;
            txtStep10.ReadOnly = _readOnly;
            txtStep11.ReadOnly = _readOnly;
            txtStep12.ReadOnly = _readOnly;
            txtStep13.ReadOnly = _readOnly;
            txtStep14.ReadOnly = _readOnly;

            txtFilename1.ReadOnly = _readOnly;
            txtFilename2.ReadOnly = _readOnly;
            txtFilename3.ReadOnly = _readOnly;
            txtFilename4.ReadOnly = _readOnly;
            txtFilename5.ReadOnly = _readOnly;
            txtFilename6.ReadOnly = _readOnly;
            txtFilename7.ReadOnly = _readOnly;
            txtFilename8.ReadOnly = _readOnly;
            txtFilename9.ReadOnly = _readOnly;
            txtFilename10.ReadOnly = _readOnly;
            txtFilename11.ReadOnly = _readOnly;
            txtFilename12.ReadOnly = _readOnly;
            txtFilename13.ReadOnly = _readOnly;
            txtFilename14.ReadOnly = _readOnly;
        }
    }
}
