using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XyratexOSC.UI
{
    /// <summary>
    /// Provides an LED indicator with on/off states
    /// </summary>
    public partial class LEDIndicator : UserControl
    {
        private Image _onColor = Properties.Resources.LED_Red_On;
        private Image _offColor = Properties.Resources.LED_Off;
        private LEDColor _color;
        private bool _on = false;

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
                        _onColor = Properties.Resources.LED_Blue_On;
                        break;
                    case LEDColor.Green:
                        _onColor = Properties.Resources.LED_Green_On;
                        break;
                    case LEDColor.Orange:
                        _onColor = Properties.Resources.LED_Orange_On;
                        break;
                    case LEDColor.Yellow:
                        _onColor = Properties.Resources.LED_Yellow_On;
                        break;
                    case LEDColor.Red:
                        _onColor = Properties.Resources.LED_Red_On;
                        break;
                }

                UpdateImage();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LEDIndicator"/> is on.
        /// </summary>
        /// <value>
        ///   <c>true</c> if on; otherwise, <c>false</c>.
        /// </value>
        public bool On
        {
            get
            {
                return _on;
            }
            set
            {
                if (_on == value)
                    return;

                _on = value;

                UpdateImage();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LEDIndicator"/> class.
        /// </summary>
        public LEDIndicator()
        {
            InitializeComponent();
            DoubleBuffered = true;

            BackgroundImage = _offColor;
        }

        private void UpdateImage()
        {
            if (!On)
                BackgroundImage = _offColor;
            else
                BackgroundImage = _onColor;
        }
    }
}