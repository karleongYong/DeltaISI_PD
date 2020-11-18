using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XyratexOSC.UI
{
    /// <summary>
    /// A panel that will be double buffered by Windows.
    /// </summary>
    public class DoubleBufferedPanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleBufferedPanel"/> class.
        /// </summary>
        public DoubleBufferedPanel()
        {
            this.DoubleBuffered = true;
        }
    }
}
