using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Yams
{
    public static class Yam
    {
        private static readonly List<TypeMap> maps = new List<TypeMap>();

        public static TypeMap Map(Type from, Type to)
        {
            var map = maps.FirstOrDefault(mp => mp.SourceType == from && mp.DestinationType == to);
            if (map == null)
            {
                map = new TypeMap
                {
                    DestinationType = to,
                    SourceType = from
                };

                maps.Add(map);
            }

            return map;
        }

        public static void Clear()
        {
            maps.Clear();
        }
    }

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
