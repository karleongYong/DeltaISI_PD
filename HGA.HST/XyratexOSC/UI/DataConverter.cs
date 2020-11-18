using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace XyratexOSC.UI
{
    /// <summary>
    /// Provides a type converter to convert expandable objects of the specified genertic type to a string representation.
    /// </summary>
    /// <typeparam name="T">Any type</typeparam>
    public class DataConverter<T> : ExpandableObjectConverter
    {
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type" /> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
        {
            if (destinationType == typeof(T))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information. Converts to string using the object's ToString().
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the <paramref name="value" /> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted value.
        /// </returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
        {
            if (destinationType == typeof(System.String) && value is T)
            {
                T so = (T)value;
                return so.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
