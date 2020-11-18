//************************************************************************************************/
//						SEAGATE FACTORY OF THE FUTURE AUTOMATION.
//					PROJECT : HDA.BUD (WINDOWS APPLICATION OUTPUT).
//	FILE : C:\AAS_SDK\PROJECTS_NET\HDA\BUD\SOURCE\CONTROLLERS\HWCRFIDRW.CS 
//						DEVELOPMENT ENGINEER : SANJAY SUNDARESAN.
//								CREATED ON : FEB. 23, 2006.
//									REVISION : 06.XX.XX.
//************************************************************************************************/

// Reference namespaces used.

// System Namespaces.
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;	// For registry operations.
// Project Namespaces.
using Seagate.AAS.Parsel;
using SerialPorts;

//************************************************************************************************/
/// <summary> Namespace : BUD. The contents of the BUD project are defined under this namespace. </summary>

namespace BUD
{
	//************************************************************************************************/
	/// <summary> HWCRFIDRWSetup Class -- Contains setup information for the HWCRFIDRW class . </summary>

	public class HWCRFIDRWSetup
	{
		//************************************************************************************************/
		// Declare all fields here.
		
		public int StationNumber = 32;

		//************************************************************************************************/
		// Declare all enumerations here.

		//************************************************************************************************/
		// Declare all positions here.
		
		//************************************************************************************************/
		/// <summary> Function : HWCRFIDRWSetup () -- Constructor. </summary>
		/// <returns> None. </returns>

		public HWCRFIDRWSetup ()
		{
		}
		//************************************************************************************************/
		/// <summary> Function : ~HWCRFIDRWSetup () -- Destructor. </summary>
		/// <returns> None. </returns>

		~HWCRFIDRWSetup ()
		{	// De-allocate all memory allocated from the heap (with the "new" keyword).
			// The .NET garbage collector will purge it back to the stack and heap.

			// Call the .NET garbage collector to purge memory back to stack and heap.
			GC.Collect ();
		}
		//************************************************************************************************/
		/// <summary> Function : LoadInformation (). </summary>

		public void LoadInformation ()
		{
			RegistryKey regkey = null;
			// Base key to be "LocalMachine\Software\Seagate Technology\FOF BUD\Setup\RFID-RW".
			string basekey = @"Software\Seagate Technology\FOF BUD\Setup\RFID-RW\";
			string keylocation = "";
			
			// Read the "HKEY_LOCAL_MACHINE\Software\Seagate Technology\FOF BUD\Setup\RFID-RW\Setup" key.
			keylocation = basekey + "Setup";
			// Use the Microsoft.Win32.Registry class' to open the subkey under HKEY_LOCAL_MACHINE.
			// (Args = key location, write access needed ? = yes). Returns reference to RegistryKey or null (if not found).
			regkey = Registry.LocalMachine.OpenSubKey (keylocation, true);
			// If registry key is not found, create it now at the location.
			if (regkey == null)
				regkey = Registry.LocalMachine.CreateSubKey (keylocation);

			// Read the following setup parameters from the registry in the "\RFID-RW\Setup" key.
			// Args = Name, value to return if name is not found. Returns as a object, but contains the value in the same data type
			// as registry was written.
			// Read the "Station Number" name-value pair.
			StationNumber = (int) regkey.GetValue ("Station Number", 32);
		}
		//************************************************************************************************/
		/// <summary> Function : SaveInformation (). </summary>

		public void SaveInformation ()
		{
			RegistryKey regkey = null;
			// Base key to be "LocalMachine\Software\Seagate Technology\FOF BUD\Setup\RFID-RW".
			string basekey = @"Software\Seagate Technology\FOF BUD\Setup\RFID-RW\";
			string keylocation = "";
			
			// Read the "HKEY_LOCAL_MACHINE\Software\Seagate Technology\FOF BUD\Setup\RFID-RW\Setup" key.
			keylocation = basekey + "Setup";
			// Use the Microsoft.Win32.Registry class' to open the subkey under HKEY_LOCAL_MACHINE.
			// (Args = key location, write access needed ? = yes). Returns reference to RegistryKey or null (if not found).
			regkey = Registry.LocalMachine.OpenSubKey (keylocation, true);
			// If registry key is not found, create it now at the location.
			if (regkey == null)
				regkey = Registry.LocalMachine.CreateSubKey (keylocation);

			// Save the following name-value pairs in the "\RFID-RW\Setup" key.
			// Args = Name, value to write (object).
			// Save the "Station Number" name-value pair.
			regkey.SetValue ("Station Number", this.StationNumber);
		}
		//************************************************************************************************/
	}	// End of HWCRFIDRWSetup class.

	//************************************************************************************************/
	//************************************************************************************************/
	/// <summary> HWCRFIDRW Class -- Controller for the HMS 827 RFID Reader/Writer. </summary>
	
	public class HWCRFIDRW
	{	
		// Import the DLL "kernel32.dll" as unmanaged library -- Used for GetTickCount () function.
		// Tag it with the attribute "DllImport".  Indicates that the tagged method "GetTickCount ()" is
		// exposed by an unmanaged library as a static entry point.
		[DllImport ("kernel32.dll", SetLastError = true)]
		static extern int GetTickCount ();

		//************************************************************************************************/
		// Declare all fields here.

		// Fields common to all hardware controller classes.
		private BUDWorkcell.StationOperation _Operation = BUDWorkcell.StationOperation.None;
		private BUDWorkcell.StationRunMode _RunMode = BUDWorkcell.StationRunMode.None;
		private bool _HWSimulationMode = false;
		private BUDIOManifest _IOManifest = null;
		private string _StatusMessage = "";
		private string _ExceptionMessage = "";
		private long _ExceptionCode = 0x0;
		private HWCRFIDRWSetup _Setup = null;
		
		// Hardware controller specific fields.
		private RFIDType _RFIDType = RFIDType.Unknown;
		private int _COMPort = 0;
		private SerialPort _SerialPort = null;
		private WithEvents _SerialPortEvents;
		private System.Text.ASCIIEncoding _ASCIIEncoder = new System.Text.ASCIIEncoding ();
		private string _ProcessCode = "";
		// Create a struct array to store the HSA information read from the HSA tray RFID tag.
		// Static field (one instance of field regardless of however many instances of class).
		// Assume a maximum of 50 HSA's per tray for now.
		public static TrayDataDefinition [] TrayHSAAttribute = new TrayDataDefinition [50];

		// I/O/S Map.
	
		//************************************************************************************************/
		// Declare all positions (points table) here.

//		private int _Setup.StationNumber = 32;

		//************************************************************************************************/
		// Declare all constants here.
		
		private const int _RFIDCommunicationsTimeOut = 3000;	// in msec.
		// HDA RFID tag map.
		private const int _DriveTagHDASNStartAddress = 20;
		private const int _DriveTagProcessCodeStartAddress = 48;
		private const int _DriveTagFailCodeStartAddress = 106;
		private const int _DriveTagHSAConfigStartAddress = 132;
		private const int _DriveTagHSAAttributeInfoStartAddress = 278;
		private const int _DriveTagStationHistoryStartAddress = 448;
		// Tray RFID tag map.
		private const int _TrayTagHSAInfoStartAddress = 5;
		private const int _TrayTagHSAInfoBytesPerPart = 32;
		private const int _TrayTagHSAInfoNumberOfParts = 20;

		//************************************************************************************************/
		// Declare all enumerations here.
		
		public enum RFIDType
		{
			Unknown		= 0	,
			HDAZone0		,
			HDAZone2		,
			TrayFeeder		,
		}
		//************************************************************************************************/
		/// <summary> Constructor for HWCRFIDRW. </summary>
		
		public HWCRFIDRW ()
		{	// Create instance of HWCRobotGantrySetup object -- carries setup information.
			_Setup = new HWCRFIDRWSetup ();
			// Load the setup information from the Registry.
			_Setup.LoadInformation ();
			// Re-save the setup information to the registry again...in case some new keys were created in 
			// this version of software or new software installation,....
			_Setup.SaveInformation ();
			// Create the status message.
			_StatusMessage = "Creating the HWCRFIDRW () class....Done.";
		}
		//************************************************************************************************/
		/// <summary> Destructor for HWCRFIDRW. </summary>
		
		~HWCRFIDRW ()
		{	// If serial port is open, close it first.
			if (_SerialPort.IsOpen)
				_SerialPort.Close ();
			// De-allocate all memory allocated from the heap (with the "new" keyword).
			// The .NET garbage collector will purge it back to the stack and heap.				
			_SerialPort = null;
			_ASCIIEncoder = null;
			_Setup = null;

			// Call the .NET garbage collector to purge memory back to stack and heap.
			GC.Collect ();
		}
		//************************************************************************************************/
		/// <summary> Property : IOManifest object for the BUD Workcell. </summary>
		
		public BUDIOManifest IOManifest
		{
			set	{	_IOManifest = value;	}
		}
		//************************************************************************************************/
		/// <summary> Property : BUD Operation Mode for the Workcell (New Build / Rework). </summary>
		
		public BUDWorkcell.StationOperation Operation
		{
			set	{	_Operation = value;	}
		}
		//************************************************************************************************/
		/// <summary> Property : BUD Run Mode for the Workcell (Auto / StandAlone / Bypass / Dry Run). </summary>
		
		public BUDWorkcell.StationRunMode RunMode
		{
			set	{	_RunMode = value;	}
		}
		//************************************************************************************************/
		/// <summary> Property : HW Simulation Mode. </summary>
		
		public bool HWSimulationMode
		{
			set	{	_HWSimulationMode = value;	}
		}
		//************************************************************************************************/
		/// <summary> RFID R/W Type (Location) -- HDA Zone-0, HDA Zone-2, Tray Feeder. </summary>
		
		public RFIDType RFIDRWLocation
		{
			set	{	_RFIDType = value;	}
		}
		//************************************************************************************************/
		/// <summary> Property : Status message of this controller. </summary>
		/// <returns> String. </returns>
		
		public string Status
		{	
			get	{	return _StatusMessage;	}
		}
		//************************************************************************************************/
		/// <summary> Property : Exception code for an exception thrown by this controller. </summary>
		/// <returns> Long. </returns>
		
		public long ExceptionCode
		{	
			get	{	return _ExceptionCode;	}
		}
		//************************************************************************************************/
		/// <summary> Function : DisplayIdleMessage (). </summary>
		/// <returns> None. </returns>
		
		public void DisplayIdleMessage ()
		{	// Set status message to "Idle".
			_StatusMessage = "Idle.";
		}
		//************************************************************************************************/
		/// <summary> Function : InitializeController (). </summary>

		public void InitializeController ()
		{	
			bool result = false;

			// Create and initialize the serial port for communicating with the RFID R/W.
			try
			{	// Create the status message.
				_StatusMessage = "Creating the serial port....";
				
				// Create event handler struct first -- Seagate.AAS.IO.SerialPorts\SerialBase.cs.
				_SerialPortEvents = new WithEvents ();
				// Create instance of serial port.
				_SerialPort = new SerialPort (_SerialPortEvents);

				// Configure the serial port settings (19200 bps, 8 data bits, 1 stop bit, no parity, no flow control).
				_SerialPort.Cnfg.BaudRate	= LineSpeed.Baud_19200;
				_SerialPort.Cnfg.DataBits	= ByteSize.Eight;
				_SerialPort.Cnfg.StopBits	= StopBits.One;
				_SerialPort.Cnfg.Parity		= Parity.None;
				_SerialPort.Cnfg.FlowCtrl	= Handshake.None;
				// Based on the location of this RFID R/W, pick appropriate COM port.
				switch (_RFIDType)
				{
					case RFIDType.HDAZone0		:	_COMPort = 3;	break;
					case RFIDType.HDAZone2		:	_COMPort = 4;	break;
					case RFIDType.TrayFeeder	:	_COMPort = 7;	break;
				}
				// Only if NOT in hardware simulation mode, interface with hardware.
				if (!_HWSimulationMode)
				{	// Update status message.	
					_StatusMessage = "Opening the COM port....";
					// Open (initialize) the serial port with the configuration settings.
					result = _SerialPort.Open (_COMPort);
					// If initialization failed or port is 'really' not open (redundant check), throw a Parsel exception.
					if (!result || !_SerialPort.IsOpen)
					{	// Based on the location of this RFID R/W, assign appropriate exception code.
						switch (_RFIDType)
						{
							case RFIDType.HDAZone0 :
								_ExceptionCode = ErrorCodes.HWCRFIDHDAZone0FailedToInitializeComPort;
								_ExceptionMessage = "HWCRFID R/W : HDA Zone-0 : ";
								break;
							case RFIDType.HDAZone2 :
								_ExceptionCode = ErrorCodes.HWCRFIDHDAZone2FailedToInitializeComPort;
								_ExceptionMessage = "HWCRFID R/W : HDA Zone-2 : ";
								break;
							case RFIDType.TrayFeeder :
								_ExceptionCode = ErrorCodes.HWCRFIDTrayFeederFailedToInitializeComPort;
								_ExceptionMessage = "HWCRFID R/W : Tray Feeder : ";
								break;
						}
						// Compose exception message with pre-defined error code in format 0x_____.
						_ExceptionMessage += "Failed to Initialize COM Port !!";
						_ExceptionMessage += " " + "Error Code : 0x" + _ExceptionCode.ToString ("Y").PadLeft (5, '0') + ".";
						// Throw ParselException.
						throw new ParselException (_ExceptionMessage);
					}
				}
				// Update status message.
				_StatusMessage = "COM Port initialized.";
			}
			// Catch any ParselExceptions thrown.  We probably just threw it. So just re-throw it.
			catch (ParselException exception)
			{
				throw exception;
			}
			// Catch any exceptions thrown.  Re-package it to ParselException object and throw exception.
			catch
			{	// Based on the location of this RFID R/W, assign appropriate exception code.
				switch (_RFIDType)
				{
					case RFIDType.HDAZone0 :
						_ExceptionCode = ErrorCodes.HWCRFIDHDAZone0FailedToInitializeComPort;
						_ExceptionMessage = "HWCRFID R/W : HDA Zone-0 : ";
						break;
					case RFIDType.HDAZone2 :
						_ExceptionCode = ErrorCodes.HWCRFIDHDAZone2FailedToInitializeComPort;
						_ExceptionMessage = "HWCRFID R/W : HDA Zone-2 : ";
						break;
					case RFIDType.TrayFeeder :
						_ExceptionCode = ErrorCodes.HWCRFIDTrayFeederFailedToInitializeComPort;
						_ExceptionMessage = "HWCRFID R/W : Tray Feeder : ";
						break;
				}
				// Compose exception message with pre-defined error code in format 0x_____.
				_ExceptionMessage += "Failed to Initialize COM Port !!";
				_ExceptionMessage += " " + "Error Code : 0x" + _ExceptionCode.ToString ("Y").PadLeft (5, '0') + ".";
				// Throw ParselException.
				throw new ParselException (_ExceptionMessage);
			}
		}
		//************************************************************************************************/
		/// <summary> Function : ReadTag (). </summary>

		private string ReadTag (int startAddress, int numberOfBytes)
		{
			string response = "";
			byte [] txmessage;
			byte [] rxmessage;
			byte [] extractedrxmessage;
			uint txresult = 0;
			uint rxresult = 0;
			int quotient = 0;
			int remainder = 0;
			int starttime = 0;
			// Response is "02 AA 05 00 XX 00 XX .... FF FF 03" if data is returned
			// Response is "02 AA FF 00 ## FF FF 03" if error reported. Error Code = ##
			uint numrxcharsexpected = 6 + (2 * (uint) numberOfBytes);

			// Update status message.
			_StatusMessage = "Reading the RFID tag [bytes " + startAddress.ToString () + "--";
			_StatusMessage += (startAddress + numberOfBytes).ToString () + "]....";
			
			// Only if NOT in hardware simulation mode and NOT in dry cycle mode, interface with hardware.
			if ( (!_HWSimulationMode) && (_RunMode != BUDWorkcell.StationRunMode.DryRun) )
			{
				// Command = "02 AA 05 00 00 00 02 0B B8 FF FF 03"
				txmessage = new byte [12];
				txmessage [0]  = 0x02;					// STX character
				txmessage [1]  = 0xAA;					// Start of Command Character
				txmessage [2]  = 0x05;					// Command Byte = 0x05 = Block Read
				// Calculate the MSB and LSB values for the start address (quotient & remainder of value / 256).
				quotient = Math.DivRem (startAddress, 256, out remainder);
				txmessage [3]  = (byte) quotient;		// Start Address MSB (integer division quotient)
				txmessage [4]  = (byte) remainder;		// Start Address LSB (integer division remainder)
				// Calculate the MSB and LSB values for the number of bytes (quotient & remainder of value / 256).
				quotient = Math.DivRem (numberOfBytes, 256, out remainder);					
				txmessage [5]  = (byte) quotient;		// Data Size MSB (integer division quotient)
				txmessage [6]  = (byte) remainder;		// Data Size LSB (integer division remainder)
				// Calculate the MSB and LSB values for the read time-out (quotient & remainder of value / 256).
				quotient = Math.DivRem (_RFIDCommunicationsTimeOut, 256, out remainder);					
				txmessage [7]  = (byte) quotient;		// Timeout MSB (integer division quotient)
				txmessage [8]  = (byte) remainder;		// Timeout LSB (integer division remainder)
				txmessage [9]  = 0xFF;					// End of Command Character
				txmessage [10] = 0xFF;					// End of Command Character
				txmessage [11] = 0x03;					// ETX character

				// Flush the serial port's Rx and Tx queues (for safety).
				_SerialPort.Flush ();

				// Send the byte array to the serial port. Returns the number of bytes sent.
				txresult = _SerialPort.Send (txmessage);
				// If transmission failed (no. of characters reported as transmitted is NOT the no.of characters 
				// we actually tried to send), throw new Parsel exception.
				if (txresult != txmessage.Length)
					throw new Exception ("Failed to transmit serial command !!");
		
				// Response is "02 AA 05 00 XX 00 XX .... FF FF 03" if data is returned
				// Response is "02 AA FF 00 ## FF FF 03" if error reported. Error Code = ##

				// Clock the start time of waiting on a response.
				starttime = GetTickCount ();
				// Peek at the number of bytes (using the Ready() method) in the port's receive buffer (do NOT remove them yet). 
				// If expected no.of characters has not been received, sleep.  Loop around until we got the count we expected, 
				// or we time out.
				do
				{	// Pause for 10 msec (Sleep present in System.Threading.Thread namespace).
					System.Threading.Thread.Sleep (10);
				}
				while ( (GetTickCount () - starttime <= _RFIDCommunicationsTimeOut) && (_SerialPort.Ready () < numrxcharsexpected) );

				// If number of bytes in the port's receive buffer is >= the number of characters we expect to receive,
				// proceed to extract the characters from the buffer to the byte array.
				// _SerialPort.Recv () should "in-theory" return the number of characters received.  But it is not reliable,
				// especially on multi-reads. Therefore use them only when absolutely necessary.
				if (_SerialPort.Ready () >= numrxcharsexpected)
					rxresult = _SerialPort.Recv (out rxmessage);
				else	// If we don't have expected number of characters, we probably timed out. Throw new Exception.
					throw new Exception ("Serial response timed-out !!");

				// Response is "02 AA 05 00 XX 00 XX .... FF FF 03" if data is returned.
				// Response is "02 AA FF 00 ## FF FF 03" if error reported. Error Code = ##.

				// If 2nd byte (0-based index is 0xFF, it is an error reported by the RFID R/W.
				// Decode error code at 4th byte (0-based index).
				if (rxmessage[2] == 0xFF)
				{	// Update status message.
					_StatusMessage = "Read tag failed !!";
					// Byte-4 (0-based index) will contain error code. Call function to decode it. 
					// Throw exception with translated error message.
					throw new Exception (DecodeErrorMessage (rxmessage [4]));
				}
				// If <ETX> is at the correct location (0-based index), extract the data read from the correct
				// locations in the rxmessage.
				else if (rxmessage [5 + (2 * numberOfBytes)] == 0x03)
				{	// Create a new byte array to hold the extracted RxD message.
					extractedrxmessage = new byte [numberOfBytes];
					int j = 0;    // Initialize j.
					// Iterate through raw data byte array and pull out LSB data byte values.
					for (int i = 4; i <= (numberOfBytes * 2) + 2; i = i+2)  // Byte 4,6,8,... (starting with 0)
					{	
						extractedrxmessage [j] = rxmessage [i];   // Extract read data from LSB's
						j++;                  // Increment counter for data char pointer
					}
					// Decode the received bytes that are ASCII encoded (0x30 for 0, 0x41 for A, etc.) to corresponding
					// string of characters using the ASCII encoder's GetString () function (System.Text.ASCIIEncoding namespace).
					response = _ASCIIEncoder.GetString (extractedrxmessage);
					// Update status message.
					_StatusMessage = "Read tag returned a response : " + response;
					// Return the decoded string.
					return response;
				}
				// If neither 0xFF was returned @ byte-2 (nor) <ETX> is where it is expected to be, response is corrupted.
				else
					throw new Exception ("Serial response corrupted !!");
			}
			// If in HW simulation mode, or in dry cycle mode....
			else
			{
				_StatusMessage += "Done.";
				return "Simulated.";
			}
		}
		//************************************************************************************************/
		/// <summary> Function : WriteTag (). </summary>

		private void WriteTag (int startAddress, int numberOfBytes, byte [] dataToWrite)
		{
			byte [] txmessage;
			byte [] rxmessage;
			uint txresult = 0;
			uint rxresult = 0;
			int quotient = 0;
			int remainder = 0;
			int starttime = 0;
			// Response is like "02 AA 06 FF FF 03" if write operation was successful.
			// Response is like "02 AA FF 00 ## FF FF 03" if error reported. Error Code = ##
			uint numrxcharsexpected = 6;

			// Update status message.
			_StatusMessage = "Writing the RFID tag [bytes " + startAddress.ToString () + "--";
			_StatusMessage += (startAddress + numberOfBytes).ToString () + "]....";
			
			// Only if NOT in hardware simulation mode and NOT in dry cycle mode, interface with hardware.
			if ( (!_HWSimulationMode) && (_RunMode != BUDWorkcell.StationRunMode.DryRun) )
			{
				// Command = "02 AA 06 00 04 0B B8 00 XX 00 XX ... FF FF 03"
				txmessage = new byte [12 + (2 * numberOfBytes)];
				txmessage [0]  = 0x02;					// STX character
				txmessage [1]  = 0xAA;					// Start of Command Character
				txmessage [2]  = 0x06;					// Command Byte = 0x06 = Block Write
				// Calculate the MSB and LSB values for the start address (quotient & remainder of value / 256).
				quotient = Math.DivRem (startAddress, 256, out remainder);
				txmessage [3]  = (byte) quotient;		// Start Address MSB (integer division quotient)
				txmessage [4]  = (byte) remainder;		// Start Address LSB (integer division remainder)
				// Calculate the MSB and LSB values for the number of bytes (quotient & remainder of value / 256).
				quotient = Math.DivRem (numberOfBytes, 256, out remainder);					
				txmessage [5]  = (byte) quotient;		// Data Size MSB (integer division quotient)
				txmessage [6]  = (byte) remainder;		// Data Size LSB (integer division remainder)
				// Calculate the MSB and LSB values for the read time-out (quotient & remainder of value / 256).
				quotient = Math.DivRem (_RFIDCommunicationsTimeOut, 256, out remainder);					
				txmessage [7]  = (byte) quotient;		// Timeout MSB (integer division quotient)
				txmessage [8]  = (byte) remainder;		// Timeout LSB (integer division remainder)

				// For data, 0x00 is in MSB, Data is in LSB
				for (int i = 0; i < numberOfBytes; i++)
				{
					txmessage [9  + (i * 2)] = 0x00;			// MSB . Bytes 9, 11, 13,...
					txmessage [10 + (i * 2)] = dataToWrite [i];	// LSB with data from byte [].  Bytes 10, 12, 14,...
				}

				// Follow it up with "FF FF 0x03"
				txmessage [9  + (numberOfBytes * 2)] = 0xFF;	// End of Command Character
				txmessage [10 + (numberOfBytes * 2)] = 0xFF;	// End of Command Character
				txmessage [11 + (numberOfBytes * 2)] = 0x03;	// ETX character

				// Flush the serial port's Rx and Tx queues (for safety).
				_SerialPort.Flush ();

				// Send the byte array to the serial port. Returns the number of bytes sent.
				txresult = _SerialPort.Send (txmessage);
				// If transmission failed (no. of characters reported as transmitted is NOT the no.of characters 
				// we actually tried to send), throw new Parsel exception.
				if (txresult != txmessage.Length)
					throw new Exception ("Failed to transmit serial command !!");
		
				// Response is like "02 AA 06 FF FF 03" if write operation was successful.
				// Response is like "02 AA FF 00 ## FF FF 03" if error reported. Error Code = ##

				// Clock the start time of waiting on a response.
				starttime = GetTickCount ();
				// Peek at the number of bytes (using the Ready() method) in the port's receive buffer (do NOT remove them yet). 
				// If expected no.of characters has not been received, sleep.  Loop around until we got the count we expected, 
				// or we time out.
				do
				{	// Pause for 10 msec (Sleep present in System.Threading.Thread namespace).
					System.Threading.Thread.Sleep (10);
				}
				while ( (GetTickCount () - starttime <= _RFIDCommunicationsTimeOut) && (_SerialPort.Ready () < numrxcharsexpected) );

				// If number of bytes in the port's receive buffer is >= the number of characters we expect to receive,
				// proceed to extract the characters from the buffer to the byte array.
				// _SerialPort.Recv () should "in-theory" return the number of characters received.  But it is not reliable,
				// especially on multi-reads. Therefore use them only when absolutely necessary.
				if (_SerialPort.Ready () >= numrxcharsexpected)
					rxresult = _SerialPort.Recv (out rxmessage);
				else	// If we don't have expected number of characters, we probably timed out. Throw new Exception.
					throw new Exception ("Serial response timed-out !!");

				// Response is like "02 AA 06 FF FF 03" if write operation was successful.
				// Response is like "02 AA FF 00 ## FF FF 03" if error reported. Error Code = ##
				// If 2nd byte (0-based index is 0xFF, it is an error reported by the RFID R/W.
				// Decode error code at 4th byte (0-based index).
				if (rxmessage[2] == 0xFF)
				{	// Update status message.
					_StatusMessage = "Write tag failed !!";
					// Byte-4 (0-based index) will contain error code. Call function to decode it. 
					// Throw exception with translated error message.
					throw new Exception (DecodeErrorMessage (rxmessage [4]));
				}
				// If 0x06 is at byte-2 (0-based index), write operation was successful.
				else if (rxmessage [2] == 0x06)
				{	// Update status message.
					_StatusMessage = "Write tag operation successful.";
				}
				// If neither 0xFF was returned @ byte-2 (nor) 0x06 was returned @ byte-2, response is corrupted.
				else
					throw new Exception ("Serial response corrupted !!");
			}
			// If in HW simulation mode, or in dry cycle mode....
			else
			{
				_StatusMessage += "Done.";
			}
		}
		//************************************************************************************************/
		/// <summary> Function : ClearTag (). </summary>

		public void ClearTag (int startAddress, int numberOfBytes)
		{
			byte [] txmessage;
			byte [] rxmessage;
			uint txresult = 0;
			uint rxresult = 0;
			int quotient = 0;
			int remainder = 0;
			int starttime = 0;
			// Response is like "02 AA 04 FF FF 03" if fill operation was successful.
			// Response is like "02 AA FF 00 ## FF FF 03" if error reported. Error Code = ##
			uint numrxcharsexpected = 6;

			// Update status message.
			_StatusMessage = "Clearing the RFID tag [bytes " + startAddress.ToString () + "--";
			_StatusMessage += (startAddress + numberOfBytes).ToString () + "]....";
			
			// Only if NOT in hardware simulation mode and NOT in dry cycle mode, interface with hardware.
			if ( (!_HWSimulationMode) && (_RunMode != BUDWorkcell.StationRunMode.DryRun) )
			{
				// Command = "02 AA 04 00 04 00 10 0B B8 00 XX FF FF 03"
				txmessage = new byte [14];
				txmessage [0]  = 0x02;					// STX character
				txmessage [1]  = 0xAA;					// Start of Command Character
				txmessage [2]  = 0x04;					// Command Byte = 0x04 = Fill tag
				// Calculate the MSB and LSB values for the start address (quotient & remainder of value / 256).
				quotient = Math.DivRem (startAddress, 256, out remainder);
				txmessage [3]  = (byte) quotient;		// Start Address MSB (integer division quotient)
				txmessage [4]  = (byte) remainder;		// Start Address LSB (integer division remainder)
				// Calculate the MSB and LSB values for the number of bytes (quotient & remainder of value / 256).
				quotient = Math.DivRem (numberOfBytes, 256, out remainder);					
				txmessage [5]  = (byte) quotient;		// Data Size MSB (integer division quotient)
				txmessage [6]  = (byte) remainder;		// Data Size LSB (integer division remainder)
				// Calculate the MSB and LSB values for the read time-out (quotient & remainder of value / 256).
				quotient = Math.DivRem (_RFIDCommunicationsTimeOut, 256, out remainder);					
				txmessage [7]  = (byte) quotient;		// Timeout MSB (integer division quotient)
				txmessage [8]  = (byte) remainder;		// Timeout LSB (integer division remainder)
				// Follow it up with data to be filled.
				txmessage [9]  = 0x00;					// Data to be filled MSB.
				txmessage [10] = 0x00;					// Data to be filled LSB. 0x00 = ASCII for NULL.
				// Follow it up with "FF FF 0x03"
				txmessage [11] = 0xFF;					// End of Command Character
				txmessage [12] = 0xFF;					// End of Command Character
				txmessage [13] = 0x03;					// ETX character

				// Flush the serial port's Rx and Tx queues (for safety).
				_SerialPort.Flush ();

				// Send the byte array to the serial port. Returns the number of bytes sent.
				txresult = _SerialPort.Send (txmessage);
				// If transmission failed (no. of characters reported as transmitted is NOT the no.of characters 
				// we actually tried to send), throw new Parsel exception.
				if (txresult != txmessage.Length)
					throw new Exception ("Failed to transmit serial command !!");
		
				// Response is like "02 AA 04 FF FF 03" if write operation was successful.
				// Response is like "02 AA FF 00 ## FF FF 03" if error reported. Error Code = ##

				// Clock the start time of waiting on a response.
				starttime = GetTickCount ();
				// Peek at the number of bytes (using the Ready() method) in the port's receive buffer (do NOT remove them yet). 
				// If expected no.of characters has not been received, sleep.  Loop around until we got the count we expected, 
				// or we time out.
				do
				{	// Pause for 10 msec (Sleep present in System.Threading.Thread namespace).
					System.Threading.Thread.Sleep (10);
				}
				while ( (GetTickCount () - starttime <= _RFIDCommunicationsTimeOut) && (_SerialPort.Ready () < numrxcharsexpected) );

				// If number of bytes in the port's receive buffer is >= the number of characters we expect to receive,
				// proceed to extract the characters from the buffer to the byte array.
				// _SerialPort.Recv () should "in-theory" return the number of characters received.  But it is not reliable,
				// especially on multi-reads. Therefore use them only when absolutely necessary.
				if (_SerialPort.Ready () >= numrxcharsexpected)
					rxresult = _SerialPort.Recv (out rxmessage);
				else	// If we don't have expected number of characters, we probably timed out. Throw new Exception.
					throw new Exception ("Serial response timed-out !!");

				// Response is like "02 AA 04 FF FF 03" if write operation was successful.
				// Response is like "02 AA FF 00 ## FF FF 03" if error reported. Error Code = ##
				// If 2nd byte (0-based index is 0xFF, it is an error reported by the RFID R/W.
				// Decode error code at 4th byte (0-based index).
				if (rxmessage[2] == 0xFF)
				{	// Update status message.
					_StatusMessage = "Write tag failed !!";
					// Byte-3 (0-based index) will contain error code. Call function to decode it. 
					// Throw exception with translated error message.
					throw new Exception (DecodeErrorMessage (rxmessage [4]));
				}
				// If 0x04 is at byte-2 (0-based index), write operation was successful.
				else if (rxmessage [2] == 0x04)
				{	// Update status message.
					_StatusMessage = "Write tag operation successful.";
				}
				// If neither 0xFF was returned @ byte-2 (nor) 0x04 was returned @ byte-2, response is corrupted.
				else
					throw new Exception ("Serial response corrupted !!");
			}
			// If in HW simulation mode, or in dry cycle mode....
			else
			{
				_StatusMessage += "Done.";
			}
		}
		//************************************************************************************************/
		/// <summary> Function : ReadTagID (). </summary>

		public string ReadTagID ()
		{
			string response = "";
			byte [] txmessage;
			byte [] rxmessage;
			byte [] extractedrxmessage;
			uint txresult = 0;
			uint rxresult = 0;
			int quotient = 0;
			int remainder = 0;
			int starttime = 0;
			// Response is "02 AA 07 00 XX 00 XX 00	XX 00 XX FF FF 03" if data is returned
			// Response is "02 AA FF 00 ## FF FF 03" if error reported. Error Code = ##
			uint numrxcharsexpected = 14;

			try
			{	// Update status message.
				_StatusMessage = "Reading the RFID tag ID....";
			
				// Only if NOT in hardware simulation mode and NOT in dry cycle mode, interface with hardware.
				if ( (!_HWSimulationMode) && (_RunMode != BUDWorkcell.StationRunMode.DryRun) )
				{
					// Command = "02 AA 07 0B B8 FF FF 03"
					txmessage = new byte [8];
					txmessage [0] = 0x02;					// STX character
					txmessage [1] = 0xAA;					// Start of Command Character
					txmessage [2] = 0x07;					// Command Byte = 0x07 = Tag ID read.
					// Calculate the MSB and LSB values for the read time-out (quotient & remainder of value / 256).
					quotient = Math.DivRem (_RFIDCommunicationsTimeOut, 256, out remainder);					
					txmessage [3] = (byte) quotient;		// Timeout MSB (integer division quotient)
					txmessage [4] = (byte) remainder;		// Timeout LSB (integer division remainder)
					txmessage [5] = 0xFF;					// End of Command Character
					txmessage [6] = 0xFF;					// End of Command Character
					txmessage [7] = 0x03;					// ETX character

					// Flush the serial port's Rx and Tx queues (for safety).
					_SerialPort.Flush ();

					// Send the byte array to the serial port. Returns the number of bytes sent.
					txresult = _SerialPort.Send (txmessage);
					// If transmission failed (no. of characters reported as transmitted is NOT the no.of characters 
					// we actually tried to send), throw new Parsel exception.
					if (txresult != txmessage.Length)
						throw new Exception ("Failed to transmit serial command !!");
		
					// Response is "02 AA 07 00 XX 00 XX 00	XX 00 XX FF FF 03" if data is returned
					// Response is "02 AA FF 00 ## FF FF 03" if error reported. Error Code = ##

					// Clock the start time of waiting on a response.
					starttime = GetTickCount ();
					// Peek at the number of bytes (using the Ready() method) in the port's receive buffer (do NOT remove them yet). 
					// If expected no.of characters has not been received, sleep.  Loop around until we got the count we expected, 
					// or we time out.
					do
					{	// Pause for 10 msec (Sleep present in System.Threading.Thread namespace).
						System.Threading.Thread.Sleep (10);
					}
					while ( (GetTickCount () - starttime <= _RFIDCommunicationsTimeOut) && (_SerialPort.Ready () < numrxcharsexpected) );

					// If number of bytes in the port's receive buffer is >= the number of characters we expect to receive,
					// proceed to extract the characters from the buffer to the byte array.
					// _SerialPort.Recv () should "in-theory" return the number of characters received.  But it is not reliable,
					// especially on multi-reads. Therefore use them only when absolutely necessary.
					if (_SerialPort.Ready () >= numrxcharsexpected)
						rxresult = _SerialPort.Recv (out rxmessage);
					else	// If we don't have expected number of characters, we probably timed out. Throw new Exception.
						throw new Exception ("Serial response timed-out !!");

					// Response is "02 AA 07 00 XX 00 XX 00	XX 00 XX FF FF 03" if data is returned
					// Response is "02 AA FF 00 ## FF FF 03" if error reported. Error Code = ##

					// If 2nd byte (0-based index is 0xFF, it is an error reported by the RFID R/W.
					// Decode error code at 4th byte (0-based index).
					if (rxmessage[2] == 0xFF)
					{	// Update status message.
						_StatusMessage = "Read tag failed !!";
						// Byte-4 (0-based index) will contain error code. Call function to decode it. 
						// Throw exception with translated error message.
						throw new Exception (DecodeErrorMessage (rxmessage [4]));
					}
					// If <ETX> is at the correct location (0-based index), extract the data read from the correct
					// locations in the rxmessage.
					else if (rxmessage [13] == 0x03)
					{	// Create a new byte array to hold the extracted RxD message.
						extractedrxmessage = new byte [4];
						int j = 0;    // Initialize j.
						// Iterate through raw data byte array and pull out LSB data byte values.
						for (int i = 4; i <= 10; i = i+2)  // Byte 4,6,8,10. (starting with 0)
						{	
							extractedrxmessage [j] = rxmessage [i];   // Extract read data from LSB's
							j++;                  // Increment counter for data char pointer
						}
						// Decode the received bytes that are ASCII encoded (0x30 for 0, 0x41 for A, etc.) to corresponding
						// string of characters using the ASCII encoder's GetString () function (System.Text.ASCIIEncoding namespace).
						response = _ASCIIEncoder.GetString (extractedrxmessage);
						// Update status message.
						_StatusMessage = "Read tag ID returned a response : " + response;
						// Return the decoded string.
						return response;
					}
					// If neither 0xFF was returned @ byte-2 (nor) <ETX> is where it is expected to be, response is corrupted.
					else
						throw new Exception ("Serial response corrupted !!");
				}
				// If in HW simulation mode, or in dry cycle mode....
				else
				{
					_StatusMessage += "Done.";
					return "Simulated.";
				}
			}
			// Catch any exceptions thrown.  Re-package it to ParselException object and throw exception.
			catch (Exception exception)
			{	// Based on the location of this RFID R/W, assign appropriate exception code.
				switch (_RFIDType)
				{
					case RFIDType.HDAZone0 :
						_ExceptionCode = ErrorCodes.HWCRFIDHDAZone0FailedToReadTagID;
						_ExceptionMessage = "HWCRFID R/W : HDA Zone-0 : ";
						break;
					case RFIDType.HDAZone2 :
						_ExceptionCode = ErrorCodes.HWCRFIDHDAZone2FailedToReadTagID;
						_ExceptionMessage = "HWCRFID R/W : HDA Zone-2 : ";
						break;
					case RFIDType.TrayFeeder :
						_ExceptionCode = ErrorCodes.HWCRFIDTrayFeederFailedToReadTagID;
						_ExceptionMessage = "HWCRFID R/W : HDA Zone-0 : ";
						break;
				}
				// Compose exception message with pre-defined error code in format 0x_____.
				_ExceptionMessage += "Failed to read RFID tag ID !! ";
				_ExceptionMessage += exception.Message;
				_ExceptionMessage += " " + "Error Code : 0x" + _ExceptionCode.ToString ("Y").PadLeft (5, '0') + ". ";
				// Throw ParselException.
				throw new ParselException (_ExceptionMessage);
			}
		}
		//************************************************************************************************/
		/// <summary> Function : DecodeErrorMessage (). </summary>

		private string DecodeErrorMessage (byte errorCode)
		{
			string errorMessage = "";
			// Based on error code the RFID R/W hardware returned, compose error message.
			switch (errorCode)
			{
				case 0x01	:	errorMessage = "RFID H/W Error : Non-contiguous read failed.";							break;
				case 0x02	:	errorMessage = "RFID H/W Error : Non-contiguous write failed.";							break;
				case 0x03	:	errorMessage = "RFID H/W Error : Non-contiguous read / write configuration failed.";	break;
				case 0x04	:	errorMessage = "RFID H/W Error : Fill operation failed.";								break;
				case 0x05	:	errorMessage = "RFID H/W Error : Block read operation failed.";							break;
				case 0x06	:	errorMessage = "RFID H/W Error : Block write operation failed.";						break;
				case 0x08	:	errorMessage = "RFID H/W Error : Search tag operation failed.";							break;
				case 0x19	:	errorMessage = "RFID H/W Error : Protected address access violation.";					break;
				case 0x20	:	errorMessage = "RFID H/W Error : Non-contiguous read/write without configuration.";		break;
				case 0x21	:	errorMessage = "RFID H/W Error : Input command does not match format.";					break;
				default		:	errorMessage = "RFID H/W Error : Unmatched error code.";								break;
			}

			return errorMessage;	// Return error message.
		}
		//************************************************************************************************/
		/// <summary> Function : ReadHDATagInformation (). </summary>

		public void ReadHDATagInformation (HDADataDefinition dataDefinition)
		{
			try
			{	// Update status message.
				_StatusMessage = "Reading HDA process information....";

				// Read the process information from the RFID tag.
				// HDA Serial Number :	020 - 027. (8 bytes).
				// HDA Part Number :	028 - 036. (9 bytes).
				// Sub Build Group :	037 - 047. (11 bytes).
				// Process Code :		048 - 103. (56 bytes).
				// HDA SN Prefix :		104 - 105. (2 bytes).
				// Fail Code :			106 - 109. (4 bytes).
				string processInformation = ReadTag (_DriveTagHDASNStartAddress, 90);

				// Extract the information from the response.(Args = Starting char index (0-based), length).
				// HDA Serial Number -- RFID memory : 020 - 027. (8 bytes) -- String chars : 000 - 007.
				dataDefinition.HDASerialNumber = processInformation.Substring (0, 8);
				// HDA Part Number -- RFID memory : 028 - 036. (9 bytes) -- String chars : 008 - 016.
				dataDefinition.HDAPartNumber = processInformation.Substring (8, 9);
				// Process Code -- RFID memory : 048 - 103. (56 bytes) -- String chars : 028 - 083.
				_ProcessCode = processInformation.Substring (28, 56);
				// HDA SN Prefix -- RFID memory : 104 - 105. (2 bytes) -- String chars : 084 - 085.
				dataDefinition.HDASerialNumberPrefix = processInformation.Substring (84, 2);
				// Fail Code -- RFID memory : 106 - 109. (4 bytes) -- String chars : 086 - 089. Convert to integer.
				dataDefinition.FailCode = Int32.Parse (processInformation.Substring (86, 4));

				// If the first '1' in the process code string is NOT this station, it indicates an upstream
				// pending process. Do not process this drive.
				if (_ProcessCode.IndexOf ('1') != _Setup.StationNumber)
				{	// Update status message.
					_StatusMessage = "Process code shows an upstream pending process. Skip drive.";
					// Tag the process state of the data definition object to "bypass drive".
					dataDefinition.ProcessState = HDADataDefinition.ProcessStates.BypassDrive;
					return;		// Flag that this drive is NOT to be processed.
				}
				// If the first '1' in the process code string is this station, check to see if the fail code
				// is 0000. If not, it indicates a failed process (possibly by BUD station). Do not process this drive.
				if (dataDefinition.FailCode != 0000)
				{	// Update status message.
					_StatusMessage = "Fail code shows a failed process. Skip drive.";
					// Tag the process state of the data definition object to "bypass drive".
					dataDefinition.ProcessState = HDADataDefinition.ProcessStates.BypassDrive;
					return;		// Flag that this drive is NOT to be processed.
				}
				
				// Update status message.
				_StatusMessage = "Reading the HSA configuration information....";
				// Read the HSA configuration information -- RFID memory : 132 - 143 (12 bytes).
				dataDefinition.HSAConfiguration = ReadTag (_DriveTagHSAConfigStartAddress, 12);

				// Tag the process state of the data definition object to "process drive".
				dataDefinition.ProcessState = HDADataDefinition.ProcessStates.ProcessDrive;

				// Update status message.
				_StatusMessage = "HDA is due for process at this station.";

				// Return true to flag that the HDA is due for process at this station.
				return;
			}
			// Catch any ParselExceptions thrown.  We probably just threw it. So just re-throw it.
			catch (ParselException exception)
			{
				throw exception;
			}
			// Catch any exceptions thrown.  Re-package it to ParselException object and throw exception.
			catch (Exception exception)
			{	// Based on the location of this RFID R/W, assign appropriate exception code.
				switch (_RFIDType)
				{
					case RFIDType.HDAZone0 :
						_ExceptionCode = ErrorCodes.HWCRFIDHDAZone0FailedToReadProcessInformation;
						_ExceptionMessage = "HWCRFID R/W : HDA Zone-0 : ";
						break;
					case RFIDType.HDAZone2 :
						_ExceptionCode = ErrorCodes.HWCRFIDHDAZone2FailedToReadProcessInformation;
						_ExceptionMessage = "HWCRFID R/W : HDA Zone-2 : ";
						break;
				}
				// Compose exception message with pre-defined error code in format 0x_____.
				_ExceptionMessage += "Failed to read HDA process information !! ";
				_ExceptionMessage += exception.Message;
				_ExceptionMessage += " " + "Error Code : 0x" + _ExceptionCode.ToString ("Y").PadLeft (5, '0') + ". ";
				// Throw ParselException.
				throw new ParselException (_ExceptionMessage);
			}
		}
		//************************************************************************************************/
		/// <summary> Function : WriteHDATagInformation (). </summary>

		public void WriteHDATagInformation (HDADataDefinition dataDefinition)
		{
			System.Text.ASCIIEncoding asciiEncoder = new System.Text.ASCIIEncoding ();
			try
			{	// Update status message.
				_StatusMessage = "Verifying HDA serial number information....";
				
				// Since zone-2 is going to be writing to the RFID tag, we need to make sure that the data
				// we are writing is consistent with what we expect. In other words, zone-1 will process the HDA,
				// and release the HDA to zone-2. It will also pass a reference to the data definition object.
				// Need to verify that the data and the HDA match (use the HDA serial number information for that).
				// Read the process information from the RFID tag.
				// HDA Serial Number :	020 - 027. (8 bytes).
				string hdaserialnumber = ReadTag (_DriveTagHDASNStartAddress, 8);
				// If HDA serial numbers don't match between the data definition object and physical HDA, throw exception.
				if (hdaserialnumber != dataDefinition.HDASerialNumber)
				{	// Compose exception message with pre-defined error code in format 0x_____.
					_ExceptionCode = ErrorCodes.HWCRFIDHDAZone2DataInconsistencyFailure;
					_ExceptionMessage = "HWCRFID R/W : HDA Zone-2 : ";
					_ExceptionMessage += "HDA serial number of data collection object does not match that of HDA at zone-2 !!";
					_ExceptionMessage += " Data collection object : " + dataDefinition.HDASerialNumber;
					_ExceptionMessage += ". HDA @ zone-2 : " + hdaserialnumber + ".";
					_ExceptionMessage += " " + "Error Code : 0x" + _ExceptionCode.ToString ("Y").PadLeft (5, '0') + ". ";
					// Throw ParselException.
					throw new ParselException (_ExceptionMessage);
				}

				_StatusMessage = "Writing the process information to the HDA....";
				// If our internal fail code is 0000, it is a passed drive. Mark "X" at the process byte for the station.
				if (dataDefinition.FailCode == 0000)
				{	// Write "X" at process byte for station.
					// Args = Start address, number of bytes to write, data -- convert from string to ascii encoded byte array.
					WriteTag (_DriveTagProcessCodeStartAddress + _Setup.StationNumber - 1, 1, asciiEncoder.GetBytes ("X"));
				}
				// If our internal fail code is NOT 0000, it is a failed drive. Mark "1" at the process byte for the station.
				else
				{	// Write "1" at process byte for station.
					// Args = Start address, number of bytes to write, data -- convert from string to ascii encoded byte array.
					WriteTag (_DriveTagProcessCodeStartAddress + _Setup.StationNumber - 1, 1, asciiEncoder.GetBytes ("1"));
					// Write the fail code at the correct location on the RFID tag.
					WriteTag (_DriveTagFailCodeStartAddress, 4, asciiEncoder.GetBytes (dataDefinition.FailCode.ToString ()));
				}

				_StatusMessage = "Writing the HSA attribute information to the HDA....";
				// Compose HSA attribute information (about the HSA that was installed in the HDA) -- read from
				// the tray RFID tag and written to the HDA RFID tag.
				string hsaInformation = "";
				hsaInformation = dataDefinition.HSASerialNumber;
				hsaInformation += dataDefinition.PreAmpVendorCode;
				hsaInformation += dataDefinition.PreAmpDateCode;
				hsaInformation += dataDefinition.PreAmpRevisionNumber;
				hsaInformation += dataDefinition.HSAPartNumber;
				hsaInformation += dataDefinition.HSARevisionCode;
				hsaInformation += dataDefinition.HSAManufacturerCode;
				hsaInformation += dataDefinition.HSAManufactureWorkWeek;
				hsaInformation += dataDefinition.HGAVendorCode;

				// Write the HSA part information on the HDA RFID tag using the string compiled above.
				// Args = Start address, number of bytes to write, data -- convert from string to ascii encoded byte array.
				WriteTag (_DriveTagHSAAttributeInfoStartAddress, 28, asciiEncoder.GetBytes (hsaInformation));

				_StatusMessage = "Writing the station history information to the HDA....";
				// Create a new byte array for storing station history information -- machine time, convey time,
				// operation code, tool code.
				byte [] stationHistory = new byte [4];

				// Compose byte-coded machine time information. Multiply by 10, round to integer value between 0..255.
				// Ex : 4.3 sec = 43; 27.3 sec = 255 (limited).
				byte machinetime = (byte) Math.Round ( (dataDefinition.MachineTime * 10.0), 0);
				// Limit between range 0..255 (byte strength).
				if (machinetime < 0)
					machinetime = 0;
				else if (machinetime > 255)
					machinetime = 255;
				// Copy information to station history byte array.
				stationHistory [0] = machinetime;

				// Compose byte-coded convey time information. Multiply by 10, round to integer value between 0..255.
				// Ex : 4.3 sec = 43; 27.3 sec = 255 (limited).
				byte conveytime = (byte) Math.Round ( (dataDefinition.ConveyTime * 10.0), 0);
				// Limit between range 0..255 (byte strength).
				if (conveytime < 0)
					conveytime = 0;
				else if (conveytime > 255)
					conveytime = 255;
				// Copy information to station history byte array.
				stationHistory [2] = conveytime;

				// Compose byte-coded operation code information -- integer value between 0..255.
				// Copy information to station history byte array.
				stationHistory [1] = dataDefinition.OperationCode;
				// Compose byte-coded station special code information -- integer value between 0..255.
				// Copy information to station history byte array.
				stationHistory [3] = dataDefinition.SpecialCode;

				// Write the station history information on the HDA RFID tag using the byte array compiled above.
				// Args = Start address, number of bytes to write, data -- convert from string to ascii encoded byte array.
				// Byte 448-451 = Station-1, 452-455 = Station-2,....
				WriteTag (_DriveTagStationHistoryStartAddress + ((_Setup.StationNumber - 1) * 4), 4, stationHistory);

				// Update status message.
				_StatusMessage = "HDA RFID tag has been successfully written.";

				return;
			}
			// Catch any ParselExceptions thrown.  We probably just threw it. So just re-throw it.
			catch (ParselException exception)
			{
				throw exception;
			}
			// Catch any exceptions thrown.  Re-package it to ParselException object and throw exception.
			catch (Exception exception)
			{	// Assign exception code.
				_ExceptionCode = ErrorCodes.HWCRFIDHDAZone2FailedToWriteProcessInformation;
				_ExceptionMessage = "HWCRFID R/W : HDA Zone-2 : ";
				// Compose exception message with pre-defined error code in format 0x_____.
				_ExceptionMessage += "Failed to write HDA information to HDA RFID tag !!";
				_ExceptionMessage += exception.Message;
				_ExceptionMessage += " " + "Error Code : 0x" + _ExceptionCode.ToString ("Y").PadLeft (5, '0') + ". ";
				// Throw ParselException.
				throw new ParselException (_ExceptionMessage);
			}
		}
		//************************************************************************************************/
		/// <summary> Function : ReadTrayTagInformation (). </summary>

		public void ReadTrayTagInformation ()
		{
			int currentpartindex = 0;
			int startaddress = 0;
			int numberofbytes = 0;
			int numberofpartstoread = 0;
			string hsaTrayData = "";

			try
			{	// Update status message.
				_StatusMessage = "Reading HSA tray tag information....";

				// Read the HSA part information from the tray RFID tag.
				// HSA Serial Number			: 01 - 09.	(9 bytes).
				// Pre-amp Vendor Code			: 10.		(1 byte).
				// Pre-amp Date Code			: 11 - 12.	(2 bytes).
				// Pre-amp Revision Code		: 13 - 14.	(2 bytes).
				// HSA Part Number				: 15 - 23.	(9 bytes).
				// HSA Revision Code			: 24.		(1 byte).
				// HSA Manufacturer Code		: 25.		(1 byte).
				// Seagate Manufacture Workweek	: 26 - 27.	(2 bytes).
				// HGA Vendor Code				: 28.		(1 byte).
				// Spares						: 29 - 32.	(4 bytes).
				// HSA part attribute information starts @ byte 5 for the first part.

				// Initialize index of part information being read (0th HSA).
				currentpartindex = 0;
				// While not done reading information for number of parts in tray....
				while (currentpartindex < _TrayTagHSAInfoNumberOfParts)
				{	// Clear string.
					hsaTrayData = "";
					// Calculate start address.
					startaddress = _TrayTagHSAInfoStartAddress + (currentpartindex * _TrayTagHSAInfoBytesPerPart);
					// RFID R/W can only read a maximum of 100 bytes per read. Assuming ~30 bytes per HSA, we can read
					// a maximum of 3 HSA's each time. If there are less than 3 parts remaining to be read, go ahead and
					// read all of the remaining.
					if ( (_TrayTagHSAInfoNumberOfParts - currentpartindex) > 3)
						numberofpartstoread = 3;
                    else
						numberofpartstoread = _TrayTagHSAInfoNumberOfParts - currentpartindex;
					// Calculate the number of bytes to be read based on the number of parts to be read.
					numberofbytes = _TrayTagHSAInfoBytesPerPart * numberofpartstoread;
					// Read the HSA RFID tag data (Args = start address, number of bytes).
					hsaTrayData = ReadTag (startaddress, numberofbytes);

					// Extract the information from the response.(Substring Args = Starting char index (0-based), length).
					// hsaTrayData will contain information on "numberofpartstoread" HSA's. We have to parse it.
					// Do a for loop....currentpartindex = currentpartindex; currentpartindex < what we have read so far; currentpartindex++.
					for (; currentpartindex <= currentpartindex + numberofpartstoread; currentpartindex++)
					{	// Extract the information into the static struct array. Substring args = starting char index (0-based), length.
						TrayHSAAttribute [currentpartindex].HSASerialNumber			= hsaTrayData.Substring ( 0, 9);
						TrayHSAAttribute [currentpartindex].PreAmpVendorCode		= hsaTrayData.Substring ( 9, 1);
						TrayHSAAttribute [currentpartindex].PreAmpDateCode			= hsaTrayData.Substring (10, 2);
						TrayHSAAttribute [currentpartindex].PreAmpRevisionNumber	= hsaTrayData.Substring (12, 2);
						TrayHSAAttribute [currentpartindex].HSAPartNumber			= hsaTrayData.Substring (14, 9);
						TrayHSAAttribute [currentpartindex].HSARevisionCode			= hsaTrayData.Substring (23, 1);
						TrayHSAAttribute [currentpartindex].HSAManufacturerCode		= hsaTrayData.Substring (24, 1);
						TrayHSAAttribute [currentpartindex].HSAManufactureWorkWeek	= hsaTrayData.Substring (25, 2);
						TrayHSAAttribute [currentpartindex].HGAVendorCode			= hsaTrayData.Substring (27, 1);
						// To keep the math simple for the next part, go ahead and strip the information for the HSA that
						// was just read and discard it. "Remove args" = starting char index (0-based), no.of chars.
						hsaTrayData.Remove (0, 32);
					}
				}

				// Update status message.
				_StatusMessage = "HSA tray tag information read successfully !!";
				return;		// Return.
			}
			// Catch any exceptions thrown.  Re-package it to ParselException object and throw exception.
			catch (Exception exception)
			{	// Compose exception message with pre-defined error code in format 0x_____.
				_ExceptionCode = ErrorCodes.HWCRFIDTrayFeederFailedToReadHSAAttributeInformation;
				_ExceptionMessage = "HWCRFID R/W : Tray RFID R/W : Failed to read tray attribute information !! ";
				_ExceptionMessage += exception.Message;
				_ExceptionMessage += " " + "Error Code : 0x" + _ExceptionCode.ToString ("Y").PadLeft (5, '0') + ". ";
				// Throw ParselException.
				throw new ParselException (_ExceptionMessage);
			}
		}
		//************************************************************************************************/
	}	// End of Class -- HWCRFIDRW

	//************************************************************************************************/

}	// End of Namespace -- BUD.

//************************************************************************************************/
