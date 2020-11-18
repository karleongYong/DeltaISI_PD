using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DesktopTester.Utils;

namespace DesktopTester.Data
{
    public class TestProbeAPICommand
    {
        // 1
        public const byte HST_get_status_Message_ID = (byte)MessageID.HST_get_status;
        public const byte HST_get_status_Message_Size = 3;        

        // 2
        public const byte HST_config_res_meas_Message_ID = (byte)MessageID.HST_config_res_meas;
        public const byte HST_config_res_meas_Message_Size = 16;

        // 3
        public const byte HST_config_cap_meas_Message_ID = (byte)MessageID.HST_config_cap_meas;
        public const byte HST_config_cap_meas_Message_Size = 11;

        // 4
        public const byte HST_config_short_detection_Message_ID = (byte)MessageID.HST_config_short_detection;
        public const byte HST_config_short_detection_Message_Size = 15;

        // 5
        public const byte HST_meas_channel_enable_Message_ID = (byte)MessageID.HST_meas_channel_enable;
        public const byte HST_meas_channel_enable_Message_Size = 11;

        // 6
        public const byte HST_hga_enable_Message_ID = (byte)MessageID.HST_hga_enable;
        public const byte HST_hga_enable_Message_Size = 13;

        // 7
        public const byte HST_get_product_id_Message_ID = (byte)MessageID.HST_get_product_id;
        public const byte HST_get_product_id_Message_Size = 3;

        // 8
        public const byte HST_get_operation_mode_Message_ID = (byte)MessageID.HST_get_operation_mode;
        public const byte HST_get_operation_mode_Message_Size = 3;

        // 9
        public const byte HST_start_meas_Message_ID = (byte)MessageID.HST_start_meas;
        public const byte HST_start_meas_Message_Size = 4;

        // 10
        public const byte HST_get_short_detection_Message_ID = (byte)MessageID.HST_get_short_detection;
        public const byte HST_get_short_detection_Message_Size = 3; 

        // 11
        public const byte HST_get_res_results_Message_ID = (byte)MessageID.HST_get_res_results;
        public const byte HST_get_res_results_Message_Size = 3;        

        // 12
        public const byte HST_get_cap_results_Message_ID = (byte)MessageID.HST_get_cap_results;
        public const byte HST_get_cap_results_Message_Size = 3;

        // 13
        public const byte HST_get_bias_voltages_Message_ID = (byte)MessageID.HST_get_bias_voltages;
        public const byte HST_get_bias_voltages_Message_Size = 3;

        // 14
        public const byte HST_get_results_by_hga_Message_ID = (byte)MessageID.HST_get_results_by_hga;
        public const byte HST_get_results_by_hga_Message_Size = 5;

        // 15
        public const byte HST_get_bias_by_hga_Message_ID = (byte)MessageID.HST_get_bias_by_hga;
        public const byte HST_get_bias_by_hga_Message_Size = 4;

        // 16
        public const byte HST_get_sensing_by_hga_Message_ID = (byte)MessageID.HST_get_sensing_by_hga;
        public const byte HST_get_sensing_by_hga_Message_Size = 4;

        // 17
        public const byte HST_calibration_enable_Message_ID = (byte)MessageID.HST_calibration_enable;
        public const byte HST_calibration_enable_Message_Size = 4;

        // 18
        public const byte HST_start_auto_calibration_Message_ID = (byte)MessageID.HST_start_auto_calibration;
        public const byte HST_start_auto_calibration_Message_Size = 3;

        // 19
        public const byte HST_save_calibration_data_Message_ID = (byte)MessageID.HST_save_calibration_data;
        public const byte HST_save_calibration_data_Message_Size = 3;

        // 20
        public const byte HST_get_calibration_data_Message_ID = (byte)MessageID.HST_get_calibration_data;
        public const byte HST_get_calibration_data_Message_Size = 3;

        // 21
        public const byte HST_manual_set_calibration_Message_ID = (byte)MessageID.HST_manual_set_calibration;
        public const byte HST_manual_set_calibration_Message_Size = 10;

        // 22
        public const byte HST_eeprom_write_Message_ID = (byte)MessageID.HST_eeprom_write;
        public const byte HST_eeprom_write_Message_Size = 7;

        // 23
        public const byte HST_eeprom_read_Message_ID = (byte)MessageID.HST_eeprom_read;
        public const byte HST_eeprom_read_Message_Size = 6;

        // 24
        public const byte HST_dac_write_Message_ID = (byte)MessageID.HST_dac_write;
        public const byte HST_dac_write_Message_Size = 6;

        // 25
        public const byte HST_dac_read_Message_ID = (byte)MessageID.HST_dac_read;
        public const byte HST_dac_read_Message_Size = 4;

        // 26
        public const byte HST_dac_output_enable_Message_ID = (byte)MessageID.HST_dac_output_enable;
        public const byte HST_dac_output_enable_Message_Size = 4;

        // 27
        public const byte HST_adc_write_Message_ID = (byte)MessageID.HST_adc_write;
        public const byte HST_adc_write_Message_Size = 8;

        // 28
        public const byte HST_adc_read_Message_ID = (byte)MessageID.HST_adc_read;
        public const byte HST_adc_read_Message_Size = 5;

        // 29
        public const byte HST_get_adc_voltages_Message_ID = (byte)MessageID.HST_get_adc_voltages;
        public const byte HST_get_adc_voltages_Message_Size = 5;

        // 30
        public const byte HST_set_mux_Message_ID = (byte)MessageID.HST_set_mux;
        public const byte HST_set_mux_Message_Size = 5;

        // 31
        public const byte HST_set_temp_calibration_Message_ID = (byte)MessageID.HST_set_temp_calibration;
        public const byte HST_set_temp_calibration_Message_Size = 5;

        // 32
        public const byte HST_config_temp_meas_Message_ID = (byte)MessageID.HST_config_temp_meas;
        public const byte HST_config_temp_meas_Message_Size = 4;

        // 33
        public const byte HST_get_temperature_Message_ID = (byte)MessageID.HST_get_temperature;
        public const byte HST_get_temperature_Message_Size = 3;

        // 34
        public const byte HST_get_cap_secondary_results_Message_ID = (byte)MessageID.HST_get_cap_secondary_results;
        public const byte HST_get_cap_secondary_results_Message_Size = 3;

        // 35
        public const byte HST_get_cap_reading_Message_ID = (byte)MessageID.HST_get_cap_reading;
        public const byte HST_get_cap_reading_Message_Size = 3;

        // 36
        public const byte HST_start_self_test_Message_ID = (byte)MessageID.HST_start_self_test;
        public const byte HST_start_self_test_Message_Size = 3;

        // 37
        public const byte HST_get_firmware_version_Message_ID = (byte)MessageID.HST_get_firmware_version;
        public const byte HST_get_firmware_version_Message_Size = 3;

        // 38
        public const byte HST_calibrate_offset_Message_ID = (byte)MessageID.HST_calibrate_offset;
        public const byte HST_calibrate_offset_Message_Size = 3;

        // 39
        public const byte HST_get_calibration_offset_Message_ID = (byte)MessageID.HST_get_calibration_offset;
        public const byte HST_get_calibration_offset_Message_Size = 3;

        // 43
        public const byte HST_flex_cable_calibration_Message_ID = (byte)MessageID.HST_flex_cable_calibration;
        public const byte HST_flex_cable_calibration_Message_Size = 4;

        // 44
        public const byte HST_get_cable_calibration_res_results_Message_ID = (byte)MessageID.HST_get_cable_calibration_res_results;
        public const byte HST_get_cable_calibration_res_results_Message_Size = 4;

        // 45
        public const byte HST_set_cable_compensation_Message_ID = (byte)MessageID.HST_set_cable_compensation;
        public const byte HST_set_cable_compensation_Message_Size = 11;

        // 46
        public const byte HST_clear_all_cable_compensation_Message_ID = (byte)MessageID.HST_clear_all_cable_compensation;
        public const byte HST_clear_all_cable_compensation_Message_Size = 3;
        
        // 47
        public const byte HST_set_short_detection_threshold_Message_ID = (byte)MessageID.HST_set_short_detection_threshold;
        public const byte HST_set_short_detection_threshold_Message_Size = 7;

        // 48
        public const byte HST_get_short_detection_threshold_Message_ID = (byte)MessageID.HST_get_short_detection_threshold;
        public const byte HST_get_short_detection_threshold_Message_Size = 3;

        // 49
        public const byte HST_set_temp1_offset_Message_ID = (byte)MessageID.HST_set_temp1_offset;
        public const byte HST_set_temp1_offset_Message_Size = 4;

        // 50
        public const byte HST_get_temp1_offset_Message_ID = (byte)MessageID.HST_get_temp1_offset;
        public const byte HST_get_temp1_offset_Message_Size = 3;

        // 51
        public const byte HST_get_cable_calibration_cap_results_Message_ID = (byte)MessageID.HST_get_cable_calibration_cap_results;
        public const byte HST_get_cable_calibration_cap_results_Message_Size = 4;

        // 52
        public const byte HST_set_precisor_cap_compensation_Message_ID = (byte)MessageID.HST_set_precisor_cap_compensation;
        public const byte HST_set_precisor_cap_compensation_Message_Size = 85;

        // 53
        public const byte HST_get_precisor_cap_compensation_Message_ID = (byte)MessageID.HST_get_precisor_cap_compensation;
        public const byte HST_get_precisor_cap_compensation_Message_Size = 4;

        // 54
        public const byte HST_save_precisor_cap_compensation_Message_ID = (byte)MessageID.HST_save_precisor_cap_compensation;
        public const byte HST_save_precisor_cap_compensation_Message_Size = 3;


        /*********************************************************************************************************************************************/
        // Desktop Tester commands
        // 80
        public const byte HST_desktop_tester_connect_Message_ID = (byte)MessageID.HST_desktop_tester_connect;
        public const byte HST_desktop_tester_connect_Message_Size = 5;

        // 81
        public const byte HST_desktop_tester_set_homing_axis_Message_ID = (byte)MessageID.HST_desktop_tester_set_homing_axis;
        public const byte HST_desktop_tester_set_homing_axis_Message_Size = 4;

        // 82
        public const byte HST_desktop_tester_set_servo_off_Message_ID = (byte)MessageID.HST_desktop_tester_set_servo_off;
        public const byte HST_desktop_tester_set_servo_off_Message_Size = 3;

        // 83
        public const byte HST_desktop_tester_set_servo_abs_move_axis_Message_ID = (byte)MessageID.HST_desktop_tester_set_servo_abs_move_axis;
        public const byte HST_desktop_tester_set_servo_abs_move_axis_Message_Size = 20;

        // 84
        public const byte HST_desktop_tester_set_servo_rel_move_axis_Message_ID = (byte)MessageID.HST_desktop_tester_set_servo_rel_move_axis;
        public const byte HST_desktop_tester_set_servo_rel_move_axis_Message_Size = 8;

        // 85
        public const byte HST_desktop_tester_get_servo_positions_Message_ID = (byte)MessageID.HST_desktop_tester_get_servo_positions;
        public const byte HST_desktop_tester_get_servo_positions_Message_Size = 3;

        // 86
        public const byte HST_desktop_tester_teach_servo_position_Message_ID = (byte)MessageID.HST_desktop_tester_teach_servo_position;
        public const byte HST_desktop_tester_teach_servo_position_Message_Size = 4;

        // 87
        public const byte HST_desktop_tester_move_to_taught_servo_position_Message_ID = (byte)MessageID.HST_desktop_tester_move_to_taught_servo_position;
        public const byte HST_desktop_tester_move_to_taught_servo_position_Message_Size = 4;

        // 88
        public const byte HST_desktop_tester_save_taught_position_Message_ID = (byte)MessageID.HST_desktop_tester_save_taught_position;
        public const byte HST_desktop_tester_save_taught_position_Message_Size = 3;

        // 89
        public const byte HST_desktop_tester_set_output_Message_ID = (byte)MessageID.HST_desktop_tester_set_output;
        public const byte HST_desktop_tester_set_output_Message_Size = 5;

        // 90
        public const byte HST_desktop_tester_get_all_inputs_Message_ID = (byte)MessageID.HST_desktop_tester_get_all_inputs;
        public const byte HST_desktop_tester_get_all_inputs_Message_Size = 3;

        // 91
        public const byte HST_desktop_tester_load_hga_Message_ID = (byte)MessageID.HST_desktop_tester_load_hga;
        public const byte HST_desktop_tester_load_hga_Message_Size = 4;

        // 92
        public const byte HST_desktop_tester_eject_hga_Message_ID = (byte)MessageID.HST_desktop_tester_eject_hga;
        public const byte HST_desktop_tester_eject_hga_Message_Size = 3;

        // 93
        public const byte HST_desktop_tester_disconnect_Message_ID = (byte)MessageID.HST_desktop_tester_disconnect;
        public const byte HST_desktop_tester_disconnect_Message_Size = 3;

        // 94
        public const byte HST_desktop_tester_jog_Message_ID = (byte)MessageID.HST_desktop_tester_jog;
        public const byte HST_desktop_tester_jog_Message_Size = 5;

        // 95
        public const byte HST_desktop_tester_jog_stop_Message_ID = (byte)MessageID.HST_desktop_tester_jog_stop;
        public const byte HST_desktop_tester_jog_stop_Message_Size = 4;

        // 96
        public const byte HST_desktop_tester_precisor_vac_on_Message_ID = (byte)MessageID.HST_desktop_tester_precisor_vac_on;
        public const byte HST_desktop_tester_precisor_vac_on_Message_Size = 4;


        public byte CommandID
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

        public TestProbeAPICommand(byte commandID, byte commandSize, byte[] commandParameter)
        {
            CommandID = commandID;
            CommandSize = commandSize;
            CommandParameter = commandParameter;
        }
    }
}
