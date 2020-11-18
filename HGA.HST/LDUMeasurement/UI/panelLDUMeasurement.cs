using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Windows.Forms.DataVisualization.Charting;
using XyratexOSC.Logging;
namespace LDUMeasurement.UI
{
    public partial class panelLDUMeasurement : UserControl
    {

        public event EventLDUHandler EventLDU;

        public double[,] LDUVoltagePoint = new double[10, 21];
        public double[,] LEDVoltagePoint = new double[10, 21];
        
        public double[] LDU_M = new double[10];
        public double[] LDU_C = new double[10];

        public double[] LED_M = new double[10];
        public double[] LED_C = new double[10];

        public double[] ISweep_M_Trend1 = new double[10];
        public double[] ISweep_C_Trend1 = new double[10];

        public double[] ISweep_M_Trend2 = new double[10];
        public double[] ISweep_C_Trend2 = new double[10];

        public double[,] LDUPhotodiodeVoltagePoint = new double[10, 21];

        public double[] LDUMaxVPD = new double[10];

        public double[] LDU_IThreshold = new double[10];

        private int timercount;
        private double i_threshold_error;
        public bool LDUEnable
        { set;
          get;
        }

        public int LEDStartCurrent
        {
            get;
            set;
        }

        public int LEDStopCurrent
        {
            get;
            set;
        }

        public int LEDStepSize
        {
            get;
            set;
        }

        public bool LDUSweepMode
        {
            get;
            set;
        }

        public int LDUStartCurrent
        {
            get;
            set;
        }

        public int LDUStopCurrent
        {
            get;
            set;
        }

        public int LDUStepSize
        {
            get;
            set;
        }

        public int T1StopPoint
        {
            get;
            set;
        }       
        public int T2StartPoint
        {
            get;
            set;
        }
        public int LDUISweep1
        {
            get;
            set;
        }

        public int LDUISweep2
        {
            get;
            set;
        }

        public int LDUISweep3
        {
            get;
            set;
        }

        public int LDUISweep4
        {
            get;
            set;
        }

        public int LDUTimeInterval
        {
            get;
            set;
        }

        public bool Point_4Mode
        {
            get;
            set;
        }

        public double[,] SetUpdateLDUVoltage
        {
            get { return LDUPhotodiodeVoltagePoint; }
            set { LDUPhotodiodeVoltagePoint = value; }
        }

        public panelLDUMeasurement()
        {
            InitializeComponent();
            for (int hga = 0; hga< 10; hga++)
            {
                LDU_M[hga] = 0;
                LDU_C[hga] = 0;
                LED_M[hga] = 0;
                LED_C[hga] = 0;
                ISweep_M_Trend1[hga] = 0;
                ISweep_C_Trend1[hga] = 0;
                ISweep_M_Trend2[hga] = 0;
                ISweep_C_Trend2[hga] = 0;
                LDU_IThreshold[hga] = 0;
                LDUMaxVPD[hga] = 0;
                for (int point = 0; point < 21; point++)
                {
                    LDUVoltagePoint[hga, point] = 0;
                    LEDVoltagePoint[hga, point] = 0;
                    LDUPhotodiodeVoltagePoint[hga, point] = 0;
                }
            }

            //I want to initialize all the variable to the default setting set in GUI

            LDUEnable = checkBoxEnableLDU.Checked;
            LEDStartCurrent = int.Parse(textBoxLEDStartCurrent.Text);
            LEDStopCurrent = int.Parse(TextboxLEDStopCurrent.Text);
            LEDStepSize = int.Parse(textBoxLEDStepCurrent.Text);
            LDUSweepMode = checkBoxSweepMode.Checked;

            LDUStartCurrent = int.Parse(TextboxLDUStartCurrent.Text);
            LDUStopCurrent = int.Parse(TextboxLDUStopCurrent.Text);
            LDUStepSize = int.Parse(TextboxLDUStepCurrent.Text);
            T1StopPoint = int.Parse(textBoxT1StopPoint.Text);
            T2StartPoint = int.Parse(textBoxT2StartPoint.Text);

            Point_4Mode = checkBox4PtMode.Checked;

            LDUISweep1 = int.Parse(TextboxISweep1.Text);
            LDUISweep2 = int.Parse(TextboxISweep2.Text);
            LDUISweep3 = int.Parse(TextboxISweep3.Text);
            LDUISweep4 = int.Parse(TextboxISweep4.Text);
            LDUTimeInterval = int.Parse(TextboxTimeDuration.Text);
			
			i_threshold_error = 0.02;
			
            checkBoxEnableLDU.Enabled = false;

            textBoxLEDStartCurrent.Enabled = false;
            TextboxLEDStopCurrent.Enabled = false;
            textBoxLEDStepCurrent.Enabled = false;

            checkBoxSweepMode.Enabled = false;
            TextboxLDUStartCurrent.Enabled = false;
            TextboxLDUStopCurrent.Enabled = false;
            TextboxLDUStepCurrent.Enabled = false;

            textBoxT1StopPoint.Enabled = false;
            textBoxT2StartPoint.Enabled = false;

            checkBox4PtMode.Enabled = false;
            TextboxISweep1.Enabled = false;
            TextboxISweep2.Enabled = false;
            TextboxISweep3.Enabled = false;
            TextboxISweep4.Enabled = false;

            TextboxTimeDuration.Enabled = false;
        }

        public void UpdateData(int hga)
        {
            int stoppoint = int.Parse(textBoxT1StopPoint.Text);
            int startpoint = int.Parse(textBoxT2StartPoint.Text);

            double[] current = new double[8];
            double lduStartCurrentLimit;
            double LDUXPointSize;
            double lduStopCurrentLimit;

            double cal_results_trend1;
            double cal_results_trend2;
            double error_trend1;
            double error_trend2;

            double result; //=1;//size;

            Double.TryParse(TextboxLDUStepCurrent.Text, out result);
            LDUXPointSize = result / 1000;
            Double.TryParse(TextboxLDUStartCurrent.Text, out result);
            lduStartCurrentLimit = result / 1000;
            Double.TryParse(TextboxLDUStopCurrent.Text, out result);
            lduStopCurrentLimit = result / 1000;

            for (int x = 0; x < 8; x++)
            {
                current[x] = 0; //initialize to 0
            }
            for (int x = 0; x < 8; x++)
            {
                current[x] = lduStartCurrentLimit + (x * LDUXPointSize);

                if (current[x] > lduStopCurrentLimit)
                {
                    current[x] = lduStopCurrentLimit;
                    break;
                }
            }
            if (checkBoxSweepMode.Checked)
            {


                //first get the calculated equation
                UpdateFullSweepPhotoDiodeGraph_line1(hga, stoppoint, false);


                cal_results_trend1 = ISweep_M_Trend1[hga] * current[stoppoint] + ISweep_C_Trend1[hga];


                error_trend1 = Math.Abs(LDUPhotodiodeVoltagePoint[hga, stoppoint] - cal_results_trend1);
                Log.Info(this, "1) LDUPhotodiodeVoltagePoint[{0}][{1}] = {2}, calculation ={3} , y = {4}X + {5}, error = {6}", hga, stoppoint,
                        LDUPhotodiodeVoltagePoint[hga, stoppoint].ToString("F5", CultureInfo.InvariantCulture),
                        cal_results_trend1.ToString("F5", CultureInfo.InvariantCulture),
                        ISweep_M_Trend1[hga].ToString("F5", CultureInfo.InvariantCulture),
                        ISweep_C_Trend1[hga].ToString("F5", CultureInfo.InvariantCulture),
                        error_trend1.ToString("F5", CultureInfo.InvariantCulture));
                while (error_trend1 > i_threshold_error && stoppoint > 1)
                {
                    stoppoint = stoppoint - 1;
                    UpdateFullSweepPhotoDiodeGraph_line1(hga, stoppoint, false);

                    cal_results_trend1 = ISweep_M_Trend1[hga] * current[stoppoint] + ISweep_C_Trend1[hga];
                    error_trend1 = Math.Abs(LDUPhotodiodeVoltagePoint[hga, stoppoint] - cal_results_trend1);

                    Log.Info(this, "2) LDUPhotodiodeVoltagePoint[{0}][{1}] = {2}, calculation ={3} , y = {4}X + {5}, error = {6}", hga, stoppoint,
                        LDUPhotodiodeVoltagePoint[hga, stoppoint].ToString("F5", CultureInfo.InvariantCulture),
                        cal_results_trend1.ToString("F5", CultureInfo.InvariantCulture),
                        ISweep_M_Trend1[hga].ToString("F5", CultureInfo.InvariantCulture),
                        ISweep_C_Trend1[hga].ToString("F5", CultureInfo.InvariantCulture),
                        error_trend1.ToString("F5", CultureInfo.InvariantCulture));

                    if (stoppoint == 1)
                    {
                        break;
                    }
                }

                //Trend 2
                UpdateFullSweepPhotoDiodeGraph_line2(hga, startpoint, false);
                cal_results_trend2 = ISweep_M_Trend2[hga] * current[startpoint] + ISweep_C_Trend2[hga];
                error_trend2 = Math.Abs(LDUPhotodiodeVoltagePoint[hga, stoppoint] - cal_results_trend2);
                Log.Info(this, "3) LDUPhotodiodeVoltagePoint[{0}][{1}] = {2}, calculation ={3} , y = {4}X + {5}, error = {6}", hga, startpoint, LDUPhotodiodeVoltagePoint[hga, startpoint].ToString("F5", CultureInfo.InvariantCulture),
                       cal_results_trend2.ToString("F5", CultureInfo.InvariantCulture),
                       ISweep_M_Trend2[hga].ToString("F5", CultureInfo.InvariantCulture),
                       ISweep_C_Trend2[hga].ToString("F5", CultureInfo.InvariantCulture),
                       error_trend2.ToString("F5", CultureInfo.InvariantCulture));

                while (error_trend2 > i_threshold_error && startpoint < 7)
                {
                    startpoint = startpoint + 1;
                    UpdateFullSweepPhotoDiodeGraph_line2(hga, startpoint, false);

                    cal_results_trend2 = ISweep_M_Trend2[hga] * current[startpoint] + ISweep_C_Trend2[hga];
                    error_trend2 = Math.Abs(LDUPhotodiodeVoltagePoint[hga, startpoint] - cal_results_trend2);

                    
                    Log.Info(this, "4) LDUPhotodiodeVoltagePoint[{0}][{1}] = {2}, calculation ={3} , y = {4}X + {5}, error = {6}", hga, startpoint, LDUPhotodiodeVoltagePoint[hga, startpoint].ToString("F5", CultureInfo.InvariantCulture),
                       cal_results_trend2.ToString("F5", CultureInfo.InvariantCulture),
                       ISweep_M_Trend2[hga].ToString("F5", CultureInfo.InvariantCulture),
                       ISweep_C_Trend2[hga].ToString("F5", CultureInfo.InvariantCulture),
                       error_trend2.ToString("F5", CultureInfo.InvariantCulture));
                    if (startpoint == 7 || LDUPhotodiodeVoltagePoint[hga, startpoint] == 0 )
                    {
                        break;
                    }
                }


                UpdateLEDGraph(hga,false);              
                UpdateLDUGraph(hga, false);
                
            }
            if (checkBox4PtMode.Checked)
            {

                UpdateLEDGraph(hga, false);
                Update4PtSweepPhotoDiodeGraph_line1(hga, false);
                Update4PtSweepPhotoDiodeGraph_line2(hga,false);
                Update4PtLDUGraph(hga,false);

            }

            LDU_IThreshold[hga] = (ISweep_C_Trend2[hga] - ISweep_C_Trend1[hga]) / (ISweep_M_Trend1[hga] - ISweep_M_Trend2[hga]);

            if (this.textBoxLEDInterceptVoltage.InvokeRequired)
            {
                textBoxLEDInterceptVoltage.Invoke(new MethodInvoker(() =>
                {
                    textBoxLEDInterceptVoltage.Text = LED_C[hga].ToString("F6", CultureInfo.InvariantCulture);
                    textBoxIThreshold.Text = LDU_IThreshold[hga].ToString("F6", CultureInfo.InvariantCulture);
                    textBoxRLDU.Text = (LDU_M[hga] * 1000).ToString();
                    textBoxVoltageTon.Text = LDU_C[hga].ToString();
                    textBoxVPDMax.Text = LDUMaxVPD[hga].ToString();
                }));
            }
            else
            {
                textBoxLEDInterceptVoltage.Text = LED_C[hga].ToString("F6", CultureInfo.InvariantCulture);
                textBoxIThreshold.Text = LDU_IThreshold[hga].ToString("F6", CultureInfo.InvariantCulture);
                textBoxRLDU.Text = (LDU_M[hga] * 1000).ToString();
                textBoxVoltageTon.Text = LDU_C[hga].ToString();
                textBoxVPDMax.Text = LDUMaxVPD[hga].ToString();
            }

        }

        private void comboBoxHGASelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            int hga = comboBoxHGASelection.SelectedIndex;
            int stoppoint = int.Parse(textBoxT1StopPoint.Text);
            int startpoint = int.Parse(textBoxT2StartPoint.Text);
            double [] current = new double[8];
            double lduStartCurrentLimit;
            double LDUXPointSize;
            double lduStopCurrentLimit;

            double cal_results_trend1;
            double cal_results_trend2;
            double error_trend1;
            double error_trend2;

            double result; //=1;//size;


        //    LDUVoltagePoint[0, 0] = 1.858;
        //    LDUVoltagePoint[0, 1] = 1.876;
        //    LDUVoltagePoint[0, 2] = 1.894;
        //    LDUVoltagePoint[0, 3] = 1.911;
        //    LDUVoltagePoint[0, 4] = 1.928;
        //    LDUVoltagePoint[0, 5] = 1.945;
        //    LDUVoltagePoint[0, 6] = 1.962;
        //    LDUVoltagePoint[0, 7] = 1.979;

            Double.TryParse(TextboxLDUStepCurrent.Text, out result);
            LDUXPointSize = result / 1000;
            Double.TryParse(TextboxLDUStartCurrent.Text, out result);
            lduStartCurrentLimit = result / 1000;
            Double.TryParse(TextboxLDUStopCurrent.Text, out result);
            lduStopCurrentLimit = result / 1000;

            for (int x = 0; x < 8; x++)
            {
                current[x] = 0; //initialize to 0
            }
            for (int x = 0; x < 8; x++)
            {
                current[x] = lduStartCurrentLimit + (x * LDUXPointSize);

                if(current[x] > lduStopCurrentLimit)
                {
                    current[x] = lduStopCurrentLimit;
                    break;
                }
            }

            if (checkBoxSweepMode.Checked)
            {
               
                //first get the calculated equation
                UpdateFullSweepPhotoDiodeGraph_line1(hga, stoppoint, true);
                

                cal_results_trend1 = ISweep_M_Trend1[hga] * current[stoppoint] + ISweep_C_Trend1[hga];
                

                error_trend1 = Math.Abs(LDUPhotodiodeVoltagePoint[hga, stoppoint] - cal_results_trend1);
                Log.Info(this, "1) LDUPhotodiodeVoltagePoint[{0}][{1}] = {2}, calculation ={3} , y = {4}X + {5}, error = {6}", hga, stoppoint,
                        LDUPhotodiodeVoltagePoint[hga, stoppoint].ToString("F5", CultureInfo.InvariantCulture),
                        cal_results_trend1.ToString("F5", CultureInfo.InvariantCulture), 
                        ISweep_M_Trend1[hga].ToString("F5", CultureInfo.InvariantCulture),
                        ISweep_C_Trend1[hga].ToString("F5", CultureInfo.InvariantCulture),
                        error_trend1.ToString("F5", CultureInfo.InvariantCulture));
                while (error_trend1 > i_threshold_error && stoppoint > 1)
                {                  
                    stoppoint = stoppoint - 1;
                    UpdateFullSweepPhotoDiodeGraph_line1(hga, stoppoint, true);

                    cal_results_trend1 = ISweep_M_Trend1[hga] * current[stoppoint] + ISweep_C_Trend1[hga];
                    error_trend1 = Math.Abs(LDUPhotodiodeVoltagePoint[hga, stoppoint] - cal_results_trend1);

                    Log.Info(this, "2) LDUPhotodiodeVoltagePoint[{0}][{1}] = {2}, calculation ={3} , y = {4}X + {5}, error = {6}", hga, stoppoint, 
                        LDUPhotodiodeVoltagePoint[hga, stoppoint].ToString("F5", CultureInfo.InvariantCulture), 
                        cal_results_trend1.ToString("F5", CultureInfo.InvariantCulture), 
                        ISweep_M_Trend1[hga].ToString("F5", CultureInfo.InvariantCulture), 
                        ISweep_C_Trend1[hga].ToString("F5", CultureInfo.InvariantCulture), 
                        error_trend1.ToString("F5", CultureInfo.InvariantCulture));
                    if(stoppoint == 1)
                    {
                        break;
                    }
                }
                UpdateFullSweepPhotoDiodeGraph_line2(hga, startpoint, true);
                cal_results_trend2 = ISweep_M_Trend2[hga] * current[startpoint] + ISweep_C_Trend2[hga];
                error_trend2 = Math.Abs(LDUPhotodiodeVoltagePoint[hga, stoppoint] - cal_results_trend2);
                Log.Info(this, "3) LDUPhotodiodeVoltagePoint[{0}][{1}] = {2}, calculation ={3} , y = {4}X + {5}, error = {6}", hga, startpoint, LDUPhotodiodeVoltagePoint[hga, startpoint].ToString("F5", CultureInfo.InvariantCulture),
                        cal_results_trend2.ToString("F5", CultureInfo.InvariantCulture),
                        ISweep_M_Trend2[hga].ToString("F5", CultureInfo.InvariantCulture),
                        ISweep_C_Trend2[hga].ToString("F5", CultureInfo.InvariantCulture),
                        error_trend2.ToString("F5", CultureInfo.InvariantCulture));

                while (error_trend2 > i_threshold_error && startpoint < 7)
                {
                    startpoint = startpoint + 1;
                    UpdateFullSweepPhotoDiodeGraph_line2(hga, startpoint, true);

                    cal_results_trend2 = ISweep_M_Trend2[hga] * current[startpoint] + ISweep_C_Trend2[hga];
                    error_trend2 = Math.Abs(LDUPhotodiodeVoltagePoint[hga, startpoint] - cal_results_trend2);

                    Log.Info(this, "4) LDUPhotodiodeVoltagePoint[{0}][{1}] = {2}, calculation ={3} , y = {4}X + {5}, error = {6}", hga, startpoint, LDUPhotodiodeVoltagePoint[hga, startpoint].ToString("F5", CultureInfo.InvariantCulture), 
                        cal_results_trend2.ToString("F5", CultureInfo.InvariantCulture), 
                        ISweep_M_Trend2[hga].ToString("F5", CultureInfo.InvariantCulture), 
                        ISweep_C_Trend2[hga].ToString("F5", CultureInfo.InvariantCulture), 
                        error_trend2.ToString("F5", CultureInfo.InvariantCulture));
                    if (startpoint == 7 || LDUPhotodiodeVoltagePoint[hga, startpoint] == 0)
                {
                        break;
                    }

                }

                
                UpdateLEDGraph(hga, true);
               
                UpdateLDUGraph(hga,true);

                
            }
            if(checkBox4PtMode.Checked)
            {
            

                
                UpdateLEDGraph(hga,true);
                Update4PtSweepPhotoDiodeGraph_line1(hga,true);
                Update4PtSweepPhotoDiodeGraph_line2(hga,true);
                Update4PtLDUGraph(hga,true);
                
            }
            
            LDU_IThreshold[hga] = (ISweep_C_Trend2[hga] - ISweep_C_Trend1[hga]) / (ISweep_M_Trend1[hga] - ISweep_M_Trend2[hga]);

            if (this.textBoxLEDInterceptVoltage.InvokeRequired)
            {
                textBoxLEDInterceptVoltage.Invoke(new MethodInvoker(() =>
                {
                    textBoxLEDInterceptVoltage.Text = LED_C[hga].ToString("F5", CultureInfo.InvariantCulture);
                    textBoxIThreshold.Text = LDU_IThreshold[hga].ToString("F5", CultureInfo.InvariantCulture);
                    textBoxRLDU.Text = (LDU_M[hga] * 1000).ToString();
                    textBoxVoltageTon.Text = LDU_C[hga].ToString();
                    textBoxVPDMax.Text = LDUMaxVPD[hga].ToString();
                }));
            }
            else
            {
                textBoxLEDInterceptVoltage.Text = LED_C[hga].ToString("F5", CultureInfo.InvariantCulture);
                textBoxIThreshold.Text = LDU_IThreshold[hga].ToString("F5", CultureInfo.InvariantCulture);
                textBoxRLDU.Text = (LDU_M[hga] * 1000).ToString();
                textBoxVoltageTon.Text = LDU_C[hga].ToString();
                textBoxVPDMax.Text = LDUMaxVPD[hga].ToString();
            }

           
        }

        private void checkBoxEnableLDU_CheckedChanged(object sender, EventArgs e)
        {
            LDUEnable = checkBoxEnableLDU.Checked;
        }

        private void checkBoxSweepMode_CheckedChanged(object sender, EventArgs e)
        {           
            checkBox4PtMode.Checked = !checkBoxSweepMode.Checked;
            try
            {
         //       comboBoxHGASelection_SelectedIndexChanged(null, null);
            }
            catch { }
        }

        private void checkBox4PtMode_CheckedChanged(object sender, EventArgs e)
        {                      
            checkBoxSweepMode.Checked = !checkBox4PtMode.Checked;
            try
            {
        //        comboBoxHGASelection_SelectedIndexChanged(null, null);
            }
            catch { }
        }

        private void panelLDUMeasurement_Load(object sender, EventArgs e)
        {

            if (this.checkBoxEnableLDU.InvokeRequired)
            {
                checkBoxEnableLDU.Invoke(new MethodInvoker(() =>
                {
                    checkBoxEnableLDU.Checked = LDUEnable;
                    textBoxLEDStartCurrent.Text = LEDStartCurrent.ToString();
                    TextboxLEDStopCurrent.Text = LEDStopCurrent.ToString();
                    textBoxLEDStepCurrent.Text = LEDStepSize.ToString();
                    checkBoxSweepMode.Checked = LDUSweepMode;

                    TextboxLDUStartCurrent.Text = LDUStartCurrent.ToString();
                    TextboxLDUStopCurrent.Text = LDUStopCurrent.ToString();
                    TextboxLDUStepCurrent.Text = LDUStepSize.ToString();
                    textBoxT1StopPoint.Text = T1StopPoint.ToString();
                    textBoxT2StartPoint.Text = T2StartPoint.ToString();

                    checkBox4PtMode.Checked = Point_4Mode;

                    TextboxISweep1.Text = LDUISweep1.ToString();
                    TextboxISweep2.Text = LDUISweep2.ToString();
                    TextboxISweep3.Text = LDUISweep3.ToString();
                    TextboxISweep4.Text = LDUISweep4.ToString();
                    TextboxTimeDuration.Text = LDUTimeInterval.ToString();
                }));
            }
            else
            {
                checkBoxEnableLDU.Checked = LDUEnable;
                textBoxLEDStartCurrent.Text = LEDStartCurrent.ToString();
                TextboxLEDStopCurrent.Text = LEDStopCurrent.ToString();
                textBoxLEDStepCurrent.Text = LEDStepSize.ToString();
                checkBoxSweepMode.Checked = LDUSweepMode;

                TextboxLDUStartCurrent.Text = LDUStartCurrent.ToString();
                TextboxLDUStopCurrent.Text = LDUStopCurrent.ToString();
                TextboxLDUStepCurrent.Text = LDUStepSize.ToString();
                textBoxT1StopPoint.Text = T1StopPoint.ToString();
                textBoxT2StartPoint.Text = T2StartPoint.ToString();

                checkBox4PtMode.Checked = Point_4Mode;

                TextboxISweep1.Text = LDUISweep1.ToString();
                TextboxISweep2.Text = LDUISweep2.ToString();
                TextboxISweep3.Text = LDUISweep3.ToString();
                TextboxISweep4.Text = LDUISweep4.ToString();
                TextboxTimeDuration.Text = LDUTimeInterval.ToString();

            }


               
        }

        private void buttonLDUSettings_Click(object sender, EventArgs e)
        {
            try
            {
                LDUEnable = checkBoxEnableLDU.Checked;
                LEDStartCurrent = int.Parse(textBoxLEDStartCurrent.Text);
                LEDStopCurrent = int.Parse(TextboxLEDStopCurrent.Text);
                LEDStepSize = int.Parse(textBoxLEDStepCurrent.Text);

                LDUSweepMode = checkBoxSweepMode.Checked;
                LDUStartCurrent = int.Parse(TextboxLDUStartCurrent.Text);
                LDUStopCurrent = int.Parse(TextboxLDUStopCurrent.Text);
                LDUStepSize = int.Parse(TextboxLDUStepCurrent.Text);
                T1StopPoint = int.Parse(textBoxT1StopPoint.Text);
                T2StartPoint = int.Parse(textBoxT2StartPoint.Text);
                Point_4Mode = checkBox4PtMode.Checked;

                LDUISweep1 = int.Parse(TextboxISweep1.Text);
                LDUISweep2 = int.Parse(TextboxISweep2.Text);
                LDUISweep3 = int.Parse(TextboxISweep3.Text);
                LDUISweep4 = int.Parse(TextboxISweep4.Text);

                LDUTimeInterval = int.Parse(TextboxTimeDuration.Text);



                EventLDU.Invoke(this, new EventLDUSettings(LDUEnable, LEDStartCurrent, LEDStopCurrent, LEDStepSize,
                                                    LDUSweepMode, LDUStartCurrent, LDUStopCurrent, LDUStepSize,
                                                    Point_4Mode,
                                                    LDUISweep1, LDUISweep2, LDUISweep3, LDUISweep4, LDUTimeInterval,
                                                    T1StopPoint, T2StartPoint));
            }
            catch(Exception ex)
            { }
            
        }

        public void UpdateLDUGraph(int HGAIndex, bool updategraph)
        {
            LDU_M[HGAIndex] = 0;
            LDU_C[HGAIndex] = 0;


            double temp_1 = 0;
            double temp_2 = 0;
            double temp_3 = 0;
            double temp_4 = 0;
            double temp_5 = 0;
            double temp_6 = 0;

            double YBar_LDUVoltageAverage = 0;
            double XBar_LDUCurrentAverage = 0;


            int LDUpoints = 0;


            double LDUSlope = 0;

            double b_ldu = 0;


            int LDUtotalPoints = 3;


            // double[] data = { 1, 2, 2, 2, 2, 2, 7, 5, 6, 4 };


            double LDUXPointSize; //=1;//size;
            double lduStartCurrentLimit; //= 0; //start; // starting point
            double lduStopCurrentLimit; //= 20;// stop; //end point

            double result; //=1;//size;

            Double.TryParse(TextboxLDUStepCurrent.Text, out result);
            LDUXPointSize = result / 1000;
            Double.TryParse(TextboxLDUStartCurrent.Text, out result);
            lduStartCurrentLimit = result / 1000;
            Double.TryParse(TextboxLDUStopCurrent.Text, out result);
            lduStopCurrentLimit = result / 1000;


            double[] XBarLDU = new double[LDUtotalPoints];
            double[] YBarLDU = new double[LDUtotalPoints];

            //double[] LDUVoltageLinearFitPoint = { 1, 2, 2, 2, 2, 2, 4, 4, 4, 4 };// new double[totalPoints];
            //double[] LEDVoltageLinearFitPoint = { 1, 2, 2, 2, 2, 2, 4, 4, 4, 4 };


            double[] LDUVoltageLinearFitPoint = new double[LDUtotalPoints];// new double[totalPoints];


         //   labelymxcLDU.Text = "";
            //labelymxcLED.Text = "";

            int k = 0;

            try
            {
                //17/11/2020
                //Request to use point 1, 2 and 3 to do calculation
                // calculate x and y bar
                for (int i = 0; i < LDUtotalPoints; i++) //
                {
                    if (LDUVoltagePoint[HGAIndex, i] != 0)
                    {
                        temp_1 += LDUVoltagePoint[HGAIndex, i];
                        temp_3 += lduStartCurrentLimit + (i * LDUXPointSize);

                        
                        LDUpoints++;
                        Log.Info(this, "HGA{0}, temp_1={1}, temp_3 = {2}, LDUpoints = {3} , lduVoltage= {4}", HGAIndex, temp_1, temp_3, LDUpoints, LDUVoltagePoint[HGAIndex, i]);
                    }
                }
                YBar_LDUVoltageAverage = temp_1 / LDUpoints;
                XBar_LDUCurrentAverage = temp_3 / LDUpoints;

                Log.Info(this, "HGA{0}, YBar_LDUVoltageAverage={1}, XBar_LDUCurrentAverage = {2}, LDUpoints = {3}, temp_1 = {4}, temp_3 = {5} ", HGAIndex, YBar_LDUVoltageAverage, XBar_LDUCurrentAverage, LDUpoints, temp_1, temp_3);

                if (temp_1 == 0 && temp_3 == 0)
                {
                    YBar_LDUVoltageAverage = 0;
                    XBar_LDUCurrentAverage = 0;

                }
                //17/11/2020
                //Request to use point 1, 2 and 3 to do calculation
                for (int i = 0; i < 3; i++)// LDUpoints; i++)
                {
                    YBarLDU[i] = LDUVoltagePoint[HGAIndex, i] - YBar_LDUVoltageAverage;//(Y-YBar)
                    XBarLDU[i] = (lduStartCurrentLimit + (i * LDUXPointSize)) - XBar_LDUCurrentAverage; // (X -XBar)

                    temp_4 += XBarLDU[i] * YBarLDU[i]; //(X -XBar) * (Y-YBar) = LDU                    
                    temp_6 += XBarLDU[i] * XBarLDU[i]; // (X -XBar)^2

                    Log.Info(this, "HGA{0}, YBarLDU[i]={1}, XBarLDU[i] = {2}, LDUpoints = {3}, temp_4 = {4}, temp_6 = {5} ,  LDUVoltagePoint[HGAIndex, i] = {6} ", HGAIndex, YBarLDU[i], XBarLDU[i], LDUpoints, temp_4, temp_6, LDUVoltagePoint[HGAIndex, i]);

                }
                //calculate M
                LDUSlope = (temp_4 / temp_6); // m -->(X -XBar) * (Y-YBar)/(X -XBar)^2
                Log.Info(this, "HGA{0}, LDUSlope = {1}, LDUpoints = {2}, temp_4 = {3}, temp_6 = {4} ", HGAIndex, LDUSlope, LDUpoints, temp_4, temp_6);


                if (temp_4 == 0 && temp_6 == 0)
                {
                    LDUSlope = 0;
                }

                b_ldu = YBar_LDUVoltageAverage - (LDUSlope * XBar_LDUCurrentAverage);

                if (this.textBoxITrendLineLDU.InvokeRequired)
                {
                    textBoxITrendLineLDU.Invoke(new MethodInvoker(() =>
                    {
                        textBoxITrendLineLDU.Text = String.Format("y={0} X + {1}", (LDUSlope).ToString("F3", CultureInfo.InvariantCulture), b_ldu.ToString("F6", CultureInfo.InvariantCulture));
                    }));
                }
                else
                {
                    textBoxITrendLineLDU.Text = String.Format("y={0} X + {1}", (LDUSlope).ToString("F3", CultureInfo.InvariantCulture), b_ldu.ToString("F6", CultureInfo.InvariantCulture));
                }
                //sweep
                LDU_M[HGAIndex] = LDUSlope;
                Log.Info(this, "HGA{0}, LDUSlope = {1}, LDU_M[HGAIndex]  = {2}, temp_4 = {3}, temp_6 = {4} ", HGAIndex, LDUSlope, LDU_M[HGAIndex], temp_4, temp_6);
                LDU_C[HGAIndex] = b_ldu;// Math.Round(, 4, MidpointRounding.ToEven);
                if (!updategraph)
                    return;
                chartLDU.Series[0].Points.Clear();
                chartLDU.Series[1].Points.Clear();

                temp_3 = 100000;
                temp_1 = 0;
                for (double i = lduStartCurrentLimit; i <= lduStopCurrentLimit; i += LDUXPointSize)
                {

                    if (k == LDUpoints)
                        break;

                    DataPoint d_LDU = new DataPoint(i, LDUVoltagePoint[HGAIndex, k]);
                    d_LDU.ToolTip = "#VALY";

                    LDUVoltageLinearFitPoint[k] = (LDUSlope * i) + b_ldu;

                    DataPoint d_LDULFit = new DataPoint(i, LDUVoltageLinearFitPoint[k]);
                    d_LDULFit.ToolTip = "#VALY";
                   

                    chartLDU.Series[0].Points.Add(d_LDU);
                    chartLDU.Series[1].Points.Add(d_LDULFit);

                    if (LDUVoltagePoint[HGAIndex, k] > temp_1)
                        temp_1 = LDUVoltagePoint[HGAIndex, k];

                    if (temp_3 > LDUVoltagePoint[HGAIndex, k])
                        temp_3 = LDUVoltagePoint[HGAIndex, k];

                    k++;
                }

                this.chartLDU.ChartAreas[0].AxisX.Interval = LDUXPointSize;
                this.chartLDU.ChartAreas[0].AxisX.Minimum = (double)(lduStartCurrentLimit);
                this.chartLDU.ChartAreas[0].AxisX.Maximum = (double)(lduStopCurrentLimit + LDUXPointSize);
                this.chartLDU.ChartAreas[0].AxisY.Interval = 0.5;//(double)(lduStopCurrentLimit + LDUXPointSize) / LDUpoints;
                this.chartLDU.ChartAreas[0].AxisY.Minimum = 0;// temp_3 - chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, 0] * 0.5;//(0.5 * LDUVoltagePoint[HGAIndex, 0]);
                this.chartLDU.ChartAreas[0].AxisY.Maximum = 5;//temp_1 + chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, Ypoints - 1] + 0.2;

            }

            catch
            {

            }

        }

        public void UpdateLEDGraph(int HGAIndex, bool updategraph)
        {
            double temp_1 = 0;
            double temp_2 = 0;
            double temp_3 = 0;
            double temp_4 = 0;
            double temp_5 = 0;
            double temp_6 = 0;

            double YBar_LEDVoltageAverage = 0;
            double XBar_LEDCurrentAverage = 0;


            int LEDpoints = 0;


            double LEDSlope = 0;

            double b_led = 0;


            int LEDtotalPoints = 8;

            double LEDXPointSize;//size;
            double ledStartCurrentLimit; //start; // starting point
            double ledStopCurrentLimit;// stop; //end point



            double result; //=1;//size;

            
            Double.TryParse(textBoxLEDStepCurrent.Text, out result);
            LEDXPointSize = result / 1000;
            Double.TryParse(textBoxLEDStartCurrent.Text, out result);
            ledStartCurrentLimit = result / 1000;
            Double.TryParse(TextboxLEDStopCurrent.Text, out result);
            ledStopCurrentLimit = result / 1000;


            double[] XBarLED = new double[LEDtotalPoints];
            double[] YBarLED = new double[LEDtotalPoints];

            double[] LEDVoltageLinearFitPoint = new double[LEDtotalPoints];// new double[totalPoints];



         //   labelymxcLED.Text = "";

            int k = 0;

            try
            {
                // calculate x and y bar
                for (int i = 0; i < LEDtotalPoints; i++)
                {
                    if (LEDVoltagePoint[HGAIndex, i] != 0)
                    {
                        temp_1 += LEDVoltagePoint[HGAIndex, i];
                        temp_3 += ledStartCurrentLimit + (i * LEDXPointSize);


                        LEDpoints++;
                    }


                }
                YBar_LEDVoltageAverage = temp_1 / LEDpoints;
                XBar_LEDCurrentAverage = temp_3 / LEDpoints;

                if (temp_1 == 0 && temp_3 == 0)
                {
                    YBar_LEDVoltageAverage = 0;
                    XBar_LEDCurrentAverage = 0;

                }
                for (int i = 0; i < LEDpoints; i++)
                {
                    YBarLED[i] = LEDVoltagePoint[HGAIndex, i] - YBar_LEDVoltageAverage;//(Y-YBar)
                    XBarLED[i] = (ledStartCurrentLimit + (i * LEDXPointSize)) - XBar_LEDCurrentAverage; // (X -XBar)

                    temp_4 += XBarLED[i] * YBarLED[i]; //(X -XBar) * (Y-YBar) = LDU                    
                    temp_6 += XBarLED[i] * XBarLED[i]; // (X -XBar)^2
                }
                //calculate M
                LEDSlope = (temp_4 / temp_6); // m -->(X -XBar) * (Y-YBar)/(X -XBar)^2


                if (temp_4 == 0 && temp_6 == 0)
                {
                    LEDSlope = 0;
                }

                b_led = YBar_LEDVoltageAverage - (LEDSlope * XBar_LEDCurrentAverage);

                if (this.textBoxLEDTrendLine.InvokeRequired)
                {
                    textBoxLEDTrendLine.Invoke(new MethodInvoker(() =>
                    {
                        textBoxLEDTrendLine.Text = String.Format("y={0} X + {1}", (LEDSlope).ToString("F5", CultureInfo.InvariantCulture), b_led.ToString("F5", CultureInfo.InvariantCulture));
                        textBoxLEDInterceptVoltage.Text = String.Format("Y-Intercept = {0}", b_led.ToString("F5", CultureInfo.InvariantCulture));
                    }));
                }
                else
                {
                    textBoxLEDTrendLine.Text = String.Format("y={0} X + {1}", (LEDSlope).ToString("F5", CultureInfo.InvariantCulture), b_led.ToString("F5", CultureInfo.InvariantCulture));
                    textBoxLEDInterceptVoltage.Text = String.Format("Y-Intercept = {0}", b_led.ToString("F5", CultureInfo.InvariantCulture));
                }

                LED_M[HGAIndex] = LEDSlope;// Math.Round(LEDSlope,4,MidpointRounding.ToEven);
                LED_C[HGAIndex] = b_led;// Math.Round(b_led, 4, MidpointRounding.ToEven);
                if (!updategraph)
                    return;

                chartLED.Series[0].Points.Clear();
                chartLED.Series[1].Points.Clear();

                temp_3 = 100000;
                temp_1 = 0;
                for (double i = ledStartCurrentLimit; i <= ledStopCurrentLimit; i += LEDXPointSize)
                {

                    if (k == LEDpoints)
                        break;

                    DataPoint d_LED = new DataPoint(i, LEDVoltagePoint[HGAIndex, k]);
                    d_LED.ToolTip = "#VALY";

                    LEDVoltageLinearFitPoint[k] = (LEDSlope * i) + b_led;

                    DataPoint d_LEDFit = new DataPoint(i, LEDVoltageLinearFitPoint[k]);
                    d_LEDFit.ToolTip = "#VALY";

                    chartLED.Series[0].Points.Add(d_LED);
                    chartLED.Series[1].Points.Add(d_LEDFit);

                    if (LEDVoltagePoint[HGAIndex, k] > temp_1)
                        temp_1 = LEDVoltagePoint[HGAIndex, k];

                    if (temp_3 > LEDVoltagePoint[HGAIndex, k])
                        temp_3 = LEDVoltagePoint[HGAIndex, k];

                    k++;
                }

                this.chartLED.ChartAreas[0].AxisX.Interval = LEDXPointSize;
                this.chartLED.ChartAreas[0].AxisX.Minimum = (double)(ledStartCurrentLimit);
                this.chartLED.ChartAreas[0].AxisX.Maximum = (double)(ledStopCurrentLimit + LEDXPointSize);
                this.chartLED.ChartAreas[0].AxisY.Interval = 0.5;//(double)(ledStopCurrentLimit + LEDXPointSize) / LEDpoints;
                this.chartLED.ChartAreas[0].AxisY.Minimum = temp_3 - chartLED.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, 0] * 0.5;//(0.5 * LDUVoltagePoint[HGAIndex, 0]);
                this.chartLED.ChartAreas[0].AxisY.Maximum = temp_1 + chartLED.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, Ypoints - 1] + 0.2;

            }

            catch
            {

            }

        }
  
        public void Update4PtSweepPhotoDiodeGraph_line1(int HGAIndex, bool updategraph)
        {

            double temp_1 = 0;
            double temp_2 = 0;
            double temp_3 = 0;
            double temp_4 = 0;
            double temp_5 = 0;
            double temp_6 = 0;

            double YBar_LDUVoltageAverage = 0;
            double XBar_LDUCurrentAverage = 0;


            int LDUpoints = 0;


            double LDUSlope = 0;

            double b_ldu = 0;


            int LDUtotalPoints = 2; //4 point


            // double[] data = { 1, 2, 2, 2, 2, 2, 7, 5, 6, 4 }; 


            double LDUXPointSize; //=1;//size;
            double lduStartCurrentLimit; //= 0; //start; // starting point
            double lduStopCurrentLimit; //= 20;// stop; //end point

            double result; //=1;//size;

         //   Double.TryParse(TextboxLDUStepCurrent.Text, out result);
         //   LDUXPointSize = result / 1000;
            Double.TryParse(TextboxISweep1.Text, out result);
            lduStartCurrentLimit = result / 1000;
            Double.TryParse(TextboxISweep2.Text, out result);
            lduStopCurrentLimit = result / 1000;

            LDUXPointSize = lduStopCurrentLimit - lduStartCurrentLimit;

            double[] XBarLDU = new double[LDUtotalPoints];
            double[] YBarLDU = new double[LDUtotalPoints];

            //double[] LDUVoltageLinearFitPoint = { 1, 2, 2, 2, 2, 2, 4, 4, 4, 4 };// new double[totalPoints];
            //double[] LEDVoltageLinearFitPoint = { 1, 2, 2, 2, 2, 2, 4, 4, 4, 4 };


            double[] LDUVoltageLinearFitPoint = new double[LDUtotalPoints];// new double[totalPoints];


            //   labelymxcLDU.Text = "";
            //labelymxcLED.Text = "";

            int k = 0;

            try
            {
                // calculate x and y bar
                for (int i = 0; i < LDUtotalPoints; i++)
                {
                    if (LDUPhotodiodeVoltagePoint[HGAIndex, i] != 0)
                    {
                        temp_1 += LDUPhotodiodeVoltagePoint[HGAIndex, i];
                        temp_3 += lduStartCurrentLimit + (i * LDUXPointSize);


                        LDUpoints++;
                    }
                }

                YBar_LDUVoltageAverage = temp_1 / LDUpoints;
                XBar_LDUCurrentAverage = temp_3 / LDUpoints;

                if (temp_1 == 0 && temp_3 == 0)
                {
                    YBar_LDUVoltageAverage = 0;
                    XBar_LDUCurrentAverage = 0;

                }
                for (int i = 0; i < LDUpoints; i++)
                {
                    YBarLDU[i] = LDUPhotodiodeVoltagePoint[HGAIndex, i] - YBar_LDUVoltageAverage;//(Y-YBar)
                    XBarLDU[i] = (lduStartCurrentLimit + (i * LDUXPointSize)) - XBar_LDUCurrentAverage; // (X -XBar)

                    temp_4 += XBarLDU[i] * YBarLDU[i]; //(X -XBar) * (Y-YBar) = LDU                    
                    temp_6 += XBarLDU[i] * XBarLDU[i]; // (X -XBar)^2
                }
                //calculate M
                LDUSlope = (temp_4 / temp_6); // m -->(X -XBar) * (Y-YBar)/(X -XBar)^2


                if (temp_4 == 0 && temp_6 == 0)
                {
                    LDUSlope = 0;
                }
                if (Math.Abs(LDUSlope) < 0.001)
                    LDUSlope = 0;
                b_ldu = YBar_LDUVoltageAverage - (LDUSlope * XBar_LDUCurrentAverage);

                if (this.textBoxITrendLine1.InvokeRequired)
                {
                    textBoxITrendLine1.Invoke(new MethodInvoker(() =>
                    {
                        textBoxITrendLine1.Text = String.Format("y={0} X + {1}", (LDUSlope).ToString("F5", CultureInfo.InvariantCulture), b_ldu.ToString("F5", CultureInfo.InvariantCulture));
                    }));
                }
                else
                {
                    textBoxITrendLine1.Text = String.Format("y={0} X + {1}", (LDUSlope).ToString("F5", CultureInfo.InvariantCulture), b_ldu.ToString("F5", CultureInfo.InvariantCulture));
                }

                ISweep_M_Trend1[HGAIndex] = LDUSlope;// Math.Round(LDUSlope, 4, MidpointRounding.ToEven);
                ISweep_C_Trend1[HGAIndex] = b_ldu;// Math.Round(b_ldu, 4, MidpointRounding.ToEven);
                if (!updategraph)
                    return;

                chartLDU.Series[2].Points.Clear();
                chartLDU.Series[3].Points.Clear();
                temp_3 = 100000;
                temp_1 = 0;
                for (double i = lduStartCurrentLimit; i <= lduStopCurrentLimit; i += LDUXPointSize)
                {

                    if (k == LDUpoints)
                        break;

                    DataPoint d_LDU = new DataPoint(i, LDUPhotodiodeVoltagePoint[HGAIndex, k]);
                    d_LDU.ToolTip = "#VALY";

                    LDUVoltageLinearFitPoint[k] = (LDUSlope * i) + b_ldu;

                    DataPoint d_LDULFit = new DataPoint(i, LDUVoltageLinearFitPoint[k]);
                    d_LDULFit.ToolTip = "#VALY";


                    chartLDU.Series[2].Points.Add(d_LDU);
                    chartLDU.Series[3].Points.Add(d_LDULFit);

                    if (LDUPhotodiodeVoltagePoint[HGAIndex, k] > temp_1)
                        temp_1 = LDUPhotodiodeVoltagePoint[HGAIndex, k];

                    if (temp_3 > LDUPhotodiodeVoltagePoint[HGAIndex, k])
                        temp_3 = LDUPhotodiodeVoltagePoint[HGAIndex, k];

                    k++;
                }

                this.chartLDU.ChartAreas[0].AxisX.Interval = LDUXPointSize;
                this.chartLDU.ChartAreas[0].AxisX.Minimum = (double)(lduStartCurrentLimit);
                this.chartLDU.ChartAreas[0].AxisX.Maximum = (double)(lduStopCurrentLimit + LDUXPointSize);
                this.chartLDU.ChartAreas[0].AxisY.Interval = 0.5;//Math.Round((double)(lduStopCurrentLimit + LDUXPointSize) / LDUpoints, 3);
                this.chartLDU.ChartAreas[0].AxisY.Minimum = temp_3 - chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, 0] * 0.5;//(0.5 * LDUVoltagePoint[HGAIndex, 0]);
                this.chartLDU.ChartAreas[0].AxisY.Maximum = 5;//temp_1 + chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, Ypoints - 1] + 0.2;

            }

            catch
            {

            }
        }

        public void Update4PtSweepPhotoDiodeGraph_line2(int HGAIndex, bool updategraph)
        {

            double temp_1 = 0;
            double temp_2 = 0;
            double temp_3 = 0;
            double temp_4 = 0;
            double temp_5 = 0;
            double temp_6 = 0;

            double YBar_LDUVoltageAverage = 0;
            double XBar_LDUCurrentAverage = 0;


            int LDUpoints = 0;


            double LDUSlope = 0;

            double b_ldu = 0;


            int LDUtotalPoints = 2; //4 point

            double LDUXPointSize; //=1;//size;
            double lduStartCurrentLimit; //= 0; //start; // starting point
            double lduStopCurrentLimit; //= 20;// stop; //end point

            double result; //=1;//size;

           
            Double.TryParse(TextboxISweep3.Text, out result);
            lduStartCurrentLimit = result / 1000;
            Double.TryParse(TextboxISweep4.Text, out result);
            lduStopCurrentLimit = result / 1000;

            LDUXPointSize = Math.Round(lduStopCurrentLimit - lduStartCurrentLimit,2);

            double[] XBarLDU = new double[LDUtotalPoints];
            double[] YBarLDU = new double[LDUtotalPoints];

            double[] LDUVoltageLinearFitPoint = new double[LDUtotalPoints];// new double[totalPoints];

            int k = 0;

            try
            {
                // calculate x and y bar
                for (int i = 0; i < LDUtotalPoints; i++)
                {
                    if (LDUPhotodiodeVoltagePoint[HGAIndex, i+2] != 0)
                    {
                        temp_1 += LDUPhotodiodeVoltagePoint[HGAIndex, i+2];
                        temp_3 += lduStartCurrentLimit + (i * LDUXPointSize);


                        LDUpoints++;
                    }


                }
                YBar_LDUVoltageAverage = temp_1 / LDUpoints;
                XBar_LDUCurrentAverage = temp_3 / LDUpoints;

                if (temp_1 == 0 && temp_3 == 0)
                {
                    YBar_LDUVoltageAverage = 0;
                    XBar_LDUCurrentAverage = 0;

                }
                for (int i = 0; i < LDUpoints; i++)
                {
                    YBarLDU[i] = LDUPhotodiodeVoltagePoint[HGAIndex, i+2] - YBar_LDUVoltageAverage;//(Y-YBar)
                    XBarLDU[i] = (lduStartCurrentLimit + (i * LDUXPointSize)) - XBar_LDUCurrentAverage; // (X -XBar)

                    temp_4 += XBarLDU[i] * YBarLDU[i]; //(X -XBar) * (Y-YBar) = LDU                    
                    temp_6 += XBarLDU[i] * XBarLDU[i]; // (X -XBar)^2

                    LDUMaxVPD[HGAIndex] = LDUPhotodiodeVoltagePoint[HGAIndex, i + 2];
                }
                //calculate M
                LDUSlope = (temp_4 / temp_6); // m -->(X -XBar) * (Y-YBar)/(X -XBar)^2


                if (temp_4 == 0 && temp_6 == 0)
                {
                    LDUSlope = 0;
                }
                if (Math.Abs(LDUSlope) < 0.001)
                    LDUSlope = 0;
                b_ldu = YBar_LDUVoltageAverage - (LDUSlope * XBar_LDUCurrentAverage);

                if (this.textBoxITrendLine2.InvokeRequired)
                {
                    textBoxITrendLine2.Invoke(new MethodInvoker(() =>
                    {
                        textBoxITrendLine2.Text = String.Format("y={0} X + {1}", (LDUSlope).ToString("F5", CultureInfo.InvariantCulture), b_ldu.ToString("F5", CultureInfo.InvariantCulture));
                    }));
                }
                else
                {
                    textBoxITrendLine2.Text = String.Format("y={0} X + {1}", (LDUSlope).ToString("F5", CultureInfo.InvariantCulture), b_ldu.ToString("F5", CultureInfo.InvariantCulture));
                }

                ISweep_M_Trend2[HGAIndex] = LDUSlope;// Math.Round(LDUSlope, 4, MidpointRounding.ToEven);
                ISweep_C_Trend2[HGAIndex] = b_ldu;// Math.Round(b_ldu, 4, MidpointRounding.ToEven);

                if (!updategraph)
                    return;
                chartLDU.Series[4].Points.Clear();
                chartLDU.Series[5].Points.Clear();
                temp_3 = 100000;
                temp_1 = 0;
                for (double i = lduStartCurrentLimit; i <= lduStopCurrentLimit; i += LDUXPointSize)
                {

                    if (k == LDUpoints)
                        break;

                    DataPoint d_LDU = new DataPoint(i, LDUPhotodiodeVoltagePoint[HGAIndex, k + 2]);
                    d_LDU.ToolTip = "#VALY";

                    LDUVoltageLinearFitPoint[k] = (LDUSlope * i) + b_ldu;

                    DataPoint d_LDULFit = new DataPoint(i, LDUVoltageLinearFitPoint[k]);
                    d_LDULFit.ToolTip = "#VALY";


                    chartLDU.Series[4].Points.Add(d_LDU);
                    chartLDU.Series[5].Points.Add(d_LDULFit);

                    if (LDUPhotodiodeVoltagePoint[HGAIndex, k+2] > temp_1)
                        temp_1 = LDUPhotodiodeVoltagePoint[HGAIndex, k+2];

                    if (temp_3 > LDUPhotodiodeVoltagePoint[HGAIndex, k+2])
                        temp_3 = LDUPhotodiodeVoltagePoint[HGAIndex, k+2];

                    k++;
                }

                this.chartLDU.ChartAreas[0].AxisX.Interval = LDUXPointSize;
                this.chartLDU.ChartAreas[0].AxisX.Minimum = 0;// (double)(lduStartCurrentLimit);
                this.chartLDU.ChartAreas[0].AxisX.Maximum = (double)(lduStopCurrentLimit + LDUXPointSize);
                this.chartLDU.ChartAreas[0].AxisY.Interval = 0.5;//(double)(lduStopCurrentLimit + LDUXPointSize) / LDUpoints;
                this.chartLDU.ChartAreas[0].AxisY.Minimum = 0;// temp_3 - chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, 0] * 0.5;//(0.5 * LDUVoltagePoint[HGAIndex, 0]);
                this.chartLDU.ChartAreas[0].AxisY.Maximum = temp_1 + chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, Ypoints - 1] + 0.2;

            }

            catch
            {

            }
        }

        public void Update4PtLDUGraph(int HGAIndex, bool updategraph)
        {

            double temp_1 = 0;
            double temp_2 = 0;
            double temp_3 = 0;
            double temp_4 = 0;
            double temp_5 = 0;
            double temp_6 = 0;

            double YBar_LDUVoltageAverage = 0;
            double XBar_LDUCurrentAverage = 0;


            int LDUpoints = 0;


            double LDUSlope = 0;

            double b_ldu = 0;


            int LDUtotalPoints = 2; //4 point

            double LDUXPointSize; //=1;//size;
            double [] ISweepCurrentPoint = new double[2];
            double result; //=1;//size;

            Double.TryParse(TextboxISweep1.Text, out result);
            ISweepCurrentPoint[0] = result / 1000;
            Double.TryParse(TextboxISweep2.Text, out result);
            ISweepCurrentPoint[1] = result / 1000;

            LDUXPointSize = ISweepCurrentPoint[1] - ISweepCurrentPoint[0];

            double[] XBarLDU = new double[LDUtotalPoints];
            double[] YBarLDU = new double[LDUtotalPoints];

            //double[] LDUVoltageLinearFitPoint = { 1, 2, 2, 2, 2, 2, 4, 4, 4, 4 };// new double[totalPoints];
            //double[] LEDVoltageLinearFitPoint = { 1, 2, 2, 2, 2, 2, 4, 4, 4, 4 };


            double[] LDUVoltageLinearFitPoint = new double[LDUtotalPoints];// new double[totalPoints];
            int k = 0;

            try
            {
                // calculate x and y bar
                for (int i = 0; i < LDUtotalPoints; i++)
                {
                    if (LDUVoltagePoint[HGAIndex, i] != 0)
                    {
                        temp_1 += LDUVoltagePoint[HGAIndex, i];
                        //temp_3 += lduStartCurrentLimit + (i * LDUXPointSize);
                        temp_3 += ISweepCurrentPoint[i];

                        LDUpoints++;
                    }


                }
                YBar_LDUVoltageAverage = temp_1 / LDUpoints;
                XBar_LDUCurrentAverage = temp_3 / LDUpoints;

                if (temp_1 == 0 && temp_3 == 0)
                {
                    YBar_LDUVoltageAverage = 0;
                    XBar_LDUCurrentAverage = 0;

                }
                for (int i = 0; i < LDUpoints; i++)
                {
                    YBarLDU[i] = LDUVoltagePoint[HGAIndex, i] - YBar_LDUVoltageAverage;//(Y-YBar)
                    //XBarLDU[i] = (lduStartCurrentLimit + (i * LDUXPointSize)) - XBar_LDUCurrentAverage; // (X -XBar)
                    XBarLDU[i] = ISweepCurrentPoint[i] - XBar_LDUCurrentAverage; // (X -XBar)
                    temp_4 += XBarLDU[i] * YBarLDU[i]; //(X -XBar) * (Y-YBar) = LDU                    
                    temp_6 += XBarLDU[i] * XBarLDU[i]; // (X -XBar)^2
                }
                //calculate M
                LDUSlope = (temp_4 / temp_6); // m -->(X -XBar) * (Y-YBar)/(X -XBar)^2


                if (temp_4 == 0 && temp_6 == 0)
                {
                    LDUSlope = 0;
                }

                b_ldu = YBar_LDUVoltageAverage - (LDUSlope * XBar_LDUCurrentAverage);

                if (this.textBoxITrendLineLDU.InvokeRequired)
                {
                    textBoxITrendLineLDU.Invoke(new MethodInvoker(() =>
                    {
                        textBoxITrendLineLDU.Text = String.Format("y={0} X + {1}", (LDUSlope).ToString("F5", CultureInfo.InvariantCulture), b_ldu.ToString("F5", CultureInfo.InvariantCulture));
                    }));
                }
                else
                {
                    textBoxITrendLineLDU.Text = String.Format("y={0} X + {1}", (LDUSlope).ToString("F5", CultureInfo.InvariantCulture), b_ldu.ToString("F5", CultureInfo.InvariantCulture));
                }
                //4pt
                LDU_M[HGAIndex] = LDUSlope;// Math.Round(LDUSlope, 4, MidpointRounding.ToEven);
                LDU_C[HGAIndex] = b_ldu;// Math.Round(b_ldu, 4, MidpointRounding.ToEven);

                if (!updategraph)
                    return;

                chartLDU.Series[0].Points.Clear();
                chartLDU.Series[1].Points.Clear();

                temp_3 = 100000;
                temp_1 = 0;
                //for (double i = lduStartCurrentLimit; i <= lduStopCurrentLimit; i += LDUXPointSize)
                for (int i = 0; i <= 2; i ++)
                {

                    if (k == LDUpoints)
                        break;

                    DataPoint d_LDU = new DataPoint(ISweepCurrentPoint[i], LDUVoltagePoint[HGAIndex, k]);
                    d_LDU.ToolTip = "#VALY";

                    LDUVoltageLinearFitPoint[k] = (LDUSlope * ISweepCurrentPoint[i]) + b_ldu;

                    DataPoint d_LDULFit = new DataPoint(ISweepCurrentPoint[i], LDUVoltageLinearFitPoint[k]);
                    d_LDULFit.ToolTip = "#VALY";


                    chartLDU.Series[0].Points.Add(d_LDU);
                    chartLDU.Series[1].Points.Add(d_LDULFit);

                    if (LDUVoltagePoint[HGAIndex, k] > temp_1)
                        temp_1 = LDUVoltagePoint[HGAIndex, k];

                    if (temp_3 > LDUVoltagePoint[HGAIndex, k])
                        temp_3 = LDUVoltagePoint[HGAIndex, k];

                    k++;
                }

                this.chartLDU.ChartAreas[0].AxisX.Interval = LDUXPointSize;
                this.chartLDU.ChartAreas[0].AxisX.Minimum = (double)(ISweepCurrentPoint[0]);
                this.chartLDU.ChartAreas[0].AxisX.Maximum = (double)(ISweepCurrentPoint[3] + ISweepCurrentPoint[0]);
                this.chartLDU.ChartAreas[0].AxisY.Interval = 0.5;//Math.Round((double)(lduStopCurrentLimit + LDUXPointSize) / LDUpoints, 3);
                this.chartLDU.ChartAreas[0].AxisY.Minimum = 0;// temp_3 - chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, 0] * 0.5;//(0.5 * LDUVoltagePoint[HGAIndex, 0]);
                this.chartLDU.ChartAreas[0].AxisY.Maximum = 5;//temp_1 + chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, Ypoints - 1] + 0.2;

            }

            catch
            {

            }
        }

        public void UpdateFullSweepPhotoDiodeGraph_line1(int HGAIndex, bool updategraph)
        {

            double temp_1 = 0;
            double temp_2 = 0;
            double temp_3 = 0;
            double temp_4 = 0;
            double temp_5 = 0;
            double temp_6 = 0;

            double YBar_LDUVoltageAverage = 0;
            double XBar_LDUCurrentAverage = 0;


            int LDUpoints = 0;


            double LDUSlope = 0;

            double b_ldu = 0;


            int LDUtotalPoints = int.Parse(textBoxT1StopPoint.Text) + 1;
            
            double LDUXPointSize; //=1;//size;
            double lduStartCurrentLimit; //= 0; //start; // starting point
            double lduStopCurrentLimit; //= 20;// stop; //end point

            double result; //=1;//size;

            Double.TryParse(TextboxLDUStepCurrent.Text, out result);
            LDUXPointSize = result / 1000;
            Double.TryParse(TextboxLDUStartCurrent.Text, out result);
            lduStartCurrentLimit = result / 1000;
           // Double.TryParse(TextboxLDUStopCurrent.Text, out result);
            lduStopCurrentLimit = lduStartCurrentLimit + (LDUXPointSize * (LDUtotalPoints - 1));


            double[] XBarLDU = new double[LDUtotalPoints];
            double[] YBarLDU = new double[LDUtotalPoints];

            double[] LDUVoltageLinearFitPoint = new double[LDUtotalPoints];// new double[totalPoints];

            int k = 0;

            try
            {
                // calculate x and y bar
                for (int i = 0; i < LDUtotalPoints; i++)
                {
                    if (LDUPhotodiodeVoltagePoint[HGAIndex, i] != 0)
                    {
                        temp_1 += LDUPhotodiodeVoltagePoint[HGAIndex, i];
                        temp_3 += lduStartCurrentLimit + (i * LDUXPointSize);


                        LDUpoints++;
                    }


                }
                YBar_LDUVoltageAverage = temp_1 / LDUpoints;
                XBar_LDUCurrentAverage = temp_3 / LDUpoints;

                if (temp_1 == 0 && temp_3 == 0)
                {
                    YBar_LDUVoltageAverage = 0;
                    XBar_LDUCurrentAverage = 0;

                }
                for (int i = 0; i < LDUpoints; i++)
                {
                    YBarLDU[i] = LDUPhotodiodeVoltagePoint[HGAIndex, i] - YBar_LDUVoltageAverage;//(Y-YBar)
                    XBarLDU[i] = (lduStartCurrentLimit + (i * LDUXPointSize)) - XBar_LDUCurrentAverage; // (X -XBar)

                    temp_4 += XBarLDU[i] * YBarLDU[i]; //(X -XBar) * (Y-YBar) = LDU                    
                    temp_6 += XBarLDU[i] * XBarLDU[i]; // (X -XBar)^2
                }
                //calculate M
                LDUSlope = (temp_4 / temp_6); // m -->(X -XBar) * (Y-YBar)/(X -XBar)^2


                if (temp_4 == 0 && temp_6 == 0)
                {
                    LDUSlope = 0;
                }
                if (Math.Abs(LDUSlope) < 0.001)
                    LDUSlope = 0;
                b_ldu = YBar_LDUVoltageAverage - (LDUSlope * XBar_LDUCurrentAverage);

                if (this.textBoxITrendLine1.InvokeRequired)
                {
                    textBoxITrendLine1.Invoke(new MethodInvoker(() =>
                    {
                        textBoxITrendLine1.Text = String.Format("y={0} X + {1}", (LDUSlope).ToString("F5", CultureInfo.InvariantCulture), b_ldu.ToString("F5", CultureInfo.InvariantCulture));
                    }));
                }
                else
                {
                    textBoxITrendLine1.Text = String.Format("y={0} X + {1}", (LDUSlope).ToString("F5", CultureInfo.InvariantCulture), b_ldu.ToString("F5", CultureInfo.InvariantCulture));
                }

                ISweep_M_Trend1[HGAIndex] = LDUSlope;// Math.Round(LDUSlope, 4, MidpointRounding.ToEven);
                ISweep_C_Trend1[HGAIndex] = b_ldu;// Math.Round(b_ldu, 4, MidpointRounding.ToEven);

                if (!updategraph)
                    return;
                chartLDU.Series[2].Points.Clear();
                chartLDU.Series[3].Points.Clear();
                temp_3 = 100000;
                temp_1 = 0;
                for (double i = lduStartCurrentLimit; i <= lduStopCurrentLimit; i += LDUXPointSize)
                {

                    if (k == LDUpoints)
                        break;

                    DataPoint d_LDU = new DataPoint(i, LDUPhotodiodeVoltagePoint[HGAIndex, k]);
                    d_LDU.ToolTip = "#VALY";

                    LDUVoltageLinearFitPoint[k] = (LDUSlope * i) + b_ldu;

                    DataPoint d_LDULFit = new DataPoint(i, LDUVoltageLinearFitPoint[k]);
                    d_LDULFit.ToolTip = "#VALY";


                    chartLDU.Series[2].Points.Add(d_LDU);
                    chartLDU.Series[3].Points.Add(d_LDULFit);

                    if (LDUPhotodiodeVoltagePoint[HGAIndex, k] > temp_1)
                        temp_1 = LDUPhotodiodeVoltagePoint[HGAIndex, k];

                    if (temp_3 > LDUPhotodiodeVoltagePoint[HGAIndex, k])
                        temp_3 = LDUPhotodiodeVoltagePoint[HGAIndex, k];

                    k++;
                }

                this.chartLDU.ChartAreas[0].AxisX.Interval = LDUXPointSize;
                this.chartLDU.ChartAreas[0].AxisX.Minimum = (double)(lduStartCurrentLimit);
                this.chartLDU.ChartAreas[0].AxisX.Maximum = (double)(lduStopCurrentLimit + LDUXPointSize);
                this.chartLDU.ChartAreas[0].AxisY.Interval = 0.5;//(double)(lduStopCurrentLimit + LDUXPointSize) / LDUpoints;
                this.chartLDU.ChartAreas[0].AxisY.Minimum = 0; // temp_3 - chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, 0] * 0.5;//(0.5 * LDUVoltagePoint[HGAIndex, 0]);
                this.chartLDU.ChartAreas[0].AxisY.Maximum = 5;//temp_1 + chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, Ypoints - 1] + 0.2;

            }

            catch
            {

            }

        }

        public void UpdateFullSweepPhotoDiodeGraph_line2(int HGAIndex, bool updategraph)
        {

            double temp_1 = 0;
            double temp_2 = 0;
            double temp_3 = 0;
            double temp_4 = 0;
            double temp_5 = 0;
            double temp_6 = 0;

            double YBar_LDUVoltageAverage = 0;
            double XBar_LDUCurrentAverage = 0;


            int LDUpoints = 0;


            double LDUSlope = 0;

            double b_ldu = 0;


            int LDUtotalPoints = 8 - int.Parse(textBoxT2StartPoint.Text);

            double LDUXPointSize; //=1;//size;
            double lduStartCurrentLimit; //= 0; //start; // starting point
            double lduStopCurrentLimit; //= 20;// stop; //end point

            double result; //=1;//size;
            int startpoint = 0;

            Double.TryParse(TextboxLDUStepCurrent.Text, out result);
            LDUXPointSize = result / 1000;
            Double.TryParse(textBoxT2StartPoint.Text, out result); // the starting point of the sampling
            startpoint = (int)result;
            Double.TryParse(TextboxLDUStartCurrent.Text, out result);
            var strpnt = result / 1000;

         //   lduStartCurrentLimit = LDUXPointSize  * startpoint; // in mAa
            Double.TryParse(TextboxLDUStopCurrent.Text, out result);
            lduStopCurrentLimit = result / 1000;// LDUXPointSize * (LDUtotalPoints - 1);

            lduStartCurrentLimit = strpnt + LDUXPointSize * startpoint; // in mAa

            double[] XBarLDU = new double[LDUtotalPoints];
            double[] YBarLDU = new double[LDUtotalPoints];

            double[] LDUVoltageLinearFitPoint = new double[LDUtotalPoints];// new double[totalPoints];

            int k = 0;

            try
            {
                // calculate x and y bar
                for (int i = 0; i < LDUtotalPoints; i++)
                {
                    if (LDUPhotodiodeVoltagePoint[HGAIndex, i+ startpoint] != 0)
                    {
                        temp_1 += LDUPhotodiodeVoltagePoint[HGAIndex, i + startpoint];
                        temp_3 += lduStartCurrentLimit + (i * LDUXPointSize);


                        LDUpoints++;
                    }


                }
                YBar_LDUVoltageAverage = temp_1 / LDUpoints;
                XBar_LDUCurrentAverage = temp_3 / LDUpoints;

                if (temp_1 == 0 && temp_3 == 0)
                {
                    YBar_LDUVoltageAverage = 0;
                    XBar_LDUCurrentAverage = 0;

                }
                for (int i = 0; i < LDUpoints; i++)
                {
                    YBarLDU[i] = LDUPhotodiodeVoltagePoint[HGAIndex, i + startpoint] - YBar_LDUVoltageAverage;//(Y-YBar)
                    XBarLDU[i] = (lduStartCurrentLimit + (i * LDUXPointSize)) - XBar_LDUCurrentAverage; // (X -XBar)

                    temp_4 += XBarLDU[i] * YBarLDU[i]; //(X -XBar) * (Y-YBar) = LDU                    
                    temp_6 += XBarLDU[i] * XBarLDU[i]; // (X -XBar)^2

                    LDUMaxVPD[HGAIndex] = LDUPhotodiodeVoltagePoint[HGAIndex, i + startpoint];
                }
                //calculate M
                LDUSlope = (temp_4 / temp_6); // m -->(X -XBar) * (Y-YBar)/(X -XBar)^2


                if (temp_4 == 0 && temp_6 == 0)
                {
                    LDUSlope = 0;
                }
                if (Math.Abs(LDUSlope) < 0.001)
                    LDUSlope = 0;
                b_ldu = YBar_LDUVoltageAverage - (LDUSlope * XBar_LDUCurrentAverage);

                if (this.textBoxITrendLine2.InvokeRequired)
                {
                    textBoxITrendLine2.Invoke(new MethodInvoker(() =>
                    {
                        textBoxITrendLine2.Text = String.Format("y={0} X + {1}", (LDUSlope).ToString("F5", CultureInfo.InvariantCulture), b_ldu.ToString("F5", CultureInfo.InvariantCulture));
                    }));
                }
                else
                {
                    textBoxITrendLine2.Text = String.Format("y={0} X + {1}", (LDUSlope).ToString("F5", CultureInfo.InvariantCulture), b_ldu.ToString("F5", CultureInfo.InvariantCulture));
                }

                ISweep_M_Trend2[HGAIndex] = LDUSlope;// Math.Round(LDUSlope, 4);
                ISweep_C_Trend2[HGAIndex] = b_ldu;// Math.Round(b_ldu, 4);

                temp_3 = 100000;
                temp_1 = 0;
                if (!updategraph)
                    return;
                chartLDU.Series[4].Points.Clear();
                chartLDU.Series[5].Points.Clear();

                for (double i = lduStartCurrentLimit; i <= lduStopCurrentLimit; i += LDUXPointSize)
                {

                    if (k == LDUpoints)
                        break;

                    DataPoint d_LDU = new DataPoint(i, LDUPhotodiodeVoltagePoint[HGAIndex, k + startpoint]);
                    d_LDU.ToolTip = "#VALY";

                    LDUVoltageLinearFitPoint[k] = (LDUSlope * i) + b_ldu;

                    DataPoint d_LDULFit = new DataPoint(i, LDUVoltageLinearFitPoint[k]);
                    d_LDULFit.ToolTip = "#VALY";


                    chartLDU.Series[4].Points.Add(d_LDU);
                    chartLDU.Series[5].Points.Add(d_LDULFit);

                    if (LDUPhotodiodeVoltagePoint[HGAIndex, k + startpoint] > temp_1)
                        temp_1 = LDUPhotodiodeVoltagePoint[HGAIndex, k + startpoint];

                    if (temp_3 > LDUPhotodiodeVoltagePoint[HGAIndex, k + startpoint])
                        temp_3 = LDUPhotodiodeVoltagePoint[HGAIndex, k + startpoint];

                    k++;
                }

                this.chartLDU.ChartAreas[0].AxisX.Interval = LDUXPointSize;
                this.chartLDU.ChartAreas[0].AxisX.Minimum = (double)(lduStartCurrentLimit);
                this.chartLDU.ChartAreas[0].AxisX.Maximum = (double)(lduStopCurrentLimit + LDUXPointSize);
                this.chartLDU.ChartAreas[0].AxisY.Interval = 0.5;//(double)(lduStopCurrentLimit + LDUXPointSize) / LDUpoints;
                this.chartLDU.ChartAreas[0].AxisY.Minimum = 0; //temp_3 - chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, 0] * 0.5;//(0.5 * LDUVoltagePoint[HGAIndex, 0]);
                this.chartLDU.ChartAreas[0].AxisY.Maximum = 5;// temp_1 + chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, Ypoints - 1] + 0.2;

            }

            catch
            {

            }
        }

        private void timerRefreshPage_Tick(object sender, EventArgs e)
        {
            if(timercount < 5)            
                this.panelLDUMeasurement_Load(null, null);
            else
                timerRefreshPage.Stop();
            timercount++;
        }

        public void UpdateSetting()
        {
            timercount = 0;
            timerRefreshPage.Start();
        }

        public void UpdateCalculationParameter()
        {
            this.panelLDUMeasurement_Load(null, null);
        }

    /*    public void UpdateLDUGraphAndCalculateUsingLastFewPoints(int HGAIndex, bool no_graph)
        {

         //   chartLDU.Series[0].Points.Clear();
         //   chartLDU.Series[1].Points.Clear();
            LDU_M[HGAIndex] = 0;
            LDU_C[HGAIndex] = 0;


            double temp_1 = 0;
            double temp_2 = 0;
            double temp_3 = 0;
            double temp_4 = 0;
            double temp_5 = 0;
            double temp_6 = 0;

            double YBar_LDUVoltageAverage = 0;
            double XBar_LDUCurrentAverage = 0;


            int LDUpoints = 0;


            double LDUSlope = 0;

            double b_ldu = 0;


            int LDUtotalPoints = 8;


            // double[] data = { 1, 2, 2, 2, 2, 2, 7, 5, 6, 4 };


            double LDUXPointSize; //=1;//size;
            double lduStartCurrentLimit; //= 0; //start; // starting point
            double lduStopCurrentLimit; //= 20;// stop; //end point

            double result; //=1;//size;

            Double.TryParse(TextboxLDUStepCurrent.Text, out result);
            LDUXPointSize = result / 1000;
            Double.TryParse(TextboxLDUStartCurrent.Text, out result);
            lduStartCurrentLimit = result / 1000;
            Double.TryParse(TextboxLDUStopCurrent.Text, out result);
            lduStopCurrentLimit = result / 1000;


            double[] XBarLDU = new double[LDUtotalPoints];
            double[] YBarLDU = new double[LDUtotalPoints];
            double[] LDUVoltageLinearFitPoint = new double[LDUtotalPoints];// new double[totalPoints];

            int k = 0;
            int startingpoint = 0;
            int actual_points = 0;
            //count the actual point
            for (int i = 0; i < LDUtotalPoints; i++)
            {
                if (LDUVoltagePoint[HGAIndex, i] != 0)
                {
                    actual_points++;
                }
            }
            startingpoint = 0;//actual_points - 5;
            try
            {
                // calculate x and y bar
                for (int i = startingpoint; i < actual_points; i++)
                {
                    if (LDUVoltagePoint[HGAIndex, i] != 0)
                    {
                        temp_1 += LDUVoltagePoint[HGAIndex, i];
                        temp_3 += lduStartCurrentLimit + (i * LDUXPointSize);


                        LDUpoints++;
                    }


                }
                YBar_LDUVoltageAverage = temp_1 / LDUpoints;
                XBar_LDUCurrentAverage = temp_3 / LDUpoints;

                if (temp_1 == 0 && temp_3 == 0)
                {
                    YBar_LDUVoltageAverage = 0;
                    XBar_LDUCurrentAverage = 0;

                }
                for (int i = 0; i < LDUpoints; i++)
                {
                    YBarLDU[i] = LDUVoltagePoint[HGAIndex, i + startingpoint] - YBar_LDUVoltageAverage;//(Y-YBar)
                    XBarLDU[i] = (lduStartCurrentLimit + (i * LDUXPointSize)) - XBar_LDUCurrentAverage; // (X -XBar)

                    temp_4 += XBarLDU[i] * YBarLDU[i]; //(X -XBar) * (Y-YBar) = LDU                    
                    temp_6 += XBarLDU[i] * XBarLDU[i]; // (X -XBar)^2
                }
                //calculate M
                LDUSlope = (temp_4 / temp_6); // m -->(X -XBar) * (Y-YBar)/(X -XBar)^2


                if (temp_4 == 0 && temp_6 == 0)
                {
                    LDUSlope = 0;
                }

                b_ldu = YBar_LDUVoltageAverage - (LDUSlope * XBar_LDUCurrentAverage);
                textBoxITrendLineLDU.Text = String.Format("y={0} X + {1}", (LDUSlope).ToString("F5", CultureInfo.InvariantCulture), b_ldu.ToString("F5", CultureInfo.InvariantCulture));

                LDU_M[HGAIndex] = LDUSlope;//Math.Round(LDUSlope, 4, MidpointRounding.ToEven);
                LDU_C[HGAIndex] = b_ldu;// Math.Round(b_ldu, 4, MidpointRounding.ToEven);




                if (!no_graph)
                {
                    chartLDU.Series[0].Points.Clear();
                    chartLDU.Series[1].Points.Clear();
                    temp_3 = 100000;
                    temp_1 = 0;
                    for (double i = lduStartCurrentLimit; i <= lduStopCurrentLimit; i += LDUXPointSize)
                    {

                        if (k == actual_points)
                            break;

                        DataPoint d_LDU = new DataPoint(i, LDUVoltagePoint[HGAIndex, k]);
                        d_LDU.ToolTip = "#VALY";

                        LDUVoltageLinearFitPoint[k] = (LDUSlope * i) + b_ldu;

                        DataPoint d_LDULFit = new DataPoint(i, LDUVoltageLinearFitPoint[k]);
                        d_LDULFit.ToolTip = "#VALY";


                        chartLDU.Series[0].Points.Add(d_LDU);
                        chartLDU.Series[1].Points.Add(d_LDULFit);

                        if (LDUVoltagePoint[HGAIndex, k] > temp_1)
                            temp_1 = LDUVoltagePoint[HGAIndex, k];

                        if (temp_3 > LDUVoltagePoint[HGAIndex, k])
                            temp_3 = LDUVoltagePoint[HGAIndex, k];

                        k++;
                    }

                    this.chartLDU.ChartAreas[0].AxisX.Interval = LDUXPointSize;
                    this.chartLDU.ChartAreas[0].AxisX.Minimum = (double)(lduStartCurrentLimit);
                    this.chartLDU.ChartAreas[0].AxisX.Maximum = (double)(lduStopCurrentLimit + LDUXPointSize);
                    this.chartLDU.ChartAreas[0].AxisY.Interval = 0.5;//(double)(lduStopCurrentLimit + LDUXPointSize) / LDUpoints;
                    this.chartLDU.ChartAreas[0].AxisY.Minimum = 0;// temp_3 - chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, 0] * 0.5;//(0.5 * LDUVoltagePoint[HGAIndex, 0]);
                    this.chartLDU.ChartAreas[0].AxisY.Maximum = 5;// 10;//temp_1 + chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, Ypoints - 1] + 0.2;
                }
            }

            catch
            {

            }

        }

*/
        public void UpdateFullSweepPhotoDiodeGraph_line1(int HGAIndex, int StopPoint, bool updateGraph)
        {
          //  chartLDU.Series[2].Points.Clear();
          //  chartLDU.Series[3].Points.Clear();


            double temp_1 = 0;
            double temp_2 = 0;
            double temp_3 = 0;
            double temp_4 = 0;
            double temp_5 = 0;
            double temp_6 = 0;

            double YBar_LDUVoltageAverage = 0;
            double XBar_LDUCurrentAverage = 0;


            int LDUpoints = 0;


            double LDUSlope = 0;

            double b_ldu = 0;


            int LDUtotalPoints = StopPoint + 1;//int.Parse(textBoxT1StopPoint.Text) + 1;

            double LDUXPointSize; //=1;//size;
            double lduStartCurrentLimit; //= 0; //start; // starting point
            double lduStopCurrentLimit; //= 20;// stop; //end point

            double result; //=1;//size;

            Double.TryParse(TextboxLDUStepCurrent.Text, out result);
            LDUXPointSize = result / 1000;
            Double.TryParse(TextboxLDUStartCurrent.Text, out result);
            lduStartCurrentLimit = result / 1000;
            // Double.TryParse(TextboxLDUStopCurrent.Text, out result);
            lduStopCurrentLimit = lduStartCurrentLimit + (LDUXPointSize * (LDUtotalPoints - 1));


            double[] XBarLDU = new double[LDUtotalPoints];
            double[] YBarLDU = new double[LDUtotalPoints];

            double[] LDUVoltageLinearFitPoint = new double[LDUtotalPoints];// new double[totalPoints];

            int k = 0;

            try
            {
                // calculate x and y bar
                for (int i = 0; i < LDUtotalPoints; i++)
                {
                    if (LDUPhotodiodeVoltagePoint[HGAIndex, i] != 0)
                    {
                        temp_1 += LDUPhotodiodeVoltagePoint[HGAIndex, i];
                        temp_3 += lduStartCurrentLimit + (i * LDUXPointSize);


                        LDUpoints++;
                    }


                }
                YBar_LDUVoltageAverage = temp_1 / LDUpoints;
                XBar_LDUCurrentAverage = temp_3 / LDUpoints;

                if (temp_1 == 0 && temp_3 == 0)
                {
                    YBar_LDUVoltageAverage = 0;
                    XBar_LDUCurrentAverage = 0;

                }
                for (int i = 0; i < LDUpoints; i++)
                {
                    YBarLDU[i] = LDUPhotodiodeVoltagePoint[HGAIndex, i] - YBar_LDUVoltageAverage;//(Y-YBar)
                    XBarLDU[i] = (lduStartCurrentLimit + (i * LDUXPointSize)) - XBar_LDUCurrentAverage; // (X -XBar)

                    temp_4 += XBarLDU[i] * YBarLDU[i]; //(X -XBar) * (Y-YBar) = LDU                    
                    temp_6 += XBarLDU[i] * XBarLDU[i]; // (X -XBar)^2
                }
                //calculate M
                LDUSlope = (temp_4 / temp_6); // m -->(X -XBar) * (Y-YBar)/(X -XBar)^2


                if (temp_4 == 0 && temp_6 == 0)
                {
                    LDUSlope = 0;
                }
                if (Math.Abs(LDUSlope) < 0.001)
                    LDUSlope = 0;
                b_ldu = YBar_LDUVoltageAverage - (LDUSlope * XBar_LDUCurrentAverage);
                textBoxITrendLine1.Text = String.Format("y={0} X + {1}", (LDUSlope).ToString("F5", CultureInfo.InvariantCulture), b_ldu.ToString("F5", CultureInfo.InvariantCulture));
                //textBoxRLDU.Text = LDUSlope.ToString("F3", CultureInfo.InvariantCulture);
                //  labelLDU_Y_Intercept.Text = String.Format("Y-Intercept = {0}", b_ldu.ToString("F3", CultureInfo.InvariantCulture));

                ISweep_M_Trend1[HGAIndex] = LDUSlope;// Math.Round(LDUSlope, 4, MidpointRounding.ToEven);
                ISweep_C_Trend1[HGAIndex] = b_ldu;// Math.Round(b_ldu, 4, MidpointRounding.ToEven);

                if (updateGraph)
                {
                    chartLDU.Series[2].Points.Clear();
                    chartLDU.Series[3].Points.Clear();
                    temp_3 = 100000;
                    temp_1 = 0;
                    for (double i = lduStartCurrentLimit; i <= lduStopCurrentLimit; i += LDUXPointSize)
                    {

                        if (k == LDUpoints)
                            break;

                        DataPoint d_LDU = new DataPoint(i, LDUPhotodiodeVoltagePoint[HGAIndex, k]);
                        d_LDU.ToolTip = "#VALY";

                        LDUVoltageLinearFitPoint[k] = (LDUSlope * i) + b_ldu;

                        DataPoint d_LDULFit = new DataPoint(i, LDUVoltageLinearFitPoint[k]);
                        d_LDULFit.ToolTip = "#VALY";


                        chartLDU.Series[2].Points.Add(d_LDU);
                        chartLDU.Series[3].Points.Add(d_LDULFit);

                        if (LDUPhotodiodeVoltagePoint[HGAIndex, k] > temp_1)
                            temp_1 = LDUPhotodiodeVoltagePoint[HGAIndex, k];

                        if (temp_3 > LDUPhotodiodeVoltagePoint[HGAIndex, k])
                            temp_3 = LDUPhotodiodeVoltagePoint[HGAIndex, k];

                        k++;
                    }

                    this.chartLDU.ChartAreas[0].AxisX.Interval = LDUXPointSize;
                    this.chartLDU.ChartAreas[0].AxisX.Minimum = (double)(lduStartCurrentLimit);
                    this.chartLDU.ChartAreas[0].AxisX.Maximum = (double)(lduStopCurrentLimit + LDUXPointSize);
                    this.chartLDU.ChartAreas[0].AxisY.Interval = 0.5;//(double)(lduStopCurrentLimit + LDUXPointSize) / LDUpoints;
                    this.chartLDU.ChartAreas[0].AxisY.Minimum = 0;//temp_3 - chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, 0] * 0.5;//(0.5 * LDUVoltagePoint[HGAIndex, 0]);
                    this.chartLDU.ChartAreas[0].AxisY.Maximum = 5;// temp_1 + chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, Ypoints - 1] + 0.2;
                }
            }

            catch
            {

            }

        }

        public void UpdateFullSweepPhotoDiodeGraph_line2(int HGAIndex, int StartPoint, bool updateGraph)
        {
         //   chartLDU.Series[4].Points.Clear();
         //   chartLDU.Series[5].Points.Clear();



            double temp_1 = 0;
            double temp_2 = 0;
            double temp_3 = 0;
            double temp_4 = 0;
            double temp_5 = 0;
            double temp_6 = 0;

            double YBar_LDUVoltageAverage = 0;
            double XBar_LDUCurrentAverage = 0;


            int LDUpoints = 0;


            double LDUSlope = 0;

            double b_ldu = 0;


            int LDUtotalPoints = 8 - StartPoint;//int.Parse(textBoxT2StartPoint.Text);

            double LDUXPointSize; //=1;//size;
            double lduStartCurrentLimit; //= 0; //start; // starting point
            double lduStopCurrentLimit; //= 20;// stop; //end point

            double result; //=1;//size;
            int startpoint = 0;

            Double.TryParse(TextboxLDUStepCurrent.Text, out result);
            LDUXPointSize = result / 1000;
            //Double.TryParse(textBoxT2StartPoint.Text, out result); // the starting point of the sampling
            startpoint = StartPoint;//(int)result;
            Double.TryParse(TextboxLDUStartCurrent.Text, out result);
            var strpnt = result / 1000;

            //   lduStartCurrentLimit = LDUXPointSize  * startpoint; // in mAa
            Double.TryParse(TextboxLDUStopCurrent.Text, out result);
            lduStopCurrentLimit = result / 1000;// LDUXPointSize * (LDUtotalPoints - 1);

            lduStartCurrentLimit = strpnt + LDUXPointSize * startpoint; // in mAa

            double[] XBarLDU = new double[LDUtotalPoints];
            double[] YBarLDU = new double[LDUtotalPoints];

            double[] LDUVoltageLinearFitPoint = new double[LDUtotalPoints];// new double[totalPoints];

            int k = 0;

            try
            {
                // calculate x and y bar
                for (int i = 0; i < LDUtotalPoints; i++)
                {
                    if (LDUPhotodiodeVoltagePoint[HGAIndex, i + startpoint] != 0)
                    {
                        temp_1 += LDUPhotodiodeVoltagePoint[HGAIndex, i + startpoint];
                        temp_3 += lduStartCurrentLimit + (i * LDUXPointSize);


                        LDUpoints++;
                    }


                }
                YBar_LDUVoltageAverage = temp_1 / LDUpoints;
                XBar_LDUCurrentAverage = temp_3 / LDUpoints;

                if (temp_1 == 0 && temp_3 == 0)
                {
                    YBar_LDUVoltageAverage = 0;
                    XBar_LDUCurrentAverage = 0;

                }
                for (int i = 0; i < LDUpoints; i++)
                {
                    YBarLDU[i] = LDUPhotodiodeVoltagePoint[HGAIndex, i + startpoint] - YBar_LDUVoltageAverage;//(Y-YBar)
                    XBarLDU[i] = (lduStartCurrentLimit + (i * LDUXPointSize)) - XBar_LDUCurrentAverage; // (X -XBar)

                    temp_4 += XBarLDU[i] * YBarLDU[i]; //(X -XBar) * (Y-YBar) = LDU                    
                    temp_6 += XBarLDU[i] * XBarLDU[i]; // (X -XBar)^2

                    LDUMaxVPD[HGAIndex] = LDUPhotodiodeVoltagePoint[HGAIndex, i + startpoint];
                }
                //calculate M
                LDUSlope = (temp_4 / temp_6); // m -->(X -XBar) * (Y-YBar)/(X -XBar)^2


                if (temp_4 == 0 && temp_6 == 0)
                {
                    LDUSlope = 0;
                }
                if (Math.Abs(LDUSlope) < 0.001)
                    LDUSlope = 0;
                b_ldu = YBar_LDUVoltageAverage - (LDUSlope * XBar_LDUCurrentAverage);
                textBoxITrendLine2.Text = String.Format("y={0} X + {1}", (LDUSlope).ToString("F5", CultureInfo.InvariantCulture), b_ldu.ToString("F5", CultureInfo.InvariantCulture));
                //   textBoxRLDU.Text = LDUSlope.ToString("F3", CultureInfo.InvariantCulture);
                //  labelLDU_Y_Intercept.Text = String.Format("Y-Intercept = {0}", b_ldu.ToString("F3", CultureInfo.InvariantCulture));

                ISweep_M_Trend2[HGAIndex] = LDUSlope;//Math.Round(LDUSlope, 4);
                ISweep_C_Trend2[HGAIndex] = b_ldu;// Math.Round(b_ldu, 4);

                if (updateGraph)
                {
                    chartLDU.Series[4].Points.Clear();
                    chartLDU.Series[5].Points.Clear();
                    temp_3 = 100000;
                    temp_1 = 0;
                    for (double i = lduStartCurrentLimit; i <= lduStopCurrentLimit; i += LDUXPointSize)
                    {

                        if (k == LDUpoints)
                            break;

                        DataPoint d_LDU = new DataPoint(i, LDUPhotodiodeVoltagePoint[HGAIndex, k + startpoint]);
                        d_LDU.ToolTip = "#VALY";

                        LDUVoltageLinearFitPoint[k] = (LDUSlope * i) + b_ldu;

                        DataPoint d_LDULFit = new DataPoint(i, LDUVoltageLinearFitPoint[k]);
                        d_LDULFit.ToolTip = "#VALY";


                        chartLDU.Series[4].Points.Add(d_LDU);
                        chartLDU.Series[5].Points.Add(d_LDULFit);

                        if (LDUPhotodiodeVoltagePoint[HGAIndex, k + startpoint] > temp_1)
                            temp_1 = LDUPhotodiodeVoltagePoint[HGAIndex, k + startpoint];

                        if (temp_3 > LDUPhotodiodeVoltagePoint[HGAIndex, k + startpoint])
                            temp_3 = LDUPhotodiodeVoltagePoint[HGAIndex, k + startpoint];

                        k++;
                    }

                    this.chartLDU.ChartAreas[0].AxisX.Interval = LDUXPointSize;
                    this.chartLDU.ChartAreas[0].AxisX.Minimum = (double)(lduStartCurrentLimit);
                    this.chartLDU.ChartAreas[0].AxisX.Maximum = (double)(lduStopCurrentLimit + LDUXPointSize);
                    this.chartLDU.ChartAreas[0].AxisY.Interval = 0.5;//(double)(lduStopCurrentLimit + LDUXPointSize) / LDUpoints;
                    this.chartLDU.ChartAreas[0].AxisY.Minimum = 0;// temp_3 - chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, 0] * 0.5;//(0.5 * LDUVoltagePoint[HGAIndex, 0]);
                    this.chartLDU.ChartAreas[0].AxisY.Maximum = 5;// temp_1 + chartLDU.ChartAreas[0].AxisY.Interval;//LDUVoltagePoint[HGAIndex, Ypoints - 1] + 0.2;
                }
            }

            catch
            {

            }
        }

    }
}
