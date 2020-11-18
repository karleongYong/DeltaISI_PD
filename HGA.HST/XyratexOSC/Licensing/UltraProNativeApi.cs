using System.Runtime.InteropServices;

/// <summary>
/// Imports the UltraPro Native Library (ux32w.dll)
/// </summary>
public class UltraProNativeAPI
{
    private const string _dllName = "ux32w.dll";
    
   private UltraProNativeAPI()
   { }

   #region  Import UltraPro API from UltraPro Dyanamic Library (ux32w.dll)

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlActivateLicense")]
   public static extern ushort SFNTsntlActivateLicense(/* IN */ byte[] packet,
      /* IN */ int toolkitCellAddress,
      /* IN */ ushort writePassword,
      /* IN */ ushort activatePassword1,
      /* IN */ ushort activatePassword2);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlCleanup")]
   public static extern ushort SFNTsntlCleanup();

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlDecrementCounter")]
   public static extern ushort SFNTsntlDecrementCounter(/* IN */ byte[] packet,
      /* IN */ int toolkitCellAddress,
      /* IN */ ushort writePassword,
      /* IN */ ushort decrementValue);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlGetContactServer")]
   public static extern ushort SFNTsntlGetContactServer(/* IN */ byte[] packet,
      /* OUT*/ byte[] serverName,
      /* IN */ uint length);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlGetFullStatus")]
   public static extern ushort SFNTsntlGetFullStatus(/* IN */byte[] packet);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlGetHardLimit")]
   public static extern ushort SFNTsntlGetHardLimit(/* IN */ byte[] packet,
      /* OUT*/ out ushort hardLimit);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlGetKeyType")]
   public static extern ushort SFNTsntlGetKeyType(/* IN */ byte[] packet,
      /* OUT*/ out ushort keyFamily,
      /* OUT*/ out ushort keyFormFactor,
      /* OUT*/ out ushort keyMemorySize);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlGetLicense")]
   public static extern ushort SFNTsntlGetLicense(/* IN */ byte[] packet,
      /* IN */ ushort developerID,
      /* IN */ ushort designID,
      /* IN */ int toolkitCellAddress);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlGetVersion")]
   public static extern ushort SFNTsntlGetVersion(/* IN */ byte[] packet,
      /* OUT*/ out byte majorVersion,
      /* OUT*/ out byte minorVersion,
      /* OUT*/ out byte revision,
      /* OUT*/ out byte osDrvrType);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlInitialize")]
   public static extern ushort SFNTsntlInitialize(/* OUT*/ byte[] packet);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlLockData")]
   public static extern ushort SFNTsntlLockData(/* IN */ byte[] packet,
      /* IN */ int toolkitCellAddress,
      /* IN */ ushort writePassword);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlQueryLicense")]
   public static extern ushort SFNTsntlQueryLicense(/* IN */ byte[] packet,
      /* IN */ int toolkitCellAddress,
      /* IN */ ushort writePassword,
      /* IN */ byte[] queryData,
      /* OUT*/ byte[] response,
      /* OUT*/ ref uint response32,
      /* IN */ ushort length);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlQueryLicenseDecrement")]
   public static extern ushort SFNTsntlQueryLicenseDecrement(/* IN */ byte[] packet,
      /* IN */ int toolkitCellAddress,
      /* IN */ ushort writePassword,
      /* IN */ ushort decrementAmount,
      /* IN */ byte[] queryData,
      /* OUT*/ byte[] response,
      /* OUT*/ ref uint response32,
      /* IN */ ushort length);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlQueryLicenseLease")]
   public static extern ushort SFNTsntlQueryLicenseLease(/* IN */ byte[] packet,
      /* IN */ int toolkitCellAddress,
      /* IN */ ushort writePassword,
      /* IN */ byte[] queryData,
      /* OUT*/ byte[] response,
      /* OUT*/ ref uint response32,
      /* IN */ ushort length);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlQueryLicenseSimple")]
   public static extern ushort SFNTsntlQueryLicenseSimple(/* IN */ byte[] packet,
      /* IN */ int toolkitCellAddress,
      /* IN */ byte[] queryData,
      /* OUT*/ byte[] response,
      /* OUT*/ ref uint response32,
      /* IN */ ushort length);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlReadLease")]
   public static extern ushort SFNTsntlReadLease(/* IN */ byte[] packet,
      /* IN */ int toolkitCellAddress,
      /* OUT*/ byte[] dateValue,
      /* OUT*/ out ushort daysLeft);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlReadString")]
   public static extern ushort SFNTsntlReadString(/* IN */ byte[] packet,
      /* IN */ int toolkitCellAddress,
      /* OUT*/ byte[] stringBuffer,
      /* IN */ ushort stringLength);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlReadValue")]
   public static extern ushort SFNTsntlReadValue(/* IN */ byte[] packet,
      /* IN */ int toolkitCellAddress,
      /* OUT*/ out uint data);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlReleaseLicense")]
   public static extern ushort SFNTsntlReleaseLicense(/* IN */ byte[] packet,
      /* IN */     int toolkitCellAddress,
      /* IN/OUT */ ref ushort numUserLic);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlSetContactServer")]
   public static extern ushort SFNTsntlSetContactServer(/* IN */ byte[] packet,
      /* IN */ [MarshalAs(UnmanagedType.LPStr)] string serverName);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlSetHeartBeat")]
   public static extern ushort SFNTsntlSetHeartBeat(/* IN */ byte[] packet,
      /* IN */ int heartBeat);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlSetProtocol")]
   public static extern ushort SFNTsntlSetProtocol(/* IN */ byte[] packet,
      /* IN */ int protocol);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlSetSharedLicense")]
   public static extern ushort SFNTsntlSetSharedLicense(/* IN */ byte[] packet,
      /* IN */ ushort shareMode,
      /* IN */ byte[] reserved);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlWriteString")]
   public static extern ushort SFNTsntlWriteString(/* IN */ byte[] packet,
      /* IN */ int toolkitCellAddress,
      /* IN */ [MarshalAs(UnmanagedType.LPStr)] string sBuff,
      /* IN */ byte flag,
      /* IN */ ushort writePassword);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlWriteValue")]
   public static extern ushort SFNTsntlWriteValue(/* IN */ byte[] packet,
      /* IN */ int toolkitCellAddress,
      /* IN */ uint data,
      /* IN */ byte flag,
      /* IN */ ushort writePassword);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlUnlockData")]
   public static extern ushort SFNTsntlUnlockData(/* IN */ byte[] packet,
      /* IN */ int toolkitCellAddress,
      /* IN */ ushort writePassword,
      /* IN */ ushort overwritePassword1,
      /* IN */ ushort overwritePassword2);

   ///<summary>See UltraPro Documentation.</summary>
   [DllImport(_dllName,
      CharSet = CharSet.Ansi,
      EntryPoint = "SFNTsntlCheckTerminalService")]
   public static extern ushort SFNTsntlCheckTerminalService(/* IN */ byte[] packet,
      /* IN */ ushort termserv);

   #endregion
}