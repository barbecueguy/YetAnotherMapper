using System;
using System.Linq;
using System.Linq.Expressions;
using Yams;

namespace CandiedYams
{
    public class TypeMap<TSource, TDestination> : TypeMap
    {
        public TypeMap(TypeMap map)
        {
            this.DestinationType = map.DestinationType;
            foreach (var propertyMap in map.PropertyMaps)
                this.Add(propertyMap);
            this.SourceType = map.SourceType;
        }

        public TDestination Map(TSource source)
        {
            return (TDestination)base.Map(source);
        }

        public TypeMap<TSource, TDestination> For<TProperty>(
            Expression<Func<TDestination, TProperty>> destination,
            Func<TSource, TProperty> mappingFunction)
        {
            var destinationType = typeof(TDestination);
            var property = ((MemberExpression)destination.Body);

           this.Add(new PropertyMap
                {
                    DestinationProperty = destinationType.GetProperty(property.Member.Name),
                    MappingFunction = o => mappingFunction((TSource)o)
                });

            return this;
        }
    }
}
