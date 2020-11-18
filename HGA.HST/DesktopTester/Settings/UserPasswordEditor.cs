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
//using Seagate.AAS.HGA.HST.UI;
using DesktopTester.UserControls;


namespace DesktopTester.Settings
{
    public class UserPasswordCOMPortSettingsEditor : UITypeEditor
    {
        private IWindowsFormsEditorService _COMPortSettingsEditorService;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) 
        {
            if( (context != null) && (provider != null)) 
            {
                User user = context.Instance as User;
                if (user != null)
                {
                    // Access the Property Browser's UI display service
                    _COMPortSettingsEditorService =
                        (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                    if (_COMPortSettingsEditorService != null)
                    {
                        using (UserPasswordChangeForm modalCOMPortSettingsEditor = new UserPasswordChangeForm(context.Instance as User))
                        {
                            // Display the UI COMPortSettingsEditor dialog
                            if (_COMPortSettingsEditorService.ShowDialog(modalCOMPortSettingsEditor) == DialogResult.OK)
                                if (!String.IsNullOrEmpty(modalCOMPortSettingsEditor.NewPassword))
                                    return modalCOMPortSettingsEditor.NewPassword;
                        }
                    }
                }
            }

            return value.ToString();
        }

        private void OnListBoxSelectedValueChanged(object sender, EventArgs e)
        {
            _COMPortSettingsEditorService.CloseDropDown();
        }
    }
}
