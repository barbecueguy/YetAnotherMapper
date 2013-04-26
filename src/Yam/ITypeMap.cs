using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Yams
{
    public interface ITypeMap
    {
        Type SourceType { get; }
        Type DestinationType { get; }
        List<PropertyMap> PropertyMaps { get; }
        ITypeMap Add(PropertyMap propertyMap);
        object Map(object source);
    }
}