using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YamTestClasses
{
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Id { get; set; }
        public List<Address> Addresses { get; set; }        
    }
}
