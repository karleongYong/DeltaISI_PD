using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XyratexOSC.Logging;

namespace XyratexOSC.UI
{
    /// <summary>
    /// Provides standard UI functions.
    /// </summary>
    public static class UIUtility
    {
        private static Color _xyratexBlue = Color.FromArgb(4, 85, 138);

        /// <summary>
        /// Gets the Xyratex blue.
        /// </summary>
        /// <value>
        /// The Xyratex blue.
        /// </value>
        public static Color XyratexBlue
        {
            get
            {
                return _xyratexBlue;
            }
        }

        /// <summary>
        /// Synchronously invokes an action on the thread that owns the specified control, if required.
        /// </summary>
        /// <param name="c">The control.</param>
        /// <param name="a">The action.</param>
        public static void Invoke(Control c, Action a)
        {
            if (c == null || !c.InvokeRequired)
                a();
            else
            {
                if (a != null && c!= null)
                {
                    c.Invoke(a);
                }
            }
        }

        /// <summary>
        /// Asynchronously invokes an action on the thread that owns the specified control, if required.
        /// </summary>
        /// <param name="c">The control.</param>
        /// <param name="a">The action.</param>
        public static void BeginInvoke(Control c, Action a)
        {
            if (c == null || !c.InvokeRequired)
                a();
            else
                c.BeginInvoke(a);
        }

        /// <summary>
        /// Starts the an action asynchronously on the thread that owns the specified control, an optionally supplies actions to execute when completed or errored.
        /// </summary>
        /// <param name="c">The control.</param>
        /// <param name="action">The action.</param>
        /// <param name="onComplete">The action on complete.</param>
        /// <param name="onError">The action on error.</param>
        public static void StartAsync(Control c, Action action, Action onComplete = null, Action<Exception> onError = null)
        {
            StartAsync(c, action, TaskCreationOptions.None, onComplete, onError);
        }

        /// <summary>
        /// Starts the an action asynchronously on the thread that owns the specified control, an optionally supplies actions to execute when completed or errored.
        /// </summary>
        /// <param name="c">The control.</param>
        /// <param name="action">The action.</param>
        /// <param name="taskOptions">The task options.</param>
        /// <param name="onComplete">The action on complete.</param>
        /// <param name="onError">The action on error.</param>
        public static void StartAsync(Control c, Action action, TaskCreationOptions taskOptions, Action onComplete = null, Action<Exception> onError = null)
        {
            if (action == null)
                return;

            Task.Factory.StartNew(action, taskOptions).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    if (onError == null)
                    {
                        Log.Error("Error", t.Exception.InnerException);
                        Notify.BannerError(t.Exception.InnerException.Message, t.Exception.InnerException.ToString());
                    }
                    else
                    {
                        onError(t.Exception.InnerException);
                    }
                }

                if (onComplete != null)
                    onComplete();
            });
        }
    }
}
