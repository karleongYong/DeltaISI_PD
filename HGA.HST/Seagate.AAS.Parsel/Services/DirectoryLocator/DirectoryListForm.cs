//
//
//  © Copyright 2003 Seagate LLC.
//  All Rights Reserved.
//
//  NOTICE: This file contains source code, ideas, techniques, and 
//  information (the Information) which are Proprietary and Confidential 
//  Information of Seagate LLC. This Information may not be used by or 
//  disclosed to any third party except under written license, and shall 
//  be subject to the limitations prescribed under license.
//
//  [9/26/2005] by Sabrina Murray
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Seagate.AAS.Parsel
{
	/// <summary>
	/// DirectoryListForm
	///		Provides a user interface for the directory locater service. 
	/// </summary>

	public class DirectoryListForm : System.Windows.Forms.Form
	{
		// Nested declarations -------------------------------------------------
		enum CheckNaming
		{
			Correct,
			DirectoryIncorrect,
			TypeIncorrect,
			DirAndTypeIncorrect,
			Incorrect
		};

		// Member variables ----------------------------------------------------
		private System.Windows.Forms.Button buttonDeleteDirectory;
		private System.Windows.Forms.Button buttonSaveDirectoryList;
		private System.Windows.Forms.Label labelDirPath;
		private System.Windows.Forms.Label labelDirName;
		private System.Windows.Forms.Label labelSelectedDirectory;	
		private System.Windows.Forms.Button buttonAddNewDir;
		private Seagate.AAS.UI.TouchscreenTextBox touchscreenTextBoxDirPath;
		private Seagate.AAS.UI.TouchscreenTextBox touchscreenTextBoxDirName;
		private System.Windows.Forms.TreeView treeViewDirectoryList;	
		private System.Windows.Forms.TextBox textBoxSelectedDirectory;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private Services.DirectoryLocator dirLocator = new Services.DirectoryLocator();

		// Constructors & Finalizers -------------------------------------------
		public DirectoryListForm()
		{
			InitializeComponent();
			FillTreeView();
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		// Properties ----------------------------------------------------------
		// Methods  ----------------------------------------------------------
		// Events -------------------------------------------------------------		

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("C:");
            this.treeViewDirectoryList = new System.Windows.Forms.TreeView();
            this.buttonAddNewDir = new System.Windows.Forms.Button();
            this.buttonDeleteDirectory = new System.Windows.Forms.Button();
            this.buttonSaveDirectoryList = new System.Windows.Forms.Button();
            this.textBoxSelectedDirectory = new System.Windows.Forms.TextBox();
            this.labelSelectedDirectory = new System.Windows.Forms.Label();
            this.touchscreenTextBoxDirPath = new Seagate.AAS.UI.TouchscreenTextBox();
            this.touchscreenTextBoxDirName = new Seagate.AAS.UI.TouchscreenTextBox();
            this.labelDirPath = new System.Windows.Forms.Label();
            this.labelDirName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // treeViewDirectoryList
            // 
            this.treeViewDirectoryList.Location = new System.Drawing.Point(22, 44);
            this.treeViewDirectoryList.Name = "treeViewDirectoryList";
            treeNode1.Name = "";
            treeNode1.Text = "C:";
            this.treeViewDirectoryList.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.treeViewDirectoryList.Size = new System.Drawing.Size(330, 328);
            this.treeViewDirectoryList.TabIndex = 2;
            this.treeViewDirectoryList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewDirectoryList_AfterSelect);
            // 
            // buttonAddNewDir
            // 
            this.buttonAddNewDir.Location = new System.Drawing.Point(22, 378);
            this.buttonAddNewDir.Name = "buttonAddNewDir";
            this.buttonAddNewDir.Size = new System.Drawing.Size(110, 34);
            this.buttonAddNewDir.TabIndex = 3;
            this.buttonAddNewDir.Text = "Add New Directory";
            this.buttonAddNewDir.Click += new System.EventHandler(this.buttonAddNewViperDir_Click);
            // 
            // buttonDeleteDirectory
            // 
            this.buttonDeleteDirectory.Location = new System.Drawing.Point(240, 378);
            this.buttonDeleteDirectory.Name = "buttonDeleteDirectory";
            this.buttonDeleteDirectory.Size = new System.Drawing.Size(110, 34);
            this.buttonDeleteDirectory.TabIndex = 4;
            this.buttonDeleteDirectory.Text = "Delete Directory";
            this.buttonDeleteDirectory.Click += new System.EventHandler(this.buttonDeleteDirectory_Click);
            // 
            // buttonSaveDirectoryList
            // 
            this.buttonSaveDirectoryList.Location = new System.Drawing.Point(95, 460);
            this.buttonSaveDirectoryList.Name = "buttonSaveDirectoryList";
            this.buttonSaveDirectoryList.Size = new System.Drawing.Size(185, 33);
            this.buttonSaveDirectoryList.TabIndex = 6;
            this.buttonSaveDirectoryList.Text = "Save Changes to Directory List";
            this.buttonSaveDirectoryList.Click += new System.EventHandler(this.buttonSaveDirectoryList_Click);
            // 
            // textBoxSelectedDirectory
            // 
            this.textBoxSelectedDirectory.Enabled = false;
            this.textBoxSelectedDirectory.Location = new System.Drawing.Point(22, 22);
            this.textBoxSelectedDirectory.Name = "textBoxSelectedDirectory";
            this.textBoxSelectedDirectory.Size = new System.Drawing.Size(330, 20);
            this.textBoxSelectedDirectory.TabIndex = 7;
            this.textBoxSelectedDirectory.Text = "C:\\Seagate\\Viper\\Bin";
            // 
            // labelSelectedDirectory
            // 
            this.labelSelectedDirectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSelectedDirectory.ForeColor = System.Drawing.Color.Teal;
            this.labelSelectedDirectory.Location = new System.Drawing.Point(22, 5);
            this.labelSelectedDirectory.Name = "labelSelectedDirectory";
            this.labelSelectedDirectory.Size = new System.Drawing.Size(136, 16);
            this.labelSelectedDirectory.TabIndex = 8;
            this.labelSelectedDirectory.Text = "Selected Directory:";
            // 
            // touchscreenTextBoxDirPath
            // 
            this.touchscreenTextBoxDirPath.AlphaNumOnly = false;
            this.touchscreenTextBoxDirPath.BackColor = System.Drawing.Color.White;
            this.touchscreenTextBoxDirPath.FormTitle = "Enter Text";
            this.touchscreenTextBoxDirPath.Location = new System.Drawing.Point(110, 438);
            this.touchscreenTextBoxDirPath.MinLength = 0;
            this.touchscreenTextBoxDirPath.Name = "touchscreenTextBoxDirPath";
            this.touchscreenTextBoxDirPath.NoWhiteSpace = false;
            this.touchscreenTextBoxDirPath.OnlyCaps = false;
            this.touchscreenTextBoxDirPath.Size = new System.Drawing.Size(235, 20);
            this.touchscreenTextBoxDirPath.TabIndex = 16;
            // 
            // touchscreenTextBoxDirName
            // 
            this.touchscreenTextBoxDirName.AlphaNumOnly = false;
            this.touchscreenTextBoxDirName.BackColor = System.Drawing.Color.White;
            this.touchscreenTextBoxDirName.FormTitle = "Enter Text";
            this.touchscreenTextBoxDirName.Location = new System.Drawing.Point(110, 416);
            this.touchscreenTextBoxDirName.MinLength = 0;
            this.touchscreenTextBoxDirName.Name = "touchscreenTextBoxDirName";
            this.touchscreenTextBoxDirName.NoWhiteSpace = false;
            this.touchscreenTextBoxDirName.OnlyCaps = false;
            this.touchscreenTextBoxDirName.Size = new System.Drawing.Size(115, 20);
            this.touchscreenTextBoxDirName.TabIndex = 15;
            // 
            // labelDirPath
            // 
            this.labelDirPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDirPath.ForeColor = System.Drawing.Color.Teal;
            this.labelDirPath.Location = new System.Drawing.Point(25, 438);
            this.labelDirPath.Name = "labelDirPath";
            this.labelDirPath.Size = new System.Drawing.Size(85, 16);
            this.labelDirPath.TabIndex = 14;
            this.labelDirPath.Text = "Directory Path";
            // 
            // labelDirName
            // 
            this.labelDirName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDirName.ForeColor = System.Drawing.Color.Teal;
            this.labelDirName.Location = new System.Drawing.Point(25, 416);
            this.labelDirName.Name = "labelDirName";
            this.labelDirName.Size = new System.Drawing.Size(85, 16);
            this.labelDirName.TabIndex = 12;
            this.labelDirName.Text = "Directory Name";
            // 
            // DirectoryListForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(379, 527);
            this.Controls.Add(this.touchscreenTextBoxDirPath);
            this.Controls.Add(this.touchscreenTextBoxDirName);
            this.Controls.Add(this.labelDirPath);
            this.Controls.Add(this.labelDirName);
            this.Controls.Add(this.buttonSaveDirectoryList);
            this.Controls.Add(this.labelSelectedDirectory);
            this.Controls.Add(this.textBoxSelectedDirectory);
            this.Controls.Add(this.buttonDeleteDirectory);
            this.Controls.Add(this.buttonAddNewDir);
            this.Controls.Add(this.treeViewDirectoryList);
            this.Name = "DirectoryListForm";
            this.Text = "DirectoryListForm";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void buttonAddNewViperDir_Click(object sender, System.EventArgs e)
		{
			string[] directoryNames = new string[10];
			char[] separator = new char[1];
			string path = touchscreenTextBoxDirPath.Text;
			string dirName = touchscreenTextBoxDirName.Text;
			
			if(path.Length != 0 && dirName.Length != 0)
			{
				try
				{
					//dirLocator.SetDirectory(dirName,path);
					AddFileToView(path);
				}
				catch(Exception ex)
				{
					MessageBox.Show("Error Adding New Directory: " + ex.Message);
				}				
			}
			else
			{
				MessageBox.Show("Path or Directory Name not present");
			}			
		}

		private void buttonDeleteDirectory_Click(object sender, System.EventArgs e)
		{
// 			string directoryName = touchscreenTextBoxDirName.Text;
// 			treeViewDirectoryList.SelectedNode.Remove();
		}

		private void buttonSaveDirectoryList_Click(object sender, System.EventArgs e)
		{
		}

		private void treeViewDirectoryList_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
// 			string path = e.Node.FullPath.ToString();
// 			textBoxSelectedDirectory.Text = path;
// 			touchscreenTextBoxDirPath.Text = path;
// 			touchscreenTextBoxDirName.Text = "";
// 			ICollection keys = dirLocator.DirectoryList.Keys;
// 
// 			foreach(string key in keys)
// 			{
// 				if(String.Compare((string)dirLocator.DirectoryList[key],path) == 0)
// 				{
// 					touchscreenTextBoxDirName.Text = key;
// 				}
// 			}
		}


		// Internal Methods  ----------------------------------------------------------
		private void FillTreeView()
		{
// 			foreach(string path in dirLocator.DirectoryList.Values)
// 			{
// 				AddFileToView(path);
// 			}
		}

		private void AddFileToView(string path)
		{
			bool nodeFound = false;
			//string fullDirName = Path.GetDirectoryName(path);
			//string fileName = Path.GetFileName(path);
			string[] directoryNames = new string[10];
			char[] separator = new char[1];
			TreeNode baseNode = new TreeNode(@"C:\");

			separator[0]= '\x5C';
			directoryNames = path.Split(separator,10);
			treeViewDirectoryList.SelectedNode = treeViewDirectoryList.Nodes[0];
			treeViewDirectoryList.BeginUpdate();

			for(int i=1; i<directoryNames.Length; i++)
			{
				
				if(directoryNames[i].Length != 0)
				{
					nodeFound = false;
					foreach(TreeNode tn in treeViewDirectoryList.SelectedNode.Nodes)
					{
						if(String.Compare(tn.Text,directoryNames[i]) == 0)
						{
							nodeFound = true;
							treeViewDirectoryList.SelectedNode = 
								treeViewDirectoryList.SelectedNode.Nodes[treeViewDirectoryList.SelectedNode.Nodes.IndexOf(tn)]; 
							break;
						}
					}
					if(!nodeFound)
					{
						TreeNode newNode = new TreeNode(directoryNames[i]);	
						treeViewDirectoryList.SelectedNode.Nodes.Add(newNode);	
						treeViewDirectoryList.SelectedNode = 
						treeViewDirectoryList.SelectedNode.Nodes[treeViewDirectoryList.SelectedNode.Nodes.IndexOf(newNode)]; 
					}
					
				}
			}
			treeViewDirectoryList.EndUpdate();
		}

		private int CheckCorrectNaming(string path, string dirName, string dirType)
		{
			char[] separator = new char[1];
			separator[0] = '\x5C';
			string[] parsedPath = new string[10];
			bool directoryCorrect = false;
			bool typeCorrect = false;

			parsedPath = path.Split(separator,10);
			if(String.Compare(parsedPath[0],"C:") == 0)
			{
				string pathDirectory = parsedPath[parsedPath.Length-2];
				string pathType = parsedPath[parsedPath.Length-1];

				if(String.Compare(pathDirectory,dirName) == 0 || String.Compare(dirType,"NotApplicable") == 0)
				{
					directoryCorrect = true;
				}			
				if(String.Compare(pathType,dirType) == 0 || String.Compare(dirType,"NotApplicable") == 0)
				{
					typeCorrect = true;
				}
			}
			else
			{
				return  (int)CheckNaming.Incorrect;
			}
			
			if(directoryCorrect && typeCorrect)
			{
				return  (int)CheckNaming.Correct;
			}
			else if(directoryCorrect && !typeCorrect)
			{
				return  (int)CheckNaming.TypeIncorrect;
			}
			else if(!directoryCorrect && typeCorrect)
			{
				return  (int)CheckNaming.DirectoryIncorrect;
			}
			else 
			{
				return  (int)CheckNaming.DirAndTypeIncorrect;
			}
		}

	
	}
}
