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
        [TestInitialize]
        public void Initialize()
        {
            Yam.Clear();
        }

        [TestMethod]
        public void Map_MapsCommonlyNamedProperties_GivenTwoDifferentTypes()
        {
            Product source = TestData.Product;
            var yam = Yam.Map(typeof(Product), typeof(SaleItem));
            var result = yam.Map(source);

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
            TypeMap map = Yam.Map(typeof(Product), typeof(SaleItem));
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
            Assert.AreEqual(destination.Description, source.Description);
            Assert.AreEqual(destination.Id, source.Id);
            Assert.AreEqual(destination.Quantity, 1);
        }

        [TestMethod]
        public void Map_MapsAllProperties_GivenTwoObjectsOfDifferentTypesAndASourceAndDestinationProperty()
        {
            Product source = TestData.Product;
            TypeMap map = Yam.Map(typeof(Product), typeof(SaleItem));
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