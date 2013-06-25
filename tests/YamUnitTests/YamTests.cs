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
        public void CreateMap_CreatesTheMapWithDefaultPropertyMapsForCommonlyNamedProperties()
        {
            var commonProperties = from sourceProperty in typeof(Product).GetProperties().Select(p => p.Name)
                                   join destinationProperty in typeof(SaleItem).GetProperties().Select(p => p.Name)
                                   on sourceProperty equals destinationProperty
                                   select sourceProperty;

            TypeMap expected = Yam.CreateMap(typeof(Product), typeof(SaleItem));

            Assert.IsTrue(commonProperties.Count() > 0);
            Assert.AreEqual(commonProperties.Count(), expected.PropertyMaps.Count);
        }

        [Test]
        public void CreateMap_AddsTheMapToTheYam()
        {
            TypeMap expected = Yam.CreateMap(typeof(Product), typeof(SaleItem));

            TypeMap actual = Yam.GetMap(typeof(Product), typeof(SaleItem));

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void Map_TestMappingPropertiesWithTheSameDataTypes_AndTheSameNames()
        {
            Product product = new Product { Description = "Test Product", Id = 1, ShippingWeight = 3.5 };
            Yam.CreateMap(typeof(Product), typeof(SaleItem));

            SaleItem saleItem = (SaleItem)Yam.Map(product, typeof(SaleItem));

            Assert.AreEqual(product.Description, saleItem.Description);
            Assert.AreEqual(product.Id, saleItem.Id);
            Assert.AreNotEqual(product.ShippingWeight, saleItem.Weight);
        }

        [Test]
        public void Map_TestMappingPropertiesWithTheSameDataTypes_GivenThePropertyNames()
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
        public void Map_TestMappingProperties_GivenAMappingFunction()
        {
            Product product = new Product { Description = "Test Product", Id = 1, ShippingWeight = 3.5 };
            Yam.CreateMap(typeof(Product), typeof(SaleItem));
            Yam.AddPropertyMap(
                typeof(Product),
                typeof(SaleItem),
                p => 42,
                "Weight");

            SaleItem saleItem = (SaleItem)Yam.Map(product, typeof(SaleItem));

            Assert.AreEqual(product.Description, saleItem.Description);
            Assert.AreEqual(product.Id, saleItem.Id);
            Assert.AreEqual(42, saleItem.Weight);
        }

        [Test]
        public void Map_TestMappingPropertiesWithDifferentClrDataTypes()
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
        public void Map_TestMappingPropertiesWithDifferentDataTypes_WhenAMappingForThoseDataTypesExists()
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
        public void Map_TestMappingGenericListsWithSameTypeParameters()
        {
            Product product1 = new Product { Description = "Product 1", Id = 1, ShippingWeight = 3.5 };
            Product product2 = new Product { Description = "Product 2", Id = 2, ShippingWeight = 4.7 };
            List<Product> products = new List<Product> { product1, product2 };

            List<Product> saleItems = (List<Product>)Yam.Map(products, typeof(List<Product>));

            Assert.AreEqual(products.Count, saleItems.Count);
            foreach (var product in products)
                Assert.IsNotNull(saleItems.FirstOrDefault(si => si.Description == product.Description && si.Id == product.Id && si.ShippingWeight == product.ShippingWeight));
        }

        [Test]
        public void Map_TestMappingGenericListsWithDifferentTypeParameters_WhenAMappingForThoseDataTypesExists()
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
        public void Map_TestMappingPropertiesThatAreGenericLists_WhenTheGenericParameterTypesAreTheSame()
        {
            var expected = new Object1 { Name = "Object1", Scores = new List<int> { 1, 3, 5 } };

            Yam.CreateMap(typeof(Object1), typeof(Object2));

            var actual = (Object2)Yam.Map(expected, typeof(Object2));

            Assert.AreEqual(expected.Name, actual.Name);
            foreach (var item in actual.Scores)
                Assert.IsTrue(expected.Scores.Contains(item));
        }

        [Test]
        public void Map_TestMappingPropertiesThatAreGenericLists_WhenTheGenericParameterTypesAreDifferentClrDataTypes()
        {
            var expected = new Object1 { Name = "Object1", Scores = new List<int> { 1, 3, 5 } };

            Yam.CreateMap(typeof(Object1), typeof(Object3));

            var actual = (Object3)Yam.Map(expected, typeof(Object3));

            Assert.AreEqual(expected.Name, actual.Name);
            foreach (var item in actual.Scores)
                Assert.IsTrue(expected.Scores.Contains((int)item));
        }

        [Test]
        public void Map_TestMappingPropertiesThatAreGenericLists_WhenAMappingForThoseDataTypesExists()
        {
            SaleItem saleItem1 = new SaleItem { Description = "SaleItem 1", Id = 1, Weight = 3.5 };
            SaleItem saleItem2 = new SaleItem { Description = "SaleItem 2", Id = 2, Weight = 4.7 };
            List<SaleItem> saleItems = new List<SaleItem> { saleItem1, saleItem2 };
            Order order = new Order { Items = saleItems };

            Yam.CreateMap(typeof(Order), typeof(Invoice));
            Yam.CreateMap(typeof(SaleItem), typeof(Product));
            Yam.AddPropertyMap(typeof(SaleItem), typeof(Product), "Weight", "ShippingWeight");

            Invoice invoice = (Invoice)Yam.Map(order, typeof(Invoice));

            Assert.AreEqual(order.Items.Count, invoice.Items.Count);
            foreach (var item in order.Items)
                Assert.IsNotNull(invoice.Items.FirstOrDefault(it => it.Description == item.Description && it.Id == item.Id && it.ShippingWeight == item.Weight));
        }

        [Test]
        public void CreateMapOfTSourceTDestination_CreatesTheMapWithDefaultPropertyMapsForCommonlyNamedProperties()
        {
            var commonProperties = from sourceProperty in typeof(Product).GetProperties().Select(p => p.Name)
                                   join destinationProperty in typeof(SaleItem).GetProperties().Select(p => p.Name)
                                   on sourceProperty equals destinationProperty
                                   select sourceProperty;

            TypeMap expected = Yam.CreateMap<Product, SaleItem>();

            Assert.IsTrue(commonProperties.Count() > 0);
            Assert.AreEqual(commonProperties.Count(), expected.PropertyMaps.Count);
        }

        [Test]
        public void CreateMapOfTSourceTDestination_AddsTheMapToTheYam()
        {
            TypeMap expected = Yam.CreateMap<Product, SaleItem>();

            TypeMap actual = Yam.GetMap<Product, SaleItem>();

            Assert.AreSame(expected, actual);
        }

        [Test]
        public void MapOfTDestination_TestMappingListsWithDifferentParameterTypes_WhenAMappingForThoseDataTypesExists()
        {
            Product product1 = new Product { Description = "Product 1", Id = 1, ShippingWeight = 3.5 };
            Product product2 = new Product { Description = "Product 2", Id = 2, ShippingWeight = 4.7 };
            List<Product> products = new List<Product> { product1, product2 };

            Yam.CreateMap<Product, SaleItem>();
            Yam.AddPropertyMap(typeof(Product), typeof(SaleItem), "ShippingWeight", "Weight");

            List<SaleItem> saleItems = Yam.Map<List<SaleItem>>(products);

            Assert.AreEqual(products.Count, saleItems.Count);
            foreach (var product in products)
                Assert.IsNotNull(saleItems.FirstOrDefault(si => si.Description == product.Description && si.Id == product.Id && si.Weight == product.ShippingWeight));
        }
    }
}
