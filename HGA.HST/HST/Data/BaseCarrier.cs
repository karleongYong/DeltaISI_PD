using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Seagate.AAS.HGA.HST.Vision;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.Parsel.Device.RFID.Hga;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Machine;

namespace Seagate.AAS.HGA.HST.Data
{
    [Serializable]
    [TypeConverter(typeof(NamedConverter<CarrierSettings>))]
    public class BaseCarrier
    {
        private IsCarrierEmpty _isCarrierEmpty = IsCarrierEmpty.Unknown;        
        private Hga _hga1;
        private Hga _hga2;
        private Hga _hga3;
        private Hga _hga4;
        private Hga _hga5;
        private Hga _hga6;
        private Hga _hga7;
        private Hga _hga8;
        private Hga _hga9;
        private Hga _hga10;
        private bool _isMeasurementTestDone = false;
        private bool _isPassThroughMode = false;
        private bool _isLoadedInWrongDirection = false;
        private bool _isDycemBoat = false;
        private bool _isRejectedCarrier = false;
        private CarrierLocation _carrier_location = CarrierLocation.InputTurnStation;        
        private double[] _precisorNestPosition = new double[4];
        
        public BaseCarrier()
        {
            CarrierID = CommonFunctions.UNKNOWN;
            ImageFileName = CommonFunctions.UNKNOWN;

            _hga1 = new Hga(1, HGAStatus.NoHGAPresent);
            _hga2 = new Hga(2, HGAStatus.NoHGAPresent);
            _hga3 = new Hga(3, HGAStatus.NoHGAPresent);
            _hga4 = new Hga(4, HGAStatus.NoHGAPresent);
            _hga5 = new Hga(5, HGAStatus.NoHGAPresent);
            _hga6 = new Hga(6, HGAStatus.NoHGAPresent);
            _hga7 = new Hga(7, HGAStatus.NoHGAPresent);
            _hga8 = new Hga(8, HGAStatus.NoHGAPresent);
            _hga9 = new Hga(9, HGAStatus.NoHGAPresent);
            _hga10 = new Hga(10, HGAStatus.NoHGAPresent);

            _precisorNestPosition[0] = 0.00;
            _precisorNestPosition[1] = 0.00;
            _precisorNestPosition[2] = 0.00;
            _precisorNestPosition[3] = 0.00;
        }        

        public void setPrecisorNestPositionX(double valueX)
        {
            _precisorNestPosition[0] = valueX;
        }

        public void setPrecisorNestPositionY(double valueY)
        {
            _precisorNestPosition[1] = valueY;   
        }

        public void setPrecisorNestPositionZ(double valueZ)
        {
            _precisorNestPosition[2] = valueZ;
        }

        public void setPrecisorNestPositionTheta(double valueTheta)
        {
            _precisorNestPosition[3] = valueTheta;
        }

        public double getPrecisorNestPositionX()
        {
            return _precisorNestPosition[0];
        }

        public double getPrecisorNestPositionY()
        {
            return _precisorNestPosition[1];
        }

        public double getPrecisorNestPositionZ()
        {
            return _precisorNestPosition[2];
        }

        public double getPrecisorNestPositionTheta()
        {
            return _precisorNestPosition[3];
        }

        [Category("Carrier")]
        [DisplayName("Carrier ID")]
        [Description("Unique Carrier ID.")]
        public string CarrierID
        {
            get;
            set;
        }

        [ReadOnly(false)]
        [Browsable(false)]
        [Category("Carrier")]
        [DisplayName("Is Carrier Empty")]
        [Description("Specify whether the carrier is empty with no HGAs loaded.")]      
        public IsCarrierEmpty IsCarrierEmpty
        {
            get
            {
                return _isCarrierEmpty;
            }
            set
            {
                _isCarrierEmpty = value;
            }
        }

        [Category("Carrier")]
        [DisplayName("Is Pass Through Mode")]
        [Description("Enable Pass Through Mode.")]
        [Browsable(false)]
        public bool IsPassThroughMode
        {
            get
            {
                return _isPassThroughMode;
            }
            set
            {
                _isPassThroughMode = value;
            }
        }

        [Category("Carrier")]
        [DisplayName("Is Wrong Direction")]
        [Description("Carrier is loaded in wrong direction.")]
        [Browsable(false)]
        public bool IsLoadedInWrongDirection
        {
            get
            {
                return _isLoadedInWrongDirection;
            }
            set
            {
                _isLoadedInWrongDirection = value;
            }
        }

        [Category("Carrier")]
        [DisplayName("Is a Dycem Boat")]
        [Description("Check whether boat is a dycem boat or not.")]
        [Browsable(false)]
        public bool IsDycemBoat
        {
            get
            {
                return _isDycemBoat;
            }
            set
            {
                _isDycemBoat = value;
            }
        }

        [Category("Carrier")]
        [DisplayName("Is a rejected carrier")]
        [Description("Carrier is rejected.")]
        [Browsable(false)]
        public bool IsRejectedCarrier
        {
            get
            {
                return _isRejectedCarrier;
            }
            set
            {
                _isRejectedCarrier = value;
            }
        }

        [Category("Measurement")]
        [DisplayName("Is Measurement Test Done")]
        [Description("Specify whether to skip the measurement test.")]
        [Browsable(false)]
        public bool IsMeasurementTestDone
        {
            get
            {
                return _isMeasurementTestDone;
            }
            set
            {
                _isMeasurementTestDone = value;
            }
        }

        [Category("Vision")]
        [DisplayName("Image File Name")]
        [Description("Full Path File Name of the Carrier Image.")]
        [Browsable(false)]
        public string ImageFileName
        {
            get;
            set;
        }


        [Category("HGAs")]
        [DisplayName("HGA1")]
        [Description("HGA1's information")]
        public Hga Hga1
        {
            get
            {
                return _hga1;
            }
            set
            {
                _hga1 = value;
            }
        }

        [Category("HGAs")]
        [DisplayName("HGA2")]
        [Description("HGA2's information")]
        public Hga Hga2
        {
            get
            {
                return _hga2;
            }
            set
            {
                _hga2 = value;
            }
        }

        [Category("HGAs")]
        [DisplayName("HGA3")]
        [Description("HGA3's information")]
        public Hga Hga3
        {
            get
            {
                return _hga3;
            }
            set
            {
                _hga3 = value;
            }
        }

        [Category("HGAs")]
        [DisplayName("HGA4")]
        [Description("HGA4's information")]
        public Hga Hga4
        {
            get
            {
                return _hga4;
            }
            set
            {
                _hga4 = value;
            }
        }

        [Category("HGAs")]
        [DisplayName("HGA5")]
        [Description("HGA5's information")]
        public Hga Hga5
        {
            get
            {
                return _hga5;
            }
            set
            {
                _hga5 = value;
            }
        }

        [Category("HGAs")]
        [DisplayName("HGA6")]
        [Description("HGA6's information")]
        public Hga Hga6
        {
            get
            {
                return _hga6;
            }
            set
            {
                _hga6 = value;
            }
        }

        [Category("HGAs")]
        [DisplayName("HGA7")]
        [Description("HGA7's information")]
        public Hga Hga7
        {
            get
            {
                return _hga7;
            }
            set
            {
                _hga7 = value;
            }
        }

        [Category("HGAs")]
        [DisplayName("HGA8")]
        [Description("HGA8's information")]
        public Hga Hga8
        {
            get
            {
                return _hga8;
            }
            set
            {
                _hga8 = value;
            }
        }

        [Category("HGAs")]
        [DisplayName("HGA9")]
        [Description("HGA9's information")]
        public Hga Hga9
        {
            get
            {
                return _hga9;
            }
            set
            {
                _hga9 = value;
            }
        }

        [Category("HGAs")]
        [DisplayName("HGA10")]
        [Description("HGA10's information")]
        public Hga Hga10
        {
            get
            {
                return _hga10;
            }
            set
            {
                _hga10 = value;
            }
        }

        [Browsable(false)]
        public CarrierLocation CarrierCurrentLocation
        {
            get
            {
                return _carrier_location;
            }
            set
            {
                _carrier_location = value;
            }
        }
    }
}
