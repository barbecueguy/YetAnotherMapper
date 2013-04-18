using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yams;
using YamTestClasses;

namespace SimpleMapperUnitTests
{
    [TestClass]
    public class PropertyMapTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Map_Throws_WhenDestinationPropertyIsNull()
        {
            Product product = new Product();
            SaleItem saleItem = new SaleItem();
            PropertyMap map = new PropertyMap();
            map.Map(product, saleItem);
        }

        [TestMethod]
        public void Map_SetsDestinationPropertyToItsDefaultValue_WhenSourcePropertyIsNullAndMappingFunctionIsNull()
        {
            Product product = new Product();
            SaleItem saleItem = new SaleItem();
            PropertyMap map = new PropertyMap();

            map.DestinationProperty = product.GetType().GetProperty("Id");
            map.Map(product, saleItem);
            Assert.AreEqual(0, saleItem.Id);

            map.DestinationProperty = product.GetType().GetProperty("Description");
            map.Map(product, saleItem);
            Assert.AreEqual(null, saleItem.Description);

            map.DestinationProperty = product.GetType().GetProperty("Quantity");
            map.Map(product, saleItem);
            Assert.AreEqual(0, saleItem.Quantity);

            map.DestinationProperty = product.GetType().GetProperty("ShippingWeight");
            map.Map(product, saleItem);
            Assert.AreEqual(0, saleItem.ShippingWeight);
        }
    }
}
