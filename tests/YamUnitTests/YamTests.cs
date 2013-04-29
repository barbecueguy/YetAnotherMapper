using System;
using NUnit.Framework;
using Yams;
using YamTestClasses;

namespace SimpleMapperUnitTests
{
    [TestFixture]
    public class YamTests
    {
        [SetUp]
        public void TestInitialize()
        {
            Yam.Clear();
        }

        [Test]
        public void Yam_CreateMap_ReturnsTheSameObject_GivenTheSameSourceAndDestinationTypes()
        {
            var sourceType = typeof(Product);
            var destinationType = typeof(SaleItem);
            var expected = Yam.CreateMap(sourceType, destinationType);

            var actual = Yam.CreateMap(sourceType, destinationType);

            Assert.AreSame(expected, actual, "Expected the same reference, but found a different reference");
        }

        [Test]
        public void Yam_CreateMapOfTSourceTDestination_ReturnsTheSameObject_GivenTheSameSourceAndDestinationTypes()
        {
            var expected = Yam.CreateMap<Product,SaleItem>();

            var actual = Yam.CreateMap<Product, SaleItem>();

            Assert.AreSame(expected, actual, "Expected the same reference, but found a different reference");
        }

        [Test]
        public void Yam_Map_ReturnsTheSameMap_WhenAMapForAGivenSourceAndDestinationAlreadyExists()
        {
            var map = new TypeMap<Product, SaleItem>();
            Assert.AreSame(map, Yam.AddMap(map), "Expected the same object reference, but found another");
        }

        [Test]
        public void Yam_MapsCommonlyNamedProperties_GivenTwoDifferentObjectTypes()
        {
            Product source = TestData.Product;
            var destination = Yam<Product, SaleItem>.Map(source);

            Assert.IsInstanceOf<SaleItem>(destination);
            Assert.AreEqual(source.Description, destination.Description);
            Assert.AreEqual(source.Id, destination.Id);
            Assert.AreEqual(0, destination.Quantity);
        }

        [Test]
        public void Yam_MapsAllProperties_GivenTwoObjectsOfDifferentTypesAndMappingFunction()
        {
            Product source = TestData.Product;
            var result = Yam<Product, SaleItem>
                .For(dest => dest.Id, src => src.Id)
                .For(dest => dest.Description, src => src.Description)
                .For(dest => dest.ShippingWeight, src => src.Weight)
                .For(dest => dest.Quantity, src => 1)
                .Map(source);

            Assert.IsInstanceOf<SaleItem>(result);
            Assert.AreEqual(result.Description, source.Description);
            Assert.AreEqual(result.Id, source.Id);
            Assert.AreEqual(result.ShippingWeight, source.Weight);
            Assert.AreEqual(result.Quantity, 1);
        }

        [Test]
        public void YamOfTSource_MapOfTDestination_MapsAsExpected()
        {
            Product source = TestData.Product;
            Yam<Product, SaleItem>
                .For(dest => dest.Id, src => src.Id)
                .For(dest => dest.Description, src => src.Description)
                .For(dest => dest.ShippingWeight, src => src.Weight)
                .For(dest => dest.Quantity, src => 1);

            var result = Yam<Product>.Map<SaleItem>().Map(source);

            Assert.IsInstanceOf<SaleItem>(result);
            Assert.AreEqual(result.Description, source.Description);
            Assert.AreEqual(result.Id, source.Id);
            Assert.AreEqual(result.ShippingWeight, source.Weight);
            Assert.AreEqual(result.Quantity, 1);
        }

        [Test]
        public void Yam_MapOfTDestination_MapsAsExpected()
        {
            Product source = TestData.Product;
            Yam<Product, SaleItem>
                .For(dest => dest.Id, src => src.Id)
                .For(dest => dest.Description, src => src.Description)
                .For(dest => dest.ShippingWeight, src => src.Weight)
                .For(dest => dest.Quantity, src => 1);

            var result = Yam.Map<SaleItem>(source);

            Assert.IsInstanceOf<SaleItem>(result);
            Assert.AreEqual(result.Description, source.Description);
            Assert.AreEqual(result.Id, source.Id);
            Assert.AreEqual(result.ShippingWeight, source.Weight);
            Assert.AreEqual(result.Quantity, 1);
        }
    }
}
