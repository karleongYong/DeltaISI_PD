using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XyratexOSC.Settings.UI
{
    /// <summary>
    /// A form for displaying an editor that shows/changes application settings
    /// </summary>
    public partial class SettingsEditForm : Form
    {
        private bool _cancelClosed = false;

        /// <summary>
        /// Gets or sets the <see cref="SettingsEditor"/> to be used in this form.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public SettingsEditor Editor
        {
            get
            {
                if (this.Controls.Count < 1)
                    return null;

                return this.Controls[0] as SettingsEditor;
            }
            set
            {
                if (this.Controls.Count > 0)
                {
                    if (this.Controls[0] == value)
                        return;

                    SettingsEditor oldEditor = this.Controls[0] as SettingsEditor;

                    if (oldEditor != null)
                        oldEditor.Cancelled -= Editor_Cancelled;

                    this.Controls.Clear();
                }

                SettingsEditor editor = value as SettingsEditor;

                if (editor == null)
                    return;

                editor.Dock = DockStyle.Fill;
                editor.Cancelled += Editor_Cancelled;

                this.Controls.Add(editor);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsEditForm"/> class.
        /// </summary>
        public SettingsEditForm()
            : this(null)
        {
            //For the VS designer
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsEditForm"/> class.
        /// </summary>
        /// <param name="editor">The editor.</param>
        public SettingsEditForm(SettingsEditor editor) 
        {
            InitializeComponent();
            Editor = editor;
        }

        private void SettingsEditForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();

            if (e.CloseReason == CloseReason.UserClosing || _cancelClosed)
                e.Cancel = true;

            _cancelClosed = false;
        }

        private void Editor_Cancelled(object sender, EventArgs e)
        {
            _cancelClosed = true;
            this.Close();
        }
    }
}
