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

            map.PropertyMaps.AddRange(GetDefaultPropertyMaps(sourceType, destinationType));
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
                if (Yam.IsGenericEnumerable(sourceType) && Yam.IsGenericCollection(destinationType))
                {
                    return Yam.MapLists(sourceType, destinationType, source);
                }

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

        public static TypeMap AddPropertyMap<TSource, TDestination>(string sourcePropertyName, string destinationPropertyName)
        {
            return AddPropertyMap(typeof(TSource), typeof(TDestination), sourcePropertyName, destinationPropertyName);
        }

        public static TypeMap AddPropertyMap<TSource, TDestination>(Func<object, object> mappingFunction, string destinationPropertyName)
        {
            return AddPropertyMap(typeof(TSource), typeof(TDestination), mappingFunction, destinationPropertyName);
        }

        public static TypeMap AddPropertyMap<TSource, TDestination>(Expression<Func<object, object>> sourceExpression, Expression<Func<object, object>> destinationExpression)
        {
            return Yam.AddPropertyMap<TSource, TDestination>(sourceExpression.Compile(), ((MemberExpression)destinationExpression.Body).Member.Name);
        }

        public static TDestination Map<TDestination>(object source)
        {
            TypeMap map = Yam.GetMap(source.GetType(), typeof(TDestination));
            if (map == null)
                Yam.CreateMap(source.GetType(), typeof(TDestination));

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

                if (Yam.IsGenericEnumerable(sourceProperty.PropertyType) && Yam.IsGenericCollection(destinationProperty.PropertyType))
                {
                    var destinationValue = Yam.MapLists(sourceProperty.PropertyType, destinationProperty.PropertyType, sourceValue);
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

        private static List<PropertyMap> GetDefaultPropertyMaps(Type sourceType, Type destinationType)
        {
            var sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetIndexParameters().Any() == false);
            var destinationProperties = destinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetIndexParameters().Any() == false);
            var commonProperties = from sourceProperty in sourceProperties
                                   join destinationProperty in destinationProperties on sourceProperty.Name equals destinationProperty.Name
                                   select new PropertyMap(sourceProperty, destinationProperty);
            return commonProperties.ToList();
        }

        private static bool IsGenericEnumerable(Type sourceType)
        {
            if (sourceType.IsGenericType)
            {
                var definition = sourceType.GetGenericTypeDefinition();
                var interfaces = definition.GetInterfaces().Select(i => i.Name);
                if (interfaces.Contains(typeof(IEnumerable<>).Name))
                    return true;
            }

            return false;
        }

        private static bool IsGenericCollection(Type destinationType)
        {
            if (destinationType.IsGenericType)
            {
                var definition = destinationType.GetGenericTypeDefinition();
                var interfaces = definition.GetInterfaces().Select(i => i.Name);
                if (interfaces.Contains(typeof(ICollection<>).Name))
                    return true;
            }

            return false;
        }

        private static bool IsConvertible(Type sourceType)
        {
            var interfaces = sourceType.GetInterfaces().Select(i => i.Name);
            return interfaces.Contains("IConvertible");
        }

        private static object MapLists(Type sourceType, Type destinationType, object sourceList)
        {
            var sourceParameter = sourceType.GetGenericArguments()[0];
            var destinationParameter = destinationType.GetGenericArguments()[0];
            TypeMap map = Yam.GetMap(sourceParameter, destinationParameter);            
            if (map == null && !IsConvertible(sourceParameter))
                throw new Exception(string.Format("No map defined from {0} to {1}", sourceType, destinationType));

            var destinationList = Activator.CreateInstance(destinationType);
            foreach (var item in ((IEnumerable)sourceList))
            {
                destinationType.GetMethod("Add").Invoke(destinationList, new[] { Yam.Map(item, destinationParameter) });
            }

            return destinationList;
        }

        private static readonly List<TypeMap> maps = new List<TypeMap>();
    }
}
