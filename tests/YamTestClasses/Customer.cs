using System.Collections.Generic;

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
