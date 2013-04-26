using System;
using System.Reflection;

namespace Yams
{
    public class PropertyMap
    {
        public PropertyInfo SourceProperty { get; protected set; }
        public PropertyInfo DestinationProperty { get; protected set; }
        public Func<object, object> MappingFunction { get; protected set; }

        public PropertyMap(PropertyInfo sourceProperty, PropertyInfo destinationProperty)
        {
            this.SourceProperty = sourceProperty;
            this.DestinationProperty = destinationProperty;
        }

        public PropertyMap(PropertyInfo sourceProperty, PropertyInfo destinationProperty, Func<object, object> mappingFunction)
        {
            this.SourceProperty = sourceProperty;
            this.DestinationProperty = destinationProperty;
            this.MappingFunction = mappingFunction;
        }

        public PropertyMap(PropertyInfo destinationProperty, Func<object, object> mappingFunction)
        {
            this.DestinationProperty = destinationProperty;
            this.MappingFunction = mappingFunction;
        }

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
                var type = DestinationProperty.PropertyType;
                MappingFunction = src => GetDefaultValue(type);
            }
            else if (MappingFunction == null)
            {
                MappingFunction = src => SourceProperty.GetValue(src, null);
            }

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
