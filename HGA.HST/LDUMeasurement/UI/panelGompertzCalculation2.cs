using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using XyratexOSC.Logging;
using System.Globalization;
using System.Windows.Forms.DataVisualization.Charting;

namespace LDUMeasurement.UI
{
    public partial class panelGompertzCalculation2 : UserControl
    {
        private const bool SIMULATION = false;
        private const int GOMPERTZ_SAMPLE = 8;
        private const int MONITORING_SAMPLE = 10;
        private const int HGA = 10;
        private Stopwatch [] stopwatch = new Stopwatch[HGA];
        double[,] Ptime;
        int split;
        double[,] Ith;
        double Amin, Amax, Bmin, Bmax, Cmin, Cmax, Dmin, Dmax;

        private double[] current = new double[GOMPERTZ_SAMPLE];
        public double[,] InputVpd = new double[HGA, GOMPERTZ_SAMPLE];
        //define vpd , vpd best on each 8 points 
        private double[,] vpd = new double[HGA, GOMPERTZ_SAMPLE];
        private double[,] vpdbest = new double[HGA, GOMPERTZ_SAMPLE];

        double[,] xx ;
        double[,] yy ;
        double[,] yy1;
        double[,] yy2;
        private double step;
        private int totalstep;
        double[,] sse;
        int inter;
        //set seebest default 
        double[] ssebest = new double[HGA];//0;
        double []ssebest2 = new double[HGA];//2;
        double []abest2 = new double[HGA];//2;
        double[] bbest2 = new double[HGA];//2;
        double[] cbest2 = new double[HGA];//2;
        double[] dbest2 = new double[HGA];//2;
        public double[] I_threshold = new double[HGA];//2;
        int[] best2 = new int[HGA];
        private bool UseRandom;
        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;

       /*     object[] GompertzSetting = new object[11];
            GompertzSetting[0] = 10;
            GompertzSetting[1] = 0.1;           
            GompertzSetting[2] = 2;
            GompertzSetting[3] = 3;
            GompertzSetting[4] = 7;
            GompertzSetting[5] = 1;
            GompertzSetting[6] = 8;
            GompertzSetting[7] = 9;
            GompertzSetting[8] = 16;
            GompertzSetting[9] = "Random";
            GompertzSetting[10] = false;
           
            LoadSettings(GompertzSetting);*/
            SetCurrent(10100, 700, 15000);
            Initialize_Gompertz_Variables();
            for (int hga = 0; hga < HGA; hga++)
            {
                Gompertz_Calculation(hga);
            }
            this.button1.Enabled = true;
        }


        public void StartCalculate()
        {
            Initialize_Gompertz_Variables();
            for (int hga = 0; hga < HGA; hga++)
            {
                Gompertz_Calculation(hga);
            }

        }
        private void comboBoxHGA_SelectedIndexChanged(object sender, EventArgs e)
        {
            int hga = comboBoxHGA.SelectedIndex;
            PlotGraph(hga);
        }

        private void comboBoxCalculationMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            UseRandom = comboBoxCalculationMethod.SelectedIndex == 0 ? true : false;
        }

        public void Initialize_Gompertz_Variables()
        {
            //step = 0.001;
            //total interation loop calculation from split setting
            inter = Convert.ToInt32(Math.Pow(split, 4));
            sse = new double[HGA, inter];
            totalstep = Convert.ToInt32((current[7] - current[0]) / step + 1);

            xx = new double[HGA, totalstep];
            yy = new double[HGA, totalstep];
            yy1 = new double[HGA, totalstep];
            yy2 = new double[HGA, totalstep];

            for(int i = 0; i < HGA; i++)
            {
                ssebest[i] = 0;
                ssebest2[i] = 2;
                abest2[i] = 0;
                bbest2[i] = 0;
                cbest2[i] = 0;
                dbest2[i] = 0;
                I_threshold[i] = 0;
                best2[i] = 0;
                stopwatch[i] = new Stopwatch();
            }
            Array.Clear(Ith, 0, Ith.Length);
            Array.Clear(Ptime, 0, Ptime.Length);
            Array.Clear(sse, 0, sse.Length);
            Array.Clear(xx, 0, xx.Length);
            Array.Clear(yy, 0, yy.Length);
            Array.Clear(yy1, 0, yy1.Length);
            Array.Clear(yy2, 0, yy2.Length);
        }
        public panelGompertzCalculation2()
        {
            InitializeComponent();

            textBox_abest.Enabled = false;
            textBox_amin.Enabled = false;
            textBox_amax.Enabled = false;
            textBox_bmin.Enabled = false;
            textBox_bmax.Enabled = false;
            textBox_cmin.Enabled = false;
            textBox_cmax.Enabled = false;
            textBox_dmin.Enabled = false;
            textBox_dmax.Enabled = false;
            textBox_Ithreshold.Enabled = false;
            textBox_max_delta_Ithreshold.Enabled = false;
            textBox_max_processing_time.Enabled = false;
            textBox_split.Enabled = false;
            textBox_sse.Enabled = false;
            textBox_stopwatch.Enabled = false;
            textBox_abest.Enabled = false;
            textBox_bbest.Enabled = false;
            textBox_cbest.Enabled = false;
            textBox_dbest.Enabled = false;
            comboBoxCalculationMethod.Enabled = false;
            textBox_best.Enabled = false;
            Ptime = new double[HGA, MONITORING_SAMPLE];
            Ith = new double[HGA, MONITORING_SAMPLE];

            

            if(SIMULATION)
            {
                //set fix value x for this model only
                //on HST , we will use current 8 points from HST measurement 
                current[0] = 10.1;
                current[1] = 10.8;
                current[2] = 11.5;
                current[3] = 12.2;
                current[4] = 12.9;
                current[5] = 13.6;
                current[6] = 14.3;
                current[7] = 15;

                //set fix value y for this model only 
                //on HST , we will use V-PD output 8 points from HST measurement 

                //HGA1 
            /*    InputVpd[0,0] = 0.248;
                InputVpd[0, 1] = 0.269;
                InputVpd[0, 2] = 0.294;
                InputVpd[0, 3]= 0.327;
                InputVpd[0, 4] = 0.396;
                InputVpd[0, 5] = 1.343;
                InputVpd[0, 6] = 2.58;
                InputVpd[0, 7]= 3.731;

                //HGA2 
                InputVpd[1,0] = 0.246;
                InputVpd[1, 1] = 0.269;
                InputVpd[1, 2] = 0.298;
                InputVpd[1, 3]= 0.359;
                InputVpd[1, 4] = 1.366;
                InputVpd[1, 5] = 2.46;
                InputVpd[1, 6] = 3.595;
                InputVpd[1, 7]= 4.511;


                //HGA3 
                InputVpd[2,0] = 0.251;
                InputVpd[2, 1] = 0.272;
                InputVpd[2, 2] = 0.296;
                InputVpd[2, 3]= 0.33;
                InputVpd[2, 4] = 0.488;
                InputVpd[2, 5] = 1.581;
                InputVpd[2, 6] = 2.595;
                InputVpd[2, 7]= 3.669;

                //HGA4 
                InputVpd[3,0] = 0.23;
                InputVpd[3, 1] = 0.249;
                InputVpd[3, 2] = 0.273;
                InputVpd[3, 3]= 0.308;
                InputVpd[3, 4] = 0.774;
                InputVpd[3, 5] = 1.806;
                InputVpd[3, 6] = 2.774;
                InputVpd[3, 7]= 3.666; 
*/
                //HGA5
                InputVpd[0,0] = 0.233;
                InputVpd[0,1] = 0.252;
                InputVpd[0,2] = 0.277;
                InputVpd[0,3] = 0.328;
                InputVpd[0,4] = 1.221;
                InputVpd[0,5] = 2.338;
                InputVpd[0,6] = 3.35;
                InputVpd[0,7] = 4.289;

            }
            else
            { button1.Visible = false; }
        }


        public void Gompertz_Calculation(int hga)
        {
            stopwatch[hga].Start();
          
           

            for (int q = 0; q < MONITORING_SAMPLE; q++)
            {

                //define random function 	
                var rand = new System.Random();

                //set a range search in all probability 
                double amin = Amin;
                double amax = Amax;
                double apart = (amax - amin) / split;
                double[] aused = new double[split + 1];

                for (int a11 = 0; a11 < split + 1; a11++)
                {
                    aused[0] = amin;
                    aused[split] = amax;
                    if (a11 != 0 && a11 != split)
                    {
                        aused[a11] = amin + apart * a11;
                    }
                }

                //set b range search in all probability 
                double bmin = Bmin;
                double bmax = Bmax;
                double bpart = (bmax - bmin) / split;
                double[] bused = new double[split + 1];

                for (int b11 = 0; b11 < split + 1; b11++)
                {
                    bused[0] = bmin;
                    bused[split] = bmax;
                    if (b11 != 0 && b11 != split + 1)
                    {
                        bused[b11] = bmin + bpart * b11;
                    }
                }

                //set c range search in all probability 			
                double cmin = Cmin;
                double cmax = Cmax;
                double cpart = (cmax - cmin) / split;
                double[] cused = new double[split + 1];

                for (int c11 = 0; c11 < split + 1; c11++)
                {
                    cused[0] = cmin;
                    cused[split] = cmax;
                    if (c11 != 0 && c11 != split)
                    {
                        cused[c11] = cmin + cpart * c11;
                    }
                }


                //set d range search in all probability 			
                double dmin = Dmin;
                double dmax = Dmax;
                double dpart = (dmax - dmin) / split;
                double[] dused = new double[split + 1];
                for (int d11 = 0; d11 < split + 1; d11++)
                {
                    dused[0] = dmin;
                    dused[split] = dmax;
                    if (d11 != 0 && d11 != split)
                    {
                        dused[d11] = dmin + dpart * d11;
                    }
                }


               // double abest2 = 0, bbest2 = 0, cbest2 = 0, dbest2 = 0;

              

                //define a,b,c,d array for all possibility test 
                double[] a = new double[inter];
                double[] b = new double[inter];
                double[] c = new double[inter];
                double[] d = new double[inter];

                //define sse for all possibility test 
                

               

                //set seebest default 
               // double ssebest = 0, ssebest2 = 2;
                //set best2 position value 
               // int best2 = 0;                             
                // start searching in all  possibility  
                int ii = 0;

                while (ii < inter)
                {

                    for (int a1 = 0; a1 < split; a1++)
                    {

                        for (int b1 = 0; b1 < split; b1++)
                        {

                            for (int c1 = 0; c1 < split; c1++)
                            {

                                for (int d1 = 0; d1 < split; d1++)
                                {
                                    sse[hga, ii] = 0;
                                    //assign a,b,c,d value from random all possibility range
                                    if (UseRandom) //Random
                                    {
                                        a[ii] = rand.NextDoubleRange(aused[a1], aused[a1 + 1]);
                                        b[ii] = rand.NextDoubleRange(bused[b1], bused[b1 + 1]);
                                        c[ii] = rand.NextDoubleRange(cused[c1], cused[c1 + 1]);
                                        d[ii] = rand.NextDoubleRange(dused[d1], dused[d1 + 1]);
                                    }
                                    else
                                    {
                                        a[ii] = (aused[a1] + aused[a1 + 1]) / 2;
                                        b[ii] = (bused[b1] + bused[b1 + 1]) / 2;
                                        c[ii] = (cused[c1] + cused[c1 + 1]) / 2;
                                        d[ii] = (dused[d1] + dused[d1 + 1]) / 2;
                                    }
                                    // vpd and sse calculation 
                                    for (int i1 = 0; i1 < 8; i1++)
                                    {
                                        
                                        vpd[hga, i1] = a[ii] + (b[ii] - a[ii]) * Math.Exp(-1 * Math.Exp(-1 * c[ii] * (current[i1] - d[ii])));

                                        // objective function : minimum least square error 
                                        sse[hga, ii] = sse[hga, ii] + Math.Pow((InputVpd[hga,i1] - vpd[hga, i1]), 2);

                                    }

                                    //assign the minimum sse for the best point output without sse targer to control processing time
                                    // this concept is for finding the best value as program can do only 
                                    if (sse[hga, ii] < ssebest2[hga])
                                    {
                                        ssebest2[hga] = sse[hga, ii];
                                        best2[hga] = ii;
                                        abest2[hga] = a[ii];
                                        bbest2[hga] = b[ii];
                                        cbest2[hga] = c[ii];
                                        dbest2[hga] = d[ii];

                                    }
                              
                                    ii++;

                                }

                            }

                        }

                    }

                }
                // end searching in all  possibility  

                //calculate the vpdbest & ssebest from best point for checking only 
                for (int i3 = 0; i3 < 8; i3++)
                {

                    vpdbest[hga, i3] = abest2[hga] + (bbest2[hga] - abest2[hga]) * Math.Exp(-1 * Math.Exp(-1 * cbest2[hga] * (current[i3] - dbest2[hga])));

                    // objective function : minimum least square error 
                    ssebest[hga] = ssebest[hga] + Math.Pow((InputVpd[hga,i3] - vpdbest[hga, i3]), 2);

                }


                // start 2nd derivative calculation for i-threshold

              //  double step = 0.001;
              //  int totalstep = Convert.ToInt32((current[7] - current[0]) / step + 1);
                //double[,] xx = new double[HGA, totalstep];
                //double[,] yy = new double[HGA, totalstep];
                //double[,] yy1 = new double[HGA, totalstep];
                //double[,] yy2 = new double[HGA, totalstep];
                yy1[hga, 0] = 0;
                yy2[hga, 0] = 0;
                yy2[hga, 1] = 0;
             //   double Ithreshold = 0;
                double maxIth = 0;

                for (int i4 = 0; i4 < totalstep; i4++)
                {

                    xx[hga, i4] = current[0] + step * i4;
                    yy[hga, i4] = abest2[hga] + (bbest2[hga] - abest2[hga]) * Math.Exp(-1 * Math.Exp(-1 * cbest2[hga] * (xx[hga, i4] - dbest2[hga])));

                    if (i4 > 0)
                    {
                        yy1[hga, i4] = (yy[hga, i4] - yy[hga, i4 - 1]) / step;
                    }
                    if (i4 > 1)
                    {
                        yy2[hga, i4] = (yy1[hga, i4] - yy1[hga, i4 - 1]) / step;
                    }

                    if (yy2[hga, i4] > maxIth)
                    {
                        maxIth = yy2[hga, i4];
                        I_threshold[hga] = xx[hga, i4];
                    //    Log.Info(this, "HGA{0}, Ithreshold = {1}", hga, Ithreshold[hga]);
                    }

                }
                // end 2nd derivative calculation for i-threshold


            
                //collect I-threshold from 10 time for monitoring model only 
                Ith[hga, q] = I_threshold[hga];
               // Log.Info(this, "--------- HGA{0}, Ithreshold = {1}", hga, Ith[hga, q]);
                //collect processing time from 10 time for monitoring model only 
                Ptime[hga, q] = Convert.ToDouble(stopwatch[hga].ElapsedMilliseconds);

            }


            stopwatch[hga].Stop();


        }



        public void LoadSettings(object[] GompertzSetting) //step1
        {
            textBox_split.Text = GompertzSetting[0].ToString();              
            textBox_amin.Text = GompertzSetting[1].ToString();
            textBox_amax.Text = GompertzSetting[2].ToString();
            textBox_bmin.Text = GompertzSetting[3].ToString();
            textBox_bmax.Text = GompertzSetting[4].ToString();
            textBox_cmin.Text = GompertzSetting[5].ToString();
            textBox_cmax.Text = GompertzSetting[6].ToString();
            textBox_dmin.Text = GompertzSetting[7].ToString();
            textBox_dmax.Text = GompertzSetting[8].ToString();
            comboBoxCalculationMethod.SelectedIndex = GompertzSetting[9].ToString() == "Random" ? 0 : 1;
            checkBoxUseGompertz.Checked = GompertzSetting[10].ToString() == "True" ? true : false;
            step = Convert.ToDouble(GompertzSetting[11]);
            UseRandom = comboBoxCalculationMethod.SelectedIndex == 0 ? true : false;

            split = int.Parse(textBox_split.Text);
            Amin = double.Parse(textBox_amin.Text);
            Amax = double.Parse(textBox_amax.Text);
            Bmin = double.Parse(textBox_bmin.Text);
            Bmax = double.Parse(textBox_bmax.Text);
            Cmin = double.Parse(textBox_cmin.Text);
            Cmax = double.Parse(textBox_cmax.Text);
            Dmin = double.Parse(textBox_dmin.Text);
            Dmax = double.Parse(textBox_dmax.Text);
            
        }

        public void SetCurrent(int start_I, int size_I, int stop_I) //step2
        {
            double _startCurrent = (double)start_I / 1000;
            double _stopCurrent = (double)stop_I / 1000;
            double _sizeCurrent = (double)size_I / 1000;
            for (int x = 0; x < GOMPERTZ_SAMPLE; x++)
            {
                current[x] = _startCurrent + (_sizeCurrent * x);

                if (current[x] == _stopCurrent || current[x] > _stopCurrent)
                {
                    current[x] = _stopCurrent;
                    Log.Info(this, "DerivativeCalculation Current {0}mA", current[x].ToString("F3", CultureInfo.InvariantCulture));
                    break;
                }
                Log.Info(this, "DerivativeCalculation Current {0}mA", current[x].ToString("F3", CultureInfo.InvariantCulture));
            }
        }

        public void PlotGraph(int hga)
        {
            chartIThreshold.Series[0].Points.Clear();

            chartProcessingTIme.Series[0].Points.Clear();

            chartVPD.Series[0].Points.Clear();
            chartVPD.Series[1].Points.Clear();

            chartSSE.Series[0].Points.Clear();

            chartGompertz.Series[0].Points.Clear();
            chartGompertz.Series[1].Points.Clear();
            chartGompertz.Series[2].Points.Clear();
            //plot output of VPD-current from HST vs VPD predict from model 
            double[] y = new double[GOMPERTZ_SAMPLE];
            double[] data_vpdbest = new double[GOMPERTZ_SAMPLE];
            y[0] = InputVpd[hga, 0];
            y[1] = InputVpd[hga, 1];
            y[2] = InputVpd[hga, 2];
            y[3] = InputVpd[hga, 3];
            y[4] = InputVpd[hga, 4];
            y[5] = InputVpd[hga, 5];
            y[6] = InputVpd[hga, 6];
            y[7] = InputVpd[hga, 7];

            data_vpdbest[0] = vpdbest[hga, 0];
            data_vpdbest[1] = vpdbest[hga, 1];
            data_vpdbest[2] = vpdbest[hga, 2];
            data_vpdbest[3] = vpdbest[hga, 3];
            data_vpdbest[4] = vpdbest[hga, 4];
            data_vpdbest[5] = vpdbest[hga, 5];
            data_vpdbest[6] = vpdbest[hga, 6];
            data_vpdbest[7] = vpdbest[hga, 7];

            chartVPD.Series[0].Points.DataBindXY(current, y);
            chartVPD.Series[1].Points.DataBindXY(current, data_vpdbest);
            chartVPD.ChartAreas[0].AxisY.IsStartedFromZero = false;

            //plot output of fit line plot function with 1st derivative and 2nd derivative 
            double[] _yy = new double[totalstep];
            double[] _yy1 = new double[totalstep];
            double[] _yy2 = new double[totalstep];
            double[] _xx = new double[totalstep];
            for(int i =0; i< totalstep; i++)
            {
                _yy[i] = yy[hga, i];
                _yy1[i] = yy1[hga, i];
                _yy2[i] = yy2[hga, i];
                _xx[i] = xx[hga, i];
            }
        //    Array.Copy(yy, 0, _yy, 0, totalstep);
        //    Array.Copy(yy1, hga * totalstep, _yy1, 0, _yy1.Length);
        //    Array.Copy(yy2, hga * totalstep, _yy2, 0, _yy2.Length);
        //    Array.Copy(xx, hga * totalstep, _xx, 0, _xx.Length);

            chartGompertz.Series[0].Points.DataBindXY(_xx, _yy);
            chartGompertz.Series[1].Points.DataBindXY(_xx, _yy1);
            chartGompertz.Series[2].Points.DataBindXY(_xx, _yy2);
            chartGompertz.ChartAreas[0].AxisY.IsStartedFromZero = false;

            //show results 
            textBox_sse.Text = Convert.ToString(ssebest2[hga]);
            textBox_abest.Text = Convert.ToString(abest2[hga]);
            textBox_bbest.Text = Convert.ToString(bbest2[hga]);
            textBox_cbest.Text = Convert.ToString(cbest2[hga]);
            textBox_dbest.Text = Convert.ToString(dbest2[hga]);
            textBox_Ithreshold.Text = Convert.ToString(I_threshold[hga]);
            textBox_best.Text = Convert.ToString(best2[hga]);

            //show processing time 
            try
            {
                textBox_stopwatch.Text = Convert.ToString(stopwatch[hga].ElapsedMilliseconds);
            }
            catch { }
            //plot sse 
            double[] _sse = new double[inter];
            for (int i = 0; i < inter; i++)
            {
                _sse[i] = sse[hga, i];
            }
            //    Array.Copy(sse, hga * inter, _sse, 0, _sse.Length);
            chartSSE.Series[0].Points.DataBindY(_sse);

            double[] _Ith = new double[10];
            //   Array.Copy(Ith, hga * 10, _Ith, 0, _Ith.Length);

            for (int i = 0; i < 10; i++)
            {
                _Ith[i] = Ith[hga, i];
            }
            chartIThreshold.Series[0].Points.DataBindY(_Ith);
            chartIThreshold.ChartAreas[0].AxisY.IsStartedFromZero = false;

            double[] _Ptime = new double[10];
            //  Array.Copy(Ptime, hga * 10, _Ptime, 0, _Ptime.Length);
            for (int i = 0; i < 10; i++)
            {
                _Ptime[i] = Ptime[hga, i];
            }
            chartProcessingTIme.Series[0].Points.DataBindY(_Ptime);
            chartProcessingTIme.ChartAreas[0].AxisY.IsStartedFromZero = false;

            textBox_max_delta_Ithreshold.Text = Convert.ToString(_Ith.Max() - _Ith.Min());

            textBox_max_processing_time.Text = Convert.ToString(_Ptime.Max());
         //   textBox_stopwatch.Text = Convert.ToString(stopwatch[hga].ElapsedMilliseconds);
            //collect I-threshold from 10 time for monitoring model only 
            // Ith[hga, q] = Ithreshold[hga];

            //collect processing time from 10 time for monitoring model only 
            // Ptime[hga, q] = Convert.ToDouble(stopwatch[hga].ElapsedMilliseconds);

        }


        public bool UseGompertzCalculation()
        {
            return checkBoxUseGompertz.Checked;
        }

    
    }

    public static class RandomExtensionMethods
    {
        public static double NextDoubleRange(this System.Random random, double minNumber, double maxNumber)
        {
            return random.NextDouble() * (maxNumber - minNumber) + minNumber;
        }
    }
}
