using System;
using CandiedYams;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yams;
using YamTestClasses;

namespace CandiedYamUnitTests
{
    [TestClass]
    public class CandiedYamTests
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
            var result = Yams<Product, SaleItem>.Map(source);

            Assert.IsInstanceOfType(result, typeof(SaleItem));
            Assert.AreEqual(result.Description, source.Description);
            Assert.AreEqual(result.Id, source.Id);
            Assert.AreEqual(result.Quantity, 0);
        }

        [TestMethod]
        public void Map_MapsAllProperties_GivenTwoObjectsOfDifferentTypesAndMappingExpressions()
        {
            Product source = TestData.Product;
            var result = Yams<Product, SaleItem>
                .For(dest => dest.Quantity, src => 1)
                .For(dest => dest.ShippingWeight, src => src.Weight)
                .Map(source);

            Assert.IsInstanceOfType(result, typeof(SaleItem));
            Assert.AreEqual(result.Description, source.Description);
            Assert.AreEqual(result.Id, source.Id);
            Assert.AreEqual(result.Quantity, 1);
        }
    }
}
