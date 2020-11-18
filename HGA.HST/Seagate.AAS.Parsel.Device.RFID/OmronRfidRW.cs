//************************************************************************************************/
//						SEAGATE FACTORY OF THE FUTURE AUTOMATION.
//						DEVELOPMENT ENGINEER : SANJAY SUNDARESAN.
//						Modified by : Sabrina Murray
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

namespace Seagate.AAS.Parsel.Device.RFID
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
		private bool _HWSimulationMode = false;
		private string _StatusMessage = "";
		private string _ExceptionMessage = "";
		private long _ExceptionCode = 0x0;
		private HWCRFIDRWSetup _Setup = null;
		
		// Hardware controller specific fields.
		private int _COMPort = 0;
		private SerialPort _SerialPort = null;
		private WithEvents _SerialPortEvents;
		private System.Text.ASCIIEncoding _ASCIIEncoder = new System.Text.ASCIIEncoding ();
		private string _ProcessCode = "";
		private bool _isInitialized = false;
		
		// I/O/S Map.
	
		//************************************************************************************************/
		// Declare all positions (points table) here.

//		private int _Setup.StationNumber = 32;

		//************************************************************************************************/
		// Declare all constants here.
		
		private const int _RFIDCommunicationsTimeOut = 5000;	// in msec.
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
		
		public void Dispose()
		{	// If serial port is open, close it first.
			if( _SerialPort != null )
			{
				if (_SerialPort.IsOpen)
					_SerialPort.Close ();
			}
			
			// De-allocate all memory allocated from the heap (with the "new" keyword).
			// The .NET garbage collector will purge it back to the stack and heap.				
			_SerialPort = null;
			_ASCIIEncoder = null;
			_Setup = null;

			// Call the .NET garbage collector to purge memory back to stack and heap.
			GC.Collect ();
		}
		//************************************************************************************************/
		/// <summary> Property : HW Simulation Mode. </summary>
		
		public bool HWSimulationMode
		{
			set	{	_HWSimulationMode = value;	}
		}
		//************************************************************************************************/
		/// <summary> Property : Is tag reader initialized. </summary>
		
		public bool IsInitialized
		{
			get	{	return _isInitialized;	}
		}
		//************************************************************************************************/
		/// <summary> Property : Status message of this controller. </summary>
		/// <returns> String. </returns>
		
		public string Status
		{	
			get	{	return _StatusMessage;	}
		}
		//************************************************************************************************/
		/// <summary> Property : ComPort. </summary>
		/// <returns> String. </returns>
		
		public int COMPort
		{	
			get	{	return _COMPort;	}
			set	{	_COMPort = value;	}
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

		public void InitializeController (int comPort)
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
				_SerialPort.Cnfg.BaudRate	= LineSpeed.Baud_9600;
				_SerialPort.Cnfg.DataBits	= ByteSize.Eight;
				_SerialPort.Cnfg.StopBits	= StopBits.One;
				_SerialPort.Cnfg.Parity		= Parity.None;
				_SerialPort.Cnfg.FlowCtrl	= Handshake.None;
				if (comPort < 10)
					_SerialPort.Cnfg.PortName = "COM" + comPort.ToString() + ":";
				else	// COM ports greater than 9 need to be formatted differently -- SS.
					_SerialPort.Cnfg.PortName = "\\\\.\\COM" + comPort.ToString ();

				// Based on the location of this RFID R/W, pick appropriate COM port.
				if( comPort > 0 && comPort < 20 )
					this._COMPort = comPort;
				else
					throw new ParselException ("Invalid Com Port. Cannot Initialize Device");
				
				// Only if NOT in hardware simulation mode, interface with hardware.
				if (!_HWSimulationMode)
				{	// Update status message.	
					_StatusMessage = "Opening the COM port....";
					// Open (initialize) the serial port with the configuration settings.
					result = _SerialPort.Open (_COMPort);
					// If initialization failed or port is 'really' not open (redundant check), throw a Parsel exception.
					if (!result || !_SerialPort.IsOpen)
					{	
						// Compose exception message with pre-defined error code in format 0x_____.
						_ExceptionMessage += " Failed to Initialize RFID COM Port !!";
						//_ExceptionCode = 1 ;
						//_ExceptionMessage += " " + "Error Code : 0x" + _ExceptionCode.ToString ("Y").PadLeft (5, '0') + ".";
						// Throw ParselException.
						throw new ParselException (_ExceptionMessage);
					}
				}
				// Update status message.
				_StatusMessage = "COM Port initialized.";
				_isInitialized = true;
			}
			// Catch any ParselExceptions thrown.  We probably just threw it. So just re-throw it.
			catch (ParselException exception)
			{
				_isInitialized = false;
				throw exception;
				//return;
			}
			// Catch any exceptions thrown.  Re-package it to ParselException object and throw exception.
			catch(Exception exception)
			{	
				// Compose exception message with pre-defined error code in format 0x_____.
				_ExceptionMessage += " Failed to Initialize COM Port !!";
				_ExceptionMessage += exception.Message;
				_ExceptionMessage += " " + "Error Code : 0x" + _ExceptionCode.ToString ("Y").PadLeft (5, '0') + ". ";
				_isInitialized = false;
				// Throw ParselException.
				throw new ParselException (_ExceptionMessage);
			}
		}
		//************************************************************************************************/
		/// <summary> Function : ReadTag (). </summary>

		public string ReadTag (int startAddress, int numberOfBytes)
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
			if ( (!_HWSimulationMode))
			{
				if (numberOfBytes > 100)
					throw new Exception ("Cannot read more than 100 Bytes at a time.");
		
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
					byte[] msg = new byte[101];
					int j = 0;    // Initialize j.
					
					// Iterate through raw data byte array and pull out LSB data byte values.
					for (int i = 4; i <= (numberOfBytes * 2) + 2; i = i+2)  // Byte 4,6,8,... (starting with 0)
					{	
						msg[j] = rxmessage[i];
						if(msg[j] == '\0')
							msg[j] = (byte)'-';
						j++;                  // Increment counter for data char pointer
					}

					// Decode the received bytes that are ASCII encoded (0x30 for 0, 0x41 for A, etc.) to corresponding
					// string of characters using the ASCII encoder's GetString () function (System.Text.ASCIIEncoding namespace).
					msg[j] = (byte)'\0';
					string trim = "\0";
					string s = _ASCIIEncoder.GetString (msg);
					response = s.TrimEnd(trim.ToCharArray()); 

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

		public void WriteTag (int startAddress, int numberOfBytes, byte [] dataToWrite)
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
			if ( (!_HWSimulationMode))
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
			if ( (!_HWSimulationMode))
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
				if ( (!_HWSimulationMode))
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
						
						long id = new long();
						id += Convert.ToInt64(extractedrxmessage[3]) << 24;
						id +=  Convert.ToInt64(extractedrxmessage[2]) << 16;
						id +=  Convert.ToInt64(extractedrxmessage[1]) << 8;
						id +=  Convert.ToInt64(extractedrxmessage[0]) ;

						// Decode the received bytes that are ASCII encoded (0x30 for 0, 0x41 for A, etc.) to corresponding
						// string of characters using the ASCII encoder's GetString () function (System.Text.ASCIIEncoding namespace).
						response = id.ToString();//ASCIIEncoder.GetString (extractedrxmessage);
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
			{	
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
	}	// End of Class -- HWCRFIDRW

}	

//************************************************************************************************/
