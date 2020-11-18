using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using XyratexOSC.IO;

namespace XyratexOSC.UI
{
    /// <summary>
    /// The application central location for sending UI notifications.
    /// </summary>
    public class Notify
    {
        private LightTower _lightTower;

        // static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<Notify> _instance = new Lazy<Notify>(() => new Notify());

        // accessor for instance
        internal static Notify Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private Notify()
        {
        }

        /// <summary>
        /// Gets or sets the light tower interface in order to allow pop-ups and other notifications to trigger light stack responses.
        /// </summary>
        /// <value>
        /// The tool light tower.
        /// </value>
        public static LightTower LightTower
        {
            get
            {
                return Instance._lightTower;
            }
            set
            {
                Instance._lightTower = value;
            }
        }

        /// <summary>
        /// Sends a pop-up dialog notification.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="request">The operator request.</param>
        /// <param name="customButtonText">The custom button text if required.</param>
        /// <param name="customAction">The custom action if required.</param>
        /// <returns></returns>
        public static NotifyButton PopUp(string title, string message, NotifyButton buttons, string request = "", string customButtonText = "", Action customAction = null)
        {
            return PopUp(title, message, buttons, LightStackColor.Yellow, request, customButtonText, customAction);
        }

        /// <summary>
        /// Sends a pop-up dialog notification.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="buttons">The buttons.</param>
        /// <param name="blinkColor">Light stack colors that should blink until this popup is dismissed.</param>
        /// <param name="request">The operator request.</param>
        /// <param name="customButtonText">The custom button text if required.</param>
        /// <param name="customAction">The custom action if required.</param>
        /// <returns></returns>
        public static NotifyButton PopUp(string title, string message, NotifyButton buttons, LightStackColor blinkColor, string request = "", string customButtonText = "", Action customAction = null)
        {
            NotifyButton clickedButton = NotifyButton.Cancel;

            Form activeForm = Form.ActiveForm;
            if (activeForm == null && Application.OpenForms.Count > 0)
                activeForm = Application.OpenForms[Application.OpenForms.Count - 1];

            UIUtility.Invoke(activeForm, () =>
            {
                using (NotifyForm notify = new NotifyForm(title, message, buttons, request, customButtonText, customAction))
                {
                    LightStackColor prevColor = LightStackColor.Off;
                    bool lightTowerSet = false;

                    try
                    {
                        if (LightTower != null && blinkColor != LightStackColor.Off)
                        {
                            prevColor = LightTower.GetBlink();
                            LightTower.Off(LightStackColor.All);
                            LightTower.Blink(blinkColor);
                            lightTowerSet = true;
                        }
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        notify.ShowDialog();
                        clickedButton = notify.ClickedButton;

                        if (lightTowerSet)
                        {
                            LightTower.Off(LightStackColor.All);
                            LightTower.Blink(prevColor);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            });

            return clickedButton;
        }

        /// <summary>
        /// Sends a pop-up dialog notification.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="request">The request.</param>
        /// <param name="buttonNames">The custom button names.</param>
        /// <returns></returns>
        public static string PopUp(string title, string message, string request, params string[] buttonNames)
        {
            return PopUp(title, message, request, LightStackColor.Yellow, buttonNames);
        }

        /// <summary>
        /// Sends a pop-up dialog notification.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="request">The request.</param>
        /// <param name="defaultButtonIndex">The index of the buttonNames array which is set to be the default button.</param>
        /// <param name="buttonNames">The custom button names.</param>
        /// <returns></returns>
        public static string PopUp(string title, string message, string request, int defaultButtonIndex, params string[] buttonNames)
        {
            return PopUp(title, message, request, LightStackColor.Yellow, defaultButtonIndex, buttonNames);
        }

        /// <summary>
        /// Sends a pop-up dialog notification.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="request">The request.</param>
        /// <param name="blinkColor">Light stack colors that should blink until this popup is dismissed.</param>
        /// <param name="defaultButtonIndex">The index of the buttonNames array which is set to be the default button.</param>
        /// <param name="buttonNames">The custom button names.</param>
        /// <returns></returns>
        public static string PopUp(string title, string message, string request, LightStackColor blinkColor, int defaultButtonIndex, params string[] buttonNames)
        {
            string customClickedName = "";

            Form activeForm = Form.ActiveForm;
            if (activeForm == null && Application.OpenForms.Count > 0)
                activeForm = Application.OpenForms[Application.OpenForms.Count - 1];

            int notifyCount = 0;
            foreach (Form f in Application.OpenForms)
                if (f is NotifyForm)
                    notifyCount++;

            UIUtility.Invoke(activeForm, () =>
            {
                using (NotifyForm notify = new NotifyForm(title, message, request, defaultButtonIndex, buttonNames))
                {
                    if (notifyCount > 0)
                    {
                        notify.StartPosition = FormStartPosition.Manual;
                        Rectangle bounds = Screen.PrimaryScreen.Bounds;
                        Point center = new Point((bounds.Width - notify.Width) / 2, (bounds.Height - notify.Height) / 2);
                        notify.Location = new Point(center.X + (50 * notifyCount), center.Y + (50 * notifyCount));
                    }

                    LightStackColor prevColor = LightStackColor.Off;
                    bool lightTowerSet = false;

                    try
                    {
                        if (LightTower != null && blinkColor != LightStackColor.Off)
                        {
                            prevColor = LightTower.GetBlink();
                            LightTower.Off(LightStackColor.All);
                            LightTower.Blink(blinkColor);
                            lightTowerSet = true;
                        }
                    }
                    catch (Exception) 
                    { 
                    }

                    try
                    {
                        notify.ShowDialog();
                        customClickedName = notify.ClickedCustomName;

                        if (lightTowerSet)
                        {
                            LightTower.Off(LightStackColor.All);
                            LightTower.Blink(prevColor);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            });

            return customClickedName;
        }

        /// <summary>
        /// Sends a pop-up dialog notification.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="request">The request.</param>
        /// <param name="blinkColor">Light stack colors that should blink until this popup is dismissed.</param>
        /// <param name="buttonNames">The custom button names.</param>
        /// <returns></returns>
        public static string PopUp(string title, string message, string request, LightStackColor blinkColor, params string[] buttonNames)
        {
            string customClickedName = "";

            Form activeForm = Form.ActiveForm;
            if (activeForm == null && Application.OpenForms.Count > 0)
                activeForm = Application.OpenForms[Application.OpenForms.Count - 1];

            int notifyCount = 0;
            foreach (Form f in Application.OpenForms)
                if (f is NotifyForm)
                    notifyCount++;

            UIUtility.Invoke(activeForm, () =>
            {
                using (NotifyForm notify = new NotifyForm(title, message, request, buttonNames))
                {
                    if (notifyCount > 0)
                    {
                        notify.StartPosition = FormStartPosition.Manual;
                        Rectangle bounds = Screen.PrimaryScreen.Bounds;
                        Point center = new Point((bounds.Width - notify.Width) / 2, (bounds.Height - notify.Height) / 2);
                        notify.Location = new Point(center.X + (50 * notifyCount), center.Y + (50 * notifyCount));
                    }

                    LightStackColor prevColor = LightStackColor.Off;
                    bool lightTowerSet = false;

                    try
                    {
                        if (LightTower != null && blinkColor != LightStackColor.Off)
                        {
                            prevColor = LightTower.GetBlink();
                            LightTower.Off(LightStackColor.All);
                            LightTower.Blink(blinkColor);
                            lightTowerSet = true;
                        }
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        notify.ShowDialog();
                        customClickedName = notify.ClickedCustomName;

                        if (lightTowerSet)
                        {
                            LightTower.Off(LightStackColor.All);
                            LightTower.Blink(prevColor);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            });

            return customClickedName;
        }

        /// <summary>
        /// Sends an error pop-up dialog notification. The border and title will be in red.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="ex">The exception.</param>
        public static void PopUpError(string title, Exception ex)
        {
            PopUpError(title, ex, LightStackColor.Red);
        }

        /// <summary>
        /// Sends an error pop-up dialog notification. The border and title will be in red.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="blinkColor">Light stack colors that should blink until this popup is dismissed.</param>
        public static void PopUpError(string title, Exception ex, LightStackColor blinkColor)
        {
            string message = ex.Message;

            if (ex.InnerException != null)
                message += Environment.NewLine + Environment.NewLine + ex.InnerException.Message;

            PopUpError(title, message, blinkColor, ex.ToString());
        }

        /// <summary>
        /// Sends an error pop-up dialog notification. The border and title will be in red.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="details">The error details.</param>
        public static void PopUpError(string title, string message, string details = "")
        {
            PopUpError(title, message, LightStackColor.Red, details);
        }

        /// <summary>
        /// Sends an error pop-up dialog notification. The border and title will be in red.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="details">The error details.</param>
        /// <param name="blinkColor">Light stack colors that should blink until this popup is dismissed.</param>
        public static void PopUpError(string title, string message, LightStackColor blinkColor, string details = "")
        {
            Form activeForm = Form.ActiveForm;
            if (activeForm == null && Application.OpenForms.Count > 0)
                activeForm = Application.OpenForms[Application.OpenForms.Count - 1];

            int notifyCount = 0;
            foreach (Form f in Application.OpenForms)
                if (f is NotifyForm)
                    notifyCount++;

            UIUtility.Invoke(activeForm, () =>
            {
                using (NotifyForm notify = new NotifyForm(title, message, "", "Details", "Copy", "OK"))
                {
                    notify.BackColor = Color.OrangeRed;
                    notify.CustomButtonText = details;
                    notify.FormClosing += notify_FormClosing;

                    if (notifyCount > 0)
                    {
                        notify.StartPosition = FormStartPosition.Manual;
                        Rectangle bounds = Screen.PrimaryScreen.Bounds;
                        Point center = new Point((bounds.Width - notify.Width) / 2, (bounds.Height - notify.Height) / 2);
                        notify.Location = new Point(center.X + (50 * notifyCount), center.Y + (50 * notifyCount));
                    }

                    LightStackColor prevColor = LightStackColor.Off;
                    bool lightTowerSet = false;

                    try
                    {
                        if (LightTower != null && blinkColor != LightStackColor.Off)
                        {
                            prevColor = LightTower.GetBlink();
                            LightTower.Off(LightStackColor.All);
                            LightTower.Blink(blinkColor);
                            lightTowerSet = true;
                        }
                    }
                    catch (Exception) 
                    { 
                    }

                    try
                    {
                        notify.ShowDialog();
                        
                        if (lightTowerSet)
                        {
                            LightTower.Off(LightStackColor.All);
                            LightTower.Blink(prevColor);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            });
        }

        private static void notify_FormClosing(object sender, FormClosingEventArgs e)
        {
            NotifyForm notifyForm = sender as NotifyForm;
            if (notifyForm == null)
                return;
             
            if (notifyForm.ClickedCustomName == "Details")
            {
                notifyForm.Message += Environment.NewLine + Environment.NewLine + notifyForm.CustomButtonText;

                foreach (Button button in notifyForm.Buttons)
                    if (button.Text == "Details")
                        button.Enabled = false;

                e.Cancel = true;
            }
            else if (notifyForm.ClickedCustomName == "Copy")
            {
                string clipboardText = notifyForm.Title + Environment.NewLine + notifyForm.Message;

                if (!clipboardText.Contains(notifyForm.CustomButtonText))
                    clipboardText += Environment.NewLine + Environment.NewLine + notifyForm.CustomButtonText;

                Thread t = new Thread(obj => Clipboard.SetText(obj.ToString()));
                t.SetApartmentState(ApartmentState.STA);
                t.Start(clipboardText);

                e.Cancel = true;
            }
        }

        /// <summary>
        /// Sends a banner notification to be displayed at the top of the application notification panel.
        /// </summary>
        /// <param name="banner">The pre-constructed banner.</param>
        public static void Banner(NotifyBanner banner)
        {
            Form mainForm = null;

            if (Application.OpenForms.Count > 0)
                mainForm = Application.OpenForms[0];

            UIUtility.Invoke(mainForm, () =>
            {
                EventHandler<NotifyBannerEventArgs> bannerNotified = Instance.BannerNotified;

                if (bannerNotified == null)
                    return;

                bannerNotified(Instance, new NotifyBannerEventArgs(banner));
            });
        }

        /// <summary>
        /// Sends a banner notification to be displayed at the top of the application notification panel.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="closeText">The close text (defaults to OK).</param>
        /// <param name="actionText">The action text if providing an interactive button for the user to respond.</param>
        /// <param name="action">The custom action when the action button is clicked.</param>
        /// <returns></returns>
        public static NotifyBanner Banner(string message, string closeText = "", string actionText = "", Action action = null)
        {
            NotifyBanner banner = Banner(message, closeText, actionText, action);
            return banner;
        }

        /// <summary>
        /// Sends a banner notification (with 2 custom actions) to be displayed at the top of the application notification panel.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="closeText">The close text (defaults to OK).</param>
        /// <param name="action1Text">First action text.</param>
        /// <param name="action1">First action.</param>
        /// <param name="action2Text">Second action text.</param>
        /// <param name="action2">Second action.</param>
        /// <returns></returns>
        public static NotifyBanner Banner(string message, string closeText, string action1Text, Action action1, string action2Text, Action action2)
        {
            NotifyBanner banner = Banner(message, closeText, action1Text, action1, action2Text, action2);
            return banner;
        }

        /// <summary>
        /// Sends a banner notification (with 3 custom actions) to be displayed at the top of the application notification panel.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="closeText">The close text (defaults to OK).</param>
        /// <param name="action1Text">First action text.</param>
        /// <param name="action1">First action.</param>
        /// <param name="action2Text">Second action text.</param>
        /// <param name="action2">Second action.</param>
        /// <param name="action3Text">Third action text.</param>
        /// <param name="action3">Third action.</param>
        /// <returns></returns>
        public static NotifyBanner Banner(string message, string closeText, string action1Text, Action action1, string action2Text, Action action2, string action3Text, Action action3)
        {
            Form mainForm = null;

            if (Application.OpenForms.Count > 0)
                mainForm = Application.OpenForms[0];

            NotifyBanner banner = null;

            UIUtility.Invoke(mainForm, () => 
            {
                EventHandler<NotifyBannerEventArgs> bannerNotified = Instance.BannerNotified;

                if (bannerNotified == null)
                    return;                
                
                banner = new NotifyBanner();
                banner.Message = message;
                banner.CloseText = closeText;

                if (!String.IsNullOrEmpty(action1Text))
                    banner.AddAction(action1Text, action1);

                if (!String.IsNullOrEmpty(action2Text))
                    banner.AddAction(action2Text, action2);

                if (!String.IsNullOrEmpty(action3Text))
                    banner.AddAction(action3Text, action3);

                bannerNotified(Instance, new NotifyBannerEventArgs(banner));
            });

            return banner;
        }

        /// <summary>
        /// Starts a <see cref="WizardBanner"/> notification, which is displayed at the top of the application notification panel.
        /// </summary>
        /// <param name="steps">The steps.</param>
        /// <returns></returns>
        public static WizardBanner StartWizard(params WizardStep[] steps)
        {
            Form mainForm = null;

            if (Application.OpenForms.Count > 0)
                mainForm = Application.OpenForms[0];

            WizardBanner banner = null;

            UIUtility.Invoke(mainForm, () =>
            {
                EventHandler<NotifyBannerEventArgs> bannerNotified = Instance.BannerNotified;

                if (bannerNotified == null)
                    return;

                banner = new WizardBanner(steps);

                bannerNotified(Instance, new NotifyBannerEventArgs(banner));
            });

            return banner;
        }

        /// <summary>
        /// Sends an error banner notification to be displayed at the top of the application notification panel. This will be in red.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="details">The error details.</param>
        /// <returns></returns>
        public static ErrorBanner BannerError(string message, string details = "")
        {
            Form mainForm = null;

            if (Application.OpenForms.Count > 0)
                mainForm = Application.OpenForms[0];

            ErrorBanner banner = null;

            UIUtility.Invoke(mainForm, () =>
            {
                EventHandler<NotifyBannerEventArgs> bannerNotified = Instance.BannerNotified;

                if (bannerNotified == null)
                    return;

                banner = new ErrorBanner(message, details);

                bannerNotified(Instance, new NotifyBannerEventArgs(banner));
            });

            return banner;
        }


        internal event EventHandler<NotifyBannerEventArgs> BannerNotified;
    }

    /// <summary>
    /// User Message Filter
    /// </summary>
    public class UserMessageFilter : IMessageFilter
    {
        /// <summary>
        /// Filters out a message before it is dispatched.
        /// </summary>
        /// <param name="m">The message to be dispatched. You cannot modify this message.</param>
        /// <returns>
        /// true to filter the message and stop it from being dispatched; false to allow the message to continue to the next filter or control.
        /// </returns>
        public bool PreFilterMessage(ref Message m)
        {
            return false;
            /*
            //Blocks all messages relating to the mouse buttons
            if (m.Msg >= 0x201 && m.Msg <= 0x208)
                return false;

            //Blocks all key down and key up messages
            if (m.Msg == 0x100 || m.Msg == 0x101)
                return false;

            return true;
             * */
        }
    }
}