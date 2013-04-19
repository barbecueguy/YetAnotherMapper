using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;

namespace Yams
{
    public class TypeMap
    {
        public Type SourceType;
        public Type DestinationType;        

        public TypeMap()
        {
            this.propertyMaps = new List<PropertyMap>();
        }

        public TypeMap(Type sourceType, Type destinationType)
            : this()
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");
            if (destinationType == null)
                throw new ArgumentNullException("destinationType");

            this.SourceType = sourceType;
            this.DestinationType = destinationType;
        }

        public ReadOnlyCollection<PropertyMap> PropertyMaps { get { return new ReadOnlyCollection<PropertyMap>(propertyMaps); } }

        public TypeMap Add(PropertyMap propertyMap)
        {
            var pm = propertyMaps.FirstOrDefault(mp => mp.DestinationProperty == propertyMap.DestinationProperty);
            if (pm != null)
            {
                propertyMaps.Remove(pm);
            }

            propertyMaps.Add(propertyMap);
            return this;
        }

        public object Map(object source)
        {
            object destination = Activator.CreateInstance(DestinationType);
            var destinationProperties =
                DestinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.GetIndexParameters().Any() == false && prop.GetSetMethod() != null);
            foreach (var property in destinationProperties)
            {
                    var propertyMap = propertyMaps.FirstOrDefault(pm => pm.DestinationProperty == property);
                    if (propertyMap == null)
                    {
                        propertyMap = new PropertyMap
                        {
                            DestinationProperty = property,
                            SourceProperty = SourceType.GetProperty(property.Name)
                        };

                        propertyMaps.Add(propertyMap);
                    }

                    propertyMap.Map(source, destination);
            }

            return destination;
        }

        private readonly List<PropertyMap> propertyMaps;
    }
}