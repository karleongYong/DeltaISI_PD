using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace XyratexOSC.UI
{
    /// <summary>
    /// Provides an About Dialog with version and copyright information.
    /// </summary>
    public partial class AboutForm : Form
    {
        private string _copyrightThirdParty = "";

        /// <summary>
        /// Gets or sets the Seagate copyright information.
        /// </summary>
        /// <value>
        /// Our copyright.
        /// </value>
        public string CopyrightXyratex
        {
            get
            {
                return lblCopyright.Text;
            }
            set
            {
                lblCopyright.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the copyright information of third party libraries.
        /// </summary>
        /// <value>
        /// The third party copyright information.
        /// </value>
        public string CopyrightThirdParty
        {
            get
            {
                return _copyrightThirdParty;
            }
            set
            {
                _copyrightThirdParty = value;

                List<string> assemblies = GetAssemblies();

                txtAssemblies.Text = "Loaded Assemblies:" + Environment.NewLine + Environment.NewLine;
                txtAssemblies.Text += String.Join(Environment.NewLine, assemblies) + Environment.NewLine + Environment.NewLine;
                txtAssemblies.Text += _copyrightThirdParty;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutForm"/> class.
        /// </summary>
        public AboutForm()
        {
            InitializeComponent();
        }

        #region FormMove

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        /// <summary>
        /// Sends a user-input message to the operating system.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="Msg">The MSG.</param>
        /// <param name="wParam">The w parameter.</param>
        /// <param name="lParam">The l parameter.</param>
        /// <returns></returns>
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        /// <summary>
        /// Releases the user-input capture.
        /// </summary>
        /// <returns></returns>
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void mainPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        #endregion

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            lblProduct.Text = Application.ProductName;
            lblVersion.Text = "Version " + Application.ProductVersion;
        }

        /// <summary>
        /// Gets a list of the assembly names that are referenced in the main application.
        /// </summary>
        /// <returns>The list of names</returns>
        public static List<string> GetAssemblies()
        {
            Assembly appExe = Assembly.GetEntryAssembly();
            return GetAssemblies(0, new Assembly[] { appExe });
        }

        private static List<string> GetAssemblies(int depth, Assembly[] assembliesLoaded)
        {
            List<string> assemblies = new List<string>();

            if (depth > 2)
                return assemblies;

            string depthString = "";

            for (int i = 0; i < depth; i++)
                depthString += "   ";

            if (depth > 0)
                depthString += "> ";

            foreach (Assembly assembly in assembliesLoaded)
            {
                AssemblyName name = assembly.GetName();

                if (name.Name.StartsWith("System") ||
                    name.Name.StartsWith("Microsoft") ||
                    name.Name.StartsWith("Windows") ||
                    name.Name.StartsWith("mscorlib"))
                    continue;

                try
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

                    string infoVersion = fvi.FileVersion + " | " + fvi.ProductVersion;

                    string assemblyInfo = String.Format("{0}{1} ({2}) [{3}]", depthString, name.Name, infoVersion, assembly.Location);
                    assemblies.Add(assemblyInfo);
                }
                catch (Exception)
                {
                    // Ignore error since we are just logging.  In some cases where an "Anonymous Assembly" is loaded (not sure how it come about),
                    // it causes Invoke Except from the above.
                }

                List<Assembly> childAssemblies = new List<Assembly>();

                foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
                {
                    if (assemblyName.Name.StartsWith("System") ||
                        assemblyName.Name.StartsWith("Microsoft") ||
                        assemblyName.Name.StartsWith("mscorlib"))
                        continue;

                    Assembly childAssembly = Assembly.ReflectionOnlyLoad(assemblyName.FullName);
                    childAssemblies.Add(childAssembly);
                }

                List<string> childInfo = GetAssemblies(depth + 1, childAssemblies.ToArray());

                assemblies.AddRange(childInfo);
            }

            return assemblies;
        }
    }
}
