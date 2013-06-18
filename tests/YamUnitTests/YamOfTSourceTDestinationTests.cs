using System;
using System.Linq;
using NUnit.Framework;
using YamTestClasses;
using Yams;

namespace SimpleMapperUnitTests
{
    [TestFixture]
    public class YamOfTSourceTDestinationTests
    {
        [Test]
        public void CreateMapOfTSourceTDestination_CreatesTheMapAndAddsItToTheYam()
        {
            TypeMap expected = Yam.CreateMap<SaleItem, Product>();
            TypeMap actual = Yam.GetMap<SaleItem, Product>();

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void MapOfTDestination_MapsCommonlyNamedPropertiesThatAreListsOfDifferntTypesWhenAMappingForThoseTypesExists()
        {
            Yam.CreateMap<Order, Invoice>();
            Yam.CreateMap<SaleItem, Product>();
            SaleItem saleItem = new SaleItem { Description = "Test Sale Item", Id = 1, Weight = 3.4 };

            Product product = Yam.Map<Product>(saleItem);

            Assert.AreEqual(saleItem.Description, product.Description);
            Assert.AreEqual(saleItem.Id, product.Id);
            Assert.AreEqual(0, product.ShippingWeight);
        }

        [Test]
        public void MapOfTDestination_MapsTheSourceObjectToTheDestinationType()
        {
            Product product = new Product { Description = "Test Product", Id = 1, ShippingWeight = 3.5 };
            SaleItem saleItem = Yam.Map<SaleItem>(product);

            Assert.AreEqual(product.Description, saleItem.Description);
            Assert.AreEqual(product.Id, saleItem.Id);
            Assert.AreEqual(0, saleItem.Weight);
        }        
    }
}
