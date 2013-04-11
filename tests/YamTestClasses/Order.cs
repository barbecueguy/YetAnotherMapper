using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YamTestClasses
{
    public class Order
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public Address ShippingAddress { get; set; }
        public List<Product> Items { get; set; }
    }
}
