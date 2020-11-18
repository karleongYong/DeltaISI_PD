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
//  [6/6/2006]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using ZedGraph;

namespace Seagate.AAS.Utils.PointStore
{
	/// <summary>
	/// Summary description for UCTeachPointHistoryChart.
	/// </summary>
	public class TeachPointHistoryChart : System.Windows.Forms.UserControl
	{
        private ZedGraph.ZedGraphControl zedGraphControl1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TeachPointHistoryChart()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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

        private void zedGraphControl1_Load(object sender, System.EventArgs e)
        {
        }

        #region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedGraphControl1.IsAutoScrollRange = false;
            this.zedGraphControl1.IsEnableHPan = true;
            this.zedGraphControl1.IsEnableHZoom = true;
            this.zedGraphControl1.IsEnableVPan = true;
            this.zedGraphControl1.IsEnableVZoom = true;
            this.zedGraphControl1.IsPrintFillPage = true;
            this.zedGraphControl1.IsPrintKeepAspectRatio = true;
            this.zedGraphControl1.IsScrollY2 = false;
            this.zedGraphControl1.IsShowContextMenu = true;
            this.zedGraphControl1.IsShowCopyMessage = true;
            this.zedGraphControl1.IsShowCursorValues = false;
            this.zedGraphControl1.IsShowHScrollBar = false;
            this.zedGraphControl1.IsShowPointValues = false;
            this.zedGraphControl1.IsShowVScrollBar = false;
            this.zedGraphControl1.IsZoomOnMouseCenter = false;
            this.zedGraphControl1.Location = new System.Drawing.Point(0, 0);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.PanButtons = System.Windows.Forms.MouseButtons.Left;
            this.zedGraphControl1.PanButtons2 = System.Windows.Forms.MouseButtons.Middle;
            this.zedGraphControl1.PanModifierKeys2 = System.Windows.Forms.Keys.None;
            this.zedGraphControl1.PointDateFormat = "g";
            this.zedGraphControl1.PointValueFormat = "G";
            this.zedGraphControl1.ScrollMaxX = 0;
            this.zedGraphControl1.ScrollMaxY = 0;
            this.zedGraphControl1.ScrollMaxY2 = 0;
            this.zedGraphControl1.ScrollMinX = 0;
            this.zedGraphControl1.ScrollMinY = 0;
            this.zedGraphControl1.ScrollMinY2 = 0;
            this.zedGraphControl1.Size = new System.Drawing.Size(104, 72);
            this.zedGraphControl1.TabIndex = 0;
            this.zedGraphControl1.ZoomButtons = System.Windows.Forms.MouseButtons.Left;
            this.zedGraphControl1.ZoomButtons2 = System.Windows.Forms.MouseButtons.None;
            this.zedGraphControl1.ZoomModifierKeys = System.Windows.Forms.Keys.None;
            this.zedGraphControl1.ZoomModifierKeys2 = System.Windows.Forms.Keys.None;
            this.zedGraphControl1.ZoomStepFraction = 0.1;
            this.zedGraphControl1.Load += new System.EventHandler(this.zedGraphControl1_Load);
            // 
            // UCTeachPointHistoryChart
            // 
            this.Controls.Add(this.zedGraphControl1);
            this.Name = "UCTeachPointHistoryChart";
            this.Size = new System.Drawing.Size(104, 72);
            this.ResumeLayout(false);

        }
		#endregion

        public void Update(object subject)
        {
            if(subject is TeachPoint)
                AssignTeachPoint((TeachPoint)subject);
        }

        public void AssignTeachPoint(TeachPoint tPoint)
        {
            CreateChart(zedGraphControl1, tPoint);
        }

        // Call this method from the Form_Load method, passing your ZedGraphControl
        public void CreateChart(ZedGraphControl zgc, TeachPoint tPoint)
        {
            // clear previous values
            GraphPane myPane = zgc.GraphPane;
            myPane.CurveList.Clear();

            // Set the titles and axis labels
//             myPane.Title = "Teachpoint shift:" + tPoint.Name;
//             myPane.XAxis.Title = "Updates";
//             myPane.YAxis.Title = "Parameter A";
//             myPane.Y2Axis.Title = "Parameter B";
    
            // Make up some data points based on the Sine function
            //PointPairList[] list = new PointPairList[tPoint.Dimension];
            //PointPairList list = new PointPairList();
            LineItem myCurve = null;
            SymbolType simbol = SymbolType.Circle;
            System.Drawing.Color color = Color.Blue;
            for (int dim = 0; dim < tPoint.Dimension; dim++)
            {
                PointPairList list = new PointPairList();
                list.Clear();
                for (int idx = 0; idx < tPoint.History.History.Count; idx++)
                {
                    NPoint npoint = (NPoint)tPoint.History.History[idx];
                    double y = npoint.Coordinate[dim];
                    list.Add((double)idx, y);
                }

                myCurve = myPane.AddCurve(tPoint.DimensionNames[dim], list, color, simbol++);
                myCurve.Symbol.Fill = new Fill( Color.Gray );
            }

    
            // Associate this curve with the Y2 axis
            //myCurve.IsY2Axis = true;
    
//             // Show the x axis grid
//            myPane.XAxis.IsShowGrid = true;
//     
//             // Make the Y axis scale red
//             myPane.YAxis.ScaleFontSpec.FontColor = Color.Blue;
//             myPane.YAxis.TitleFontSpec.FontColor = Color.Blue;
//             // turn off the opposite tics so the Y tics don't show up on the Y2 axis
//             myPane.YAxis.IsOppositeTic = false;
//             myPane.YAxis.IsMinorOppositeTic = false;
//             // Don't display the Y zero line
//             myPane.YAxis.IsZeroLine = false;
//             // Align the Y axis labels so they are flush to the axis
//             myPane.YAxis.ScaleAlign = AlignP.Inside;
//             // Manually set the axis range
//             //myPane.YAxis.Min = -30;
//             //myPane.YAxis.Max = 30;
//     
            // Enable the Y2 axis display
            //myPane.Y2Axis.IsVisible = true;
            // Make the Y2 axis scale blue
            //myPane.Y2Axis.ScaleFontSpec.FontColor = Color.Blue;
            //myPane.Y2Axis.TitleFontSpec.FontColor = Color.Blue;
            // turn off the opposite tics so the Y2 tics don't show up on the Y axis
            //myPane.Y2Axis.IsOppositeTic = false;
            //myPane.Y2Axis.IsMinorOppositeTic = false;
            // Display the Y2 axis grid lines
            //myPane.Y2Axis.IsShowGrid = true;
            // Align the Y2 axis labels so they are flush to the axis
            //myPane.Y2Axis.ScaleAlign = AlignP.Inside;
    
            // Fill the axis background with a gradient
//            myPane.AxisFill = new Fill( Color.White, Color.LightGray, 45.0f );
    
            // Calculate the Axis Scale Ranges
            zgc.AxisChange();

            zgc.Refresh();
        }

    
	}
}
