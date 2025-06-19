using System;
using System.ComponentModel;

namespace Peritos.Common.Extensions
{
    /// <summary>
    /// Extension methods for working with <see cref="Enum"/> types.
    /// </summary>
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            return value.GetAttribute<DescriptionAttribute>().Description;
        }

        /// <summary>
        /// Converts one enum to another using string values first, then integers value if string parsing fails.
        /// </summary>
        public static TEnum To<TEnum>(this Enum value) where TEnum : struct, Enum
        {
            Enum.TryParse<TEnum>(value.ToString(), out var result);
            return result;
        }

        /// <summary>
        /// Gets a custom attribute of type <typeparamref name="T"/> defined on the enum value.
        /// </summary>
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Length > 0
              ? (T)attributes[0]
              : null;
        }
    }
}
