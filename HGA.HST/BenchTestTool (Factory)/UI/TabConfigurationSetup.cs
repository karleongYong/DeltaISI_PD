using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Globalization;
using XyratexOSC.Logging;
using XyratexOSC.UI;
using XyratexOSC.Utilities;
using Seagate.AAS.Utils;
using BenchTestsTool.Data;
using BenchTestsTool.Data.IncomingTestProbeData;
using BenchTestsTool.Data.OutgoingTestProbeData;
using BenchTestsTool.Utils;
using System.Runtime.InteropServices;

namespace BenchTestsTool.UI
{
    public partial class frmMain
    {
        private void txtConfigurationSetupCH1Current_TextChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void txtConfigurationSetupCH2Current_TextChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void txtConfigurationSetupCH3Current_TextChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void txtConfigurationSetupCH4Current_TextChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void txtConfigurationSetupCH5Current_TextChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void txtConfigurationSetupCH6Current_TextChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void txtConfigurationSetupAvgCurrentSampleCount_TextChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void txtConfigurationSetupTempTimeConstant_TextChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void txtConfigurationSetupFrequency_TextChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void txtConfigurationSetupBiasVoltage_TextChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void txtConfigurationSetupPeak2PeakVoltage_TextChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void txtConfigurationSetupAvgVoltageSampleCount_TextChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupCh1_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupCh2_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupCh3_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupCh4_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupCh5_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupCh6_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupCapa1_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupCapa2_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupHGA1_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupHGA2_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupHGA3_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupHGA4_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupHGA5_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupHGA6_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupHGA7_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupHGA8_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupHGA9_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupHGA10_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR1C2_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR1C3_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR1C4_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR1C5_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR1C6_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR1C7_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR1C8_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR1C9_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR1C10_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR1C11_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR1C12_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR2C1_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR2C3_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR2C4_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR2C5_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR2C6_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR2C7_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR2C8_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR2C9_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR2C10_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR2C11_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR2C12_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR3C1_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR3C2_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR3C4_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR3C5_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR3C6_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR3C7_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR3C8_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR3C9_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR3C10_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR3C11_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR3C12_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR4C1_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR4C2_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR4C3_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR4C5_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR4C6_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR4C7_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR4C8_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR4C9_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR4C10_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR4C11_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR4C12_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR5C1_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR5C2_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR5C3_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR5C4_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR5C6_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR5C7_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR5C8_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR5C9_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR5C10_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR5C11_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR5C12_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR6C1_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR6C2_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR6C3_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR6C4_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR6C5_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR6C7_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR6C8_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR6C9_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR6C10_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR6C11_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR6C12_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR7C1_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR7C2_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR7C3_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR7C4_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR7C5_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR7C6_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR7C8_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR7C9_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR7C10_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR7C11_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR7C12_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR8C1_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR8C2_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR8C3_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR8C4_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR8C5_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR8C6_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR8C7_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR8C9_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR8C10_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR8C11_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR8C12_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR9C1_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR9C2_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR9C3_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR9C4_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR9C5_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR9C6_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR9C7_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR9C8_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR9C10_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR9C11_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR9C12_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR10C1_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR10C2_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR10C3_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR10C4_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR10C5_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR10C6_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR10C7_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR10C8_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR10C9_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR10C11_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR10C12_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR11C1_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR11C2_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR11C3_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR11C4_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR11C5_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR11C6_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR11C7_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR11C8_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR11C9_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR11C10_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR11C12_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR12C1_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR12C2_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR12C3_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR12C4_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR12C5_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR12C6_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR12C7_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR12C8_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR12C9_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR12C10_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void chkConfigurationSetupR12C11_CheckedChanged(object sender, EventArgs e)
        {
            IsConfigurationSetupTempered = true;
            tabPageConfigurationSetup.Refresh();
        }

        private void tabPageConfigurationSetup_Paint(object sender, PaintEventArgs e)
        {
            base.OnPaint(e);

            System.Drawing.SolidBrush myBrush;
            if (IsConfigurationSetupTempered == true)
            {
                myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red);
            }
            else
            {
                myBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Green);
            }

            System.Drawing.Graphics tabPageConfigurationSetupGraphics = tabPageConfigurationSetup.CreateGraphics();
            tabPageConfigurationSetupGraphics.FillEllipse(myBrush, new Rectangle(580, 430, 40, 40));
            myBrush.Dispose();
            tabPageConfigurationSetupGraphics.Dispose();
        }        

        private void btnConfigurationLoadConfiguration_Click(object sender, EventArgs e)
        {            
            CommonFunctions.Instance.LoadConfigurationSetupRecipe();

            //Bias Current
            txtConfigurationSetupCH1Current.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.Ch1BiasCurrent.ToString();
            txtConfigurationSetupCH2Current.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.Ch2BiasCurrent.ToString();
            txtConfigurationSetupCH3Current.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.Ch3BiasCurrent.ToString();
            txtConfigurationSetupCH4Current.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.Ch4BiasCurrent.ToString();
            txtConfigurationSetupCH5Current.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.Ch5BiasCurrent.ToString(); 
            txtConfigurationSetupCH6Current.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.Ch6BiasCurrent.ToString();
            txtConfigurationSetupAvgCurrentSampleCount.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.BiasCurrentSampleCountForAverage.ToString();

            //Temperature
            txtConfigurationSetupTempTimeConstant.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.TimeConstant.ToString();

            //BiasVoltage
            txtConfigurationSetupFrequency.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.Frequency.ToString();
            txtConfigurationSetupBiasVoltage.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.BiasVoltage.ToString();
            txtConfigurationSetupPeak2PeakVoltage.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.PeakVoltage.ToString();
            txtConfigurationSetupAvgVoltageSampleCount.Text = CommonFunctions.Instance.ConfigurationSetupRecipe.BiasVoltageSampleCountForAverage.ToString();

            //HGA Channel
            chkConfigurationSetupCh1.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer == 1);
            chkConfigurationSetupCh2.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA == 1);
            chkConfigurationSetupCh3.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater == 1);
            chkConfigurationSetupCh4.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater == 1);
            chkConfigurationSetupCh5.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1 == 1);
            chkConfigurationSetupCh6.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2 == 1);
            chkConfigurationSetupCapa1.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.CapacitanceCh1 == 1);
            chkConfigurationSetupCapa2.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.CapacitanceCh2 == 1);

            //HGA
            chkConfigurationSetupHGA1.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1 == 1);
            chkConfigurationSetupHGA2.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2 == 1);
            chkConfigurationSetupHGA3.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3 == 1);
            chkConfigurationSetupHGA4.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4 == 1);
            chkConfigurationSetupHGA5.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5 == 1);
            chkConfigurationSetupHGA6.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6 == 1);
            chkConfigurationSetupHGA7.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7 == 1);
            chkConfigurationSetupHGA8.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8 == 1);
            chkConfigurationSetupHGA9.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9 == 1);
            chkConfigurationSetupHGA10.Checked = (CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10 == 1);

            //Row1
            chkConfigurationSetupR1C2.Checked = false;
            chkConfigurationSetupR1C3.Checked = false;
            chkConfigurationSetupR1C4.Checked = false;
            chkConfigurationSetupR1C5.Checked = false;
            chkConfigurationSetupR1C6.Checked = false;
            chkConfigurationSetupR1C7.Checked = false;
            chkConfigurationSetupR1C8.Checked = false;
            chkConfigurationSetupR1C9.Checked = false;
            chkConfigurationSetupR1C10.Checked = false;
            chkConfigurationSetupR1C11.Checked = false;
            chkConfigurationSetupR1C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 2)
            {
                chkConfigurationSetupR1C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 3)
            {
                chkConfigurationSetupR1C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 4)
            {
                chkConfigurationSetupR1C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 5)
            {
                chkConfigurationSetupR1C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 6)
            {
                chkConfigurationSetupR1C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 7)
            {
                chkConfigurationSetupR1C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 8)
            {
                chkConfigurationSetupR1C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 9)
            {
                chkConfigurationSetupR1C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 10)
            {
                chkConfigurationSetupR1C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 11)
            {
                chkConfigurationSetupR1C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing == 12)
            {
                chkConfigurationSetupR1C12.Checked = true;
            }

            //Row2
            chkConfigurationSetupR2C1.Checked = false;
            chkConfigurationSetupR2C3.Checked = false;
            chkConfigurationSetupR2C4.Checked = false;
            chkConfigurationSetupR2C5.Checked = false;
            chkConfigurationSetupR2C6.Checked = false;
            chkConfigurationSetupR2C7.Checked = false;
            chkConfigurationSetupR2C8.Checked = false;
            chkConfigurationSetupR2C9.Checked = false;
            chkConfigurationSetupR2C10.Checked = false;
            chkConfigurationSetupR2C11.Checked = false;
            chkConfigurationSetupR2C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 1)
            {
                chkConfigurationSetupR2C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 3)
            {
                chkConfigurationSetupR2C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 4)
            {
                chkConfigurationSetupR2C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 5)
            {
                chkConfigurationSetupR2C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 6)
            {
                chkConfigurationSetupR2C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 7)
            {
                chkConfigurationSetupR2C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 8)
            {
                chkConfigurationSetupR2C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 9)
            {
                chkConfigurationSetupR2C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 10)
            {
                chkConfigurationSetupR2C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 11)
            {
                chkConfigurationSetupR2C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing == 12)
            {
                chkConfigurationSetupR2C12.Checked = true;
            }

            //Row3
            chkConfigurationSetupR3C1.Checked = false;
            chkConfigurationSetupR3C2.Checked = false;
            chkConfigurationSetupR3C4.Checked = false;
            chkConfigurationSetupR3C5.Checked = false;
            chkConfigurationSetupR3C6.Checked = false;
            chkConfigurationSetupR3C7.Checked = false;
            chkConfigurationSetupR3C8.Checked = false;
            chkConfigurationSetupR3C9.Checked = false;
            chkConfigurationSetupR3C10.Checked = false;
            chkConfigurationSetupR3C11.Checked = false;
            chkConfigurationSetupR3C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 1)
            {
                chkConfigurationSetupR3C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 2)
            {
                chkConfigurationSetupR3C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 4)
            {
                chkConfigurationSetupR3C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 5)
            {
                chkConfigurationSetupR3C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 6)
            {
                chkConfigurationSetupR3C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 7)
            {
                chkConfigurationSetupR3C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 8)
            {
                chkConfigurationSetupR3C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 9)
            {
                chkConfigurationSetupR3C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 10)
            {
                chkConfigurationSetupR3C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 11)
            {
                chkConfigurationSetupR3C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing == 12)
            {
                chkConfigurationSetupR3C12.Checked = true;
            }

            //Row4
            chkConfigurationSetupR4C1.Checked = false;
            chkConfigurationSetupR4C2.Checked = false;
            chkConfigurationSetupR4C3.Checked = false;
            chkConfigurationSetupR4C5.Checked = false;
            chkConfigurationSetupR4C6.Checked = false;
            chkConfigurationSetupR4C7.Checked = false;
            chkConfigurationSetupR4C8.Checked = false;
            chkConfigurationSetupR4C9.Checked = false;
            chkConfigurationSetupR4C10.Checked = false;
            chkConfigurationSetupR4C11.Checked = false;
            chkConfigurationSetupR4C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 1)
            {
                chkConfigurationSetupR4C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 2)
            {
                chkConfigurationSetupR4C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 3)
            {
                chkConfigurationSetupR4C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 5)
            {
                chkConfigurationSetupR4C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 6)
            {
                chkConfigurationSetupR4C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 7)
            {
                chkConfigurationSetupR4C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 8)
            {
                chkConfigurationSetupR4C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 9)
            {
                chkConfigurationSetupR4C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 10)
            {
                chkConfigurationSetupR4C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 11)
            {
                chkConfigurationSetupR4C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing == 12)
            {
                chkConfigurationSetupR4C12.Checked = true;
            }

            //Row5
            chkConfigurationSetupR5C1.Checked = false;
            chkConfigurationSetupR5C2.Checked = false;
            chkConfigurationSetupR5C3.Checked = false;
            chkConfigurationSetupR5C4.Checked = false;
            chkConfigurationSetupR5C6.Checked = false;
            chkConfigurationSetupR5C7.Checked = false;
            chkConfigurationSetupR5C8.Checked = false;
            chkConfigurationSetupR5C9.Checked = false;
            chkConfigurationSetupR5C10.Checked = false;
            chkConfigurationSetupR5C11.Checked = false;
            chkConfigurationSetupR5C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 1)
            {
                chkConfigurationSetupR5C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 2)
            {
                chkConfigurationSetupR5C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 3)
            {
                chkConfigurationSetupR5C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 4)
            {
                chkConfigurationSetupR5C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 6)
            {
                chkConfigurationSetupR5C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 7)
            {
                chkConfigurationSetupR5C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 8)
            {
                chkConfigurationSetupR5C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 9)
            {
                chkConfigurationSetupR5C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 10)
            {
                chkConfigurationSetupR5C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 11)
            {
                chkConfigurationSetupR5C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing == 12)
            {
                chkConfigurationSetupR5C12.Checked = true;
            }

            //Row6
            chkConfigurationSetupR6C1.Checked = false;
            chkConfigurationSetupR6C2.Checked = false;
            chkConfigurationSetupR6C3.Checked = false;
            chkConfigurationSetupR6C4.Checked = false;
            chkConfigurationSetupR6C5.Checked = false;
            chkConfigurationSetupR6C7.Checked = false;
            chkConfigurationSetupR6C8.Checked = false;
            chkConfigurationSetupR6C9.Checked = false;
            chkConfigurationSetupR6C10.Checked = false;
            chkConfigurationSetupR6C11.Checked = false;
            chkConfigurationSetupR6C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 1)
            {
                chkConfigurationSetupR6C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 2)
            {
                chkConfigurationSetupR6C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 3)
            {
                chkConfigurationSetupR6C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 4)
            {
                chkConfigurationSetupR6C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 5)
            {
                chkConfigurationSetupR6C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 7)
            {
                chkConfigurationSetupR6C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 8)
            {
                chkConfigurationSetupR6C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 9)
            {
                chkConfigurationSetupR6C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 10)
            {
                chkConfigurationSetupR6C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 11)
            {
                chkConfigurationSetupR6C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing == 12)
            {
                chkConfigurationSetupR6C12.Checked = true;
            }

            //Row7
            chkConfigurationSetupR7C1.Checked = false;
            chkConfigurationSetupR7C2.Checked = false;
            chkConfigurationSetupR7C3.Checked = false;
            chkConfigurationSetupR7C4.Checked = false;
            chkConfigurationSetupR7C5.Checked = false;
            chkConfigurationSetupR7C6.Checked = false;
            chkConfigurationSetupR7C8.Checked = false;
            chkConfigurationSetupR7C9.Checked = false;
            chkConfigurationSetupR7C10.Checked = false;
            chkConfigurationSetupR7C11.Checked = false;
            chkConfigurationSetupR7C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 1)
            {
                chkConfigurationSetupR7C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 2)
            {
                chkConfigurationSetupR7C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 3)
            {
                chkConfigurationSetupR7C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 4)
            {
                chkConfigurationSetupR7C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 5)
            {
                chkConfigurationSetupR7C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 6)
            {
                chkConfigurationSetupR7C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 8)
            {
                chkConfigurationSetupR7C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 9)
            {
                chkConfigurationSetupR7C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 10)
            {
                chkConfigurationSetupR7C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 11)
            {
                chkConfigurationSetupR7C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing == 12)
            {
                chkConfigurationSetupR7C12.Checked = true;
            }

            //Row8
            chkConfigurationSetupR8C1.Checked = false;
            chkConfigurationSetupR8C2.Checked = false;
            chkConfigurationSetupR8C3.Checked = false;
            chkConfigurationSetupR8C4.Checked = false;
            chkConfigurationSetupR8C5.Checked = false;
            chkConfigurationSetupR8C6.Checked = false;
            chkConfigurationSetupR8C7.Checked = false;
            chkConfigurationSetupR8C9.Checked = false;
            chkConfigurationSetupR8C10.Checked = false;
            chkConfigurationSetupR8C11.Checked = false;
            chkConfigurationSetupR8C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 1)
            {
                chkConfigurationSetupR8C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 2)
            {
                chkConfigurationSetupR8C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 3)
            {
                chkConfigurationSetupR8C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 4)
            {
                chkConfigurationSetupR8C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 5)
            {
                chkConfigurationSetupR8C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 6)
            {
                chkConfigurationSetupR8C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 7)
            {
                chkConfigurationSetupR8C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 9)
            {
                chkConfigurationSetupR8C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 10)
            {
                chkConfigurationSetupR8C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 11)
            {
                chkConfigurationSetupR8C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing == 12)
            {
                chkConfigurationSetupR8C12.Checked = true;
            }

            //Row9
            chkConfigurationSetupR9C1.Checked = false;
            chkConfigurationSetupR9C2.Checked = false;
            chkConfigurationSetupR9C3.Checked = false;
            chkConfigurationSetupR9C4.Checked = false;
            chkConfigurationSetupR9C5.Checked = false;
            chkConfigurationSetupR9C6.Checked = false;
            chkConfigurationSetupR9C7.Checked = false;
            chkConfigurationSetupR9C8.Checked = false;
            chkConfigurationSetupR9C10.Checked = false;
            chkConfigurationSetupR9C11.Checked = false;
            chkConfigurationSetupR9C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 1)
            {
                chkConfigurationSetupR9C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 2)
            {
                chkConfigurationSetupR9C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 3)
            {
                chkConfigurationSetupR9C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 4)
            {
                chkConfigurationSetupR9C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 5)
            {
                chkConfigurationSetupR9C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 6)
            {
                chkConfigurationSetupR9C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 7)
            {
                chkConfigurationSetupR9C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 8)
            {
                chkConfigurationSetupR9C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 10)
            {
                chkConfigurationSetupR9C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 11)
            {
                chkConfigurationSetupR9C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing == 12)
            {
                chkConfigurationSetupR9C12.Checked = true;
            }

            //Row10
            chkConfigurationSetupR10C1.Checked = false;
            chkConfigurationSetupR10C2.Checked = false;
            chkConfigurationSetupR10C3.Checked = false;
            chkConfigurationSetupR10C4.Checked = false;
            chkConfigurationSetupR10C5.Checked = false;
            chkConfigurationSetupR10C6.Checked = false;
            chkConfigurationSetupR10C7.Checked = false;
            chkConfigurationSetupR10C8.Checked = false;
            chkConfigurationSetupR10C9.Checked = false;
            chkConfigurationSetupR10C11.Checked = false;
            chkConfigurationSetupR10C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 1)
            {
                chkConfigurationSetupR10C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 2)
            {
                chkConfigurationSetupR10C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 3)
            {
                chkConfigurationSetupR10C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 4)
            {
                chkConfigurationSetupR10C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 5)
            {
                chkConfigurationSetupR10C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 6)
            {
                chkConfigurationSetupR10C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 7)
            {
                chkConfigurationSetupR10C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 8)
            {
                chkConfigurationSetupR10C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 9)
            {
                chkConfigurationSetupR10C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 11)
            {
                chkConfigurationSetupR10C11.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing == 12)
            {
                chkConfigurationSetupR10C12.Checked = true;
            }

            //Row11
            chkConfigurationSetupR11C1.Checked = false;
            chkConfigurationSetupR11C2.Checked = false;
            chkConfigurationSetupR11C3.Checked = false;
            chkConfigurationSetupR11C4.Checked = false;
            chkConfigurationSetupR11C5.Checked = false;
            chkConfigurationSetupR11C6.Checked = false;
            chkConfigurationSetupR11C7.Checked = false;
            chkConfigurationSetupR11C8.Checked = false;
            chkConfigurationSetupR11C9.Checked = false;
            chkConfigurationSetupR11C10.Checked = false;
            chkConfigurationSetupR11C12.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 1)
            {
                chkConfigurationSetupR11C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 2)
            {
                chkConfigurationSetupR11C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 3)
            {
                chkConfigurationSetupR11C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 4)
            {
                chkConfigurationSetupR11C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 5)
            {
                chkConfigurationSetupR11C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 6)
            {
                chkConfigurationSetupR11C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 7)
            {
                chkConfigurationSetupR11C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 8)
            {
                chkConfigurationSetupR11C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 9)
            {
                chkConfigurationSetupR11C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 10)
            {
                chkConfigurationSetupR11C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing == 12)
            {
                chkConfigurationSetupR11C12.Checked = true;
            }

            //Row12
            chkConfigurationSetupR12C1.Checked = false;
            chkConfigurationSetupR12C2.Checked = false;
            chkConfigurationSetupR12C3.Checked = false;
            chkConfigurationSetupR12C4.Checked = false;
            chkConfigurationSetupR12C5.Checked = false;
            chkConfigurationSetupR12C6.Checked = false;
            chkConfigurationSetupR12C7.Checked = false;
            chkConfigurationSetupR12C8.Checked = false;
            chkConfigurationSetupR12C9.Checked = false;
            chkConfigurationSetupR12C10.Checked = false;
            chkConfigurationSetupR12C11.Checked = false;
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 1)
            {
                chkConfigurationSetupR12C1.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 2)
            {
                chkConfigurationSetupR12C2.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 3)
            {
                chkConfigurationSetupR12C3.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 4)
            {
                chkConfigurationSetupR12C4.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 5)
            {
                chkConfigurationSetupR12C5.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 6)
            {
                chkConfigurationSetupR12C6.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 7)
            {
                chkConfigurationSetupR12C7.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 8)
            {
                chkConfigurationSetupR12C8.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 9)
            {
                chkConfigurationSetupR12C9.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 10)
            {
                chkConfigurationSetupR12C10.Checked = true;
            }
            if (CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing == 11)
            {
                chkConfigurationSetupR12C11.Checked = true;
            }

            IsConfigurationSetupTempered = false;
            tabPageConfigurationSetup.Refresh();
        }

        private void btnConfigurationSaveConfiguration_Click(object sender, EventArgs e)
        {
            if(ValidateShortCircuitSettings() == false)
            {
                return;
            }

            disableAllAPIButtons();

            SaveConfigurationToFile();

            CommonFunctions.Instance.LoadConfigurationSetupRecipe();
            
            // TestProbe2ConfigResistanceMeasurement            
            byte[] ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtConfigurationSetupCH1Current.Text) ? 0 : int.Parse(txtConfigurationSetupCH1Current.Text));
            TestProbe2ConfigResistanceMeasurement.Ch1BiasCurrentLSB = ByteArray[0];
            TestProbe2ConfigResistanceMeasurement.Ch1BiasCurrentMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtConfigurationSetupCH2Current.Text) ? 0 : int.Parse(txtConfigurationSetupCH2Current.Text));
            TestProbe2ConfigResistanceMeasurement.Ch2BiasCurrentLSB = ByteArray[0];
            TestProbe2ConfigResistanceMeasurement.Ch2BiasCurrentMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtConfigurationSetupCH3Current.Text) ? 0 : int.Parse(txtConfigurationSetupCH3Current.Text));
            TestProbe2ConfigResistanceMeasurement.Ch3BiasCurrentLSB = ByteArray[0];
            TestProbe2ConfigResistanceMeasurement.Ch3BiasCurrentMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtConfigurationSetupCH4Current.Text) ? 0 : int.Parse(txtConfigurationSetupCH4Current.Text));
            TestProbe2ConfigResistanceMeasurement.Ch4BiasCurrentLSB = ByteArray[0];
            TestProbe2ConfigResistanceMeasurement.Ch4BiasCurrentMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtConfigurationSetupCH5Current.Text) ? 0 : int.Parse(txtConfigurationSetupCH5Current.Text));
            TestProbe2ConfigResistanceMeasurement.Ch5BiasCurrentLSB = ByteArray[0];
            TestProbe2ConfigResistanceMeasurement.Ch5BiasCurrentMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtConfigurationSetupCH6Current.Text) ? 0 : int.Parse(txtConfigurationSetupCH6Current.Text));
            TestProbe2ConfigResistanceMeasurement.Ch6BiasCurrentLSB = ByteArray[0];
            TestProbe2ConfigResistanceMeasurement.Ch6BiasCurrentMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtConfigurationSetupAvgCurrentSampleCount.Text) ? 0 : int.Parse(txtConfigurationSetupAvgCurrentSampleCount.Text));
            TestProbe2ConfigResistanceMeasurement.SampleCountForAverage = ByteArray[0];

            byte[] ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe2ConfigResistanceMeasurement);

            TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_config_res_meas_Message_ID, TestProbeAPICommand.HST_config_res_meas_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);
            

            // TestProbe3ConfigCapacitanceMeasurement
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtConfigurationSetupFrequency.Text) ? 0 : int.Parse(txtConfigurationSetupFrequency.Text));
            TestProbe3ConfigCapacitanceMeasurement.FrequencyLSB = ByteArray[0];
            TestProbe3ConfigCapacitanceMeasurement.FrequencyMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtConfigurationSetupBiasVoltage.Text) ? 0 : int.Parse(txtConfigurationSetupBiasVoltage.Text));
            TestProbe3ConfigCapacitanceMeasurement.BiasVoltageLSB = ByteArray[0];
            TestProbe3ConfigCapacitanceMeasurement.BiasVoltageMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtConfigurationSetupPeak2PeakVoltage.Text) ? 0 : int.Parse(txtConfigurationSetupPeak2PeakVoltage.Text));
            TestProbe3ConfigCapacitanceMeasurement.PeakVoltageLSB = ByteArray[0];
            TestProbe3ConfigCapacitanceMeasurement.PeakVoltageMSB = ByteArray[1];

            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtConfigurationSetupAvgVoltageSampleCount.Text) ? 0 : int.Parse(txtConfigurationSetupAvgVoltageSampleCount.Text));
            TestProbe3ConfigCapacitanceMeasurement.SampleCountForAverage = ByteArray[0];

            ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe3ConfigCapacitanceMeasurement);

            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_config_cap_meas_Message_ID, TestProbeAPICommand.HST_config_cap_meas_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);


            // TestProbe4ConfigShortDetection
            TestProbe4ConfigShortDetection.WPlusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.WPlusPairing;
            TestProbe4ConfigShortDetection.WMinusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.WMinusPairing;
            TestProbe4ConfigShortDetection.TAPlusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.TAPlusPairing;
            TestProbe4ConfigShortDetection.TAMinusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.TAMinusPairing;
            TestProbe4ConfigShortDetection.WHPlusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.WHPlusPairing;
            TestProbe4ConfigShortDetection.WHMinusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.WHMinusPairing;
            TestProbe4ConfigShortDetection.RHPlusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.RHPlusPairing;
            TestProbe4ConfigShortDetection.RHMinusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.RHMinusPairing;
            TestProbe4ConfigShortDetection.R1PlusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.R1PlusPairing;
            TestProbe4ConfigShortDetection.R1MinusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.R1MinusPairing;
            TestProbe4ConfigShortDetection.R2PlusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.R2PlusPairing;
            TestProbe4ConfigShortDetection.R2MinusPairing = CommonFunctions.Instance.ConfigurationSetupRecipe.R2MinusPairing;

            ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe4ConfigShortDetection);

            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_config_short_detection_Message_ID, TestProbeAPICommand.HST_config_short_detection_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);


            // TestProbe5MeasurementChannelEnable
            TestProbe5MeasurementChannelEnable.ResistanceCh1Writer = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh1Writer;
            TestProbe5MeasurementChannelEnable.ResistanceCh2TA = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh2TA;
            TestProbe5MeasurementChannelEnable.ResistanceCh3WriteHeater = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh3WriteHeater;
            TestProbe5MeasurementChannelEnable.ResistanceCh4ReadHeater = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh4ReadHeater;
            TestProbe5MeasurementChannelEnable.ResistanceCh5Read1 = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh5Read1;
            TestProbe5MeasurementChannelEnable.ResistanceCh6Read2 = CommonFunctions.Instance.ConfigurationSetupRecipe.ResistanceCh6Read2;
            TestProbe5MeasurementChannelEnable.CapacitanceCh1 = CommonFunctions.Instance.ConfigurationSetupRecipe.CapacitanceCh1;
            TestProbe5MeasurementChannelEnable.CapacitanceCh2 = CommonFunctions.Instance.ConfigurationSetupRecipe.CapacitanceCh2;

            ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe5MeasurementChannelEnable);

            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_meas_channel_enable_Message_ID, TestProbeAPICommand.HST_meas_channel_enable_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);


            // TestProbe6HGAEnable
            TestProbe6HGAEnable.HGA1 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA1;
            TestProbe6HGAEnable.HGA2 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA2;
            TestProbe6HGAEnable.HGA3 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA3;
            TestProbe6HGAEnable.HGA4 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA4;
            TestProbe6HGAEnable.HGA5 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA5;
            TestProbe6HGAEnable.HGA6 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA6;
            TestProbe6HGAEnable.HGA7 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA7;
            TestProbe6HGAEnable.HGA8 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA8;
            TestProbe6HGAEnable.HGA9 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA9;
            TestProbe6HGAEnable.HGA10 = CommonFunctions.Instance.ConfigurationSetupRecipe.HGA10;

            ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe6HGAEnable);

            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_hga_enable_Message_ID, TestProbeAPICommand.HST_hga_enable_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            
            // TestProbe32ConfigTemperatureMeasurement
            ByteArray = BitConverter.GetBytes(string.IsNullOrEmpty(txtConfigurationSetupTempTimeConstant.Text) ? 0 : int.Parse(txtConfigurationSetupTempTimeConstant.Text));
            TestProbe32ConfigTemperatureMeasurement.TimeConstant = ByteArray[0];

            ByteArrayFromStructure = CommonFunctions.Instance.StructureToByteArray(TestProbe32ConfigTemperatureMeasurement);

            APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_config_temp_meas_Message_ID, TestProbeAPICommand.HST_config_temp_meas_Message_Size, ByteArrayFromStructure);
            CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

            bool commandSentToMicroprocessor = constructAndSendWriteDataBuffer();

            enableAllAPIButtons();            

            if (commandSentToMicroprocessor)
            {
                IsConfigurationSetupTempered = false;
                tabPageConfigurationSetup.Refresh();

                Notify.PopUp("Command Sent To uProcessor", "Configuration saved.", null, "OK");
            }
        }

        private bool IsValidShortDetectionConfiguration(out bool IsRow, out int RowColumnNumber)
        {
            bool ValidShortCircuitConfiguration = true;
            IsRow = true;
            RowColumnNumber = 0;

            //Row1
            int row1CheckedCount = 0;
            if (chkConfigurationSetupR1C2.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C3.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C4.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C5.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C6.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C7.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C8.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C9.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C10.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C11.Checked)
            {
                row1CheckedCount++;
            }
            if (chkConfigurationSetupR1C12.Checked)
            {
                row1CheckedCount++;
            }

            //Row2
            int row2CheckedCount = 0;
            if (chkConfigurationSetupR2C1.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C3.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C4.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C5.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C6.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C7.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C8.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C9.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C10.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C11.Checked)
            {
                row2CheckedCount++;
            }
            if (chkConfigurationSetupR2C12.Checked)
            {
                row2CheckedCount++;
            }

            //Row3
            int row3CheckedCount = 0;
            if (chkConfigurationSetupR3C1.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C2.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C4.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C5.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C6.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C7.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C8.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C9.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C10.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C11.Checked)
            {
                row3CheckedCount++;
            }
            if (chkConfigurationSetupR3C12.Checked)
            {
                row3CheckedCount++;
            }

            //Row4
            int row4CheckedCount = 0;
            if (chkConfigurationSetupR4C1.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C2.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C3.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C5.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C6.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C7.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C8.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C9.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C10.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C11.Checked)
            {
                row4CheckedCount++;
            }
            if (chkConfigurationSetupR4C12.Checked)
            {
                row4CheckedCount++;
            }

            //Row5
            int row5CheckedCount = 0;
            if (chkConfigurationSetupR5C1.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C2.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C3.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C4.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C6.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C7.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C8.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C9.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C10.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C11.Checked)
            {
                row5CheckedCount++;
            }
            if (chkConfigurationSetupR5C12.Checked)
            {
                row5CheckedCount++;
            }

            //Row6
            int row6CheckedCount = 0;
            if (chkConfigurationSetupR6C1.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C2.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C3.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C4.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C5.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C7.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C8.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C9.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C10.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C11.Checked)
            {
                row6CheckedCount++;
            }
            if (chkConfigurationSetupR6C12.Checked)
            {
                row6CheckedCount++;
            }

            //Row7
            int row7CheckedCount = 0;
            if (chkConfigurationSetupR7C1.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C2.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C3.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C4.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C5.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C6.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C8.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C9.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C10.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C11.Checked)
            {
                row7CheckedCount++;
            }
            if (chkConfigurationSetupR7C12.Checked)
            {
                row7CheckedCount++;
            }

            //Row8
            int row8CheckedCount = 0;
            if (chkConfigurationSetupR8C1.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C2.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C3.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C4.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C5.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C6.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C7.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C9.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C10.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C11.Checked)
            {
                row8CheckedCount++;
            }
            if (chkConfigurationSetupR8C12.Checked)
            {
                row8CheckedCount++;
            }

            //Row9
            int row9CheckedCount = 0;
            if (chkConfigurationSetupR9C1.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C2.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C3.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C4.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C5.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C6.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C7.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C8.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C10.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C11.Checked)
            {
                row9CheckedCount++;
            }
            if (chkConfigurationSetupR9C12.Checked)
            {
                row9CheckedCount++;
            }

            //Row10
            int row10CheckedCount = 0;
            if (chkConfigurationSetupR10C1.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C2.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C3.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C4.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C5.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C6.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C7.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C8.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C9.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C11.Checked)
            {
                row10CheckedCount++;
            }
            if (chkConfigurationSetupR10C12.Checked)
            {
                row10CheckedCount++;
            }

            //Row11
            int row11CheckedCount = 0;
            if (chkConfigurationSetupR11C1.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C2.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C3.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C4.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C5.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C6.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C7.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C8.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C9.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C10.Checked)
            {
                row11CheckedCount++;
            }
            if (chkConfigurationSetupR11C12.Checked)
            {
                row11CheckedCount++;
            }

            //Row12
            int row12CheckedCount = 0;
            if (chkConfigurationSetupR12C1.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C2.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C3.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C4.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C5.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C6.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C7.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C8.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C9.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C10.Checked)
            {
                row12CheckedCount++;
            }
            if (chkConfigurationSetupR12C11.Checked)
            {
                row12CheckedCount++;
            }


            if (row1CheckedCount > 1 || row2CheckedCount > 1 || row3CheckedCount > 1 || row4CheckedCount > 1 || row5CheckedCount > 1
                || row6CheckedCount > 1 || row7CheckedCount > 1 || row8CheckedCount > 1 || row9CheckedCount > 1 || row10CheckedCount > 1
                || row11CheckedCount > 1 || row12CheckedCount > 1)
            {
                ValidShortCircuitConfiguration = false;
                IsRow = true;

                if(row1CheckedCount > 1)
                {
                    RowColumnNumber = 1;
                }
                else if (row2CheckedCount > 1)
                {
                    RowColumnNumber = 2;
                }
                else if (row3CheckedCount > 1)
                {
                    RowColumnNumber = 3;
                }
                else if (row4CheckedCount > 1)
                {
                    RowColumnNumber = 4;
                }
                else if (row5CheckedCount > 1)
                {
                    RowColumnNumber = 5;
                }
                else if (row6CheckedCount > 1)
                {
                    RowColumnNumber = 6;
                }
                else if (row7CheckedCount > 1)
                {
                    RowColumnNumber = 7;
                }
                else if (row8CheckedCount > 1)
                {
                    RowColumnNumber = 8;
                }
                else if (row9CheckedCount > 1)
                {
                    RowColumnNumber = 9;
                }
                else if (row10CheckedCount > 1)
                {
                    RowColumnNumber = 10;
                }
                else if (row11CheckedCount > 1)
                {
                    RowColumnNumber = 11;
                }
                else
                {
                    RowColumnNumber = 12;
                }
            }


            //Column1
            int column1CheckedCount = 0;
            if (chkConfigurationSetupR2C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR3C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR4C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR5C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR6C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR7C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR8C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR9C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR10C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR11C1.Checked)
            {
                column1CheckedCount++;
            }
            if (chkConfigurationSetupR12C1.Checked)
            {
                column1CheckedCount++;
            }

            //Column2
            int column2CheckedCount = 0;
            if (chkConfigurationSetupR1C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR3C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR4C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR5C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR6C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR7C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR8C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR9C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR10C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR11C2.Checked)
            {
                column2CheckedCount++;
            }
            if (chkConfigurationSetupR12C2.Checked)
            {
                column2CheckedCount++;
            }

            //Column3
            int column3CheckedCount = 0;
            if (chkConfigurationSetupR1C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR2C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR4C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR5C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR6C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR7C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR8C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR9C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR10C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR11C3.Checked)
            {
                column3CheckedCount++;
            }
            if (chkConfigurationSetupR12C3.Checked)
            {
                column3CheckedCount++;
            }

            //Column4
            int column4CheckedCount = 0;
            if (chkConfigurationSetupR1C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR2C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR3C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR5C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR6C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR7C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR8C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR9C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR10C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR11C4.Checked)
            {
                column4CheckedCount++;
            }
            if (chkConfigurationSetupR12C4.Checked)
            {
                column4CheckedCount++;
            }

            //Column5
            int column5CheckedCount = 0;
            if (chkConfigurationSetupR1C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR2C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR3C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR4C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR6C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR7C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR8C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR9C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR10C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR11C5.Checked)
            {
                column5CheckedCount++;
            }
            if (chkConfigurationSetupR12C5.Checked)
            {
                column5CheckedCount++;
            }

            //Column6
            int column6CheckedCount = 0;
            if (chkConfigurationSetupR1C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR2C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR3C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR4C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR5C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR7C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR8C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR9C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR10C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR11C6.Checked)
            {
                column6CheckedCount++;
            }
            if (chkConfigurationSetupR12C6.Checked)
            {
                column6CheckedCount++;
            }

            //Column7
            int column7CheckedCount = 0;
            if (chkConfigurationSetupR1C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR2C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR3C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR4C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR5C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR6C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR8C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR9C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR10C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR11C7.Checked)
            {
                column7CheckedCount++;
            }
            if (chkConfigurationSetupR12C7.Checked)
            {
                column7CheckedCount++;
            }

            //Column8
            int column8CheckedCount = 0;
            if (chkConfigurationSetupR1C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR2C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR3C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR4C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR5C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR6C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR7C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR9C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR10C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR11C8.Checked)
            {
                column8CheckedCount++;
            }
            if (chkConfigurationSetupR12C8.Checked)
            {
                column8CheckedCount++;
            }

            //Column9
            int column9CheckedCount = 0;
            if (chkConfigurationSetupR1C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR2C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR3C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR4C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR5C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR6C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR7C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR8C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR10C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR11C9.Checked)
            {
                column9CheckedCount++;
            }
            if (chkConfigurationSetupR12C9.Checked)
            {
                column9CheckedCount++;
            }

            //Column10
            int column10CheckedCount = 0;
            if (chkConfigurationSetupR1C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR2C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR3C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR4C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR5C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR6C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR7C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR8C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR9C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR11C10.Checked)
            {
                column10CheckedCount++;
            }
            if (chkConfigurationSetupR12C10.Checked)
            {
                column10CheckedCount++;
            }

            //Column11
            int column11CheckedCount = 0;
            if (chkConfigurationSetupR1C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR2C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR3C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR4C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR5C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR6C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR7C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR8C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR9C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR10C11.Checked)
            {
                column11CheckedCount++;
            }
            if (chkConfigurationSetupR12C11.Checked)
            {
                column11CheckedCount++;
            }

            //Column12
            int column12CheckedCount = 0;
            if (chkConfigurationSetupR1C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR2C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR3C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR4C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR5C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR6C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR7C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR8C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR9C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR10C12.Checked)
            {
                column12CheckedCount++;
            }
            if (chkConfigurationSetupR11C12.Checked)
            {
                column12CheckedCount++;
            }

            if (column1CheckedCount > 2 || column2CheckedCount > 2 || column3CheckedCount > 2 || column4CheckedCount > 2 || column5CheckedCount > 2
                || column6CheckedCount > 2 || column7CheckedCount > 2 || column8CheckedCount > 2 || column9CheckedCount > 2 || column10CheckedCount > 2
                || column11CheckedCount > 2 || column12CheckedCount > 2)
            {
                ValidShortCircuitConfiguration = false;
                IsRow = false;

                if (column1CheckedCount > 2)
                {
                    RowColumnNumber = 1;
                }
                else if (column2CheckedCount > 2)
                {
                    RowColumnNumber = 2;
                }
                else if (column3CheckedCount > 2)
                {
                    RowColumnNumber = 3;
                }
                else if (column4CheckedCount > 2)
                {
                    RowColumnNumber = 4;
                }
                else if (column5CheckedCount > 2)
                {
                    RowColumnNumber = 5;
                }
                else if (column6CheckedCount > 2)
                {
                    RowColumnNumber = 6;
                }
                else if (column7CheckedCount > 2)
                {
                    RowColumnNumber = 7;
                }
                else if (column8CheckedCount > 2)
                {
                    RowColumnNumber = 8;
                }
                else if (column9CheckedCount > 2)
                {
                    RowColumnNumber = 9;
                }
                else if (column10CheckedCount > 2)
                {
                    RowColumnNumber = 10;
                }
                else if (column11CheckedCount > 2)
                {
                    RowColumnNumber = 11;
                }
                else
                {
                    RowColumnNumber = 12;
                }
            }

            return ValidShortCircuitConfiguration;
        }

        private bool ValidateShortCircuitSettings()
        {
            bool ValidShortCircuitSettings = true;
            bool IsRow = true;
            int RowColumnNumber = 0;
            string RowColumnHeading = "";

            if (IsValidShortDetectionConfiguration(out IsRow, out RowColumnNumber) == false)
            {
                switch (RowColumnNumber)
                {
                    case 1:
                        RowColumnHeading = "W+";
                        break;
                    case 2:
                        RowColumnHeading = "W-";
                        break;
                    case 3:
                        RowColumnHeading = "TA+";
                        break;
                    case 4:
                        RowColumnHeading = "TA-";
                        break;
                    case 5:
                        RowColumnHeading = "wH+";
                        break;
                    case 6:
                        RowColumnHeading = "wH-";
                        break;
                    case 7:
                        RowColumnHeading = "rH+";
                        break;
                    case 8:
                        RowColumnHeading = "rH-";
                        break;
                    case 9:
                        RowColumnHeading = "R1+";
                        break;
                    case 10:
                        RowColumnHeading = "R1-";
                        break;
                    case 11:
                        RowColumnHeading = "R2+";
                        break;
                    case 12:
                        RowColumnHeading = "R2-";
                        break;
                }

                if (IsRow)
                {
                    Notify.PopUpError("Configuration Not Saved", String.Format("Invalid short-circuit detection settings found due to row {0} with heading of '{1}' has more than one check-box checked. Configuration not saved.", RowColumnNumber, RowColumnHeading));                    
                }
                else
                {
                    Notify.PopUpError("Configuration Not Saved", String.Format("Invalid short-circuit detection settings found due to column {0} with heading of '{1}' has more than two check-boxes checked.", RowColumnNumber, RowColumnHeading));
                }

                ValidShortCircuitSettings = false;
            }
            return ValidShortCircuitSettings;
        }

        private void SaveConfigurationToFile()
        {
            string ConfigurationSetupRecipeFilePath = string.Format("{0}ConfigurationSetupRecipe.rcp", CommonFunctions.Instance.RecipeFileDirectory);
            if (!Directory.Exists(CommonFunctions.Instance.RecipeFileDirectory))
            {
                Directory.CreateDirectory(CommonFunctions.Instance.RecipeFileDirectory);
            }

            Log.Info("Startup", "Saving recipe for configuration & setup to {0}.", ConfigurationSetupRecipeFilePath);

            SettingsXml _xml = new SettingsXml(ConfigurationSetupRecipeFilePath);

            _xml.OpenSection("Resistance");
            // Bias Current
            _xml.Write("BiasCurrentChannel1", txtConfigurationSetupCH1Current.Text);
            _xml.Write("BiasCurrentChannel2", txtConfigurationSetupCH2Current.Text);
            _xml.Write("BiasCurrentChannel3", txtConfigurationSetupCH3Current.Text);
            _xml.Write("BiasCurrentChannel4", txtConfigurationSetupCH4Current.Text);
            _xml.Write("BiasCurrentChannel5", txtConfigurationSetupCH5Current.Text);
            _xml.Write("BiasCurrentChannel6", txtConfigurationSetupCH6Current.Text);
            _xml.Write("ResistanceSampleCount", txtConfigurationSetupAvgCurrentSampleCount.Text);
            _xml.CloseSection();

            _xml.OpenSection("Temperature");
            // Temperature
            _xml.Write("TemperatureTimeConstant", txtConfigurationSetupTempTimeConstant.Text);
            _xml.CloseSection();

            _xml.OpenSection("Capacitance");
            // Capacitance
            _xml.Write("Frequency", txtConfigurationSetupFrequency.Text);
            _xml.Write("BiasVoltage", txtConfigurationSetupBiasVoltage.Text);
            _xml.Write("Peak2PeakVoltage", txtConfigurationSetupPeak2PeakVoltage.Text);
            _xml.Write("CapacitanceSampleCount", txtConfigurationSetupAvgVoltageSampleCount.Text);
            _xml.CloseSection();

            _xml.OpenSection("EnableHGAChannel");
            // Enable HGA Channel
            _xml.Write("EnableHGAChannel1", (chkConfigurationSetupCh1.Checked == true));
            _xml.Write("EnableHGAChannel2", (chkConfigurationSetupCh2.Checked == true));
            _xml.Write("EnableHGAChannel3", (chkConfigurationSetupCh3.Checked == true));
            _xml.Write("EnableHGAChannel4", (chkConfigurationSetupCh4.Checked == true));
            _xml.Write("EnableHGAChannel5", (chkConfigurationSetupCh5.Checked == true));
            _xml.Write("EnableHGAChannel6", (chkConfigurationSetupCh6.Checked == true));
            _xml.Write("EnableHGACapacitance1", (chkConfigurationSetupCapa1.Checked == true));
            _xml.Write("EnableHGACapacitance2", (chkConfigurationSetupCapa2.Checked == true));
            _xml.CloseSection();


            _xml.OpenSection("EnableHGA");
            // Enable HGA
            _xml.Write("EnableHGA1", (chkConfigurationSetupHGA1.Checked == true));
            _xml.Write("EnableHGA2", (chkConfigurationSetupHGA2.Checked == true));
            _xml.Write("EnableHGA3", (chkConfigurationSetupHGA3.Checked == true));
            _xml.Write("EnableHGA4", (chkConfigurationSetupHGA4.Checked == true));
            _xml.Write("EnableHGA5", (chkConfigurationSetupHGA5.Checked == true));
            _xml.Write("EnableHGA6", (chkConfigurationSetupHGA6.Checked == true));
            _xml.Write("EnableHGA7", (chkConfigurationSetupHGA7.Checked == true));
            _xml.Write("EnableHGA8", (chkConfigurationSetupHGA8.Checked == true));
            _xml.Write("EnableHGA9", (chkConfigurationSetupHGA9.Checked == true));
            _xml.Write("EnableHGA10", (chkConfigurationSetupHGA10.Checked == true));
            _xml.CloseSection();

            _xml.OpenSection("ShortCircuitDetection");
            // Short Circuit Detection
            //Row1
            _xml.Write("EnableR1C2", (chkConfigurationSetupR1C2.Checked == true));
            _xml.Write("EnableR1C3", (chkConfigurationSetupR1C3.Checked == true));
            _xml.Write("EnableR1C4", (chkConfigurationSetupR1C4.Checked == true));
            _xml.Write("EnableR1C5", (chkConfigurationSetupR1C5.Checked == true));
            _xml.Write("EnableR1C6", (chkConfigurationSetupR1C6.Checked == true));
            _xml.Write("EnableR1C7", (chkConfigurationSetupR1C7.Checked == true));
            _xml.Write("EnableR1C8", (chkConfigurationSetupR1C8.Checked == true));
            _xml.Write("EnableR1C9", (chkConfigurationSetupR1C9.Checked == true));
            _xml.Write("EnableR1C10", (chkConfigurationSetupR1C10.Checked == true));
            _xml.Write("EnableR1C11", (chkConfigurationSetupR1C11.Checked == true));
            _xml.Write("EnableR1C12", (chkConfigurationSetupR1C12.Checked == true));

            //Row2
            _xml.Write("EnableR2C1", (chkConfigurationSetupR2C1.Checked == true));
            _xml.Write("EnableR2C3", (chkConfigurationSetupR2C3.Checked == true));
            _xml.Write("EnableR2C4", (chkConfigurationSetupR2C4.Checked == true));
            _xml.Write("EnableR2C5", (chkConfigurationSetupR2C5.Checked == true));
            _xml.Write("EnableR2C6", (chkConfigurationSetupR2C6.Checked == true));
            _xml.Write("EnableR2C7", (chkConfigurationSetupR2C7.Checked == true));
            _xml.Write("EnableR2C8", (chkConfigurationSetupR2C8.Checked == true));
            _xml.Write("EnableR2C9", (chkConfigurationSetupR2C9.Checked == true));
            _xml.Write("EnableR2C10", (chkConfigurationSetupR2C10.Checked == true));
            _xml.Write("EnableR2C11", (chkConfigurationSetupR2C11.Checked == true));
            _xml.Write("EnableR2C12", (chkConfigurationSetupR2C12.Checked == true));

            //Row3
            _xml.Write("EnableR3C1", (chkConfigurationSetupR3C1.Checked == true));
            _xml.Write("EnableR3C2", (chkConfigurationSetupR3C2.Checked == true));
            _xml.Write("EnableR3C4", (chkConfigurationSetupR3C4.Checked == true));
            _xml.Write("EnableR3C5", (chkConfigurationSetupR3C5.Checked == true));
            _xml.Write("EnableR3C6", (chkConfigurationSetupR3C6.Checked == true));
            _xml.Write("EnableR3C7", (chkConfigurationSetupR3C7.Checked == true));
            _xml.Write("EnableR3C8", (chkConfigurationSetupR3C8.Checked == true));
            _xml.Write("EnableR3C9", (chkConfigurationSetupR3C9.Checked == true));
            _xml.Write("EnableR3C10", (chkConfigurationSetupR3C10.Checked == true));
            _xml.Write("EnableR3C11", (chkConfigurationSetupR3C11.Checked == true));
            _xml.Write("EnableR3C12", (chkConfigurationSetupR3C12.Checked == true));

            //Row4
            _xml.Write("EnableR4C1", (chkConfigurationSetupR4C1.Checked == true));
            _xml.Write("EnableR4C2", (chkConfigurationSetupR4C2.Checked == true));
            _xml.Write("EnableR4C3", (chkConfigurationSetupR4C3.Checked == true));
            _xml.Write("EnableR4C5", (chkConfigurationSetupR4C5.Checked == true));
            _xml.Write("EnableR4C6", (chkConfigurationSetupR4C6.Checked == true));
            _xml.Write("EnableR4C7", (chkConfigurationSetupR4C7.Checked == true));
            _xml.Write("EnableR4C8", (chkConfigurationSetupR4C8.Checked == true));
            _xml.Write("EnableR4C9", (chkConfigurationSetupR4C9.Checked == true));
            _xml.Write("EnableR4C10", (chkConfigurationSetupR4C10.Checked == true));
            _xml.Write("EnableR4C11", (chkConfigurationSetupR4C11.Checked == true));
            _xml.Write("EnableR4C12", (chkConfigurationSetupR4C12.Checked == true));

            //Row5
            _xml.Write("EnableR5C1", (chkConfigurationSetupR5C1.Checked == true));
            _xml.Write("EnableR5C2", (chkConfigurationSetupR5C2.Checked == true));
            _xml.Write("EnableR5C3", (chkConfigurationSetupR5C3.Checked == true));
            _xml.Write("EnableR5C4", (chkConfigurationSetupR5C4.Checked == true));
            _xml.Write("EnableR5C6", (chkConfigurationSetupR5C6.Checked == true));
            _xml.Write("EnableR5C7", (chkConfigurationSetupR5C7.Checked == true));
            _xml.Write("EnableR5C8", (chkConfigurationSetupR5C8.Checked == true));
            _xml.Write("EnableR5C9", (chkConfigurationSetupR5C9.Checked == true));
            _xml.Write("EnableR5C10", (chkConfigurationSetupR5C10.Checked == true));
            _xml.Write("EnableR5C11", (chkConfigurationSetupR5C11.Checked == true));
            _xml.Write("EnableR5C12", (chkConfigurationSetupR5C12.Checked == true));

            //Row6
            _xml.Write("EnableR6C1", (chkConfigurationSetupR6C1.Checked == true));
            _xml.Write("EnableR6C2", (chkConfigurationSetupR6C2.Checked == true));
            _xml.Write("EnableR6C3", (chkConfigurationSetupR6C3.Checked == true));
            _xml.Write("EnableR6C4", (chkConfigurationSetupR6C4.Checked == true));
            _xml.Write("EnableR6C5", (chkConfigurationSetupR6C5.Checked == true));
            _xml.Write("EnableR6C7", (chkConfigurationSetupR6C7.Checked == true));
            _xml.Write("EnableR6C8", (chkConfigurationSetupR6C8.Checked == true));
            _xml.Write("EnableR6C9", (chkConfigurationSetupR6C9.Checked == true));
            _xml.Write("EnableR6C10", (chkConfigurationSetupR6C10.Checked == true));
            _xml.Write("EnableR6C11", (chkConfigurationSetupR6C11.Checked == true));
            _xml.Write("EnableR6C12", (chkConfigurationSetupR6C12.Checked == true));

            //Row7
            _xml.Write("EnableR7C1", (chkConfigurationSetupR7C1.Checked == true));
            _xml.Write("EnableR7C2", (chkConfigurationSetupR7C2.Checked == true));
            _xml.Write("EnableR7C3", (chkConfigurationSetupR7C3.Checked == true));
            _xml.Write("EnableR7C4", (chkConfigurationSetupR7C4.Checked == true));
            _xml.Write("EnableR7C5", (chkConfigurationSetupR7C5.Checked == true));
            _xml.Write("EnableR7C6", (chkConfigurationSetupR7C6.Checked == true));
            _xml.Write("EnableR7C8", (chkConfigurationSetupR7C8.Checked == true));
            _xml.Write("EnableR7C9", (chkConfigurationSetupR7C9.Checked == true));
            _xml.Write("EnableR7C10", (chkConfigurationSetupR7C10.Checked == true));
            _xml.Write("EnableR7C11", (chkConfigurationSetupR7C11.Checked == true));
            _xml.Write("EnableR7C12", (chkConfigurationSetupR7C12.Checked == true));

            //Row8
            _xml.Write("EnableR8C1", (chkConfigurationSetupR8C1.Checked == true));
            _xml.Write("EnableR8C2", (chkConfigurationSetupR8C2.Checked == true));
            _xml.Write("EnableR8C3", (chkConfigurationSetupR8C3.Checked == true));
            _xml.Write("EnableR8C4", (chkConfigurationSetupR8C4.Checked == true));
            _xml.Write("EnableR8C5", (chkConfigurationSetupR8C5.Checked == true));
            _xml.Write("EnableR8C6", (chkConfigurationSetupR8C6.Checked == true));
            _xml.Write("EnableR8C7", (chkConfigurationSetupR8C7.Checked == true));
            _xml.Write("EnableR8C9", (chkConfigurationSetupR8C9.Checked == true));
            _xml.Write("EnableR8C10", (chkConfigurationSetupR8C10.Checked == true));
            _xml.Write("EnableR8C11", (chkConfigurationSetupR8C11.Checked == true));
            _xml.Write("EnableR8C12", (chkConfigurationSetupR8C12.Checked == true));

            //Row9
            _xml.Write("EnableR9C1", (chkConfigurationSetupR9C1.Checked == true));
            _xml.Write("EnableR9C2", (chkConfigurationSetupR9C2.Checked == true));
            _xml.Write("EnableR9C3", (chkConfigurationSetupR9C3.Checked == true));
            _xml.Write("EnableR9C4", (chkConfigurationSetupR9C4.Checked == true));
            _xml.Write("EnableR9C5", (chkConfigurationSetupR9C5.Checked == true));
            _xml.Write("EnableR9C6", (chkConfigurationSetupR9C6.Checked == true));
            _xml.Write("EnableR9C7", (chkConfigurationSetupR9C7.Checked == true));
            _xml.Write("EnableR9C8", (chkConfigurationSetupR9C8.Checked == true));
            _xml.Write("EnableR9C10", (chkConfigurationSetupR9C10.Checked == true));
            _xml.Write("EnableR9C11", (chkConfigurationSetupR9C11.Checked == true));
            _xml.Write("EnableR9C12", (chkConfigurationSetupR9C12.Checked == true));

            //Row10
            _xml.Write("EnableR10C1", (chkConfigurationSetupR10C1.Checked == true));
            _xml.Write("EnableR10C2", (chkConfigurationSetupR10C2.Checked == true));
            _xml.Write("EnableR10C3", (chkConfigurationSetupR10C3.Checked == true));
            _xml.Write("EnableR10C4", (chkConfigurationSetupR10C4.Checked == true));
            _xml.Write("EnableR10C5", (chkConfigurationSetupR10C5.Checked == true));
            _xml.Write("EnableR10C6", (chkConfigurationSetupR10C6.Checked == true));
            _xml.Write("EnableR10C7", (chkConfigurationSetupR10C7.Checked == true));
            _xml.Write("EnableR10C8", (chkConfigurationSetupR10C8.Checked == true));
            _xml.Write("EnableR10C9", (chkConfigurationSetupR10C9.Checked == true));
            _xml.Write("EnableR10C11", (chkConfigurationSetupR10C11.Checked == true));
            _xml.Write("EnableR10C12", (chkConfigurationSetupR10C12.Checked == true));

            //Row11
            _xml.Write("EnableR11C1", (chkConfigurationSetupR11C1.Checked == true));
            _xml.Write("EnableR11C2", (chkConfigurationSetupR11C2.Checked == true));
            _xml.Write("EnableR11C3", (chkConfigurationSetupR11C3.Checked == true));
            _xml.Write("EnableR11C4", (chkConfigurationSetupR11C4.Checked == true));
            _xml.Write("EnableR11C5", (chkConfigurationSetupR11C5.Checked == true));
            _xml.Write("EnableR11C6", (chkConfigurationSetupR11C6.Checked == true));
            _xml.Write("EnableR11C7", (chkConfigurationSetupR11C7.Checked == true));
            _xml.Write("EnableR11C8", (chkConfigurationSetupR11C8.Checked == true));
            _xml.Write("EnableR11C9", (chkConfigurationSetupR11C9.Checked == true));
            _xml.Write("EnableR11C10", (chkConfigurationSetupR11C10.Checked == true));
            _xml.Write("EnableR11C12", (chkConfigurationSetupR11C12.Checked == true));

            //Row12
            _xml.Write("EnableR12C1", (chkConfigurationSetupR12C1.Checked == true));
            _xml.Write("EnableR12C2", (chkConfigurationSetupR12C2.Checked == true));
            _xml.Write("EnableR12C3", (chkConfigurationSetupR12C3.Checked == true));
            _xml.Write("EnableR12C4", (chkConfigurationSetupR12C4.Checked == true));
            _xml.Write("EnableR12C5", (chkConfigurationSetupR12C5.Checked == true));
            _xml.Write("EnableR12C6", (chkConfigurationSetupR12C6.Checked == true));
            _xml.Write("EnableR12C7", (chkConfigurationSetupR12C7.Checked == true));
            _xml.Write("EnableR12C8", (chkConfigurationSetupR12C8.Checked == true));
            _xml.Write("EnableR12C9", (chkConfigurationSetupR12C9.Checked == true));
            _xml.Write("EnableR12C10", (chkConfigurationSetupR12C10.Checked == true));
            _xml.Write("EnableR12C11", (chkConfigurationSetupR12C11.Checked == true));
            _xml.CloseSection();

            _xml.Save();
        }
	}
}