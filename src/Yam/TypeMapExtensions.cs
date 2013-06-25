
namespace Yams
{
    public static class TypeMapExtensions
    {
        public static TDestination Map<TSource, TDestination>(this TypeMap<TSource, TDestination> typeMap, TSource source)
        {
            return Yam.Map<TDestination>(source);
        }
    }
}
