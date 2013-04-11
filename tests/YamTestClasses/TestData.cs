using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YamTestClasses
{
    public static class TestData
    {
        public static Address Address
        {
            get
            {
                return new Address
                {
                    City = "Peoria",
                    State = "IL",
                    Street = "1400 University",
                    Zip = "61614"
                };
            }
        }

        public static Customer Customer
        {
            get
            {
                return new Customer
                {
                    Addresses = new List<Address> { TestData.Address },
                    FirstName = "Bob",
                    Id = "12",
                    LastName = "Smith"
                };
            }
        }

        public static SaleItem SaleItem
        {
            get
            {
                return new SaleItem
                {
                    Description = "Some Product",
                    Id = 15,
                    Quantity = 1
                };
            }
        }

        public static Product Product
        {
            get
            {
                return new Product
                {
                    Description = "Some Sale Item",
                    Id = 15,
                    Weight = 3.2
                };
            }
        }

        public static Order Order
        {
            get
            {
                return new Order
                {
                    Customer = TestData.Customer,
                    Id = 12,
                    Items = new List<Product> { TestData.Product },
                    ShippingAddress = TestData.Address
                };
            }
        }
    }
}
