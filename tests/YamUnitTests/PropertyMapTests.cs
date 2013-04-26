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
        public void PropertyMap_Map_Throws_WhenDestinationPropertyIsNull()
        {
            Product product = new Product();
            SaleItem saleItem = new SaleItem();
            PropertyMap map = new PropertyMap(sourceProperty: null, destinationProperty: null);
            map.Map(product, saleItem);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PropertyMap_Map_Throws_WhenSourceObjectIsNull()
        {
            Product product = null;
            SaleItem saleItem = new SaleItem();
            PropertyMap map = new PropertyMap(sourceProperty: null, destinationProperty: null);
            map.Map(product, saleItem);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PropertyMap_Map_Throws_WhenDestinationObjectIsNull()
        {
            Product product = new Product();
            SaleItem saleItem = null;
            PropertyMap map = new PropertyMap(sourceProperty: null, destinationProperty: null);
            map.Map(product, saleItem);
        }

        [TestMethod]
        public void PropertyMap_Map_SetsDestinationPropertyToItsDefaultValue_WhenSourcePropertyIsNullAndMappingFunctionIsNull()
        {
            SaleItem saleItem = new SaleItem();
            Product product = new Product();
            PropertyMap map = new PropertyMap(
                sourceProperty: null,
                destinationProperty: product.GetType().GetProperty("Id"),
                mappingFunction: null);

            map.Map(saleItem, product);
            Assert.AreEqual(0, product.Id);

            map = new PropertyMap(
                destinationProperty: product.GetType().GetProperty("Description"),
                mappingFunction: null);
            map.Map(saleItem, product);
            Assert.AreEqual(null, product.Description);

            map = new PropertyMap(
                destinationProperty: product.GetType().GetProperty("Weight"),
                mappingFunction: null);
            map.Map(saleItem, product);
            Assert.AreEqual(0, product.Weight);
        }

        [TestMethod]
        public void PropertyMap_Map_SetsDestinationPropertyValueToSourcePropertyValue_WhenTheSourcePropertyIsProvidedAndTheMapFunctionIsNull()
        {
            SaleItem saleItem = new SaleItem { Id = 1, Description = "Test", ShippingWeight = 3.0 };
            Product product = new Product();
            PropertyMap map = new PropertyMap(
                sourceProperty: saleItem.GetType().GetProperty("Id"),
                destinationProperty: product.GetType().GetProperty("Id"),
                mappingFunction: null);
            map.Map(saleItem, product);
            Assert.AreEqual(saleItem.Id, product.Id);

            map = new PropertyMap(
                destinationProperty: product.GetType().GetProperty("Description"),
                sourceProperty: saleItem.GetType().GetProperty("Description"));
            map.Map(saleItem, product);
            Assert.AreEqual(saleItem.Description, product.Description);

            map = new PropertyMap(
                destinationProperty: product.GetType().GetProperty("Weight"),
                sourceProperty: saleItem.GetType().GetProperty("ShippingWeight"));
            map.Map(saleItem, product);
            Assert.AreEqual(saleItem.ShippingWeight, product.Weight);
        }

        [TestMethod]
        public void PropertyMap_Map_SetsDestinationPropertyValueToMapFunctionResult_WhenTheSourcePropertyIsNullAndTheMapFunctionIsProvided()
        {
            SaleItem saleItem = new SaleItem { Id = 1, Description = "Test", ShippingWeight = 3.0 };
            Product product = new Product();
            PropertyMap map = new PropertyMap(
                destinationProperty: product.GetType().GetProperty("Id"),
                mappingFunction: si => 4);
            map.Map(saleItem, product);
            Assert.AreEqual(4, product.Id);

            map = new PropertyMap(
                destinationProperty: product.GetType().GetProperty("Description"),
                mappingFunction: si => "Description from mapping function");
            map.Map(saleItem, product);
            Assert.AreEqual("Description from mapping function", product.Description);

            map = new PropertyMap(
                destinationProperty: product.GetType().GetProperty("Weight"),
                mappingFunction: si => 12);
            map.Map(saleItem, product);
            Assert.AreEqual(12, product.Weight);
        }

        [TestMethod]
        public void PropertyMap_Map_SetsDestinationPropertyValueToMapFunctionResult_WhenTheSourcePropertyIsProvidedAndTheMapFunctionIsProvided()
        {
            SaleItem saleItem = new SaleItem { Id = 1, Description = "Test", ShippingWeight = 3.0 };
            Product product = new Product();
            PropertyMap map = new PropertyMap(
                destinationProperty: product.GetType().GetProperty("Id"),
                sourceProperty: saleItem.GetType().GetProperty("Id"),
                mappingFunction: si => 4);
            map.Map(saleItem, product);
            Assert.AreEqual(4, product.Id);

            map = new PropertyMap(
                destinationProperty: product.GetType().GetProperty("Description"),
                sourceProperty: saleItem.GetType().GetProperty("Description"),
                mappingFunction: si => "Description from mapping function");
            map.Map(saleItem, product);
            Assert.AreEqual("Description from mapping function", product.Description);

            map = new PropertyMap(
                destinationProperty: product.GetType().GetProperty("Weight"),
                sourceProperty: saleItem.GetType().GetProperty("ShippingWeight"),
                mappingFunction: si => 12);
            map.Map(saleItem, product);
            Assert.AreEqual(12, product.Weight);
        }
    }
}
