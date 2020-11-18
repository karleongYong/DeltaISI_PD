using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/* This common data structure is used for the messages returned by the Measurement MicroProcessor for the following API commands:
 * 
 *     Command ID           Command Name
 *     ==========           ============
 *         1                HST_get_status
 *         2                HST_config_res_meas
 *         3                HST_config_cap_meas
 *         4                HST_config_short_detection
 *         5                HST_meas_channel_enable
 *         6                HST_hga_enable
 *         9                HST_start_meas
 *        17                HST_calibration_enable
 *        19                HST_save_calibration_data
 *        22                HST_eeprom_write
 *        24                HST_dac_write
 *        26                HST_dac_output_enable
 *        27                HST_adc_write
 *        30                HST_set_mux
 *        32                HST_config_temp_meas
 *        38                HST_unsolicited_status
 */
namespace BenchTestsTool.Data.IncomingTestProbeData
{
    public unsafe struct TestProbeGetStatusAndErrorCode
    {        
        public byte StartOfMessage;
        public byte MessageSize;
        public byte MessageType;
        public byte MessageID;
        public byte Status;        // 0:READY; 1:BUSY; 2:ERROR
        public byte ErrorCode;     // Always 0 unless when Status=ERROR, this byte indicates the error code.
        

        public static TestProbeGetStatusAndErrorCode ReadStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(TestProbeGetStatusAndErrorCode*)pb;
            }
        } 
    }
}
