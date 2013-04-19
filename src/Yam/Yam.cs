using System;
using System.Collections.Generic;
using System.Linq;

namespace Yams
{
    public static class Yam
    {
        private static List<TypeMap> maps = new List<TypeMap>();

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
}
