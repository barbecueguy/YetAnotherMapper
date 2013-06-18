using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
}