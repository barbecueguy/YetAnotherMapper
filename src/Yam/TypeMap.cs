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

    public class TypeMap<TSource, TDestination>
    {
        public Type SourceType { get { return this.typeMap.SourceType; } }
        public Type DestinationType { get { return this.typeMap.DestinationType; } }
        public List<PropertyMap> PropertyMaps { get { return this.typeMap.PropertyMaps; } }

        public TypeMap()
        {
            this.typeMap = new TypeMap(typeof(TSource), typeof(TDestination));
        }

        public TypeMap(TypeMap typeMap)
        {
            this.typeMap = typeMap;
        }

        public static implicit operator TypeMap<TSource, TDestination>(TypeMap typeMap)
        {
            TypeMap<TSource, TDestination> newTypeMap = new TypeMap<TSource, TDestination>(typeMap);
            return newTypeMap;
        }        

        private readonly TypeMap typeMap;
    }
}