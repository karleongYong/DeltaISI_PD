using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XyratexOSC.Factory
{
    public class SecsList : List<ISecsValue>, ISecsValue
    {
        private readonly string _format = "L";
        
        /// <summary>
        /// Gets the SECS format string.
        /// </summary>
        /// <returns></returns>
        public string GetSECSFormat()
        {
            return _format;
        }

        public int GetSECSLength()
        {
            return this.Count;
        }

        public SecsList(int length)
            : base(length)
        {

        }
    }
}
