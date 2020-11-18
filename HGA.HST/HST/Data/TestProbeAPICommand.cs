using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.Data
{
    public class TestProbeAPICommand
    {
        // 1
        public const byte HST_get_status_Message_ID = (byte)MessageID.HST_get_status;
        public const string HST_get_status_Message_Name = "HST_get_status";
        public const byte HST_get_status_Message_Size = 3;

        // 2
        public const byte HST_config_res_meas_Message_ID = (byte)MessageID.HST_config_res_meas;
        public const string HST_config_res_meas_Message_Name = "HST_config_res_meas";
        public const byte HST_config_res_meas_Message_Size = 16;

        // 3
        public const byte HST_config_cap_meas_Message_ID = (byte)MessageID.HST_config_cap_meas;
        public const string HST_config_cap_meas_Message_Name = "HST_config_cap_meas";
        public const byte HST_config_cap_meas_Message_Size = 11;

        // 4
        public const byte HST_config_short_detection_Message_ID = (byte)MessageID.HST_config_short_detection;
        public const string HST_config_short_detection_Message_Name = "HST_config_short_detection";
        public const byte HST_config_short_detection_Message_Size = 15;

        // 5
        public const byte HST_meas_channel_enable_Message_ID = (byte)MessageID.HST_meas_channel_enable;
        public const string HST_meas_channel_enable_Message_Name = "HST_meas_channel_enable";
        public const byte HST_meas_channel_enable_Message_Size = 11;

        // 6
        public const byte HST_hga_enable_Message_ID = (byte)MessageID.HST_hga_enable;
        public const string HST_hga_enable_Message_Name = "HST_hga_enable";
        public const byte HST_hga_enable_Message_Size = 13;

        // 7
        public const byte HST_get_conversion_board_id_Message_ID = (byte)MessageID.HST_get_conversion_board_id;
        public const string HST_get_conversion_board_id_Message_Name = "HST_get_conversion_board_id";
        public const byte HST_get_conversion_board_id_Message_Size = 3;

        // 8
        public const byte HST_get_operation_mode_Message_ID = (byte)MessageID.HST_get_operation_mode;
        public const string HST_get_operation_mode_Message_Name = "HST_get_operation_mode";
        public const byte HST_get_operation_mode_Message_Size = 3;

        // 9
        public const byte HST_start_meas_Message_ID = (byte)MessageID.HST_start_meas;
        public const string HST_start_meas_Message_Name = "HST_start_meas";
        public const byte HST_start_meas_Message_Size = 4;

        // 10
        public const byte HST_get_short_detection_Message_ID = (byte)MessageID.HST_get_short_detection;
        public const string HST_get_short_detection_Message_Name = "HST_get_short_detection";
        public const byte HST_get_short_detection_Message_Size = 3;

        // 11
        public const byte HST_get_res_results_Message_ID = (byte)MessageID.HST_get_res_results;
        public const string HST_get_res_results_Message_Name = "HST_get_res_results";
        public const byte HST_get_res_results_Message_Size = 3;

        // 12
        public const byte HST_get_cap_results_Message_ID = (byte)MessageID.HST_get_cap_results;
        public const string HST_get_cap_results_Message_Name = "HST_get_cap_results";
        public const byte HST_get_cap_results_Message_Size = 3;

        // 13
        public const byte HST_get_bias_voltages_Message_ID = (byte)MessageID.HST_get_bias_voltages;
        public const string HST_get_bias_voltages_Message_Name = "HST_get_bias_voltages";
        public const byte HST_get_bias_voltages_Message_Size = 3;

        // 14
        public const byte HST_get_results_by_hga_Message_ID = (byte)MessageID.HST_get_results_by_hga;
        public const string HST_get_results_by_hga_Message_Name = "HST_get_results_by_hga";
        public const byte HST_get_results_by_hga_Message_Size = 5;

        // 15
        public const byte HST_get_bias_by_hga_Message_ID = (byte)MessageID.HST_get_bias_by_hga;
        public const string HST_get_bias_by_hga_Message_Name = "HST_get_bias_by_hga";
        public const byte HST_get_bias_by_hga_Message_Size = 4;

        // 16
        public const byte HST_get_sensing_by_hga_Message_ID = (byte)MessageID.HST_get_sensing_by_hga;
        public const string HST_get_sensing_by_hga_Message_Name = "HST_get_sensing_by_hga";
        public const byte HST_get_sensing_by_hga_Message_Size = 4;

        // 17
        public const byte HST_calibration_enable_Message_ID = (byte)MessageID.HST_calibration_enable;
        public const string HST_calibration_enable_Message_Name = "HST_calibration_enable";
        public const byte HST_calibration_enable_Message_Size = 4;

        // 18
        public const byte HST_start_auto_calibration_Message_ID = (byte)MessageID.HST_start_auto_calibration;
        public const string HST_start_auto_calibration_Message_Name = "HST_start_auto_calibration";
        public const byte HST_start_auto_calibration_Message_Size = 3;

        // 19
        public const byte HST_save_calibration_data_Message_ID = (byte)MessageID.HST_save_calibration_data;
        public const string HST_save_calibration_data_Message_Name = "HST_save_calibration_data";
        public const byte HST_save_calibration_data_Message_Size = 3;

        // 20
        public const byte HST_get_calibration_data_Message_ID = (byte)MessageID.HST_get_calibration_data;
        public const string HST_get_calibration_data_Message_Name = "HST_get_calibration_data";
        public const byte HST_get_calibration_data_Message_Size = 3;

        // 21
        public const byte HST_manual_set_calibration_Message_ID = (byte)MessageID.HST_manual_set_calibration;
        public const string HST_manual_set_calibration_Message_Name = "HST_manual_set_calibration";
        public const byte HST_manual_set_calibration_Message_Size = 10;

        // 22
        public const byte HST_eeprom_write_Message_ID = (byte)MessageID.HST_eeprom_write;
        public const string HST_eeprom_write_Message_Name = "HST_eeprom_write";
        public const byte HST_eeprom_write_Message_Size = 7;

        // 23
        public const byte HST_eeprom_read_Message_ID = (byte)MessageID.HST_eeprom_read;
        public const string HST_eeprom_read_Message_Name = "HST_eeprom_read";
        public const byte HST_eeprom_read_Message_Size = 6;

        // 24
        public const byte HST_dac_write_Message_ID = (byte)MessageID.HST_dac_write;
        public const string HST_dac_write_Message_Name = "HST_dac_write";
        public const byte HST_dac_write_Message_Size = 6;

        // 25
        public const byte HST_dac_read_Message_ID = (byte)MessageID.HST_dac_read;
        public const string HST_dac_read_Message_Name = "HST_dac_read";
        public const byte HST_dac_read_Message_Size = 4;

        // 26
        public const byte HST_dac_output_enable_Message_ID = (byte)MessageID.HST_dac_output_enable;
        public const string HST_dac_output_enable_Message_Name = "HST_dac_output_enable";
        public const byte HST_dac_output_enable_Message_Size = 4;

        // 27
        public const byte HST_adc_write_Message_ID = (byte)MessageID.HST_adc_write;
        public const string HST_adc_write_Message_Name = "HST_adc_write";
        public const byte HST_adc_write_Message_Size = 8;

        // 28
        public const byte HST_adc_read_Message_ID = (byte)MessageID.HST_adc_read;
        public const string HST_adc_read_Message_Name = "HST_adc_read";
        public const byte HST_adc_read_Message_Size = 5;

        // 29
        public const byte HST_get_adc_voltages_Message_ID = (byte)MessageID.HST_get_adc_voltages;
        public const string HST_get_adc_voltages_Message_Name = "HST_get_adc_voltages";
        public const byte HST_get_adc_voltages_Message_Size = 5;

        // 30
        public const byte HST_set_mux_Message_ID = (byte)MessageID.HST_set_mux;
        public const string HST_set_mux_Message_Name = "HST_set_mux";
        public const byte HST_set_mux_Message_Size = 5;

        // 31
        public const byte HST_set_temp_calibration_Message_ID = (byte)MessageID.HST_set_temp_calibration;
        public const string HST_set_temp_calibration_Message_Name = "HST_set_temp_calibration";
        public const byte HST_set_temp_calibration_Message_Size = 5;

        // 32
        public const byte HST_config_temp_meas_Message_ID = (byte)MessageID.HST_config_temp_meas;
        public const string HST_config_temp_meas_Message_Name = "HST_config_temp_meas";
        public const byte HST_config_temp_meas_Message_Size = 4;

        // 33
        public const byte HST_get_temperature_Message_ID = (byte)MessageID.HST_get_temperature;
        public const string HST_get_temperature_Message_Name = "HST_get_temperature";
        public const byte HST_get_temperature_Message_Size = 3;

        // 34
        public const byte HST_get_cap_secondary_results_Message_ID = (byte)MessageID.HST_get_cap_secondary_results;
        public const string HST_get_cap_secondary_results_Message_Name = "HST_get_cap_secondary_results";
        public const byte HST_get_cap_secondary_results_Message_Size = 3;

        // 35
        public const byte HST_get_cap_reading_Message_ID = (byte)MessageID.HST_get_cap_reading;
        public const string HST_get_cap_reading_Message_Name = "HST_get_cap_reading";
        public const byte HST_get_cap_reading_Message_Size = 3;

        // 36
        public const byte HST_start_self_test_Message_ID = (byte)MessageID.HST_start_self_test;
        public const string HST_start_self_test_Message_Name = "HST_start_self_test";
        public const byte HST_start_self_test_Message_Size = 3;

        // 37
        public const byte HST_get_firmware_version_Message_ID = (byte)MessageID.HST_get_firmware_version;
        public const string HST_get_firmware_version_Message_Name = "HST_get_firmware_version";
        public const byte HST_get_firmware_version_Message_Size = 3;

        // 38
        public const byte HST_calibrate_offset_Message_ID = (byte)MessageID.HST_calibrate_offset;
        public const string HST_calibrate_offset_Message_Name = "HST_calibrate_offset";
        public const byte HST_calibrate_offset_Message_Size = 3;

        // 39
        public const byte HST_get_calibration_offset_Message_ID = (byte)MessageID.HST_get_calibration_offset;
        public const string HST_get_calibration_offset_Message_Name = "HST_get_calibration_offset";
        public const byte HST_get_calibration_offset_Message_Size = 3;

        // 43
        public const byte HST_flex_cable_calibration_Message_ID = (byte)MessageID.HST_flex_cable_calibration;
        public const string HST_flex_cable_calibration_Message_Name = "HST_flex_cable_calibration";
        public const byte HST_flex_cable_calibration_Message_Size = 4;

        // 44
        public const byte HST_get_cable_calibration_res_results_Message_ID = (byte)MessageID.HST_get_cable_calibration_res_results;
        public const string HST_get_cable_calibration_res_results_Message_Name = "HST_get_cable_calibration_res_results";
        public const byte HST_get_cable_calibration_res_results_Message_Size = 4;

        // 45
        public const byte HST_set_cable_compensation_Message_ID = (byte)MessageID.HST_set_cable_compensation;
        public const string HST_set_cable_compensation_Message_Name = "HST_set_cable_compensation";
        public const byte HST_set_cable_compensation_Message_Size = 11;

        // 46
        public const byte HST_clear_all_cable_compensation_Message_ID = (byte)MessageID.HST_clear_all_cable_compensation;
        public const string HST_clear_all_cable_compensation_Message_Name = "HST_clear_all_cable_compensation";
        public const byte HST_clear_all_cable_compensation_Message_Size = 3;

        // 47
        public const byte HST_set_short_detection_threshold_Message_ID = (byte)MessageID.HST_set_short_detection_threshold;
        public const string HST_set_short_detection_threshold_Message_Name = "HST_set_short_detection_threshold";
        public const byte HST_set_short_detection_threshold_Message_Size = 7;

        // 48
        public const byte HST_get_short_detection_threshold_Message_ID = (byte)MessageID.HST_get_short_detection_threshold;
        public const string HST_get_short_detection_threshold_Message_Name = "HST_get_short_detection_threshold";
        public const byte HST_get_short_detection_threshold_Message_Size = 3;

        // 49
        public const byte HST_set_temp1_offset_Message_ID = (byte)MessageID.HST_set_temp1_offset;
        public const string HST_set_temp1_offset_Message_Name = "HST_set_temp1_offset";
        public const byte HST_set_temp1_offset_Message_Size = 4;

        // 50
        public const byte HST_get_temp1_offset_Message_ID = (byte)MessageID.HST_get_temp1_offset;
        public const string HST_get_temp1_offset_Message_Name = "HST_get_temp1_offset";
        public const byte HST_get_temp1_offset_Message_Size = 3;

        // 51
        public const byte HST_get_cable_calibration_cap_results_Message_ID = (byte)MessageID.HST_get_cable_calibration_cap_results;
        public const string HST_get_cable_calibration_cap_results_Message_Name = "HST_get_cable_calibration_cap_results";
        public const byte HST_get_cable_calibration_cap_results_Message_Size = 4;

        // 52
        public const byte HST_set_precisor_cap_compensation_Message_ID = (byte)MessageID.HST_set_precisor_cap_compensation;
        public const string HST_set_precisor_cap_compensation_Message_Name = "HST_set_precisor_cap_compensation";
        public const byte HST_set_precisor_cap_compensation_Message_Size = 85;

        // 53
        public const byte HST_get_precisor_cap_compensation_Message_ID = (byte)MessageID.HST_get_precisor_cap_compensation;
        public const byte HST_get_precisor_cap_compensation_Message_Size = 4;

        // 54
        public const byte HST_save_precisor_cap_compensation_Message_ID = (byte)MessageID.HST_save_precisor_cap_compensation;
        public const string HST_save_precisor_cap_compensation_Message_Name = "HST_save_precisor_cap_compensation";
        public const byte HST_save_precisor_cap_compensation_Message_Size = 3;

        // 55
        public const byte HST_get_res_meas_configuration_Message_ID = (byte)MessageID.HST_get_res_meas_configuration;
        public const string HST_get_res_meas_configuration_Message_Name = "HST_get_res_meas_configuration";
        public const byte HST_get_res_meas_configuration_Message_Size = 3;

        // 56
        public const byte HST_get_short_detection_configuration_Message_ID = (byte)MessageID.HST_get_short_detection_configuration;
        public const string HST_get_short_detection_configuration_Message_Name = "HST_get_short_detection_configuration";
        public const byte HST_get_short_detection_configuration_Message_Size = 3;

        // 57
        public const byte HST_config_res_meas2_Message_ID = (byte)MessageID.HST_config_res_meas2;
        public const string HST_config_res_meas2_Message_Name = "HST_config_res_meas2";
        public const byte HST_config_res_meas2_Message_Size = 18;

        // 58
        public const byte HST_get_res_meas2_configuration_Message_ID = (byte)MessageID.HST_get_res_meas2_configuration;
        public const string HST_get_res_meas2_configuration_Message_Name = "HST_get_res_meas2_configuration";
        public const byte HST_get_res_meas2_configuration_Message_Size = 3;


        //07-02-2019

        // 59
        public const byte HST_set_short_detection_threshold2_Message_ID = (byte)MessageID.HST_set_short_detection_threshold2;
        public const string HST_set_short_detection_threshold2_Message_Name = "HST_set_short_detection_threshold2";
        public const byte HST_set_short_detection_threshold2_Message_Size = 28;

        // 60
        public const byte HST_get_short_detection_threshold2_Message_ID = (byte)MessageID.HST_get_short_detection_threshold2;
        public const string HST_get_short_detection_threshold2_Message_Name = "HST_get_short_detection_threshold2";
        public const byte HST_get_short_detection_threshold2_Message_Size = 3;

        // 61
        public const byte HST_set_cable_calibration_res_offset_ch_Message_ID = (byte)MessageID.HST_set_cable_calibration_res_offset_ch;
        public const string HST_set_cable_calibration_res_offset_ch_Message_Name = "HST_set_cable_calibration_res_offset_ch";
        public const byte HST_set_cable_calibration_res_offset_ch_Message_Size = 28;

        // 62
        public const byte HST_set_ldu_configuration_Message_ID = (byte)MessageID.HST_set_ldu_configuration;
        public const string HST_set_ldu_configuration_Message_Name = "HST_set_ldu_configuration";
        public const byte HST_set_ldu_configuration_Message_Size = 28;

        // 63
        public const byte HST_get_ldu_data_by_hga_Message_ID = (byte)MessageID.HST_get_ldu_data_by_hga;
        public const string HST_get_ldu_data_by_hga_Message_Name = "HST_get_ldu_data_by_hga";
        public const byte HST_get_ldu_data_by_hga_Message_Size = 4;

        // 64
        public const byte HST_get_ldu_voltages_Message_ID = (byte)MessageID.HST_get_ldu_voltages;
        public const string HST_get_ldu_voltages_Message_Name = "HST_get_ldu_voltages";
        public const byte HST_get_ldu_voltages_Message_Size = 3;


        // 65
        public const byte HST_set_channel_configuration_Message_ID = (byte)MessageID.HST_set_channel_configuration;
        public const string HST_set_channel_configuration_Message_Name = "HST_set_channel_configuration";
        public const byte HST_set_channel_configuration_Message_Size = 28;

        // 66
        public const byte HST_set_ch_short_det_cur_ratio_Message_ID = (byte)MessageID.HST_set_ch_short_det_cur_ratio;
        public const string HST_set_ch_short_det_cur_ratio_Message_Name = "HST_set_ch_short_det_cur_ratio";
        public const byte HST_set_ch_short_det_cur_ratio_Message_Size = 15;

        // 67
        public const byte HST_get_ch_short_det_cur_ratio_Message_ID = (byte)MessageID.HST_get_ch_short_det_cur_ratio;
        public const string HST_get_ch_short_det_cur_ratio_Message_Name = "HST_get_ch_short_det_cur_ratio";
        public const byte HST_get_ch_short_det_cur_ratio_Message_Size = 3;

        // 68
        public const byte HST_set_short_detection_vol_threshold_Message_ID = (byte)MessageID.HST_set_short_detection_vol_threshold;
        public const string HST_set_short_detection_vol_threshold_Message_Name = "HST_set_short_detection_vol_threshold";
        public const byte HST_set_short_detection_vol_threshold_Message_Size = 8;

        // 69
        public const byte HST_get_short_limits_Message_ID = (byte)MessageID.HST_get_short_limits;
        public const string HST_get_short_limits_Message_Name = "HST_get_short_limits";
        public const byte HST_get_short_limits_Message_Size = 3;

        //70
        public const byte HST_get_short_ratio_Message_ID = (byte)MessageID.HST_get_short_ratio;
        public const string HST_get_short_ratio_Message_Name = "HST_get_short_ratio";
        public const byte HST_get_short_ratio_Message_Size = 3;

        // 71
        public const byte HST_get_short_detection_vol_threshold_Message_ID = (byte)MessageID.HST_get_short_detection_vol_threshold;
        public const string HST_get_short_detection_vol_threshold_Message_Name = "HST_get_short_detection_vol_threshold";
        public const byte HST_get_short_detection_vol_threshold_Message_Size = 3;

        //72
        public const byte HST_get_vol_delta_Message_ID = (byte)MessageID.HST_get_vol_delta;
        public const string HST_get_vol_delta_Message_Name = "HST_get_vol_delta";
        public const byte HST_get_vol_delta_Message_Size = 3;

        //73
        public const byte HST_get_vol_delta_threshold_Message_ID = (byte)MessageID.HST_get_vol_delta_threshold;
        public const string HST_get_vol_delta_threshold_Message_Name = "HST_get_vol_delta_threshold";
        public const byte HST_get_vol_delta_threshold_Message_Size = 3;


        //74
        public const byte HST_get_vol_voltage_pad_Message_ID = (byte)MessageID.HST_get_voltage_pad;
        public const string HST_get_voltage_pad_Message_Name = "HST_get_voltage_pad";
        public const byte HST_get_voltage_pad_Message_Size = 3;

        //79
        public const byte HST_get_all_bias_voltage_Message_ID = (byte)MessageID.HST_get_all_bias_voltage;
        public const string HST_get_all_bias_voltage_Message_Name = "HST_get_all_bias_voltage";
        public const byte HST_get_all_bias_voltage_Message_Size = 3;

        //80
        public const byte HST_get_all_sensing_voltage_Message_ID = (byte)MessageID.HST_get_all_sensing_voltage;
        public const string HST_get_all_sensing_voltage_Message_Name = "HST_get_all_sensing_voltage";
        public const byte HST_get_all_sensing_voltage_Message_Size = 3;

        //81
        public const byte HST_swap_ch3_and_ch4_Message_ID = (byte)MessageID.HST_swap_ch3_and_ch4;
        public const string HST_swap_ch3_and_ch4_Message_Name = "HST_swap_ch3_and_ch4";
        public const byte HST_swap_ch3_and_ch4_Message_Size = 4;
        //

        //82
        public const byte HST_write_config_reg_Message_ID = (byte)MessageID.HST_write_config_reg;
        public const string HST_write_config_reg_Message_Name = "HST_write_config_reg";
        public const byte HST_write_config_reg_Message_Size = 3;

        //83
        public const byte HST_read_config_reg_Message_ID = (byte)MessageID.HST_read_config_reg;
        public const string HST_read_config_reg_Message_Name = "HST_read_config_reg";
        public const byte HST_read_config_reg_Message_Size = 3;

        //84
        public const byte HST_get_conversion_reg_data_Message_ID = (byte)MessageID.HST_get_conversion_reg_data;
        public const string HST_get_conversion_reg_data_Message_Name = "HST_get_conversion_reg_data";
        public const byte HST_get_conversion_reg_data_Message_Size = 3;

        //85
        public const byte HST_set_ldu_configuration_2_Message_ID = (byte)MessageID.HST_set_ldu_configuration_2;
        public const string HST_set_ldu_configuration_2_Message_Name = "HST_set_ldu_configuration_2";
        public const byte HST_set_ldu_configuration_2_Message_Size = 31;//28+3       

        // 86
        public const byte HST_get_photodiode_data_by_hga_Message_ID = (byte)MessageID.HST_get_photodiode_data_by_hga;
        public const string HST_get_photodiode_data_by_hga_Message_Name = "HST_get_photodiode_data_by_hga";
        public const byte HST_get_photodiode_data_by_hga_Message_Size = 4;

        // 87
        public const byte HST_get_ldu_configuration_2_Message_ID = (byte)MessageID.HST_get_ldu_configuration_2;
        public const string HST_get_ldu_configuration_2_Message_Name = "HST_get_ldu_configuration_2";
        public const byte HST_get_ldu_configuration_2_Message_Size = 3;

        // 88
        public const byte HST_get_half_LDU_Data_Message_ID = (byte)MessageID.HST_get_half_LDU_Data;
        public const string HST_get_half_LDU_Data_Message_Name = "HST_get_half_LDU_Data";
        public const byte HST_get_half_LDU_Data_Message_Size = 4;

        // 89
        public const byte HST_get_half_photodiode_data_Message_ID = (byte)MessageID.HST_get_half_photodiode_data;
        public const string HST_get_half_photodiode_data_Message_Name = "HST_get_half_photodiode_data";
        public const byte HST_get_half_photodiode_data_Message_Size = 4;
        public byte CommandID
        {
            get;
            set;
        }

        public string CommandName
        {
            get;
            set;
        }

        public byte CommandSize
        {
            get;
            set;
        }

        public byte[] CommandParameter
        {
            get;
            set;
        }

        public TestProbeAPICommand(byte commandID, string commandName, byte commandSize)
        {
            CommandID = commandID;
            CommandName = commandName;
            CommandSize = commandSize;
        }

        public TestProbeAPICommand(byte commandID, string commandName, byte commandSize, byte[] commandParameter)
        {
            CommandID = commandID;
            CommandName = commandName;
            CommandSize = commandSize;
            CommandParameter = commandParameter;
        }
    }
}
