using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XyratexOSC.UI
{
    /// <summary>
    /// A Windows 7 style IP address textbox interface. This simplifies IP address input, and performs validation.
    /// </summary>
    public partial class IPAddressTextBox : UserControl
    {
        /// <summary>
        /// Gets or sets the IP-v4 address using shortened formatting. (ex. 192.168.1.1).
        /// </summary>
        /// <value>
        /// The IP address.
        /// </value>
        public string IPAddress
        {
            get
            {
                string[] addr = new string[4];

                if (String.IsNullOrEmpty(textBox1.Text))
                    addr[0] = "0";
                else
                    addr[0] = textBox1.Text;

                if (String.IsNullOrEmpty(textBox2.Text))
                    addr[1] = "0";
                else
                    addr[1] = textBox2.Text;

                if (String.IsNullOrEmpty(textBox3.Text))
                    addr[2] = "0";
                else
                    addr[2] = textBox3.Text;

                if (String.IsNullOrEmpty(textBox4.Text))
                    addr[3] = "0";
                else
                    addr[3] = textBox4.Text;

                return String.Join(".",addr);
            }
            set
            {
                SetIP(value);
            }
        }

        /// <summary>
        /// Gets or sets the IP-v4 address using full formatting. (ex. 192.168.001.001).
        /// </summary>
        /// <value>
        /// The IP address.
        /// </value>
        public string IPAddressFullFormat
        {
            get
            {
                int[] addr = new int[4];

                try
                {
                    if (String.IsNullOrEmpty(textBox1.Text))
                        addr[0] = 0;
                    else
                        addr[0] = Int32.Parse(textBox1.Text);

                    if (String.IsNullOrEmpty(textBox2.Text))
                        addr[1] = 0;
                    else
                        addr[1] = Int32.Parse(textBox2.Text);

                    if (String.IsNullOrEmpty(textBox3.Text))
                        addr[2] = 0;
                    else
                        addr[2] = Int32.Parse(textBox3.Text);

                    if (String.IsNullOrEmpty(textBox4.Text))
                        addr[3] = 0;
                    else
                        addr[3] = Int32.Parse(textBox4.Text);
                }
                catch (Exception)
                {
                    return "000.000.000.000";
                }

                return String.Format("{0:D3}.{1:D3}.{2:D3}.{3:D3}",
                    addr[0],
                    addr[1],
                    addr[2],
                    addr[3]);
            }
            set
            {
                SetIP(value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IPAddressTextBox"/> class.
        /// </summary>
        public IPAddressTextBox()
        {
            InitializeComponent();
        }

        private void SetIP(string address)
        {
            string[] addr = address.Split('.');
            if (addr.Length != 4)
                return;

            try
            {
                textBox1.Text = addr[0];
                textBox2.Text = addr[1];
                textBox3.Text = addr[2];
                textBox4.Text = addr[3];
            }
            catch
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
            }
        }

        private void Text_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox current = (TextBox)sender;
            e.Handled = true;

            if (e.KeyChar == '.' || 
                e.KeyChar == ' ' ||
                current.Text.Length - current.SelectionLength >= 3)
            {
                TextBox next = current.Tag as TextBox;

                if (next != null)
                    next.Focus();

                return;
            }
            else if (e.KeyChar >= '0' && e.KeyChar <= '9')
            {
                e.Handled = false;
            }
        }

        private void Text_TextChanged(object sender, EventArgs e)
        {
            TextBox current = (TextBox)sender;
            if (String.IsNullOrWhiteSpace(current.Text))
            {
                current.Text = "";
                return;
            }

            int num;

            if (Int32.TryParse(current.Text, out num) && num <= 255)
                return;
            else
                current.Text = "255";
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            TextBox current = sender as TextBox;
            current.SelectAll();
        }
    }
}
