using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using XyratexOSC.UI;

namespace XyratexOSC.Settings.UI
{
    /// <summary>
    /// Settings editor that is based on the treeview control.
    /// </summary>
    public class TreeViewEditor : SettingsEditor
    {
        private TreeView treeView;
        private TableLayoutPanel itemLayoutPanel;
        private Label lblName;
        private Label lblDescendants;
        private RichTextBox detailsBox;
        private System.ComponentModel.IContainer components;
        private ToolTip toolTipError;
        private SplitContainer splitContainer1;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewEditor"/> class.
        /// </summary>
        public TreeViewEditor()
            : base()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when refreshing settings.
        /// </summary>
        protected override void OnRefreshSettings()
        {
            UpdateTreeView();
            UpdateInfo();

            IsDirty = false;
        }

        /// <summary>
        /// Saves settings using settings document.
        /// </summary>
        protected override void OnSave()
        {
            base.OnSave();
        }

        /// <summary>
        /// Loads settings from settings document.
        /// </summary>
        protected override void OnLoad()
        {
            if (SettingsDocument != null)
            {
                SettingsDocument.NodeChanged -= SettingsNodeChanged;
                SettingsDocument.NodeInserted -= SettingsNodeInserted;
                SettingsDocument.NodeRemoved -= SettingsNodeRemoved;
            }

            base.OnLoad();
            UpdateTreeView();

            SettingsDocument.NodeChanged += new SettingsNodeChangeEventHandler(SettingsNodeChanged);
            SettingsDocument.NodeInserted += new SettingsNodeChangeEventHandler(SettingsNodeInserted);
            SettingsDocument.NodeRemoved += new SettingsNodeChangeEventHandler(SettingsNodeRemoved);

            if (_settingsObject != null)
                SettingsConverter.UpdateObjectFromNode(_settingsObject, SettingsDocument);
        }

        private void SettingsNodeChanged(SettingsNode sender, SettingsNodeChangeEventArgs e)
        {
            string[] nodeNames = e.Node.FullPath.Split(e.Node.PathSeparator);

            if (nodeNames.Length < 1)
                return;

            TreeNode changedNode = treeView.Nodes[nodeNames[0]];
            for (int i = 1; i < nodeNames.Length; i++)
            {
                if (changedNode == null)
                    return;

                changedNode = changedNode.Nodes[nodeNames[i]];
            }

            UIUtility.Invoke(this, () =>
            {
                TreeNode valueNode = changedNode.FirstNode;
                valueNode.Text = e.Node.Value.ToString();
                valueNode.Name = valueNode.Text;
            });
        }

        private void SettingsNodeInserted(SettingsNode sender, SettingsNodeChangeEventArgs e)
        {
            string[] nodeNames = e.Node.FullPath.Split(e.Node.PathSeparator);

            if (nodeNames.Length < 1)
                return;

            TreeNodeCollection nodes = treeView.Nodes;

            for (int i = 0; i < nodeNames.Length - 1; i++)
            {
                TreeNode node = nodes[nodeNames[i]];
                if (node == null)
                    return;

                nodes = node.Nodes;
            }

            UIUtility.Invoke(this, () =>
            {
                TreeNode newNode = new TreeNode(e.Node.Name);
                newNode.Name = newNode.Text;
                newNode.Tag = e.Node;
                nodes.Add(newNode);
            });
        }

        private void SettingsNodeRemoved(SettingsNode sender, SettingsNodeChangeEventArgs e)
        {
            string[] nodeNames = e.Node.FullPath.Split(e.Node.PathSeparator);

            if (nodeNames.Length < 1)
                return;

            TreeNodeCollection nodes = treeView.Nodes;

            for (int i = 0; i < nodeNames.Length - 1; i++)
            {
                TreeNode node = nodes[nodeNames[i]];
                if (node == null)
                    return;

                nodes = node.Nodes;
            }

            UIUtility.Invoke(this, () =>
            {
                if (nodes.ContainsKey(e.Node.Name))
                    nodes.RemoveByKey(e.Node.Name);
            });
        }

        /// <summary>
        /// Called when accept is clicked.
        /// </summary>
        protected override void OnAccept()
        {
            UpdateSettingsNodes(treeView.Nodes);

            UpdateInfo();

            SettingsConverter.UpdateObjectFromNode(_settingsObject, SettingsDocument);

            IsDirty = false;
        }

        /// <summary>
        /// Restores settings prior to firing the cancelled event.
        /// </summary>
        protected override void OnCancel()
        {
            RefreshSettings();
        }

        private void UpdateSettingsNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text != ((SettingsNode)node.Tag).Name)
                {
                    try
                    {
                        SettingsNode setting = node.Tag as SettingsNode;

                        if (setting == null)
                            continue;

                        if (setting.Name == node.Text)
                            continue;

                        setting.Name = node.Text;

                        if (setting.IsAValue)
                            Logging.Log.Info("Settings", "{0} = {1}", setting.Parent.FullPath, node.Text);
                    }
                    catch (InvalidCastException)
                    {
                        node.Text = ((SettingsNode)node.Tag).Name;
                    }
                }
                UpdateSettingsNodes(node.Nodes);
            }
        }

        /// <summary>
        /// Updates the TreeView with the current SettingsDocument.
        /// </summary>
        public void UpdateTreeView()
        {
            if (SettingsDocument != null)
                CopySettingsToTree(SettingsDocument.Nodes, treeView.Nodes);

            treeView.Refresh();
        }

        /// <summary>
        /// Used internally to deeply copy the SettingsDocument.
        /// </summary>
        /// <param name="settingsCollection">The current settings collection.</param>
        /// <param name="treeCollection">The current tree node collection.</param>
        private void CopySettingsToTree(SettingsNodeList settingsCollection, TreeNodeCollection treeCollection)
        {
            if (settingsCollection == null)
                return;

            List<TreeNode> validTreeNodes = new List<TreeNode>();
            List<TreeNode> invalidTreeNodes = new List<TreeNode>();

            for (int i = 0; i < settingsCollection.Count; i++)
            {
                if (settingsCollection[i] != null)
                {
                    string nodeName = settingsCollection[i].Name;
                    TreeNode treeNode = null;

                    if (!treeCollection.ContainsKey(nodeName))
                    {
                        treeNode = new TreeNode(nodeName);
                        treeNode.Name = nodeName;
                        treeNode.Tag = settingsCollection[i];
                        treeCollection.Add(treeNode);
                    }
                    else
                    {
                        treeNode = treeCollection[nodeName];
                        treeNode.Tag = settingsCollection[i];
                    }

                    if (treeNode.Text != treeNode.Name)
                        treeNode.Text = treeNode.Name;

                    validTreeNodes.Add(treeNode);

                    CopySettingsToTree(settingsCollection[i].Nodes, treeNode.Nodes);
                }
            }

            foreach (TreeNode node in treeCollection)
                if (!validTreeNodes.Contains(node))
                    invalidTreeNodes.Add(node);

            foreach (TreeNode node in invalidTreeNodes)
                treeCollection.Remove(node);
        }


        private void UpdateInfo()
        {
            if (SettingsDocument == null)
                return;

            if (treeView.SelectedNode == null || treeView.SelectedNode.Tag == null)
            {
                lblDescendants.Text = "";
                lblName.Text = "(select a setting)";
                detailsBox.Text = "";
                return;
            }

            SettingsNode selectedNode = (SettingsNode)treeView.SelectedNode.Tag;
            SettingsNode editingNode = selectedNode;

            if (selectedNode.IsAValue)
                editingNode = selectedNode.Parent;

            if (editingNode.Parent == null)
                lblDescendants.Text = "";
            else
                lblDescendants.Text = editingNode.Parent.FullPath.Replace(SettingsDocument.PathSeparator.ToString(), " > ");

            lblName.Text = editingNode.Name;
            
            if (selectedNode.IsAValue || selectedNode.HasAValue)
                detailsBox.Text = String.Format("Value: {0}\n\n{1}{2}", editingNode.Value, selectedNode.ReadOnly ? "(Read-Only)\n\n" : "", editingNode.Info).Trim();
            else
                detailsBox.Text = editingNode.Info.Trim();
        }

        private void tree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            SettingsNode setting = e.Node.Tag as SettingsNode;

            if (setting == null || !setting.IsAValue)
                return;

            try
            {
                object value = SettingsConverter.ConvertValue(e.Label, setting.Type);
            }
            catch (Exception ex)
            {
                Logging.Log.Error("Settings", "{0} cannot be applied to '{1}'. {2}", e.Label, setting.Parent.FullPath, ex.Message);
                toolTipError.Show(String.Format("Restored original value.", e.Label), treeView, e.Node.Bounds.Right + 10, e.Node.Bounds.Top - 5, 2000);
                e.CancelEdit = true;
                return;
            }

            IsDirty = (setting.Name != e.Label) || TreeNodesDirty(treeView.Nodes, e.Node);
        }

        private bool TreeNodesDirty(TreeNodeCollection treeCollection, TreeNode editingNode)
        {
            if (treeCollection == null)
                return false;

            foreach (TreeNode treeNode in treeCollection)
            {
                if (treeNode == editingNode) //skip currently editing node
                    continue;

                if (treeNode.Text != treeNode.Name)
                    return true;

                if (TreeNodesDirty(treeNode.Nodes, editingNode))
                    return true;
            }

            return false;
        }

        private void tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateInfo();
        }

        private void tree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // If this is not a value node or set to read only, cancel the edit.
            if (SettingsDocument == null || treeView.SelectedNode.Nodes.Count > 0 || ((SettingsNode)treeView.SelectedNode.Tag).ReadOnly)
                e.CancelEdit = true;
        }

        private void EditSelectedValue()
        {
            if (SettingsDocument == null)
                return;

            if (treeView.SelectedNode != null)
            {
                if (treeView.SelectedNode.Nodes.Count == 0 && treeView.SelectedNode.Parent != null)
                {
                    treeView.SelectedNode.BeginEdit();
                }
                else if (treeView.SelectedNode.Nodes.Count == 1 && treeView.SelectedNode.Nodes[0].Nodes.Count == 0)
                {
                    treeView.SelectedNode = treeView.SelectedNode.Nodes[0];
                    treeView.SelectedNode.BeginEdit();
                }
            }
        }

        private void tree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (treeView.SelectedNode == null)
                return;

            if (treeView.SelectedNode.Parent != e.Node)
                EditSelectedValue();
        }

        private void splitContainer1_Panel2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            EditSelectedValue();
        }

        private void lblName_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            EditSelectedValue();
        }

        private void descBox_DoubleClick(object sender, EventArgs e)
        {
            EditSelectedValue();
        }

        private void lblName_DoubleClick(object sender, EventArgs e)
        {
            EditSelectedValue();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.itemLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblName = new System.Windows.Forms.Label();
            this.lblDescendants = new System.Windows.Forms.Label();
            this.detailsBox = new System.Windows.Forms.RichTextBox();
            this.toolTipError = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.itemLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 34);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.itemLayoutPanel);
            this.splitContainer1.Size = new System.Drawing.Size(541, 476);
            this.splitContainer1.SplitterDistance = 198;
            this.splitContainer1.TabIndex = 3;
            // 
            // treeView
            // 
            this.treeView.BackColor = System.Drawing.SystemColors.ControlLight;
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.FullRowSelect = true;
            this.treeView.HotTracking = true;
            this.treeView.LabelEdit = true;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Margin = new System.Windows.Forms.Padding(0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(196, 474);
            this.treeView.TabIndex = 0;
            this.treeView.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tree_BeforeLabelEdit);
            this.treeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tree_AfterLabelEdit);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterSelect);
            this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tree_NodeMouseDoubleClick);
            // 
            // itemLayoutPanel
            // 
            this.itemLayoutPanel.BackColor = System.Drawing.SystemColors.Control;
            this.itemLayoutPanel.ColumnCount = 1;
            this.itemLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.itemLayoutPanel.Controls.Add(this.lblName, 0, 1);
            this.itemLayoutPanel.Controls.Add(this.lblDescendants, 0, 0);
            this.itemLayoutPanel.Controls.Add(this.detailsBox, 0, 2);
            this.itemLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.itemLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.itemLayoutPanel.Name = "itemLayoutPanel";
            this.itemLayoutPanel.RowCount = 3;
            this.itemLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.itemLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.itemLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.itemLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.itemLayoutPanel.Size = new System.Drawing.Size(337, 474);
            this.itemLayoutPanel.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(3, 22);
            this.lblName.Name = "lblName";
            this.lblName.Padding = new System.Windows.Forms.Padding(3);
            this.lblName.Size = new System.Drawing.Size(331, 24);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "(select a setting)";
            // 
            // lblDescendants
            // 
            this.lblDescendants.AutoSize = true;
            this.lblDescendants.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDescendants.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescendants.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblDescendants.Location = new System.Drawing.Point(3, 0);
            this.lblDescendants.Name = "lblDescendants";
            this.lblDescendants.Padding = new System.Windows.Forms.Padding(3);
            this.lblDescendants.Size = new System.Drawing.Size(331, 22);
            this.lblDescendants.TabIndex = 0;
            // 
            // detailsBox
            // 
            this.detailsBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.detailsBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsBox.Location = new System.Drawing.Point(8, 54);
            this.detailsBox.Margin = new System.Windows.Forms.Padding(8);
            this.detailsBox.Name = "detailsBox";
            this.detailsBox.ReadOnly = true;
            this.detailsBox.Size = new System.Drawing.Size(321, 412);
            this.detailsBox.TabIndex = 2;
            this.detailsBox.Text = "";
            // 
            // toolTipError
            // 
            this.toolTipError.AutomaticDelay = 100;
            this.toolTipError.BackColor = System.Drawing.Color.Tomato;
            this.toolTipError.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Error;
            this.toolTipError.ToolTipTitle = "Invalid Value";
            // 
            // TreeViewEditor
            // 
            this.Controls.Add(this.splitContainer1);
            this.Name = "TreeViewEditor";
            this.Controls.SetChildIndex(this.splitContainer1, 0);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.itemLayoutPanel.ResumeLayout(false);
            this.itemLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
