using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    public enum GemEventId
    {
        EquipmentOffLine = 1,
        ControlStateLocal = 2,
        ControlStateRemote = 3,
        OperatorCommandIssued = 4,
        ProcessingStarted = 5,
        ProcessingCompleted = 6,
        ProcessingStopped = 7,
        ProcessingStateChange = 8,
        AlarmDetected = 9,
        AlarmCleared = 10,
        MaterialReceived = 16,
        MaterialRemoved = 17
    }
}
