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

namespace Seagate.AAS.Parsel.Device.SafetyController
{
    public enum ErrorCodeSafetyController
    {
        Unknown = -1,

        SUCCESS = 0,     // No errors
        MEMORY_ERR = 1,       // The internal data structures couldn't be allocated
        RANGE_ERR = 2,        // Parameter out of range
        NOT_FOUND = 3,        // Station not found
        NOT_NEXT = 4,         // Station is out of sequence
        CHECKSUM_ERR = 5,     // Tag's checksum doesn't match data
        WRITECOUNT_ERR = 6,   // The max allowable writes has been reached
        WRITE_VERIFY_ERR = 8, // Read after write failure
        TIMEOUT_ERR = 10,     // No data was received
        ASYNC_BUSY = 12,      // An asynchronous operation is already running
        ASYNC_FAIL = 13,      // The asynchronous operation failed
                
        NO_DATA = 114,
        INTERNAL_ERR    = 115, // not from RFIDh
        SerialPortError     = 117,
        SerialPortOpenError = 118,
        WRITEPROTECT_ERR  = 119
    }



    public class ExceptionSafetyController : ParselException
    {
        // Nested declarations -------------------------------------------------

        // Member variables ----------------------------------------------------
/*        static Dictionary<string, int[]> errorInfo = new Dictionary<string, int[]>
        {
           {"Force mode timeout",                                        new int[] {101, 4}},
           {"Invalid configuration",                                     new int[] {101, 6}},
           {"System failure",                                            new int[] {101, 7}},
           {"External test signal failure at safety input",              new int[] {103, 0}},
           {"Internal circuit error at safety input",                    new int[] {103, 1}},
           {"Discrepancy error at safety input",                         new int[] {103, 2}},
           {"Overload detected at test output",                          new int[] {103, 4}},
           {"Stuck at high detected at test output",                     new int[] {103, 6}},
           {"Under current detected using muting lamp",                  new int[] {103, 7}},
           {"Over current detected at safety output",                    new int[] {104, 0}},
           {"Short circuit detected at safety output",                   new int[] {104, 1}},
           {"Stuck at high detected at safety output",                   new int[] {104, 2}},
           {"Dual channel violation at safety output",                   new int[] {104, 3}},
           {"Internal circuit error at safety output",                   new int[] {104, 4}},
           {"Output PS voltage low",                                     new int[] {105, 1}},
           {"Output PS off circuit error",                               new int[] {105, 4}},
           {"Internal circuit error at test output",                     new int[] {105, 5}},
           {"Function block status error",                               new int[] {107, 2}},
           {"Internal NVS access error",                                 new int[] {108, 0}},
           {"Unsupported expansion IO unit",                             new int[] {108, 1}},
           {"Expansion IO unit maximum connection number exceeded",      new int[] {108, 2}},
           {"Expansion IO unit configuration mismatch",                  new int[] {108, 3}},
           {"Expansion IO bus error",                                    new int[] {108, 4}},
           {"Unsupported option board",                                  new int[] {108, 5}},
           {"Option board communication error or communication timeout", new int[] {108, 6}},
           {"Option board communication error or not mounted",           new int[] {108, 7}},
           {"Memory cassette not inserted or incorrect memory cassette", new int[] {109, 1}},
           {"Memory cassette removed or access error",                   new int[] {109, 2}},
           {"Internal NVS access error during " +
            "execution of memory cassette functions",                    new int[] {109, 3}},
           {"Restore model information mismatch",                        new int[] {109, 4}},
           {"Device password mismatch between " +
            "restore memory cassette and unit",                          new int[] {109, 5}},
           {"Restore prohibit error",                                    new int[] {109, 6}},
           {"Incorrect configuration data at restore",                   new int[] {109, 7}},
           {"Unconfigured unit at backup",                               new int[] {110, 0}},
           {"Unlocked unit at backup error",                             new int[] {110, 1}},
        };
*/
        //private string _errorMessage;
        private ErrorCodeSafetyController _errorCode;

        // Constructors & Finalizers -------------------------------------------

        public ExceptionSafetyController()
        { }
        public ExceptionSafetyController(string msg)
            : base(msg)
        { }
        public ExceptionSafetyController(string msg, Exception ex)
            : base(msg, ex)
        { }

        // Properties ----------------------------------------------------------

        public ErrorCodeSafetyController ErrorCodeSafetyController
        { get { return _errorCode; } }

        // Methods -------------------------------------------------------------

        public static ExceptionSafetyController Create(ErrorCodeSafetyController errorCode)
        { return Create(errorCode, ""); }

        public static ExceptionSafetyController Create(ErrorCodeSafetyController errorCode, string message)
        {
            string errorMessage = "";
            Severity severity = Severity.None;
            switch (errorCode)
            {
                case ErrorCodeSafetyController.Unknown:
                    errorMessage = "Unknown error.";
                    severity = Severity.Error;
                    break;
               case ErrorCodeSafetyController.MEMORY_ERR:
                    errorMessage = "Memory error. The internal data structures couldn't be allocated.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeSafetyController.RANGE_ERR:
                    errorMessage = "Parameter out of range error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeSafetyController.NOT_FOUND:
                    errorMessage = "Station code not found error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeSafetyController.NOT_NEXT:
                    errorMessage = "Station is out of sequence error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeSafetyController.CHECKSUM_ERR:
                    errorMessage = "Check sum error found on received data from Safety Controller.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeSafetyController.WRITECOUNT_ERR:
                    errorMessage = "The maximum number of write cycles has been reached on the RFID chip for the tray at the " + message + ".";
                    severity = Severity.Error;
                    break;
                //case ErrorCodeSafetyController.INVALID_ID:
                //    break;
                case ErrorCodeSafetyController.WRITE_VERIFY_ERR:
                    errorMessage = "Read after write failure.";
                    severity = Severity.Error;
                    break;
                 case ErrorCodeSafetyController.TIMEOUT_ERR:
                    errorMessage = "No data was received error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeSafetyController.ASYNC_BUSY:
                    errorMessage = "An asynchronous operation is already running.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeSafetyController.ASYNC_FAIL:
                    errorMessage = "The asynchronous operation failed.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeSafetyController.NO_DATA:
                    errorMessage = " Data carrier missing.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeSafetyController.INTERNAL_ERR:
                    errorMessage = "DLL internal error.";
                    severity = Severity.Error;
                    break;
                case ErrorCodeSafetyController.SerialPortError:
                    errorMessage = "ErrorCodeSafetyController on port " + message + ".  Check that all cables are connected and the port is not being used by another application.";
                    break;
                case ErrorCodeSafetyController.SerialPortOpenError:
                    errorMessage = "ErrorCodeSafetyController opening port " + message + ".  Check that it is not being used by another application.";
                    break;
                case ErrorCodeSafetyController.WRITEPROTECT_ERR:
                    errorMessage = "RFID Write Protect Error.";
                    break;
                default:
                    errorMessage = "Omron internal error reading/writing the RFID tag on the tray at the " + message + ".";
                    severity = Severity.Error;
                    break;
            }

            ExceptionSafetyController ex = new ExceptionSafetyController(errorMessage);
            ex.severity = severity;
            ex._errorCode = errorCode;
            return ex;
        }

        // Event handlers ------------------------------------------------------

        // Internal methods ----------------------------------------------------

    }
}


/*

    ERROR_FLAG_INDCES = {
        'Force mode timeout':                                        (101, 4),
        'Invalid configuration':                                     (101, 6),
        'System failure':                                            (101, 7),
        'External test signal failure at safety input':              (103, 0),
        'Internal circuit error at safety input':                    (103, 1),
        'Discrepancy error at safety input':                         (103, 2),
        'Overload detected at test output':                          (103, 4),
        'Stuck at high detected at test output':                     (103, 6),
        'Under current detected using muting lamp':                  (103, 7),
        'Over current detected at safety output':                    (104, 0),
        'Short circuit detected at safety output':                   (104, 1),
        'Stuck at high detected at safety output':                   (104, 2),
        'Dual channel violation at safety output':                   (104, 3),
        'Internal circuit error at safety output':                   (104, 4),
        'Output PS voltage low':                                     (105, 1),
        'Output PS off circuit error':                               (105, 4),
        'Internal circuit error at test output':                     (105, 5),
        'Function block status error':                               (107, 2),
        'Internal NVS access error':                                 (108, 0),
        'Unsupported expansion IO unit':                             (108, 1),
        'Expansion IO unit maximum connection number exceeded':      (108, 2),
        'Expansion IO unit configuration mismatch':                  (108, 3),
        'Expansion IO bus error':                                    (108, 4),
        'Unsupported option board':                                  (108, 5),
        # 'Option board communication error or communication timeout': (108, 6),
        'Option board communication error or not mounted':           (108, 7),
        'Memory cassette not inserted or incorrect memory cassette': (109, 1),
        'Memory cassette removed or access error':                   (109, 2),
        'Internal NVS access error during ' +
        'execution of memory cassette functions':                    (109, 3),
        'Restore model information mismatch':                        (109, 4),
        'Device password mismatch between ' +
        'restore memory cassette and unit':                          (109, 5),
        'Restore prohibit error':                                    (109, 6),
        'Incorrect configuration data at restore':                   (109, 7),
        'Unconfigured unit at backup':                               (110, 0),
        'Unlocked unit at backup error':                             (110, 1),
    }

*/