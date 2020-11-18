using Seagate.AAS.HGA.HST.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using XyratexOSC.UI;
using XyratexOSC.XMath;
using Seagate.AAS.Parsel.Hw;

namespace Seagate.AAS.HGA.HST.Settings
{
    public class MoveProfileSettings
    {
        [Category("Input Station")]
        [DisplayName("Input EE Z (Max Acceleration = 3000, Max Decceleration = 3000, Max Velocity = 250)")]
        [Description("Input EE Z motion profile configuration.")]
        public MoveProfile InputEEZ
        {
            get;
            set;
        }

        [Category("Precisor Station")]
        [DisplayName("Precisor X (Max Acceleration = 3500, Max Decceleration = 3500, Max Velocity = 1000)")]
        [Description("Precisor X motion profile configuration.")]
        public MoveProfile PrecisorX
        {
            get;
            set;
        }

        [Category("Precisor Station")]
        [DisplayName("Precisor Y (Max Acceleration = 1000, Max Decceleration = 1000, Max Velocity = 50)")]
        [Description("Precisor Y motion profile configuration.")]
        public MoveProfile PrecisorY
        {
            get;
            set;
        }

        [Category("Precisor Station")]
        [DisplayName("Precisor Theta (Max Acceleration = 100, Max Decceleration = 100, Max Velocity = 10)")]
        [Description("Precisor Theta motion profile configuration.")]
        public MoveProfile PrecisorTheta
        {
            get;
            set;
        }

        [Category("Test Station")]
        [DisplayName("Test Probe Z (Max Acceleration = 3000, Max Decceleration = 3000, Max Velocity = 250)")]
        [Description("Test Probe Z motion profile configuration.")]
        public MoveProfile TestProbeZ
        {
            get;
            set;
        }

        [Category("Output Station")]
        [DisplayName("Output EE Z (Max Acceleration = 3000, Max Decceleration = 3000, Max Velocity = 250)")]
        [Description("Output EE Z motion profile configuration.")]
        public MoveProfile OutputEEZ
        {
            get;
            set;
        }

        public MoveProfileSettings()
        {
            InputEEZ = new MoveProfile();
            PrecisorX = new MoveProfile();
            PrecisorY = new MoveProfile();
            PrecisorTheta = new MoveProfile();
            TestProbeZ = new MoveProfile();
            OutputEEZ = new MoveProfile();
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class MoveProfile
        {

            public MoveProfile()
            {
                Acceleration = 0;
                Deceleration = 0;
                Velocity = 0;
                SettlingWindow = 0;
            }


            [Category("Settings")]
            [DisplayName("Acceleration(mm/s²)")]
            [Description("Acceleration of an axis")]
            public int Acceleration
            {
                get;

                set;

            }
            [Category("Settings")]
            [DisplayName("Decceleration(mm/s²)")]
            [Description("Decceleration of an axis")]
            public int Deceleration
            {
                get;

                set;

            }

            [Category("Settings")]
            [DisplayName("Velocity(mm/s²)")]
            [Description("Velocity of an axis")]
            public int Velocity
            {
                get;

                set;

            }

            [Category("Settings")]
            [DisplayName("SettlingWindow")]
            [Description("Settling Window of an axis in ms (miliseconds)")]
            public int SettlingWindow
            {
                get;

                set;

            }
        }
    }
}
