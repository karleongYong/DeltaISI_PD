using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Seagate.AAS.HGA.HST.UI.Utils
{
    public partial class PartsCheckDisplay : UserControl
    {
        private int lblNum;
        private int lblNumClicked = 99;
        private bool allowUserToggle = false;
        private System.Windows.Forms.Label[] lblArray; // Declaring array of Label
        

        public PartsCheckDisplay()
        {
            InitializeComponent();
        }

        public int LblNumClicked { get { return lblNumClicked; } set { lblNumClicked = value; } }
        public bool AllowUserToggle { get { return allowUserToggle; } set { allowUserToggle = value; } }
        public void ShowLabel(int _controlCreated, bool _colorCode)
        {
            int xPos = 0;
            int yPos = 6;
            lblNum = _controlCreated;
            AddControls(lblNum); // Create six Labels
            int n = 0;
            while (n < lblNum)
            {
                lblArray[n].Tag = n;
                lblArray[n].Width = 18;
                lblArray[n].Height = 18;
                //lblArray[n].BackColor=System.Drawing.Color.White;
                if (_colorCode)
                    lblArray[n].BackColor = System.Drawing.Color.Green;
                else
                    lblArray[n].BackColor = System.Drawing.Color.White;

                lblArray[n].Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                lblArray[n].BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                lblArray[n].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                //lblArray[n].BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                //if (n > 9)
                //{
                    // lblArray[n].Text = lblArray[n - 10].Tag.ToString();
                //    if (n == 19)
                //        lblArray[n].Font = new System.Drawing.Font("Microsoft Sans Serif", 6.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                //    lblArray[n].Text = Convert.ToString(n + 1 - 10);
                //}
                //else
                //{
                    //lblArray[n].Text = lblArray[n].Tag.ToString() ;
                    if (n >= 9)
                        lblArray[n].Font = new System.Drawing.Font("Microsoft Sans Serif", 6.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    lblArray[n].Text = Convert.ToString(n + 1);
                //}
                //if(yPos > 80) // Three Labels in one column
                //{
                yPos = 6;
                xPos = xPos + lblArray[n].Width + 1;
                //}
                lblArray[n].Left = xPos - 18;
                lblArray[n].Top = yPos - 6;
                yPos = yPos + lblArray[n].Height + 2;
                pnlLabels.Controls.Add(lblArray[n]);
                // the Event of click Button
                lblArray[n].Click += new System.EventHandler(ClickLabel);
                n++;
            }
            // btnAddLabel.Enabled = false; // not need to this button now
            //btnRemoveLabel.Enabled = true; // we need to remove any
            // label2.Visible = true;
        }



        // Result of the event click Label
        public void ClickLabel(Object sender, System.EventArgs e)
        {
            if (allowUserToggle)
            {
                int n = (int)((System.Windows.Forms.Label)sender).Tag; // number of the Label
                lblNumClicked = n;
            }
            // number of TxtBox controls must be >= Label controls
            //if (n <= txtNum)
            //{
            //    // Copy text from the TextBox[n] to the Label[n]
            //    lblArray[n].Text = txtArray[n].Text;
            //    System.Windows.Forms.MessageBox.Show("Copy text from the TextBox #" +
            //        n.ToString() + " to the Label #" + n.ToString());
            //}
            //else
            //{
            //    // if any TextBox not found:
            //    System.Windows.Forms.MessageBox.Show("You have clicked Label " +
            //        n.ToString());
            //}
        }
        private void AddControls(int cNumber)
        {

            // assign number of controls 
            lblArray = new System.Windows.Forms.Label[cNumber + 1];
            for (int i = 0; i < cNumber + 1; i++)
            {
                // Initialize one variable
                lblArray[i] = new System.Windows.Forms.Label();
            }
        }
        public void SetLabel(int labelIndex, bool colorState, bool mustbeEmpty)
        {
            if (colorState)
            {
                if (mustbeEmpty)
                {
                    lblArray[labelIndex].BackColor = System.Drawing.Color.Red;
                    lblArray[labelIndex].ForeColor = System.Drawing.SystemColors.ButtonHighlight;
                }
                else
                {
                    lblArray[labelIndex].BackColor = System.Drawing.Color.Green;
                    lblArray[labelIndex].ForeColor = System.Drawing.SystemColors.ButtonHighlight;
                }
            }
            else
            {
                lblArray[labelIndex].BackColor = System.Drawing.SystemColors.Control;
                lblArray[labelIndex].ForeColor = System.Drawing.SystemColors.ControlText;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // ShowLabel();
        }



    }


}
