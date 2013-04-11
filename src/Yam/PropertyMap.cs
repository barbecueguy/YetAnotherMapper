using System;
using System.Linq;
using System.Reflection;

namespace Yams
{
    public class PropertyMap
    {
        public PropertyInfo SourceProperty;
        public PropertyInfo DestinationProperty;
        public Func<object, object> MappingFunction;

        public void Map(object source, object destination)
        {
            if (MappingFunction == null && SourceProperty == null)
            {
                DestinationProperty.SetValue(destination, GetDefaultValue(DestinationProperty.PropertyType), null);
                return;
            }

            if (MappingFunction == null)
                MappingFunction = src => SourceProperty.GetValue(src, null);

            DestinationProperty.SetValue(destination, MappingFunction(source), null);
        }

        public override string ToString()
        {
            return string.Format("Property {0} using {1} mapping", DestinationProperty.Name, SourceProperty == null ? "custom" : "default");
        }

        private object GetDefaultValue(Type type)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}
