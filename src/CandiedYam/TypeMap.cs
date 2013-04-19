using System;
using System.Linq;
using System.Linq.Expressions;
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
