using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Seagate.AAS.HGA.HST.Settings
{
    public class NamedConverter<T> : ExpandableObjectConverter where T : INamed
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return typeof(string) == destinationType;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (typeof(string) == destinationType)
            {
                T namedValue = (T)value;
                if (namedValue != null)
                    return namedValue.Name;
            }

            return "(none)";
        }
    }
}
