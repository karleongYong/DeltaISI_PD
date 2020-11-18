using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace XyratexOSC.IO
{
    /// <summary>
    /// Represents a light tower beacon
    /// </summary>
    public class LightTower : IDisposable
    {
        private IOComponent _IO = IOComponent.Instance;
        private Timer _blinkTimer;
        private LightStackColor _blinking;
        private LightStackColor _steady;
        private LightStackColor _current;
        private bool _blinkOn;

        private DOBit[] _bits;
        private DOBit _red;
        private DOBit _yellow;
        private DOBit _green;
        private DOBit _blue;
        
        /// <summary>
        /// Event to update light stack in the UI at the same timer as the blinking timer
        /// </summary>
        public event EventHandler<LightStackEventArgs> StackUpdate;

        /// <summary>
        /// Initializes a new instance of the <see cref="LightTower"/> class using the 
        /// default DO bit names : 'BeaconRed','BeaconYellow','BeaconGeen','BeaconBlue'.
        /// </summary>
        public LightTower()
            : this("BeaconRed", "BeaconYellow", "BeaconGreen", "BeaconBlue")
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightTower"/> class.
        /// </summary>
        /// <param name="redName">Name of the red DO bit.</param>
        /// <param name="yellowName">Name of the yellow DO bit.</param>
        /// <param name="greenName">Name of the green DO bit.</param>
        /// <param name="blueName">Name of the blue DO bit.</param>
        /// <exception cref="System.ArgumentException">
        /// redName
        /// or
        /// yellowName 
        /// or
        /// greenName
        /// or
        /// blueName
        /// </exception>
        public LightTower(string redName, string yellowName, string greenName, string blueName)
            : this(IOComponent.Instance.DOBits[redName],
                   IOComponent.Instance.DOBits[yellowName],
                   IOComponent.Instance.DOBits[greenName], 
                   IOComponent.Instance.DOBits[blueName])
        {
            if (_red == null)
                throw new ArgumentException("redName", String.Format("DO channel '{0}' does not exist in the current IO configuration.", redName));
            if (_yellow == null)
                throw new ArgumentException("yellowName", String.Format("DO channel '{0}' does not exist in the current IO configuration.", yellowName));
            if (_green == null)
                throw new ArgumentException("greenName", String.Format("DO channel '{0}' does not exist in the current IO configuration.", greenName));
            if (_blue == null)
                throw new ArgumentException("blueName", String.Format("DO channel '{0}' does not exist in the current IO configuration.", blueName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightTower"/> class.
        /// </summary>
        /// <param name="red">The red DO bit.</param>
        /// <param name="yellow">The yellow DO bit.</param>
        /// <param name="green">The green DO bit.</param>
        /// <param name="blue">The blue DO bit.</param>
        public LightTower(DOBit red, DOBit yellow, DOBit green, DOBit blue)
        {
            _red = red;
            _yellow = yellow;
            _green = green;
            _blue = blue;
            _bits = new DOBit[] { _red, _yellow, _green, _blue };

            _blinkTimer = new Timer();
            _blinkTimer.Interval = 500;
            _blinkTimer.Elapsed += BlinkTimer_Elapsed;
            _blinkTimer.Enabled = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _blinkTimer.Elapsed -= BlinkTimer_Elapsed;
            _blinkTimer.Enabled = false;

            _red.Set(false);
            _yellow.Set(false);
            _green.Set(false);
            _blue.Set(false);
        }

        private void BlinkTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _blinkTimer.Elapsed -= BlinkTimer_Elapsed;

            try
            {
                //
                // Set light stack
                //

                if (!_blinkOn)
                    Set(_steady);
                else
                    Set(_blinking | _steady);

                // Get current light stack (use GetOutWordCached to get all light output bits in one set)

                _current = Get();

                EventHandler<LightStackEventArgs> stackUpdate = StackUpdate;
                if (stackUpdate != null)
                    stackUpdate(this, new LightStackEventArgs(_current));
            }
            finally
            {
                _blinkOn = !_blinkOn;

                _blinkTimer.Elapsed += BlinkTimer_Elapsed;
            }
        }

        private LightStackColor Get()
        {
            LightStackColor color = LightStackColor.Off;

            bool[] values = _bits.Get().ToArray();

            if (values[0])
                color |= LightStackColor.Red;
            if (values[1])
                color |= LightStackColor.Yellow;
            if (values[2])
                color |= LightStackColor.Green;
            if (values[3])
                color |= LightStackColor.Blue;

            return color;
        }

        private void Set(LightStackColor colors)
        {
            int[] values = new int[4];

            if (colors.HasFlag(LightStackColor.Red))
                values[0] = 1;
            if (colors.HasFlag(LightStackColor.Yellow))
                values[1] = 1;
            if (colors.HasFlag(LightStackColor.Green))
                values[2] = 1;
            if (colors.HasFlag(LightStackColor.Blue))
                values[3] = 1;

            _bits.SetValues(values);
        }

        /// <summary>
        /// Sets the specified colors to blink at every half-second interval.
        /// </summary>
        /// <param name="colors">The colors.</param>
        public void Blink(LightStackColor colors)
        {
            if (colors.HasFlag(LightStackColor.Red))
                _steady &= ~LightStackColor.Red;
            if (colors.HasFlag(LightStackColor.Yellow))
                _steady &= ~LightStackColor.Yellow;
            if (colors.HasFlag(LightStackColor.Green))
                _steady &= ~LightStackColor.Green;
            if (colors.HasFlag(LightStackColor.Blue))
                _steady &= ~LightStackColor.Blue;

            _blinking |= colors;
        }

        /// <summary>
        /// Sets the specified colors to steady on.
        /// </summary>
        /// <param name="colors">The colors.</param>
        public void Steady(LightStackColor colors)
        {
            _steady |= colors;
        }

        /// <summary>
        /// Turns off the specified colors.
        /// </summary>
        /// <param name="colors">The colors.</param>
        public void Off(LightStackColor colors)
        {
            _steady &= ~colors;
            _blinking &= ~colors;
        }

        /// <summary>
        /// Gets the current stack colors.
        /// </summary>
        /// <returns></returns>
        public LightStackColor GetStack()
        {
            return _current;
        }

        /// <summary>
        /// Gets the current blinking stack colors.
        /// </summary>
        /// <returns></returns>
        public LightStackColor GetBlink()
        {
            return _blinking;
        }

        /// <summary>
        /// Gets the current steady stack colors.
        /// </summary>
        /// <returns></returns>
        public LightStackColor GetSteady()
        {
            return _steady;
        }
    }

    /// <summary>
    /// Light Stack event data.
    /// </summary>
    public class LightStackEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the current light stack colors.
        /// </summary>
        /// <value>
        /// The colors.
        /// </value>
        public LightStackColor Colors
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LightStackEventArgs"/> class.
        /// </summary>
        /// <param name="colors">The colors.</param>
        public LightStackEventArgs(LightStackColor colors)
        {
            Colors = colors;
        }
    }
}
