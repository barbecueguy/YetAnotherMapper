using System;
using System.Reflection;

namespace Yams
{
    public sealed class PropertyMap
    {
        public PropertyInfo SourceProperty { get; private set; }
        public PropertyInfo DestinationProperty { get; private set; }
        public Func<object, object> MappingFunction { get; private set; }

        internal PropertyMap(PropertyInfo sourceProperty, PropertyInfo destinationProperty, Func<object, object> mappingFunction)
        {
            this.SourceProperty = sourceProperty;
            this.DestinationProperty = destinationProperty;
            this.MappingFunction = mappingFunction;
        }

        public override string ToString()
        {
            return string.Format("Property {0} using {1} mapping", DestinationProperty.Name, SourceProperty == null ? "custom" : "default");
        }

        // TODO: do i need this?
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
