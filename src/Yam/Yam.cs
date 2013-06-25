using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Yams
{
    public static class Yam
    {
        #region Basic Functions
        public static TypeMap CreateMap(Type sourceType, Type destinationType)
        {
            TypeMap map = Yam.GetMap(sourceType, destinationType);
            if (map != null)
                Yam.maps.Remove(map);

            map = new TypeMap(sourceType, destinationType);
            foreach (var name in Yam.GetCommonPropertyNames(sourceType, destinationType))
            {
                PropertyMap propertyMap = new PropertyMap(sourceType.GetProperty(name), destinationType.GetProperty(name));
                map.PropertyMaps.Add(propertyMap);
            }

            maps.Add(map);
            return map;
        }

        public static TypeMap GetMap(Type sourceType, Type destinationType)
        {
            TypeMap map = Yam.maps.SingleOrDefault(m => m.SourceType == sourceType && m.DestinationType == destinationType);
            return map;
        }

        public static object Map(object source, Type destinationType)
        {
            var sourceType = source.GetType();
            TypeMap map = Yam.GetMap(sourceType, destinationType);
            if (map == null)
            {
                if (sourceType.IsGenericEnumerable() && destinationType.IsGenericCollection())
                {
                    return Yam.MapLists(destinationType, source);
                }

                if (sourceType.Name == destinationType.Name)
                {
                    if (source is ICloneable)
                        return ((ICloneable)source).Clone();

                    return source; // TODO: actually create a new one at some point
                }

                if (sourceType.IsConvertible())
                    return Convert.ChangeType(source, destinationType);

                throw new Exception(string.Format("No map defined from {0} to {1}", source.GetType(), destinationType));
            }

            var destination = Activator.CreateInstance(destinationType);
            foreach (var propertyMap in map.PropertyMaps)
                Yam.MapProperty(propertyMap, source, destination);

            return destination;
        }

        public static TypeMap AddPropertyMap(
            Type sourceType,
            Type destinationType,
            string sourcePropertyName,
            string destinationPropertyName)
        {
            TypeMap typeMap = Yam.GetMap(sourceType, destinationType);
            if (typeMap == null)
                throw new Exception(string.Format("No map defined from {0} to {1}", sourceType, destinationType));

            PropertyInfo sourceProperty = sourceType.GetProperty(sourcePropertyName);
            PropertyInfo destinationProperty = destinationType.GetProperty(destinationPropertyName);
            PropertyMap propertyMap = new PropertyMap(sourceProperty, destinationProperty);

            var currentPropertyMap = typeMap.PropertyMaps.SingleOrDefault(pm => pm.DestinationPropertyName == destinationPropertyName);
            if (currentPropertyMap != null)
                typeMap.PropertyMaps.Remove(currentPropertyMap);

            typeMap.PropertyMaps.Add(propertyMap);
            return typeMap;
        }

        public static TypeMap AddPropertyMap(
            Type sourceType,
            Type destinationType,
            Func<object, object> mappingFunction,
            string destinationPropertyName)
        {
            TypeMap typeMap = Yam.GetMap(sourceType, destinationType);
            if (typeMap == null)
                throw new Exception(string.Format("No map defined from {0} to {1}", sourceType, destinationType));

            PropertyInfo destinationProperty = destinationType.GetProperty(destinationPropertyName);
            Type sourcePropertyType = mappingFunction.Method.ReturnType;
            PropertyMap propertyMap = new PropertyMap(sourcePropertyType, destinationProperty, mappingFunction);

            var currentPropertyMap = typeMap.PropertyMaps.SingleOrDefault(pm => pm.DestinationPropertyName == destinationPropertyName);
            if (currentPropertyMap != null)
                typeMap.PropertyMaps.Remove(currentPropertyMap);

            typeMap.PropertyMaps.Add(propertyMap);
            return typeMap;
        }

        public static void Clear()
        {
            Yam.maps.Clear();
        }
        #endregion

        #region Fluent Functions
        public static TypeMap CreateMap<TSource, TDestination>()
        {
            return Yam.CreateMap(typeof(TSource), typeof(TDestination));
        }

        public static TypeMap GetMap<TSource, TDestination>()
        {
            return Yam.GetMap(typeof(TSource), typeof(TDestination));
        }

        public static TypeMap AddPropertyMap<TSource, TDestination, TSourceProperty, TDestinationProperty>(
            Expression<Func<TSource, TSourceProperty>> sourceExpression,
            Expression<Func<TDestination, TDestinationProperty>> destinationExpression)
        {
            var destinationPropertyName = ((MemberExpression)destinationExpression.Body).Member.Name;
            var mappingFunction = sourceExpression.Compile();
            return Yam.AddPropertyMap(typeof(TSource), typeof(TDestination), o => mappingFunction((TSource)o), destinationPropertyName);
        }

        public static TDestination Map<TDestination>(object source)
        {
            return (TDestination)Yam.Map(source, typeof(TDestination));
        }
        #endregion

        private static void MapProperty(PropertyMap propertyMap, object source, object destination)
        {
            var sourceType = source.GetType();
            var destinationType = destination.GetType();

            var destinationProperty = destinationType.GetProperty(propertyMap.DestinationPropertyName);
            var sourceProperty = sourceType.GetProperty(propertyMap.SourcePropertyName);

            object sourceValue = null;
            if (string.IsNullOrEmpty(propertyMap.SourcePropertyName))
                sourceValue = propertyMap.MappingFunction(source);
            else
                sourceValue = sourceProperty.GetValue(source, null);

            try
            {
                destinationProperty.SetValue(destination, sourceValue, null);
            }
            catch (ArgumentException ex)
            {
                TypeMap typeMap = Yam.GetMap(sourceProperty.PropertyType, destinationProperty.PropertyType);
                if (typeMap != null)
                {
                    var destinationValue = Yam.Map(sourceValue, destinationProperty.PropertyType);
                    destinationProperty.SetValue(destination, destinationValue, null);
                    return;
                }

                if (sourceProperty.PropertyType.IsGenericEnumerable() && destinationProperty.PropertyType.IsGenericCollection())
                {
                    var destinationValue = Yam.MapLists(destinationProperty.PropertyType, sourceValue);
                    destinationProperty.SetValue(destination, destinationValue, null);
                    return;
                }

                if (ex.Message.Contains("cannot be converted"))
                {
                    if (sourceValue is IConvertible)
                    {
                        destinationProperty.SetValue(destination, Convert.ChangeType(sourceValue, destinationProperty.PropertyType), null);
                        return;
                    }
                }

                throw;
            }
        }

        private static IEnumerable<string> GetCommonPropertyNames(Type sourceType, Type destinationType)
        {
            var sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetIndexParameters().Any() == false)
                .Select(p => p.Name);
            var destinationProperties = destinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetIndexParameters().Any() == false)
                .Select(p => p.Name);
            var commonPropertyNames = sourceProperties.Intersect(destinationProperties);
            return commonPropertyNames;
        }

        private static object MapLists(Type destinationType, object sourceList)
        {
            var destinationParameter = destinationType.GetGenericArguments()[0];

            var destinationList = Activator.CreateInstance(destinationType);
            foreach (var item in ((IEnumerable)sourceList))
            {
                destinationType.GetMethod("Add").Invoke(destinationList, new[] { Yam.Map(item, destinationParameter) });
            }

            return destinationList;
        }

        private static readonly List<TypeMap> maps = new List<TypeMap>();
    }

    public static class Yam<TSource, TDestination>
    {
        public static TypeMap<TSource,TDestination> Use<TSourceProperty, TDestinationProperty>(
            Expression<Func<TSource, TSourceProperty>> sourceExpression,
            Expression<Func<TDestination, TDestinationProperty>> destinationExpression)
        {
            var typeMap = Yam.GetMap<TSource, TDestination>();
            if (typeMap == null)
                typeMap = Yam.CreateMap<TSource, TDestination>();
            Yam.AddPropertyMap<TSource, TDestination, TSourceProperty, TDestinationProperty>(sourceExpression, destinationExpression);
            return new TypeMap<TSource, TDestination>();
        }

        public static TDestination Map(TSource source)
        {
            var map = Yam.GetMap<TSource, TDestination>();
            if (map == null)
                map = Yam.CreateMap<TSource, TDestination>();
            return Yam.Map<TDestination>(source);
        }
    }
}
