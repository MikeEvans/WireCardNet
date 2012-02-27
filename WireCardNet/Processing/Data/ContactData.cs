using System;
using System.Collections.Generic;
using System.Linq;

namespace WireCardNet.Processing.Data
{
    public class ContactData : AbstractData
    {
        public ContactData() : base("CONTACT_DATA")
        {
        }

        public string IPAddress { get; set; }
    }
}