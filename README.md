Yam is a basic, simple, concise mapping tool.

I've been using .NET mapping tools (e.g. AutoMapper) for years, but I often find the existing tools to be too complex, 
too verbose, too feature rich. Most of the time I just want a simple tool to do simple mappings with a simple, concise 
syntax. This is my attempt at such a tool.

# Common Usage
    
*The most common usage:*

    // create a simple mapping
    Yam.CreateMap<SaleItem, Product>;
    
    // map the object
    SaleItem saleItem = new SaleItem { ... };
    Product product = Yam.Map<Product>(saleItem);
    
*The simplest usage:*

    SaleItem saleItem = new SaleItem { ... };
    Product product = Yam<SaleItem, Product>.Map(saleItem);
    
*Chain method calls in order to create more 'fluent' code:*
    
    // create a mapping and add a property mapping to it.
    Yam<SaleItem, Product>
      .Use(saleItem => saleItem.Weight, product => product.ShippingWeight)
      .Use(saleItem => saleItem.Description.Replace("SaleItem", "Product"), product => product.Description);
    
    SaleItem saleItem = new SaleItem { ... };
    Product product = Yam.Map<Product>(saleItem);
    
    // or do everything at once    
    Product product = Yam<SaleItem, Product>
      .Use(saleItem => saleItem.Weight, product => product.ShippingWeight)
      .Use(saleItem => saleItem.Description.Replace("SaleItem", "Product"), product => product.Description);
      .Map(expected);
 
# Api
## Generic Methods

*The follwoing generic methods exist on the `Yam<TSource, TDestination>` class, and are the syntactic sugar that, in my mind,
bring the simple interface together.*

    public static TypeMap<TSource,TDestination> Use<TSourceProperty, TDestinationProperty>(
            Expression<Func<TSource, TSourceProperty>> sourceExpression,
            Expression<Func<TDestination, TDestinationProperty>> destinationExpression)
    public static TDestination Map(TSource source)
    
    
*The following generic methods exist on the `Yam` class and help to provide a simpler interface; or (in the case of AddPropertyMap) 
building blocks for a higher level generic method.*
    
    public static TypeMap CreateMap<TSource, TDestination>()
    public static TypeMap GetMap<TSource, TDestination>()
    public static TypeMap AddPropertyMap<TSource, TDestination, TSourceProperty, TDestinationProperty>(
            Expression<Func<TSource, TSourceProperty>> sourceExpression,
            Expression<Func<TDestination, TDestinationProperty>> destinationExpression)
    public static TDestination Map<TDestination>(object source)

## Non-Generic Methods

*The non-generic methods provide the basic api and are the building blocks for the framework. All higher-level methods
utilize the non-generic methods to do their work.*

    public static TypeMap CreateMap(Type sourceType, Type destinationType)
    public static TypeMap GetMap(Type sourceType, Type destinationType)
    public static object Map(object source, Type destinationType)
    public static TypeMap AddPropertyMap(
            Type sourceType, 
            Type destinationType, 
            string sourcePropertyName, 
            string destinationPropertyName)
    public static TypeMap AddPropertyMap(
            Type sourceType,
            Type destinationType,
            Func<object, object> mappingFunction,
            string destinationPropertyName)
    public static void Clear()
    
