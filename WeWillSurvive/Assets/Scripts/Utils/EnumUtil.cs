using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

        public static int GetEnumIndex<T>(T value) where T : Enum
        {
            T[] all = (T[])Enum.GetValues(typeof(T));
            return Array.IndexOf(all, value);
        }


        public static T? GetEnumByDescription<T>(string description) where T : struct, Enum
        {
            foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attr = field.GetCustomAttribute<DescriptionAttribute>();
                if (attr != null && attr.Description == description)
                {
                    if (Enum.TryParse<T>(field.Name, out var result))
                        return result;
                }
            }

            return null;
        }
    }
}
