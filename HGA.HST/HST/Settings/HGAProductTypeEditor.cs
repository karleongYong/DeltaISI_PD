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
    public class HGAProductTypeEditor : UITypeEditor
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

            _editorService.DropDownControl(lb);
            if (lb.SelectedItem == null) // no selection, return the passed-in value as is
                return value;

            return lb.SelectedItem;
        }

        private void OnListBoxSelectedValueChanged(object sender, EventArgs e)
        {
            _editorService.CloseDropDown();
        }
    }
}
