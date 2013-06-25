using System;
using System.Collections.Generic;

namespace Yams
{
    public sealed class TypeMap
    {
        public Type SourceType { get; private set; }
        public Type DestinationType { get; private set; }
        public List<PropertyMap> PropertyMaps { get { return propertyMaps; } }

        internal TypeMap(Type sourceType, Type destinationType)
        {
            this.SourceType = sourceType;
            this.DestinationType = destinationType;
        }

        private readonly List<PropertyMap> propertyMaps = new List<PropertyMap>();
    }

    public class TypeMap<TSource, TDestination> { }
}