using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yams;
using YamTestClasses;

namespace SimpleMapperUnitTests
{
    [TestClass]
    public class YamTests
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Yam.Clear();
        }

        [TestMethod]
        public void Yam_CreateMap_ReturnsTheSameObject_GivenTheSameSourceAndDestinationTypes()
        {
            var sourceType = typeof(Product);
            var destinationType = typeof(SaleItem);
            var expected = Yam.CreateMap(sourceType, destinationType);

            var actual = Yam.CreateMap(sourceType, destinationType);

            Assert.AreSame(expected, actual, "Expected the same reference, but found a different reference");
        }

        [TestMethod]
        public void Yam_CreateMapOfTSourceTDestination_ReturnsTheSameObject_GivenTheSameSourceAndDestinationTypes()
        {
            var expected = Yam.CreateMap<Product,SaleItem>();

            var actual = Yam.CreateMap<Product, SaleItem>();

            Assert.AreSame(expected, actual, "Expected the same reference, but found a different reference");
        }

        [TestMethod]
        public void Yam_Map_ReturnsTheSameMap_WhenAMapForAGivenSourceAndDestinationAlreadyExists()
        {
            var map = new TypeMap<Product, SaleItem>();
            Assert.AreSame(map, Yam.AddMap(map), "Expected the same object reference, but found another");
        }

        [TestMethod]
        public void Yam_MapsCommonlyNamedProperties_GivenTwoDifferentObjectTypes()
        {
            Product source = TestData.Product;
            var destination = Yam<Product, SaleItem>.Map(source);

            Assert.IsInstanceOfType(destination, typeof(SaleItem));
            Assert.AreEqual(source.Description, destination.Description);
            Assert.AreEqual(source.Id, destination.Id);
            Assert.AreEqual(0, destination.Quantity);
        }

        [TestMethod]
        public void Yam_MapsAllProperties_GivenTwoObjectsOfDifferentTypesAndMappingFunction()
        {
            Product source = TestData.Product;
            var result = Yam<Product, SaleItem>
                .For(dest => dest.Id, src => src.Id)
                .For(dest => dest.Description, src => src.Description)
                .For(dest => dest.ShippingWeight, src => src.Weight)
                .For(dest => dest.Quantity, src => 1)
                .Map(source);

            Assert.IsInstanceOfType(result, typeof(SaleItem));
            Assert.AreEqual(result.Description, source.Description);
            Assert.AreEqual(result.Id, source.Id);
            Assert.AreEqual(result.ShippingWeight, source.Weight);
            Assert.AreEqual(result.Quantity, 1);
        }

        [TestMethod]
        public void YamOfTSource_MapOfTDestination_MapsAsExpected()
        {
            Product source = TestData.Product;
            Yam<Product, SaleItem>
                .For(dest => dest.Id, src => src.Id)
                .For(dest => dest.Description, src => src.Description)
                .For(dest => dest.ShippingWeight, src => src.Weight)
                .For(dest => dest.Quantity, src => 1);

            var result = Yam<Product>.Map<SaleItem>().Map(source);

            Assert.IsInstanceOfType(result, typeof(SaleItem));
            Assert.AreEqual(result.Description, source.Description);
            Assert.AreEqual(result.Id, source.Id);
            Assert.AreEqual(result.ShippingWeight, source.Weight);
            Assert.AreEqual(result.Quantity, 1);
        }

        [TestMethod]
        public void Yam_MapOfTDestination_MapsAsExpected()
        {
            Product source = TestData.Product;
            Yam<Product, SaleItem>
                .For(dest => dest.Id, src => src.Id)
                .For(dest => dest.Description, src => src.Description)
                .For(dest => dest.ShippingWeight, src => src.Weight)
                .For(dest => dest.Quantity, src => 1);

            var result = Yam.Map<SaleItem>(source);

            Assert.IsInstanceOfType(result, typeof(SaleItem));
            Assert.AreEqual(result.Description, source.Description);
            Assert.AreEqual(result.Id, source.Id);
            Assert.AreEqual(result.ShippingWeight, source.Weight);
            Assert.AreEqual(result.Quantity, 1);
        }
    }
}
