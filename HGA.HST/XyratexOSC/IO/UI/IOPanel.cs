using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using XyratexOSC.IO;
using XyratexOSC.UI;

namespace XyratexOSC.IO.UI
{
    /// <summary>
    /// Provides a paged arrangement of LED controls that represent machine IO bits
    /// </summary>
    public partial class IOPanel : UserControl
    {
        /// <summary>The number of I/O bits displayed at a time.</summary>
        private const int NUM_IO_DISPLAY = 24;
        /// <summary>The added height difference with the I/O names showing.</summary>
        public const int EXP_HEIGHT_DIFF = 128;
        
        private IOComponent _io;
        private LEDButton[] _inBits;
        private LEDButton[] _outBits;
        private int _bitGroup = 0;
        private bool _namesShowing = false;

        /// <summary>
        /// Gets or sets a value indicating whether to display each IO bit label.
        /// </summary>
        /// <value>
        ///   <c>true</c> if displaying names; otherwise, <c>false</c>.
        /// </value>
        [ReadOnly(true)]
        public bool ShowNames
        {
            get
            {
                return _namesShowing;
            }
            set
            {
                if (_namesShowing == value)
                    return;

                _namesShowing = value;

                if (_namesShowing)
                {
                    toolTips.Active = false;
                    btnShowNames.Text = "Hide Names";
                    this.Height += EXP_HEIGHT_DIFF;
                }
                else
                {
                    toolTips.Active = true;
                    btnShowNames.Text = "Show Names";
                    this.Height -= EXP_HEIGHT_DIFF;
                }

                UpdateIOBits();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IOPanel"/> class.
        /// </summary>
        public IOPanel()
        {
            InitializeComponent();
            _io = IOComponent.Instance;

            _outBits = new LEDButton[24];
            _outBits[0] = ledButton1;
            _outBits[1] = ledButton2;
            _outBits[2] = ledButton3;
            _outBits[3] = ledButton4;
            _outBits[4] = ledButton5;
            _outBits[5] = ledButton6;
            _outBits[6] = ledButton7;
            _outBits[7] = ledButton8;
            _outBits[8] = ledButton9;
            _outBits[9] = ledButton10;
            _outBits[10] = ledButton11;
            _outBits[11] = ledButton12;
            _outBits[12] = ledButton13;
            _outBits[13] = ledButton14;
            _outBits[14] = ledButton15;
            _outBits[15] = ledButton16;
            _outBits[16] = ledButton17;
            _outBits[17] = ledButton18;
            _outBits[18] = ledButton19;
            _outBits[19] = ledButton20;
            _outBits[20] = ledButton21;
            _outBits[21] = ledButton22;
            _outBits[22] = ledButton23;
            _outBits[23] = ledButton24;

            _inBits = new LEDButton[24];
            _inBits[0] = ledButton25;
            _inBits[1] = ledButton26;
            _inBits[2] = ledButton27;
            _inBits[3] = ledButton28;
            _inBits[4] = ledButton29;
            _inBits[5] = ledButton30;
            _inBits[6] = ledButton31;
            _inBits[7] = ledButton32;
            _inBits[8] = ledButton33;
            _inBits[9] = ledButton34;
            _inBits[10] = ledButton35;
            _inBits[11] = ledButton36;
            _inBits[12] = ledButton37;
            _inBits[13] = ledButton38;
            _inBits[14] = ledButton39;
            _inBits[15] = ledButton40;
            _inBits[16] = ledButton41;
            _inBits[17] = ledButton42;
            _inBits[18] = ledButton43;
            _inBits[19] = ledButton44;
            _inBits[20] = ledButton45;
            _inBits[21] = ledButton46;
            _inBits[22] = ledButton47;
            _inBits[23] = ledButton48;

            for (int i = 0; i < NUM_IO_DISPLAY; i++)
            {
                _inBits[i].Tag = i;
                _outBits[i].Tag = i;
            }
        }

        private void IOPanel_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;

            ScrollIOBits.LargeChange = 1;
            ScrollIOBits.SmallChange = 1;
            ScrollIOBits.Maximum = Math.Max(_io.DIBits.Count, _io.DOBits.Count) / NUM_IO_DISPLAY;
            ScrollIOBits.Value = ScrollIOBits.Maximum;
            ScrollIOBits.Visible = (ScrollIOBits.Maximum > 0);
            TxtIOBitIndex.Visible = ScrollIOBits.Visible;

            UpdateIOBits();
        }

        /// <summary>
        /// Gets the I/O bits values.
        /// </summary>
        public void UpdateState()
        {
            int firstBit = (_bitGroup * NUM_IO_DISPLAY);

            int[] outValues = _io.DOBits.Skip(firstBit)
                                        .Take(24)
                                        .GetValues(TimeSpan.FromMilliseconds(100))
                                        .ToArray();

            int[] inValues = _io.DIBits.Skip(firstBit)
                            .Take(24)
                            .GetValues(TimeSpan.FromMilliseconds(100))
                            .ToArray();

            UIUtility.Invoke(this, () =>
            {
                for (int i = 0; i < outValues.Length; i++)
                    _outBits[i].Checked = outValues[i] != 0;

                for (int i = 0; i < inValues.Length; i++)
                    _inBits[i].Checked = inValues[i] != 0;
            });
        }

        private void UpdateIOBits()
        {
            toolTips.RemoveAll();

            for (int i = 0; i < NUM_IO_DISPLAY; i++)
            {
                int bitNum = i + NUM_IO_DISPLAY * _bitGroup;
                string name = "";

                //In Bit Name
                name = "";
                if (bitNum < _io.DIBits.Count)
                    name = _io.DIBits[bitNum].Name;

                toolTips.SetToolTip(_inBits[i], name);

                //In Bit readonly
                if (bitNum < _io.DIBits.Count && _io.DIBits[bitNum].Simulated)
                    _inBits[i].ReadOnly = false;
                else
                    _inBits[i].ReadOnly = true;

                //Out Bit Name
                name = "";
                if (bitNum < _io.DOBits.Count)
                    name = _io.DOBits[bitNum].Name;

                toolTips.SetToolTip(_outBits[i], name);

                //Out Bit Readonly
                if (bitNum < _io.DOBits.Count && _io.DOBits[bitNum].ReadOnly)
                    _outBits[i].ReadOnly = true;
                else
                    _outBits[i].ReadOnly = false;
            }

            Refresh();
        }

        private void IOPanel_Paint(object sender, PaintEventArgs e)
        {
            if (ShowNames)
            {
                //Paint Output Bit Labels
                float myAngle = -45;

                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                e.Graphics.RotateTransform(myAngle);

                System.Drawing.Font myFont = new System.Drawing.Font(Font.Name, 7.5F);

                int ledWidth = ledButton1.Width;
                float AdjStartX = -12;
                float AdjStartY = 74;

                for (int i = NUM_IO_DISPLAY - 1; i >= 0; i--)
                {
                    int bitIndex = i + _bitGroup * NUM_IO_DISPLAY;
                    if (bitIndex < _io.DOBits.Count)
                    {
                        string bitName = _io.DOBits[bitIndex].Name;
                        if (!String.IsNullOrEmpty(bitName))
                            bitName = "-" + bitName;

                        e.Graphics.DrawString(bitName, myFont, Brushes.Black, AdjStartX, AdjStartY);
                    }

                    e.Graphics.TranslateTransform(ledWidth - 5, ledWidth - 5);
                    if (i % 8 == 0)
                        e.Graphics.TranslateTransform(ledWidth - 11, ledWidth - 11);
                }

                e.Graphics.ResetTransform();
                e.Graphics.RotateTransform(-1 * myAngle);

                AdjStartX = 115;
                AdjStartY = 39;

                for (int i = NUM_IO_DISPLAY - 1; i >= 0; i--)
                {
                    int bitIndex = i + _bitGroup * NUM_IO_DISPLAY;
                    if (bitIndex < _io.DIBits.Count)
                    {
                        string bitName = _io.DIBits[bitIndex].Name;
                        if (!String.IsNullOrEmpty(bitName))
                            bitName = "-" + bitName;

                        e.Graphics.DrawString(bitName, myFont, Brushes.Black, AdjStartX, AdjStartY);
                    }

                    e.Graphics.TranslateTransform(ledWidth - 5, -1 * (ledWidth - 5));
                    if (i % 8 == 0)
                        e.Graphics.TranslateTransform(ledWidth - 11, -1 * (ledWidth - 11));
                }
            }
        }

        private void ScrollIOBits_Scroll(object sender, ScrollEventArgs e)
        {
            _bitGroup = ScrollIOBits.Maximum - ScrollIOBits.Value;
            TxtIOBitIndex.Text = Convert.ToString(_bitGroup + 1);
            lblBit0.Text = (_bitGroup * 24 + 0).ToString();
            lblBit7.Text = (_bitGroup * 24 + 7).ToString();
            lblBit8.Text = (_bitGroup * 24 + 8).ToString();
            lblBit15.Text = (_bitGroup * 24 + 15).ToString();
            lblBit16.Text = (_bitGroup * 24 + 16).ToString();
            lblBit23.Text = (_bitGroup * 24 + 23).ToString();

            UpdateState();
            UpdateIOBits();
        }

        private void OutBit_Click(object sender, EventArgs e)
        {
            LEDButton led = sender as LEDButton;

            if (led == null || led.ReadOnly)
                return;

            int bitNum = Convert.ToInt32(led.Tag) + _bitGroup * NUM_IO_DISPLAY;
            if (bitNum >= _io.DOBits.Count)
                return;

            DOBit bit = _io.DOBits[bitNum];
            if (bit == null)
                return;

            bool prevState = led.Checked;

            led.Enabled = false;

            Task.Factory.StartNew<bool>(() =>
            {
                // toggle the value                
                bool value = !bit.Get();
                bit.Set(value);
                return value;
            })
            .ContinueWith(t =>
            {
                UIUtility.BeginInvoke(this, () =>
                {
                    led.Checked = t.Result;
                    led.Enabled = true;
                });

                if (t.IsFaulted)
                    Notify.PopUpError(String.Format("Failed to Set {0}", bit.Name), t.Exception.InnerException);
            });
        }

        private void InBit_Click(object sender, EventArgs e)
        {
            LEDButton led = sender as LEDButton;

            if (led == null || led.ReadOnly)
                return;

            int bitNum = Convert.ToInt32(led.Tag) + _bitGroup * NUM_IO_DISPLAY;
            if (bitNum >= _io.DIBits.Count)
                return;

            DIBit bit = _io.DIBits[bitNum];
            if (bit == null)
                return;

            bool prevState = led.Checked;

            led.Enabled = false;

            Task.Factory.StartNew<bool>(() =>
            {
                // toggle the value                
                bool value = !bit.Get();
                bit.Set(value);
                return value;
            })
            .ContinueWith(t =>
            {
                UIUtility.BeginInvoke(this, () =>
                {
                    led.Checked = t.Result;
                    led.Enabled = true;
                });

                if (t.IsFaulted)
                    Notify.PopUpError(String.Format("Failed to Set {0}", bit.Name), t.Exception.InnerException);
            });
        }

        private void btnShowNames_Click(object sender, EventArgs e)
        {
            ShowNames = !ShowNames;
        }
    }
}