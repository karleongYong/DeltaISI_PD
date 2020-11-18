using System;
using System.Collections.Generic;
using System.Text;

namespace Seagate.AAS.Parsel.Exceptions
{
    public class Util
    {
        static object _locker = new object();

        public static void LimitDoubleValueDecimalPoints(object[] data, int decimalPoints)
        {
            lock (_locker)
            {
                if (data == null) return;
                for (int x = 0; x < data.Length; x++)
                {
                    if (data[x] is Double)
                    {
                        data[x] = System.Math.Round((double)data[x], decimalPoints);
                    }
                }
            }
        }
    }
}
