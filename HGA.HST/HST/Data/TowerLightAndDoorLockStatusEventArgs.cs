using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Seagate.AAS.HGA.HST.Data
{
    public class TowerLightAndDoorLockStatusEventArgs : EventArgs
    {
        public bool AmberLightOn
        {
            get;
            set;
        }

        public bool RedLightOn
        {
            get;
            set;
        }

        public bool DoorLock
        {
            get;
            set;
        }

        public TowerLightAndDoorLockStatusEventArgs(bool amberLightOn, bool redLightOn, bool doorLock)
        {
            AmberLightOn = amberLightOn;
            RedLightOn = redLightOn;
            DoorLock = doorLock;
        }
    }
}

