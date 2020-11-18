using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchTestsTool.Utils
{
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
        Unknown = 0
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
        HST_get_product_id = 7,
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
        HST_unsolicited_status = 255
    }

    public enum GUIPage
    {
        CommonCaptionBanner = 1,
        BenchTestsTabPage = 2,
        FunctionalTestsTabPage = 3,
        ConfigurationSetupTabPage = 4,
        PCBACalibrationTabPage = 5,
        CableCalibrationTabPage = 6,
        PrecisorCompensationTabPage = 7,
        DebugTabPage = 8,
        ManufacturingTestTabPage = 9

    }

    public enum ShortDetection
    {
        NoTest = 0,
        Open = 1,
        Short = 2
    }

    public enum CalibrationType
    {
        ResistanceCalibration = 0,
        CapacitanceCalibration = 1
    }

    public enum CommandStatus
    {
        Ready = 0,
        Busy = 1,
        Error = 2
    }
}
