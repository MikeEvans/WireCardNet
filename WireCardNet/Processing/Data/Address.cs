using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WireCardNet.Processing.Data
{
    public class Address : AbstractData
    {
        public Address() : base("ADDRESS") { }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string EMail { get; set; }
    }
}
