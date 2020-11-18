using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace XyratexOSC.Utilities
{
    /// <summary>
    /// Provides Enum value extensions for getting enum member attributes
    /// </summary>
    public static class EnumExtensions
    {

        /// <summary>
        /// Gets the specified attribute for this enum value.
        /// </summary>
        /// <typeparam name="T">The Attribute Type</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.MissingMemberException">
        /// No attributes defined for the specified enum.
        /// or
        /// The specified attribute is not defined for the specified enum.
        /// </exception>
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var valueName = value.ToString();
            var memberInfo = type.GetMember(valueName);

            if (memberInfo == null || memberInfo.Length < 1)
                throw new MissingMemberException("No attributes defined for " + valueName);

            Type attributeType = typeof(T);
            var attributes = memberInfo[0].GetCustomAttributes(attributeType, false);
            
            if (memberInfo == null || memberInfo.Length < 1)
                throw new MissingMemberException(String.Format("No {0} is defined for {1}", attributeType.ToString(), valueName));

            return (T)attributes[0];
        }

        /// <summary>
        /// Gets the description attribute for this enum value.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>The description attribute.</returns>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();

            //Tries to find a DescriptionAttribute for a potential friendly name
            MemberInfo[] memberInfo = type.GetMember(value.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            //If we have no description attribute, just return the ToString of the enum
            return value.ToString();
        }
    }
}
