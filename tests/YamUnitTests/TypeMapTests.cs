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
        public void TypeMap_Map_MapsCommonlyNamedProperties_GivenTwoDifferentTypes()
        {
            Product source = TestData.Product;
            var map = new TypeMap<Product, SaleItem>();
            var result = map.Map(source);

            Assert.IsInstanceOfType(result, typeof(SaleItem));
            var destination = result as SaleItem;
            Assert.AreEqual(destination.Description, source.Description);
            Assert.AreEqual(destination.Id, source.Id);
            Assert.AreEqual(destination.Quantity, 0);
        }

        [TestMethod]
        public void TypeMap_Map_MapsAllProperties_GivenTwoObjectsOfDifferentTypesAndAMappingFunction()
        {
            Product source = TestData.Product;
            var map = new TypeMap<Product, SaleItem>();
            map.Add(new PropertyMap(map.DestinationType.GetProperty("Quantity"), o => 1));
            map.Add(new PropertyMap(map.DestinationType.GetProperty("ShippingWeight"), o => ((Product)o).Weight));

            var result = map.Map(source);

            Assert.IsInstanceOfType(result, typeof(SaleItem));
            var destination = result as SaleItem;
            Assert.AreEqual(source.Description, destination.Description, "Expected descriptions to be equal, but found {0}source: {1}{0} and destination: {1}", Environment.NewLine, source.Description, destination.Description);
            Assert.AreEqual(destination.Id, source.Id);
            Assert.AreEqual(destination.Quantity, 1);
        }

        [TestMethod]
        public void TypeMap_Map_MapsAllProperties_GivenTwoObjectsOfDifferentTypesAndASourceAndDestinationProperty()
        {
            Product source = TestData.Product;
            var map = new TypeMap<Product, SaleItem>();
            map.Add(new PropertyMap(
                destinationProperty: map.DestinationType.GetProperty("Quantity"),
                mappingFunction: o => 1));
            map.Add(new PropertyMap(
                destinationProperty: map.DestinationType.GetProperty("ShippingWeight"),
                sourceProperty: map.SourceType.GetProperty("Weight")));

            var result = map.Map(source);

            Assert.IsInstanceOfType(result, typeof(SaleItem));
            var destination = result as SaleItem;
            Assert.AreEqual(destination.Description, source.Description);
            Assert.AreEqual(destination.Id, source.Id);
            Assert.AreEqual(destination.Quantity, 1);
        }

        [TestMethod]
        public void GenericTypeMap_Constructor_SetsSourceAndDestinationPropertyTypesToPassedTypeMapPropertyTypes()
        {
            var expected = new TypeMap<Product, SaleItem>();
            var actual = new TypeMap<Product, SaleItem>(expected);

            Assert.AreEqual(expected.DestinationType, actual.DestinationType);
            Assert.AreEqual(expected.SourceType, actual.SourceType);
        }

        [TestMethod]
        public void GenericTypeMap_Map_MapsCommonlyNamedProperties_GivenTwoDifferentTypes()
        {
            Product source = TestData.Product;
            var result = new TypeMap<Product, SaleItem>().Map(source);

            Assert.IsInstanceOfType(result, typeof(SaleItem));
            Assert.AreEqual(result.Description, source.Description);
            Assert.AreEqual(result.Id, source.Id);
            Assert.AreEqual(result.Quantity, 0);
        }

        [TestMethod]
        public void GenericTypeMap_Map_MapsAllProperties_GivenTwoObjectsOfDifferentTypesAndMappingFunction()
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
        public void GenericTypeMap_Map_MapsAllProperties_GivenTwoObjectsOfDifferentTypesAndASourcePropertyAndNonLambdaMappingFunction()
        {
            TypeMap<Product, SaleItem> map = new TypeMap<Product, SaleItem>()
                .For(dest => dest.Description, src => TestData.TestStringMappingFunction(src))
                .For(dest => dest.ShippingWeight, src => src.Weight)
                .For(dest => dest.Id, src => src.Id);
            Product source = TestData.Product;
            var expectedDescription = TestData.TestStringMappingFunction(source);
            var destination = map.Map(source);

            Assert.IsInstanceOfType(destination, typeof(SaleItem));
            Assert.AreEqual(expectedDescription, destination.Description);
            Assert.AreEqual(source.Id, destination.Id);
            Assert.AreEqual(0, destination.Quantity);
        }

        [TestMethod]
        public void GenericTypeMap_Map_MapsCorrectly_GivenANewPropertyMapForAnExistingMap()
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