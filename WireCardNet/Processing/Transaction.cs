using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace WireCardNet.Processing
{
    public abstract class Transaction
    {
        public TransactionMode Mode { get; set; }
        public string TransactionId { get; set; }

        internal abstract XmlElement GetXml(XmlDocument doc);
    }
}
