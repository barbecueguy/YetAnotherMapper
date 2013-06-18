using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using YamTestClasses;
using Yams;

namespace SimpleMapperUnitTests
{
    [TestFixture]
    public class YamTests
    {
        [SetUp]
        public void SetUp()
        {
            Yam.Clear();
        }

        [Test]
        public void CreateMap_CreatesTheMap_And_AddsTheMapToTheYam()
        {
            TypeMap expected = Yam.CreateMap(typeof(Product), typeof(SaleItem));

            TypeMap actual = Yam.GetMap(typeof(Product), typeof(SaleItem));

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void CreateMap_CreatesTheMapAndAddsTheDefaultPropertyMapsForCommonlyNamedProperties()
        {
            TypeMap expected = Yam.CreateMap(typeof(Product), typeof(SaleItem));

            TypeMap actual = Yam.GetMap(typeof(Product), typeof(SaleItem));

            Assert.IsTrue(actual.PropertyMaps.Count > 0);
        }

        [Test]
        public void Map_MapsPropertiesWithTheSameNames()
        {
            Product product = new Product { Description = "Test Product", Id = 1, ShippingWeight = 3.5 };
            Yam.CreateMap(typeof(Product), typeof(SaleItem));

            SaleItem saleItem = (SaleItem)Yam.Map(product, typeof(SaleItem));

            Assert.AreEqual(product.Description, saleItem.Description);
            Assert.AreEqual(product.Id, saleItem.Id);
            Assert.AreNotEqual(product.ShippingWeight, saleItem.Weight);
        }

        [Test]
        public void Map_MapsPropertiesWithDifferentNamesGivenBothPropertyNames()
        {
            Product product = new Product { Description = "Test Product", Id = 1, ShippingWeight = 3.5 };
            Yam.CreateMap(typeof(Product), typeof(SaleItem));
            Yam.AddPropertyMap(
                typeof(Product),
                typeof(SaleItem),
                "ShippingWeight",
                "Weight");

            SaleItem saleItem = (SaleItem)Yam.Map(product, typeof(SaleItem));

            Assert.AreEqual(product.Description, saleItem.Description);
            Assert.AreEqual(product.Id, saleItem.Id);
            Assert.AreEqual(product.ShippingWeight, saleItem.Weight);
        }

        [Test]
        public void Map_MapsPropertiesWithDifferentNamesGivenADestinationPropertyNameAndAMappingFunction()
        {
            Product product = new Product { Description = "Test Product", Id = 1, ShippingWeight = 3.5 };
            Yam.CreateMap(typeof(Product), typeof(SaleItem));
            Yam.AddPropertyMap(
                typeof(Product),
                typeof(SaleItem),
                p => ((Product)p).ShippingWeight,
                "Weight");

            SaleItem saleItem = (SaleItem)Yam.Map(product, typeof(SaleItem));

            Assert.AreEqual(product.Description, saleItem.Description);
            Assert.AreEqual(product.Id, saleItem.Id);
            Assert.AreEqual(product.ShippingWeight, saleItem.Weight);
        }

        [Test]
        public void Map_MapsPropertiesWithTheSameNameButDifferentPropertyTypesGivenAMappingFunction()
        {
            Person object1 = new Person { Age = 21 };
            Yam.CreateMap(typeof(Person), typeof(Employee));
            Yam.AddPropertyMap(typeof(Person), typeof(Employee), o => (decimal)((Person)o).Age, "Age");

            Employee object2 = (Employee)Yam.Map(object1, typeof(Employee));

            Assert.AreEqual(object1.Age, object2.Age);
        }

        [Test]
        public void Map_MapsPropertiesWithTheSameNameButDifferentPropertyTypesWhenBothPropertiesAreClrTypes()
        {
            Person person = new Person { Age = 21 };
            Yam.CreateMap(typeof(Person), typeof(Employee));
            Yam.AddPropertyMap(typeof(Person), typeof(Employee), "Age", "Age");

            Employee employee = (Employee)Yam.Map(person, typeof(Employee));

            Assert.AreEqual(person.Age, employee.Age);

            employee = new Employee { Age = 21 };
            Yam.CreateMap(typeof(Employee), typeof(Person));
            Yam.AddPropertyMap(typeof(Employee), typeof(Person), "Age", "Age");

            person = (Person)Yam.Map(employee, typeof(Person));

            Assert.AreEqual(employee.Age, person.Age);
        }

        [Test]
        public void Map_MapsPropertiesWithTheSameNameButDifferentPropertyTypesWhenAMappingAlreadyExistsForThePropertyTypes()
        {
            Flyer flyer = new Flyer { Addressee = new Person { Age = 21, FirstName = "John", LastName = "Smith" } };

            Yam.CreateMap(typeof(Person), typeof(Customer));
            Yam.CreateMap(typeof(Flyer), typeof(Invoice));
            Yam.AddPropertyMap(typeof(Flyer), typeof(Invoice), "Addressee", "Customer");

            Invoice invoice = (Invoice)Yam.Map(flyer, typeof(Invoice));

            Assert.AreEqual(flyer.Addressee.Age, invoice.Customer.Age);
            Assert.AreEqual(flyer.Addressee.FirstName, invoice.Customer.FirstName);
            Assert.AreEqual(flyer.Addressee.LastName, invoice.Customer.LastName);
        }

        [Test]
        public void Map_MapsListsOfObjectsWhenAMappingForTheObjectsExists()
        {
            Product product1 = new Product { Description = "Product 1", Id = 1, ShippingWeight = 3.5 };
            Product product2 = new Product { Description = "Product 2", Id = 2, ShippingWeight = 4.7 };
            List<Product> products = new List<Product> { product1, product2 };

            Yam.CreateMap(typeof(Product), typeof(SaleItem));
            Yam.AddPropertyMap(typeof(Product), typeof(SaleItem), "ShippingWeight", "Weight");

            List<SaleItem> saleItems = (List<SaleItem>)Yam.Map(products, typeof(List<SaleItem>));

            Assert.AreEqual(products.Count, saleItems.Count);
            foreach (var product in products)
                Assert.IsNotNull(saleItems.FirstOrDefault(si => si.Description == product.Description && si.Id == product.Id && si.Weight == product.ShippingWeight));
        }

        [Test]
        public void Map_MapsCommonlyNamePropertiesThatAreListsOfDifferentTypesWhenAMappingForTheTypesExist()
        {
            SaleItem SaleItem1 = new SaleItem { Description = "SaleItem 1", Id = 1, Weight = 3.5 };
            SaleItem SaleItem2 = new SaleItem { Description = "SaleItem 2", Id = 2, Weight = 4.7 };
            List<SaleItem> SaleItems = new List<SaleItem> { SaleItem1, SaleItem2 };
            Order order = new Order { Items = SaleItems };

            Yam.CreateMap(typeof(Order), typeof(Invoice));
            Yam.CreateMap(typeof(SaleItem), typeof(Product));
            Yam.AddPropertyMap(typeof(SaleItem), typeof(Product), "Weight", "ShippingWeight");

            Invoice invoice = (Invoice)Yam.Map(order, typeof(Invoice));

            Assert.AreEqual(order.Items.Count, invoice.Items.Count);
            foreach (var item in order.Items)
                Assert.IsNotNull(invoice.Items.FirstOrDefault(it => it.Description == item.Description && it.Id == item.Id && it.ShippingWeight == item.Weight));
        }
    }
}
