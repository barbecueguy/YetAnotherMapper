using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Yams
{
    public static class Yam
    {
        private static readonly List<ITypeMap> maps = new List<ITypeMap>();

        public static ITypeMap AddMap(ITypeMap map)
        {
            var it = maps.FirstOrDefault(mp => mp.SourceType == map.SourceType && mp.DestinationType == map.DestinationType);
            if (ReferenceEquals(it, map))
                return map;

            maps.Remove(it);
            maps.Add(map);
            return map;
        }

        public static ITypeMap CreateMap<TSource, TDestination>()
        {            
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            var map = maps.FirstOrDefault(mp => mp.SourceType == sourceType && mp.DestinationType == destinationType);
            if (map == null)
            {
                map = new TypeMap<TSource, TDestination>();
                maps.Add(map);
            }

            return map;
        }

        public static ITypeMap CreateMap<TSource, TDestination>(TSource source, TDestination destination)
        {
            return CreateMap<TSource, TDestination>();
        }        

        public static ITypeMap GetMap<TSource, TDestination>()
        {
            return GetMap(typeof(TSource), typeof(TDestination));
        }

        public static ITypeMap GetMap(Type sourceType, Type destinationType)
        {
            var map = maps.FirstOrDefault(mp => mp.SourceType == sourceType && mp.DestinationType == destinationType);
            return map;
        }

        public static ITypeMap RemoveMap(ITypeMap map)
        {
            maps.Remove(map);
            return map;
        }

        public static void Clear()
        {
            maps.Clear();
        }

        public static TDestination Map<TDestination>(object o)
        {
            var sourceType = o.GetType();
            var destinationType = typeof(TDestination);
            var map = GetMap(sourceType, destinationType);
            if (map == null)
            {
                map = CreateMap(o, default(TDestination));
                map = AddMap(map);
            }

            return (TDestination)map.Map(o);
        }
    }

    public static class Yam<TSource>
    {
        public static TypeMap<TSource, TDestination> Map<TDestination>()
        {
            return new TypeMap<TSource, TDestination>();
        }

        public static TDestination Map<TDestination>(TSource source)
        {
            return Yam<TSource, TDestination>.Map(source);
        }
    }

    public static class Yam<TSource, TDestination>
    {
        public static TypeMap<TSource, TDestination> AddMap(TypeMap<TSource, TDestination> map)
        {
            map = (TypeMap<TSource, TDestination>)Yam.AddMap(map);
            return map;
        }

        public static TypeMap<TSource, TDestination> CreateMap()
        {
            var map = new TypeMap<TSource, TDestination>();
            map = (TypeMap<TSource, TDestination>)Yam.AddMap(map);
            return map;
        }

        public static TypeMap<TSource, TDestination> CreateMap(ITypeMap map)
        {
            if (map == null)
                throw new ArgumentNullException("map");

            TypeMap<TSource, TDestination> result = null;
            if (map is TypeMap<TSource, TDestination> == false)
            {
                result = new TypeMap<TSource, TDestination>(map);
            }

            result = (TypeMap<TSource, TDestination>)Yam.AddMap(result);
            return result;
        }

        public static TypeMap<TSource, TDestination> For<TProperty>(
            Expression<Func<TDestination, TProperty>> destination,
            Expression<Func<TSource, TProperty>> mappingFunction)
        {
            TypeMap<TSource, TDestination> newMap = null;
            var existingMap = Yam.GetMap<TSource, TDestination>();
            if (existingMap == null)
            {
                newMap = CreateMap();
            }
            else if (existingMap is TypeMap<TSource, TDestination> == false)
            {
                newMap = CreateMap(existingMap);
            }

            newMap = AddMap(newMap);

            var propertyExpression = (MemberExpression)destination.Body;
            var destinationProperty = newMap.DestinationType.GetProperty(propertyExpression.Member.Name);

            var propertyMap = new PropertyMap(destinationProperty, o => mappingFunction.Compile()((TSource)o));
            newMap.Add(propertyMap);

            return newMap;
        }

        public static TypeMap<TSource, TDestination> GetMap()
        {
            var map = Yam.GetMap<TSource, TDestination>() as TypeMap<TSource, TDestination>;
            return map;
        }

        public static TDestination Map(TSource source)
        {            
            return Yam.Map<TDestination>(source);
        }
    }
}
