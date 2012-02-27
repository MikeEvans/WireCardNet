using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WireCardNet.Processing.Data
{
    public class CorpTrustCenterData : AbstractData
    {
        public CorpTrustCenterData() : base("CORPTRUSTCENTER_DATA") { }

        public Address Address { get; set; }
    }
}
