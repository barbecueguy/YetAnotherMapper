using System;
using System.Linq.Expressions;
using Yams;

namespace CandiedYams
{
    public static class Yam<TSource, TDestination>
    {
        public static TypeMap<TSource, TDestination> For<TProperty>(
            Expression<Func<TDestination, TProperty>> destination,
            Func<TSource, TProperty> mappingFunction)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            var property = ((MemberExpression)destination.Body);

            var map = Yam.Map(sourceType, destinationType)
                .Add(new PropertyMap
                {
                    DestinationProperty = destinationType.GetProperty(property.Member.Name),
                    MappingFunction = o => mappingFunction((TSource)o)
                });

            return new TypeMap<TSource, TDestination>(map);
        }

        public static TDestination Map(TSource source)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            return (TDestination)Yam.Map(sourceType, destinationType).Map(source);
        }
    }
}
