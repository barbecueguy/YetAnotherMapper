using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Yams
{
    public class TypeMap<TSource, TDestination> : ITypeMap
    {
        public Type SourceType { get; set; }
        public Type DestinationType { get; set; }
        public List<PropertyMap> PropertyMaps
        {
            get
            {
                return propertyMaps;
            }
        }

        public TypeMap()
        {
            propertyMaps = new List<PropertyMap>();
            this.DestinationType = typeof(TDestination);
            this.SourceType = typeof(TSource);
        }

        public TypeMap(ITypeMap map)
            : this()
        {
            if (this.DestinationType != map.DestinationType || this.SourceType != map.SourceType)
                throw new ArgumentOutOfRangeException("map is not the same source and destination type");

            foreach (var propertyMap in map.PropertyMaps)
                this.Add(propertyMap);
        }

        public ITypeMap Add(PropertyMap propertyMap)
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
                    propertyMap = new PropertyMap(SourceType.GetProperty(property.Name), property);
                    propertyMaps.Add(propertyMap);
                }

                propertyMap.Map(source, destination);
            }

            return destination;
        }

        public TDestination Map(TSource source)
        {
            return (TDestination)Map((object)source);
        }

        public TypeMap<TSource, TDestination> For<TProperty>(
            Expression<Func<TDestination, TProperty>> destination,
            Expression<Func<TSource, TProperty>> mappingFunction)
        {
            PropertyInfo sourceProperty = null;
            PropertyInfo destinationProperty = GetProperty(destination);
            if (mappingFunction.Body.NodeType == ExpressionType.MemberAccess)
                sourceProperty = GetProperty(mappingFunction);

            var propertyMap = new PropertyMap(sourceProperty, destinationProperty, o => mappingFunction.Compile()((TSource)o));
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

        private readonly List<PropertyMap> propertyMaps;
    }
}