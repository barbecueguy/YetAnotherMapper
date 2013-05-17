using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Yams
{
    public sealed class TypeMap<TSource, TDestination>
    {
        private readonly List<PropertyMap> propertyMaps;

        public Type SourceType { get; private set; }
        public Type DestinationType { get; private set; }
        public IEnumerable<PropertyMap> PropertyMaps
        {
            get
            {
                return propertyMaps.ToArray();
            }
        }

        internal TypeMap()
        {
            propertyMaps = new List<PropertyMap>();
            this.DestinationType = typeof(TDestination);
            this.SourceType = typeof(TSource);

            var destinationProperties = this.DestinationType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetIndexParameters().Any() == false);
            foreach (PropertyInfo destinationProperty in destinationProperties)
            {
                var sourceProperty = this.SourceType.GetProperty(destinationProperty.Name);
                if (sourceProperty == null)
                    continue;

                var propertyMap = new PropertyMap(sourceProperty, destinationProperty, null);
                this.propertyMaps.Add(propertyMap);
            }
        }

        public TypeMap<TSource, TDestination> Add(PropertyMap propertyMap)
        {
            var pm = propertyMaps.FirstOrDefault(mp => mp.DestinationProperty == propertyMap.DestinationProperty);
            if (pm != null)
            {
                propertyMaps.Remove(pm);
            }

            propertyMaps.Add(propertyMap);
            return this;
        }

        public TypeMap<TSource, TDestination> For<TProperty>(
            Expression<Func<TDestination, TProperty>> destination,
            Expression<Func<TSource, TProperty>> mappingFunction)
        {
            var map = Yam.For(destination, mappingFunction);
            return map;
        }

        public TDestination Map(TSource source)
        {
            var result = Yam.Map<TSource, TDestination>(source);
            return result;
        }
    }
}