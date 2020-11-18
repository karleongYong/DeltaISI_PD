using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Seagate.AAS.Parsel.Hw;
using Seagate.AAS.Parsel.Device;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.Parsel.Device.SeaveyorZone;
using Seagate.AAS.HGA.HST.Exceptions;
using Seagate.AAS.HGA.HST.Vision;
using Seagate.AAS.HGA.HST.Recipe;
using System.Threading;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.Utils;
using XyratexOSC.UI;
using XyratexOSC.Logging;
using qf4net;

namespace Seagate.AAS.HGA.HST.Controllers
{
    public class DataManagingController : ControllerHST
    {
        private SLDR_PARAM_BIN_DATA SLDR_PARAM_BIN = new SLDR_PARAM_BIN_DATA();
        private ReadWriteRFIDController _rfidController;

        // Constructors ------------------------------------------------------------
        public DataManagingController(HSTWorkcell workcell, string controllerID, string controllerName)
            : base(workcell, controllerID, controllerName)
        {
            _workcell = workcell;
            this._ioManifest = (HSTIOManifest)workcell.IOManifest;
        }

        public override void InitializeController()
        {
            _rfidController = _workcell.Process.InputStationProcess.Controller.RfidController;

        }

        public void GetSLDRData(Carrier inputCarrier)
        {
            SLDR_PARAM_BIN_DATA SLDR_PARAM_BIN = new SLDR_PARAM_BIN_DATA();
            List<ISI_Data_Map> ISI_data_map = new List<ISI_Data_Map>();

            try
            {
                for (int _hga = 0; _hga < _rfidController.FolaTagDataReadInfor.CarrierSize; _hga++)
                {
                    SLDR_PARAM_BIN.Clear();
                    var getData = FISManager.Instance.GetSLDR_PARAM_BIN_Data(SLDR_PARAM_BIN, _rfidController.FolaTagDataReadInfor[_hga].HgaSN);
                    IBSObj ibsobj = null;
                    ISI_Data_Map isi_Data_Map = new ISI_Data_Map
                    {
                        slot = _hga,
                        HgaSN = inputCarrier.RFIDData.RFIDTag[_hga].HgaSN,
                        ISIReader1Data = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES),
                        ISIReader2Data = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES_RD2)
                    };

                    ISI_data_map.Add(isi_Data_Map);

                    switch (_hga)
                    {
                        case 0:
                            inputCarrier.Hga1.DeltaISIResistanceRD1 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES);
                            inputCarrier.Hga1.DeltaISIResistanceRD2 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES_RD2);
                            if(inputCarrier.WorkOrderData.IBSCheck && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN != null && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN.Trim() == "2")
                                inputCarrier.Hga1.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT2_RES);
                            else
                                inputCarrier.Hga1.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES);

                            inputCarrier.Hga1.TST_STATUS = SLDR_PARAM_BIN.TST_STATUS.Trim() != string.Empty ? Convert.ToChar(SLDR_PARAM_BIN.TST_STATUS.Trim()) : '\0';
                            inputCarrier.Hga1.Slider_Lot_Number = SLDR_PARAM_BIN.SLDR_LOT_ID;
                            inputCarrier.Hga1.IsGetISIPassed = getData;
                            inputCarrier.Hga1.ISI_WAF_WTR_RES = SLDR_PARAM_BIN.WAF_WTR_RES;
                            inputCarrier.Hga1.ISI_WAF_WTR_HTR_RES = SLDR_PARAM_BIN.WAF_WTR_HTR_RES;
                            inputCarrier.Hga1.ISI_WAF_TAD_RES = SLDR_PARAM_BIN.WAF_TAD_RES;
                            inputCarrier.Hga1.ISI_WAF_RDR_HTR_RES = SLDR_PARAM_BIN.WAF_RDR_HTR_RES;
                            inputCarrier.Hga1.ISI_RES_AT_ET = SLDR_PARAM_BIN.SLDR_RES;
                            inputCarrier.Hga1.ISI_RES_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_RES_RD2;
                            inputCarrier.Hga1.ISI_AMP_AT_ET = SLDR_PARAM_BIN.SLDR_F1;
                            inputCarrier.Hga1.ISI_AMP_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_F1_RD2;
                            inputCarrier.Hga1.ISI_ASYM_AT_ET = SLDR_PARAM_BIN.SLDR_ASYM;
                            inputCarrier.Hga1.ISI_ASYM_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_ASYM_RD2;
                            inputCarrier.Hga1.ISI_TAB = SLDR_PARAM_BIN.SLDR_TAB;
                            inputCarrier.Hga1.ISI_ET_RD2_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES);
                            inputCarrier.Hga1.ISI_ET_RD1_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES);
                            inputCarrier.Hga1.UTIC_DATA = FISManager.Instance.GetUTICMachineNumber(_rfidController.FolaTagDataReadInfor[_hga].HgaSN);
                            //if(!string.IsNullOrEmpty(inputCarrier.Hga1.UTIC_DATA.EVENT_DATE))
                            //{
                            //    var getUticDate = DateTime.Parse(inputCarrier.Hga1.UTIC_DATA.EVENT_DATE).ToString("dd-MMM-yy:HH:mm:ss").ToUpper();
                                
                            //}


                            inputCarrier.Hga1.Set_sdet_reader1(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES));
                            inputCarrier.Hga1.Set_sdet_reader2(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES));
                            inputCarrier.Hga1.Set_sdet_writer(Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES));

                            if(inputCarrier.WorkOrderData.IBSCheck)
                                inputCarrier.Hga1.IBS_Data = new IBSObj(SLDR_PARAM_BIN.ISI_RD_IBS_PATTERN, SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN, inputCarrier.HGATabType);
                            break;
                        case 1:
                            inputCarrier.Hga2.DeltaISIResistanceRD1 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES);
                            inputCarrier.Hga2.DeltaISIResistanceRD2 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES_RD2);
                            if (inputCarrier.WorkOrderData.IBSCheck && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN != null && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN.Trim() == "2")
                                inputCarrier.Hga2.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT2_RES);
                            else
                                inputCarrier.Hga2.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES);
                            inputCarrier.Hga2.TST_STATUS = SLDR_PARAM_BIN.TST_STATUS.Trim() != string.Empty ? Convert.ToChar(SLDR_PARAM_BIN.TST_STATUS.Trim()) : '\0';
                            inputCarrier.Hga2.Slider_Lot_Number = SLDR_PARAM_BIN.SLDR_LOT_ID;
                            inputCarrier.Hga2.IsGetISIPassed = getData;
                            inputCarrier.Hga2.ISI_WAF_WTR_RES = SLDR_PARAM_BIN.WAF_WTR_RES;
                            inputCarrier.Hga2.ISI_WAF_WTR_HTR_RES = SLDR_PARAM_BIN.WAF_WTR_HTR_RES;
                            inputCarrier.Hga2.ISI_WAF_TAD_RES = SLDR_PARAM_BIN.WAF_TAD_RES;
                            inputCarrier.Hga2.ISI_WAF_RDR_HTR_RES = SLDR_PARAM_BIN.WAF_RDR_HTR_RES;
                            inputCarrier.Hga2.ISI_RES_AT_ET = SLDR_PARAM_BIN.SLDR_RES;
                            inputCarrier.Hga2.ISI_RES_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_RES_RD2;
                            inputCarrier.Hga2.ISI_AMP_AT_ET = SLDR_PARAM_BIN.SLDR_F1;
                            inputCarrier.Hga2.ISI_AMP_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_F1_RD2;
                            inputCarrier.Hga2.ISI_ASYM_AT_ET = SLDR_PARAM_BIN.SLDR_ASYM;
                            inputCarrier.Hga2.ISI_ASYM_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_ASYM_RD2;
                            inputCarrier.Hga2.ISI_TAB = SLDR_PARAM_BIN.SLDR_TAB;
                            inputCarrier.Hga2.ISI_ET_RD2_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES);
                            inputCarrier.Hga2.ISI_ET_RD1_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES);
                            inputCarrier.Hga2.UTIC_DATA = FISManager.Instance.GetUTICMachineNumber(_rfidController.FolaTagDataReadInfor[_hga].HgaSN);

                            inputCarrier.Hga2.Set_sdet_reader1(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES));
                            inputCarrier.Hga2.Set_sdet_reader2(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES));
                            inputCarrier.Hga2.Set_sdet_writer(Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES));

                            if (inputCarrier.WorkOrderData.IBSCheck)
                                inputCarrier.Hga2.IBS_Data = new IBSObj(SLDR_PARAM_BIN.ISI_RD_IBS_PATTERN, SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN, inputCarrier.HGATabType);
                            break;
                        case 2:
                            inputCarrier.Hga3.DeltaISIResistanceRD1 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES);
                            inputCarrier.Hga3.DeltaISIResistanceRD2 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES_RD2);
                            if (inputCarrier.WorkOrderData.IBSCheck && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN != null && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN.Trim() == "2")
                                inputCarrier.Hga3.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT2_RES);
                            else
                                inputCarrier.Hga3.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES);
                            inputCarrier.Hga3.TST_STATUS = SLDR_PARAM_BIN.TST_STATUS.Trim() != string.Empty ? Convert.ToChar(SLDR_PARAM_BIN.TST_STATUS.Trim()) : '\0';
                            inputCarrier.Hga3.Slider_Lot_Number = SLDR_PARAM_BIN.SLDR_LOT_ID;
                            inputCarrier.Hga3.IsGetISIPassed = getData;
                            inputCarrier.Hga3.ISI_WAF_WTR_RES = SLDR_PARAM_BIN.WAF_WTR_RES;
                            inputCarrier.Hga3.ISI_WAF_WTR_HTR_RES = SLDR_PARAM_BIN.WAF_WTR_HTR_RES;
                            inputCarrier.Hga3.ISI_WAF_TAD_RES = SLDR_PARAM_BIN.WAF_TAD_RES;
                            inputCarrier.Hga3.ISI_WAF_RDR_HTR_RES = SLDR_PARAM_BIN.WAF_RDR_HTR_RES;
                            inputCarrier.Hga3.ISI_RES_AT_ET = SLDR_PARAM_BIN.SLDR_RES;
                            inputCarrier.Hga3.ISI_RES_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_RES_RD2;
                            inputCarrier.Hga3.ISI_AMP_AT_ET = SLDR_PARAM_BIN.SLDR_F1;
                            inputCarrier.Hga3.ISI_AMP_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_F1_RD2;
                            inputCarrier.Hga3.ISI_ASYM_AT_ET = SLDR_PARAM_BIN.SLDR_ASYM;
                            inputCarrier.Hga3.ISI_ASYM_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_ASYM_RD2;
                            inputCarrier.Hga3.ISI_TAB = SLDR_PARAM_BIN.SLDR_TAB;
                            inputCarrier.Hga3.ISI_ET_RD2_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES);
                            inputCarrier.Hga3.ISI_ET_RD1_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES);
                            inputCarrier.Hga3.UTIC_DATA = FISManager.Instance.GetUTICMachineNumber(_rfidController.FolaTagDataReadInfor[_hga].HgaSN);

                            inputCarrier.Hga3.Set_sdet_reader1(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES));
                            inputCarrier.Hga3.Set_sdet_reader2(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES));
                            inputCarrier.Hga3.Set_sdet_writer(Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES));

                            if (inputCarrier.WorkOrderData.IBSCheck)
                                inputCarrier.Hga3.IBS_Data = new IBSObj(SLDR_PARAM_BIN.ISI_RD_IBS_PATTERN, SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN, inputCarrier.HGATabType);
                            break;
                        case 3:
                            inputCarrier.Hga4.DeltaISIResistanceRD1 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES);
                            inputCarrier.Hga4.DeltaISIResistanceRD2 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES_RD2);
                            if (inputCarrier.WorkOrderData.IBSCheck && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN != null && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN.Trim() == "2")
                                inputCarrier.Hga4.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT2_RES);
                            else
                                inputCarrier.Hga4.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES);
                            inputCarrier.Hga4.TST_STATUS = SLDR_PARAM_BIN.TST_STATUS.Trim() != string.Empty ? Convert.ToChar(SLDR_PARAM_BIN.TST_STATUS.Trim()) : '\0';
                            inputCarrier.Hga4.Slider_Lot_Number = SLDR_PARAM_BIN.SLDR_LOT_ID;
                            inputCarrier.Hga4.IsGetISIPassed = getData;
                            inputCarrier.Hga4.ISI_WAF_WTR_RES = SLDR_PARAM_BIN.WAF_WTR_RES;
                            inputCarrier.Hga4.ISI_WAF_WTR_HTR_RES = SLDR_PARAM_BIN.WAF_WTR_HTR_RES;
                            inputCarrier.Hga4.ISI_WAF_TAD_RES = SLDR_PARAM_BIN.WAF_TAD_RES;
                            inputCarrier.Hga4.ISI_WAF_RDR_HTR_RES = SLDR_PARAM_BIN.WAF_RDR_HTR_RES;
                            inputCarrier.Hga4.ISI_RES_AT_ET = SLDR_PARAM_BIN.SLDR_RES;
                            inputCarrier.Hga4.ISI_RES_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_RES_RD2;
                            inputCarrier.Hga4.ISI_AMP_AT_ET = SLDR_PARAM_BIN.SLDR_F1;
                            inputCarrier.Hga4.ISI_AMP_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_F1_RD2;
                            inputCarrier.Hga4.ISI_ASYM_AT_ET = SLDR_PARAM_BIN.SLDR_ASYM;
                            inputCarrier.Hga4.ISI_ASYM_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_ASYM_RD2;
                            inputCarrier.Hga4.ISI_TAB = SLDR_PARAM_BIN.SLDR_TAB;
                            inputCarrier.Hga4.ISI_ET_RD2_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES);
                            inputCarrier.Hga4.ISI_ET_RD1_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES);
                            inputCarrier.Hga4.UTIC_DATA = FISManager.Instance.GetUTICMachineNumber(_rfidController.FolaTagDataReadInfor[_hga].HgaSN);

                            inputCarrier.Hga4.Set_sdet_reader1(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES));
                            inputCarrier.Hga4.Set_sdet_reader2(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES));
                            inputCarrier.Hga4.Set_sdet_writer(Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES));

                            if (inputCarrier.WorkOrderData.IBSCheck)
                                inputCarrier.Hga4.IBS_Data = new IBSObj(SLDR_PARAM_BIN.ISI_RD_IBS_PATTERN, SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN, inputCarrier.HGATabType);
                            break;
                        case 4:
                            inputCarrier.Hga5.DeltaISIResistanceRD1 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES);
                            inputCarrier.Hga5.DeltaISIResistanceRD2 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES_RD2);
                            if (inputCarrier.WorkOrderData.IBSCheck && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN != null && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN.Trim() == "2")
                                inputCarrier.Hga5.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT2_RES);
                            else
                                inputCarrier.Hga5.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES);
                            inputCarrier.Hga5.TST_STATUS = SLDR_PARAM_BIN.TST_STATUS.Trim() != string.Empty ? Convert.ToChar(SLDR_PARAM_BIN.TST_STATUS.Trim()) : '\0';
                            inputCarrier.Hga5.Slider_Lot_Number = SLDR_PARAM_BIN.SLDR_LOT_ID;
                            inputCarrier.Hga5.IsGetISIPassed = getData;
                            inputCarrier.Hga5.ISI_WAF_WTR_RES = SLDR_PARAM_BIN.WAF_WTR_RES;
                            inputCarrier.Hga5.ISI_WAF_WTR_HTR_RES = SLDR_PARAM_BIN.WAF_WTR_HTR_RES;
                            inputCarrier.Hga5.ISI_WAF_TAD_RES = SLDR_PARAM_BIN.WAF_TAD_RES;
                            inputCarrier.Hga5.ISI_WAF_RDR_HTR_RES = SLDR_PARAM_BIN.WAF_RDR_HTR_RES;
                            inputCarrier.Hga5.ISI_RES_AT_ET = SLDR_PARAM_BIN.SLDR_RES;
                            inputCarrier.Hga5.ISI_RES_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_RES_RD2;
                            inputCarrier.Hga5.ISI_AMP_AT_ET = SLDR_PARAM_BIN.SLDR_F1;
                            inputCarrier.Hga5.ISI_AMP_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_F1_RD2;
                            inputCarrier.Hga5.ISI_ASYM_AT_ET = SLDR_PARAM_BIN.SLDR_ASYM;
                            inputCarrier.Hga5.ISI_ASYM_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_ASYM_RD2;
                            inputCarrier.Hga5.ISI_TAB = SLDR_PARAM_BIN.SLDR_TAB;
                            inputCarrier.Hga5.ISI_ET_RD2_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES);
                            inputCarrier.Hga5.ISI_ET_RD1_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES);
                            inputCarrier.Hga5.UTIC_DATA = FISManager.Instance.GetUTICMachineNumber(_rfidController.FolaTagDataReadInfor[_hga].HgaSN);

                            inputCarrier.Hga5.Set_sdet_reader1(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES));
                            inputCarrier.Hga5.Set_sdet_reader2(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES));
                            inputCarrier.Hga5.Set_sdet_writer(Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES));

                            if (inputCarrier.WorkOrderData.IBSCheck)
                                inputCarrier.Hga5.IBS_Data = new IBSObj(SLDR_PARAM_BIN.ISI_RD_IBS_PATTERN, SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN, inputCarrier.HGATabType);
                            break;
                        case 5:
                            inputCarrier.Hga6.DeltaISIResistanceRD1 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES);
                            inputCarrier.Hga6.DeltaISIResistanceRD2 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES_RD2);
                            if (inputCarrier.WorkOrderData.IBSCheck && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN != null && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN.Trim() == "2")
                                inputCarrier.Hga6.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT2_RES);
                            else
                                inputCarrier.Hga6.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES);
                            inputCarrier.Hga6.TST_STATUS = SLDR_PARAM_BIN.TST_STATUS.Trim() != string.Empty ? Convert.ToChar(SLDR_PARAM_BIN.TST_STATUS.Trim()) : '\0';
                            inputCarrier.Hga6.Slider_Lot_Number = SLDR_PARAM_BIN.SLDR_LOT_ID;
                            inputCarrier.Hga6.IsGetISIPassed = getData;
                            inputCarrier.Hga6.ISI_WAF_WTR_RES = SLDR_PARAM_BIN.WAF_WTR_RES;
                            inputCarrier.Hga6.ISI_WAF_WTR_HTR_RES = SLDR_PARAM_BIN.WAF_WTR_HTR_RES;
                            inputCarrier.Hga6.ISI_WAF_TAD_RES = SLDR_PARAM_BIN.WAF_TAD_RES;
                            inputCarrier.Hga6.ISI_WAF_RDR_HTR_RES = SLDR_PARAM_BIN.WAF_RDR_HTR_RES;
                            inputCarrier.Hga6.ISI_RES_AT_ET = SLDR_PARAM_BIN.SLDR_RES;
                            inputCarrier.Hga6.ISI_RES_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_RES_RD2;
                            inputCarrier.Hga6.ISI_AMP_AT_ET = SLDR_PARAM_BIN.SLDR_F1;
                            inputCarrier.Hga6.ISI_AMP_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_F1_RD2;
                            inputCarrier.Hga6.ISI_ASYM_AT_ET = SLDR_PARAM_BIN.SLDR_ASYM;
                            inputCarrier.Hga6.ISI_ASYM_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_ASYM_RD2;
                            inputCarrier.Hga6.ISI_TAB = SLDR_PARAM_BIN.SLDR_TAB;
                            inputCarrier.Hga6.ISI_ET_RD2_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES);
                            inputCarrier.Hga6.ISI_ET_RD1_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES);

                            inputCarrier.Hga6.UTIC_DATA = FISManager.Instance.GetUTICMachineNumber(_rfidController.FolaTagDataReadInfor[_hga].HgaSN);

                            inputCarrier.Hga6.Set_sdet_reader1(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES));
                            inputCarrier.Hga6.Set_sdet_reader2(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES));
                            inputCarrier.Hga6.Set_sdet_writer(Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES));

                            if (inputCarrier.WorkOrderData.IBSCheck)
                                inputCarrier.Hga6.IBS_Data = new IBSObj(SLDR_PARAM_BIN.ISI_RD_IBS_PATTERN, SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN, inputCarrier.HGATabType);
                            break;
                        case 6:
                            inputCarrier.Hga7.DeltaISIResistanceRD1 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES);
                            inputCarrier.Hga7.DeltaISIResistanceRD2 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES_RD2);
                            if (inputCarrier.WorkOrderData.IBSCheck && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN != null && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN.Trim() == "2")
                                inputCarrier.Hga7.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT2_RES);
                            else
                                inputCarrier.Hga7.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES);
                            inputCarrier.Hga7.TST_STATUS = SLDR_PARAM_BIN.TST_STATUS.Trim() != string.Empty ? Convert.ToChar(SLDR_PARAM_BIN.TST_STATUS.Trim()) : '\0';
                            inputCarrier.Hga7.Slider_Lot_Number = SLDR_PARAM_BIN.SLDR_LOT_ID;
                            inputCarrier.Hga7.IsGetISIPassed = getData;
                            inputCarrier.Hga7.ISI_WAF_WTR_RES = SLDR_PARAM_BIN.WAF_WTR_RES;
                            inputCarrier.Hga7.ISI_WAF_WTR_HTR_RES = SLDR_PARAM_BIN.WAF_WTR_HTR_RES;
                            inputCarrier.Hga7.ISI_WAF_TAD_RES = SLDR_PARAM_BIN.WAF_TAD_RES;
                            inputCarrier.Hga7.ISI_WAF_RDR_HTR_RES = SLDR_PARAM_BIN.WAF_RDR_HTR_RES;
                            inputCarrier.Hga7.ISI_RES_AT_ET = SLDR_PARAM_BIN.SLDR_RES;
                            inputCarrier.Hga7.ISI_RES_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_RES_RD2;
                            inputCarrier.Hga7.ISI_AMP_AT_ET = SLDR_PARAM_BIN.SLDR_F1;
                            inputCarrier.Hga7.ISI_AMP_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_F1_RD2;
                            inputCarrier.Hga7.ISI_ASYM_AT_ET = SLDR_PARAM_BIN.SLDR_ASYM;
                            inputCarrier.Hga7.ISI_ASYM_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_ASYM_RD2;
                            inputCarrier.Hga7.ISI_TAB = SLDR_PARAM_BIN.SLDR_TAB;
                            inputCarrier.Hga7.ISI_ET_RD2_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES);
                            inputCarrier.Hga7.ISI_ET_RD1_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES);

                            inputCarrier.Hga7.UTIC_DATA = FISManager.Instance.GetUTICMachineNumber(_rfidController.FolaTagDataReadInfor[_hga].HgaSN);

                            inputCarrier.Hga7.Set_sdet_reader1(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES));
                            inputCarrier.Hga7.Set_sdet_reader2(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES));
                            inputCarrier.Hga7.Set_sdet_writer(Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES));

                            if (inputCarrier.WorkOrderData.IBSCheck)
                                inputCarrier.Hga7.IBS_Data = new IBSObj(SLDR_PARAM_BIN.ISI_RD_IBS_PATTERN, SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN, inputCarrier.HGATabType);

                            break;
                        case 7:
                            inputCarrier.Hga8.DeltaISIResistanceRD1 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES);
                            inputCarrier.Hga8.DeltaISIResistanceRD2 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES_RD2);
                            if (inputCarrier.WorkOrderData.IBSCheck && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN != null && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN.Trim() == "2")
                                inputCarrier.Hga8.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT2_RES);
                            else
                                inputCarrier.Hga8.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES);
                            inputCarrier.Hga8.TST_STATUS = SLDR_PARAM_BIN.TST_STATUS.Trim() != string.Empty ? Convert.ToChar(SLDR_PARAM_BIN.TST_STATUS.Trim()) : '\0';
                            inputCarrier.Hga8.Slider_Lot_Number = SLDR_PARAM_BIN.SLDR_LOT_ID;
                            inputCarrier.Hga8.IsGetISIPassed = getData;
                            inputCarrier.Hga8.ISI_WAF_WTR_RES = SLDR_PARAM_BIN.WAF_WTR_RES;
                            inputCarrier.Hga8.ISI_WAF_WTR_HTR_RES = SLDR_PARAM_BIN.WAF_WTR_HTR_RES;
                            inputCarrier.Hga8.ISI_WAF_TAD_RES = SLDR_PARAM_BIN.WAF_TAD_RES;
                            inputCarrier.Hga8.ISI_WAF_RDR_HTR_RES = SLDR_PARAM_BIN.WAF_RDR_HTR_RES;
                            inputCarrier.Hga8.ISI_RES_AT_ET = SLDR_PARAM_BIN.SLDR_RES;
                            inputCarrier.Hga8.ISI_RES_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_RES_RD2;
                            inputCarrier.Hga8.ISI_AMP_AT_ET = SLDR_PARAM_BIN.SLDR_F1;
                            inputCarrier.Hga8.ISI_AMP_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_F1_RD2;
                            inputCarrier.Hga8.ISI_ASYM_AT_ET = SLDR_PARAM_BIN.SLDR_ASYM;
                            inputCarrier.Hga8.ISI_ASYM_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_ASYM_RD2;
                            inputCarrier.Hga8.ISI_TAB = SLDR_PARAM_BIN.SLDR_TAB;
                            inputCarrier.Hga8.ISI_ET_RD2_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES);
                            inputCarrier.Hga8.ISI_ET_RD1_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES);
                            inputCarrier.Hga8.UTIC_DATA = FISManager.Instance.GetUTICMachineNumber(_rfidController.FolaTagDataReadInfor[_hga].HgaSN);

                            inputCarrier.Hga8.Set_sdet_reader1(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES));
                            inputCarrier.Hga8.Set_sdet_reader2(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES));
                            inputCarrier.Hga8.Set_sdet_writer(Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES));

                            if (inputCarrier.WorkOrderData.IBSCheck)
                                inputCarrier.Hga8.IBS_Data = new IBSObj(SLDR_PARAM_BIN.ISI_RD_IBS_PATTERN, SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN, inputCarrier.HGATabType);
                            break;
                        case 8:
                            inputCarrier.Hga9.DeltaISIResistanceRD1 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES);
                            inputCarrier.Hga9.DeltaISIResistanceRD2 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES_RD2);
                            if (inputCarrier.WorkOrderData.IBSCheck && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN != null && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN.Trim() == "2")
                                inputCarrier.Hga9.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT2_RES);
                            else
                                inputCarrier.Hga9.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES);
                            inputCarrier.Hga9.TST_STATUS = SLDR_PARAM_BIN.TST_STATUS.Trim() != string.Empty ? Convert.ToChar(SLDR_PARAM_BIN.TST_STATUS.Trim()) : '\0';
                            inputCarrier.Hga9.Slider_Lot_Number = SLDR_PARAM_BIN.SLDR_LOT_ID;
                            inputCarrier.Hga9.IsGetISIPassed = getData;
                            inputCarrier.Hga9.ISI_WAF_WTR_RES = SLDR_PARAM_BIN.WAF_WTR_RES;
                            inputCarrier.Hga9.ISI_WAF_WTR_HTR_RES = SLDR_PARAM_BIN.WAF_WTR_HTR_RES;
                            inputCarrier.Hga9.ISI_WAF_TAD_RES = SLDR_PARAM_BIN.WAF_TAD_RES;
                            inputCarrier.Hga9.ISI_WAF_RDR_HTR_RES = SLDR_PARAM_BIN.WAF_RDR_HTR_RES;
                            inputCarrier.Hga9.ISI_RES_AT_ET = SLDR_PARAM_BIN.SLDR_RES;
                            inputCarrier.Hga9.ISI_RES_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_RES_RD2;
                            inputCarrier.Hga9.ISI_AMP_AT_ET = SLDR_PARAM_BIN.SLDR_F1;
                            inputCarrier.Hga9.ISI_AMP_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_F1_RD2;
                            inputCarrier.Hga9.ISI_ASYM_AT_ET = SLDR_PARAM_BIN.SLDR_ASYM;
                            inputCarrier.Hga9.ISI_ASYM_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_ASYM_RD2;
                            inputCarrier.Hga9.ISI_TAB = SLDR_PARAM_BIN.SLDR_TAB;
                            inputCarrier.Hga9.ISI_ET_RD2_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES);                           
                            inputCarrier.Hga9.ISI_ET_RD1_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES);

                            inputCarrier.Hga9.UTIC_DATA = FISManager.Instance.GetUTICMachineNumber(_rfidController.FolaTagDataReadInfor[_hga].HgaSN);

                            inputCarrier.Hga9.Set_sdet_reader1(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES));
                            inputCarrier.Hga9.Set_sdet_reader2(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES));
                            inputCarrier.Hga9.Set_sdet_writer(Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES));

                            if (inputCarrier.WorkOrderData.IBSCheck)
                                inputCarrier.Hga9.IBS_Data = new IBSObj(SLDR_PARAM_BIN.ISI_RD_IBS_PATTERN, SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN, inputCarrier.HGATabType);
                            break;
                        case 9:
                            inputCarrier.Hga10.DeltaISIResistanceRD1 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES);
                            inputCarrier.Hga10.DeltaISIResistanceRD2 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES_RD2);
                            if (inputCarrier.WorkOrderData.IBSCheck && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN != null && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN.Trim() == "2")
                                inputCarrier.Hga10.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT2_RES);
                            else
                                inputCarrier.Hga10.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES);
                            inputCarrier.Hga10.TST_STATUS = SLDR_PARAM_BIN.TST_STATUS.Trim() != string.Empty ? Convert.ToChar(SLDR_PARAM_BIN.TST_STATUS.Trim()) : '\0';
                            inputCarrier.Hga10.Slider_Lot_Number = SLDR_PARAM_BIN.SLDR_LOT_ID;
                            inputCarrier.Hga10.IsGetISIPassed = getData;
                            inputCarrier.Hga10.ISI_WAF_WTR_RES = SLDR_PARAM_BIN.WAF_WTR_RES;
                            inputCarrier.Hga10.ISI_WAF_WTR_HTR_RES = SLDR_PARAM_BIN.WAF_WTR_HTR_RES;
                            inputCarrier.Hga10.ISI_WAF_TAD_RES = SLDR_PARAM_BIN.WAF_TAD_RES;
                            inputCarrier.Hga10.ISI_WAF_RDR_HTR_RES = SLDR_PARAM_BIN.WAF_RDR_HTR_RES;
                            inputCarrier.Hga10.ISI_RES_AT_ET = SLDR_PARAM_BIN.SLDR_RES;
                            inputCarrier.Hga10.ISI_RES_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_RES_RD2;
                            inputCarrier.Hga10.ISI_AMP_AT_ET = SLDR_PARAM_BIN.SLDR_F1;
                            inputCarrier.Hga10.ISI_AMP_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_F1_RD2;
                            inputCarrier.Hga10.ISI_ASYM_AT_ET = SLDR_PARAM_BIN.SLDR_ASYM;
                            inputCarrier.Hga10.ISI_ASYM_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_ASYM_RD2;
                            inputCarrier.Hga10.ISI_TAB = SLDR_PARAM_BIN.SLDR_TAB;
                            inputCarrier.Hga10.ISI_ET_RD2_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES);
                            inputCarrier.Hga10.ISI_ET_RD1_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES);
                            inputCarrier.Hga10.UTIC_DATA = FISManager.Instance.GetUTICMachineNumber(_rfidController.FolaTagDataReadInfor[_hga].HgaSN);

                            inputCarrier.Hga10.Set_sdet_reader1(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES));
                            inputCarrier.Hga10.Set_sdet_reader2(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES));
                            inputCarrier.Hga10.Set_sdet_writer(Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES));

                            if (inputCarrier.WorkOrderData.IBSCheck)
                                inputCarrier.Hga10.IBS_Data = new IBSObj(SLDR_PARAM_BIN.ISI_RD_IBS_PATTERN, SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN, inputCarrier.HGATabType);
                            break;
                    }
                }

                try
                {
                    _workcell.ISIDataListIn.Remove(inputCarrier.CarrierID);
                    _workcell.ISIDataListIn.Add(inputCarrier.CarrierID, ISI_data_map);
                }
                catch (Exception)
                {
                }
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.InputStationLifterExtendError, ex);
            }
        }

        public void GetSLDRData_New(Carrier inputCarrier)
        {
            SLDR_PARAM_BIN_DATA SLDR_PARAM_BIN = new SLDR_PARAM_BIN_DATA();
            List<ISI_Data_Map> ISI_data_map = new List<ISI_Data_Map>();

            try
            {
                for (int _hga = 0; _hga < _rfidController.FolaTagDataReadInfor.CarrierSize; _hga++)
                {
                    SLDR_PARAM_BIN.Clear();
                    var getData = FISManager.Instance.GetSLDR_PARAM_BIN_Data(SLDR_PARAM_BIN, _rfidController.FolaTagDataReadInfor[_hga].HgaSN);
                    IBSObj ibsobj = null;
                    ISI_Data_Map isi_Data_Map = new ISI_Data_Map
                    {
                        slot = _hga,
                        HgaSN = inputCarrier.RFIDData.RFIDTag[_hga].HgaSN,
                        ISIReader1Data = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES),
                        ISIReader2Data = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES_RD2)
                    };

                    ISI_data_map.Add(isi_Data_Map);

                    var hga = new Hga(0, HGAStatus.Unknown);
                    switch (_hga)
                    {
                        case 0:
                            hga = inputCarrier.Hga1;
                            break;
                        case 1:
                            hga = inputCarrier.Hga2;
                            break;
                        case 2:
                            hga = inputCarrier.Hga3;
                            break;
                        case 3:
                            hga = inputCarrier.Hga4;
                            break;
                        case 4:
                            hga = inputCarrier.Hga5;
                            break;
                        case 5:
                            hga = inputCarrier.Hga6;
                            break;
                        case 6:
                            hga = inputCarrier.Hga7;
                            break;
                        case 7:
                            hga = inputCarrier.Hga8;
                            break;
                        case 8:
                            hga = inputCarrier.Hga9;
                            break;
                        case 9:
                            hga = inputCarrier.Hga10;
                            break;
                    }

                    hga.DeltaISIResistanceRD1 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES);
                    hga.DeltaISIResistanceRD2 = Convert.ToDouble(SLDR_PARAM_BIN.SLDR_RES_RD2);
                    if (inputCarrier.WorkOrderData.IBSCheck && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN != null && SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN.Trim() == "2")
                        hga.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT2_RES);
                    else
                        hga.DeltaISIResistanceWriter = Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES);
                    hga.TST_STATUS = SLDR_PARAM_BIN.TST_STATUS.Trim() != string.Empty ? Convert.ToChar(SLDR_PARAM_BIN.TST_STATUS.Trim()) : '\0';
                    hga.Slider_Lot_Number = SLDR_PARAM_BIN.SLDR_LOT_ID;
                    hga.IsGetISIPassed = getData;
                    hga.ISI_WAF_WTR_RES = SLDR_PARAM_BIN.WAF_WTR_RES;
                    hga.ISI_WAF_WTR_HTR_RES = SLDR_PARAM_BIN.WAF_WTR_HTR_RES;
                    hga.ISI_WAF_TAD_RES = SLDR_PARAM_BIN.WAF_TAD_RES;
                    hga.ISI_WAF_RDR_HTR_RES = SLDR_PARAM_BIN.WAF_RDR_HTR_RES;
                    hga.ISI_RES_AT_ET = SLDR_PARAM_BIN.SLDR_RES;
                    hga.ISI_RES_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_RES_RD2;
                    hga.ISI_AMP_AT_ET = SLDR_PARAM_BIN.SLDR_F1;
                    hga.ISI_AMP_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_F1_RD2;
                    hga.ISI_ASYM_AT_ET = SLDR_PARAM_BIN.SLDR_ASYM;
                    hga.ISI_ASYM_AT_ET_RD2 = SLDR_PARAM_BIN.SLDR_ASYM_RD2;
                    hga.ISI_TAB = SLDR_PARAM_BIN.SLDR_TAB;
                    hga.ISI_ET_RD2_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES);
                    hga.ISI_ET_RD1_RES = Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES);
                    hga.Last_ET_Threshold = Convert.ToDouble(SLDR_PARAM_BIN.ET_LAS_THRESHOLD);
                    hga.UTIC_DATA = FISManager.Instance.GetUTICMachineNumber(_rfidController.FolaTagDataReadInfor[_hga].HgaSN);
                    hga.Set_sdet_reader1(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD1_RES));
                    hga.Set_sdet_reader2(Convert.ToDouble(SLDR_PARAM_BIN.ET_RD2_RES));
                    hga.Set_sdet_writer(Convert.ToDouble(SLDR_PARAM_BIN.ET_WRT1_RES));

                    if (inputCarrier.WorkOrderData.IBSCheck)
                        hga.IBS_Data = new IBSObj(SLDR_PARAM_BIN.ISI_RD_IBS_PATTERN, SLDR_PARAM_BIN.ISI_WR_IBS_PATTERN, inputCarrier.HGATabType);
                }

                try
                {
                    _workcell.ISIDataListIn.Remove(inputCarrier.CarrierID);
                    _workcell.ISIDataListIn.Add(inputCarrier.CarrierID, ISI_data_map);
                }
                catch (Exception)
                {
                }
            }
            catch (Exception ex)
            {
                HSTException.Throw(HSTErrors.InputStationLifterExtendError, ex);
            }
        }

        public void CheckSLDRDataCorrect(Carrier inputCarrier)
        {
            var foundGetSortFailed = false;
            int foundGetSortFaileCount = 0;
            for (int slot = 0; slot < inputCarrier.RFIDData.RFIDTagData.CarrierSize; slot++)
            {
                //Assigned hga running slot
                var hga = new Hga(0, HGAStatus.Unknown);
                switch (slot)
                {
                    case 0:
                        hga = inputCarrier.Hga1;
                        break;
                    case 1:
                        hga = inputCarrier.Hga2;
                        break;
                    case 2:
                        hga = inputCarrier.Hga3;
                        break;
                    case 3:
                        hga = inputCarrier.Hga4;
                        break;
                    case 4:
                        hga = inputCarrier.Hga5;
                        break;
                    case 5:
                        hga = inputCarrier.Hga6;
                        break;
                    case 6:
                        hga = inputCarrier.Hga7;
                        break;
                    case 7:
                        hga = inputCarrier.Hga8;
                        break;
                    case 8:
                        hga = inputCarrier.Hga9;
                        break;
                    case 9:
                        hga = inputCarrier.Hga10;
                        break;
                }

                if (hga.TST_STATUS.Equals('\0') || hga.TST_STATUS.Equals(string.Empty) || hga.TST_STATUS.Equals('0'))
                {
                    foundGetSortFailed = true;
                    foundGetSortFaileCount++;
                }
            }

            if (foundGetSortFailed && foundGetSortFaileCount > 2)
            {
                if (FISManager.Instance.IsFISConnected)
                    CommonFunctions.Instance.GetputErrorMessage = String.Format("High FAILGETSORT from old SDET Slider(build on last 6 months), please call process technician");
                else
                    CommonFunctions.Instance.GetputErrorMessage = String.Format("GetputServer Connection Failed, please call technician");
                CommonFunctions.Instance.StopMachineRunDueToGetputError = true;
                QF.Instance.Publish(new QEvent(HSTWorkcell.SigStopMachineRun));
            }
        }

    }
}
