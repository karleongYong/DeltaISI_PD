using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace XyratexOSC
{
    /// <summary>
    /// Provides a type converter for Named objects (see <see cref="INamed"/>) that displays the object name instead of the object type in a properties window.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NamedConverter<T> : ExpandableObjectConverter where T : INamed
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return typeof(string) == destinationType;
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the <paramref name="value" /> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
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
