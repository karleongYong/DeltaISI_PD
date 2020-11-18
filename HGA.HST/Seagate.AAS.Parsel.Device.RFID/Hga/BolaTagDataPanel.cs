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
    public partial class BolaTagDataPanel : UserControl
    {
        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------

        private bool _readOnly = true;

        // Constructors & Finalizers -------------------------------------------

        public BolaTagDataPanel()
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

        public void DisplayTagData(BolaTagData tag)
        {
            txtWorkorder.Text = tag.WorkOrder;
            txtVersion.Text = tag.WorkOrderVersion.ToString();
            txtLaststep.Text = tag.LastStep.ToString();
            lblChecksum.Text = string.Format("0x{0:X2}", tag.CheckSum);
            txtPartnumber.Text = tag.PartNumber;
            txtTrayID.Text = tag.TrayID;

            if (tag.Tab == 'D')
                cboTabupdown.SelectedIndex = 0;
            else if (tag.Tab == 'U')
                cboTabupdown.SelectedIndex = 1;
            else cboTabupdown.SelectedIndex = -1;

            cboVendorsite.SelectedIndex = -1;
            for (int i = 0; i < cboVendorsite.Items.Count; i++)
            {
                string vendor = cboVendorsite.Items[i].ToString();
                int sep = vendor.IndexOf(' ');
                string code = vendor.Substring(0, sep);
                if (code.Equals(tag.VendorCode.ToString()))
                {
                    cboVendorsite.SelectedIndex = i;
                    break;
                }
             }

            if (tag.TrayType < 0)
                cboTrayType.SelectedIndex = -1;
            else
            {
                if (tag.TrayType >= cboTrayType.Items.Count)
                {
                    int startAddNo = cboTrayType.Items.Count;
                    for (int i=startAddNo;i<=tag.TrayType;i++)
                        cboTrayType.Items.Add(string.Format("{0}:", i));
                }
                cboTrayType.SelectedIndex = tag.TrayType;
            }

            txtHeadsn1.Text = tag[0].HgaSN;
            txtHeadsn2.Text = tag[1].HgaSN;
            txtHeadsn3.Text = tag[2].HgaSN;
            txtHeadsn4.Text = tag[3].HgaSN;
            txtHeadsn5.Text = tag[4].HgaSN;
            txtHeadsn6.Text = tag[5].HgaSN;
            txtHeadsn7.Text = tag[6].HgaSN;
            txtHeadsn8.Text = tag[7].HgaSN;
            txtHeadsn9.Text = tag[8].HgaSN;
            txtHeadsn10.Text = tag[9].HgaSN;
            txtHeadsn11.Text = tag[10].HgaSN;
            txtHeadsn12.Text = tag[11].HgaSN;
            txtHeadsn13.Text = tag[12].HgaSN;
            txtHeadsn14.Text = tag[13].HgaSN;
            txtHeadsn15.Text = tag[14].HgaSN;
            txtHeadsn16.Text = tag[15].HgaSN;
            txtHeadsn17.Text = tag[16].HgaSN;
            txtHeadsn18.Text = tag[17].HgaSN;
            txtHeadsn19.Text = tag[18].HgaSN;
            txtHeadsn20.Text = tag[19].HgaSN;

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
            txtHeadstatus11.Text = tag[10].Status.ToString();
            txtHeadstatus12.Text = tag[11].Status.ToString();
            txtHeadstatus13.Text = tag[12].Status.ToString();
            txtHeadstatus14.Text = tag[13].Status.ToString();
            txtHeadstatus15.Text = tag[14].Status.ToString();
            txtHeadstatus16.Text = tag[15].Status.ToString();
            txtHeadstatus17.Text = tag[16].Status.ToString();
            txtHeadstatus18.Text = tag[17].Status.ToString();
            txtHeadstatus19.Text = tag[18].Status.ToString();
            txtHeadstatus20.Text = tag[19].Status.ToString();
        }

        public void UpdateTag(BolaTagData tag)
        {
            tag.WorkOrder = txtWorkorder.Text.Trim();
            tag.WorkOrderVersion = Convert.ToChar(txtVersion.Text.Trim());
            tag.LastStep = int.Parse(txtLaststep.Text.Trim());
            tag.PartNumber = txtPartnumber.Text.Trim();
            tag.TrayID = txtTrayID.Text;

            if (cboTabupdown.SelectedIndex >= 0)
                tag.Tab = cboTabupdown.Text[0];
            else tag.Tab = ' ';

            if (cboVendorsite.SelectedIndex >= 0)
            {
                int sep = cboVendorsite.Text.IndexOf('-');
                string code = cboVendorsite.Text.Substring(0, sep).Trim();
                tag.VendorCode = int.Parse(code);
            }
            else tag.VendorCode = 0;

            if (cboTrayType.SelectedIndex >= 0)
            {
                int sep = cboTrayType.Text.IndexOf(':');
                string typeNo = cboTrayType.Text.Substring(0, sep).Trim();
                tag.TrayType = int.Parse(typeNo);
            }
            else tag.TrayType = 0;

            //serial numbers
            tag[0].HgaSN = txtHeadsn1.Text;
            tag[1].HgaSN = txtHeadsn2.Text;
            tag[2].HgaSN = txtHeadsn3.Text;
            tag[3].HgaSN = txtHeadsn4.Text;
            tag[4].HgaSN = txtHeadsn5.Text;
            tag[5].HgaSN = txtHeadsn6.Text;
            tag[6].HgaSN = txtHeadsn7.Text;
            tag[7].HgaSN = txtHeadsn8.Text;
            tag[8].HgaSN = txtHeadsn9.Text;
            tag[9].HgaSN = txtHeadsn10.Text;
            tag[10].HgaSN = txtHeadsn11.Text;
            tag[11].HgaSN = txtHeadsn12.Text;
            tag[12].HgaSN = txtHeadsn13.Text;
            tag[13].HgaSN = txtHeadsn14.Text;
            tag[14].HgaSN = txtHeadsn15.Text;
            tag[15].HgaSN = txtHeadsn16.Text;
            tag[16].HgaSN = txtHeadsn17.Text;
            tag[17].HgaSN = txtHeadsn18.Text;
            tag[18].HgaSN = txtHeadsn19.Text;
            tag[19].HgaSN = txtHeadsn20.Text;

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
            tag[10].Status = txtHeadstatus11.Text == "" ? ' ' : txtHeadstatus11.Text[0];
            tag[11].Status = txtHeadstatus12.Text == "" ? ' ' : txtHeadstatus12.Text[0];
            tag[12].Status = txtHeadstatus13.Text == "" ? ' ' : txtHeadstatus13.Text[0];
            tag[13].Status = txtHeadstatus14.Text == "" ? ' ' : txtHeadstatus14.Text[0];
            tag[14].Status = txtHeadstatus15.Text == "" ? ' ' : txtHeadstatus15.Text[0];
            tag[15].Status = txtHeadstatus16.Text == "" ? ' ' : txtHeadstatus16.Text[0];
            tag[16].Status = txtHeadstatus17.Text == "" ? ' ' : txtHeadstatus17.Text[0];
            tag[17].Status = txtHeadstatus18.Text == "" ? ' ' : txtHeadstatus18.Text[0];
            tag[18].Status = txtHeadstatus19.Text == "" ? ' ' : txtHeadstatus19.Text[0];
            tag[19].Status = txtHeadstatus20.Text == "" ? ' ' : txtHeadstatus20.Text[0];
        }

        /// <summary>
        /// Clear tag data and update UI panel
        /// </summary>
        /// <param name="tag"></param>
        public void ClearTag(BolaTagData tag)
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
            txtPartnumber.ReadOnly = _readOnly;
            cboTabupdown.Enabled = !_readOnly;
            cboVendorsite.Enabled = !_readOnly;
            cboTrayType.Enabled = !_readOnly;

            txtTrayID.ReadOnly = _readOnly;

            txtHeadsn1.ReadOnly = _readOnly;
            txtHeadsn2.ReadOnly = _readOnly;
            txtHeadsn3.ReadOnly = _readOnly;
            txtHeadsn4.ReadOnly = _readOnly;
            txtHeadsn5.ReadOnly = _readOnly;
            txtHeadsn6.ReadOnly = _readOnly;
            txtHeadsn7.ReadOnly = _readOnly;
            txtHeadsn8.ReadOnly = _readOnly;
            txtHeadsn9.ReadOnly = _readOnly;
            txtHeadsn10.ReadOnly = _readOnly;
            txtHeadsn11.ReadOnly = _readOnly;
            txtHeadsn12.ReadOnly = _readOnly;
            txtHeadsn13.ReadOnly = _readOnly;
            txtHeadsn14.ReadOnly = _readOnly;
            txtHeadsn15.ReadOnly = _readOnly;
            txtHeadsn16.ReadOnly = _readOnly;
            txtHeadsn17.ReadOnly = _readOnly;
            txtHeadsn18.ReadOnly = _readOnly;
            txtHeadsn19.ReadOnly = _readOnly;
            txtHeadsn20.ReadOnly = _readOnly;

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
            txtHeadstatus11.ReadOnly = _readOnly;
            txtHeadstatus12.ReadOnly = _readOnly;
            txtHeadstatus13.ReadOnly = _readOnly;
            txtHeadstatus14.ReadOnly = _readOnly;
            txtHeadstatus15.ReadOnly = _readOnly;
            txtHeadstatus16.ReadOnly = _readOnly;
            txtHeadstatus17.ReadOnly = _readOnly;
            txtHeadstatus18.ReadOnly = _readOnly;
            txtHeadstatus19.ReadOnly = _readOnly;
            txtHeadstatus20.ReadOnly = _readOnly;
        }
    }
}
