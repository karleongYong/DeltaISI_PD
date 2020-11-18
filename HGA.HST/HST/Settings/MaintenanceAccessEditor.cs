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
    public class MaintenanceAccessEditor : UITypeEditor
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

            if (context == null)
                return value;

            User user = context.Instance as User;
            if (user == null)
                return value;

            IList<string> MaintenanceAccessLevels = new List<string>();

            if (user.Level == UserLevel.Administrator)
                MaintenanceAccessLevels.Add("Yes");
            else if (user.Level <= UserLevel.Operator)
                MaintenanceAccessLevels.Add("No");
            else
            {
                MaintenanceAccessLevels.Add("Yes");
                MaintenanceAccessLevels.Add("No");
            }

            foreach (string maintenanceAccess in MaintenanceAccessLevels)
            {
                int index = lb.Items.Add(maintenanceAccess);
                string userMaintenanceAccess = (Boolean.Equals(value, true)) ? "Yes" : "No";

                if (String.Equals(maintenanceAccess, userMaintenanceAccess))
                    lb.SelectedIndex = index;
            }

            _editorService.DropDownControl(lb);
            if (lb.SelectedItem == null)
                return value;

            return ((string)lb.SelectedItem == "Yes") ? true : false;
        }

        private void OnListBoxSelectedValueChanged(object sender, EventArgs e)
        {
            _editorService.CloseDropDown();
        }
    }
}
