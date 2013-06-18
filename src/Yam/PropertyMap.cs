using System;
using System.Reflection;

namespace Yams
{
    public sealed class PropertyMap
    {
        public string SourcePropertyName { get; private set; }
        public string DestinationPropertyName { get; private set; }
        public Type SourcePropertyType { get; private set; }
        public Type DestinationPropertyType { get; private set; }
        public Func<object, object> MappingFunction { get; private set; }

        internal PropertyMap(PropertyInfo sourceProperty, PropertyInfo destinationProperty)
        {
            this.SourcePropertyName = sourceProperty.Name;
            this.DestinationPropertyName = destinationProperty.Name;
            this.SourcePropertyType = sourceProperty.PropertyType;
            this.DestinationPropertyType = destinationProperty.PropertyType;
        }

        internal PropertyMap(Type sourcePropertyType, PropertyInfo destinationProperty, Func<object, object> mappingFunction)
        {
            this.SourcePropertyName = "";
            this.DestinationPropertyName = destinationProperty.Name;
            this.SourcePropertyType = sourcePropertyType;
            this.DestinationPropertyType = destinationProperty.PropertyType;
            this.MappingFunction = mappingFunction;
        }

        public override string ToString()
        {
            return string.Format("Property {0} using {1} mapping", DestinationPropertyName, SourcePropertyName == null ? "custom" : "default");
        }
    }
}
