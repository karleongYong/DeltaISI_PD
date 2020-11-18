using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Seagate.AAS.HGA.HST.Settings
{
    public class TestProbeTypeEditor : UITypeEditor
    {
        private IWindowsFormsEditorService _editorService;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            _editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            ListBox lb = new ListBox();
            lb.SelectionMode = SelectionMode.One;
            lb.SelectedValueChanged += OnListBoxSelectedValueChanged;

            IEnumerable<TestProbeType> TestProbeTypes = CalibrationSettings.Instance.MeasurementTest.TestProbeTypes;

            foreach (TestProbeType type in TestProbeTypes.OrderBy(s => s.Name))
            {
                int index = lb.Items.Add(type.Name);

                if (String.Equals(type.Name, value))
                    lb.SelectedIndex = index;
            }

            _editorService.DropDownControl(lb);
            if (lb.SelectedItem == null)
                return value;

            return lb.SelectedItem;
        }

        private void OnListBoxSelectedValueChanged(object sender, EventArgs e)
        {
            _editorService.CloseDropDown();
        }
    }
}
