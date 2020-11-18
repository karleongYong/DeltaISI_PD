using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.UI
{
    class NotifyBannerEventArgs : EventArgs
    {
        public NotifyBanner Banner
        {
            get;
            set;
        }

        public NotifyBannerEventArgs(NotifyBanner banner) :
            base()
        {
            Banner = banner;
        }
    }
}
