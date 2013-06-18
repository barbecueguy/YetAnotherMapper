using System;
using System.Linq;

namespace Yams
{
    public static class Extensions
    {
        public static bool IsPrimitiveOr(this Type t, params Type[] type)
        {
            return t.IsPrimitive || type.Contains(t);
        }
    }
}
