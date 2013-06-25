using System.Collections.Generic;

namespace YamTestClasses
{
    public class Invoice
    {
        public Customer Customer { get; set; }

        public List<Product> Items { get; set; }
    }
}
