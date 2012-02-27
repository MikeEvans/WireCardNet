using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace WireCardNet.Processing
{
    public class TransactionResponse
    {
        internal TransactionResponse(XmlNode n)
        {
            if (n.Attributes["mode"] != null)
            {
                Mode = (TransactionMode) Enum.Parse(typeof (TransactionMode), n.Attributes["mode"].Value, true);
            }

            TransactionId = n.SelectSingleNode("TransactionID").InnerText;
            ProcessingStatus = new ProcessingStatus(n.SelectSingleNode("PROCESSING_STATUS"));
        }

        public TransactionMode Mode { get; internal set; }
        public string TransactionId { get; internal set; }
        public ProcessingStatus ProcessingStatus { get; internal set; }
    }
}