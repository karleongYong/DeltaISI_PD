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
//  [2007/05/08] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Equipment;
using XyratexOSC.UI;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.UI
{
    public partial class PanelNavigation : UserControl
    {
        // Member variables ----------------------------------------------------

        protected Color btnSelectedColor = SystemColors.ControlDark;
        private Point _popupPoint = new Point(0, -5);
        private RadioButton _activeButton = null;
        private bool _popupEnabled = true;
        private bool showPopupMenus = true;
        private UserAccessSettings _userAccess;
        public bool RunMode { get; set; }
        

        // Constructors & Finalizers -------------------------------------------

        public PanelNavigation()
        {
            InitializeComponent();
        }        
        
        // Properties ----------------------------------------------------------
        
        // Methods -------------------------------------------------------------

        public void AssignWorkcell(HSTWorkcell workcell)
        {
            _userAccess = workcell.HSTSettings.getUserAccessSettings();            
            _userAccess.UserChanged += new EventHandler(UserChanged);
        }
        
        // Event handlers ------------------------------------------------------

        private void PanelNavigation_Load(object sender, System.EventArgs e)
        {
            if (this.DesignMode)
                return;

            _activeButton = btnOperation;
            btnOperation.BackColor = btnSelectedColor;

            foreach (string workcellID in Machine.HSTMachine.Instance.WorkcellsMap.Keys)
            {
                MenuItem newMenuItem = new MenuItem(workcellID);
                int slot = ((IWorkcell)Machine.HSTMachine.Instance.WorkcellsMap[workcellID]).Slot;

                newMenuItem.RadioCheck = true;
                newMenuItem.OwnerDraw = true;
                newMenuItem.MeasureItem += new MeasureItemEventHandler(newMenuItem_MeasureItem);
                newMenuItem.DrawItem += new DrawItemEventHandler(newMenuItem_DrawItem);
                newMenuItem.Click += new EventHandler(newMenuItem_Click);
                this.subNavigationMenu.MenuItems.Add(GetMenuPosition(slot), newMenuItem);
                _popupPoint.Y -= 50;
            }
            if (this.subNavigationMenu.MenuItems.Count > 0)
            {
                //this.subNavigationMenu.MenuItems[0].Checked = true;
                this._activeButton = this.btnOperation;
                this.newMenuItem_Click(this.subNavigationMenu.MenuItems[0], new EventArgs());
            }
            if (this.subNavigationMenu.MenuItems.Count <= 1)
                _popupEnabled = false;            
            
            EnableDisablePanel();            
        }

        private void PanelNavigation_Resize(object sender, System.EventArgs e)
        {
            // size and distribute buttons
            Size btnSize = this.Size;
            btnSize.Width   = this.Size.Width/this.Controls.Count - 16;;  // 8 unit margin
            btnSize.Height -= 16; 
            int i=0;
			foreach (RadioButton rButton in this.Controls) 
			{
				rButton.Size = btnSize;
				rButton.Location = new Point(8 + i*(this.Size.Width/this.Controls.Count), 8);
				i++;
			}
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

//             Graphics g = e.Graphics;
//             Pen myPen = new Pen(SystemColors.ControlDarkDark);
//             g.DrawLine(myPen, new Point(0, 0), new Point(this.Width - 1, 0));
        }

        private int GetMenuPosition(int slot)
        {
            int position = 0;
            for (int i = 0; i < this.subNavigationMenu.MenuItems.Count; i++)
            {
                if (slot > ((IWorkcell)Machine.HSTMachine.Instance.WorkcellsMap[this.subNavigationMenu.MenuItems[i].Text]).Slot)
                    position++;
            }
            return position;
        }

        private void newMenuItem_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemWidth = this.Size.Width / this.Controls.Count - 16 * 2 - 2;
            e.ItemHeight = 50;
        }

        private void newMenuItem_DrawItem(object sender, DrawItemEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;

            Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 2);

            SizeF strSize = e.Graphics.MeasureString(mi.Text, this.Font);
            int strX = e.Bounds.X + ((e.Bounds.Width) / 2) - (int)(strSize.Width / 2);
            int strY = e.Bounds.Y + ((e.Bounds.Height) / 2) - (int)(strSize.Height / 2);
            Font myFont = this.Font;

            if (mi.Checked)
            {
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Control), rect);
                int dotSize = 8;
                e.Graphics.FillEllipse(Brushes.Black, dotSize, (e.Bounds.Y + e.Bounds.Height / 2 - dotSize / 2), dotSize, dotSize);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(this.BackColor), rect);
            }
            e.Graphics.DrawRectangle(Pens.Black, rect);
            e.Graphics.DrawString(mi.Text, myFont, Brushes.Black, strX, strY);
        }

        public void SetPanel(string panel)
        {
            object button = null;
            switch (panel)
            {
                case "Operation":   button = (object)btnOperation;   break;
                case "Recipe": button = (object)btnRecipe; break;
                case "Setup": button = (object)btnSetup; break;
                case "Diagnostic":   button = (object)btnDiagnostic;   break;
                case "Data":   button = (object)btnData;   break;                
                case "Help":   button = (object)btnHelp;   break;
                default:   return;
            }
            ((RadioButton)button).Checked = true;
            radiobtn_MouseDown(button, null);
            radiobtn_MouseUp(button, null);
        }

        private void newMenuItem_Click(object sender, EventArgs e)
        {
            MenuItem clickedMenuItem = (MenuItem)sender;
            foreach (MenuItem mi in this.subNavigationMenu.MenuItems)
            {
                if (mi.Text == clickedMenuItem.Text)
                {
                    clickedMenuItem.Checked = true;
                    this._activeButton.Checked = true;
                    Seagate.AAS.Parsel.Services.ServiceManager.MenuNavigator.OnNavMenu(this._activeButton.Text, clickedMenuItem.Text);
                }
                else
                {
                    mi.Checked = false;
                }
            }
        }

        private void tmrPopUp_Tick(object sender, System.EventArgs e)
        {
            if (this.DesignMode) return;

            tmrPopUp.Enabled = false;
            this.subNavigationMenu.Show(this._activeButton, _popupPoint);
        }

        protected virtual void radiobtn_CheckedChanged(object sender, System.EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Checked)
            {
                rb.BackColor = btnSelectedColor;
            }
            else
            {
                rb.BackColor = SystemColors.ControlLightLight;
            }
        }

        protected virtual void radiobtn_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.DesignMode) return;

            this._activeButton = (RadioButton)sender;

            if (_popupEnabled)
                tmrPopUp.Enabled = true;
        }

        protected virtual void radiobtn_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.DesignMode) return;

            RadioButton rb = (RadioButton)sender;

            if (tmrPopUp.Enabled)  //did not timeout, switch functional area only, do not show workcell navigation
            {
                tmrPopUp.Enabled = false;
            }
            Seagate.AAS.Parsel.Services.ServiceManager.MenuNavigator.OnMainMenu(rb.Text);
        }

        public Color ButtonSelectedColor
        {
            set { btnSelectedColor = value; }
        }

        public bool ShowPopupMenus
        {
            set { showPopupMenus = value; }
        }

        private void UserChanged(object sender, EventArgs e)
        {
            try
            {
                UIUtility.Invoke(this, () =>
                {
                    EnableDisablePanel();
                });
            }
            catch { }
        }

        private void EnableDisablePanel()
        {
            if (!HSTMachine.Workcell.CCCMachineTriggeringActivated && !HSTMachine.Workcell.Process.IsRunState)
            if (_userAccess != null)
            {
                switch (_userAccess.getCurrentUser().Level)
                {
                    case UserLevel.Monitor:
                        DisableButtons(btnSetup, btnDiagnostic, btnData, btnRecipe);                        
                        break;
                    case UserLevel.Operator:
                        DisableButtons(btnSetup, btnDiagnostic, btnRecipe);
                        EnableButtons(btnData);
                        break;
                    case UserLevel.Engineer:
                        EnableButtons(btnSetup, btnDiagnostic, btnData, btnRecipe);
                        break;
                    case UserLevel.Technician:
                        DisableButtons(btnSetup, btnRecipe);
                        EnableButtons(btnDiagnostic, btnData);
                        break;
                    case UserLevel.Administrator:                    
                        EnableButtons(btnSetup, btnDiagnostic, btnData, btnRecipe);                                               
                        break;
                }
            }
        }

        public void DisableButtons(params RadioButton[] btns)
        {
            foreach(RadioButton btn in btns)
            {
                btn.Enabled = false;
            }
        }

        public void EnableButtons(params RadioButton[] btns)
        {
            foreach (RadioButton btn in btns)
            {
                btn.Enabled = true;
            }
        }

    }
}
