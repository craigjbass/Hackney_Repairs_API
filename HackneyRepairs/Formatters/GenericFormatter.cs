using System;
using System.Collections.Generic;

namespace HackneyRepairs.Formatters
{
    public static class GenericFormatter
    {
        public static void TrimStringAttributes(object result)
        {
            foreach (var property in result.GetType().GetProperties())
            {
                if (property.PropertyType.Name == "String") 
                {
                    string value = (string)property.GetValue(result, null);
                    if (!string.IsNullOrEmpty(value))
                    {
                        property.SetValue(result, value.TrimEnd());
                    }
                }
            }
        }

        public static void TrimStringAttributesInEnumerable(IEnumerable<object> results)
        {
            foreach (var obj in results)
            {
                TrimStringAttributes(obj);
            }
        }

        public static string CommaSeparate(string[] references)
        {
            return String.Join(", ", references);
        }
    }
}
