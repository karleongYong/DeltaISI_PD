using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XyratexOSC.Properties;

namespace XyratexOSC.UI
{
    /// <summary>
    /// Provides a clickable LED indicator with on/pressed/off states. Used for IO bits.
    /// </summary>
    public partial class LEDButton : CheckBox
    {
        private LEDColor _color;
        private bool _mouseDown;
        private bool _mouseOver;
        private bool _readOnly;

        private Image[] _blueLED = new Image[] { Resources.LEDBTN_Blue_ro, Resources.LEDBTN_Blue_dn, Resources.LEDBTN_Blue_ov, Resources.LEDBTN_Blue_up };
        private Image[] _greenLED = new Image[] { Resources.LEDBTN_Green_ro, Resources.LEDBTN_Green_dn, Resources.LEDBTN_Green_ov, Resources.LEDBTN_Green_up };
        private Image[] _orangeLED = new Image[] { Resources.LEDBTN_Orange_ro, Resources.LEDBTN_Orange_dn, Resources.LEDBTN_Orange_ov, Resources.LEDBTN_Orange_up };
        private Image[] _yellowLED = new Image[] { Resources.LEDBTN_Yellow_ro, Resources.LEDBTN_Yellow_dn, Resources.LEDBTN_Yellow_ov, Resources.LEDBTN_Yellow_up };
        private Image[] _redLED = new Image[] { Resources.LEDBTN_Red_ro, Resources.LEDBTN_Red_dn, Resources.LEDBTN_Red_ov, Resources.LEDBTN_Red_up };
        private Image[] _uncheckedImages = new Image[] { Resources.LEDBTN_Grey_ro, Resources.LEDBTN_Grey_dn, Resources.LEDBTN_Grey_ov, Resources.LEDBTN_Grey_up };
        private Image[] _checkedImages;

        /// <summary>
        /// Gets or sets the LED color.
        /// </summary>
        /// <value>
        /// The LED color.
        /// </value>
        public LEDColor Color
        {
            get
            {
                return _color;
            }
            set
            {
                if (_color == value)
                    return;

                _color = value;

                switch (_color)
                {
                    case LEDColor.Blue:
                        _checkedImages = _blueLED;
                        break;
                    case LEDColor.Green:
                        _checkedImages = _greenLED;
                        break;
                    case LEDColor.Orange:
                        _checkedImages = _orangeLED;
                        break;
                    case LEDColor.Yellow:
                        _checkedImages = _yellowLED;
                        break;
                    case LEDColor.Red:
                        _checkedImages = _redLED;
                        break;
                }

                UpdateImage();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this LED button accepts user input.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the LED rejects user input; otherwise, <c>false</c>.
        /// </value>
        public bool ReadOnly
        {
            get
            {
                return _readOnly;
            }
            set
            {
                if (_readOnly == value)
                    return;

                _readOnly = value;

                this.AutoCheck = !_readOnly;
                this.Cursor = _readOnly ? Cursors.Default : Cursors.Hand;

                UpdateImage();
            }
        }

        private int ButtonState
        {
            get
            {
                if (_readOnly)
                    return 0;
                else if (_mouseDown)
                    return 1;
                else if (_mouseOver)
                    return 2;
                else
                    return 3;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control should display focus rectangles.
        /// </summary>
        /// <returns>true if the control should display focus rectangles; otherwise, false.</returns>
        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LEDButton"/> class.
        /// </summary>
        public LEDButton()
        {
            _checkedImages = _redLED;
            InitializeComponent();
            DoubleBuffered = true;

            this.AutoCheck = !_readOnly;
            this.Cursor = _readOnly ? Cursors.Default : Cursors.Hand;
        }

        private void LEDButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateImage();
        }

        private void UpdateImage()
        {
            Image newImage = null;

            if (this.Checked)
                newImage = _checkedImages[ButtonState];
            else
                newImage = _uncheckedImages[ButtonState];

            if (newImage != this.Image)
                this.Image = newImage;
        }

        private void LEDButton_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;
            UpdateImage();
        }

        private void LEDButton_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
            UpdateImage();
        }

        private void LEDButton_MouseCaptureChanged(object sender, EventArgs e)
        {
            _mouseOver = false;
            _mouseDown = false;
            UpdateImage();
        }

        private void LEDButton_MouseMove(object sender, MouseEventArgs e)
        {
            _mouseOver = true;
            UpdateImage();
        }

        private void LEDButton_MouseLeave(object sender, EventArgs e)
        {
            _mouseOver = false;
            UpdateImage();
        }
    }
}