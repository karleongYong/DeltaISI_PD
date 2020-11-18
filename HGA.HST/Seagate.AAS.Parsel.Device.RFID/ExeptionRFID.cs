//
//  (c) Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [2007/01/14] Seagate HGA Automation
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Seagate.AAS.Parsel;

namespace Seagate.AAS.Parsel.Device.RFID
{
    public enum ErrorCodeRFID
    {
        Unknown = -1,

        RFID_SUCCESS                = 0x00, // No errors
        MEMORY_ERR                  = 0x01, // The internal data structures couldn't be allocated
        RANGE_ERR                   = 0x02, // Parameter out of range
        NOT_FOUND                   = 0x03, // Station not found
        NOT_NEXT                    = 0x04, // Station is out of sequence
        CHECKSUM_ERR                = 0x05, // Tag's checksum doesn't match data
        WRITECOUNT_ERR              = 0x06, // The max allowable writes has been reached
        //INVALID_CARRIER             = 0x07, // Invalid carrier Id in tag
        INVALID_ID                  = 0x07, // Invalid carrier or tray id
        WRITE_VERIFY_ERR            = 0x08, // Read after write failure
        TAGTYPE_ERR                 = 0x09, // Operation not valid for tag
        TIMEOUT_ERR                 = 0x0A, // No data was received
        NO_WORKORDER                = 0x0B, // No workorder description file was found
        ASYNC_BUSY                  = 0x0C, // An asynchronous operation is already running
        ASYNC_FAIL                  = 0x0D, // The asynchronous operation failed

        // Controller Error Code
        CTRL_BATTERY_LOW            = 0x7B, // Battery low warning indicating the battery replacement is required
        CTRL_VERICAL_PARITY_ERROR   = 0x10, // Vertical parity error (do not occur on V600-CHUD)
        CTRL_FRAMING_ERROR          = 0x11, // Framing Error (do not occur on V600-CHUD)
        CTRL_OVERRUN_ERROR          = 0x12, // Overrun Error (do not occur on V600-CHUD)
        CTRL_FORMAT_ERROR           = 0x14, // Format Error
        CTRL_EXECUTION_STATUS_ERROR = 0x15, // Execution status error
        CTRL_FRAME_LENGTH_ERROR     = 0x18, // Frame length error
        CTRL_COMM_ERROR             = 0x70, // Data carrier communications error
        CTRL_MISMATCH_ERROR         = 0x71, // Mismatch error
        CTRL_DATA_CARRIER_ERROR     = 0x72, // Data carrier non-existent error
        CTRL_DATA_ERROR             = 0x76, // Data error
        CTRL_ADDRESS_ERROR          = 0x7A, // Address error
        CTRL_ANTENNA_ERROR          = 0x7C, // Antenna error
        CTRL_WRITE_PROTECT_ERROR    = 0x7D, // Write protect error
        CTRL_MEMORY_ERROR           = 0x93, // Memory error (do not occur on V600-CHUD)
                       
        DLL_INTERNAL_ERR            = 0xF0,  // not from RFIDh
        SerialPortError             = 0xF2,
        SerialPortOpenError         = 0xF3,
        INVALID_DATA                = 0xFC  // Invalid Data

    }

    public class ExceptionRFID : ParselException
    {
        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------

        //private string _errorMessage;
        private ErrorCodeRFID _errorCode;

        // Constructors & Finalizers -------------------------------------------

        public ExceptionRFID()
        { }
        public ExceptionRFID(string msg)
            : base(msg)
        { }
        public ExceptionRFID(string msg, Exception ex)
            : base(msg, ex)
        { }

        // Properties ----------------------------------------------------------

        public ErrorCodeRFID ErrorCodeRFID { get { return _errorCode; } }

        public int ErrorCode { get { return (int) _errorCode; } }

        // Methods -------------------------------------------------------------

        public static ExceptionRFID Create(ErrorCodeRFID errorCode)
        { return Create(errorCode, ""); }

        public static ExceptionRFID Create(ErrorCodeRFID errorCode, string message)
        {
            string errorMessage = "";
            Severity severity = Severity.None;
            switch (errorCode)
            {
                case ErrorCodeRFID.Unknown:
                    errorMessage = "Unknown error.";
                    severity = Severity.Error;
                    break;

                case ErrorCodeRFID.RFID_SUCCESS:
                    errorMessage = "RFID operation succeeded.";
                    severity = Severity.None;
                    break;
                case ErrorCodeRFID.MEMORY_ERR:
                    errorMessage = "Memory error. The internal data structures couldn't be allocated.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.RANGE_ERR:
                    errorMessage = "Parameter out of range error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.NOT_FOUND:
                    errorMessage = "Station code not found error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.NOT_NEXT:
                    errorMessage = "Station is out of sequence error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.CHECKSUM_ERR:
                    errorMessage = "Check sum error for data in RFID tag. The data is corrupted.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.WRITECOUNT_ERR:
                    errorMessage = "The maximum number of write cycles has been reached on the RFID tag.";
                    severity = Severity.Error;
                    break;
                //case ErrorCodeRFID.INVALID_CARRIER:
                //    errorMessage = "The tray at the RF head " + message + " does not contain a valid tray ID.";
                //    severity = Severity.Error;
                //    break;
                case ErrorCodeRFID.INVALID_ID:
                    errorMessage = "Invalid Carrier/Tray ID.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.WRITE_VERIFY_ERR:
                    errorMessage = "Read after write failure.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.TAGTYPE_ERR:
                    errorMessage = "Operation not valid for tag error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.TIMEOUT_ERR:
                    errorMessage = "No data was received error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.NO_WORKORDER:
                    errorMessage = "No workorder description file was found error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.ASYNC_BUSY:
                    errorMessage = "An asynchronous operation is already running.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.ASYNC_FAIL:
                    errorMessage = "The asynchronous operation failed.";
                    severity = Severity.Error;
                    break;
                // Error from Controller

                case ErrorCodeRFID.CTRL_BATTERY_LOW:
                    errorMessage = "Battery low warning indicating the battery replacement is required.";
                    severity = Severity.Warning;
                    break;                   
                case ErrorCodeRFID.CTRL_VERICAL_PARITY_ERROR:
                    errorMessage = "Vertical parity error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.CTRL_FRAMING_ERROR:
                    errorMessage = "Framing Error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.CTRL_OVERRUN_ERROR:
                    errorMessage = "Overrun Error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.CTRL_FORMAT_ERROR:
                    errorMessage = "Format Error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.CTRL_EXECUTION_STATUS_ERROR:
                    errorMessage = "Execution status error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.CTRL_FRAME_LENGTH_ERROR:
                    errorMessage = "Frame length error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.CTRL_COMM_ERROR:
                    errorMessage = "Data carrier communications error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.CTRL_MISMATCH_ERROR:
                    errorMessage = "Mismatch error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.CTRL_DATA_CARRIER_ERROR:
                    errorMessage = "Data carrier non-existent error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.CTRL_DATA_ERROR:
                    errorMessage = "Data error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.CTRL_ADDRESS_ERROR:
                    errorMessage = "Address error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.CTRL_ANTENNA_ERROR:
                    errorMessage = "Antenna error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.CTRL_WRITE_PROTECT_ERROR:
                    errorMessage = "Write protect error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.CTRL_MEMORY_ERROR:
                    errorMessage = "Memory error.";
                    severity = Severity.Error;
                    break;

                // External Error
                case ErrorCodeRFID.DLL_INTERNAL_ERR:
                    errorMessage = "DLL internal error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.SerialPortError:
                    errorMessage = "Port " + message + ", Serial port error. Check that all cables are connected and the port is not being used by another application.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.SerialPortOpenError:
                    errorMessage = "Opening port " + message + " Error.  Check that it is not being used by another application.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeRFID.INVALID_DATA:
                    errorMessage = "Invalid Data.";
                    severity = Severity.Error;
                    break;
                default:
                    errorMessage = "Omron internal error reading/writing the RFID tag on the tray at the " + message + ".";
                    severity = Severity.Error;
                    break;
            }

            ExceptionRFID ex = new ExceptionRFID(errorMessage);
            ex.severity = severity;
            ex._errorCode = errorCode;
            return ex;
        }

        // Event handlers ------------------------------------------------------

        // Internal methods ----------------------------------------------------

    }
}