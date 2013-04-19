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
    }
}
