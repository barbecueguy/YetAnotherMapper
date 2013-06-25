using System;
using System.Collections.Generic;
using System.Linq;

namespace Yams
{
    internal static class TypeExtensions
    {
        public static bool IsGenericEnumerable(this Type sourceType)
        {
            if (sourceType.IsGenericType)
            {
                var definition = sourceType.GetGenericTypeDefinition();
                var interfaces = definition.GetInterfaces().Select(i => i.Name);
                if (interfaces.Contains(typeof(IEnumerable<>).Name))
                    return true;
            }

            return false;
        }

        public static bool IsGenericCollection(this Type destinationType)
        {
            if (destinationType.IsGenericType)
            {
                var definition = destinationType.GetGenericTypeDefinition();
                var interfaces = definition.GetInterfaces().Select(i => i.Name);
                if (interfaces.Contains(typeof(ICollection<>).Name))
                    return true;
            }

            return false;
        }

        public static bool IsConvertible(this Type sourceType)
        {
            var interfaces = sourceType.GetInterfaces().Select(i => i.Name);
            return interfaces.Contains("IConvertible");
        }
    }
}
