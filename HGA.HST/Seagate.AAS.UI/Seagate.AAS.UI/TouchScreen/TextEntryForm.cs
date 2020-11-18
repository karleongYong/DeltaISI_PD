using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Seagate.AAS.UI
{
	/// <summary>
	/// Summary description for TextEntryForm.
	/// </summary>
	public class TextEntryForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.Button button8;
		private System.Windows.Forms.Button button9;
		private System.Windows.Forms.Button button10;
		private System.Windows.Forms.Button button13;
		private System.Windows.Forms.Button button14;
		private System.Windows.Forms.Button button15;
		private System.Windows.Forms.Button button16;
		private System.Windows.Forms.Button button17;
		private System.Windows.Forms.Button button18;
		private System.Windows.Forms.Button button19;
		private System.Windows.Forms.Button button20;
		private System.Windows.Forms.Button button21;
		private System.Windows.Forms.Button button22;
		private System.Windows.Forms.Button button25;
		private System.Windows.Forms.Button button26;
		private System.Windows.Forms.Button button27;
		private System.Windows.Forms.Button button28;
		private System.Windows.Forms.Button button29;
		private System.Windows.Forms.Button button30;
		private System.Windows.Forms.Button button31;
		private System.Windows.Forms.Button button32;
		private System.Windows.Forms.Button button33;
		private System.Windows.Forms.Button button34;
		private System.Windows.Forms.Button button39;
		private System.Windows.Forms.Button button40;
		private System.Windows.Forms.Button button41;
		private System.Windows.Forms.Button button42;
		private System.Windows.Forms.Button button43;
		private System.Windows.Forms.Button button44;
		private System.Windows.Forms.Button button45;
		private System.Windows.Forms.Button button46;
		private System.Windows.Forms.Button button47;
		private System.Windows.Forms.Button button48;
		private System.Windows.Forms.Button button24;
		private System.Windows.Forms.TextBox textBoxEntry;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnSpace;
		private System.Windows.Forms.Button btnBS;
		private System.Windows.Forms.Button btnEnter;
		private string stringEntered = "";
		private System.Windows.Forms.CheckBox chkCaps;
		private System.Windows.Forms.Button btnClear;

        private bool        _alphaNumOnly = false;
        private bool        _noWhiteSpace = false;
        private bool        _onlyCaps = false;

        private int         _minLength = 0;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TextEntryForm(string title, string defaultText)
		{
			InitializeComponent();

			this.Text = title;
			textBoxEntry.Text = defaultText;
		}

		public string StringEntered
		{
			get
			{
				return stringEntered;
			}
		}

        public char PasswordChar
        {
            get
            {
                return textBoxEntry.PasswordChar;
            }
            set
            {
                textBoxEntry.PasswordChar = value;
            }
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.button17 = new System.Windows.Forms.Button();
            this.button18 = new System.Windows.Forms.Button();
            this.button19 = new System.Windows.Forms.Button();
            this.button20 = new System.Windows.Forms.Button();
            this.button21 = new System.Windows.Forms.Button();
            this.button22 = new System.Windows.Forms.Button();
            this.button25 = new System.Windows.Forms.Button();
            this.button26 = new System.Windows.Forms.Button();
            this.button27 = new System.Windows.Forms.Button();
            this.button28 = new System.Windows.Forms.Button();
            this.button29 = new System.Windows.Forms.Button();
            this.button30 = new System.Windows.Forms.Button();
            this.button31 = new System.Windows.Forms.Button();
            this.button32 = new System.Windows.Forms.Button();
            this.button33 = new System.Windows.Forms.Button();
            this.button34 = new System.Windows.Forms.Button();
            this.button39 = new System.Windows.Forms.Button();
            this.button40 = new System.Windows.Forms.Button();
            this.button41 = new System.Windows.Forms.Button();
            this.button42 = new System.Windows.Forms.Button();
            this.button43 = new System.Windows.Forms.Button();
            this.button44 = new System.Windows.Forms.Button();
            this.button45 = new System.Windows.Forms.Button();
            this.button46 = new System.Windows.Forms.Button();
            this.button47 = new System.Windows.Forms.Button();
            this.button48 = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.textBoxEntry = new System.Windows.Forms.TextBox();
            this.btnSpace = new System.Windows.Forms.Button();
            this.btnBS = new System.Windows.Forms.Button();
            this.btnEnter = new System.Windows.Forms.Button();
            this.button24 = new System.Windows.Forms.Button();
            this.chkCaps = new System.Windows.Forms.CheckBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button1.Location = new System.Drawing.Point(8, 64);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 56);
            this.button1.TabIndex = 0;
            this.button1.TabStop = false;
            this.button1.Text = "1";
            this.button1.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button2.Location = new System.Drawing.Point(80, 64);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(64, 56);
            this.button2.TabIndex = 1;
            this.button2.TabStop = false;
            this.button2.Text = "2";
            this.button2.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button3.Location = new System.Drawing.Point(152, 64);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(64, 56);
            this.button3.TabIndex = 2;
            this.button3.TabStop = false;
            this.button3.Text = "3";
            this.button3.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button4.Location = new System.Drawing.Point(224, 64);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(64, 56);
            this.button4.TabIndex = 3;
            this.button4.TabStop = false;
            this.button4.Text = "4";
            this.button4.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button5.Location = new System.Drawing.Point(296, 64);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(64, 56);
            this.button5.TabIndex = 4;
            this.button5.TabStop = false;
            this.button5.Text = "5";
            this.button5.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button6.Location = new System.Drawing.Point(368, 64);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(64, 56);
            this.button6.TabIndex = 5;
            this.button6.TabStop = false;
            this.button6.Text = "6";
            this.button6.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button7.Location = new System.Drawing.Point(440, 64);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(64, 56);
            this.button7.TabIndex = 6;
            this.button7.TabStop = false;
            this.button7.Text = "7";
            this.button7.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button8
            // 
            this.button8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button8.Location = new System.Drawing.Point(512, 64);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(64, 56);
            this.button8.TabIndex = 7;
            this.button8.TabStop = false;
            this.button8.Text = "8";
            this.button8.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button9
            // 
            this.button9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button9.Location = new System.Drawing.Point(584, 64);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(64, 56);
            this.button9.TabIndex = 8;
            this.button9.TabStop = false;
            this.button9.Text = "9";
            this.button9.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button10
            // 
            this.button10.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button10.Location = new System.Drawing.Point(656, 64);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(64, 56);
            this.button10.TabIndex = 9;
            this.button10.TabStop = false;
            this.button10.Text = "0";
            this.button10.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button13
            // 
            this.button13.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button13.Location = new System.Drawing.Point(24, 128);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(64, 56);
            this.button13.TabIndex = 12;
            this.button13.TabStop = false;
            this.button13.Text = "Q";
            this.button13.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button14
            // 
            this.button14.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button14.Location = new System.Drawing.Point(96, 128);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(64, 56);
            this.button14.TabIndex = 13;
            this.button14.TabStop = false;
            this.button14.Text = "W";
            this.button14.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button15
            // 
            this.button15.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button15.Location = new System.Drawing.Point(168, 128);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(64, 56);
            this.button15.TabIndex = 14;
            this.button15.TabStop = false;
            this.button15.Text = "E";
            this.button15.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button16
            // 
            this.button16.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button16.Location = new System.Drawing.Point(240, 128);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(64, 56);
            this.button16.TabIndex = 15;
            this.button16.TabStop = false;
            this.button16.Text = "R";
            this.button16.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button17
            // 
            this.button17.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button17.Location = new System.Drawing.Point(312, 128);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(64, 56);
            this.button17.TabIndex = 16;
            this.button17.TabStop = false;
            this.button17.Text = "T";
            this.button17.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button18
            // 
            this.button18.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button18.Location = new System.Drawing.Point(384, 128);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(64, 56);
            this.button18.TabIndex = 17;
            this.button18.TabStop = false;
            this.button18.Text = "Y";
            this.button18.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button19
            // 
            this.button19.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button19.Location = new System.Drawing.Point(456, 128);
            this.button19.Name = "button19";
            this.button19.Size = new System.Drawing.Size(64, 56);
            this.button19.TabIndex = 18;
            this.button19.TabStop = false;
            this.button19.Text = "U";
            this.button19.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button20
            // 
            this.button20.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button20.Location = new System.Drawing.Point(528, 128);
            this.button20.Name = "button20";
            this.button20.Size = new System.Drawing.Size(64, 56);
            this.button20.TabIndex = 19;
            this.button20.TabStop = false;
            this.button20.Text = "I";
            this.button20.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button21
            // 
            this.button21.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button21.Location = new System.Drawing.Point(600, 128);
            this.button21.Name = "button21";
            this.button21.Size = new System.Drawing.Size(64, 56);
            this.button21.TabIndex = 20;
            this.button21.TabStop = false;
            this.button21.Text = "O";
            this.button21.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button22
            // 
            this.button22.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button22.Location = new System.Drawing.Point(672, 128);
            this.button22.Name = "button22";
            this.button22.Size = new System.Drawing.Size(64, 56);
            this.button22.TabIndex = 21;
            this.button22.TabStop = false;
            this.button22.Text = "P";
            this.button22.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button25
            // 
            this.button25.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button25.Location = new System.Drawing.Point(40, 192);
            this.button25.Name = "button25";
            this.button25.Size = new System.Drawing.Size(64, 56);
            this.button25.TabIndex = 24;
            this.button25.TabStop = false;
            this.button25.Text = "A";
            this.button25.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button26
            // 
            this.button26.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button26.Location = new System.Drawing.Point(112, 192);
            this.button26.Name = "button26";
            this.button26.Size = new System.Drawing.Size(64, 56);
            this.button26.TabIndex = 25;
            this.button26.TabStop = false;
            this.button26.Text = "S";
            this.button26.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button27
            // 
            this.button27.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button27.Location = new System.Drawing.Point(184, 192);
            this.button27.Name = "button27";
            this.button27.Size = new System.Drawing.Size(64, 56);
            this.button27.TabIndex = 26;
            this.button27.TabStop = false;
            this.button27.Text = "D";
            this.button27.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button28
            // 
            this.button28.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button28.Location = new System.Drawing.Point(256, 192);
            this.button28.Name = "button28";
            this.button28.Size = new System.Drawing.Size(64, 56);
            this.button28.TabIndex = 27;
            this.button28.TabStop = false;
            this.button28.Text = "F";
            this.button28.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button29
            // 
            this.button29.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button29.Location = new System.Drawing.Point(328, 192);
            this.button29.Name = "button29";
            this.button29.Size = new System.Drawing.Size(64, 56);
            this.button29.TabIndex = 28;
            this.button29.TabStop = false;
            this.button29.Text = "G";
            this.button29.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button30
            // 
            this.button30.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button30.Location = new System.Drawing.Point(400, 192);
            this.button30.Name = "button30";
            this.button30.Size = new System.Drawing.Size(64, 56);
            this.button30.TabIndex = 29;
            this.button30.TabStop = false;
            this.button30.Text = "H";
            this.button30.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button31
            // 
            this.button31.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button31.Location = new System.Drawing.Point(472, 192);
            this.button31.Name = "button31";
            this.button31.Size = new System.Drawing.Size(64, 56);
            this.button31.TabIndex = 30;
            this.button31.TabStop = false;
            this.button31.Text = "J";
            this.button31.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button32
            // 
            this.button32.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button32.Location = new System.Drawing.Point(544, 192);
            this.button32.Name = "button32";
            this.button32.Size = new System.Drawing.Size(64, 56);
            this.button32.TabIndex = 31;
            this.button32.TabStop = false;
            this.button32.Text = "K";
            this.button32.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button33
            // 
            this.button33.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button33.Location = new System.Drawing.Point(616, 192);
            this.button33.Name = "button33";
            this.button33.Size = new System.Drawing.Size(64, 56);
            this.button33.TabIndex = 32;
            this.button33.TabStop = false;
            this.button33.Text = "L";
            this.button33.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button34
            // 
            this.button34.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button34.Location = new System.Drawing.Point(688, 192);
            this.button34.Name = "button34";
            this.button34.Size = new System.Drawing.Size(64, 56);
            this.button34.TabIndex = 33;
            this.button34.TabStop = false;
            this.button34.Text = ";";
            this.button34.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button39
            // 
            this.button39.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button39.Location = new System.Drawing.Point(704, 256);
            this.button39.Name = "button39";
            this.button39.Size = new System.Drawing.Size(64, 56);
            this.button39.TabIndex = 45;
            this.button39.TabStop = false;
            this.button39.Text = "/";
            this.button39.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button40
            // 
            this.button40.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button40.Location = new System.Drawing.Point(632, 256);
            this.button40.Name = "button40";
            this.button40.Size = new System.Drawing.Size(64, 56);
            this.button40.TabIndex = 44;
            this.button40.TabStop = false;
            this.button40.Text = ".";
            this.button40.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button41
            // 
            this.button41.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button41.Location = new System.Drawing.Point(560, 256);
            this.button41.Name = "button41";
            this.button41.Size = new System.Drawing.Size(64, 56);
            this.button41.TabIndex = 43;
            this.button41.TabStop = false;
            this.button41.Text = ",";
            this.button41.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button42
            // 
            this.button42.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button42.Location = new System.Drawing.Point(488, 256);
            this.button42.Name = "button42";
            this.button42.Size = new System.Drawing.Size(64, 56);
            this.button42.TabIndex = 42;
            this.button42.TabStop = false;
            this.button42.Text = "M";
            this.button42.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button43
            // 
            this.button43.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button43.Location = new System.Drawing.Point(416, 256);
            this.button43.Name = "button43";
            this.button43.Size = new System.Drawing.Size(64, 56);
            this.button43.TabIndex = 41;
            this.button43.TabStop = false;
            this.button43.Text = "N";
            this.button43.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button44
            // 
            this.button44.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button44.Location = new System.Drawing.Point(344, 256);
            this.button44.Name = "button44";
            this.button44.Size = new System.Drawing.Size(64, 56);
            this.button44.TabIndex = 40;
            this.button44.TabStop = false;
            this.button44.Text = "B";
            this.button44.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button45
            // 
            this.button45.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button45.Location = new System.Drawing.Point(272, 256);
            this.button45.Name = "button45";
            this.button45.Size = new System.Drawing.Size(64, 56);
            this.button45.TabIndex = 39;
            this.button45.TabStop = false;
            this.button45.Text = "V";
            this.button45.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button46
            // 
            this.button46.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button46.Location = new System.Drawing.Point(200, 256);
            this.button46.Name = "button46";
            this.button46.Size = new System.Drawing.Size(64, 56);
            this.button46.TabIndex = 38;
            this.button46.TabStop = false;
            this.button46.Text = "C";
            this.button46.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button47
            // 
            this.button47.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button47.Location = new System.Drawing.Point(128, 256);
            this.button47.Name = "button47";
            this.button47.Size = new System.Drawing.Size(64, 56);
            this.button47.TabIndex = 37;
            this.button47.TabStop = false;
            this.button47.Text = "X";
            this.button47.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // button48
            // 
            this.button48.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button48.Location = new System.Drawing.Point(56, 256);
            this.button48.Name = "button48";
            this.button48.Size = new System.Drawing.Size(64, 56);
            this.button48.TabIndex = 36;
            this.button48.TabStop = false;
            this.button48.Text = "Z";
            this.button48.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(408, 384);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(216, 56);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // textBoxEntry
            // 
            this.textBoxEntry.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.textBoxEntry.Location = new System.Drawing.Point(8, 8);
            this.textBoxEntry.Name = "textBoxEntry";
            this.textBoxEntry.Size = new System.Drawing.Size(840, 44);
            this.textBoxEntry.TabIndex = 2;
            this.textBoxEntry.Text = "";
            // 
            // btnSpace
            // 
            this.btnSpace.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.btnSpace.Location = new System.Drawing.Point(200, 320);
            this.btnSpace.Name = "btnSpace";
            this.btnSpace.Size = new System.Drawing.Size(424, 56);
            this.btnSpace.TabIndex = 49;
            this.btnSpace.TabStop = false;
            this.btnSpace.Text = "SPACE";
            this.btnSpace.Click += new System.EventHandler(this.btnSpace_Click);
            // 
            // btnBS
            // 
            this.btnBS.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.btnBS.Location = new System.Drawing.Point(728, 64);
            this.btnBS.Name = "btnBS";
            this.btnBS.Size = new System.Drawing.Size(120, 56);
            this.btnBS.TabIndex = 50;
            this.btnBS.TabStop = false;
            this.btnBS.Text = "Backspace";
            this.btnBS.Click += new System.EventHandler(this.btnBS_Click);
            // 
            // btnEnter
            // 
            this.btnEnter.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.btnEnter.Location = new System.Drawing.Point(632, 384);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(216, 56);
            this.btnEnter.TabIndex = 0;
            this.btnEnter.Text = "ENTER";
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // button24
            // 
            this.button24.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.button24.Location = new System.Drawing.Point(744, 128);
            this.button24.Name = "button24";
            this.button24.Size = new System.Drawing.Size(64, 56);
            this.button24.TabIndex = 52;
            this.button24.TabStop = false;
            this.button24.Text = "\\";
            this.button24.Click += new System.EventHandler(this.charBtn_Click);
            // 
            // chkCaps
            // 
            this.chkCaps.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkCaps.Checked = true;
            this.chkCaps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCaps.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.chkCaps.Location = new System.Drawing.Point(8, 320);
            this.chkCaps.Name = "chkCaps";
            this.chkCaps.Size = new System.Drawing.Size(184, 56);
            this.chkCaps.TabIndex = 53;
            this.chkCaps.Text = "CAPS LOCK";
            this.chkCaps.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(8, 384);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(216, 56);
            this.btnClear.TabIndex = 54;
            this.btnClear.Text = "CLEAR";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // TextEntryForm
            // 
            this.AcceptButton = this.btnEnter;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(858, 450);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.chkCaps);
            this.Controls.Add(this.textBoxEntry);
            this.Controls.Add(this.button24);
            this.Controls.Add(this.btnEnter);
            this.Controls.Add(this.btnBS);
            this.Controls.Add(this.btnSpace);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.button39);
            this.Controls.Add(this.button40);
            this.Controls.Add(this.button41);
            this.Controls.Add(this.button42);
            this.Controls.Add(this.button43);
            this.Controls.Add(this.button44);
            this.Controls.Add(this.button45);
            this.Controls.Add(this.button46);
            this.Controls.Add(this.button47);
            this.Controls.Add(this.button48);
            this.Controls.Add(this.button34);
            this.Controls.Add(this.button33);
            this.Controls.Add(this.button32);
            this.Controls.Add(this.button31);
            this.Controls.Add(this.button30);
            this.Controls.Add(this.button29);
            this.Controls.Add(this.button28);
            this.Controls.Add(this.button27);
            this.Controls.Add(this.button26);
            this.Controls.Add(this.button25);
            this.Controls.Add(this.button22);
            this.Controls.Add(this.button21);
            this.Controls.Add(this.button20);
            this.Controls.Add(this.button19);
            this.Controls.Add(this.button18);
            this.Controls.Add(this.button17);
            this.Controls.Add(this.button16);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TextEntryForm";
            this.ShowInTaskbar = false;
            this.Text = "TextEntryForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.TextEntryForm_Load);
            this.ResumeLayout(false);

        }
		#endregion

		private void btnBS_Click(object sender, System.EventArgs e)
		{
			if(textBoxEntry.Text.Length > 0)
				textBoxEntry.Text = textBoxEntry.Text.Substring(0,textBoxEntry.Text.Length-1);

            CheckLength();
		}

		private void btnSpace_Click(object sender, System.EventArgs e)
		{
			textBoxEntry.Text += " ";
            CheckLength();
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		private void btnEnter_Click(object sender, System.EventArgs e)
		{
			stringEntered = textBoxEntry.Text;
			this.DialogResult = DialogResult.OK;
		}

		private void charBtn_Click(object sender, System.EventArgs e)
		{
			if(chkCaps.Checked)
				textBoxEntry.Text += ((Button)sender).Text;
			else
				textBoxEntry.Text += ((Button)sender).Text.ToLower();

            CheckLength();
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			this.textBoxEntry.Text = "";
            CheckLength();
		}

        private void CheckLength()
        {
            if((textBoxEntry.Text.Length > textBoxEntry.MaxLength) ||
                textBoxEntry.Text.Length < _minLength)
            {
                btnEnter.Enabled = false;
                textBoxEntry.BackColor = System.Drawing.Color.Pink;             
            }
            else
            {
                btnEnter.Enabled = true;
                textBoxEntry.BackColor = System.Drawing.Color.White;
            }
        }

        private void TextEntryForm_Load(object sender, System.EventArgs e)
        {
            if(_alphaNumOnly)
            {
                button24.Enabled = false;
                button34.Enabled = false;
                button39.Enabled = false;
                button40.Enabled = false;
                button41.Enabled = false;
            }

            if(_noWhiteSpace)
            {
                btnSpace.Enabled = false;
            }

            if(_onlyCaps)
            {
                chkCaps.Enabled = false;
            }

            CheckLength();
        }

        public bool AlphaNumOnly
        {
            get { return _alphaNumOnly; }
            set { _alphaNumOnly = value; }
        }

        public bool NoWhiteSpace
        {
            get { return _noWhiteSpace; }
            set { _noWhiteSpace = value; }
        }

        public bool OnlyCaps
        {
            get { return _onlyCaps; }
            set { _onlyCaps = value; }
        }

        public int MinLength
        {
            get { return _minLength; }
            set { _minLength = value; }
        }

        public int MaxLength
        {
            get { return textBoxEntry.MaxLength; }
            set { textBoxEntry.MaxLength = value; }
        }
	}
}
