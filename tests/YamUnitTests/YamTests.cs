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
        public void Yam_Map_ReturnsTheSameMap_WhenAMapForAGivenSourceAndDestinationAlreadyExists()
        {
            TypeMap map = Yam.Map(typeof(SaleItem), typeof(Product));
            Assert.AreSame(map, Yam.Map(typeof(SaleItem), typeof(Product)), "Expected the same object reference, but found another");
        }

        [TestMethod]
        public void Yam_Map_ReturnsADifferentMap_WhenTheMapperIsClearedAndTheMapIsRecreated()
        {
            TypeMap map = Yam.Map(typeof(SaleItem), typeof(Product));
            Assert.AreSame(map, Yam.Map(typeof(SaleItem), typeof(Product)), "Expected the same object reference, but found another");

            Yam.Clear();

            Assert.AreNotSame(map, Yam.Map(typeof(SaleItem), typeof(Product)), "Expected the same object reference, but found another");
        }

        [TestMethod]
        public void Yam_MapsCommonlyNamedProperties_GivenTwoDifferentObjectTypes()
        {
            Product source = TestData.Product;
            var result = Yam<Product, SaleItem>.Map(source);

            Assert.IsInstanceOfType(result, typeof(SaleItem));
            Assert.AreEqual(result.Description, source.Description);
            Assert.AreEqual(result.Id, source.Id);
            Assert.AreEqual(result.Quantity, 0);
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
            Assert.AreEqual(result.Quantity, 1);
        }
    }
}
