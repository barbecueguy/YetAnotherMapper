using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Yams
{
    public static class Yam
    {
        public static TypeMap<TSource, TDestination> CreateMap<TSource, TDestination>(TSource source, TDestination destination)
        {
            return CreateMap<TSource, TDestination>();
        }

        public static TypeMap<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            var map = GetMap<TSource, TDestination>();
            if (map == null)
                map = new TypeMap<TSource, TDestination>();
            Maps.Add(map);
            return map;
        }

        public static TypeMap<TSource, TDestination> GetMap<TSource, TDestination>()
        {
            var destinationType = typeof(TDestination);
            var sourceType = typeof(TSource);
            var map = (TypeMap<TSource, TDestination>)GetMap(sourceType, destinationType);
            return map;
        }

        public static TypeMap<TSource, TDestination> For<TSource, TDestination, TProperty>(
            Expression<Func<TDestination, TProperty>> destinationPropertyExpression,
            Expression<Func<TSource, TProperty>> mappingExpression)
        {
            var map = Yam.GetMap<TSource, TDestination>();
            if (map == null)
                Yam.CreateMap<TSource, TDestination>();

            map = Yam.AddPropertyMap<TSource, TDestination, TProperty>(
                destinationPropertyExpression,
                mappingExpression);

            return map;
        }

        public static TDestination Map<TSource, TDestination>(TSource source)
        {
            var map = GetMap<TSource, TDestination>();
            if (map == null)
                map = CreateMap<TSource, TDestination>();

            var destination = Activator.CreateInstance<TDestination>();
            foreach (var property in map.PropertyMaps)
            {
                MapProperty(property, source, destination);
            }

            return destination;
        }

        public static TDestination Map<TDestination>(object source)
        {
            var sourceType = source.GetType();
            var destinationType = typeof(TDestination);
            var map = GetMap(sourceType, destinationType);
            if (map == null)
                map = CreateMap(source, default(TDestination));

            var destination = Activator.CreateInstance<TDestination>();
            var propertyMaps = (IEnumerable<PropertyMap>)map.GetType().GetProperty("PropertyMaps").GetValue(map, null);
            foreach (var property in propertyMaps)
            {
                MapProperty(property, source, destination);
            }

            return destination;
        }

        public static void Clear()
        {
            Maps.Clear();
        }

        #region private
        private static TypeMap<TSource, TDestination> AddPropertyMap<TSource, TDestination, TProperty>(
            Expression<Func<TDestination, TProperty>> destinationPropertyExpression,
            Expression<Func<TSource, TProperty>> mappingExpression)
        {
            var map = GetMap<TSource, TDestination>();
            if (map == null)
                throw new ArgumentOutOfRangeException(
                    "destinationPropertyExpression",
                    string.Format("No mapping found from {0} to {1}", typeof(TSource), typeof(TDestination)));

            var destinationPropertyName = ((MemberExpression)destinationPropertyExpression.Body).Member.Name;
            var sourcePropertyExpression = mappingExpression as MemberExpression;
            var sourcePropertyName = sourcePropertyExpression == null ? null : sourcePropertyExpression.Member.Name;
            var sourcePropertyInfo = sourcePropertyName == null ? null : typeof(TSource).GetProperty(sourcePropertyName);
            var destinationPropertyInfo = typeof(TDestination).GetProperty(destinationPropertyName);
            var propertyMap = new PropertyMap(
                sourcePropertyInfo,
                destinationPropertyInfo,
                src => mappingExpression.Compile()((TSource)src));
            map.Add(propertyMap);
            return map;
        }        

        private static object GetMap(Type sourceType, Type destinationType)
        {
            var map = Maps.FirstOrDefault(mp =>
            {
                var type = mp.GetType();
                var mapSourceType = type.GetProperty("SourceType").GetValue(mp, null);
                var mapDestinationType = type.GetProperty("DestinationType").GetValue(mp, null);
                return mapSourceType == sourceType && mapDestinationType == destinationType;
            });

            return map;
        }


        private static void MapProperty(PropertyMap property, object source, object destination)
        {
            if (property.DestinationProperty == null)
                throw new ArgumentOutOfRangeException("property");
            if (property.MappingFunction == null)
                property.DestinationProperty.SetValue(destination, property.SourceProperty.GetValue(source, null), null);
            else
                property.DestinationProperty.SetValue(destination, property.MappingFunction(source), null);
        }

        private static readonly List<object> Maps = new List<object>();

        #endregion
    }

    public static class Yam<TSource, TDestination>
    {
        public static TypeMap<TSource, TDestination> For<TProperty>(
            Expression<Func<TDestination, TProperty>> destinationPropertyExpression,
            Expression<Func<TSource, TProperty>> mappingExpression)
        {
            var map = Yam.GetMap<TSource, TDestination>();
            if (map == null)
                Yam.CreateMap<TSource, TDestination>();

            map = Yam.For<TSource, TDestination, TProperty>(
                destinationPropertyExpression,
                mappingExpression);

            return map;
        }

        public static TDestination Map(TSource source)
        {
            var result = Yam.Map<TSource, TDestination>(source);
            return result;
        }
    }
}
