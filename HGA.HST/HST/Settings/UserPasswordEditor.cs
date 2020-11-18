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
using Seagate.AAS.HGA.HST.UserControls;


namespace Seagate.AAS.HGA.HST.Settings
{
    public class UserPasswordEditor : UITypeEditor
    {
        private IWindowsFormsEditorService _editorService;

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
                    _editorService =
                        (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                    if (_editorService != null)
                    {
                        using (UserPasswordChangeForm modalEditor = new UserPasswordChangeForm(context.Instance as User))
                        {
                            // Display the UI editor dialog
                            if (_editorService.ShowDialog(modalEditor) == DialogResult.OK)
                                if (!String.IsNullOrEmpty(modalEditor.NewPassword))
                                    return modalEditor.NewPassword;
                        }
                    }
                }
            }

            return value.ToString();
        }

        private void OnListBoxSelectedValueChanged(object sender, EventArgs e)
        {
            _editorService.CloseDropDown();
        }
    }
}
