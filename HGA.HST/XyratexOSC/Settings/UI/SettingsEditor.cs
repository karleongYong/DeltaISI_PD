using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using XyratexOSC.UI;
using XyratexOSC.Utilities;

namespace XyratexOSC.Settings.UI
{
    /// <summary>
    /// Base control that provides the UI for editing settings objects
    /// </summary>
    public class SettingsEditor : UserControl
    {
        public event EventHandler SaveSettingsToXMLFile;        

        public event EventHandler LoadSettingsFromXMLFile;
        /// <summary>
        /// The settings object
        /// </summary>
        protected object _settingsObject;

        /// <summary>
        /// The save button
        /// </summary>
        protected Button btnSave;

        private Button btnAccept;

        /// <summary>
        /// The load button
        /// </summary>
        protected Button btnLoad;
        public Button buttonLoad;

        private FlowLayoutPanel buttonsPanel;

        /// <summary>
        /// Occurs when cancel is clicked.
        /// </summary>
        public event EventHandler Cancelled;

        /// <summary>
        /// Gets or sets the file path to be used when saving settings to file. 
        /// If <see cref="UseSaveFileDialog"/> is true, then this is used as the default file in the dialog.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use the save file dialog; otherwise saves to a single file.
        /// </summary>
        /// <value>
        ///   <c>true</c> if using the save file dialog; otherwise, <c>false</c>.
        /// </value>
        public bool UseSaveFileDialog
        {
            get;
            set;
        }

        /// <summary>
        /// In a TreeViewEditor, gets the current settings document. In a PropertyGridEditor, gets the last saved/loaded document.
        /// </summary>
        public SettingsDocument SettingsDocument
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether there are any edits to the original settings.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is dirty; otherwise, <c>false</c>.
        /// </value>
        public bool IsDirty
        {
            get
            {
                return btnAccept.Enabled;
            }
            protected set
            {
                btnAccept.Enabled = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsEditor"/> class.
        /// </summary>
        public SettingsEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the settings object.
        /// </summary>
        /// <param name="settingsObject">The settings object.</param>
        public virtual void SetSettingsObject(object settingsObject)
        {
            _settingsObject = settingsObject;
        }

        /// <summary>
        /// Gets the settings object.
        /// </summary>
        /// <returns></returns>
        public virtual object GetSettingsObject()
        {
            return _settingsObject;
        }

        /// <summary>
        /// Refreshes the settings view.
        /// </summary>
        public void RefreshSettings()
        {
            OnRefreshSettings();
        }

        /// <summary>
        /// Called when refreshing settings.
        /// </summary>
        protected virtual void OnRefreshSettings()
        {
        
        }

        /// <summary>
        /// Called when 'Save' button is clicked.
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                Accept();
            }
            catch (Exception ex)
            {
                Logging.Log.Error("Settings", ex);
                MessageBox.Show(String.Format("Failed to apply settings: {0}{1}", Environment.NewLine, ex.Message));
                return;
            }

            if (string.IsNullOrEmpty(FilePath) || UseSaveFileDialog)
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.InitialDirectory = System.IO.Path.GetDirectoryName(FilePath);
                    sfd.FileName = FilePath;
                    sfd.Filter = "Settings File (*.stp)|*.stp|Encrypted Settings File (*.stpc)|*.stpc|Strong-Encrypted Settings File (*.stpe)|*.stpe|All files|*.*";
                    sfd.OverwritePrompt = true;

                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;

                    if (String.IsNullOrEmpty(sfd.FileName))
                        return;

                    FilePath = sfd.FileName;
                }
            }

            if (String.IsNullOrEmpty(FilePath))
                return;

            try
            {
                OnSave();
            }
            catch (Exception ex)
            {
                Logging.Log.Error("Settings", ex);
                MessageBox.Show(String.Format("Failed to save settings to '{0}'.{1}{2}", FilePath, Environment.NewLine, ex.Message));
            }
        }

        /// <summary>
        /// Saves settings using settings document.
        /// </summary>
        protected virtual void OnSave()
        {
            if (SettingsDocument != null)
            {
                SettingsFileOption fileOption = SettingsFileOption.Default;

                string extension = Path.GetExtension(FilePath);
                if (String.Equals(extension, ".stpe", StringComparison.InvariantCultureIgnoreCase))
                    fileOption = SettingsFileOption.Encrypted;
                else if (String.Equals(extension, ".stpc", StringComparison.InvariantCultureIgnoreCase))
                    fileOption = SettingsFileOption.Compressed;

                SettingsDocument.Save(FilePath, fileOption);
            }
        }

        /// <summary>
        /// Loads the settings from the specified filepath.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void LoadSettings(string filePath)
        {
            try
            {
                OnLoad();
                RefreshSettings();
            }
            catch (Exception ex)
            {
                Logging.Log.Error("Settings", ex);
                MessageBox.Show(String.Format("Failed to load settings from '{0}'.{1}{2}", FilePath, Environment.NewLine, ex.Message));
            }
        }

        /// <summary>
        /// Called when 'Load' button is clicked.
        /// </summary>
        public void LoadSettings()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.FileName = FilePath;
                ofd.Filter = "Settings File (*.stp;*.stpe;*.stpc)|*.stp;*.stpe;*.stpc|All files|*.*";

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                if (String.IsNullOrEmpty(ofd.FileName))
                    return;

                FilePath = ofd.FileName;
            }

            LoadSettings(FilePath);
        }

        /// <summary>
        /// Loads settings from settings document.
        /// </summary>
        protected virtual void OnLoad()
        {
            if (SettingsDocument == null)
                SettingsDocument = new SettingsDocument(FilePath);
            else
                SettingsDocument.Load(FilePath);
        }

        /// <summary>
        /// Called when 'Accept' button is clicked.
        /// </summary>
        public void Accept()
        {
            try
            {
                OnAccept();
            }
            catch (Exception ex)
            {
                Logging.Log.Error("Settings", ex);
                MessageBox.Show(String.Format("Failed to apply settings: {0}{1}", Environment.NewLine, ex.Message));
            }
        }

        /// <summary>
        /// Called when accept is clicked.
        /// </summary>
        protected virtual void OnAccept()
        {
        
        }

        /// <summary>
        /// Called when 'Cancel' button is clicked.
        /// </summary>
        public void Cancel()
        {
            OnCancel();

            RefreshSettings();

            if (Cancelled != null)
                Cancelled(this, new EventArgs());
        }

        /// <summary>
        /// Restores settings prior to firing the <see cref="Cancelled"/> event.
        /// </summary>
        protected virtual void OnCancel()
        {

        }

        private void InitializeComponent()
        {
            this.buttonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSave = new System.Windows.Forms.Button();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            this.buttonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonsPanel
            // 
            this.buttonsPanel.Controls.Add(this.btnSave);
            this.buttonsPanel.Controls.Add(this.buttonLoad);
            this.buttonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonsPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonsPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonsPanel.Location = new System.Drawing.Point(0, 476);
            this.buttonsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.buttonsPanel.Name = "buttonsPanel";
            this.buttonsPanel.Size = new System.Drawing.Size(541, 34);
            this.buttonsPanel.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(443, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(95, 29);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(342, 3);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(95, 29);
            this.buttonLoad.TabIndex = 2;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Visible = false;
            this.buttonLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(118, 3);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(95, 29);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(219, 3);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(95, 29);
            this.btnAccept.TabIndex = 2;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // SettingsEditor
            // 
            this.Controls.Add(this.buttonsPanel);
            this.Name = "SettingsEditor";
            this.Size = new System.Drawing.Size(541, 510);
            this.VisibleChanged += new System.EventHandler(this.SettingsEditor_VisibleChanged);
            this.buttonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void SettingsEditor_VisibleChanged(object sender, EventArgs e)
        {
            RefreshSettings();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            OnAccept();
            SaveSettingsToXMLFile.SafeInvoke(this, e);
//            Notify.PopUp("Configuration File Saved", "Updated settings have been saved to disk", "", "OK");
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadSettingsFromXMLFile.LoadInvoke(this, e);
            //Notify.PopUp("Configuration File Loaded", "Updated settings have been loaded from disk", "", "OK");
            //LoadSettings();
            RefreshSettings();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                Accept();
            }
            catch (Exception ex)
            {
                Logging.Log.Error("Settings", ex);
                MessageBox.Show(String.Format("Failed to apply settings: {0}{1}.", Environment.NewLine, ex.Message));
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Cancel();
        }
    }
}
