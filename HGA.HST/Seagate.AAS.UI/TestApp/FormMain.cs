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
//  [9/6/2005]
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Seagate.AAS.UI
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class FormMain : System.Windows.Forms.Form
	{
        private Seagate.AAS.UI.Led led1;
        private Seagate.AAS.UI.FormattedNumber formattedNumber1;
        private Seagate.AAS.UI.FormattedNumber formattedNumber2;
        private Seagate.AAS.UI.TouchscreenNumBox touchscreenNumBox1;
        private Seagate.AAS.UI.TouchscreenTextBox touchscreenTextBox1;

        private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;

        bool _layoutCalled = false;
        private Seagate.AAS.UI.SimpleChart simpleChart1;
        private System.Windows.Forms.Button buttonTestChart;
        private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Button buttonDrawPareto;
		private System.Windows.Forms.Button buttonDrawLineChart;
		private System.Windows.Forms.Button buttonDraw2DataLineChart;
        private ChartPanel chart = new ChartPanel();
        private ChartPanel chart1 = new ChartPanel();
        private ChartPanel chart2 = new ChartPanel();
		string[] Xstr = {"5727","2101","2526","2390","2728","2925","2326","2450"};
		double[] X = {5727,2101,2526,2390,2728,2925,2326,2450};
		double[] Y = {27,01,26,90,28,25,26,50};
		double[] X1 = new double[100];
		double[] Y1 = new double[100];
		double[] Y2 = new double[100];
        private Seagate.AAS.UI.RadarIndicator radarIndicator1;
        private Seagate.AAS.UI.SortListView sortableListView1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private TouchscreenUpDown touchscreenUpDown1;
        private IContainer components;

		public FormMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			this.Controls.Add(chart);
			chart.Location = new System.Drawing.Point(352, 384);
			chart.Size = new System.Drawing.Size(552, 320);
			chart.Visible = false;
			this.Controls.Add(chart1);
			chart1.Location = new System.Drawing.Point(352, 384);
			chart1.Size = new System.Drawing.Size(552, 320);
			chart1.Visible = false;

			this.Controls.Add(chart2);
			chart2.Location = new System.Drawing.Point(352, 384);
			chart2.Size = new System.Drawing.Size(552, 320);
			chart2.Visible = false;
			
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            radarIndicator1.OuterTolerance = radarIndicator1.Width/2;
            radarIndicator1.InnerTolerance = radarIndicator1.Width/4;

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.led1 = new Seagate.AAS.UI.Led();
            this.button1 = new System.Windows.Forms.Button();
            this.formattedNumber1 = new Seagate.AAS.UI.FormattedNumber();
            this.formattedNumber2 = new Seagate.AAS.UI.FormattedNumber();
            this.touchscreenNumBox1 = new Seagate.AAS.UI.TouchscreenNumBox();
            this.touchscreenTextBox1 = new Seagate.AAS.UI.TouchscreenTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.simpleChart1 = new Seagate.AAS.UI.SimpleChart();
            this.buttonTestChart = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.buttonDrawPareto = new System.Windows.Forms.Button();
            this.buttonDrawLineChart = new System.Windows.Forms.Button();
            this.buttonDraw2DataLineChart = new System.Windows.Forms.Button();
            this.radarIndicator1 = new Seagate.AAS.UI.RadarIndicator();
            this.sortableListView1 = new Seagate.AAS.UI.SortListView(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.touchscreenUpDown1 = new Seagate.AAS.UI.TouchscreenUpDown();
            this.SuspendLayout();
            // 
            // led1
            // 
            this.led1.DisplayAsButton = true;
            this.led1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.led1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.led1.LedColor = Seagate.AAS.UI.LedColorList.Red;
            this.led1.Location = new System.Drawing.Point(32, 16);
            this.led1.Name = "led1";
            this.led1.Size = new System.Drawing.Size(160, 56);
            this.led1.State = true;
            this.led1.TabIndex = 0;
            this.led1.Text = "Left Escapement Open";
            this.led1.Click += new System.EventHandler(this.led1_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(208, 24);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 40);
            this.button1.TabIndex = 1;
            this.button1.Text = "Toggle State";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // formattedNumber1
            // 
            this.formattedNumber1.Location = new System.Drawing.Point(120, 96);
            this.formattedNumber1.Name = "formattedNumber1";
            this.formattedNumber1.Number = 0;
            this.formattedNumber1.NumberFormat = "0.000";
            this.formattedNumber1.Size = new System.Drawing.Size(100, 23);
            this.formattedNumber1.TabIndex = 1;
            this.formattedNumber1.Text = "0.000";
            this.formattedNumber1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // formattedNumber2
            // 
            this.formattedNumber2.Location = new System.Drawing.Point(256, 96);
            this.formattedNumber2.Name = "formattedNumber2";
            this.formattedNumber2.Number = 0;
            this.formattedNumber2.NumberFormat = "0.000";
            this.formattedNumber2.Size = new System.Drawing.Size(100, 23);
            this.formattedNumber2.TabIndex = 0;
            this.formattedNumber2.Text = "0.000";
            this.formattedNumber2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // touchscreenNumBox1
            // 
            this.touchscreenNumBox1.BackColor = System.Drawing.Color.White;
            this.touchscreenNumBox1.IntegerOnly = false;
            this.touchscreenNumBox1.Location = new System.Drawing.Point(32, 152);
            this.touchscreenNumBox1.Max = 100;
            this.touchscreenNumBox1.Min = -5;
            this.touchscreenNumBox1.Name = "touchscreenNumBox1";
            this.touchscreenNumBox1.Size = new System.Drawing.Size(100, 20);
            this.touchscreenNumBox1.TabIndex = 2;
            this.touchscreenNumBox1.Text = "10";
            // 
            // touchscreenTextBox1
            // 
            this.touchscreenTextBox1.AlphaNumOnly = false;
            this.touchscreenTextBox1.BackColor = System.Drawing.Color.White;
            this.touchscreenTextBox1.FormTitle = "Enter Text";
            this.touchscreenTextBox1.Location = new System.Drawing.Point(32, 248);
            this.touchscreenTextBox1.MinLength = 0;
            this.touchscreenTextBox1.Name = "touchscreenTextBox1";
            this.touchscreenTextBox1.NoWhiteSpace = false;
            this.touchscreenTextBox1.OnlyCaps = false;
            this.touchscreenTextBox1.Size = new System.Drawing.Size(100, 20);
            this.touchscreenTextBox1.TabIndex = 3;
            this.touchscreenTextBox1.Text = "Enter Text";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(32, 224);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 23);
            this.label1.TabIndex = 4;
            this.label1.Text = "Text entry box for touchscreen";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(32, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(184, 23);
            this.label2.TabIndex = 5;
            this.label2.Text = "Number entry box for touchscreen";
            // 
            // simpleChart1
            // 
            this.simpleChart1.BoundaryLine.Color = System.Drawing.Color.Gray;
            this.simpleChart1.BoundaryLine.Style = System.Drawing.Drawing2D.DashStyle.Solid;
            this.simpleChart1.BoundaryLine.Width = 1;
            this.simpleChart1.ChartLine.Color = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.simpleChart1.ChartLine.Style = System.Drawing.Drawing2D.DashStyle.Solid;
            this.simpleChart1.ChartLine.Width = 1;
            this.simpleChart1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.simpleChart1.Location = new System.Drawing.Point(435, 11);
            this.simpleChart1.MeanLine.Color = System.Drawing.Color.Black;
            this.simpleChart1.MeanLine.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            this.simpleChart1.MeanLine.Width = 1;
            this.simpleChart1.Name = "simpleChart1";
            this.simpleChart1.Size = new System.Drawing.Size(232, 117);
            this.simpleChart1.Symbol.Color = System.Drawing.Color.Blue;
            this.simpleChart1.Symbol.Size = 10;
            this.simpleChart1.Symbol.Style = Seagate.AAS.UI.SymbolShape.Circle;
            this.simpleChart1.TabIndex = 6;
            this.simpleChart1.Text = "Test Chart Title Test";
            // 
            // buttonTestChart
            // 
            this.buttonTestChart.Location = new System.Drawing.Point(340, 16);
            this.buttonTestChart.Name = "buttonTestChart";
            this.buttonTestChart.Size = new System.Drawing.Size(80, 41);
            this.buttonTestChart.TabIndex = 7;
            this.buttonTestChart.Text = "Test Chart";
            this.buttonTestChart.Click += new System.EventHandler(this.buttonTestChart_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.Location = new System.Drawing.Point(25, 186);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(136, 24);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "Display as Button";
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // buttonDrawPareto
            // 
            this.buttonDrawPareto.Location = new System.Drawing.Point(224, 288);
            this.buttonDrawPareto.Name = "buttonDrawPareto";
            this.buttonDrawPareto.Size = new System.Drawing.Size(95, 33);
            this.buttonDrawPareto.TabIndex = 11;
            this.buttonDrawPareto.Text = "Draw Pareto";
            this.buttonDrawPareto.Click += new System.EventHandler(this.buttonDrawPareto_Click);
            // 
            // buttonDrawLineChart
            // 
            this.buttonDrawLineChart.Location = new System.Drawing.Point(352, 288);
            this.buttonDrawLineChart.Name = "buttonDrawLineChart";
            this.buttonDrawLineChart.Size = new System.Drawing.Size(95, 33);
            this.buttonDrawLineChart.TabIndex = 12;
            this.buttonDrawLineChart.Text = "Draw Single Data Set Line Chart";
            this.buttonDrawLineChart.Click += new System.EventHandler(this.buttonDrawLineChart_Click);
            // 
            // buttonDraw2DataLineChart
            // 
            this.buttonDraw2DataLineChart.Location = new System.Drawing.Point(480, 288);
            this.buttonDraw2DataLineChart.Name = "buttonDraw2DataLineChart";
            this.buttonDraw2DataLineChart.Size = new System.Drawing.Size(95, 33);
            this.buttonDraw2DataLineChart.TabIndex = 13;
            this.buttonDraw2DataLineChart.Text = "Draw Double Data Set Line Chart";
            this.buttonDraw2DataLineChart.Click += new System.EventHandler(this.buttonDraw2DataLineChart_Click);
            // 
            // radarIndicator1
            // 
            this.radarIndicator1.InnerTolerance = 50F;
            this.radarIndicator1.Location = new System.Drawing.Point(680, 16);
            this.radarIndicator1.Name = "radarIndicator1";
            this.radarIndicator1.OuterTolerance = 100F;
            this.radarIndicator1.Size = new System.Drawing.Size(160, 112);
            this.radarIndicator1.TabIndex = 14;
            this.radarIndicator1.Text = "radarIndicator1";
            this.radarIndicator1.ValueX = 25F;
            this.radarIndicator1.ValueY = 25F;
            this.radarIndicator1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.radarIndicator1_MouseMove);
            // 
            // sortableListView1
            // 
            this.sortableListView1.ColumnNames = null;
            this.sortableListView1.FullRowSelect = true;
            this.sortableListView1.Location = new System.Drawing.Point(344, 144);
            this.sortableListView1.MaxRows = 100;
            this.sortableListView1.MultiSelect = false;
            this.sortableListView1.Name = "sortableListView1";
            this.sortableListView1.Size = new System.Drawing.Size(464, 128);
            this.sortableListView1.TabIndex = 15;
            this.sortableListView1.UseCompatibleStateImageBehavior = false;
            this.sortableListView1.View = System.Windows.Forms.View.Details;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(232, 152);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(88, 32);
            this.button2.TabIndex = 16;
            this.button2.Text = "Create Columns";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(232, 200);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(88, 32);
            this.button3.TabIndex = 17;
            this.button3.Text = "Add Data";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // touchscreenUpDown1
            // 
            this.touchscreenUpDown1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.touchscreenUpDown1.InvertDirection = false;
            this.touchscreenUpDown1.LeftRight = false;
            this.touchscreenUpDown1.Location = new System.Drawing.Point(92, 301);
            this.touchscreenUpDown1.Max = 10;
            this.touchscreenUpDown1.Min = -10;
            this.touchscreenUpDown1.Name = "touchscreenUpDown1";
            this.touchscreenUpDown1.Size = new System.Drawing.Size(100, 75);
            this.touchscreenUpDown1.TabIndex = 18;
            this.touchscreenUpDown1.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(856, 485);
            this.Controls.Add(this.touchscreenUpDown1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.sortableListView1);
            this.Controls.Add(this.radarIndicator1);
            this.Controls.Add(this.buttonDraw2DataLineChart);
            this.Controls.Add(this.buttonDrawLineChart);
            this.Controls.Add(this.buttonDrawPareto);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.buttonTestChart);
            this.Controls.Add(this.simpleChart1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.touchscreenTextBox1);
            this.Controls.Add(this.touchscreenNumBox1);
            this.Controls.Add(this.formattedNumber2);
            this.Controls.Add(this.formattedNumber1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.led1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.Form1_Layout);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
            SplashScreen.ShowSplashScreen(); 
            Application.DoEvents();

            SplashScreen.SetStatus("Starting Application");
            System.Threading.Thread.Sleep(500);           

            for (int testElement=1; testElement<=10; testElement++)
            {
                System.Threading.Thread.Sleep(500);
                SplashScreen.SetStatus("Starting Element " + testElement.ToString() + "\n");
                SplashScreen.SetStatus("Starting Element " + testElement.ToString() + "\n", SplashScreen.LogMsgType.Error);
                SplashScreen.SetStatus("Starting Element " + testElement.ToString() + "\n", SplashScreen.LogMsgType.Warning);
            }

			Application.Run(new FormMain());
		}

        private void button1_Click(object sender, System.EventArgs e)
        {
            led1.State = !led1.State;        
        }

        private void Form1_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            if( _layoutCalled == false )
            {
                _layoutCalled = true;
                //if( SplashScreen.SplashForm != null )
                //    SplashScreen.SplashForm.Owner = this;
                this.Activate();
                SplashScreen.CloseForm();
            }
        }

        private void buttonTestChart_Click(object sender, System.EventArgs e)
        {

            double [] chartValues = new double[10];
            Random random = new Random();
            for (int i=0; i<10; i++)
            {
                chartValues[i] = 10+5*random.NextDouble();
            }

            simpleChart1.YValues = chartValues;
        }

		private void led1_Click(object sender, System.EventArgs e)
		{
		
		}

		private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
		{
			led1.DisplayAsButton = checkBox1.Checked;
		}

		private void buttonDrawPareto_Click(object sender, System.EventArgs e)
		{
			chart.Visible = true;
			chart1.Visible = false;
			chart2.Visible = false;
			chart.Show();
			chart.DrawPareto(Xstr,Y,"Fail Codes","Number of failures","Failcode Pareto");
		}

		private void buttonDrawLineChart_Click(object sender, System.EventArgs e)
		{
			chart.Visible = false;
			chart1.Visible = true;
			chart2.Visible = false;
			chart1.Show();
			chart1.DrawGraph1DataSet(X1,Y1,"Time (s)","Flow","Pressure/Flow Response Plots for Test Cell 1",true);		
		
		}

		private void buttonDraw2DataLineChart_Click(object sender, System.EventArgs e)
		{
			chart.Visible = false;
			chart1.Visible = false;
			chart2.Visible = true;
			chart2.Show();

			Random random = new Random();
			for (int i=0; i<100; i++)
			{
				X1[i] = 0.01 * i;
				Y1[i] = 10+5*random.NextDouble();
				Y2[i] = 10+5*random.NextDouble();
			}

			chart2.DrawGraph2DataSet(X1,Y1,"Time (s)","Flow",Y2,"Pressure","Pressure/Flow Response Plots for Test Cell 1",false);		
		}

        private void radarIndicator1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            radarIndicator1.ValueX = e.X -radarIndicator1.Width/2;
            radarIndicator1.ValueY = radarIndicator1.Width/2 - e.Y;
        
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            sortableListView1.ColumnNames = new string[5] {"col1", "col2", "col3", "col4", "col5"};
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            sortableListView1.AddRow(new string[5] {"val1", "val2", "val3", "val4", "val5"});
        
        }

	}
}
