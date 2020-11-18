using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using Seagate.AAS.Utils;
using XyratexOSC.UI;
using XyratexOSC.Logging;
using DesktopTester.UI;
using DesktopTester.Data;
using DesktopTester.Data.IncomingTestProbeData;
using DesktopTester.Data.OutgoingTestProbeData;
using DesktopTester.Utils;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;


namespace DesktopTester.UI
{
    public partial class frmFunctionalTestsRecipeCOMPortSettingsEditor : Form
    {
        public frmFunctionalTestsRecipeCOMPortSettingsEditor()
        {
            InitializeComponent();                        
        }

        private void LoadRecipe()
        {
            CommonFunctions.Instance.LoadFunctionalTestsRecipe();

            //0Ohm
            txtFunctionalTests0CH1.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance0Ohm.ToString();
            txtFunctionalTests0CH2.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance0Ohm.ToString();
            txtFunctionalTests0CH3.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance0Ohm.ToString();
            txtFunctionalTests0CH4.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance0Ohm.ToString();
            txtFunctionalTests0CH5.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance0Ohm.ToString();
            txtFunctionalTests0CH6.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance0Ohm.ToString();

            //10Ohm
            txtFunctionalTests10CH1.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance10Ohm.ToString();
            txtFunctionalTests10CH2.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance10Ohm.ToString();
            txtFunctionalTests10CH3.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance10Ohm.ToString();
            txtFunctionalTests10CH4.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance10Ohm.ToString();
            txtFunctionalTests10CH5.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance10Ohm.ToString();
            txtFunctionalTests10CH6.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance10Ohm.ToString();

            //100Ohm
            txtFunctionalTests100CH1.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance100Ohm.ToString();
            txtFunctionalTests100CH2.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance100Ohm.ToString();
            txtFunctionalTests100CH3.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance100Ohm.ToString();
            txtFunctionalTests100CH4.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance100Ohm.ToString();
            txtFunctionalTests100CH5.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance100Ohm.ToString();
            txtFunctionalTests100CH6.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance100Ohm.ToString();

            //500Ohm
            txtFunctionalTests500CH1.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance500Ohm.ToString();
            txtFunctionalTests500CH2.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance500Ohm.ToString();
            txtFunctionalTests500CH3.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance500Ohm.ToString();
            txtFunctionalTests500CH4.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance500Ohm.ToString();
            txtFunctionalTests500CH5.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance500Ohm.ToString();
            txtFunctionalTests500CH6.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance500Ohm.ToString();

            //1000Ohm
            txtFunctionalTests1000CH1.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance1000Ohm.ToString();
            txtFunctionalTests1000CH2.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance1000Ohm.ToString();
            txtFunctionalTests1000CH3.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance1000Ohm.ToString();
            txtFunctionalTests1000CH4.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance1000Ohm.ToString();
            txtFunctionalTests1000CH5.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance1000Ohm.ToString();
            txtFunctionalTests1000CH6.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance1000Ohm.ToString();

            //10000Ohm
            txtFunctionalTests10000CH1.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch1WriterResistance10000Ohm.ToString();
            txtFunctionalTests10000CH2.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch2TAResistance10000Ohm.ToString();
            txtFunctionalTests10000CH3.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch3WHResistance10000Ohm.ToString();
            txtFunctionalTests10000CH4.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch4RHResistance10000Ohm.ToString();
            txtFunctionalTests10000CH5.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch5R1Resistance10000Ohm.ToString();
            txtFunctionalTests10000CH6.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch6R2Resistance10000Ohm.ToString();

            //Capacitance
            txtFunctionalTests100Capa.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance100pF.ToString();
            txtFunctionalTests270Capa.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance270pF.ToString();
            txtFunctionalTests470Capa.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance470pF.ToString();
            txtFunctionalTests680Capa.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance680pF.ToString();
            txtFunctionalTests820Capa.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance820pF.ToString();
            txtFunctionalTests10000Capa.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Capacitance10nF.ToString();

            //Temperature
            txtFunctionalTests0Temp.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch1Temperature.ToString();
            txtFunctionalTests50Temp.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch2Temperature.ToString();
            txtFunctionalTests100Temp.Text = CommonFunctions.Instance.FunctionalTestsRecipe.Ch3Temperature.ToString();
        }

        private void btnSaveRecipe_Click(object sender, EventArgs e)
        {            
            string FunctionalTestsRecipeFilePath = string.Format("{0}FunctionalTestsRecipe.rcp", CommonFunctions.Instance.RecipeFileDirectory);
            if (!Directory.Exists(CommonFunctions.Instance.RecipeFileDirectory))
            {
                Directory.CreateDirectory(CommonFunctions.Instance.RecipeFileDirectory);
            }
            
            Log.Info("Startup", "Saving recipe for functional tests to {0}.", FunctionalTestsRecipeFilePath);

            SettingsXml _xml = new SettingsXml(FunctionalTestsRecipeFilePath);

            _xml.OpenSection("ZeroOhm");
            // 0Ohm
            _xml.Write("Ch1WriterResistance0Ohm", txtFunctionalTests0CH1.Text);
            _xml.Write("Ch2TAResistance0Ohm", txtFunctionalTests0CH2.Text);
            _xml.Write("Ch3WHResistance0Ohm", txtFunctionalTests0CH3.Text);
            _xml.Write("Ch4RHResistance0Ohm", txtFunctionalTests0CH4.Text);
            _xml.Write("Ch5R1Resistance0Ohm", txtFunctionalTests0CH5.Text);
            _xml.Write("Ch6R2Resistance0Ohm", txtFunctionalTests0CH6.Text);
            _xml.CloseSection();

            _xml.OpenSection("TenOhm");
            // 10Ohm
            _xml.Write("Ch1WriterResistance10Ohm", txtFunctionalTests10CH1.Text);
            _xml.Write("Ch2TAResistance10Ohm", txtFunctionalTests10CH2.Text);
            _xml.Write("Ch3WHResistance10Ohm", txtFunctionalTests10CH3.Text);
            _xml.Write("Ch4RHResistance10Ohm", txtFunctionalTests10CH4.Text);
            _xml.Write("Ch5R1Resistance10Ohm", txtFunctionalTests10CH5.Text);
            _xml.Write("Ch6R2Resistance10Ohm", txtFunctionalTests10CH6.Text);
            _xml.CloseSection();

            _xml.OpenSection("OneHundredOhm");
            // 100Ohm
            _xml.Write("Ch1WriterResistance100Ohm", txtFunctionalTests100CH1.Text);
            _xml.Write("Ch2TAResistance100Ohm", txtFunctionalTests100CH2.Text);
            _xml.Write("Ch3WHResistance100Ohm", txtFunctionalTests100CH3.Text);
            _xml.Write("Ch4RHResistance100Ohm", txtFunctionalTests100CH4.Text);
            _xml.Write("Ch5R1Resistance100Ohm", txtFunctionalTests100CH5.Text);
            _xml.Write("Ch6R2Resistance100Ohm", txtFunctionalTests100CH6.Text);
            _xml.CloseSection();

            _xml.OpenSection("FiveHundredOhm");
            // 500Ohm
            _xml.Write("Ch1WriterResistance500Ohm", txtFunctionalTests500CH1.Text);
            _xml.Write("Ch2TAResistance500Ohm", txtFunctionalTests500CH2.Text);
            _xml.Write("Ch3WHResistance500Ohm", txtFunctionalTests500CH3.Text);
            _xml.Write("Ch4RHResistance500Ohm", txtFunctionalTests500CH4.Text);
            _xml.Write("Ch5R1Resistance500Ohm", txtFunctionalTests500CH5.Text);
            _xml.Write("Ch6R2Resistance500Ohm", txtFunctionalTests500CH6.Text);
            _xml.CloseSection();

            _xml.OpenSection("OneThousandOhm");
            // 1000Ohm
            _xml.Write("Ch1WriterResistance1000Ohm", txtFunctionalTests1000CH1.Text);
            _xml.Write("Ch2TAResistance1000Ohm", txtFunctionalTests1000CH2.Text);
            _xml.Write("Ch3WHResistance1000Ohm", txtFunctionalTests1000CH3.Text);
            _xml.Write("Ch4RHResistance1000Ohm", txtFunctionalTests1000CH4.Text);
            _xml.Write("Ch5R1Resistance1000Ohm", txtFunctionalTests1000CH5.Text);
            _xml.Write("Ch6R2Resistance1000Ohm", txtFunctionalTests1000CH6.Text);
            _xml.CloseSection();

            _xml.OpenSection("TenThousandOhm");
            // 10000Ohm
            _xml.Write("Ch1WriterResistance10000Ohm", txtFunctionalTests10000CH1.Text);
            _xml.Write("Ch2TAResistance10000Ohm", txtFunctionalTests10000CH2.Text);
            _xml.Write("Ch3WHResistance10000Ohm", txtFunctionalTests10000CH3.Text);
            _xml.Write("Ch4RHResistance10000Ohm", txtFunctionalTests10000CH4.Text);
            _xml.Write("Ch5R1Resistance10000Ohm", txtFunctionalTests10000CH5.Text);
            _xml.Write("Ch6R2Resistance10000Ohm", txtFunctionalTests10000CH6.Text);
            _xml.CloseSection();


            _xml.OpenSection("Capacitance");
            // Capacitance
            _xml.Write("Capacitance100pF", txtFunctionalTests100Capa.Text);
            _xml.Write("Capacitance270pF", txtFunctionalTests270Capa.Text);
            _xml.Write("Capacitance470pF", txtFunctionalTests470Capa.Text);
            _xml.Write("Capacitance680pF", txtFunctionalTests680Capa.Text);
            _xml.Write("Capacitance820pF", txtFunctionalTests820Capa.Text);
            _xml.Write("Capacitance10nF", txtFunctionalTests10000Capa.Text);
            _xml.CloseSection();

            _xml.OpenSection("Temperature");
            // Temperature
            _xml.Write("Ch1Temperature", txtFunctionalTests0Temp.Text);
            _xml.Write("Ch2Temperature", txtFunctionalTests50Temp.Text);
            _xml.Write("Ch3Temperature", txtFunctionalTests100Temp.Text);
            _xml.CloseSection();

            _xml.Save();
        }

        private void frmFunctionalTestsRecipeEditor_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("{0}FunctionalTestsRecipe.rcp", CommonFunctions.Instance.RecipeFileDirectory);
            lblProductID.Text = "Product ID: " + CommonFunctions.Instance.strProductID;
            LoadRecipe();
        }
    }
}
