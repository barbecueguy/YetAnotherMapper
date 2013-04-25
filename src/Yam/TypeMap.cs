using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Yams
{
    public class TypeMap
    {
        public Type SourceType;
        public Type DestinationType;        

        public TypeMap()
        {
            this.propertyMaps = new List<PropertyMap>();
        }

        public TypeMap(Type sourceType, Type destinationType)
            : this()
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");
            if (destinationType == null)
                throw new ArgumentNullException("destinationType");

            this.SourceType = sourceType;
            this.DestinationType = destinationType;
        }

        public ReadOnlyCollection<PropertyMap> PropertyMaps { get { return new ReadOnlyCollection<PropertyMap>(propertyMaps); } }

        public TypeMap Add(PropertyMap propertyMap)
        {
            var pm = propertyMaps.FirstOrDefault(mp => mp.DestinationProperty == propertyMap.DestinationProperty);
            if (pm != null)
            {
                propertyMaps.Remove(pm);
            }

            propertyMaps.Add(propertyMap);
            return this;
        }

        public object Map(object source)
        {
            object destination = Activator.CreateInstance(DestinationType);
            var destinationProperties =
                DestinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.GetIndexParameters().Any() == false && prop.GetSetMethod() != null);
            foreach (var property in destinationProperties)
            {
                    var propertyMap = propertyMaps.FirstOrDefault(pm => pm.DestinationProperty == property);
                    if (propertyMap == null)
                    {
                        propertyMap = new PropertyMap
                        {
                            DestinationProperty = property,
                            SourceProperty = SourceType.GetProperty(property.Name)
                        };

                        propertyMaps.Add(propertyMap);
                    }

                    propertyMap.Map(source, destination);
            }

            return destination;
        }

        private readonly List<PropertyMap> propertyMaps;
    }

    public class TypeMap<TSource, TDestination> : TypeMap
    {
        public TypeMap()
        {
            this.DestinationType = typeof(TDestination);
            this.SourceType = typeof(TSource);
        }

        public TypeMap(TypeMap map)
            : this()
        {
            if (this.DestinationType != map.DestinationType || this.SourceType != map.SourceType)
                throw new ArgumentOutOfRangeException("map is not the same source and destination type");

            foreach (var propertyMap in map.PropertyMaps)
                this.Add(propertyMap);
        }

        public TDestination Map(TSource source)
        {
            return (TDestination)base.Map(source);
        }

        public TypeMap<TSource, TDestination> For<TProperty>(
            Expression<Func<TDestination, TProperty>> destination,
            Expression<Func<TSource, TProperty>> mappingFunction)
        {
            var propertyMap = new PropertyMap
            {
                DestinationProperty = GetProperty(destination),
                MappingFunction = o => mappingFunction.Compile()((TSource)o)
            };

            if (mappingFunction.Body.NodeType == ExpressionType.MemberAccess)
                propertyMap.SourceProperty = GetProperty(mappingFunction);

            this.Add(propertyMap);
            return this;
        }

        private PropertyInfo GetProperty<TProperty, T>(Expression<Func<T, TProperty>> function)
        {
            var type = typeof(T);
            var propertyExpression = (MemberExpression)function.Body;
            var property = type.GetProperty(propertyExpression.Member.Name);
            return property;
        }
    }
}