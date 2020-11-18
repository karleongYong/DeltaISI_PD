using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using XyratexOSC.Utilities;

namespace XyratexOSC.Licensing
{
    #region LicenseLock Class

    /// <summary>
    /// A Wrapper Class to the Sentinel UltraPro security dongle for license verification.
    /// </summary>
    /// <remarks>
    /// The <see cref="LicenseLock"/> class verifies a valid license dongle is plugged into the computer on instatiation
    /// and through a call to verification (see <see cref="Verify()"/> that can be made at any time throughout the application.
    /// If a valid license is found the class does nothing, but if there is any problem with the licensing dongle a
    /// <see cref="LicensingException"/> will be thrown.
    /// </remarks>
    public class LicenseLock
    {
        #region Fields

        private Ultrapro _ultraPro;
        private Authentication _authentication;

        private ushort[] _designIds = new ushort[] { 0xACE1, 0x4F5A, 0xF26B };  // multiple design IDs are available
        private const int _designIdSorterIndex = 1;

        private const ushort _developerId = 0x8af5;
        private const int _featureCellAddress = 0x9000C;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the <see cref="Authentication"/> value from the last license <see cref="Verify()"/>.
        /// </summary>
        /// <value>The type of authentication from the last license verify.</value>
        /// <seealso cref="Authentication"/>
        public Authentication Authentication
        {
            get { return _authentication; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Xyratex users require a key.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a key is needed for Xyratex users; otherwise, <c>false</c>.
        /// </value>
        public bool XyratexKeyNeeded
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseLock"/> class.
        /// This also includes a license verification call (see <see cref="Verify()"/>).
        /// Check all available design IDs until one is found
        /// </summary>
        public LicenseLock()
        {
            ExtractEmbeddedAssembly();

            _ultraPro = new Ultrapro();

            int errCode = _ultraPro.SFNTsntlInitialize();
            if (errCode != Ultrapro.SP_ERR_SUCCESS)
                throw new LicensingException("Cannot initialize Software License Sentinel.", errCode);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseLock"/> class with the specified dongle DesignID.
        /// This also includes a license verification call (see <see cref="Verify()"/>).
        /// </summary>
        /// <param name="designId">The dongle designID.</param>
        /// <exception cref="LicensingException">The Sentinel UltraPro failed to initialize.</exception>
        public LicenseLock(ushort designId)
        {
            ExtractEmbeddedAssembly();

            _ultraPro = new Ultrapro();

            int errCode = _ultraPro.SFNTsntlInitialize();
            if (errCode != Ultrapro.SP_ERR_SUCCESS)
                throw new LicensingException("Cannot initialize Software License Sentinel.", errCode);
        }

        /// <summary>
        /// Releases the Ultrapro dongle license.
        /// </summary>
        ~LicenseLock()
        {
            _ultraPro.SFNTsntlCleanup();
        }

        private void ExtractEmbeddedAssembly()
        {
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePath = Path.Combine(assemblyDir, "ux32w.dll");

            if (File.Exists(filePath))
                return;

            byte[] resource = Environment.Is64BitProcess ? Properties.Resources.ux32w_64 : Properties.Resources.ux32w_32;

            using (FileStream fs = new FileStream(filePath, FileMode.CreateNew))
            {
                fs.Write(resource, 0, resource.Length);
            }
        }

        /// <summary>
        /// Verifies the security dongle license (except for Xyratex developers).
        /// This releases and re-acquires the UltraPro license using our developerID and designID.
        /// </summary>
        /// <returns>The <see cref="Authentication"/> result from the verification.</returns>
        public Authentication Verify()
        {
            Authentication auth = null;

            for (int i = 0; i < _designIds.Length; i++)
            {
                auth = Verify(_designIds[i]);
                if (auth.IsValid())
                    return auth;
            }

            // Failed
            return auth;
        }

        /// <summary>
        /// Verifies the security dongle license (except for Xyratex developers).
        /// This releases and re-acquires the UltraPro license using our developerID and the specified designID.
        /// </summary>
        /// <param name="designId">The design id.</param>
        /// <returns>The <see cref="Authentication"/> result from the verification.</returns>
        public Authentication Verify(ushort designId)
        {
            if (!XyratexKeyNeeded)
            {
                // Check for user authentication
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                if (identity.Name.StartsWith("XYUS\\", StringComparison.CurrentCultureIgnoreCase) ||
                    identity.Name.StartsWith("XY01\\", StringComparison.CurrentCultureIgnoreCase))
                {
                    _authentication = new Authentication(LicenseStatus.UserId, (LicenseFeatures)0xFFFF, LicenseKeyCode.SUCCESS);
                    _authentication.Details = "Xyratex user enabled license.";
                    return _authentication;
                }
            }

            _authentication = new Licensing.Authentication(LicenseStatus.Failed, LicenseFeatures.None, LicenseKeyCode.UNIT_NOT_FOUND);

            int errCode = _ultraPro.SFNTsntlGetLicense(_developerId, designId, 0);
            if (errCode != Ultrapro.SP_ERR_SUCCESS)
            {
                LicenseKeyCode code = (LicenseKeyCode)errCode;

                _authentication = new Authentication(LicenseStatus.Failed, LicenseFeatures.None, code);
                _authentication.Details = "Key detection failed. " + code.GetDescription();

                return _authentication;
            }

            try
            {
                int keyIndex = designId;
                for (int i = 0; i < _designIds.Length; i++)
                {
                    if (designId == _designIds[i])
                    {
                        keyIndex = i + 1;

                        if (i == _designIdSorterIndex)
                        {
                            _authentication = new Authentication(LicenseStatus.Key, LicenseFeatures.DEPO, LicenseKeyCode.SUCCESS);
                            _authentication.Details = String.Format("Verified key ({0:X}).", keyIndex);
                            return _authentication;
                        }
                        
                        break;
                    }
                }

                uint data = 0;
                _ultraPro.SFNTsntlReadValue(_featureCellAddress, out data);

                LicenseFeatures keyFeatures = (LicenseFeatures)data;

                _authentication = new Authentication(LicenseStatus.Key, keyFeatures, LicenseKeyCode.SUCCESS);
                _authentication.Details = String.Format("Verified key ({0:X}).", keyIndex);
                return _authentication;
            }
            finally
            {
                ushort userNum = 0;
                _ultraPro.SFNTsntlReleaseLicense(0, ref userNum);
            }
        }

        /// <summary>
        /// [Used internally for key setup]
        /// </summary>
        /// <param name="features">[internal use only]</param>
        /// <param name="writePass">[internal use only]</param>
        /// <param name="owp1">[internal use only]</param>
        /// <param name="owp2">[internal use only]</param>
        /// <returns></returns>
        public LicenseKeyCode SetFeatures(LicenseFeatures features, ushort writePass, ushort owp1, ushort owp2)
        {
            int errCode = Ultrapro.SP_ERR_UNIT_NOT_FOUND;

            for (int i = 0; i < _designIds.Length; i++)
            {
                errCode = _ultraPro.SFNTsntlGetLicense(_developerId, _designIds[i], 0);
                if (errCode == Ultrapro.SP_ERR_SUCCESS)
                    break;
            }

            if (errCode != Ultrapro.SP_ERR_SUCCESS)
                return (LicenseKeyCode)errCode;

            try
            {
                errCode = _ultraPro.SFNTsntlUnlockData(_featureCellAddress, writePass, owp1, owp2);
                if (errCode != Ultrapro.SP_ERR_SUCCESS)
                    return (LicenseKeyCode)errCode;

                byte flag = 0;
                errCode = _ultraPro.SFNTsntlWriteValue(_featureCellAddress, (uint)features, flag, writePass);

                if (errCode == Ultrapro.SP_ERR_SUCCESS)
                    _ultraPro.SFNTsntlLockData(_featureCellAddress, writePass);

                return (LicenseKeyCode)errCode;
            }
            finally
            {
                ushort userNum = 0;
                _ultraPro.SFNTsntlReleaseLicense(0, ref userNum);
            }
        }

        #endregion Methods
    }

    #endregion LicenseLock Class

    #region XyratexOSC.Licensing Namespace Documentation

    /// <summary>
    /// <para>The <see cref="XyratexOSC.Licensing"/> namespace is used for checking the XyratexOSC licensing dongle.
    /// It contains the <see cref="LicenseLock"/> wrapper class that connects to and verifies a valid Sentinel UltraPro
    /// security dongle is attached to the system.</para>
    /// </summary>
    /// <remarks>
    /// The <see cref="LicenseLock"/> class verifies a valid license dongle is plugged into the computer on instatiation
    /// and through a call to verification (see <see cref="LicenseLock.Verify()"/> that can be made at any time throughout the application.
    /// If a valid license is found the class does nothing, but if there is any problem with the licensing dongle a
    /// <see cref="LicensingException"/> will be thrown.
    /// </remarks>
    /// <example>
    /// This example shows how to add license verification to an application
    /// <code>
    /// // Instantiation includes the verification routine. This secures the application at a minimum.
    /// LicenseLock license = new LicenseLock();
    ///
    /// // Verification can occur anywhere else in your application with the following call
    /// license.Verify();
    /// </code>
    /// </example>
    /// <seealso cref="LicenseLock"/>
    /// <seealso cref="LicensingException"/>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class NamespaceDoc
    {
        //An empty class used by Sandcastle Help File Builder (http://shfb.codeplex.com/)
        //to create a summary for our Namespace
    }

    #endregion XyratexOSC.Licensing Namespace Documentation
}