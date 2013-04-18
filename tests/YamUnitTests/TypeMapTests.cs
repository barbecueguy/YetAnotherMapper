using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yams;
using YamTestClasses;

namespace SimpleMapperUnitTests
{
    [TestClass]
    public class TypeMapTests
    {
        [TestMethod]
        public void Map_MapsCommonlyNamedProperties_GivenTwoDifferentTypes()
        {
            Product source = TestData.Product;
            var map = new TypeMap { SourceType = typeof(Product), DestinationType = typeof(SaleItem) };
            var result = map.Map(source);

            Assert.IsInstanceOfType(result, typeof(SaleItem));
            var destination = result as SaleItem;
            Assert.AreEqual(destination.Description, source.Description);
            Assert.AreEqual(destination.Id, source.Id);
            Assert.AreEqual(destination.Quantity, 0);
        }

        [TestMethod]
        public void Map_MapsAllProperties_GivenTwoObjectsOfDifferentTypesAndAMappingFunction()
        {
            Product source = TestData.Product;
            TypeMap map = new TypeMap { SourceType = typeof(Product), DestinationType = typeof(SaleItem) };
            map.Add(new PropertyMap
            {
                DestinationProperty = map.DestinationType.GetProperty("Quantity"),
                MappingFunction = o => 1
            });
            map.Add(new PropertyMap
            {
                DestinationProperty = map.DestinationType.GetProperty("ShippingWeight"),
                MappingFunction = o => ((Product)o).Weight
            });

            var result = map.Map(source);

            Assert.IsInstanceOfType(result, typeof(SaleItem));
            var destination = result as SaleItem;
            Assert.AreEqual(source.Description, destination.Description, "Expected descriptions to be equal, but found {0}source: {1}{0} and destination: {1}", Environment.NewLine, source.Description, destination.Description);
            Assert.AreEqual(destination.Id, source.Id);
            Assert.AreEqual(destination.Quantity, 1);
        }

        [TestMethod]
        public void Map_MapsAllProperties_GivenTwoObjectsOfDifferentTypesAndASourceAndDestinationProperty()
        {
            Product source = TestData.Product;
            TypeMap map = new TypeMap { DestinationType = typeof(SaleItem), SourceType = typeof(Product) };
            map.Add(new PropertyMap
            {
                DestinationProperty = map.DestinationType.GetProperty("Quantity"),
                MappingFunction = o => 1
            });
            map.Add(new PropertyMap
            {
                DestinationProperty = map.DestinationType.GetProperty("ShippingWeight"),
                SourceProperty = map.SourceType.GetProperty("Weight")
            });

            var result = map.Map(source);

            Assert.IsInstanceOfType(result, typeof(SaleItem));
            var destination = result as SaleItem;
            Assert.AreEqual(destination.Description, source.Description);
            Assert.AreEqual(destination.Id, source.Id);
            Assert.AreEqual(destination.Quantity, 1);
        }
    }
}