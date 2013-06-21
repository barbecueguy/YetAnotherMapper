﻿using System;
using System.Linq;
using NUnit.Framework;
using YamTestClasses;
using Yams;

namespace SimpleMapperUnitTests
{
    [TestFixture]
    public class YamOfTSourceTDestinationTests
    {
        [SetUp]
        public void SetUp()
        {
            Yam.Clear();
        }

        [Test]
        public void Map_CreatesAMap_AndAddsItToTheYam()
        {
            var map = Yam.GetMap<SaleItem, Product>();
            Assert.IsNull(map);

            var expected = new SaleItem
            {
                Description = "Test SaleItem",
                Id = 1,
                Weight = 3.4
            };

            Yam<SaleItem, Product>.Map(expected);

            map = Yam.GetMap<SaleItem, Product>();
            Assert.IsNotNull(map);
        }

        [Test]
        public void Map_TestMappingCommonlyNamedProperties()
        {
            var expected = new SaleItem
            {
                Description = "Test SaleItem",
                Id = 1,
                Weight = 3.4
            };

            var actual = Yam<SaleItem, Product>.Map(expected);

            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(0, actual.ShippingWeight);
        }

        [Test]
        public void Map_TestMappingUsingPropertyExpressions()
        {
            var expected = new SaleItem
            {
                Description = "Test SaleItem",
                Id = 1,
                Weight = 3.4
            };

            var actual = Yam<SaleItem, Product>.Map(expected)
                

            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Weight, actual.ShippingWeight);
        }
    }
}
