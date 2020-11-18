using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LDUMeasurement
{
    public delegate void EventLoadLDUSettingsHandler(object source, EventLDUSettings e);
    public delegate void EventLDUHandler(object source, EventLDUSettings e);

    public delegate void EventLDUPointHandler(object source, EventDataPoint e);
    public class EventLDUSettings : EventArgs
    {
        private bool LDUEnable;
        private int LEDStartCurrent;
        private int LEDStopCurrent;
        private int LEDStepSize;
        private bool LDUSweepMode;
        private int LDUStartCurrent;
        private int LDUStopCurrent;
        private int LDUStepSize;
        private int LDUISweep1;
        private int LDUISweep2;
        private int LDUISweep3;
        private int LDUISweep4;
        private int LDUTimeInterval;
        private bool FourPointMode;
        private int T1StopPoint;
        private int T2StartPoint;
        public EventLDUSettings(bool _LDUEnable,         
                                int _LEDStartCurrent,
                                int _LEDStopCurrent,
                                int _LEDStepSize,
                                bool _LDUSweepMode,
                                int _LDUStartCurrent,
                                int _LDUStopCurrent,
                                int _LDUStepSize,
                                bool _i4PointMode,
                                int _LDUISweep1,
                                int _LDUISweep2,
                                int _LDUISweep3,
                                int _LDUISweep4,
                                int _LDUTimeInterval,
                                int _T1StopPoint,
                                int _T2StartPoint)
        {

            LDUEnable = _LDUEnable;
            LEDStartCurrent = _LEDStartCurrent;
            LEDStopCurrent = _LEDStopCurrent;
            LEDStepSize = _LEDStepSize;
            LDUSweepMode = _LDUSweepMode;
            LDUStartCurrent = _LDUStartCurrent;
            LDUStopCurrent = _LDUStopCurrent;
            LDUStepSize = _LDUStepSize;
            LDUISweep1 = _LDUISweep1;
            LDUISweep2 = _LDUISweep2;
            LDUISweep3 = _LDUISweep3;
            LDUISweep4 = _LDUISweep4;
            LDUTimeInterval = _LDUTimeInterval;
            FourPointMode = _i4PointMode;
            T1StopPoint = _T1StopPoint;
            T2StartPoint = _T2StartPoint;
    }

        public bool IsLDUEnable()
        {
            return LDUEnable;
        }
        public int GetLEDStartCurrent()
        {
            return LEDStartCurrent;
        }
        public int GetLEDStopCurrent()
        {
            return LEDStopCurrent;
        }

        public int GetLEDStepSize()
        {
            return LEDStepSize;
        }

        public bool IsLDUSweepMode()
        {
            return LDUSweepMode;
        }

        public int GetLDUStartCurrent()
        {
            return LDUStartCurrent;
        }
        public int GetLDUStopCurrent()
        {
            return LDUStopCurrent;
        }

        public int GetLDUStepSize()
        {
            return LDUStepSize;
        }
        public int GetT1StopPoint()
        {
            return T1StopPoint;
        }

        public int GetT2StartPoint()
        {
            return T2StartPoint;
        }
        public int GetILDUSweep1()
        {
            return LDUISweep1;
        }
        public int GetILDUSweep2()
        {
            return LDUISweep2;
        }

        public int GetILDUSweep3()
        {
            return LDUISweep3;
        }

        public int GetILDUSweep4()
        {
            return LDUISweep4;
        }

        public int GetLDUTimeInterval()
        {
            return LDUTimeInterval;
        }

        public bool GetLDU4PtMode()
        {
            return FourPointMode;
        }

    }

    public class EventDataPoint : EventArgs
    {
        int HGAIndex;
        double [] point;
        public EventDataPoint(int _HGAIndex, double [] _point)
        {
            HGAIndex = _HGAIndex;
            point = _point;
        }
        public int GetHGAIndex()
        {
            return HGAIndex;
        }

        public double [] GetPoint()
        {
            return point;
        }
    }
    public class EventLDU
    {
    }
}
