using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace WireCardNet.Processing
{
    public class FunctionResponse
    {
        private List<TransactionResponse> transactions = new List<TransactionResponse>();

        public string FunctionId { get; internal set; }

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
                        transactions.Add(new TransactionResponse(child));
                    }
                }
            }
        }

        internal TransactionResponse FindTransaction(string transactionId)
        {
            return transactions.FirstOrDefault(t => t.TransactionId == transactionId);
        }
    }
}
