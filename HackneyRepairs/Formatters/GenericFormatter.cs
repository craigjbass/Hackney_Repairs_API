using System;
using System.Collections.Generic;

namespace HackneyRepairs.Formatters
{
    public static class GenericFormatter
    {
        public static void TrimStringAttributes(IEnumerable<object> result)
        {
            foreach (var obj in result)
            {
                foreach (var property in obj.GetType().GetProperties())
                {
                    if (property.PropertyType.Name == "String") 
                    {
                        string value = (string)property.GetValue(obj, null);
                        if (value != null)
                        {
                            property.SetValue(obj, value.Trim());
                        }
                    }
                }
            }
        }
    }
}
