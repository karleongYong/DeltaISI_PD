using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Seagate.AAS.HGA.HST.Vision;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.Parsel.Device.RFID.Hga;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Machine;

namespace Seagate.AAS.HGA.HST.Data
{
    [Serializable]
    public class Carrier : BaseCarrier 
    {
        private RFIDInfo _rfidData;
        private WorkOrderData _workOrderData;
        private RFIDInfoVerify _rfidInfoVerify = RFIDInfoVerify.Unknown;
        private HGAProductTabType _HGATabType = HGAProductTabType.Unknown;
        private bool _capacitance_ch1_enable = true;
        private bool _capacitance_ch2_enable = true;

        public RFIDInfo RFIDData
        {
            get
            {
                return _rfidData;
            }
            set
            {
                _rfidData = value;
            }
        }

        public WorkOrderData WorkOrderData
        {
            get
            {
                return _workOrderData;
            }
            set
            {
                _workOrderData = value;
            }
        }

        public RFIDInfoVerify RFIDInfoVerify
        {
            get
            {
                return _rfidInfoVerify;
            }
            set
            {
                _rfidInfoVerify = value;
            }
        }

        public HGAProductTabType HGATabType
        {
            get
            {
                return _HGATabType;
            }
            set
            {
                _HGATabType = value;
            }
        }


        public bool CapacitanceCH1_Enable
        {
            get
            {
                return _capacitance_ch1_enable;
            }
            set
            {
                _capacitance_ch1_enable = value;
            }
        }

        public bool CapacitanceCH2_Enable
        {
            get
            {
                return _capacitance_ch2_enable;
            }
            set
            {
                _capacitance_ch2_enable = value;
            }
        }
    }

    public static class ExtObject
    {
        public static T DeepCopy<T>(this T objectToCopy)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, objectToCopy);

            memoryStream.Position = 0;
            T returnValue = (T)binaryFormatter.Deserialize(memoryStream);

            memoryStream.Close();
            memoryStream.Dispose();

            return returnValue;
        }
    }
}
