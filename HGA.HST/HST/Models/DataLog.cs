using System;
using System.Collections.Generic;
using System.Linq;
using Seagate.AAS.EFI.Log;
using Seagate.AAS.HGA.HST.Utils;


namespace Seagate.AAS.HGA.HST.Models
{
    public class DataLog
    {        
        public string CarrierID
        {
            get;
            set;
        }

        public int CarrierSlot
        {
            get;
            set;
        }

        public string HGASerialNumber
        {
            get;
            set;
        }

        public HGAStatus HGAStatus
        {
            get;
            set;
        }

        public double CycleTime
        {
            get;
            set;
        }

        public string WorkOrder
        {
            get;
            set;
        }

        public string ErrorMessageCode
        {
            get;
            set;
        }

        public string SetupFileName
        {
            get;
            set;
        }

        public double ReaderResistance
        {
            get;
            set;
        }
        public double Reader2Resistance
        {
            get;
            set;
        }
        public double DeltaISIReader1
        {
            get;
            set;
        }
        public double DeltaISIReader2
        {
            get;
            set;
        }
        public double WriterResistance
        {
            get;
            set;
        }

        public double rHeaterResistance 
        {
            get;
            set;
        }

        public double wHeaterResistance 
        {
            get;
            set;
        }

        public double TAResistance
        {
            get;
            set;
        }

        public ShortTest ShortTest
        {
            get;
            set;
        }

        public string ShortTestPosition
        {
            get;
            set;
        }

        public double MicroActuatorCapacitance1
        {
            get;
            set;
        }

        public double MicroActuatorCapacitance2
        {
            get;
            set;
        }

        public string ThermisterTemperature
        {
            get;
            set;
        }

        public double BiasVoltage
        {
            get;
            set;
        }

        public double BiasCurrent
        {
            get;
            set;
        }

        public string ResistanceSpec
        {
            get;
            set;
        }

        public string CapacitanceSpec1
        {
            get;
            set;
        }
        public string CapacitanceSpec2
        {
            get;
            set;
        }

        public string MachineLocation
        {
            get;
            set;
        }

        public string EquipmentID
        {
            get;
            set;
        }

        public string EquipmentType
        {
            get;
            set;
        }

       public int UPH
        {
            get;
            set;
        }

       public int UPH2
       {
           get;
           set;
       }

        public double XAxis
        {
            get;
            set;
        }

        public double YAxis
        {
            get;
            set;
        }

        public double ZAxis
        {
            get;
            set;
        }

        public double theta
        {
            get;
            set;
        }
        public SoftwareStatus SoftwareStatus
        {
            get;
            set;
        }

        public string LoginUser
        {
            get;
            set;
        }        

        public OperationMode OperationMode
        {
            get;
            set;
        }

        public string WorkOrderVersion
        {
            get;
            set;
        }

        public string DeltaISIReader1FromTable
        {
            get;
            set;
        }
        public string DeltaISIReader2FromTable
        {
            get;
            set;
        }
        public string LDURes
        {
            get;
            set;
        }
        public string LDUSpec
        {
            get;
            set;
        }
        public double Voltage_Delta1
        {
            get; set; }

        public double Voltage_Delta2
        {
            get; set; }
        public double Volt_Ratio_Ch1
        {
            get; set; }
        public double Volt_Ratio_Ch2
        {
            get; set; }
        public double Volt_Ratio_Ch3
        {
            get; set; }
        public double Volt_Ratio_Ch4
        {
            get; set; }
        public double Volt_Ratio_Ch5
        {
            get; set; }
        public double Volt_Ratio_Ch6
        {
            get; set; }

        public double Sdet_Writer
        {
            get; set; }

        public double Hst_Sdet_Delta_Writer
        {
            get; set; }
        public double Wrbridge_Pct
        {
            get; set; }
        public double Wrbridge_Adap_Spec
        {
            get; set; }
        public double Sdet_Reader1
        {
            get; set; }
        public double Sdet_Reader2
        {
            get; set; }
        public double Delta_Polarity_R1
        {
            get; set; }

        public double Delta_Polarity_R2
        {
            get; set; }

        public string Uth_Equip_Id
        {
            get; set;
        }

        public string Uth_Time
        {
            get; set;
        }

        public double led_intercept
        {
            get;set;
        }

        public double i_threshold
        {
            get; set;
        }

        public double Sdet_i_threshold
        {
            get;
            set;
        }

        public double Delta_i_threshold
        {
            get;
            set;
        }

        public double max_v_pd
        {
            get; set;
        }

        public double ldu_turn_on_voltage
        {
            get; set;
        }
        public void ResetAllData()
        {
            this.CarrierID = "";
            this.CarrierSlot = -1;
            this.HGASerialNumber = "";
            this.HGAStatus = HGAStatus.Unknown;
            this.WorkOrder = "";
            this.ErrorMessageCode = "";
            this.SetupFileName = "";
            this.ReaderResistance = -1.0;
            this.Reader2Resistance = -1.0;
            this.DeltaISIReader1 = -1.0;
            this.DeltaISIReader2 = -1.0;
            this.WriterResistance = -1.0;
            this.rHeaterResistance = -1.0;
            this.wHeaterResistance = -1.0;
            this.TAResistance = -1.0;
            this.ShortTest = ShortTest.NoTest;
            this.ShortTestPosition = "";
            this.MicroActuatorCapacitance1 = -1.0;
            this.MicroActuatorCapacitance2 = -1.0;
            this.ThermisterTemperature = "";
            this.BiasVoltage = -1.0;
            this.BiasCurrent = -1.0;
            this.ResistanceSpec = "";
            this.CapacitanceSpec1 = "";
            this.CapacitanceSpec2 = "";
            this.MachineLocation = "";
            this.EquipmentID = "";
            this.EquipmentType = "";
            this.XAxis = 999;
            this.YAxis = 999;
            this.ZAxis = 999;
            this.theta = 999;
            this.SoftwareStatus = SoftwareStatus.Unknown;
            this.LoginUser = "";
            this.OperationMode = OperationMode.Auto;
            this.WorkOrderVersion = "Unknown";
            this.LDURes = "0";
            this.LDUSpec = "0";
        }
    }


    public class DataLogBoforeAfter
    {
        public DataLogBoforeAfter()
        {
            CarrierID = string.Empty;
            WorkOrder = string.Empty;
            WorkOrderVersion = string.Empty;
            CarrierSlot = new int[10];
            HGASerialNumber = new string[10];
            HGAStatusBefore = new char[10];
            HGAStatusAfter = new char[10];
        }

        public string CarrierID
        {
            get;
            set;
        }

        public string WorkOrder
        {
            get;
            set;
        }

        public string WorkOrderVersion
        {
            get;
            set;
        }
        public int[] CarrierSlot
        {
            get;
            set;
        }

        public string[] HGASerialNumber
        {
            get;
            set;
        }

        public char[] HGAStatusBefore
        {
            get;
            set;
        }

        public char[] HGAStatusAfter
        {
            get;
            set;
        }
    }
}
