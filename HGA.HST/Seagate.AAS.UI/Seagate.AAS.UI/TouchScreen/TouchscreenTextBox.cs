using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Seagate.AAS.UI
{
	/// <summary>
	/// Summary description for TouchscreenTextBox.
	/// </summary>
	public class TouchscreenTextBox : System.Windows.Forms.TextBox
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private string title;
        private bool _alphaNumOnly = false;
        private bool _noWhiteSpace = false;        
        private bool _onlyCaps = false;        
        private int _minLength = 0;

        String previousString = "";

		public String FormTitle
		{
			get
			{
				return title;
			}
			set
			{
				title = value.ToString();
			}
		}

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                CheckText();
            }
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

        public int MinLength
        {
            get { return _minLength; }
            set { _minLength = value; }
        }
        
        public bool OnlyCaps
        {
            get { return _onlyCaps; }
            set { _onlyCaps = value; }
        }
        
        public TouchscreenTextBox()
		{
			InitializeComponent();
			title = "Enter Text";
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
		protected override void OnClick(EventArgs e)
		{
            if(this.ReadOnly)
                return;

			TextEntryForm textForm = new TextEntryForm(title,base.Text);
            textForm.AlphaNumOnly = _alphaNumOnly;
            textForm.NoWhiteSpace = _noWhiteSpace;
            textForm.MinLength    = _minLength;
            textForm.MaxLength    = this.MaxLength;
            textForm.OnlyCaps     = OnlyCaps;
            textForm.PasswordChar = this.PasswordChar;

			textForm.ShowDialog();
			if(textForm.DialogResult == DialogResult.OK)
				base.Text = textForm.StringEntered;
			textForm.Dispose();
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            // 
            // TouchscreenTextBox
            // 
            this.TextChanged += new System.EventHandler(this.TouchscreenTextBox_TextChanged);
            this.Leave += new System.EventHandler(this.TouchscreenTextBox_Leave);
            this.Enter += new System.EventHandler(this.TouchscreenTextBox_Enter);

        }
		#endregion

        private void TouchscreenTextBox_Leave(object sender, System.EventArgs e)
        {
            if(!CheckText())
                this.Text = previousString;
        }

        private void TouchscreenTextBox_Enter(object sender, System.EventArgs e)
        {
            previousString = this.Text;        
        }

        private void TouchscreenTextBox_TextChanged(object sender, System.EventArgs e)
        {
			if(_onlyCaps)
			{
				this.Text = this.Text.ToUpper();
				this.SelectionStart = this.Text.Length;
			}

            CheckText();
        }
        
        private bool CheckText()
        {          
            bool success = true;
         
            if(this.Text.Length > MaxLength || this.Text.Length < _minLength)
                success = false;            
            else
            {
                bool noWhiteSpace = true;
                bool allAlphaNum = true;

                for(int i=0; i<this.Text.Length; i++)
                {
                    char c = this.Text[i];
            
                    if(noWhiteSpace && Char.IsWhiteSpace(c))
                        noWhiteSpace = false;

                    if(allAlphaNum && !Char.IsLetter(c) && !Char.IsDigit(c))
                        allAlphaNum = false;
                }           
                       
                if(_noWhiteSpace && !noWhiteSpace)
                    success = false;

                else if(_alphaNumOnly && !allAlphaNum)
                    success = false;
            }

			if(this.ReadOnly)
			{
				this.BackColor = System.Drawing.Color.FromKnownColor(KnownColor.Control);
			}
			else
			{
				if(success)
				{
					this.BackColor = System.Drawing.Color.White;                
				}
				else
				{
					this.BackColor = System.Drawing.Color.Pink;            
				}
			}

            return success;
        }
	}
}
