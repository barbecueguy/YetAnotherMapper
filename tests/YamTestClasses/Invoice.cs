using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YamTestClasses
{
    public class Invoice
    {
        public Customer Customer { get; set; }

        public List<Product> Items { get; set; }
    }
}
