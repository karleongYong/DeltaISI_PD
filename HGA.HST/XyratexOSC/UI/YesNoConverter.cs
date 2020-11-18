using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XyratexOSC.UI
{
    /// <summary>
    /// Provides a type converter to convert <see cref="System.Boolean"/> objects to "Yes"/"No" representations.
    /// </summary>
    public class YesNoConverter : BooleanConverter
    {
        private const string _yes = "Yes";
        private const string _no = "No";

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
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (value is bool && destinationType == typeof(string))
                return (bool)value ? _yes : _no;

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Converts the given value object to a Boolean object.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" /> that specifies the culture to which to convert.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> that represents the converted <paramref name="value" />.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string txt = value as string;

            if (_no == txt) 
                return false;

            if (_yes == txt) 
                return true;

            return base.ConvertFrom(context, culture, value);
        }
    }
}
