using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using XyratexOSC.UI;
using System.Windows.Forms;
using Seagate.AAS.HGA.HST.Settings;
using Seagate.AAS.HGA.HST.Machine;
using Seagate.AAS.HGA.HST.Data;
using Seagate.AAS.HGA.HST.UI;
using Seagate.AAS.HGA.HST.Data.IncomingTestProbeData;
using Seagate.AAS.HGA.HST.Data.OutgoingTestProbeData;
using Seagate.AAS.HGA.HST.Utils;

namespace Seagate.AAS.HGA.HST.UI
{
    public partial class ProductTypeSelectionForm : Form
    {        
        public string ProductID
        {
            get;
            set;
        }  

        public ProductTypeSelectionForm()
        {
            InitializeComponent();

            if (HSTMachine.Workcell.HSTSettings.Install.OperationMode != OperationMode.Simulation)
            {
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Clear();
                TestProbeAPICommand APICommand = new TestProbeAPICommand(TestProbeAPICommand.HST_get_conversion_board_id_Message_ID, TestProbeAPICommand.HST_get_conversion_board_id_Message_Name, TestProbeAPICommand.HST_get_conversion_board_id_Message_Size, null);
                CommonFunctions.Instance.OutgoingTestProbeDataAPIs.Enqueue(APICommand);

                HSTMachine.Instance.MainForm.constructAndSendWriteDataBuffer(false); 
            }           
        }

        public static LoadProductTypeResults ShowLoadForm()
        {
            LoadProductTypeResults results = new LoadProductTypeResults();
            
            UIUtility.Invoke(Application.OpenForms[0], () =>
            {
                using (ProductTypeSelectionForm form = new ProductTypeSelectionForm())
                {                    
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        results.ProductID = form.lstProductType.SelectedItems[0].SubItems[0].Text;
                        results.ProductName = form.lstProductType.SelectedItems[0].SubItems[0].Text;
                        results.ConversionBoardID = form.lstProductType.SelectedItems[0].SubItems[1].Text;
                    }
                }
            });

            return results;
        }

        private void btnOK_Click_1(object sender, EventArgs e)
        {
            if (lstProductType.SelectedItems.Count == 0)
            {
                this.errorProvider1.SetError(lstProductType, "No product type has been selected");
                return;
            }
            if (String.IsNullOrEmpty(txtBoxProductID.Text))
            {
                this.errorProvider1.SetError(txtBoxProductID, "Product ID is empty");
                return;
            }

            if (!IsTestProbeTypeCompatible())
                return;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            ProductID = null;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void lstProductType_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            ListView listView = (ListView)sender;

            // Check if e.Item is selected and the ListView has a focus.
            if (!listView.Focused && e.Item.Selected)
            {
                Rectangle rowBounds = e.Bounds;
                int leftMargin = e.Item.GetBounds(ItemBoundsPortion.Label).Left;
                Rectangle bounds = new Rectangle(leftMargin, rowBounds.Top, rowBounds.Width - leftMargin, rowBounds.Height);
                e.Graphics.FillRectangle(SystemBrushes.Highlight, bounds);
            }
            else
                e.DrawDefault = true;
        }

        private void lstProductType_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void lstProductType_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            const int TEXT_OFFSET = 1;

            ListView listView = (ListView)sender;

            // Check if e.Item is selected and the ListView has a focus.
            if (!listView.Focused && e.Item.Selected)
            {
                Rectangle rowBounds = e.SubItem.Bounds;
                Rectangle labelBounds = e.Item.GetBounds(ItemBoundsPortion.Label);
                int leftMargin = labelBounds.Left - TEXT_OFFSET;
                Rectangle bounds = new Rectangle(rowBounds.Left + leftMargin, rowBounds.Top, e.ColumnIndex == 0 ? labelBounds.Width : (rowBounds.Width - leftMargin - TEXT_OFFSET), rowBounds.Height);
                TextFormatFlags align;

                switch (listView.Columns[e.ColumnIndex].TextAlign)
                {
                    case HorizontalAlignment.Right:
                        align = TextFormatFlags.Right;
                        break;
                    case HorizontalAlignment.Center:
                        align = TextFormatFlags.HorizontalCenter;
                        break;
                    default:
                        align = TextFormatFlags.Left;
                        break;
                }

                TextRenderer.DrawText(e.Graphics, e.SubItem.Text, listView.Font, bounds, SystemColors.HighlightText,
                    align | TextFormatFlags.SingleLine | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);
            }
            else
                e.DrawDefault = true;
        }

        private void lstProductType_DoubleClick(object sender, EventArgs e)
        {
            if (!IsTestProbeTypeCompatible())
                return;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void lstProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstProductType.SelectedItems.Count > 0)
            {
                txtBoxProductID.Text = lstProductType.SelectedItems[0].Text;
            }
        }
        
        private bool IsTestProbeTypeCompatible()
        {
            //remark for one recipe
            //foreach (HGAProductType HGAProductType in CalibrationSettings.Instance.HgaProducts.HGAProduct)
            //{
            //    //if (HGAProductType.ProductID == int.Parse(lstProductType.SelectedItems[0].SubItems[0].Text)) //change for one recipe
            //    if (HGAProductType.Name == lstProductType.SelectedItems[0].SubItems[0].Text)
            //    {
            //        if (CalibrationSettings.Instance.MeasurementTest.CurrentInstalledTestProbeType == CommonFunctions.UNKNOWN)
            //        {
            //            Notify.PopUp("Unknown test probe type", "The current installed test probe is Unknown." +
            //                "\n\nPlease ask Admin or Maintenance Engineer to ensure the installed test probe matched the Test Probe type setting in the 'hardware setting'->'Test electronics' page.", "", "OK");
            //            return false;
            //        }
            //        else if (HGAProductType.TestProbeType != CalibrationSettings.Instance.MeasurementTest.CurrentInstalledTestProbeType)
            //        {
            //            //                        this.errorProvider1.SetError(txtBoxProductID, "The current installed test probe is not compatible with the selected product type.");
            //            Notify.PopUp("Incompatible test probe for selected product type", String.Format("The type of current installed test probe '{0}' is not compatible with the selected product type.", CalibrationSettings.Instance.MeasurementTest.CurrentInstalledTestProbeType) +
            //                "\n\nPlease ask Admin or Maintenance Engineer to ensure the installed test probe matched the Test Probe type setting in the 'hardware setting'->'Test electronics' page.", "", "OK");
            //            return false;
            //        }
            //        break;
            //    }
            //}
            return true;
        }
    }

    public class LoadProductTypeResults
    {
        public string ProductID
        {
            get;
            set;
        }
        public string ProductName { get; set; }

        public string ConversionBoardID
        {
            get;
            set;
        }    
    }
}
