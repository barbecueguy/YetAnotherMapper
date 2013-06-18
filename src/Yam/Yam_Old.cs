using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Yams
{
    public static class Yam_Old
    {
        //public static TypeMap<TSource, TDestination> CreateMap<TSource, TDestination>(TSource source, TDestination destination)
        //{
        //    return CreateMap<TSource, TDestination>();
        //}

        //public static TypeMap<TSource, TDestination> CreateMap<TSource, TDestination>()
        //{
        //    var map = GetMap<TSource, TDestination>();
        //    if (map == null)
        //    {
        //        map = new TypeMap<TSource, TDestination>();
        //        Maps.Add(map);
        //    }

        //    return map;
        //}

        //public static TypeMap<TSource, TDestination> GetMap<TSource, TDestination>()
        //{
        //    var destinationType = typeof(TDestination);
        //    var sourceType = typeof(TSource);
        //    var map = (TypeMap<TSource, TDestination>)GetMap(sourceType, destinationType);
        //    return map;
        //}

        //public static TypeMap<TSource, TDestination> For<TSource, TDestination, TSourceProperty, TDestinationProperty>(
        //    Expression<Func<TDestination, TDestinationProperty>> destinationPropertyExpression,
        //    Expression<Func<TSource, TSourceProperty>> mappingExpression)
        //{
        //    var map = Yam_Old.GetMap<TSource, TDestination>();
        //    if (map == null)
        //        Yam_Old.CreateMap<TSource, TDestination>();

        //    map = Yam_Old.AddPropertyMap<TSource, TDestination, TSourceProperty, TDestinationProperty>(
        //        destinationPropertyExpression,
        //        mappingExpression);

        //    return map;
        //}

        //public static TDestination Map<TSource, TDestination>(TSource source)
        //{
        //    return (TDestination)Yam_Old.Map(source, typeof(TDestination));
        //}

        //public static TDestination Map<TDestination>(object source)
        //{
        //    return (TDestination)Yam_Old.Map(source, typeof(TDestination));
        //}

        //public static object Map(object source, Type destinationType)
        //{
        //    var sourceType = source.GetType();
        //    var destination = Activator.CreateInstance(destinationType);
        //    var map = Yam_Old.GetMap(sourceType, destinationType);
        //    if (map == null)
        //        map = CreateMap(source, destination);

        //    var propertyMaps = (IEnumerable<PropertyMap>)map.GetType().GetProperty("PropertyMaps").GetValue(map, null);
        //    foreach (var property in propertyMaps)
        //    {
        //        MapProperty(property, source, destination);
        //    }

        //    return destination;
        //}

        //public static void Clear()
        //{
        //    Maps.Clear();
        //}

        //#region private
        //private static TypeMap<TSource, TDestination> AddPropertyMap<TSource, TDestination, TSourceProperty, TDestinationProperty>(
        //    Expression<Func<TDestination, TDestinationProperty>> destinationPropertyExpression,
        //    Expression<Func<TSource, TSourceProperty>> mappingExpression)
        //{
        //    var map = GetMap<TSource, TDestination>();
        //    if (map == null)
        //        throw new ArgumentOutOfRangeException(
        //            "destinationPropertyExpression",
        //            string.Format("No mapping found from {0} to {1}", typeof(TSource), typeof(TDestination)));

        //    var destinationPropertyName = destinationPropertyExpression.Body.NodeType == ExpressionType.MemberAccess ? ((MemberExpression)destinationPropertyExpression.Body).Member.Name : "";
        //    var destinationPropertyInfo = typeof(TDestination).GetProperty(destinationPropertyName);
        //    var sourcePropertyExpression = mappingExpression as MemberExpression;
        //    var sourcePropertyName = sourcePropertyExpression == null ? null : sourcePropertyExpression.Member.Name;
        //    var sourcePropertyInfo = sourcePropertyName == null ? null : typeof(TSource).GetProperty(sourcePropertyName);
        //    var propertyMap = new PropertyMap(
        //        sourcePropertyInfo,
        //        destinationPropertyInfo,
        //        src => mappingExpression.Compile()((TSource)src));
        //    map.Add(propertyMap);
        //    return map;
        //}

        //private static object GetMap(Type sourceType, Type destinationType)
        //{
        //    var map = Maps.FirstOrDefault(mp =>
        //    {
        //        var type = mp.GetType();
        //        var mapSourceType = type.GetProperty("SourceType").GetValue(mp, null);
        //        var mapDestinationType = type.GetProperty("DestinationType").GetValue(mp, null);
        //        return mapSourceType == sourceType && mapDestinationType == destinationType;
        //    });

        //    return map;
        //}

        //private static void MapProperty(PropertyMap property, object source, object destination)
        //{
        //    if (property.DestinationProperty == null)
        //        throw new ArgumentOutOfRangeException("property");

        //    object sourceValue = null;
        //    object destinationValue = null;
        //    if (property.MappingFunction == null)
        //        sourceValue = property.SourceProperty.GetValue(source, null);
        //    else
        //        sourceValue = property.MappingFunction(source);

        //    var valueType = sourceValue.GetType();
        //    if (valueType != property.DestinationProperty.PropertyType)
        //    {
        //        // we got work to do
        //        // do I already have a mapping?
        //        var typeMap = Yam_Old.GetMap(valueType, property.DestinationProperty.PropertyType);
        //        if (typeMap != null)
        //        {
        //            destinationValue = Yam_Old.Map(sourceValue, property.DestinationProperty.PropertyType);
        //        }
        //        else
        //        {
        //            var decimalType = typeof(decimal);
        //            if (valueType.IsPrimitiveOr(decimalType))
        //            {
        //                destinationValue = sourceValue;
        //            }
        //            else
        //            {
        //                if (property.DestinationProperty.PropertyType.IsGenericType && valueType.IsGenericType)
        //                {
        //                    var iEnumerableTypeDefinition = valueType.GetInterfaces().FirstOrDefault(it => it.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        //                    var iCollectionTypeDefinition = property.DestinationProperty.PropertyType.GetInterfaces().FirstOrDefault(it => it.GetGenericTypeDefinition() == typeof(ICollection<>));

        //                    // if the source is enumerable and the destination is a collection
        //                    if (iEnumerableTypeDefinition != null && iCollectionTypeDefinition != null)
        //                    {
        //                        var sourceTypeParameterType = valueType.GetGenericArguments().First();
        //                        var destinationTypeParameterType = property.DestinationProperty.PropertyType.GetGenericArguments().First();

        //                        var constructed = (ICollection<object>)Activator.CreateInstance(iCollectionTypeDefinition.MakeGenericType(destinationTypeParameterType));

        //                        // if i have a mapping for it
        //                        typeMap = Yam_Old.GetMap(sourceTypeParameterType, destinationTypeParameterType);
        //                        if (typeMap != null)
        //                        {
        //                            foreach (var item in ((IEnumerable<object>)sourceValue))
        //                                constructed.Add(Yam_Old.Map<object>(item));
        //                        }
        //                        else
        //                            // if they are both primitives
        //                            if (sourceTypeParameterType.IsPrimitiveOr(decimalType) && destinationTypeParameterType.IsPrimitiveOr(decimalType))
        //                            {
        //                                foreach (var item in ((IEnumerable<object>)sourceValue))
        //                                {
        //                                    constructed.Add(item);
        //                                }
        //                            }

        //                        destinationValue = constructed;
        //                    }

        //                }

        //            }
        //        }
        //    }
        //    else
        //    {
        //        destinationValue = sourceValue;
        //    }

        //    property.DestinationProperty.SetValue(destination, destinationValue, null);
        //}

        //private static readonly List<object> Maps = new List<object>();

        //#endregion
    }

    public static class Yam<TSource, TDestination>
    {
        //public static TypeMap<TSource, TDestination> For<TSourceProperty, TDestinationProperty>(
        //    Expression<Func<TDestination, TDestinationProperty>> destinationPropertyExpression,
        //    Expression<Func<TSource, TSourceProperty>> mappingExpression)
        //{
        //    var map = Yam_Old.GetMap<TSource, TDestination>();
        //    if (map == null)
        //        Yam_Old.CreateMap<TSource, TDestination>();

        //    map = Yam_Old.For<TSource, TDestination, TSourceProperty, TDestinationProperty>(
        //        destinationPropertyExpression,
        //        mappingExpression);

        //    return map;
        //}

        //public static TDestination Map(TSource source)
        //{
        //    var result = Yam_Old.Map<TSource, TDestination>(source);
        //    return result;
        //}
    }
}




//public sealed class TypeMap<TSource, TDestination>
//{
//    private readonly List<PropertyMap> propertyMaps;

//    public Type SourceType { get; private set; }
//    public Type DestinationType { get; private set; }
//    public IEnumerable<PropertyMap> PropertyMaps
//    {
//        get
//        {
//            return propertyMaps.ToArray();
//        }
//    }

//    internal TypeMap()
//    {
//        propertyMaps = new List<PropertyMap>();
//        this.DestinationType = typeof(TDestination);
//        this.SourceType = typeof(TSource);
//    }

//    public TypeMap<TSource, TDestination> Add(PropertyMap propertyMap)
//    {
//        var pm = propertyMaps.FirstOrDefault(mp => mp.DestinationProperty == propertyMap.DestinationProperty);
//        if (pm != null)
//        {
//            propertyMaps.Remove(pm);
//        }

//        propertyMaps.Add(propertyMap);
//        return this;
//    }

//    public TypeMap<TSource, TDestination> For<TProperty>(
//        Expression<Func<TDestination, TProperty>> destination,
//        Expression<Func<TSource, TProperty>> mappingFunction)
//    {
//        var map = Yam_Old.For(destination, mappingFunction);
//        return map;
//    }

//    public TDestination Map(TSource source)
//    {
//        var result = Yam_Old.Map<TSource, TDestination>(source);
//        return result;
//    }
//}