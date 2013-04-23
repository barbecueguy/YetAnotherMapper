using System;
using System.Linq.Expressions;
using System.Reflection;
using Yams;

namespace CandiedYams
{
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
