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
//  [9/16/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace Seagate.AAS.Parsel.Services
{

	/// <summary>
	/// Summary description for FormErrorList.
	/// </summary>
	public class FormErrorList : System.Windows.Forms.Form
	{
		protected System.Windows.Forms.ListView MessageList;
        protected System.Windows.Forms.ColumnHeader columnHeader1;
        protected System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Timer timer1;
        protected System.Windows.Forms.TextBox textBox1;
        protected System.Windows.Forms.Button btnRight;
        protected System.Windows.Forms.Button btnMiddle;
        protected System.Windows.Forms.Button btnLeft;
        protected System.Windows.Forms.Button MinimizeBtn;
		private System.ComponentModel.IContainer components;

		public FormErrorList()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

//			// make the main bitmap transparent
//			Color transparentColor = ((Bitmap)LocUpperLeftBtn.Image).GetPixel(1,1);
//			((Bitmap)LocUpperLeftBtn.Image).MakeTransparent(transparentColor);
//			((Bitmap)LocUpperRightBtn.Image).MakeTransparent(transparentColor);
//			((Bitmap)LocLowerLeftBtn.Image).MakeTransparent(transparentColor);
//			((Bitmap)LocLowerRightBtn.Image).MakeTransparent(transparentColor);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.MessageList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnRight = new System.Windows.Forms.Button();
            this.btnMiddle = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.MinimizeBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(0, 154);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(464, 171);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "textBox1";
            // 
            // MessageList
            // 
            this.MessageList.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.MessageList.AllowColumnReorder = true;
            this.MessageList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MessageList.AutoArrange = false;
            this.MessageList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.MessageList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.MessageList.FullRowSelect = true;
            this.MessageList.GridLines = true;
            this.MessageList.HideSelection = false;
            this.MessageList.LabelWrap = false;
            this.MessageList.Location = new System.Drawing.Point(0, 0);
            this.MessageList.Name = "MessageList";
            this.MessageList.OwnerDraw = true;
            this.MessageList.Size = new System.Drawing.Size(464, 148);
            this.MessageList.TabIndex = 0;
            this.MessageList.UseCompatibleStateImageBehavior = false;
            this.MessageList.View = System.Windows.Forms.View.Details;
            this.MessageList.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.MessageList_DrawColumnHeader);
            this.MessageList.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.MessageList_DrawItem);
            this.MessageList.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.MessageList_DrawSubItem);
            this.MessageList.ItemActivate += new System.EventHandler(this.MessageList_ItemActivate);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Source";
            this.columnHeader1.Width = 140;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Error Message";
            this.columnHeader2.Width = 350;
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnRight
            // 
            this.btnRight.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnRight.Location = new System.Drawing.Point(355, 345);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(97, 45);
            this.btnRight.TabIndex = 11;
            this.btnRight.Text = "Right Button";
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnMiddle
            // 
            this.btnMiddle.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnMiddle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnMiddle.Location = new System.Drawing.Point(188, 345);
            this.btnMiddle.Name = "btnMiddle";
            this.btnMiddle.Size = new System.Drawing.Size(97, 45);
            this.btnMiddle.TabIndex = 10;
            this.btnMiddle.Text = "Middle Button";
            this.btnMiddle.Click += new System.EventHandler(this.btnMiddle_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnLeft.Location = new System.Drawing.Point(21, 345);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(97, 45);
            this.btnLeft.TabIndex = 9;
            this.btnLeft.Text = "Left Button";
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // MinimizeBtn
            // 
            this.MinimizeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MinimizeBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimizeBtn.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.MinimizeBtn.Location = new System.Drawing.Point(469, 8);
            this.MinimizeBtn.Name = "MinimizeBtn";
            this.MinimizeBtn.Size = new System.Drawing.Size(40, 40);
            this.MinimizeBtn.TabIndex = 16;
            this.MinimizeBtn.Text = "Min";
            this.MinimizeBtn.Click += new System.EventHandler(this.MinimizeBtn_Click);
            // 
            // FormErrorList
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(512, 402);
            this.ControlBox = false;
            this.Controls.Add(this.MinimizeBtn);
            this.Controls.Add(this.btnRight);
            this.Controls.Add(this.btnMiddle);
            this.Controls.Add(this.btnLeft);
            this.Controls.Add(this.MessageList);
            this.Controls.Add(this.textBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(528, 440);
            this.MinimumSize = new System.Drawing.Size(408, 112);
            this.Name = "FormErrorList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pending Errors";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public virtual void AddMessage(IErrorMessage message)
		{
		}

		protected IErrorMessage	currentMessage;
        private bool currentErrorIsFirstError = false;

		public void DisplayMessage(int n)
		{
			lock(this)
			{
				int errorCount = ServiceManager.ErrorHandler.ErrorList.Count;
				Debug.Assert(n >= 0 && n < Services.ServiceManager.ErrorHandler.ErrorList.Count,"Bad ErrorList index!");
				
				currentMessage = Services.ServiceManager.ErrorHandler.ErrorList[n] as IErrorMessage;

				if(currentMessage != null)
				{
                    //textBox1.Text = (currentMessage.FullDescription != null) ?
                    //    currentMessage.FullDescription : currentMessage.Text;

                    // let \n be the new line character instead of \r\n
                    string message = (currentMessage.FullDescription != null) ?
                        currentMessage.FullDescription : currentMessage.Text;
                    textBox1.Lines = message.Split('\n');

                    currentErrorIsFirstError = (n == 0) ? true : false;
                        
                    ShowButtons();

					SendMessage("Errors Pending");
					this.Show();
				}
			}
		}

		protected virtual void ShowButtons()
		{
			if(minimized)
				return;

            if (!currentErrorIsFirstError)
            {
                btnLeft.Hide();
                btnMiddle.Hide();
                btnRight.Hide();
            }
            else if (currentMessage != null)
            {
                // left button text
                if (currentMessage.BtnList.Left == ErrorButton.NoButton)
                    btnLeft.Hide();
                else
                {
                    btnLeft.Text = currentMessage.BtnList.Left.ToString();
                    btnLeft.Show();
                }
                // middle button text
                if (currentMessage.BtnList.Middle == ErrorButton.NoButton)
                    btnMiddle.Hide();
                else
                {
                    btnMiddle.Text = currentMessage.BtnList.Middle.ToString();
                    btnMiddle.Show();
                }
                // right button text
                if (currentMessage.BtnList.Right == ErrorButton.NoButton)
                    btnRight.Hide();
                else
                {
                    btnRight.Text = currentMessage.BtnList.Right.ToString();
                    btnRight.Show();
                }
            }
		}

		protected virtual void UpdateMessageList()
		{
			// display summary of each error in the listview
			MessageList.Items.Clear();
			foreach(IErrorMessage m in Services.ServiceManager.ErrorHandler.ErrorList)
			{
				m.ItemIndex = MessageList.Items.Add(m.Source).Index;	// add item and record its index
				int n = MessageList.Items.Count - 1;
				MessageList.Items[n].SubItems.Add(m.Text);
			}
			if(MessageList.Items.Count > 0)
			{
//				MessageList.Columns[0].Width = -1;		// size to longest string
//				MessageList.Columns[1].Width = -2;		// autosize
                MessageList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                MessageList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
			}
		}

//		public void RemoveMessage(IErrorMessage message)
//		{
//			lock(this)
//			{
//				UpdateMessageList();
//			}
//	
//			if(Services.ServiceManager.ErrorHandler.ErrorList.Count > 0)
//				DisplayMessage(0);
//			else
//			{
//				timer1.Enabled = false;
//				this.Hide();
//				SendMessage("No Errors Pending");
//				// update system state here ???
//				//Machine.Instance.GetWorkCell("");
//			}
//		}

		public void EnableTimer()
		{
			if(this.InvokeRequired)
				this.Invoke(new MethodInvoker(EnableTimer));
			else
				timer1.Enabled = true;
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			// check for change in list of registered messages
			if(ServiceManager.ErrorHandler.UpdateRequired)
			{
				UpdateMessageList();
				ServiceManager.ErrorHandler.UpdateRequired = false;
                if (Services.ServiceManager.ErrorHandler.ErrorList.Count > 0)
                {
                    MessageList.Items[0].Selected = true;
                    DisplayMessage(0);
                }
                else
                {
                    timer1.Enabled = false;
                    this.Hide();

                    if (minimized)						// set to normal window state
                        MinimizeBtn_Click(sender, e);

                    SendMessage("No Errors Pending");
                    // update system state here ???
                    //Machine.Instance.GetWorkCell("");
                }
			}
			if(currentMessage != null)
			{
//				TimeSpan t = DateTime.Now - currentMessage.TimeIn;
//				string s = "Error pending " + string.Format("{0}:{1}",t.Minutes,t.Seconds.ToString("d2"));
//				statusBar.Panels[0].Text = s;

				// look to see if the currentMessage.CustomUI wasn't added to the controls
				// because 
//				if(UserPanel.Controls.Count == 0)
//				{
//					DisplayMessage(0);
//				}
			}
		}

		private void MessageList_ItemActivate(object sender, System.EventArgs e)
		{
			if(!minimized)
				DisplayMessage(MessageList.SelectedIndices[0]);
		}

        // When using owner drawing to customise the appearance of the listview control, this DrawItem event handler method
        // is needed to allow the hightlight of the selected item in the listview to remain clearly visible after lost focus.(Instead of
        // the dim grey colour on Windows 7).
        private void MessageList_DrawItem(object sender,
            DrawListViewItemEventArgs e)
        {
            ListView listView = (ListView)sender;

            // Check if e.Item is selected and the ListView has a focus.
            if (!listView.Focused && e.Item.Selected)
            {
                Rectangle rowBounds = e.Bounds;
                int leftMargin = e.Item.GetBounds(ItemBoundsPortion.Label).Left;
                Rectangle bounds = new Rectangle(leftMargin, rowBounds.Top, rowBounds.Width - leftMargin, rowBounds.Height);
                e.Graphics.FillRectangle(SystemBrushes.Highlight, bounds);
            }
            else
                e.DrawDefault = true;
        }

        // When using owner drawing to customise the appearance of the listview control, this DrawColumnHeader event handler method
        // is needed to show the Column header in the listview. Otherwise the Column header will appear as blank.
        private void MessageList_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        // When using owner drawing to customise the appearance of the listview control, this DrawSubItem event handler method
        // is needed to allow the text of the hightlighted(selected) item in the listview to remain visible after lost focus.
        // Otherwise the text of the hightlighted(selected) item in the listview will disappear after lost focus.
        private void MessageList_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            const int TEXT_OFFSET = 1;

            ListView listView = (ListView)sender;

            // Check if e.Item is selected and the ListView has a focus.
            if (!listView.Focused && e.Item.Selected)
            {
                Rectangle rowBounds = e.SubItem.Bounds;
                Rectangle labelBounds = e.Item.GetBounds(ItemBoundsPortion.Label);
                int leftMargin = labelBounds.Left - TEXT_OFFSET;
                Rectangle bounds = new Rectangle(rowBounds.Left + leftMargin, rowBounds.Top, e.ColumnIndex == 0 ? labelBounds.Width : (rowBounds.Width - leftMargin - TEXT_OFFSET), rowBounds.Height);
                TextFormatFlags align;

                switch (listView.Columns[e.ColumnIndex].TextAlign)
                {
                    case HorizontalAlignment.Right:
                        align = TextFormatFlags.Right;
                        break;
                    case HorizontalAlignment.Center:
                        align = TextFormatFlags.HorizontalCenter;
                        break;
                    default:
                        align = TextFormatFlags.Left;
                        break;
                }

                TextRenderer.DrawText(e.Graphics, e.SubItem.Text, listView.Font, bounds, SystemColors.HighlightText,
                    align | TextFormatFlags.SingleLine | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);
            }
            else
                e.DrawDefault = true;
        }

		internal void SendMessage(string text)
		{
			MessageChannel messageChannel = ServiceManager.Messaging.GetMessageChannel("ErrorHandlerServiceMessaging");
			if (messageChannel != null)
				messageChannel.SendMessage(null, new Seagate.AAS.Parsel.Services.Message(text));
		}

		private void ShowCustomErrorPanel()
		{
			// This method should not be called, but is a place holder for what was 
			// previously done to display error panels 

			// Embed Error Panel in UI form
//			textBox1.Hide();
//			UserPanel.Controls.Clear();
//			currentMessage.CreateErrorPanel();
//			currentMessage.CustomUI.Size = UserPanel.Size;
//			try
//			{
//				UserPanel.Controls.Add(currentMessage.CustomUI);
//			}
//			catch(Exception ex)
//			{
//				MessageBox.Show("Error Handler Service problem: " + ex.Message);
//			}
		}

		private void btnLeft_Click(object sender, System.EventArgs e)
		{
            if ((currentMessage.BtnList.Left == ErrorButton.Stop) &&
                ((MessageBox.Show("Do you really want to STOP the RUN Process immediately?", "Stop Run Process Immediately", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)))
            {
                return;
            }
            Services.ServiceManager.ErrorHandler.LastHandledMessage = currentMessage;
            currentMessage.ButtonSelected = currentMessage.BtnList.Left;
            currentMessage.OnInput();
		}

		private void btnMiddle_Click(object sender, System.EventArgs e)
		{
            if ((currentMessage.BtnList.Middle == ErrorButton.Stop) &&
                ((MessageBox.Show("Do you really want to STOP the RUN Process immediately?", "Stop Run Process Immediately", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)))
            {
                return;
            }
            Services.ServiceManager.ErrorHandler.LastHandledMessage = currentMessage;
            currentMessage.ButtonSelected = currentMessage.BtnList.Middle;
            currentMessage.OnInput();
		}

		private void btnRight_Click(object sender, System.EventArgs e)
		{
            if ((currentMessage.BtnList.Right == ErrorButton.Stop) &&
                ((MessageBox.Show("Do you really want to STOP the RUN Process immediately?", "Stop Run Process Immediately", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)))
            {
                return;
            }
            Services.ServiceManager.ErrorHandler.LastHandledMessage = currentMessage;
            currentMessage.ButtonSelected = currentMessage.BtnList.Right;
            currentMessage.OnInput();
		}

		private bool minimized = false;
		private void MinimizeBtn_Click(object sender, System.EventArgs e)
		{
			Rectangle r = Screen.PrimaryScreen.WorkingArea;				// get screen area
			this.Left = r.Width / 2 - this.Width / 2;					// center left to right
			minimized = !minimized;		
			if(minimized)
			{
				this.Top = 0;											// move to top
				this.Height = this.MinimumSize.Height;					// squeeze height
				MessageList.Height = 80;
				MinimizeBtn.Text = "Max";
				btnLeft.Hide();
				btnMiddle.Hide();
				btnRight.Hide();
			}
			else
			{
				this.Height = this.MaximumSize.Height;					// expand height;
				this.Top = r.Height/2 - this.Height/2;					// center top to bottom
				MessageList.Height = 120;
				MinimizeBtn.Text = "Min";
				ShowButtons();
			}
		}
	}
}
