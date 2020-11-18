using System;
using System.Reflection;

/// <summary>
/// <para>Ultrapro Class provided by SafeNet</para>
/// <para>(C) Copyright 2007 SafeNet, Inc. All rights reserved.</para>
/// <para>This module provide defination and wrappers for UltraPro Dual Client Library APIs</para>
/// <para>(Documentation: http://xy01sps01.xy01.xyratex.com/sites/xyosc/software/Documents/Licensing/UltraPro_NET_Interface.pdf) </para>
/// </summary>
public class Ultrapro : IDisposable
{

   #region Constructor / Destructor
   /* Define API packet for communication   */
   const uint SP_APIPACKET_SIZE = 4112;
   private byte[] packet;
   private bool disposed;

   private string strError = "Unable to load the required UltraPro Library(ux32w.dll).\n" +
                     "Either the library is missing or corrupted.";

   /// <summary>
   /// Initializes a new instance of the <see cref="Ultrapro"/> class.
   /// </summary>
   internal Ultrapro()
   {
      packet = new byte[SP_APIPACKET_SIZE];
   }

   /// <summary>
   /// Releases unmanaged resources and performs other cleanup operations before the
   /// <see cref="Ultrapro"/> is reclaimed by garbage collection.
   /// </summary>
   ~Ultrapro()
   {
      Dispose(false);
   }

   /// <summary>
   /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
   /// </summary>
   public void Dispose()
   {
      Dispose(true);
      GC.SuppressFinalize(this);
   }

   /// <summary>
   /// Releases unmanaged and - optionally - managed resources
   /// </summary>
   /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
   protected virtual void Dispose(bool disposing)
   {
      if (!this.disposed)
      {
         if (disposing)
         {
            this.packet = null;
            /* only MANAGED resource should be dispose     */
            /* here that implement the IDisposable		   */
            /* Interface 								   */
         }
         /* only UNMANAGED resource should be dispose here */
         /*												  */
      }
      disposed = true;
   }
   #endregion

   #region Ultrapro API Error Codes
   // UltraPro API Error codes

   /// <summary>UltraPro ERR 0: See UltraPro Documentation.</summary>
   internal const int SP_ERR_SUCCESS = 0;

   /// <summary>UltraPro ERR 1: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_FUNCTION_CODE = 1;

   /// <summary>UltraPro ERR 2: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_PACKET = 2;

   /// <summary>UltraPro ERR 3: See UltraPro Documentation.</summary>
   internal const int SP_ERR_UNIT_NOT_FOUND = 3;

   /// <summary>UltraPro ERR 4: See UltraPro Documentation.</summary>
   internal const int SP_ERR_ACCESS_DENIED = 4;

   /// <summary>UltraPro ERR 5: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_MEMORY_ADDRESS = 5;

   /// <summary>UltraPro ERR 6: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_ACCESS_CODE = 6;

   /// <summary>UltraPro ERR 7: See UltraPro Documentation.</summary>
   internal const int SP_ERR_PORT_IS_BUSY = 7;

   /// <summary>UltraPro ERR 8: See UltraPro Documentation.</summary>
   internal const int SP_ERR_WRITE_NOT_READY = 8;

   /// <summary>UltraPro ERR 9: See UltraPro Documentation.</summary>
   internal const int SP_ERR_NO_PORT_FOUND = 9;

   /// <summary>UltraPro ERR 10: See UltraPro Documentation.</summary>
   internal const int SP_ERR_ALREADY_ZERO = 10;

   /// <summary>UltraPro ERR 11: See UltraPro Documentation.</summary>
   internal const int SP_ERR_DRIVER_OPEN_ERROR = 11;

   /// <summary>UltraPro ERR 12: See UltraPro Documentation.</summary>
   internal const int SP_ERR_DRIVER_NOT_INSTALLED = 12;

   /// <summary>UltraPro ERR 13: See UltraPro Documentation.</summary>
   internal const int SP_ERR_IO_COMMUNICATIONS_ERROR = 13;

   /// <summary>UltraPro ERR 15: See UltraPro Documentation.</summary>
   internal const int SP_ERR_PACKET_TOO_SMALL = 15;

   /// <summary>UltraPro ERR 16: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_PARAMETER = 16;

   /// <summary>UltraPro ERR 17: See UltraPro Documentation.</summary>
   internal const int SP_ERR_MEM_ACCESS_ERROR = 17;

   /// <summary>UltraPro ERR 18: See UltraPro Documentation.</summary>
   internal const int SP_ERR_VERSION_NOT_SUPPORTED = 18;

   /// <summary>UltraPro ERR 19: See UltraPro Documentation.</summary>
   internal const int SP_ERR_OS_NOT_SUPPORTED = 19;

   /// <summary>UltraPro ERR 20: See UltraPro Documentation.</summary>
   internal const int SP_ERR_QUERY_TOO_LONG = 20;

   /// <summary>UltraPro ERR 21: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_COMMAND = 21;

   /// <summary>UltraPro ERR 29: See UltraPro Documentation.</summary>
   internal const int SP_ERR_MEM_ALIGNMENT_ERROR = 29;

   /// <summary>UltraPro ERR 30: See UltraPro Documentation.</summary>
   internal const int SP_ERR_DRIVER_IS_BUSY = 30;

   /// <summary>UltraPro ERR 31: See UltraPro Documentation.</summary>
   internal const int SP_ERR_PORT_ALLOCATION_FAILURE = 31;

   /// <summary>UltraPro ERR 32: See UltraPro Documentation.</summary>
   internal const int SP_ERR_PORT_RELEASE_FAILURE = 32;

   /// <summary>UltraPro ERR 39: See UltraPro Documentation.</summary>
   internal const int SP_ERR_ACQUIRE_PORT_TIMEOUT = 39;

   /// <summary>UltraPro ERR 42: See UltraPro Documentation.</summary>
   internal const int SP_ERR_SIGNAL_NOT_SUPPORTED = 42;

   /// <summary>UltraPro ERR 44: See UltraPro Documentation.</summary>
   internal const int SP_ERR_UNKNOWN_MACHINE = 44;

   /// <summary>UltraPro ERR 45: See UltraPro Documentation.</summary>
   internal const int SP_ERR_SYS_API_ERROR = 45;

   /// <summary>UltraPro ERR 46: See UltraPro Documentation.</summary>
   internal const int SP_ERR_UNIT_IS_BUSY = 46;

   /// <summary>UltraPro ERR 47: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_PORT_TYPE = 47;

   /// <summary>UltraPro ERR 48: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_MACH_TYPE = 48;

   /// <summary>UltraPro ERR 49: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_IRQ_MASK = 49;

   /// <summary>UltraPro ERR 50: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_CONT_METHOD = 50;

   /// <summary>UltraPro ERR 51: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_PORT_FLAGS = 51;

   /// <summary>UltraPro ERR 52: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_LOG_PORT_CFG = 52;

   /// <summary>UltraPro ERR 53: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_OS_TYPE = 53;

   /// <summary>UltraPro ERR 54: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_LOG_PORT_NUM = 54;

   /// <summary>UltraPro ERR 56: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_ROUTER_FLGS = 56;

   /// <summary>UltraPro ERR 57: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INIT_NOT_CALLED = 57;

   /// <summary>UltraPro ERR 58: See UltraPro Documentation.</summary>
   internal const int SP_ERR_DRVR_TYPE_NOT_SUPPORTED = 58;

   /// <summary>UltraPro ERR 59: See UltraPro Documentation.</summary>
   internal const int SP_ERR_FAIL_ON_DRIVER_COMM = 59;


   /* Networking Error Codes */

   /// <summary>UltraPro Networking ERR 60: See UltraPro Documentation.</summary>
   internal const int SP_ERR_SERVER_PROBABLY_NOT_UP = 60;

   /// <summary>UltraPro Networking ERR 61: See UltraPro Documentation.</summary>
   internal const int SP_ERR_UNKNOWN_HOST = 61;

   /// <summary>UltraPro Networking ERR 62: See UltraPro Documentation.</summary>
   internal const int SP_ERR_SENDTO_FAILED = 62;

   /// <summary>UltraPro Networking ERR 63: See UltraPro Documentation.</summary>
   internal const int SP_ERR_SOCKET_CREATION_FAILED = 63;

   /// <summary>UltraPro Networking ERR 64: See UltraPro Documentation.</summary>
   internal const int SP_ERR_NORESOURCES = 64;

   /// <summary>UltraPro Networking ERR 65: See UltraPro Documentation.</summary>
   internal const int SP_ERR_BROADCAST_NOT_SUPPORTED = 65;

   /// <summary>UltraPro Networking ERR 66: See UltraPro Documentation.</summary>
   internal const int SP_ERR_BAD_SERVER_MESSAGE = 66;

   /// <summary>UltraPro Networking ERR 67: See UltraPro Documentation.</summary>
   internal const int SP_ERR_NO_SERVER_RUNNING = 67;

   /// <summary>UltraPro Networking ERR 68: See UltraPro Documentation.</summary>
   internal const int SP_ERR_NO_NETWORK = 68;

   /// <summary>UltraPro Networking ERR 69: See UltraPro Documentation.</summary>
   internal const int SP_ERR_NO_SERVER_RESPONSE = 69;

   /// <summary>UltraPro Networking ERR 70: See UltraPro Documentation.</summary>
   internal const int SP_ERR_NO_LICENSE_AVAILABLE = 70;

   /// <summary>UltraPro Networking ERR 71: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_LICENSE = 71;

   /// <summary>UltraPro Networking ERR 72: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INVALID_OPERATION = 72;

   /// <summary>UltraPro Networking ERR 73: See UltraPro Documentation.</summary>
   internal const int SP_ERR_BUFFER_TOO_SMALL = 73;

   /// <summary>UltraPro Networking ERR 74: See UltraPro Documentation.</summary>
   internal const int SP_ERR_INTERNAL_ERROR = 74;

   /// <summary>UltraPro Networking ERR 75: See UltraPro Documentation.</summary>
   internal const int SP_ERR_PACKET_ALREADY_INITIALIZED = 75;

   /// <summary>UltraPro Networking ERR 76: See UltraPro Documentation.</summary>
   internal const int SP_ERR_PROTOCOL_NOT_INSTALLED = 76;

   /// <summary>UltraPro Networking ERR 101: See UltraPro Documentation.</summary>
   internal const int SP_ERR_NO_LEASE_FEATURE = 101;

   /// <summary>UltraPro Networking ERR 102: See UltraPro Documentation.</summary>
   internal const int SP_ERR_LEASE_EXPIRED = 102;

   /// <summary>UltraPro Networking ERR 103: See UltraPro Documentation.</summary>
   internal const int SP_ERR_COUNTER_LIMIT_REACHED = 103;

   /// <summary>UltraPro Networking ERR 104: See UltraPro Documentation.</summary>
   internal const int SP_ERR_NO_DIGITAL_SIGNATURE = 104;

   /// <summary>UltraPro Networking ERR 105: See UltraPro Documentation.</summary>
   internal const int SP_ERR_SYS_FILE_CORRUPTED = 105;

   /// <summary>UltraPro Networking ERR 106: See UltraPro Documentation.</summary>
   internal const int SP_ERR_STRING_BUFFER_TOO_LONG = 106;

   /* Shell Error Codes */

   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_BAD_ALGO = 401;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_LONG_MSG = 402;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_READ_ERROR = 403;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_NOT_ENOUGH_MEMORY = 404;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_CANNOT_OPEN = 405;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_WRITE_ERROR = 406;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_CANNOT_OVERWRITE = 407;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_INVALID_HEADER = 408;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_TMP_CREATE_ERROR = 409;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_PATH_NOT_THERE = 410;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_BAD_FILE_INFO = 411;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_NOT_WIN32FILE = 412;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_INVALID_MACHINE = 413;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_INVALID_SECTION = 414;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_INVALID_RELOC = 415;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_CRYPT_ERROR = 416;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_SMARTHEAP_ERROR = 417;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_IMPORT_OVERWRITE_ERROR = 418;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_NO_PESHELL = 420;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_FRAMEWORK_REQUIRED = 421;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_CANNOT_HANDLE_FILE = 422;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_IMPORT_DLL_ERROR = 423;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_IMPORT_FUNCTION_ERROR = 424;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_X64_SHELL_ENGINE = 425;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_STRONG_NAME = 426;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_FRAMEWORK_10 = 427;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_FRAMEWORK_SDK_10 = 428;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_FRAMEWORK_11 = 429;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_FRAMEWORK_SDK_11 = 430;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_FRAMEWORK_20_OR_30 = 431;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_FRAMEWORK_SDK_20 = 432;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_APP_NOT_SUPPORTED = 433;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_FILE_COPY = 434;
   /// <summary>UltraPro Shell ERR</summary>
   internal const int SH_HEADER_SIZE_EXCEED = 435;

   #endregion

   #region UltraPro Constants values used by client application.

   /* Set Protocol Flags */

   /// <summary>UltraPro TCP Protocol Flag</summary>
   internal const int SP_TCP_PROTOCOL = 1;

   /// <summary>UltraPro IPX Protocol Flag</summary>
   internal const int SP_IPX_PROTOCOL = 2;
    
    /// <summary>UltraPro NETBEUI Protocol Flag</summary>
   internal const int SP_NETBEUI_PROTOCOL = 4;

   /* String for OS Driver type */

   /// <summary>DOS Real Mode local driver</summary>
   internal const int SP_DOSRM_LOCAL_DRVR = 1;  
   /// <summary>Windows 3.x local driver</summary>
   internal const int SP_WIN3x_LOCAL_DRVR = 2;
   /// <summary>Win32s local driver</summary>
   internal const int SP_WIN32s_LOCAL_DRVR = 3; 
   /// <summary>Windows 3.x system driver</summary>
   internal const int SP_WIN3x_SYS_DRVR = 4;  
   /// <summary>Windows NT system driver</summary>
   internal const int SP_WINNT_SYS_DRVR = 5;     
   /// <summary>Windows 95 system driver</summary>
   internal const int SP_WIN95_SYS_DRVR = 7;  
   /// <summary>Netware local driver</summary>
   internal const int SP_NW_LOCAL_DRVR = 8;      
   /// <summary>QNX local driver</summary>
   internal const int SP_QNX_LOCAL_DRVR = 9;   
   /// <summary>UNIX local driver</summary>
   internal const int SP_UNIX_SYS_DRVR = 10;  
   /// <summary>SOLARIS local driver</summary>
   internal const int SP_SOLARIS_SYS_DRVR = 11;
   /// <summary>Linux system driver</summary>
   internal const int SP_LINUX_SYS_DRVR = 12;
   /// <summary>Linux local driver	</summary>
   internal const int SP_LINUX_LOCAL_DRVR = 13;
   /// <summary>AIX system driver</summary>
   internal const int SP_AIX_SYS_DRVR = 14; 
   /// <summary>UNIX system  driver</summary>
   internal const int SP_UNIXWARE_SYS_DRVR = 15;

   /// <summary>Maximum number of devices</summary>
   internal const int SP_MAX_NUM_DEV = 10;

   /// <summary>Maximum host name length</summary>
   internal const int SP_MAX_NAME_LEN = 64;

   /// <summary>Maximum host address length</summary>
   internal const int SP_MAX_ADDR_LEN = 32;

   /// <summary>Maximum number of query bytes</summary>
   internal const int SP_MAX_QUERY_SIZE = 56;

   /* Making the LIcense Update Time programmable*/

   /// <summary>Update Time: Default heartbeat - 2*60 = 2 min.</summary>
   internal const int SP_LIC_UPDATE_INT = 120;
   /// <summary>Update Time: Max heartbeat - 30*24*60*60 seconds</summary>
   internal const int SP_MAX_HEARTBEAT = 2592000;
   /// <summary>Update Time: Min heartbeat - 60 seconds</summary>
   internal const int SP_MIN_HEARTBEAT = 60; 
   /// <summary>Update Time: Infinite heartbeat</summary>
   internal const int SP_INFINITE_HEARTBEAT = -1;

   /*Communication Modes */

   /// <summary>Communication Mode</summary>
   internal const string SP_STANDALONE_MODE = "SP_STANDALONE_MODE";
   /// <summary>Communication Mode</summary>
   internal const string SP_DRIVER_MODE = "SP_DRIVER_MODE";
   /// <summary>Communication Mode</summary>
   internal const string SP_LOCAL_MODE = "SP_LOCAL_MODE";
   /// <summary>Communication Mode</summary>
   internal const string SP_BROADCAST_MODE = "SP_BROADCAST_MODE";
   /// <summary>Communication Mode</summary>
   internal const string SP_ALL_MODE = "SP_ALL_MODE";
   /// <summary>Communication Mode</summary>
   internal const string SP_SERVER_MODE = "SP_SERVER_MODE";

   /* Defines for toolkit cell address */
   /// <summary>Toolkit Cell Address Type</summary>
   internal const int SP_INT8_TYPE = 1;
   /// <summary>Toolkit Cell Address Type</summary>
   internal const int SP_INT32_TYPE = 2;
   /// <summary>Toolkit Cell Address Type</summary>
   internal const int SP_STRING_TYPE = 3;
   /// <summary>Toolkit Cell Address Type</summary>
   internal const int SP_BOOL_TYPE = 4;
   /// <summary>Toolkit Cell Address Type</summary>
   internal const int SP_DATE_TYPE = 5;
   /// <summary>Toolkit Cell Address Type</summary>
   internal const int SP_LEASE_TYPE = 6;
   /// <summary>Toolkit Cell Address Type</summary>
   internal const int SP_ALGO_TYPE = 7;
   /// <summary>Toolkit Cell Address Type</summary>
   internal const int SP_COUNTER_TYPE = 8;
   /// <summary>Toolkit Cell Address Type</summary>
   internal const int SP_DATA_WORD_TYPE = 9;
   /// <summary>Toolkit Cell Address Type</summary>    
   internal const int SP_RESERVED_TYPE = 15;


   /* Sharing Flags */

   /// <summary>Sharing Flag</summary>
   internal const int SP_SHARE_USERNAME = 1;
   /// <summary>Sharing Flag</summary>
   internal const int SP_SHARE_MAC_ADDRESS = 2;
   /// <summary>Sharing Flag</summary>
   internal const int SP_SHARE_DEFAULT = 3;

   /* Key Type Constants */
   /// <summary>Key Type Constant</summary>
   internal const int SP_KEY_FORM_FACTOR_PARALLEL = 0;
   /// <summary>Key Type Constant</summary>
   internal const int SP_KEY_FORM_FACTOR_USB = 1;
   /// <summary>Key Type Constant</summary>
   internal const int SP_SUPERPRO_FAMILY_KEY = 0;
   /// <summary>Key Type Constant</summary>
   internal const int SP_ULTRAPRO_FAMILY_KEY = 1;
   /// <summary>Key Type Constant</summary>
   internal const int SP_UNKNOWN_FAMILY_KEY = 16;

   /* Defines for SFNTsntlReadValue */

   /// <summary>For SFNTsntlReadValue</summary>
   internal const int SP_SERIAL_NUMBER_INTEGER = ((SP_INT32_TYPE << 16) | 0);
   /// <summary>For SFNTsntlReadValue</summary>
   internal const int SP_DEVELOPER_ID_WORD = ((SP_DATA_WORD_TYPE << 16) | 1);
   /// <summary>For SFNTsntlReadValue</summary>
   internal const int SP_DESIGN_ID_WORD = ((SP_RESERVED_TYPE << 16) | 15);

   /* Defines for SFNTsntlReadString */

   /// <summary>For SFNTsntlReadString</summary>   
   internal const int SP_PART_NUMBER_STRING = ((SP_RESERVED_TYPE << 16) | 0x05);

   /* The term "Part Number" has been renamed to "Model Number" from the current release of UltraPro 1.1.*/
   /// <summary>Handles 'Part Number' to 'Model Number' renaming</summary>   
   internal const int SP_MODEL_NUMBER_STRING = SP_PART_NUMBER_STRING;

   /* Defines for SFNTsntlCheckTerminalService */

   /// <summary>For SFNTsntlCheckTerminalService</summary> 
   internal const int SP_TERM_SERV_CHECK_ON = 1;
   /// <summary>For SFNTsntlCheckTerminalService</summary> 
   internal const int SP_TERM_SERV_CHECK_OFF = 0;

   #endregion

   #region  Date Structure Used by SFNTsntlReadLease


   /// <summary>
   /// Data Structure Used by SFNTsntlReadLease
   /// </summary>
   internal class LeaseDate
   {
      /// <summary>Byte Array Date Buffer</summary>
      internal byte[] bDateBuffer;
      const uint MAX_DATE_LENGTH = 8;
      const int BYTE_OFFSET_YEAR = 0;
      const int BYTE_OFFSET_MONTH = 2;
      const int BYTE_OFFSET_DAY = 4;

      /// <summary>
      /// Initializes a new instance of the <see cref="LeaseDate"/> class.
      /// </summary>
      internal LeaseDate()
      {
         bDateBuffer = new byte[MAX_DATE_LENGTH];
      }

      /// <summary>
      /// Releases unmanaged resources and performs other cleanup operations before the
      /// <see cref="LeaseDate"/> is reclaimed by garbage collection.
      /// </summary>
      ~LeaseDate()
      {
         bDateBuffer = null;
      }

      /// <summary>
      /// Gets the year.
      /// </summary>
      /// <value>The year.</value>
      internal ushort Year
      {
         get
         {
            /*get the year from the buffer*/
            return (ushort)BitConverter.ToInt16(this.bDateBuffer, BYTE_OFFSET_YEAR);
         }
      }

      /// <summary>
      /// Gets the month.
      /// </summary>
      /// <value>The month.</value>
      internal ushort Month
      {
         get
         {
            return (ushort)BitConverter.ToInt16(this.bDateBuffer, BYTE_OFFSET_MONTH);
         }
      }

      /// <summary>
      /// Gets the day.
      /// </summary>
      /// <value>The day.</value>
      internal ushort Day
      {
         get
         {
            return (ushort)BitConverter.ToInt16(this.bDateBuffer, BYTE_OFFSET_DAY);
         }
      }
   }
   #endregion

   #region	internal METHODS

   /// <summary>
   /// This function validates and initializes the API packet.
   /// </summary>
   /// <returns></returns>
   internal ushort SFNTsntlInitialize()
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlInitialize(this.packet);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function releases the memory resources acquired by the UltraPro client library.
   /// </summary>
   internal void SFNTsntlCleanup()
   {
      try
      {
         UltraProNativeAPI.SFNTsntlCleanup();
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
   }

   /// <summary>
   /// This function returns the following information about the key attached to a system.
   /// </summary>
   /// <param name="keyFamily">The key family.</param>
   /// <param name="keyFormFactor">The key form factor.</param>
   /// <param name="keyMemorySize">Size of the key memory.</param>
   /// <returns></returns>
   internal ushort SFNTsntlGetKeyType(out ushort keyFamily, out ushort keyFormFactor, out ushort keyMemorySize)
   {
      ushort status = 0;
      keyFamily = 0;
      keyFormFactor = 0;
      keyMemorySize = 0;

      try
      {
         status = UltraProNativeAPI.SFNTsntlGetKeyType(this.packet, out keyFamily, out keyFormFactor, out keyMemorySize);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function returns the Sentinel Driver's version and type.
   /// </summary>
   /// <param name="majorVersion">The major version.</param>
   /// <param name="minorVersion">The minor version.</param>
   /// <param name="revision">The revision.</param>
   /// <param name="osDrvrType">Type of the OS Driver.</param>
   /// <returns></returns>
   internal ushort SFNTsntlGetVersion(out byte majorVersion, out byte minorVersion, out byte revision, out byte osDrvrType)
   {
      ushort status = 0;
      majorVersion = 0;
      minorVersion = 0;
      revision = 0;
      osDrvrType = 0;

      try
      {
         status = UltraProNativeAPI.SFNTsntlGetVersion(this.packet, out majorVersion, out minorVersion, out revision, out osDrvrType);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function retrieves the hard limit of the key from which the license was obtained.
   /// </summary>
   /// <param name="hardLimit">The hard limit.</param>
   /// <returns></returns>
   internal ushort SFNTsntlGetHardLimit(out ushort hardLimit)
   {
      ushort status = 0;
      hardLimit = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlGetHardLimit(this.packet, out hardLimit);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function obtains a license from the key programmed with your developer ID and design ID.
   /// </summary>
   /// <param name="developerID">The developer ID.</param>
   /// <param name="designID">The design ID.</param>
   /// <param name="toolkitCellAddress">The toolkit cell address.</param>
   /// <returns></returns>
   internal ushort SFNTsntlGetLicense(ushort developerID, ushort designID, int toolkitCellAddress)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlGetLicense(this.packet, developerID, designID, toolkitCellAddress);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function activates an inactive algorithm at the specified cell address.
   /// </summary>
   /// <param name="toolkitCellAddress">The toolkit cell address.</param>
   /// <param name="writePassword">The write password.</param>
   /// <param name="activatePassword1">The activate password1.</param>
   /// <param name="activatePassword2">The activate password2.</param>
   /// <returns></returns>
   internal ushort SFNTsntlActivateLicense(int toolkitCellAddress, ushort writePassword, ushort activatePassword1, ushort activatePassword2)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlActivateLicense(this.packet, toolkitCellAddress, writePassword, activatePassword1, activatePassword2);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function decrements a counter value by a specified amount.
   /// </summary>
   /// <param name="toolkitCellAddress">The toolkit cell address.</param>
   /// <param name="writePassword">The write password.</param>
   /// <param name="decrementValue">The decrement value.</param>
   /// <returns></returns>
   internal ushort SFNTsntlDecrementCounter(int toolkitCellAddress, ushort writePassword, ushort decrementValue)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlDecrementCounter(this.packet, toolkitCellAddress, writePassword, decrementValue);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This API reads the following values associated with the elements or specific cells on the key: See Documentation.
   /// </summary>
   /// <param name="toolkitCellAddress">The toolkit cell address.</param>
   /// <param name="data">The data.</param>
   /// <returns></returns>
   internal ushort SFNTsntlReadValue(int toolkitCellAddress, out uint data)
   {
      ushort status = 0;
      data = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlReadValue(this.packet, toolkitCellAddress, out data);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function reads the date up to which the application is allowed to run.
   /// </summary>
   /// <param name="toolkitCellAddress">The toolkit cell address.</param>
   /// <param name="leaseDate">The lease date.</param>
   /// <param name="daysLeft">The days left.</param>
   /// <returns></returns>
   internal ushort SFNTsntlReadLease(int toolkitCellAddress, Ultrapro.LeaseDate leaseDate, out ushort daysLeft)
   {
      ushort status = 0;
      daysLeft = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlReadLease(this.packet, toolkitCellAddress, leaseDate.bDateBuffer, out daysLeft);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function reads the following values programmed into the key: A String element, The Model Number
   /// </summary>
   /// <param name="toolkitCellAddress">The toolkit cell address.</param>
   /// <param name="stringBuffer">The string buffer.</param>
   /// <param name="stringLength">Length of the string.</param>
   /// <returns></returns>
   internal ushort SFNTsntlReadString(int toolkitCellAddress, byte[] stringBuffer, ushort stringLength)
   {
      ushort status = 0;

      try
      {
         status = UltraProNativeAPI.SFNTsntlReadString(this.packet, toolkitCellAddress, stringBuffer, stringLength);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function changes the unlocked data word to locked data word at the specified Toolkit Cell Address.
   /// </summary>
   /// <param name="toolkitCellAddress">The toolkit cell address.</param>
   /// <param name="writePassword">The write password.</param>
   /// <returns></returns>
   internal ushort SFNTsntlLockData(int toolkitCellAddress, ushort writePassword)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlLockData(this.packet, toolkitCellAddress, writePassword);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function performs all the operations that are otherwise individually performed by the
   /// SFNTsntlQueryLicenseDecrement and SFNTsntlQueryLicenseLease API functions.
   /// </summary>
   /// <param name="toolkitCellAddress">The toolkit cell address.</param>
   /// <param name="writePassword">The write password.</param>
   /// <param name="queryData">The query data.</param>
   /// <param name="response">The response.</param>
   /// <param name="response32">The response32.</param>
   /// <param name="length">The length.</param>
   /// <returns></returns>
   internal ushort SFNTsntlQueryLicense(int toolkitCellAddress, ushort writePassword, byte[] queryData, byte[] response, ref uint response32, ushort length)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlQueryLicense(this.packet, toolkitCellAddress, writePassword, queryData, response, ref response32, length);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function decrements an execution count (associated with a Full License element) by the
   /// amount specified, in addition to the normal query described in SFNTsntlQueryLicenseSimple.
   /// </summary>
   /// <param name="toolkitCellAddress">The toolkit cell address.</param>
   /// <param name="writePassword">The write password.</param>
   /// <param name="decrementAmount">The decrement amount.</param>
   /// <param name="queryData">The query data.</param>
   /// <param name="response">The response.</param>
   /// <param name="response32">The response32.</param>
   /// <param name="length">The length.</param>
   /// <returns></returns>
   internal ushort SFNTsntlQueryLicenseDecrement(int toolkitCellAddress, ushort writePassword, ushort decrementAmount, byte[] queryData, byte[] response, ref uint response32, ushort length)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlQueryLicenseDecrement(this.packet, toolkitCellAddress, writePassword, decrementAmount, queryData, response, ref response32, length);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function performs the following lease checks (in the order described below) in addition to
   /// the normal query described in SFNTsntlQueryLicenseSimple: See Documentation
   /// </summary>
   /// <param name="toolkitCellAddress">The toolkit cell address.</param>
   /// <param name="writePassword">The write password.</param>
   /// <param name="queryData">The query data.</param>
   /// <param name="response">The response.</param>
   /// <param name="response32">The response32.</param>
   /// <param name="length">The length.</param>
   /// <returns></returns>
   internal ushort SFNTsntlQueryLicenseLease(int toolkitCellAddress, ushort writePassword, byte[] queryData, byte[] response, ref uint response32, ushort length)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlQueryLicenseLease(this.packet, toolkitCellAddress, writePassword, queryData, response, ref response32, length);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function is used to query an algorithm pointed by the Toolkit Cell Address value.
   /// </summary>
   /// <param name="toolkitCellAddress">The toolkit cell address.</param>
   /// <param name="queryData">The query data.</param>
   /// <param name="response">The response.</param>
   /// <param name="response32">The response32.</param>
   /// <param name="length">The length.</param>
   /// <returns></returns>
   internal ushort SFNTsntlQueryLicenseSimple(int toolkitCellAddress, byte[] queryData, byte[] response, ref uint response32, ushort length)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlQueryLicenseSimple(this.packet, toolkitCellAddress, queryData, response, ref response32, length);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function releases the license issued to an application.
   /// </summary>
   /// <param name="toolkitCellAddress">The toolkit cell address.</param>
   /// <param name="numUserLic">The num user lic.</param>
   /// <returns></returns>
   internal ushort SFNTsntlReleaseLicense(int toolkitCellAddress, ref ushort numUserLic)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlReleaseLicense(this.packet, toolkitCellAddress, ref numUserLic);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function sets the access mode for finding the key.
   /// </summary>
   /// <param name="serverName">Name of the server.</param>
   /// <returns></returns>
   internal ushort SFNTsntlSetContactServer(string serverName)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlSetContactServer(this.packet, serverName);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function sets the heartbeat interval for maintaining the communication between a client
   /// and the Sentinel Protection Server.
   /// </summary>
   /// <param name="heartBeat">The heart beat.</param>
   /// <returns></returns>
   internal ushort SFNTsntlSetHeartBeat(int heartBeat)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlSetHeartBeat(this.packet, heartBeat);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function sets the network protocol for allowing communication between the client and
   /// Sentinel Protection Server.
   /// </summary>
   /// <param name="protocolFlag">The protocol flag.</param>
   /// <returns></returns>
   internal ushort SFNTsntlSetProtocol(int protocolFlag)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlSetProtocol(this.packet, protocolFlag);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function allows you to share a license on the basis of a user name or MAC address, if you
   /// have specified a user limit in your design.
   /// </summary>
   /// <param name="shareMode">The share mode.</param>
   /// <param name="reserved">The reserved.</param>
   /// <returns></returns>
   internal ushort SFNTsntlSetSharedLicense(ushort shareMode, byte[] reserved)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlSetSharedLicense(this.packet, shareMode, reserved);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function unlocks the locked elements.
   /// </summary>
   /// <param name="toolkitCellAddress">The toolkit cell address.</param>
   /// <param name="writePassword">The write password.</param>
   /// <param name="overWritePassword1">The over write password1.</param>
   /// <param name="overWritePassword2">The over write password2.</param>
   /// <returns></returns>
   internal ushort SFNTsntlUnlockData(int toolkitCellAddress, ushort writePassword, ushort overWritePassword1, ushort overWritePassword2)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlUnlockData(this.packet, toolkitCellAddress, writePassword, overWritePassword1, overWritePassword2);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function writes a String element in the key.
   /// </summary>
   /// <param name="toolkitCellAddress">The toolkit cell address.</param>
   /// <param name="data">The data.</param>
   /// <param name="flag">The flag.</param>
   /// <param name="writePassword">The write password.</param>
   /// <returns></returns>
   internal ushort SFNTsntlWriteString(int toolkitCellAddress, string data, byte flag, ushort writePassword)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlWriteString(this.packet, toolkitCellAddress, data, flag, writePassword);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function writes the following values on the key: See Documentation. This function requires the Write Password.
   /// </summary>
   /// <param name="toolkitCellAddress">The toolkit cell address.</param>
   /// <param name="data">The data.</param>
   /// <param name="flag">The flag.</param>
   /// <param name="writePassword">The write password.</param>
   /// <returns></returns>
   internal ushort SFNTsntlWriteValue(int toolkitCellAddress, uint data, byte flag, ushort writePassword)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlWriteValue(this.packet, toolkitCellAddress, data, flag, writePassword);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function returns the access mode set to obtain license.
   /// </summary>
   /// <param name="serverName">Name of the server.</param>
   /// <param name="length">The length.</param>
   /// <returns></returns>
   internal ushort SFNTsntlGetContactServer(byte[] serverName, uint length)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlGetContactServer(this.packet, serverName, length);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// This function obtains the return code of the last-called API function.
   /// </summary>
   /// <returns></returns>
   internal ushort SFNTsntlGetFullStatus()
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlGetFullStatus(this.packet);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   /// <summary>
   /// The function allows you to enable/disable the checking on terminal clients while SFNTsntlGetLicense
   /// API is executed.
   /// </summary>
   /// <param name="termserv">The termserv.</param>
   /// <returns></returns>
   internal ushort SFNTsntlCheckTerminalService(ushort termserv)
   {
      ushort status = 0;
      try
      {
         status = UltraProNativeAPI.SFNTsntlCheckTerminalService(this.packet, termserv);
      }
      catch (System.DllNotFoundException)
      {
         throw new System.DllNotFoundException(strError);
      }
      return status;
   }

   #endregion //////////////////////// END internal METHODS	///////////////////////////////////
}