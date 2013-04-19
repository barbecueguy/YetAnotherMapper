using System;
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
            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (DestinationProperty == null)
                throw new InvalidOperationException("DestinationProperty cannot be null");

            if (MappingFunction == null && SourceProperty == null)
            {
                var defaultValue = GetDefaultValue(DestinationProperty.PropertyType);
                DestinationProperty.SetValue(destination, defaultValue, null);
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

        private T GetDefaultValue<T>(T t) where T : new()
        {
            return default(T);
        }
    }
}
