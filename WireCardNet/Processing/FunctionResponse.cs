using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace WireCardNet.Processing
{
    public class FunctionResponse
    {
        private readonly List<TransactionResponse> _transactions = new List<TransactionResponse>();

        internal FunctionResponse(XmlNode n)
        {
            XmlNode fid;

            if ((fid = n.SelectSingleNode("FunctionID")) != null)
            {
                FunctionId = fid.InnerText;
            }

            foreach (XmlNode child in n.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    if (child.Name.EndsWith("_TRANSACTION"))
                    {
                        _transactions.Add(new TransactionResponse(child));
                    }
                }
            }
        }

        public string FunctionId { get; internal set; }

        internal TransactionResponse FindTransaction(string transactionId)
        {
            return _transactions.FirstOrDefault(t => t.TransactionId == transactionId);
        }
    }
}