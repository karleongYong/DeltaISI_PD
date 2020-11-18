using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.Utils;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Utils;
using Seagate.AAS.HGA.HST.Data;
using System.Reflection;
using XyratexOSC.UI;
using XyratexOSC.Utilities;

namespace Seagate.AAS.HGA.HST.UserControls
{
    public partial class MeasurementTestRecipeEditor : UserControl
    {

        private const string _gridPairNumber = "Pair Number";
        private const string _gridPairRowTopic = "Row Topic";
        private const string _gridPairColTopic = "Col Topic";

        private const string _gridSort1Max = "Sort1Max";
        private const string _gridSort1Min = "Sort1Min";
        private const string _gridSort2Max = "Sort2Max";
        private const string _gridSort2Min = "Sort2Min";
        private const string _gridSort3Max = "Sort3Max";
        private const string _gridSort3Min = "Sort3Min";
        private const string _gridSort4Max = "Sort4Max";
        private const string _gridSort4Min = "Sort4Min";
        private const string _gridSort5Max = "Sort5Max";
        private const string _gridSort5Min = "Sort5Min";
        private const string _gridSort6Max = "Sort6Max";
        private const string _gridSort6Min = "Sort6Min";
        private const string _gridSort7Max = "Sort7Max";
        private const string _gridSort7Min = "Sort7Min";
        private const string _gridSort8Max = "Sort8Max";
        private const string _gridSort8Min = "Sort8Min";

        private int _gridPairRowEdited = 0;
        private int _gridPairColEdited = 0;
        private bool _pairEditAcitived = false;
        private string _currentPadTopicSeledted = string.Empty;
        private bool _shortCircuitEdited = false;


        public MeasurementTestRecipeEditor()
        {
            InitializeComponent();
        }

        private void btnLoadRecipe_Click(object sender, EventArgs e)
        {
            try
            {
                string fileName = CommonFunctions.Instance.ProductRecipeName + CommonFunctions.RecipeExt;
                CommonFunctions.Instance.LoadMeasurementTestRecipe(fileName);

                txtCh1WriterResistanceMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin.ToString();
                txtCh1WriterResistanceMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax.ToString();
                txtCh2TAResistanceMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin.ToString();
                txtCh2TAResistanceMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax.ToString();
                txtCh3WHResistanceMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin.ToString();
                txtCh3WHResistanceMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax.ToString();
                txtCh4RHResistanceMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin.ToString();
                txtCh4RHResistanceMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax.ToString();
                txtCh5R1ResistanceMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin.ToString();
                txtCh5R1ResistanceMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax.ToString();
                txtCh6R2ResistanceMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin.ToString();
                txtCh6R2ResistanceMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax.ToString();
                txtCh6R2ResistanceMin.Enabled = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;
                txtCh6R2ResistanceMax.Enabled = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;

                txtCh1WriterOpenShortMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMin.ToString();
                txtCh1WriterOpenShortMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterOpenShortMax.ToString();
                txtCh2TAOpenShortMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMin.ToString();
                txtCh2TAOpenShortMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAOpenShortMax.ToString();
                txtCh3WHOpenShortMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMin.ToString();
                txtCh3WHOpenShortMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHOpenShortMax.ToString();
                txtCh4RHOpenShortMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMin.ToString();
                txtCh4RHOpenShortMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHOpenShortMax.ToString();
                txtCh5R1OpenShortMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMin.ToString();
                txtCh5R1OpenShortMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1OpenShortMax.ToString();
                txtCh6R2OpenShortMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMin.ToString();
                txtCh6R2OpenShortMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2OpenShortMax.ToString();
                txtCh6R2OpenShortMin.Enabled = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;
                txtCh6R2OpenShortMax.Enabled = !CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;

                readerImpedanceSpecR1.Text = CommonFunctions.Instance.MeasurementTestRecipe.ReaderImpedanceR1Spec.ToString();
                readerImpedanceSpecR2.Text = CommonFunctions.Instance.MeasurementTestRecipe.ReaderImpedanceR2Spec.ToString();
                writerResistanceSpecUP.Text = CommonFunctions.Instance.MeasurementTestRecipe.WriterResistanceSpecUP.ToString();
                writerResistanceSpecDN.Text = CommonFunctions.Instance.MeasurementTestRecipe.WriterResistanceSpecDN.ToString();
                chkbox_EnableDeltaISI.Checked = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable;
                txtbox_DeltaISISpec1.Text = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISISpec1.ToString();//Load delta ISI spec from measurement recipe
                txtbox_DeltaISISpec2.Text = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISISpec2.ToString();
                textBoxSDETRD2Impedance.Text = CommonFunctions.Instance.MeasurementTestRecipe.OffsetR1HSTSDET.ToString();
                textBoxRD2SDETDeltaMoreThan.Text = CommonFunctions.Instance.MeasurementTestRecipe.DeltaR1SpecMoreThan.ToString();
                textBoxRD2SDETDeltaLessThan.Text = CommonFunctions.Instance.MeasurementTestRecipe.DeltaR1SpecLessThan.ToString();
                txtCh6LDUResistanceMin.Text =
                    CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin.ToString();
                txtCh6LDUResistanceMax.Text =
                    CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax.ToString();
                txtCh6LDUOpenShortMin.Text =
                    CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMin.ToString();
                txtCh6LDUOpenShortMax.Text = 
                    CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUOpenShortMax.ToString();

                txtCh6LDUResistanceMin.Enabled = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;
                txtCh6LDUResistanceMax.Enabled = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;
                txtCh6LDUOpenShortMin.Enabled = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;
                txtCh6LDUOpenShortMax.Enabled = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;


                //ykl
                textBoxLEDVoltageLowLimit.Enabled = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;
                textBoxLEDVoltageUpperLimit.Enabled = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;
                textBoxRLDULowLimit.Enabled = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;
                textBoxRLDUUpperLimit.Enabled = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;
                textBoxIthLowLimit.Enabled = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;
                textBoxIthUpperLimit.Enabled = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;
                textBoxVPDMaxLowLimit.Enabled = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;
                textBoxVPDMaxUpperLimit.Enabled = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;
                textDeltaIThresholdNegative.Enabled = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;
                textDeltaIThresholdPositive.Enabled = CommonFunctions.Instance.ConfigurationSetupRecipe.LDUEnable;

                textBoxLEDVoltageLowLimit.Text = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec.ToString();
                textBoxIthLowLimit.Text = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecLower.ToString();
                textBoxIthUpperLimit.Text = CommonFunctions.Instance.MeasurementTestRecipe.IThresholdSpecUpper.ToString();
                textBoxRLDULowLimit.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMin.ToString();
                textBoxRLDUUpperLimit.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch6LDUResistanceMax.ToString();
                textBoxLEDVoltageLowLimit.Text = CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecLower.ToString();
                textBoxLEDVoltageUpperLimit.Text = CommonFunctions.Instance.MeasurementTestRecipe.LEDInterceptSpecUpper.ToString();
                textBoxVPDMaxLowLimit.Text = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMinSpec.ToString();
                textBoxVPDMaxUpperLimit.Text = CommonFunctions.Instance.MeasurementTestRecipe.PDVoltageMaxSpec.ToString();
                textDeltaIThresholdNegative.Text = CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdNegativeSpec.ToString();
                textDeltaIThresholdPositive.Text = CommonFunctions.Instance.MeasurementTestRecipe.DeltaIThresholdPositiveSpec.ToString();
                textBoxIth_Gompertz_LowLimit.Text = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecLower.ToString();
                textBoxIth_Gompertz_HighLimit.Text = CommonFunctions.Instance.MeasurementTestRecipe.Gompertz_IThresholdSpecUpper.ToString();
                //ykl


                textBoxETOnDiskTest.Text = CommonFunctions.Instance.MeasurementTestRecipe.SamplingETOnDisk.ToString();
                touchscreenTextBoxTSRNumber.Text = CommonFunctions.Instance.MeasurementTestRecipe.TSRNumber;
                touchscreenTextBoxTSRGroup.Text = CommonFunctions.Instance.MeasurementTestRecipe.TSRGroup;
                touchscreenTextBoxTSRSpecNo.Text = DisplayFormatter.Round(CommonFunctions.Instance.MeasurementTestRecipe.SpecNumber, 
                    DisplayFormatter.DecimalDigits.Three);
                touchscreenTextBoxTSRSpecVer.Text = CommonFunctions.Instance.MeasurementTestRecipe.SpecVersion;
                touchscreenTextBoxTSRParamId.Text = CommonFunctions.Instance.MeasurementTestRecipe.ParamID;
                touchscreenTextBoxTSRScripName.Text = CommonFunctions.Instance.MeasurementTestRecipe.ScriptName;
                touchscreenTextBoxTSRRadius.Text = DisplayFormatter.Round(CommonFunctions.Instance.MeasurementTestRecipe.Radius, 
                    DisplayFormatter.DecimalDigits.Three);
                touchscreenTextBoxTSRRPM.Text = DisplayFormatter.Round(CommonFunctions.Instance.MeasurementTestRecipe.RPM,
                    DisplayFormatter.DecimalDigits.Zero);
                touchscreenTextBoxTSRSkewAngle.Text = DisplayFormatter.Round(CommonFunctions.Instance.MeasurementTestRecipe.SkewAngle,
                    DisplayFormatter.DecimalDigits.Three);

                UpdateGrid(CommonFunctions.Instance.MeasurementTestRecipe.AdjacentPadsList);
            }
            catch (Exception ex)
            {

            }
        }

        private void MeasurementTestRecipeEditor_Load(object sender, EventArgs e)
        {
            dataGridViewAdjacentPads.AutoGenerateColumns = false;

            PopulateHGAProducts(sender, e);

            GenerateGridColumn();
            btnLoadRecipe_Click(sender, e);
        }

        private void PopulateHGAProducts(object sender, EventArgs e)
        {
            try
            {
                //remark for one recipe
                if (CalibrationSettings.Instance != null)
                {
                   UpdateRangeDisplay();
                }
            }
            catch (Exception ex)
            {
            } 
        }

        private void MeasurementTestRecipeEditor_Enter(object sender, EventArgs e)
        {
            //PopulateHGAProducts(sender, e);          
        }
       
        private void UpdateRangeDisplay()
        {            
            try
            {
                textRecipeName.Text = CommonFunctions.Instance.ProductRecipeName;
                txtProductName.Text = CommonFunctions.Instance.MeasurementTestRecipe.ProductName;

                txtCh1WriterResistanceMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMin.ToString();
                txtCh1WriterResistanceMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch1WriterResistanceMax.ToString();
                txtCh2TAResistanceMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMin.ToString();
                txtCh2TAResistanceMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch2TAResistanceMax.ToString();
                txtCh3WHResistanceMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMin.ToString();
                txtCh3WHResistanceMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch3WHResistanceMax.ToString();
                txtCh4RHResistanceMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMin.ToString();
                txtCh4RHResistanceMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch4RHResistanceMax.ToString();
                txtCh5R1ResistanceMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMin.ToString();
                txtCh5R1ResistanceMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch5R1ResistanceMax.ToString();
                txtCh6R2ResistanceMin.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMin.ToString();
                txtCh6R2ResistanceMax.Text = CommonFunctions.Instance.MeasurementTestRecipe.Ch6R2ResistanceMax.ToString();

                //Delta ISI
                chkbox_EnableDeltaISI.Checked = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISI_Enable;
                txtbox_DeltaISISpec1.Text = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISISpec1.ToString();
                txtbox_DeltaISISpec2.Text = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISISpec2.ToString();

                txtbox_DeltaISISpec1N.Text = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISISpec1N.ToString();
                txtbox_DeltaISISpec1P.Text = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISISpec1P.ToString();

                txtbox_DeltaISISpec2N.Text = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISISpec2N.ToString();
                txtbox_DeltaISISpec2P.Text = CommonFunctions.Instance.MeasurementTestRecipe.DeltaISISpec2P.ToString();


                textBoxSDETRD2Impedance.Text = CommonFunctions.Instance.MeasurementTestRecipe.OffsetR2HSTSDET.ToString();
                textBoxRD2SDETDeltaLessThan.Text = CommonFunctions.Instance.MeasurementTestRecipe.DeltaR2SpecLessThan.ToString();
                textBoxRD2SDETDeltaMoreThan.Text = CommonFunctions.Instance.MeasurementTestRecipe.DeltaR2SpecMoreThan.ToString();

            }
            catch(Exception ex)
            {

            }        
        }

        private void chkbox_EnableDeltaISI_CheckStateChanged(object sender, EventArgs e)
        {
            if ((chkbox_EnableDeltaISI.CheckState == CheckState.Checked))
            {
                txtbox_DeltaISISpec1.Enabled = true;
                txtbox_DeltaISISpec2.Enabled = true;
            }
            else
            {
                txtbox_DeltaISISpec1.Enabled = false; 
                txtbox_DeltaISISpec2.Enabled = false;
            }
        }

        private void GenerateGridColumn()
        {
            dataGridViewAdjacentPads.Rows.Clear();
            dataGridViewAdjacentPads.Columns.Clear();
            dataGridViewAdjacentPads.RowHeadersVisible = false;
            dataGridViewAdjacentPads.AutoGenerateColumns = false;
            dataGridViewAdjacentPads.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewAdjacentPads.AllowUserToAddRows = false;
            dataGridViewAdjacentPads.AllowUserToOrderColumns = false;
            dataGridViewAdjacentPads.AllowUserToResizeColumns = false;

            dataGridViewAdjacentPads.ColumnCount = 3;
            dataGridViewAdjacentPads.Columns[0].Name = _gridPairNumber;
            dataGridViewAdjacentPads.Columns[1].Name = _gridPairRowTopic;
            dataGridViewAdjacentPads.Columns[2].Name = _gridPairColTopic;

            dataGridViewAdjacentPads.Columns[_gridPairNumber].DisplayIndex = 0;
            dataGridViewAdjacentPads.Columns[_gridPairNumber].HeaderText = "Pair No.";
            dataGridViewAdjacentPads.Columns[_gridPairNumber].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewAdjacentPads.Columns[_gridPairNumber].FillWeight = 200;
            dataGridViewAdjacentPads.Columns[_gridPairNumber].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewAdjacentPads.Columns[_gridPairNumber].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewAdjacentPads.Columns[_gridPairRowTopic].DisplayIndex = 1;
            dataGridViewAdjacentPads.Columns[_gridPairRowTopic].HeaderText = "Row Circuit";
            dataGridViewAdjacentPads.Columns[_gridPairRowTopic].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewAdjacentPads.Columns[_gridPairRowTopic].FillWeight = 300;
            dataGridViewAdjacentPads.Columns[_gridPairRowTopic].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewAdjacentPads.Columns[_gridPairRowTopic].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridViewAdjacentPads.Columns[_gridPairColTopic].DisplayIndex = 2;
            dataGridViewAdjacentPads.Columns[_gridPairColTopic].HeaderText = "Col Circuit";
            dataGridViewAdjacentPads.Columns[_gridPairColTopic].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewAdjacentPads.Columns[_gridPairColTopic].FillWeight = 300;
            dataGridViewAdjacentPads.Columns[_gridPairColTopic].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewAdjacentPads.Columns[_gridPairColTopic].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            UpdateGrid(CommonFunctions.Instance.MeasurementTestRecipe.AdjacentPadsList);
        }

        private void UpdateGrid(AdjacentPadList padList)
        {
            int index = 0;
            var unknownString = Enum.GetName(typeof (AdjacentPadTopic), AdjacentPadTopic.Unknown);
            dataGridViewAdjacentPads.Rows.Clear();
            if(padList != null)
            for (int i = 0; i < padList.ListCount; i++)
            {
                var number = i + 1;
                if(padList.DataPadList[i].RowTopic != unknownString)
                {
                    var newRow = new DataGridViewRow();
                    newRow.CreateCells(dataGridViewAdjacentPads);

                    newRow.Cells[0].Value = number.ToString();
                    newRow.Cells[1].Value = padList.DataPadList[i].RowTopic;
                    newRow.Cells[2].Value = padList.DataPadList[i].ColTopic;

                    newRow.Tag = padList.DataPadList[i].Name;
                    index = dataGridViewAdjacentPads.Rows.Add(newRow);
                }

            }
        }

        private void CrossCheckAdjacentPadSetting()
        {
            var unknownString = Enum.GetName(typeof(AdjacentPadTopic), AdjacentPadTopic.Unknown);
            var currentRowSetting = unknownString;
            var currentColSetting = unknownString;

            if(!_pairEditAcitived)
            {
                if (dataGridViewAdjacentPads.Rows.Count > 0)
                {
                    for (int i = 0; i < dataGridViewAdjacentPads.Rows.Count; i++)
                    {
                        currentRowSetting = dataGridViewAdjacentPads.Rows[i].Cells[1].Value.ToString();
                        currentColSetting = dataGridViewAdjacentPads.Rows[i].Cells[2].Value.ToString();
                        if(currentRowSetting != unknownString || currentColSetting != unknownString)
                        {
                            if (currentRowSetting == unknownString)
                                SetGridCellBackColor(i, 1, Color.Red);
                            else
                                if(dataGridViewAdjacentPads.Rows[i].Cells[1].Style.BackColor != Color.White)
                                    SetGridCellBackColor(i, 1, Color.White);

                            if(currentColSetting == unknownString)
                                SetGridCellBackColor(i, 2, Color.Red);
                            else
                                if (dataGridViewAdjacentPads.Rows[i].Cells[2].Style.BackColor != Color.White)
                                    SetGridCellBackColor(i, 2, Color.White);

                        }
                    }
                }

            }

        }

        private void SetGridCellBackColor(int row, int cell, Color color)
        {
            dataGridViewAdjacentPads.Rows[row].Cells[cell].Style.BackColor = color;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CrossCheckAdjacentPadSetting();
        }

        private void CheckDuplicatePadCircuit(int colNo, int rowNo)
        {
            var unknownString = Enum.GetName(typeof(AdjacentPadTopic), AdjacentPadTopic.Unknown);
            string dataNextPadInRow = string.Empty;

            if(colNo == 1)
            {
                dataNextPadInRow = dataGridViewAdjacentPads.Rows[rowNo].Cells[2].Value.ToString();
            }else if(colNo == 2)
            {
                dataNextPadInRow = dataGridViewAdjacentPads.Rows[rowNo].Cells[1].Value.ToString();
            }
        }

        private void MeasurementTestRecipeEditor_VisibleChanged(object sender, EventArgs e)
        {
            timer1.Enabled = Visible;

            if(Visible)
            {
                btnLoadRecipe_Click(sender, e);
                PopulateHGAProducts(sender, e);
            }
        }

    }
}
