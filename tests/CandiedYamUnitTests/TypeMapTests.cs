using System;
using CandiedYams;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yams;
using YamTestClasses;

namespace CandiedYamUnitTests
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
        public void GenericTypeMap_Constructor_SetsSourceAndDestinationPropertyTypesToPassedTypeMapPropertyTypes()
        {
            TypeMap map = new TypeMap(typeof(SaleItem), typeof(Product));
            TypeMap<SaleItem, Product> genericMap = new TypeMap<SaleItem, Product>(map);

            Assert.AreEqual(map.DestinationType, genericMap.DestinationType);
            Assert.AreEqual(map.SourceType, genericMap.SourceType);
        }

        [TestMethod]
        public void TypeMap_Map_MapsCommonlyNamedProperties_GivenTwoDifferentTypes()
        {
            Product source = TestData.Product;
            var result = new TypeMap<Product, SaleItem>().Map(source);

            Assert.IsInstanceOfType(result, typeof(SaleItem));
            Assert.AreEqual(result.Description, source.Description);
            Assert.AreEqual(result.Id, source.Id);
            Assert.AreEqual(result.Quantity, 0);
        }

        [TestMethod]
        public void TypeMap_Map_MapsAllProperties_GivenTwoObjectsOfDifferentTypesAndMappingFunction()
        {
            Product source = TestData.Product;
            var result = new TypeMap<Product, SaleItem>()
                .For(dest => dest.Id, src => src.Id)
                .For(dest => dest.Description, src => src.Description)
                .For(dest => dest.ShippingWeight, src => src.Weight)
                .For(dest => dest.Quantity, src => 1)
                .Map(source);

            Assert.IsInstanceOfType(result, typeof(SaleItem));
            Assert.AreEqual(result.Description, source.Description);
            Assert.AreEqual(result.Id, source.Id);
            Assert.AreEqual(result.Quantity, 1);
        }

        [TestMethod]
        public void TypeMap_Map_MapsAllProperties_GivenTwoObjectsOfDifferentTypesAndASourcePropertyAndNonLambdaMappingFunction()
        {
            TypeMap<Product, SaleItem> map = new TypeMap<Product, SaleItem>()
                .For(dest => dest.Description, src => TestData.TestStringMappingFunction(src))
                .For(dest => dest.ShippingWeight, src => src.Weight)
                .For(dest => dest.Id, src => src.Id);
            Product source = TestData.Product;

            var destination = map.Map(source);

            Assert.IsInstanceOfType(destination, typeof(SaleItem));
            Assert.AreEqual(destination.Description, source.Description);
            Assert.AreEqual(destination.Id, source.Id);
            Assert.AreEqual(destination.Quantity, 0);
        }

        [TestMethod]
        public void TypeMap_Map_MapsCorrectly_GivenANewPropertyMapForAnExistingMap()
        {
            TypeMap<Product, SaleItem> map = new TypeMap<Product, SaleItem>()
                .For(dest => dest.Description, src => src.Description)
                .For(dest => dest.ShippingWeight, src => src.Weight)
                .For(dest => dest.Id, src => src.Id)
                .For(dest => dest.Quantity, src => 23);
            Product source = TestData.Product;

            var destination = map.Map(source);

            Assert.IsInstanceOfType(destination, typeof(SaleItem));
            Assert.AreEqual(source.Description, destination.Description);
            Assert.AreEqual(source.Id, destination.Id);
            Assert.AreEqual(source.Weight, destination.ShippingWeight);
            Assert.AreEqual(23, destination.Quantity);

            map.For(dest => dest.Quantity, src => 42)
                .For(dest => dest.Description, src => "TestDescription")
                .For(dest => dest.Id, src => 69)
                .For(dest => dest.ShippingWeight, src => 3);

            destination = map.Map(source);

            Assert.IsInstanceOfType(destination, typeof(SaleItem));
            Assert.AreEqual("TestDescription", destination.Description);
            Assert.AreEqual(69, destination.Id);
            Assert.AreEqual(3, destination.ShippingWeight);
            Assert.AreEqual(42, destination.Quantity);
        }
    }
}
