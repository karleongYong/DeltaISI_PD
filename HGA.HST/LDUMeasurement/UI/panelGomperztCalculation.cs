using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using XyratexOSC.Logging;
using System.Globalization;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;


namespace LDUMeasurement.UI
{
    public partial class panelGomperztCalculation : UserControl
    {
        private const bool SIMULATION = false;
        //     private const int SAMPLE = 8;
        private const int GOMPERTZ_SAMPLE = 8;
        private const int HGA = 10;

        private bool UseGompertz_Calculation;
        private Thread CalculationThread;
        public int Iteration;
        public double RSQ_Initial;
        public double SSE_Initial;
        public double[,] ABCDInitialValue = new double[4, 2]; // min, max
        public int Random_Scale;
        public double Weight;
        public int MaxTest;
        public int[] Max = new int[HGA];
        public double[] AdaptiveSearch = new double[4];
        public double Hard_Limit_RSQ;
        public double Hard_Limit_SSE;
        public double Max_Repeat_Case;

        public double[] Current;// = new double[GOMPERTZ_SAMPLE];
        public double[,] VPD;// = new double[HGA, GOMPERTZ_SAMPLE];
        public double[,] A_Result;
        public double[,] B_Result;
        public double[,] C_Result;
        public double[,] D_Result;

        public double[,] Gompertz;// = new double[HGA, GOMPERTZ_SAMPLE];
        public double[,] Last_Gompertz;// = new double[HGA, GOMPERTZ_SAMPLE];

        public double[,] YBAR;
        public double[,] SSR;
        public double[,] SST;
        public double[,] RSq;
        public double[,] SSE;

        public double[] MaxRSQ = new double[HGA];
        public double[] MaxRSQq = new double[HGA];

        public double[] MinSSE = new double[HGA];
        public double[] MinSSEe = new double[HGA];

        public double Power;
        public int[] best = new int[HGA];
        public double[,] Adapt = new double[HGA, 20];
        public int[] InLoop = new int[HGA];

        public double apart;
        public double bpart;
        public double cpart;
        public double dpart;

        public double amed;
        public double bmed;
        public double cmed;
        public double dmed;

        public double amin;
        public double amax;
        public double bmin;
        public double bmax;
        public double cmin;
        public double cmax;
        public double dmin;
        public double dmax;

        public double abestmin;
        public double abestmax;
        public double bbestmin;
        public double bbestmax;
        public double cbestmin;
        public double cbestmax;
        public double dbestmin;
        public double dbestmax;

        public double step;
        public double[] I_threshold = new double[HGA];

        public double Gompertz_Average;

        private Random random;
        private int seed;
        public long[] TimeTaken = new long[HGA];
        public int TargetTestTimePerHGA;
        private bool RandomCalculation;
        public void Initialize_All_Variable(int hga, bool reset_all_hga)
        {


            if (reset_all_hga)
            {
                SSE_Initial = Convert.ToDouble(textBox_SSEInitial.Text, CultureInfo.InvariantCulture);//double.Parse(textBox_SSEInitial.Text.ToString(CultureInfo.InvariantCulture));
                RSQ_Initial = Convert.ToDouble(textBox_RSQInitial.Text, CultureInfo.InvariantCulture); //double.Parse(textBox_RSQInitial.Text);
                ABCDInitialValue[0, 0] = Convert.ToDouble(textBox_amin.Text, CultureInfo.InvariantCulture);// double.Parse(textBox_amin.Text);
                ABCDInitialValue[0, 1] = Convert.ToDouble(textBox_amax.Text, CultureInfo.InvariantCulture);
                ABCDInitialValue[1, 0] = Convert.ToDouble(textBox_bmin.Text, CultureInfo.InvariantCulture);
                ABCDInitialValue[1, 1] = Convert.ToDouble(textBox_bmax.Text, CultureInfo.InvariantCulture);

                ABCDInitialValue[2, 0] = Convert.ToDouble(textBox_cmin.Text, CultureInfo.InvariantCulture);
                ABCDInitialValue[2, 1] = Convert.ToDouble(textBox_cmax.Text, CultureInfo.InvariantCulture);
                ABCDInitialValue[3, 0] = Convert.ToDouble(textBox_dmin.Text, CultureInfo.InvariantCulture);
                ABCDInitialValue[3, 1] = Convert.ToDouble(textBox_dmax.Text, CultureInfo.InvariantCulture);//

                Random_Scale = Convert.ToInt32(textBox_randonscale.Text, CultureInfo.InvariantCulture);//int.Parse(textBox_randonscale.Text);

                Weight = Convert.ToDouble(textBox_weight.Text, CultureInfo.InvariantCulture);//double.Parse(textBox_weight.Text);
                AdaptiveSearch[0] = Convert.ToDouble(textBox_adaptiveSearch_A.Text, CultureInfo.InvariantCulture) / 100;//double.Parse(textBox_adaptiveSearch_A.Text) / 100;
                AdaptiveSearch[1] = Convert.ToDouble(textBox_adaptiveSearch_B.Text, CultureInfo.InvariantCulture) / 100;//double.Parse(textBox_adaptiveSearch_B.Text) / 100;
                AdaptiveSearch[2] = Convert.ToDouble(textBox_adaptiveSearch_C.Text, CultureInfo.InvariantCulture) / 100;//double.Parse(textBox_adaptiveSearch_C.Text) / 100;
                AdaptiveSearch[3] = Convert.ToDouble(textBox_adaptiveSearch_D.Text, CultureInfo.InvariantCulture) / 100;//double.Parse(textBox_adaptiveSearch_D.Text) / 100;
                Hard_Limit_RSQ = Convert.ToDouble(textBox_HardLimit_RSQ.Text, CultureInfo.InvariantCulture);//double.Parse(textBox_HardLimit_RSQ.Text);
                Hard_Limit_SSE = Convert.ToDouble(textBox_HardLimit_SSE.Text, CultureInfo.InvariantCulture);//double.Parse(textBox_HardLimit_SSE.Text);
                MaxTest = Convert.ToInt32(textBox_Max.Text, CultureInfo.InvariantCulture);//int.Parse(textBox_Max.Text);
                step = Convert.ToDouble(textBox_Step.Text, CultureInfo.InvariantCulture);//double.Parse(textBox_Step.Text);

                Iteration = Convert.ToInt32(textBox_Iteration.Text, CultureInfo.InvariantCulture);//int.Parse(textBox_Iteration.Text);
                TargetTestTimePerHGA = (int)(Convert.ToDouble(textBoxMaxTestTimePerHGA.Text, CultureInfo.InvariantCulture) * 1000);
                A_Result = new double[HGA, Iteration];
                B_Result = new double[HGA, Iteration];
                C_Result = new double[HGA, Iteration];
                D_Result = new double[HGA, Iteration];

                YBAR = new double[HGA, Iteration];
                SSR = new double[HGA, Iteration];
                SST = new double[HGA, Iteration];
                RSq = new double[HGA, Iteration];
                SSE = new double[HGA, Iteration];



                amin = ABCDInitialValue[0, 0];
                amax = ABCDInitialValue[0, 1];
                bmin = ABCDInitialValue[1, 0];
                bmax = ABCDInitialValue[1, 1];
                cmin = ABCDInitialValue[2, 0];
                cmax = ABCDInitialValue[2, 1];
                dmin = ABCDInitialValue[3, 0];
                dmax = ABCDInitialValue[3, 1];

                apart = (amax - amin) / 6;
                bpart = (bmax - bmin) / 6;
                cpart = (cmax - cmin) / 6;
                dpart = (dmax - dmin) / 6;

                amed = (amax + amin) / 2;
                bmed = (bmax + bmin) / 2;
                cmed = (cmax + cmin) / 2;
                dmed = (dmax + dmin) / 2;

                for (int _hga = 0; _hga < HGA; _hga++)
                {
                    best[_hga] = -1;
                    MinSSEe[_hga] = 0.5;
                    MaxRSQq[_hga] = 60;
                    MinSSE[_hga] = SSE_Initial;
                    MaxRSQ[_hga] = RSQ_Initial;
                    InLoop[_hga] = 1;
                    Adapt[_hga, 0] = 1;
                    I_threshold[_hga] = 0;
                    TimeTaken[_hga] = 0;
                    Max[_hga] = 0;
                }
                //   labelBest2.Text = best[].ToString();

                Array.Clear(A_Result, 0, A_Result.Length);
                Array.Clear(B_Result, 0, B_Result.Length);
                Array.Clear(C_Result, 0, C_Result.Length);
                Array.Clear(D_Result, 0, D_Result.Length);

                Array.Clear(YBAR, 0, YBAR.Length);
                Array.Clear(SSR, 0, SSR.Length);
                Array.Clear(SST, 0, SST.Length);
                Array.Clear(RSq, 0, RSq.Length);
                Array.Clear(SSE, 0, SSE.Length);

                Power = Math.Pow(10, Random_Scale);

                Log.Info(this, "Initialize variable for all hga");

                //Data from Vpd
                if (SIMULATION)
                {
                    for (int _hga = 0; _hga < HGA; _hga++)
                    {
                        Current[0] = 10.1;
                        Current[1] = 10.8;
                        Current[2] = 11.5;
                        Current[3] = 12.2;
                        Current[4] = 12.9;
                        Current[5] = 13.6;
                        Current[6] = 14.3;
                        Current[7] = 15;

                        VPD[_hga, 0] = 0.107;
                        VPD[_hga, 1] = 0.108;
                        VPD[_hga, 2] = 0.108;
                        VPD[_hga, 3] = 0.107;

                        VPD[_hga, 4] = 0.107;
                        VPD[_hga, 5] = 0.108;
                        VPD[_hga, 6] = 0.108;
                        VPD[_hga, 7] = 0.107;

                       /* VPD[_hga, 0] = 0.118;
                        VPD[_hga, 1] = 0.119;
                        VPD[_hga, 2] = 0.12;
                        VPD[_hga, 3] = 0.281;

                        VPD[_hga, 4] = 1.202;
                        VPD[_hga, 5] = 2.031;
                        VPD[_hga, 6] = 2.847;
                        VPD[_hga, 7] = 3.653;*/
                    }
                }

            }
            else
            {
                amin = ABCDInitialValue[0, 0];
                amax = ABCDInitialValue[0, 1];
                bmin = ABCDInitialValue[1, 0];
                bmax = ABCDInitialValue[1, 1];
                cmin = ABCDInitialValue[2, 0];
                cmax = ABCDInitialValue[2, 1];
                dmin = ABCDInitialValue[3, 0];
                dmax = ABCDInitialValue[3, 1];

                apart = (amax - amin) / 6;
                bpart = (bmax - bmin) / 6;
                cpart = (cmax - cmin) / 6;
                dpart = (dmax - dmin) / 6;

                amed = (amax + amin) / 2;
                bmed = (bmax + bmin) / 2;
                cmed = (cmax + cmin) / 2;
                dmed = (dmax + dmin) / 2;


                best[hga] = -1;
                MinSSEe[hga] = 0.5;
                MaxRSQq[hga] = 60;
                MinSSE[hga] = SSE_Initial;
                MaxRSQ[hga] = RSQ_Initial;
                InLoop[hga] = 1;
                for (int n = 0; n < 10; n++)
                    Adapt[hga, n] = 0;
                Adapt[hga, 0] = 1;
                I_threshold[hga] = 0;
                TimeTaken[hga] = 0;

                //   labelBest2.Text = best[].ToString();
                int start_index = hga * Iteration;
                Array.Clear(A_Result, start_index, Iteration);
                Array.Clear(B_Result, start_index, Iteration);
                Array.Clear(C_Result, start_index, Iteration);
                Array.Clear(D_Result, start_index, Iteration);

                Array.Clear(YBAR, start_index, Iteration);
                Array.Clear(SSR, start_index, Iteration);
                Array.Clear(SST, start_index, Iteration);
                Array.Clear(RSq, start_index, Iteration);
                Array.Clear(SSE, start_index, Iteration);

                Power = Math.Pow(10, Random_Scale);


                Log.Info(this, "Initialize variable for hga {0}", hga + 1);
            }

        }
        public panelGomperztCalculation()
        {
            InitializeComponent();
            Gompertz = new double[HGA, GOMPERTZ_SAMPLE];
            Last_Gompertz = new double[HGA, GOMPERTZ_SAMPLE];
            Current = new double[GOMPERTZ_SAMPLE];
            VPD = new double[HGA, GOMPERTZ_SAMPLE];
            //    Random r = new Random();
            //    seed = r.Next(200);
            //    random = new Random(seed);

            if (!SIMULATION)
            {
                btnCalculation.Visible = false;
            }


            //Data from Vpd
            /*     if (SIMULATION)
                 {
                     for (int hga = 0; hga < HGA; hga++)
                     {
                         Current[0] = 10.1;
                         Current[1] = 10.8;
                         Current[2] = 11.5;
                         Current[3] = 12.2;
                         Current[4] = 12.9;
                         Current[5] = 13.6;
                         Current[6] = 14.3;
                         Current[7] = 15;

                         VPD[hga, 0] = 0.118;
                         VPD[hga, 1] = 0.119;
                         VPD[hga, 2] = 0.12;
                         VPD[hga, 3] = 0.281;

                         VPD[hga, 4] = 1.202;
                         VPD[hga, 5] = 2.031;
                         VPD[hga, 6] = 2.847;
                         VPD[hga, 7] = 3.653;
                     }
                 }

                 */

        }

        public bool Calculate_Random_Cases(int hga)
        {
            //initialize the value  
            double check_Vpd_data = 0;
            for (int n = 0; n < GOMPERTZ_SAMPLE; n++)
            {
                check_Vpd_data = check_Vpd_data + VPD[hga, n];
            }

            if (check_Vpd_data == 0)
                return false;
            Max[hga] = 0;

            MaxRSQ[hga] = RSQ_Initial;
            MinSSE[hga] = SSE_Initial;

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            while ((MaxRSQ[hga] < Hard_Limit_RSQ || MinSSE[hga] > Hard_Limit_SSE) && Max[hga] < MaxTest && watch.ElapsedMilliseconds < TargetTestTimePerHGA)
            {

                int i = 0;
                //   best[hga] = -1;

                if (this.comboBox1.InvokeRequired)
                {
                    comboBox1.Invoke(new MethodInvoker(() =>
                    {
                        Initialize_All_Variable(hga, false);
                        labelSeed2.Text = seed.ToString();//("F1", CultureInfo.InvariantCulture);
                        labelBest2.Text = best[hga].ToString();
                    }));
                }
                else
                {
                    Initialize_All_Variable(hga, false);
                    labelSeed2.Text = seed.ToString();//("F1", CultureInfo.InvariantCulture);
                    labelBest2.Text = best[hga].ToString();
                }
                abestmin = amin;
                abestmax = amax;
                bbestmin = bmin;
                bbestmax = bmax;
                cbestmin = cmin;
                cbestmax = cmax;
                dbestmin = dmin;
                dbestmax = dmax;

                for (int a = 0; a < 3; a++)
                {
                    for (int b = 0; b < 3; b++)
                    {
                        for (int c = 0; c < 3; c++)
                        {
                            for (int d = 0; d < 3; d++)
                            {
                                A_Result[hga, i] = First_RandomNumber(a, random); //a_temp;
                                B_Result[hga, i] = Second_RandomNumber(a, random); //b_temp;
                                C_Result[hga, i] = Third_RandomNumber(a, random); //c_temp;
                                D_Result[hga, i] = Fourth_RandomNumber(d, random);

                                Gompertz_Average = 0;

                                for (int n = 0; n < GOMPERTZ_SAMPLE; n++)
                                {
                                    Gompertz[hga, n] = A_Result[hga, i] + (B_Result[hga, i] - A_Result[hga, i]) * 1 / Math.Exp(Math.Exp(-1 * C_Result[hga, i] * (Current[n] - D_Result[hga, i])));
                                    Gompertz_Average = Gompertz_Average + Gompertz[hga, n];
                                }
                                YBAR[hga, i] = Gompertz_Average / GOMPERTZ_SAMPLE;
                                for (int n = 0; n < GOMPERTZ_SAMPLE; n++)
                                {
                                    SSR[hga, i] = SSR[hga, i] + Weight * Math.Pow(Gompertz[hga, n] - YBAR[hga, i], 2);
                                }

                                for (int n = 0; n < GOMPERTZ_SAMPLE; n++)
                                {
                                    SST[hga, i] = SST[hga, i] + Weight * Math.Pow(VPD[hga, n] - YBAR[hga, i], 2);
                                }
                                RSq[hga, i] = SSR[hga, i] / SST[hga, i] * 100;

                                for (int n = 0; n < GOMPERTZ_SAMPLE; n++)
                                {
                                    SSE[hga, i] = SSE[hga, i] + Weight * Math.Pow((VPD[hga, n] - Gompertz[hga, n]), 2);
                                }

                                if (i > 0 && SSE[hga, i] < MinSSEe[hga] && RSq[hga, i] > MaxRSQq[hga] && RSq[hga, i] < 100)
                                {
                                    MaxRSQq[hga] = RSq[hga, i];
                                    MinSSEe[hga] = SSE[hga, i];
                                    abestmin = amin;
                                    abestmax = amax;
                                    bbestmin = bmin;
                                    bbestmax = bmax;
                                    cbestmin = cmin;
                                    cbestmax = cmax;
                                    dbestmin = dmin;
                                    dbestmax = dmax;
                                    best[hga] = i;
                                }


                                i++;
                            }
                        }

                    }
                }

                for (i = 81; i < Iteration; i++)
                {
                    amin = abestmin;
                    amax = abestmax;
                    bmin = bbestmin;
                    bmax = bbestmax;
                    cmin = cbestmin;
                    cmax = cbestmax;
                    dmin = dbestmin;
                    dmax = dbestmax;
                    A_Result[hga, i] = First_RandomNumber(3, random);
                    B_Result[hga, i] = Second_RandomNumber(3, random);
                    C_Result[hga, i] = Third_RandomNumber(3, random);
                    D_Result[hga, i] = Fourth_RandomNumber(3, random);

                    Gompertz_Average = 0;

                    for (int n = 0; n < GOMPERTZ_SAMPLE; n++)
                    {
                        Gompertz[hga, n] = A_Result[hga, i] + (B_Result[hga, i] - A_Result[hga, i]) * 1 / Math.Exp(Math.Exp(-1 * C_Result[hga, i] * (Current[n] - D_Result[hga, i])));
                        Gompertz_Average = Gompertz_Average + Gompertz[hga, n];
                    }
                    YBAR[hga, i] = Gompertz_Average / GOMPERTZ_SAMPLE;
                    for (int n = 0; n < GOMPERTZ_SAMPLE; n++)
                    {
                        SSR[hga, i] = SSR[hga, i] + Weight * Math.Pow(Gompertz[hga, n] - YBAR[hga, i], 2);
                    }

                    for (int n = 0; n < GOMPERTZ_SAMPLE; n++)
                    {
                        SST[hga, i] = SST[hga, i] + Weight * Math.Pow(VPD[hga, n] - YBAR[hga, i], 2);
                    }
                    RSq[hga, i] = SSR[hga, i] / SST[hga, i] * 100;

                    for (int n = 0; n < GOMPERTZ_SAMPLE; n++)
                    {
                        SSE[hga, i] = SSE[hga, i] + Weight * Math.Pow((VPD[hga, n] - Gompertz[hga, n]), 2);
                    }

                    // ElseIf Application.WorksheetFunction.And(i > 36, SSE(i) < MinSSE, SSE(i) < Range("B15").Value, RSq(i) > MaxRSQ, RSq(i) > Range("B14").Value, RSq(i) < 100) Then

                    if (SSE[hga, i] < MinSSE[hga] &&
                        SSE[hga, i] < SSE_Initial &&
                        RSq[hga, i] > MaxRSQ[hga] &&
                        RSq[hga, i] > RSQ_Initial &&
                        RSq[hga, i] < 100)
                    {
                        double aa, bb, cc, dd;
                        MaxRSQ[hga] = RSq[hga, i];
                        MinSSE[hga] = SSE[hga, i];
                        aa = A_Result[hga, i];
                        bb = B_Result[hga, i];
                        cc = C_Result[hga, i];
                        dd = D_Result[hga, i];
                        best[hga] = i;

                        Adapt[hga, InLoop[hga] - 1] = 1 / Math.Pow(1.2, InLoop[hga]);
                        AdaptiveSearch[0] = AdaptiveSearch[0] * Adapt[hga, InLoop[hga]];
                        AdaptiveSearch[1] = AdaptiveSearch[1] * Adapt[hga, InLoop[hga]];
                        AdaptiveSearch[2] = AdaptiveSearch[2] * Adapt[hga, InLoop[hga]];
                        AdaptiveSearch[3] = AdaptiveSearch[3] * Adapt[hga, InLoop[hga]];

                        amin = aa - aa * AdaptiveSearch[0];
                        amax = aa + aa * AdaptiveSearch[0];
                        bmin = bb - bb * AdaptiveSearch[1];
                        bmax = bb + bb * AdaptiveSearch[1];
                        cmin = cc - cc * AdaptiveSearch[2];
                        cmax = cc + cc * AdaptiveSearch[2];
                        dmin = dd - dd * AdaptiveSearch[3];
                        dmax = dd + dd * AdaptiveSearch[3];

                        abestmin = amin;
                        abestmax = amax;
                        bbestmin = bmin;
                        bbestmax = bmax;
                        cbestmin = cmin;
                        cbestmax = cmax;
                        dbestmin = dmin;
                        dbestmax = dmax;
                        InLoop[hga] = InLoop[hga] + 1;

                        for (int n = 0; n < GOMPERTZ_SAMPLE; n++)
                        {
                            Last_Gompertz[hga, n] = Gompertz[hga, n];
                        }

                    }
                    else if ((i == (Iteration - 1) && MinSSE[hga] > Hard_Limit_SSE) ||
                            (i == (Iteration - 1) && MaxRSQ[hga] < Hard_Limit_RSQ))
                    {
                        //    if (Max[hga] < MaxTest)
                        {
                            Max[hga]++;
                        }

                    }
                    /*    else if (i == (Iteration - 1) &&
                                Max[hga] == MaxTest)
                        {

                            Log.Info(this, "Cannot Find Best Value");
                            MessageBox.Show("Cannot Find Best Value");
                    //        start = false;
                            break;
                        }*/
                }
            }

            TimeTaken[hga] = watch.ElapsedMilliseconds;
            watch.Stop();
            return true;
        }



        private double First_RandomNumber(int option, Random random)
        {
            // var random = new System.Random();
            double min, max;
            if (option == 0)
            {
                min = amin;
                max = amin + apart;

            }
            else if (option == 1)
            {
                min = amed - apart;
                max = amed + apart;
            }
            else if (option == 2)
            {
                min = amax - apart;
                max = amax;
            }
            else
            {
                min = abestmin;
                max = abestmax;
            }
            min = min * Power;
            max = max * Power;
            return Math.Round(random.Next((int)min, (int)max) / Power, Random_Scale);
            //  return Math.Round((random.NextDouble() * ((max * Power) - (min * Power)) + (min * Power)) / Power, Random_Scale);
            //  return Math.Round((random.Next(1000000) * ((max * Power) - (min * Power)) / 1000000 + (min * Power)) / Power, Random_Scale);
        }


        private double Second_RandomNumber(int option, Random random)
        {
            //    var random = new System.Random();
            double min, max;
            if (option == 0)
            {
                min = bmin;
                max = bmin + bpart;

            }
            else if (option == 1)
            {
                min = bmed - bpart;
                max = bmed + bpart;
            }
            else if (option == 2)
            {
                min = bmax - bpart;
                max = bmax;
            }
            else
            {
                min = bbestmin;
                max = bbestmax;
            }
            min = min * Power;
            max = max * Power;
            return Math.Round(random.Next((int)min, (int)max) / Power, Random_Scale);
            //return Math.Round((random.Next(1000000) * ((max * Power) - (min * Power))/ 1000000 + (min * Power)) / Power, Random_Scale);
        }

        private double Third_RandomNumber(int option, Random random)
        {
            //   var random = new System.Random();
            double min, max;
            if (option == 0)
            {
                min = cmin;
                max = cmin + cpart;

            }
            else if (option == 1)
            {
                min = cmed - cpart;
                max = cmed + cpart;
            }
            else if (option == 2)
            {
                min = cmax - cpart;
                max = cmax;
            }
            else
            {
                min = cbestmin;
                max = cbestmax;
            }
            min = min * Power;
            max = max * Power;
            return Math.Round(random.Next((int)min, (int)max) / Power, Random_Scale);
            return Math.Round((random.Next(1000000) * ((max * Power) - (min * Power)) / 1000000 + (min * Power)) / Power, Random_Scale);
            // return Math.Round((random.NextDouble() * ((max * Power) - (min * Power)) + (min * Power)) / Power, Random_Scale);
        }

        private double Fourth_RandomNumber(int option, Random random)
        {
            //    var random = new System.Random();
            double min, max, result;
            if (option == 0)
            {
                min = dmin;
                max = dmin + dpart;

            }
            else if (option == 1)
            {
                min = dmed - dpart;
                max = dmed + dpart;
            }
            else if (option == 2)
            {
                min = dmax - dpart;
                max = dmax;
            }
            else
            {
                min = dbestmin;
                max = dbestmax;
            }
            min = min * Power;
            max = max * Power;
            return Math.Round(random.Next((int)min, (int)max) / Power, Random_Scale);
            //  result =  Math.Round((random.NextDouble() * ((max * Power) - (min * Power)) + (min * Power)) / Power, Random_Scale);

            // return result;
            //return Math.Round((random.Next(1000000) * ((max * Power) - (min * Power)) / 1000000 + (min * Power)) / Power, Random_Scale);
        }

        public void GetCalculationResults(int hga)
        {
            if (best[hga] == -1)
                return;
            Log.Info(this, "A Best Value {0}- {1}", A_Result[hga, best[hga]].ToString("F5", CultureInfo.InvariantCulture), best[hga]);
            Log.Info(this, "B Best Value {0}- {1}", B_Result[hga, best[hga]].ToString("F5", CultureInfo.InvariantCulture), best[hga]);
            Log.Info(this, "C Best Value {0}- {1}", C_Result[hga, best[hga]].ToString("F5", CultureInfo.InvariantCulture), best[hga]);
            Log.Info(this, "D Best Value {0}- {1}", D_Result[hga, best[hga]].ToString("F5", CultureInfo.InvariantCulture), best[hga]);

            for (int n = 0; n < GOMPERTZ_SAMPLE; n++)
            {
                Log.Info(this, "LastGompertz [{0}] = {1}", n, Last_Gompertz[hga, n].ToString("F5", CultureInfo.InvariantCulture));
            }

            Log.Info(this, "RSq Best {0} - {1}", RSq[hga, best[hga]].ToString("F5", CultureInfo.InvariantCulture), best[hga]);
            Log.Info(this, "SSE Best {0} - {1}", SSE[hga, best[hga]].ToString("F5", CultureInfo.InvariantCulture), best[hga]);
            Log.Info(this, "amin {0}", amin.ToString("F5", CultureInfo.InvariantCulture));
            Log.Info(this, "amax {0}", amax.ToString("F5", CultureInfo.InvariantCulture));
            Log.Info(this, "bmin {0}", bmin.ToString("F5", CultureInfo.InvariantCulture));
            Log.Info(this, "bmax {0}", bmax.ToString("F5", CultureInfo.InvariantCulture));
            Log.Info(this, "cmin {0}", cmin.ToString("F5", CultureInfo.InvariantCulture));
            Log.Info(this, "cmax {0}", cmax.ToString("F5", CultureInfo.InvariantCulture));
            Log.Info(this, "dmin {0}", dmin.ToString("F5", CultureInfo.InvariantCulture));
            Log.Info(this, "dmax {0}", dmax.ToString("F5", CultureInfo.InvariantCulture));
            Log.Info(this, "Max {0}", Max[hga].ToString("F5", CultureInfo.InvariantCulture));

            Log.Info(this, "InLoop {0}", InLoop[hga]);
            for (int n = 0; n < 10; n++)
            {
                Log.Info(this, "Adapt [{0}] = {1}", n, Adapt[hga, n].ToString("F5", CultureInfo.InvariantCulture));
            }

            double ITHSecondd = 0;
            double SecondMax = 0.2;

            //double q = 2;
            double[] Funcx = new double[49001];
            double[] Funcy = new double[49001];
            double[] Funcy1 = new double[49001];
            double[] Funcy2 = new double[49001];

            Funcx[0] = Current[0];
            Funcy[0] = A_Result[hga, best[hga]] + (B_Result[hga, best[hga]] - A_Result[hga, best[hga]]) * 1 / Math.Exp(Math.Exp(-C_Result[hga, best[hga]] * (Funcx[0] - D_Result[hga, best[hga]])));

            for (int q = 1; q < 49000; q++)
            {
                Funcx[q] = Funcx[q - 1] + step;
                Funcy[q] = A_Result[hga, best[hga]] + (B_Result[hga, best[hga]] - A_Result[hga, best[hga]]) * 1 / Math.Exp(Math.Exp(-C_Result[hga, best[hga]] * (Funcx[q] - D_Result[hga, best[hga]])));

                Funcy1[q] = (Funcy[q] - Funcy[q - 1]) / (Funcx[q] - Funcx[q - 1]);

                Funcy2[q] = (Funcy1[q] - Funcy1[q - 1]) / (Funcx[q] - Funcx[q - 1]);
                if (q != 1 && Funcy2[q] > SecondMax)
                {
                    SecondMax = Funcy2[q];
                    ITHSecondd = Funcx[q];
                }
            }

            I_threshold[hga] = ITHSecondd;
            if (best[hga] != -1)
            {
                Log.Info(this, "IThreshold = {0}", I_threshold[hga].ToString("F5", CultureInfo.InvariantCulture));
                Log.Info(this, "MaxRSQ = {0}", MaxRSQ[hga].ToString("F5", CultureInfo.InvariantCulture));
                Log.Info(this, "MinSSE = {0}", MinSSE[hga].ToString("F5", CultureInfo.InvariantCulture));

                labelIthreshold2.Text = I_threshold[hga].ToString("F5", CultureInfo.InvariantCulture);
                labelMaxRSQ1.Text = MaxRSQ[hga].ToString("F5", CultureInfo.InvariantCulture);
                labelMinSSE2.Text = MinSSE[hga].ToString("F5", CultureInfo.InvariantCulture);
                labelBest2.Text = best[hga].ToString();
                labelMax.Text = Max[hga].ToString();
            }
            else
            {
                Log.Info(this, "IThreshold = No Best value found");
                Log.Info(this, "MaxRSQ = No Best value found");
                Log.Info(this, "MinSSE = No Best value found");

                labelIthreshold2.Text = "No Best value found";//IThreshold.ToString("F5", CultureInfo.InvariantCulture);
                labelMaxRSQ1.Text = "No Best value found";//MaxRSQ.ToString("F5", CultureInfo.InvariantCulture);
                labelMinSSE2.Text = "No Best value found";//MinSSE.ToString("F5", CultureInfo.InvariantCulture);
                labelBest2.Text = best[hga].ToString();
            }
        }


        public void StartCalculate()
        {

            Random r = new Random();
            seed = r.Next(200);
            random = new Random(seed);
            if (this.comboBox1.InvokeRequired)
            {
                comboBox1.Invoke(new MethodInvoker(() =>
                {

                    for (int hga = 0; hga < HGA; hga++)
                    {

                        if (Calculate_Random_Cases(hga))
                        {
                            GetCalculationResults(hga);
                        //    SaveToExcel(hga);
                        }
                    }

                }));
            }
            else
            {

                for (int hga = 0; hga < HGA; hga++)
                {

                    /*   VPD[hga, 0] = 0.118;
                       VPD[hga, 1] = 0.119;
                       VPD[hga, 2] = 0.12;
                       VPD[hga, 3] = 0.281;

                       VPD[hga, 4] = 1.202;
                       VPD[hga, 5] = 2.031;
                       VPD[hga, 6] = 2.847;
                       VPD[hga, 7] = 3.653;

                       Current[0] = 10.1;
                       Current[1] = 10.8;
                       Current[2] = 11.5;
                       Current[3] = 12.2;
                       Current[4] = 12.9;
                       Current[5] = 13.6;
                       Current[6] = 14.3;
                       Current[7] = 15;
   */
                    if (Calculate_Random_Cases(hga))
                    {
                        GetCalculationResults(hga);
                     //   SaveToExcel(hga);
                    }
                }
            }


        }

        public void SpawnThreadToDoCalculation()
        {
            //  ThreadStart ts = StartCalculate;
            //  CalculationThread = new Thread(ts);          
            //  CalculationThread.Start();
        }

        public void btnCalculation_Click(object sender, EventArgs e)
        {
            //   ThreadStart  ts = StartCalculate;
            // CalculationThread = new Thread(() =>
            // {
            //     Action action = new Action(StartCalculate);
            //     this.BeginInvoke(action);
            // });

            //  CalculationThread.Start();

            btnCalculation.Enabled = false;
            Random r = new Random();
            seed = r.Next(200);
            random = new Random(seed);
            Initialize_All_Variable(0, true);

            if (this.comboBox1.InvokeRequired)
            {
                comboBox1.Invoke(new MethodInvoker(() =>
                {
                    for (int hga = 0; hga < HGA; hga++)
                    {

                        if (Calculate_Random_Cases(hga))
                        {
                            GetCalculationResults(hga);
                            SaveToExcel(hga);
                        }
                    }
                    //    PlotGompertzGraph(hga);
                    comboBox1.SelectedIndex = 0;
                    comboBox1_SelectedIndexChanged(null, null);
                }));
            }
            else
            {

                for (int hga = 0; hga < HGA; hga++)
                {

                    if (Calculate_Random_Cases(hga))
                    {
                        GetCalculationResults(hga);
                        SaveToExcel(hga);
                    }
                }
                //    PlotGompertzGraph(hga);
                comboBox1.SelectedIndex = 0;
                comboBox1_SelectedIndexChanged(null, null);
            }
            btnCalculation.Enabled = true;
        }


        private void SaveToExcel(int hga)
        {
            DateTime dateTime = new DateTime();
            string LEDDirectoryPath = string.Format("{0}{1}_HGA{2}.csv", "C:\\Seagate\\HGA.HST\\Data\\BenchTestTool\\Gompertz_", seed, hga + 1);//string.Format("{0}{1}.xls", CommonFunctions.Instance.MeasurementTestFileDirectory, "BiasAndSensingVoltageData");

            if (!File.Exists(LEDDirectoryPath))
            {


                using (StreamWriter WriteToFile = new StreamWriter(LEDDirectoryPath))
                {

                    WriteToFile.Write("BestCoeeficient, Iteration, A,B, C, D, SST,SSR,SSE, RSQ, YBAR");//,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}",textBoxLDUStart.Text);                    
                    WriteToFile.WriteLine();

                }
            }

            using (FileStream fs = new FileStream(LEDDirectoryPath, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                if (best[hga] != -1)
                {
                    sw.WriteLine(String.Format("{0}", A_Result[hga, best[hga]].ToString("F5", CultureInfo.InvariantCulture)));
                    sw.WriteLine(String.Format("{0}", B_Result[hga, best[hga]].ToString("F5", CultureInfo.InvariantCulture)));
                    sw.WriteLine(String.Format("{0}", C_Result[hga, best[hga]].ToString("F5", CultureInfo.InvariantCulture)));
                    sw.WriteLine(String.Format("{0}", D_Result[hga, best[hga]].ToString("F5", CultureInfo.InvariantCulture)));
                }
                for (int n = 0; n < Iteration; n++)
                {

                    sw.Write(String.Format(",{0},{1},{2},{3},{4},{5},{6}, {7}, {8},{9}", n, A_Result[hga, n].ToString("F5", CultureInfo.InvariantCulture),
                        B_Result[hga, n].ToString("F5", CultureInfo.InvariantCulture)
                        , C_Result[hga, n].ToString("F5", CultureInfo.InvariantCulture)
                        , D_Result[hga, n].ToString("F5", CultureInfo.InvariantCulture),
                        SST[hga, n].ToString("F5", CultureInfo.InvariantCulture)
                        , SSR[hga, n].ToString("F5", CultureInfo.InvariantCulture)
                        , SSE[hga, n].ToString("F5", CultureInfo.InvariantCulture)
                        , RSq[hga, n].ToString("F5", CultureInfo.InvariantCulture)
                        , YBAR[hga, n].ToString("F5", CultureInfo.InvariantCulture)));
                    sw.WriteLine();
                }



            }

        }


        private void PlotGompertzGraph(int hga)
        {

            ChartGompertz4P.Series[0].Points.Clear();
            ChartGompertz4P.Series[1].Points.Clear();

            chartSSE.Series[0].Points.Clear();
            chartRSQ.Series[0].Points.Clear();
            chart_AResults.Series[0].Points.Clear();
            chart_BResults.Series[0].Points.Clear();
            chart_CResults.Series[0].Points.Clear();
            chart_DResults.Series[0].Points.Clear();

            double check_Vpd_data = 0;
            try
            {
                for (int n = 0; n < GOMPERTZ_SAMPLE; n++)
                {
                    check_Vpd_data = check_Vpd_data + VPD[hga, n];
                }

                if (check_Vpd_data == 0)
                {
                    labelSeed2.Text = "";
                    labelIthreshold2.Text = "";
                    labelMaxRSQ1.Text = "";
                    labelMinSSE2.Text = "";
                    labelBest2.Text = "";
                    return;
                }
            }
            catch
            { }
            try
            {



                for (int i = 0; i < GOMPERTZ_SAMPLE; i++)
                {
                    //    if (FirstDerivative_voltage[HGAIndex, i] == 0)
                    //        break;
                    DataPoint d_LDU = new DataPoint(Current[i], VPD[hga, i]);
                    d_LDU.ToolTip = "#VALY";

                    //    LDUVoltageLinearFitPoint[k] = (LDUSlope * i) + b_ldu;

                    DataPoint d_LDULFit = new DataPoint(Current[i], Last_Gompertz[hga, i]);
                    d_LDULFit.ToolTip = "#VALY";


                    ChartGompertz4P.Series[0].Points.Add(d_LDU);
                    ChartGompertz4P.Series[1].Points.Add(d_LDULFit);


                }

                this.ChartGompertz4P.ChartAreas[0].AxisX.Interval = Current[1] - Current[0];
                this.ChartGompertz4P.ChartAreas[0].AxisX.Minimum = Current[0] - 1;
                this.ChartGompertz4P.ChartAreas[0].AxisX.Maximum = Current[7] + 1;
                this.ChartGompertz4P.ChartAreas[0].AxisY.Interval = 0.5;
                this.ChartGompertz4P.ChartAreas[0].AxisY.Minimum = -2.0;
                this.ChartGompertz4P.ChartAreas[0].AxisY.Maximum = VPD[hga, 7] + 1;

            }

            catch
            {

            }


            //SSE

            try
            {



                for (int i = 0; i < Iteration; i++)
                {
                    DataPoint dataSSE = new DataPoint(i, SSE[hga, i]);
                    dataSSE.ToolTip = "#VALY";



                    chartSSE.Series[0].Points.Add(dataSSE);



                }

                this.chartSSE.ChartAreas[0].AxisX.Interval = 100;
                this.chartSSE.ChartAreas[0].AxisX.Minimum = 0;
                this.chartSSE.ChartAreas[0].AxisX.Maximum = 10 + Iteration;
                this.chartSSE.ChartAreas[0].AxisY.Interval = 0.5;
                this.chartSSE.ChartAreas[0].AxisY.Minimum = -2.0;
                this.chartSSE.ChartAreas[0].AxisY.Maximum = 20;

            }

            catch
            {

            }

            //RSQ
            try
            {



                for (int i = 0; i < Iteration; i++)
                {
                    DataPoint data = new DataPoint(i, RSq[hga, i]);
                    data.ToolTip = "#VALY";



                    chartRSQ.Series[0].Points.Add(data);



                }

                this.chartRSQ.ChartAreas[0].AxisX.Interval = 100;
                this.chartRSQ.ChartAreas[0].AxisX.Minimum = 0;
                this.chartRSQ.ChartAreas[0].AxisX.Maximum = 10 + Iteration;
                this.chartRSQ.ChartAreas[0].AxisY.Interval = 10;
                this.chartRSQ.ChartAreas[0].AxisY.Minimum = -2.0;
                this.chartRSQ.ChartAreas[0].AxisY.Maximum = 200;

            }

            catch
            {

            }

            //A
            try
            {





                for (int i = 0; i < Iteration; i++)
                {
                    DataPoint data = new DataPoint(i, A_Result[hga, i]);
                    data.ToolTip = "#VALY";



                    chart_AResults.Series[0].Points.Add(data);



                }

                this.chart_AResults.ChartAreas[0].AxisX.Interval = 100;
                this.chart_AResults.ChartAreas[0].AxisX.Minimum = 0;
                this.chart_AResults.ChartAreas[0].AxisX.Maximum = 10 + Iteration;
                this.chart_AResults.ChartAreas[0].AxisY.Interval = 0.02;
                this.chart_AResults.ChartAreas[0].AxisY.Minimum = ABCDInitialValue[0, 0] / 2;
                this.chart_AResults.ChartAreas[0].AxisY.Maximum = ABCDInitialValue[0, 1] * 2;

            }

            catch
            {

            }

            //B
            try
            {


                for (int i = 0; i < Iteration; i++)
                {
                    DataPoint data = new DataPoint(i, B_Result[hga, i]);
                    data.ToolTip = "#VALY";



                    chart_BResults.Series[0].Points.Add(data);



                }

                this.chart_BResults.ChartAreas[0].AxisX.Interval = 100;
                this.chart_BResults.ChartAreas[0].AxisX.Minimum = 0;
                this.chart_BResults.ChartAreas[0].AxisX.Maximum = 10 + Iteration;
                this.chart_BResults.ChartAreas[0].AxisY.Interval = 1;
                this.chart_BResults.ChartAreas[0].AxisY.Minimum = ABCDInitialValue[1, 0] / 2;
                this.chart_BResults.ChartAreas[0].AxisY.Maximum = ABCDInitialValue[1, 1] * 2;

            }

            catch
            {

            }


            //C
            try
            {




                for (int i = 0; i < Iteration; i++)
                {
                    DataPoint data = new DataPoint(i, C_Result[hga, i]);
                    data.ToolTip = "#VALY";



                    chart_CResults.Series[0].Points.Add(data);



                }

                this.chart_CResults.ChartAreas[0].AxisX.Interval = 100;
                this.chart_CResults.ChartAreas[0].AxisX.Minimum = 0;
                this.chart_CResults.ChartAreas[0].AxisX.Maximum = 10 + Iteration;
                this.chart_CResults.ChartAreas[0].AxisY.Interval = 0.5;
                this.chart_CResults.ChartAreas[0].AxisY.Minimum = ABCDInitialValue[2, 0] / 2;
                this.chart_CResults.ChartAreas[0].AxisY.Maximum = ABCDInitialValue[2, 1] * 2;

            }

            catch
            {

            }

            //D
            try
            {

                for (int i = 0; i < Iteration; i++)
                {
                    DataPoint data = new DataPoint(i, D_Result[hga, i]);
                    data.ToolTip = "#VALY";



                    chart_DResults.Series[0].Points.Add(data);



                }

                this.chart_DResults.ChartAreas[0].AxisX.Interval = 100;
                this.chart_DResults.ChartAreas[0].AxisX.Minimum = 0;
                this.chart_DResults.ChartAreas[0].AxisX.Maximum = 10 + Iteration;
                this.chart_DResults.ChartAreas[0].AxisY.Interval = 1;
                this.chart_DResults.ChartAreas[0].AxisY.Minimum = (int)ABCDInitialValue[3, 0];
                this.chart_DResults.ChartAreas[0].AxisY.Maximum = ABCDInitialValue[3, 1] + 1;

            }

            catch
            {

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int hga = comboBox1.SelectedIndex;

            PlotGompertzGraph(hga);

            if (best[hga] != -1)
            {
                Log.Info(this, "IThreshold = {0}", I_threshold[hga].ToString("F5", CultureInfo.InvariantCulture));
                Log.Info(this, "MaxRSQ = {0}", MaxRSQ[hga].ToString("F5", CultureInfo.InvariantCulture));
                Log.Info(this, "MinSSE = {0}", MinSSE[hga].ToString("F5", CultureInfo.InvariantCulture));

                labelIthreshold2.Text = I_threshold[hga].ToString("F5", CultureInfo.InvariantCulture);
                labelMaxRSQ1.Text = MaxRSQ[hga].ToString("F5", CultureInfo.InvariantCulture);
                labelMinSSE2.Text = MinSSE[hga].ToString("F5", CultureInfo.InvariantCulture);
                labelBest2.Text = best[hga].ToString();
                labelMax.Text = Max[hga].ToString();
                labelTimeTaken.Text = TimeTaken[hga].ToString("F1", CultureInfo.InvariantCulture);

                if (MaxRSQ[hga] > Hard_Limit_RSQ && MinSSE[hga] < Hard_Limit_SSE)//(TimeTaken[hga] > 1000)
                {
                    labelTimeTaken.ForeColor = Color.Green;
                    labelStatus.ForeColor = Color.Green;
                    labelStatus.Text = "Meet Criteria";
                }
                else
                {
                    labelTimeTaken.ForeColor = Color.Red;
                    labelStatus.ForeColor = Color.Red;
                    labelStatus.Text = "Fail to Meet Hard Limit";
                }
            }
            else
            {
                Log.Info(this, "IThreshold = No Best value found");
                Log.Info(this, "MaxRSQ = No Best value found");
                Log.Info(this, "MinSSE = No Best value found");

                labelIthreshold2.Text = "No Best value found";//IThreshold.ToString("F5", CultureInfo.InvariantCulture);
                labelMaxRSQ1.Text = "No Best value found";//MaxRSQ.ToString("F5", CultureInfo.InvariantCulture);
                labelMinSSE2.Text = "No Best value found";//MinSSE.ToString("F5", CultureInfo.InvariantCulture);
                labelBest2.Text = best[hga].ToString();
                labelTimeTaken.Text = "";//TimeTaken[hga].ToString("F1", CultureInfo.InvariantCulture);
                labelStatus.Text = "";//"Fail To Meet Criteria";
                labelMax.Text = "";// Max[hga].ToString();
            }
        }


        public void SetCurrent(int start_I, int size_I, int stop_I)
        {
            double _startCurrent = (double)start_I / 1000;
            double _stopCurrent = (double)stop_I / 1000;
            double _sizeCurrent = (double)size_I / 1000;
            for (int x = 0; x < GOMPERTZ_SAMPLE; x++)
            {
                Current[x] = _startCurrent + (_sizeCurrent * x);

                if (Current[x] == _stopCurrent || Current[x] > _stopCurrent)
                {
                    Current[x] = _stopCurrent;
                    Log.Info(this, "DerivativeCalculation Current {0}mA", Current[x].ToString("F3", CultureInfo.InvariantCulture));
                    break;
                }
                Log.Info(this, "DerivativeCalculation Current {0}mA", Current[x].ToString("F3", CultureInfo.InvariantCulture));
            }
        }

        public void LoadSettings(object[] GompertzSetting)
        {

            textBox_SSEInitial.Text = GompertzSetting[0].ToString();
            textBox_RSQInitial.Text = GompertzSetting[1].ToString();
            textBox_amin.Text = GompertzSetting[2].ToString();
            textBox_amax.Text = GompertzSetting[3].ToString();
            textBox_bmin.Text = GompertzSetting[4].ToString();
            textBox_bmax.Text = GompertzSetting[5].ToString();
            textBox_cmin.Text = GompertzSetting[6].ToString();
            textBox_cmax.Text = GompertzSetting[7].ToString();
            textBox_dmin.Text = GompertzSetting[8].ToString();
            textBox_dmax.Text = GompertzSetting[9].ToString();
            textBox_randonscale.Text = GompertzSetting[10].ToString();
            textBox_weight.Text = GompertzSetting[11].ToString();
            textBox_adaptiveSearch_A.Text = GompertzSetting[12].ToString();
            textBox_adaptiveSearch_B.Text = GompertzSetting[13].ToString();
            textBox_adaptiveSearch_C.Text = GompertzSetting[14].ToString();
            textBox_adaptiveSearch_D.Text = GompertzSetting[15].ToString();
            textBox_HardLimit_RSQ.Text = GompertzSetting[16].ToString();
            textBox_HardLimit_SSE.Text = GompertzSetting[17].ToString();
            textBox_Max.Text = GompertzSetting[18].ToString();
            textBox_Step.Text = GompertzSetting[19].ToString();
            textBox_Iteration.Text = GompertzSetting[20].ToString();

            checkBoxUseGompertz.Checked = GompertzSetting[21].ToString() == "True" ? true : false;
            textBoxMaxTestTimePerHGA.Text = GompertzSetting[22].ToString();
            comboBoxRandomCalculation.SelectedIndex = GompertzSetting[23].ToString() == "Random" ? 0 : 1;
            RandomCalculation = GompertzSetting[23].ToString() == "Random" ? true : false;
            UseGompertz_Calculation = checkBoxUseGompertz.Checked;

            SSE_Initial = double.Parse(textBox_SSEInitial.Text);
            RSQ_Initial = double.Parse(textBox_RSQInitial.Text);
            ABCDInitialValue[0, 0] = double.Parse(textBox_amin.Text);
            ABCDInitialValue[0, 1] = double.Parse(textBox_amax.Text);
            ABCDInitialValue[1, 0] = double.Parse(textBox_bmin.Text);
            ABCDInitialValue[1, 1] = double.Parse(textBox_bmax.Text);

            ABCDInitialValue[2, 0] = double.Parse(textBox_cmin.Text);
            ABCDInitialValue[2, 1] = double.Parse(textBox_cmax.Text);
            ABCDInitialValue[3, 0] = double.Parse(textBox_dmin.Text);
            ABCDInitialValue[3, 1] = double.Parse(textBox_dmax.Text);
            Random_Scale = int.Parse(textBox_randonscale.Text);
            Weight = double.Parse(textBox_weight.Text);
            AdaptiveSearch[0] = double.Parse(textBox_adaptiveSearch_A.Text) / 100;
            AdaptiveSearch[1] = double.Parse(textBox_adaptiveSearch_B.Text) / 100;
            AdaptiveSearch[2] = double.Parse(textBox_adaptiveSearch_C.Text) / 100;
            AdaptiveSearch[3] = double.Parse(textBox_adaptiveSearch_D.Text) / 100;
            Hard_Limit_RSQ = double.Parse(textBox_HardLimit_RSQ.Text);
            Hard_Limit_SSE = double.Parse(textBox_HardLimit_SSE.Text);
            MaxTest = int.Parse(textBox_Max.Text);
            step = double.Parse(textBox_Step.Text);
            Iteration = int.Parse(textBox_Iteration.Text);
            TargetTestTimePerHGA = (int)(double.Parse(textBoxMaxTestTimePerHGA.Text) * 1000);

            A_Result = new double[HGA, Iteration];
            B_Result = new double[HGA, Iteration];
            C_Result = new double[HGA, Iteration];
            D_Result = new double[HGA, Iteration];

            YBAR = new double[HGA, Iteration];
            SSR = new double[HGA, Iteration];
            SST = new double[HGA, Iteration];
            RSq = new double[HGA, Iteration];
            SSE = new double[HGA, Iteration];

        }


        public object[] SaveGompertzSetttings()
        {
            object[] setting = new object[24];
            setting[0] = textBox_SSEInitial.Text;
            setting[1] = textBox_RSQInitial.Text;
            setting[2] = textBox_amin.Text;
            setting[3] = textBox_amax.Text;
            setting[4] = textBox_bmin.Text;
            setting[5] = textBox_bmax.Text;
            setting[6] = textBox_cmin.Text;
            setting[7] = textBox_cmax.Text;
            setting[8] = textBox_dmin.Text;
            setting[9] = textBox_dmax.Text;
            setting[10] = textBox_randonscale.Text;
            setting[11] = textBox_weight.Text;
            setting[12] = textBox_adaptiveSearch_A.Text;
            setting[13] = textBox_adaptiveSearch_B.Text;
            setting[14] = textBox_adaptiveSearch_C.Text;
            setting[15] = textBox_adaptiveSearch_D.Text;
            setting[16] = textBox_HardLimit_RSQ.Text;
            setting[17] = textBox_HardLimit_SSE.Text;
            setting[18] = textBox_Max.Text;
            setting[19] = textBox_Step.Text;
            setting[20] = textBox_Iteration.Text;
            setting[21] = checkBoxUseGompertz.Checked;
            setting[22] = textBoxMaxTestTimePerHGA.Text;
            setting[23] = comboBoxRandomCalculation.SelectedIndex;
            return setting;
        }

        public bool UseGompertzCalculation()
        {
            return UseGompertz_Calculation;
        }

        private void checkBoxUseGompertz_CheckedChanged(object sender, EventArgs e)
        {
            UseGompertz_Calculation = checkBoxUseGompertz.Checked;
        }

        private void comboBoxRandomCalculation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBoxRandomCalculation.SelectedIndex == 0)
            {
                RandomCalculation = true;
            }
            else
            {
                RandomCalculation = false;
            }
        }

     
    }
}
