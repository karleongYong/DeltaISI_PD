using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Utils
{
    public enum JobStates
    {
        InJob,
        OutJob
    }

    public enum FactoryList
    {
        Teparuk = 8,
        Korat = 9,
        Unknown = 0
    }

    public enum COMPortList
    {
        COM1 = 1,
        COM2 = 2,
        COM3 = 3,
        COM4 = 4,
        COM5 = 5,
        COM6 = 6,
        COM7 = 7,
        COM8 = 8,
        COM9 = 9,
        COM10 = 10,
        COM11 = 11,
        COM12 = 12,
        COM13 = 13,
        COM14 = 14,
        COM15 = 15,
        COM16 = 16,
        COM17 = 17,
        COM18 = 18,
        COM19 = 19,
        COM20 = 20,
        COM21 = 21,
        COM22 = 22,
        COM23 = 23,
        COM24 = 24,
        COM25 = 25,
        Unknown = 0
    }        

    public enum OperationMode
    {
        Auto,
        Bypass,
        DryRun,         
        Simulation
    }    

    public enum SoftwareStatus
    {
        Start,
        Pause,
        Stop,
        Unknown
    }

    public enum ShortTest
    {
        NoShort = 0,
        Short = 1,       
        NoTest = 2
    }    
    
    public enum StatusOutLevel
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Verbose = 3,
        NumLevels = 4
    }

    public enum LoggerCategory
    {
        SystemStateTransition = 1000,
        PowerDownTime = 1001,// This is written into PowerDownTime.log in overwrite mode.
        ApplicationLaunch = 1002,
        ApplicationShutdown = 1003,

        StateTransition = 1004,

        AlarmOccured = 1010,
        AlarmCleared = 1011,
        SystemInitializationStarted = 1012,
        SystemInitializationEnded = 1013,
        MessagePrompt = 1014,
        MessagePromptResponse = 1015,

        UserLogin = 1020,
        UserLogoff = 1021,

        SpecialCommands = 1100,
        FlushSeaveyorSelection = 1101,
        ProcessTimers = 1200,

        CLURecipe = 3001,
        UnloadingCarrierVisionRecipe = 3002,
        BolaTrayVisionRecipe = 3003,
        NestVisionRecipe = 3004,
        LoadingCarrierVisionRecipe = 3005,
        MotionTeachPointRecipe = 3006,

        SystemSetup = 3020,
        SystemMachineProfileCalibrationSetup = 3021,



        BolaTrayTransferMotionProfileSetup = 3026,
        TemperatureControllerSetup = 3027,

        CarrierNotProcessed = 3200,
        CarrierBeginProcessing = 3201,
        CarrierTiltReadings = 3202,
        CarrierProcessStatus = 3204,
        CarrierProcessSummary = 3205,

        LoadStation,
        //Warning message
        WarningOccured = 4010,
        WarningCleared = 4011,
    }

    public enum MessageType
    {
        Command = 1,
        Acknowledge = 2,
        Unsolicited = 3
    }

    public enum MessageID
    {
        HST_get_status = 1,
        HST_config_res_meas = 2,
        HST_config_cap_meas = 3,
        HST_config_short_detection = 4,
        HST_meas_channel_enable = 5,
        HST_hga_enable = 6,
        HST_get_conversion_board_id = 7,
        HST_get_operation_mode = 8,
        HST_start_meas = 9,
        HST_get_short_detection = 10,
        HST_get_res_results = 11,
        HST_get_cap_results = 12,
        HST_get_bias_voltages = 13,
        HST_get_results_by_hga = 14,
        HST_get_bias_by_hga = 15,
        HST_get_sensing_by_hga = 16,
        HST_calibration_enable = 17,
        HST_start_auto_calibration = 18,
        HST_save_calibration_data = 19,
        HST_get_calibration_data = 20,
        HST_manual_set_calibration = 21,
        HST_eeprom_write = 22,
        HST_eeprom_read = 23,
        HST_dac_write = 24,
        HST_dac_read = 25,
        HST_dac_output_enable = 26,
        HST_adc_write = 27,
        HST_adc_read = 28,
        HST_get_adc_voltages = 29,
        HST_set_mux = 30,
        HST_set_temp_calibration = 31,
        HST_config_temp_meas = 32,
        HST_get_temperature = 33,
        HST_get_cap_secondary_results = 34,
        HST_get_cap_reading = 35,
        HST_start_self_test = 36,
        HST_get_firmware_version = 37,
        HST_calibrate_offset = 38,
        HST_get_calibration_offset = 39,
        HST_flex_cable_calibration = 43,
        HST_get_cable_calibration_res_results = 44,
        HST_set_cable_compensation = 45,
        HST_clear_all_cable_compensation = 46,
        HST_set_short_detection_threshold = 47,
        HST_get_short_detection_threshold = 48,
        HST_set_temp1_offset = 49,
        HST_get_temp1_offset = 50,
        HST_get_cable_calibration_cap_results = 51,
        
        HST_set_precisor_cap_compensation = 52,
        HST_get_precisor_cap_compensation = 53,
        HST_save_precisor_cap_compensation = 54,

        HST_get_res_meas_configuration = 55,
        HST_get_short_detection_configuration = 56,
        HST_config_res_meas2 = 57,
        HST_get_res_meas2_configuration = 58,

        HST_set_short_detection_threshold2 = 59,
        HST_get_short_detection_threshold2 = 60,
        HST_set_cable_calibration_res_offset_ch = 61,
        HST_set_ldu_configuration = 62,
        HST_get_ldu_data_by_hga = 63,
        HST_get_ldu_voltages = 64,
        HST_set_channel_configuration = 65,
        HST_set_ch_short_det_cur_ratio = 66,
        HST_get_ch_short_det_cur_ratio = 67,
        HST_set_short_detection_vol_threshold = 68,
        HST_get_short_limits = 69,
        HST_get_short_ratio = 70,
        HST_get_short_detection_vol_threshold = 71,
        HST_get_vol_delta = 72,
        HST_get_vol_delta_threshold = 73,
        HST_get_voltage_pad = 74,
        HST_get_all_bias_voltage = 79,
        HST_get_all_sensing_voltage = 80,
        HST_swap_ch3_and_ch4 = 81,
        HST_write_config_reg = 82,    // 82
        HST_read_config_reg = 83,     // 83

        HST_get_conversion_reg_data = 84,     // 84
        HST_set_ldu_configuration_2 = 85,     //85
        HST_get_photodiode_data_by_hga = 86,  // 86
        HST_get_ldu_configuration_2,            //87
        HST_get_half_LDU_Data,
        HST_get_half_photodiode_data,


        HST_unsolicited_status = 255
    }

    public enum PrecisorNestXAxis
    {
        Unknown,
        MovingToInputStation,
        MovingToParkedPosition,
        MovingToPrecisorStation,
        MovingToOutputStation,
        Parked,
        AtInputStation,
        AtPrecisorStation,
        AtOutputStation
    }

    public enum InputEEZAxis
    {     
        Unknown,
        Pick,
        Place,
        Parked,
        DycemCleaning
    }

    public enum TestProbeZAxis
    {
        Unknown,
        Test,
        Parked
    }

    public enum OutputEEZAxis
    {
        Unknown,
        Pick,
        Place,
        Parked,
        DycemCleaning
    }

    public enum GUIPage
    {        
        MeasurementTestMainOperationPage = 1,        
        MeasurementTestConfigurationSetupPage = 2,       
        MeasurementTestPCBACalibrationTabPage = 3,
        MeasurementTestCableCalibrationTabPage = 4,
        FunctionalTest = 5
    }

    public enum ShortDetection
    {
        NoTest = 0,
        Open = 1,
        Short = 2
    }    

    public enum CommandStatus
    {
        Ready = 0,
        Busy = 1,
        Error = 2
    }

    public enum HGAStatus
    {
        Unknown,       
        NoHGAPresent,
        HGAPresent,                
        Untested,
        TestedPass,
        TestedFail,
    }

    public enum LDUFailureType
    {
        Unknown,
        OpenShort,
        Spec
    }

    public enum ERROR_MESSAGE_CODE
    {
        Unknown,
        RCS,        //11 pad, fail reader1 short
        RCO,        //11 pad, fail reader1 open
        CS2,        //11 pad, fail reader2 short
        CO2,        //11 pad, fail reader2 open
        WRS,        //11 pad, fail writer short
        WRO,        //11 pad, fail writer open
        HRS,
        HRO,        //9 pad, fail open heater
        H2S,        //11 pad, fail heater2 short
        H2O,        //11 pad, fail heater2 open
        TAS,        //11 pad, fail ta short
        TAO,        //11 pad, fail ta open,
        CRDR,       //11 pad fail spec
        CWRR,       //11 pad fail spec
        CHTR,       //11 pad fail spec
        CHT2,       //11 pad fail spec
        CTAR,       //11 pad fail ta out spec
        CRDL,       //9 common, fail ISI data
        ADJST,
        R2R1ST,      //11 pad
        R1HRST,      //11 pad
        HRH2ST,      //11 pad
        CISL,       //11 pad
        TAR2ST,      //11 pad
        R1ST,
        R2ST,
        WST,
        WHST,
        RHST,
        TAST,
        PZT1S,       //11 pad
        PZT1O,       //11 pad
        PZT2S,       //11 pad
        PZT2O,       //11 pad

        HWHRS,      //9 pad, fail short Hw Hr
        HRR1MS,     //9 pad, fail short Hr R-
        R1MR1PS,    //9 pad, fail short R- R
        R1PTA2S,    //9 pad, fail short R Ta2
        TA2TA1S,    //9 pad, fail short Ta2 Ta1
        TA1GS,      //9 pad, fail short Ta1 G
        GNWPS,      //9 pad, fail short G W
        WPWMS,      //9 pad, fail short W W-
        HWO,        //9 pad, fail open Hw
        R1PR1MO,    //9 pad, fail open R- R
        TA2TA1O,    //9 pad, fail open Ta2 Ta1
        GNO,        //9 pad, fail open G
        WPWMO,      //9 pad, fail open W W-
        PZTO,       //9 pad, fail open PZT
        PZTS,       //9 pad, fail open PZTS
        GERROR,     //9 pad, fail open 
        GOOD,
        ONDISK,
        WRBRIDGE,
        STFAMILY,
        FAILGETSORT,
        X49,
        LEDINTCFAIL,
        LDURESFAIL,
        VPDFAIL,
        ITHRESFAIL,
        DELTAITHRESFAIL,
        R2DELTA,
        R1DELTA,
        WRFAIL,      // Writer bridge lower than 0.5 ohm 5-feb-2020
        FORCE_A,     // Mar-2020 SLDR_BIN force Error code
        FORCE_E,
        FORCE_V,
        FORCE_0,
    }

    public enum RFIDInfoVerify
    {
        Unknown,
        Matched,
        NotMatched
    }

    public enum IsCarrierEmpty
    {
        Unknown,
        Empty,
        NotEmpty
    }

    public enum HGAProductTailType
    {
        Unknown,
        LongTail,
        ShortTail
    }

    public enum HGAProductTabType
    {
        Unknown,
        Up,
        Down
    }

    public enum EnumTestProbeType
    {
        Unknown,
        RoseWood,
        V11
    }

    public enum RFIDUpdateOption
    {
        DoNotUpdate,
        UpdateALL,
        UpdateOnlyHGAStatus,
        UpdateOnlyProcessStep
    }

    public enum CarrierLocation
    {
        InputTurnStation,
        InputStation,
        BufferStation,
        OutputStation,
        OutputTurnStation
    }

    public enum CameraLocation
    {
        InputStation,
        PrecisorStation,
        OutputStation
    }

    public enum AutomationSystem
    {
        ImmediateStop = 1000000,
        NoStop = 2000000,
    }

    public enum Process
    {
        InputProcess = 100000,
        TestingProcess = 200000,
        OutputProcess = 300000,
        SafetyProcess = 400000,
        EnvironmentProcess = 500000,
    }

    public enum Location
    {
        InputConveyor = 1000,
        InputTurnStation = 2000,
        InputDetectionCamera = 3000,
        InputRFID = 4000,
        InputLifter = 5000,
        InputCarrierScrewDriver = 6000,
        InputHandler = 7000,
        InputEndEffector = 8000,
        BufferConveyor = 9000,
        PrecisorNest = 10000,
        TestProbeHandler = 11000,
        TestProbeFixture = 12000,
        TestElectronics = 13000,
        AlignmentCamera = 14000,
        OutputDetectionCamera = 15000,
        OutputRFID = 16000,
        OutputLifter = 17000,
        OutputCarrierScrewdriver = 18000,
        OutputHandler = 19000,
        OutputEndEffector = 20000,
        OutputTurnStation = 21000,
        OutputConveyor = 22000,
        EMOButton = 23000,
        AutomationStartStopButton = 24000,
        SafetyController = 25000,
        GroundMaster = 26000,
        GroundMonitor = 27000,
        Door = 28000,
        LCRMeter = 29000,
        NPaqMR = 30000,
        LightController = 31000,
        ULPAFFU = 32000,
        TemperatureProbe1 = 33000,
        TemperatureProbe2 = 34000,
        TemperatureProbe3 = 35000,
        ExhaustFan = 36000,
        IPC = 37000,
        InputBoatStopper = 38000,
        OutputBoatStopper = 39000,
        Flattener = 40000,
        PrecisorNestTabSelector1 = 41000,
        PrecisorNestTabSelector2 = 42000,
        PrecisorNestTabSelector3 = 43000,
        PrecisorNestTabSelector4 = 44000,
        PrecisorNestTabSelector5 = 45000,
        PrecisorNestTabSelector6 = 46000,
        PrecisorNestTabSelector7 = 47000,
        FRL = 48000,
        AerotechController = 49000
    }

    /// <summary>
    /// All HST machine errors defination
    /// </summary>
    public enum HSTErrors
    {
        // Process errors 1 ~ 900            

        // InputProcess InputConveyor
//        InputStationSensorNotOnError = AutomationSystem.ImmediateStop + Process.InputProcess + Location.InputConveyor + 1,
//        InputStationSensorNotOffError = AutomationSystem.ImmediateStop + Process.InputProcess + Location.InputConveyor + 2,
        InputStationInPositionNotOnError = AutomationSystem.NoStop + Process.InputProcess + Location.InputConveyor + 1,
        InputStationInPositionNotOffError = AutomationSystem.NoStop + Process.InputProcess + Location.InputConveyor + 2,

        // Input Process InputTurnStation
        InputTurnStationRotaryRotateClockwiseError = AutomationSystem.NoStop + Process.InputProcess + Location.InputTurnStation + 1,
        InputTurnStationRotaryRotateCounterClockwiseError = AutomationSystem.NoStop + Process.InputProcess + Location.InputTurnStation + 2,
        InputTurnStationEntranceClearNotOnError = AutomationSystem.NoStop + Process.InputProcess + Location.InputTurnStation + 3,
//        InputTurnStationSensorNotOnError = AutomationSystem.ImmediateStop + Process.InputProcess + Location.InputTurnStation + 4,
//        InputTurnStationSensorNotOffError = AutomationSystem.ImmediateStop + Process.InputProcess + Location.InputTurnStation + 5,
        InputTurnStationInPositionNotOnError = AutomationSystem.NoStop + Process.InputProcess + Location.InputTurnStation + 4,
        InputTurnStationInPositionNotOffError = AutomationSystem.NoStop + Process.InputProcess + Location.InputTurnStation + 5,
        InputTurnStationExitClearNotOnError = AutomationSystem.NoStop + Process.InputProcess + Location.InputTurnStation + 6,

        // Input Process InputDetectionCamera
        InputDetectionCameraError = AutomationSystem.NoStop + Process.InputProcess + Location.InputDetectionCamera + 1,
        InputDetectionCameraCarrierLoadedInWrongDirection = AutomationSystem.NoStop + Process.InputProcess + Location.InputDetectionCamera + 2,

        // Input Process InputRFID
        InputRFIDReadFailed = AutomationSystem.NoStop + Process.InputProcess + Location.InputRFID + 1,
        InputRFIDReadFoundAllHGAsFailed = AutomationSystem.NoStop + Process.InputProcess + Location.InputRFID + 2,
        InputRFIDReadFoundAHGAsMissingSerialNo = AutomationSystem.NoStop + Process.InputProcess + Location.InputRFID + 3,
        InputRFIDReadFoundProductTypeMismatch = AutomationSystem.NoStop + Process.InputProcess + Location.InputRFID + 4,
        InputRFIDReadWorkorderFailed = AutomationSystem.NoStop + Process.InputProcess + Location.InputRFID + 5,

        // Input Process InputLifter
        InputStationLifterExtendError = AutomationSystem.NoStop + Process.InputProcess + Location.InputLifter + 1,
        InputStationLifterRetractError = AutomationSystem.NoStop + Process.InputProcess + Location.InputLifter + 2,

        // Input Process InputBoatStopper
        InputStationStopperExtendError = AutomationSystem.NoStop + Process.InputProcess + Location.InputBoatStopper + 1,
        InputStationStopperRetractError = AutomationSystem.NoStop + Process.InputProcess + Location.InputBoatStopper + 2,

        // Input Process InputCarrierScrewDriver
        InputStationCarrierScrewDriverExtendError = AutomationSystem.NoStop + Process.InputProcess + Location.InputCarrierScrewDriver + 1,
        InputStationCarrierScrewDriverRetractError = AutomationSystem.NoStop + Process.InputProcess + Location.InputCarrierScrewDriver + 2,
        InputStationCarrierScrewDriverRotateClockwiseError = AutomationSystem.NoStop + Process.InputProcess + Location.InputCarrierScrewDriver + 3,
        InputStationCarrierScrewDriverRotateCounterClockwiseError = AutomationSystem.NoStop + Process.InputProcess + Location.InputCarrierScrewDriver + 4,

        // Input Process InputHandler
        InputHandlerZAxisHomeError = AutomationSystem.NoStop + Process.InputProcess + Location.InputHandler + 1,
        InputHandlerZAxisMoveDownError = AutomationSystem.NoStop + Process.InputProcess + Location.InputHandler + 2,
        InputHandlerZAxisMoveUpError = AutomationSystem.NoStop + Process.InputProcess + Location.InputHandler + 3,
        InputHandlerZAxisReadPositionError = AutomationSystem.NoStop + Process.InputProcess + Location.InputHandler + 4,

        // Input Process InputEndEffector
        InputEndEffectorVacuumPressureSensor1NotOnError = AutomationSystem.NoStop + Process.InputProcess + Location.InputEndEffector + 1,
        InputEndEffectorVacuumPressureSensor1NotOffError = AutomationSystem.NoStop + Process.InputProcess + Location.InputEndEffector + 2,
        InputEndEffectorVacuumPressureSensor2NotOnError = AutomationSystem.NoStop + Process.InputProcess + Location.InputEndEffector + 3,
        InputEndEffectorVacuumPressureSensor2NotOffError = AutomationSystem.NoStop + Process.InputProcess + Location.InputEndEffector + 4,

        // Test Process Buffer Station
        BufferStationInPositionNotOnError = AutomationSystem.NoStop + Process.TestingProcess + Location.BufferConveyor + 1,
        BufferStationInPositionNotOffError = AutomationSystem.NoStop + Process.TestingProcess + Location.BufferConveyor + 2,

        // Testing Process PrecisorNest
        PrecisorNestXAxisHomeError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 1,
        PrecisorNestXAxisMoveLeftError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 2,
        PrecisorNestXAxisMoveRightError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 3,
        PrecisorNestXAxisReadPositionError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 4,
        PrecisorNestYAxisHomeError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 5,
        PrecisorNestYAxisMoveFrontError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 6,
        PrecisorNestYAxisMoveBackError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 7,
        PrecisorNestYAxisReadPositionError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 8,
        PrecisorNestThetaAxisHomeError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 9,
        PrecisorNestThetaAxisRotateClockwiseError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 10,
        PrecisorNestThetaAxisRotateCounterClockwiseError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 11,
        PrecisorNestThetaAxisReadPositionError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 12,
        PrecisorNestVacuumPressureSensor1NotOnError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 13,
        PrecisorNestVacuumPressureSensor1NotOffError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 14,
        PrecisorNestVacuumPressureSensor2NotOnError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 15,
        PrecisorNestVacuumPressureSensor2NotOffError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 16,
        PrecisorNestVacuumPressureSensor3NotOnError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 17,
        PrecisorNestVacuumPressureSensor3NotOffError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 18,
        PrecisorNestVacuumPressureSensor4NotOnError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 19,
        PrecisorNestVacuumPressureSensor4NotOffError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 20,
        PrecisorNestVacuumPressureSensor5NotOnError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 21,
        PrecisorNestVacuumPressureSensor5NotOffError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 22,
        PrecisorNestVacuumPressureSensor6NotOnError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 23,
        PrecisorNestVacuumPressureSensor6NotOffError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 24,
        PrecisorNestVacuumPressureSensor7NotOnError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 25,
        PrecisorNestVacuumPressureSensor7NotOffError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 26,
        PrecisorNestXAxisMoveToTestStationError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 27,
        PrecisorNestXAxisMoveToParkPositionError = AutomationSystem.NoStop + Process.TestingProcess + Location.PrecisorNest + 28,

        // Testing Process TestProbeHandler
        TestProbeHandlerZAxisHomeError = AutomationSystem.NoStop + Process.TestingProcess + Location.TestProbeHandler + 1,
        TestProbeHandlerZAxisMoveDownError = AutomationSystem.NoStop + Process.TestingProcess + Location.TestProbeHandler + 2,
        TestProbeHandlerZAxisMoveUpError = AutomationSystem.NoStop + Process.TestingProcess + Location.TestProbeHandler + 3,
        TestProbeHandlerZAxisReadPositionError = AutomationSystem.NoStop + Process.TestingProcess + Location.TestProbeHandler + 4,

        // Testing Process TestProbeFixture
        TestProbeFixtureCalibrationError = AutomationSystem.NoStop + Process.TestingProcess + Location.TestProbeFixture + 1,

        // Testing Process TestElectronics
        TestElectronicsMeasurementError = AutomationSystem.NoStop + Process.TestingProcess + Location.TestElectronics + 1,
        TestElectronicsWrongProductType = AutomationSystem.NoStop + Process.TestingProcess + Location.TestElectronics + 2,
        TestElectronicsOutdatedMeasurementTestConfiguration = AutomationSystem.NoStop + Process.TestingProcess + Location.TestElectronics + 3,
        TestElectronicsSerialPortCommunicationError = AutomationSystem.NoStop + Process.TestingProcess + Location.TestElectronics + 4,
        TestElectronicsTICErrorDetection = AutomationSystem.NoStop + Process.TestingProcess + Location.TestElectronics + 5,
        TestElectronicsMeasurementTimeout = AutomationSystem.NoStop + Process.TestingProcess + Location.TestElectronics + 6,
        TestElectronicsCRDLErrorDetection = AutomationSystem.NoStop + Process.TestingProcess + Location.TestElectronics + 7,
        TestElectronecsGetputErrorDetection = AutomationSystem.NoStop + Process.TestingProcess + Location.TestElectronics + 8,
        TestElectronecsGetputErrorDetection2 = AutomationSystem.NoStop + Process.TestingProcess + Location.TestElectronics + 9,
        TestElectronicsRecipeChangedErrorDetection = AutomationSystem.NoStop + Process.TestingProcess + Location.TestElectronics + 10,
        TestElectronicsWriterBridgeDetection = AutomationSystem.NoStop + Process.TestingProcess + Location.TestElectronics + 11,

        // Testing Process AlignmentCamera
        AlignmentCameraError = AutomationSystem.NoStop + Process.TestingProcess + Location.AlignmentCamera + 1,

        // Output Process OutputDetectionCamera
        OutputDetectionCameraError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputDetectionCamera + 1,
        OutputDetectionCameraHGANotPickedUpAtInputStationError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputDetectionCamera + 2,
        OutputDetectionCameraFailedToPlaceBackAllHGAsAfterMeasurementError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputDetectionCamera + 3,

        // Output Process OutputRFID
        OutputRFIDReadError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputRFID + 1,
        OutputRFIDWriteError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputRFID + 2,

        // Output Process OutputLifter
        OutputStationLifterExtendError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputLifter + 1,
        OutputStationLifterRetractError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputLifter + 2,
        OutputStationStopperExtendError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputLifter + 3,
        OutputStationStopperRetractError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputLifter + 4,

        // Output Process OutputCarrierScrewdriver
        OutputStationCarrierScrewDriverExtendError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputCarrierScrewdriver + 1,
        OutputStationCarrierScrewDriverRetractError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputCarrierScrewdriver + 2,
        OutputStationCarrierScrewDriverRotateClockwiseError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputCarrierScrewdriver + 3,
        OutputStationCarrierScrewDriverRotateCounterClockwiseError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputCarrierScrewdriver + 4,

        // Output Process OutputHandler
        OutputHandlerZAxisHomeError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputHandler + 1,
        OutputHandlerZAxisMoveDownError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputHandler + 2,
        OutputHandlerZAxisMoveUpError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputHandler + 3,
        OutputHandlerZAxisReadPositionError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputHandler + 4,

        // Output Process OutputEndEffector
        OutputEndEffectorVacuumPressureSensor1NotOnError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputEndEffector + 1,
        OutputEndEffectorVacuumPressureSensor1NotOffError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputEndEffector + 2,
        OutputEndEffectorVacuumPressureSensor2NotOnError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputEndEffector + 3,
        OutputEndEffectorVacuumPressureSensor2NotOffError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputEndEffector + 4,

        // Output Process OutputConveyor
//        OutputStationSensorNotOnError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputConveyor + 1,
//        OutputStationSensorNotOffError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputConveyor + 2,
        OutputStationInPositionNotOnError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputConveyor + 1,
        OutputStationInPositionNotOffError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputConveyor + 2,
        OutputConveyorCongestionError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputConveyor + 3,

        // Output Process OutputTurnStation
        OutputTurnStationRotaryRotateClockwiseError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputTurnStation + 1,
        OutputTurnStationRotaryRotateCounterClockwiseError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputTurnStation + 2,
        OutputTurnStationEntranceClearNotOnError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputTurnStation + 3,
//        OutputTurnStationSensorNotOnError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputTurnStation + 4,
//        OutputTurnStationSensorNotOffError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputTurnStation + 5,
        OutputTurnStationInPositionNotOnError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputTurnStation + 4,
        OutputTurnStationInPositionNotOffError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputTurnStation + 5,
        OutputTurnStationExitClearNotOnError = AutomationSystem.NoStop + Process.OutputProcess + Location.OutputTurnStation + 6,      

        // Safety Process Door
        DoorNotLockError = AutomationSystem.NoStop + Process.SafetyProcess + Location.Door + 1,
        EMOTriggeredError = AutomationSystem.NoStop + Process.SafetyProcess + Location.Door + 2,
        GroundMasterError = AutomationSystem.NoStop + Process.SafetyProcess + Location.Door + 3,

        // Safety Process SafetyController
        SafetyControllerPresentError = AutomationSystem.NoStop + Process.SafetyProcess + Location.SafetyController + 1,

        TICTriggeringError = AutomationSystem.NoStop + Process.TestingProcess + Location.SafetyController + 1,
        PerformanceTriggeringError = AutomationSystem.NoStop + Process.TestingProcess + Location.SafetyController + 1,
        ProbeFunctionalTestTriggeringError = AutomationSystem.NoStop + Process.TestingProcess + Location.SafetyController + 1,
        ErrorCodeTriggeringError = AutomationSystem.NoStop + Process.TestingProcess + Location.SafetyController + 1,
        SamplingTriggeringError = AutomationSystem.NoStop + Process.TestingProcess + Location.SafetyController + 1,
        ANCTriggeringError = AutomationSystem.NoStop + Process.SafetyProcess + Location.SafetyController + 1,
        MotionAxitError = AutomationSystem.NoStop + Process.TestingProcess + Location.SafetyController + 1
    }

    public enum I_ThresholdCalculationMethod
    {
        Ymxc = 0,
        SecondDerivative = 1,
        Gompertz = 2,
        Unknown = 3
    }

    public enum GompertzCalculationMethod
    {
        Random = 0,
        Fix = 1
    }
}
