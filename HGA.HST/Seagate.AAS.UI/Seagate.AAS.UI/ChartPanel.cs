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
//  [4/5/2006] by Sabrina Murray using ZedGraph  
//
//  http://zedgraph.org/wiki/index.php?title=Main_Page
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.IO;
using System.Text;
using System.Diagnostics;

using GDIDB;

namespace Seagate.AAS.UI
{
	/// <summary>
	/// Summary description for PanelChart.
	/// </summary>
	public class ChartPanel : System.Windows.Forms.UserControl
	{
		// Nested declarations -------------------------------------------------
        
		// Member variables ----------------------------------------------------
   		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonRescale;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelErrorMessage;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private DBGraphics memGraphics;
		//protected GraphPane myPane = new GraphPane();
        //private double xMaxScale = 0;
        //private double yMaxScale = 0;
        //private double xMinScale = 0;
        //private double yMinScale = 0;
        //private double y2MaxScale = 0;
		private System.Windows.Forms.Label labelY2Axis;
		private System.Windows.Forms.Label labelY2Max;
		private System.Windows.Forms.Label labelY2Min;
		private Seagate.AAS.UI.TouchscreenNumBox textBoxXMin;
		private Seagate.AAS.UI.TouchscreenNumBox textBoxXMax;
		private Seagate.AAS.UI.TouchscreenNumBox textBoxYMin;
		private Seagate.AAS.UI.TouchscreenNumBox textBoxYMax;
		private Seagate.AAS.UI.TouchscreenNumBox textBoxY2Min;
		private Seagate.AAS.UI.TouchscreenNumBox textBoxY2Max;
		//private double y2MinScale = 0;
		
		// Constructors & Finalizers -------------------------------------------
        public ChartPanel()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			memGraphics = new  DBGraphics();
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelY2Axis = new System.Windows.Forms.Label();
			this.labelY2Max = new System.Windows.Forms.Label();
			this.labelY2Min = new System.Windows.Forms.Label();
			this.labelErrorMessage = new System.Windows.Forms.Label();
			this.buttonRescale = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxXMin = new Seagate.AAS.UI.TouchscreenNumBox();
			this.textBoxXMax = new Seagate.AAS.UI.TouchscreenNumBox();
			this.textBoxYMin = new Seagate.AAS.UI.TouchscreenNumBox();
			this.textBoxYMax = new Seagate.AAS.UI.TouchscreenNumBox();
			this.textBoxY2Min = new Seagate.AAS.UI.TouchscreenNumBox();
			this.textBoxY2Max = new Seagate.AAS.UI.TouchscreenNumBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textBoxY2Max);
			this.groupBox1.Controls.Add(this.textBoxY2Min);
			this.groupBox1.Controls.Add(this.textBoxYMax);
			this.groupBox1.Controls.Add(this.textBoxYMin);
			this.groupBox1.Controls.Add(this.textBoxXMax);
			this.groupBox1.Controls.Add(this.textBoxXMin);
			this.groupBox1.Controls.Add(this.labelY2Axis);
			this.groupBox1.Controls.Add(this.labelY2Max);
			this.groupBox1.Controls.Add(this.labelY2Min);
			this.groupBox1.Controls.Add(this.labelErrorMessage);
			this.groupBox1.Controls.Add(this.buttonRescale);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
			this.groupBox1.Location = new System.Drawing.Point(640, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(160, 352);
			this.groupBox1.TabIndex = 22;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Scaling";
			// 
			// labelY2Axis
			// 
			this.labelY2Axis.Location = new System.Drawing.Point(16, 168);
			this.labelY2Axis.Name = "labelY2Axis";
			this.labelY2Axis.Size = new System.Drawing.Size(128, 16);
			this.labelY2Axis.TabIndex = 38;
			this.labelY2Axis.Text = "Y2- AXIS";
			this.labelY2Axis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelY2Axis.Visible = false;
			// 
			// labelY2Max
			// 
			this.labelY2Max.Location = new System.Drawing.Point(96, 184);
			this.labelY2Max.Name = "labelY2Max";
			this.labelY2Max.Size = new System.Drawing.Size(50, 24);
			this.labelY2Max.TabIndex = 37;
			this.labelY2Max.Text = "Max";
			this.labelY2Max.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelY2Max.Visible = false;
			// 
			// labelY2Min
			// 
			this.labelY2Min.Location = new System.Drawing.Point(16, 184);
			this.labelY2Min.Name = "labelY2Min";
			this.labelY2Min.Size = new System.Drawing.Size(50, 24);
			this.labelY2Min.TabIndex = 36;
			this.labelY2Min.Text = "Min";
			this.labelY2Min.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelY2Min.Visible = false;
			// 
			// labelErrorMessage
			// 
			this.labelErrorMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelErrorMessage.Location = new System.Drawing.Point(12, 288);
			this.labelErrorMessage.Name = "labelErrorMessage";
			this.labelErrorMessage.Size = new System.Drawing.Size(136, 56);
			this.labelErrorMessage.TabIndex = 33;
			this.labelErrorMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonRescale
			// 
			this.buttonRescale.Location = new System.Drawing.Point(16, 240);
			this.buttonRescale.Name = "buttonRescale";
			this.buttonRescale.Size = new System.Drawing.Size(128, 40);
			this.buttonRescale.TabIndex = 32;
			this.buttonRescale.Text = "Rescale Graph";
			this.buttonRescale.Click += new System.EventHandler(this.buttonRescale_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(128, 16);
			this.label4.TabIndex = 31;
			this.label4.Text = "Y- AXIS";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(96, 112);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(50, 24);
			this.label5.TabIndex = 30;
			this.label5.Text = "Max";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(16, 112);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(50, 24);
			this.label6.TabIndex = 29;
			this.label6.Text = "Min";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(128, 16);
			this.label3.TabIndex = 26;
			this.label3.Text = "X- AXIS";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(96, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(50, 24);
			this.label2.TabIndex = 25;
			this.label2.Text = "Max";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(50, 24);
			this.label1.TabIndex = 24;
			this.label1.Text = "Min";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textBoxXMin
			// 
			this.textBoxXMin.BackColor = System.Drawing.Color.Pink;
			this.textBoxXMin.IntegerOnly = false;
			this.textBoxXMin.Location = new System.Drawing.Point(16, 64);
			this.textBoxXMin.Max = 100;
			this.textBoxXMin.Min = -5;
			this.textBoxXMin.Name = "textBoxXMin";
			this.textBoxXMin.Size = new System.Drawing.Size(50, 26);
			this.textBoxXMin.TabIndex = 39;
			this.textBoxXMin.Text = "";
			// 
			// textBoxXMax
			// 
			this.textBoxXMax.BackColor = System.Drawing.Color.Pink;
			this.textBoxXMax.IntegerOnly = false;
			this.textBoxXMax.Location = new System.Drawing.Point(96, 64);
			this.textBoxXMax.Max = 100;
			this.textBoxXMax.Min = -5;
			this.textBoxXMax.Name = "textBoxXMax";
			this.textBoxXMax.Size = new System.Drawing.Size(50, 26);
			this.textBoxXMax.TabIndex = 40;
			this.textBoxXMax.Text = "";
			// 
			// textBoxYMin
			// 
			this.textBoxYMin.BackColor = System.Drawing.Color.Pink;
			this.textBoxYMin.IntegerOnly = false;
			this.textBoxYMin.Location = new System.Drawing.Point(16, 136);
			this.textBoxYMin.Max = 100;
			this.textBoxYMin.Min = -5;
			this.textBoxYMin.Name = "textBoxYMin";
			this.textBoxYMin.Size = new System.Drawing.Size(50, 26);
			this.textBoxYMin.TabIndex = 41;
			this.textBoxYMin.Text = "";
			// 
			// textBoxYMax
			// 
			this.textBoxYMax.BackColor = System.Drawing.Color.Pink;
			this.textBoxYMax.IntegerOnly = false;
			this.textBoxYMax.Location = new System.Drawing.Point(96, 136);
			this.textBoxYMax.Max = 100;
			this.textBoxYMax.Min = -5;
			this.textBoxYMax.Name = "textBoxYMax";
			this.textBoxYMax.Size = new System.Drawing.Size(50, 26);
			this.textBoxYMax.TabIndex = 42;
			this.textBoxYMax.Text = "";
			// 
			// textBoxY2Min
			// 
			this.textBoxY2Min.BackColor = System.Drawing.Color.Pink;
			this.textBoxY2Min.IntegerOnly = false;
			this.textBoxY2Min.Location = new System.Drawing.Point(16, 208);
			this.textBoxY2Min.Max = 100;
			this.textBoxY2Min.Min = -5;
			this.textBoxY2Min.Name = "textBoxY2Min";
			this.textBoxY2Min.Size = new System.Drawing.Size(50, 26);
			this.textBoxY2Min.TabIndex = 43;
			this.textBoxY2Min.Text = "";
			// 
			// textBoxY2Max
			// 
			this.textBoxY2Max.BackColor = System.Drawing.Color.Pink;
			this.textBoxY2Max.IntegerOnly = false;
			this.textBoxY2Max.Location = new System.Drawing.Point(96, 208);
			this.textBoxY2Max.Max = 100;
			this.textBoxY2Max.Min = -5;
			this.textBoxY2Max.Name = "textBoxY2Max";
			this.textBoxY2Max.Size = new System.Drawing.Size(50, 26);
			this.textBoxY2Max.TabIndex = 44;
			this.textBoxY2Max.Text = "";
			// 
			// PanelChart
			// 
			this.Controls.Add(this.groupBox1);
			this.Name = "PanelChart";
			this.Size = new System.Drawing.Size(800, 352);
			this.Resize += new System.EventHandler(this.PanelChart_Resize);
			this.Load += new System.EventHandler(this.PanelChart_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelChart_Paint);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		// Properties ----------------------------------------------------------
		// Public Methods -------------------------------------------------------------
        public void DrawGraph1DataSet( double[] xData, double[] yData, string xLabel, string yLabel, string title, bool showDataPoints)
		{
			try
			{
// 				memGraphics.CreateDoubleBuffer(this.CreateGraphics(),
// 					this.ClientRectangle.Width, this.ClientRectangle.Height);
// 
// 				//Hide Additional Scaling Labels
// 				groupBox1.Visible = true;
// 				labelY2Min.Visible = false;
// 				labelY2Max.Visible = false;
// 				labelY2Axis.Visible = false;
// 				textBoxY2Min.Visible = false;
// 				textBoxY2Max.Visible = false;
// 
// 				//Setup Graph-- Labels, Size, Scale
// 				int width = this.Width - groupBox1.Width;
// 				int height = this.Height;
// 				myPane.CurveList.Clear();
// 				myPane.PaneRect = new Rectangle(1,1,width,height);//10,10,10,10);
// 				myPane.Title = title;
// 				myPane.XAxis.Title = xLabel;
// 				myPane.YAxis.Title = yLabel;
// 				
// 				myPane.YAxis.MaxAuto= true; 
// 				myPane.YAxis.MinAuto = true;
// 				myPane.XAxis.MaxAuto= true; 
// 				myPane.XAxis.MinAuto = true;
// 				
// 				//Plot the data
// 				LineItem curve;
// 				curve = myPane.AddCurve( yLabel, xData, yData, Color.Blue, SymbolType.None );
// 				curve.Line.Width = 1.5F;
// 				curve.Line.IsSmooth = true;
// 				curve.Symbol.Fill = new Fill( Color.Fuchsia);
// 				curve.Symbol.Size = 20;
// 				if( showDataPoints)
// 					curve.Symbol.IsVisible = true;
// 				else curve.Symbol.IsVisible = false;
// 
// 				myPane.PaneFill = new Fill( Color.FromArgb(102, 153, 153));
// 				myPane.AxisFill = new Fill( Color.White);
// 			
// 				myPane.XAxis.IsShowGrid = true;
// 				myPane.YAxis.IsShowGrid = true;
// 				
// 				myPane.AxisChange( this.CreateGraphics() );
			}
			catch( Exception ex )
			{
				string errMsg = string.Format("An error occurred and the graph can not be drawn. Error: {0}",ex.Message);
				labelErrorMessage.Text = errMsg;
			}
		}
		public void DrawGraph1DataSet( double[] xData, double[] yData, string xLabel, string yLabel,string title, double xMaxScale, double yMaxScale,double xMinScale, double yMinScale, bool showDataPoints)
		{
			try
			{
// 				memGraphics.CreateDoubleBuffer(this.CreateGraphics(),
// 					this.ClientRectangle.Width, this.ClientRectangle.Height);
// 
// 				//Hide Additional Scaling Labels
// 				groupBox1.Visible = true;
// 				labelY2Min.Visible = false;
// 				labelY2Max.Visible = false;
// 				labelY2Axis.Visible = false;
// 				textBoxY2Min.Visible = false;
// 				textBoxY2Max.Visible = false;
// 
// 				//Setup Graph-- Labels, Size, Scale
// 				int width = this.Width - groupBox1.Width;
// 				int height = this.Height;
// 				myPane.CurveList.Clear();
// 				myPane.PaneRect = new Rectangle(1,1,width,height);//10,10,10,10);
// 				myPane.Title = title;
// 				myPane.XAxis.Title = xLabel;
// 				myPane.YAxis.Title = yLabel;
// 				
// 				myPane.YAxis.Max = yMaxScale;
// 				myPane.YAxis.Min = yMinScale;
// 				myPane.XAxis.Max = xMaxScale;
// 				myPane.XAxis.Min = xMinScale;
// 				
// 				//Plot the data
// 				LineItem curve;
// 				curve = myPane.AddCurve( yLabel, xData, yData, Color.Blue, SymbolType.None );
// 				curve.Line.Width = 1.5F;
// 				curve.Line.IsSmooth = true;
// 				//curve.Line.SmoothTension = 0.6F;
// 				curve.Symbol.Fill = new Fill( Color.Blue );
// 				if( showDataPoints)
// 					curve.Symbol.IsVisible = true;
// 				else curve.Symbol.IsVisible = false;
// 
// 				myPane.PaneFill = new Fill( Color.FromArgb(102, 153, 153));
// 				myPane.AxisFill = new Fill( Color.White);
// 			
// 				myPane.XAxis.IsShowGrid = true;
// 				myPane.YAxis.IsShowGrid = true;
// 				
// 				myPane.AxisChange( this.CreateGraphics() );
			}
			catch( Exception ex )
			{
				string errMsg = string.Format("An error occurred and the graph can not be drawn. Error: {0}",ex.Message);
				labelErrorMessage.Text = errMsg;
			}
		}
		
		public void DrawGraph2DataSet( double[] xData, double[] yData, string xLabel, string yLabel,double[] y2Data, string y2Label, string title, bool showDataPoints)
		{
			try
			{
// 				memGraphics.CreateDoubleBuffer(this.CreateGraphics(),
// 					this.ClientRectangle.Width, this.ClientRectangle.Height);
// 
// 				//Display Additional Scaling Labels
// 				groupBox1.Visible = true;
// 				labelY2Min.Visible = true;
// 				labelY2Max.Visible = true;
// 				labelY2Axis.Visible = true;
// 				textBoxY2Min.Visible = true;
// 				textBoxY2Max.Visible = true;
// 
// 				//Setup Graph-- Labels, Size, Scale
// 				int width = this.Width - groupBox1.Width;
// 				int height = this.Height;
// 				myPane.CurveList.Clear();
// 				myPane.PaneRect = new Rectangle(1,1,width,height);//10,10,10,10);
// 				myPane.Title = title;
// 				myPane.XAxis.Title = xLabel;
// 				myPane.YAxis.Title = yLabel;
// 				myPane.Y2Axis.Title = y2Label;
// 				
// 				myPane.YAxis.MaxAuto= true; 
// 				myPane.YAxis.MinAuto = true;
// 				myPane.XAxis.MaxAuto= true; 
// 				myPane.XAxis.MinAuto = true;				
// 				myPane.Y2Axis.MaxAuto= true; 
// 				myPane.Y2Axis.Min = 0.0;
// 				
// 				//Plot the data
// 				LineItem curve;
// 				curve = myPane.AddCurve( yLabel, xData, yData, Color.Blue, SymbolType.None );
// 				curve.Line.Width = 1.5F;
// 				curve.Line.IsSmooth = true;
// 				//curve.Line.SmoothTension = 0.6F;
// 				curve.Symbol.Fill = new Fill( Color.Blue );
// 				if( showDataPoints)
// 					curve.Symbol.IsVisible = true;
// 				else curve.Symbol.IsVisible = false;
// 
// 				LineItem curve2;
// 				curve2 = myPane.AddCurve( y2Label, xData, y2Data, Color.Fuchsia, SymbolType.None );
// 				curve2.Line.Width = 1.5F;
// 				curve2.Line.IsSmooth = true;
// 				//curve.Line.SmoothTension = 0.6F;
// 				curve2.Symbol.Fill = new Fill( Color.Fuchsia );
// 				curve2.IsY2Axis = true;
// 				if( showDataPoints)
// 					curve2.Symbol.IsVisible = true;
// 				else curve2.Symbol.IsVisible = false;
// 				
// 				myPane.PaneFill = new Fill( Color.FromArgb(102, 153, 153));
// 				myPane.AxisFill = new Fill( Color.White);
// 			
// 				myPane.XAxis.IsShowGrid = true;
// 				myPane.YAxis.IsShowGrid = true;
// 				myPane.Y2Axis.IsScaleVisible = true;
// 				myPane.Y2Axis.IsVisible = true;
// 				
// 				myPane.AxisChange( this.CreateGraphics() );
// 				this.Refresh();
			}
			catch( Exception ex )
			{
				string errMsg = string.Format("An error occurred and the graph can not be drawn. Error: {0}",ex.Message);
				labelErrorMessage.Text = errMsg;
			}
		}
			
		public void DrawPareto( string[] xStr, double[] yData, string xLabel, string yLabel, string title)
		{
			try
			{
// 				memGraphics.CreateDoubleBuffer(this.CreateGraphics(),
// 					this.ClientRectangle.Width, this.ClientRectangle.Height);
// 
// 				//Hide Additional Scaling Labels
// 				groupBox1.Visible = false;
// 
// 				//Setup Graph-- Labels, Size, Scale
// 				int width = this.Width;
// 				int height = this.Height;
// 				
// 				myPane.CurveList.Clear();
// 				myPane.PaneRect = new Rectangle(1,1,width,height);//10,10,10,10);
// 				myPane.Title = title;
// 				myPane.XAxis.Title = xLabel;
// 				myPane.YAxis.Title = yLabel;
// 				
// 				myPane.YAxis.MaxAuto = true;
// 				myPane.YAxis.MinAuto = true;
// 				
// 				//Plot the data
// 				BarItem bar = myPane.AddBar( "",null,yData, Color.SteelBlue);
// 				bar.Bar.Fill = new Fill( Color.RosyBrown, Color.White, Color.RosyBrown );
// 				//myPane.ClusterScaleWidth = 100;
// 				//myPane.BarType = BarType.Stack;
// 
// 				myPane.XAxis.IsTicsBetweenLabels = true;
// 				myPane.XAxis.TextLabels = xStr;
// 				myPane.XAxis.Type = AxisType.Text;
// 				myPane.Legend.IsVisible = false;
// 
// 				myPane.PaneFill = new Fill( Color.FromArgb(102, 153, 153));
// 				myPane.AxisFill = new Fill( Color.White);
// 			
// 				myPane.YAxis.IsShowGrid = true;
// 				
// 				myPane.AxisChange( this.CreateGraphics() );
			}
			catch( Exception ex )
			{
				string errMsg = string.Format("An error occurred and the graph can not be drawn. Error: {0}",ex.Message);
				labelErrorMessage.Text = errMsg;
			}
		}
		
		
		// Private Methods -------------------------------------------------------------
		private void buttonRescale_Click(object sender, System.EventArgs e)
		{
			try
			{
// 				memGraphics.CreateDoubleBuffer(this.CreateGraphics(),
// 					this.ClientRectangle.Width, this.ClientRectangle.Height);
// 
// 				if( textBoxXMin.Text!=  "")
// 				{
// 					xMinScale = Convert.ToDouble(textBoxXMin.Text);
// 					myPane.XAxis.Min = xMinScale;
// 				}
// 				else
// 					myPane.XAxis.MinAuto=true;
// 				
// 				if( textBoxXMax.Text!=  "")
// 				{
// 					xMaxScale = Convert.ToDouble(textBoxXMax.Text);
// 					myPane.XAxis.Max = xMaxScale;
// 				}
// 				else
// 					myPane.XAxis.MaxAuto = true;
// 				
// 				if( textBoxYMin.Text!=  "")
// 				{
// 					yMinScale = Convert.ToDouble(textBoxYMin.Text);
// 					myPane.YAxis.Min = yMinScale;
// 				}
// 				else
// 					myPane.YAxis.MinAuto = true;
// 				
// 				if( textBoxYMax.Text!=  "")
// 				{
// 					yMaxScale = Convert.ToDouble(textBoxYMax.Text);
// 					myPane.YAxis.Max = yMaxScale;
// 				}
// 				else
// 					myPane.YAxis.MaxAuto = true;
// 
// 				
// 				if( labelY2Min.Visible == true )
// 				{
// 					if( textBoxY2Min.Text!=  "")
// 					{
// 						y2MinScale = Convert.ToDouble(textBoxY2Min.Text);
// 						myPane.Y2Axis.Min = y2MinScale;
// 					}
// 					else
// 						myPane.Y2Axis.MinAuto = true;
// 
// 					if( textBoxY2Max.Text!=  "")
// 					{
// 						y2MaxScale = Convert.ToDouble(textBoxY2Max.Text);
// 						myPane.Y2Axis.Max = y2MaxScale;	
// 					}
// 					else
// 						myPane.Y2Axis.MaxAuto = true;
// 				}
// 			
// 				myPane.AxisChange( this.CreateGraphics() );
// 				this.Refresh();
// 				//this.PanelChart_Paint();
			}
			catch( Exception ex )
			{
				string errMsg = string.Format("An error occurred and the graph can not be rescaled. Most likely a text box is incorrectly formatted or missing text. Error: {0}",ex.Message);
				labelErrorMessage.Text = errMsg;
			}
			
		}

		private void PanelChart_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			SolidBrush brush = new SolidBrush( Color.Gray );
			if ( memGraphics.CanDoubleBuffer() )
			{
				// Fill in Background (for effieciency only the area that has been clipped)
				memGraphics.g.FillRectangle( new SolidBrush(SystemColors.Window),
					e.ClipRectangle.X, e.ClipRectangle.Y,
					e.ClipRectangle.Width, e.ClipRectangle.Height);

				// Do our drawing using memGraphics.g instead e.Graphics
		     
				memGraphics.g.FillRectangle( brush, this.ClientRectangle );
				
// 				if ( myPane != null )
// 					myPane.Draw( memGraphics.g );
		   
				// Render to the form
				memGraphics.Render( e.Graphics );
			}
			else	// if double buffer is not available, do without it
			{
				e.Graphics.FillRectangle( brush, this.ClientRectangle );
// 				if ( myPane != null )
// 					myPane.Draw( e.Graphics );
			}
		}

		private void PanelChart_Load(object sender, System.EventArgs e)
		{
		
		}

		private void PanelChart_Resize(object sender, System.EventArgs e)
		{
			memGraphics.CreateDoubleBuffer(this.CreateGraphics(),
				this.ClientRectangle.Width, this.ClientRectangle.Height);
			Invalidate();
		}
	}
}
