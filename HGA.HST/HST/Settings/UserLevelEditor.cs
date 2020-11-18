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
    public class UserLevelEditor : UITypeEditor
    {
        private IWindowsFormsEditorService _editorService;
        private string _doNotHavePermissions = "You do not have permission to change.";
        private string _loginToChange = "Login as this user to change.";

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
            
            IList<UserLevel> userLevels = new List<UserLevel>();

            foreach (UserLevel userLevel in Enum.GetValues(typeof(UserLevel)))
            {
                    userLevels.Add(userLevel);
            }

            foreach (UserLevel userLevel in userLevels)
            {
                int index = lb.Items.Add(userLevel);

                if (String.Equals(userLevel, value))
                    lb.SelectedIndex = index;
            }

            _editorService.DropDownControl(lb);

            if (lb.SelectedItem == null) // no selection, return the passed-in value as is
                return value;

            if (String.Equals(lb.SelectedItem.ToString(), _doNotHavePermissions))
                return value;

            if (String.Equals(lb.SelectedItem.ToString(), _loginToChange))
                return value;

            return lb.SelectedItem;
        }

        private void OnListBoxSelectedValueChanged(object sender, EventArgs e)
        {
            _editorService.CloseDropDown();
        }
    }
}
