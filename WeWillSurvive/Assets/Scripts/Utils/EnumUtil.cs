using System;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace WeWillSurvive.Util
{
    public static class EnumUtil 
    {
        public static string GetDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static string[] GetEnumDescriptions<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<Enum>()
                .Select(e => GetDescription(e))
                .ToArray();
        }
    }
}
