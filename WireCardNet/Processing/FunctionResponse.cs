using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace WireCardNet.Processing
{
    public class FunctionResponse
    {
        private readonly List<TransactionResponse> transactions = new List<TransactionResponse>();

        internal FunctionResponse(XmlNode n)
        {
            XmlNode fid;

            if ((fid = n.SelectSingleNode("FunctionID")) != null)
            {
                FunctionId = fid.InnerText;
            }

            foreach (XmlNode child in n.ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Element) 
                    continue;

                if (child.Name.EndsWith("_TRANSACTION"))
                {
                    this.transactions.Add(new TransactionResponse(child));
                }
            }
        }

        public List<TransactionResponse> Transactions
        {
            get { return this.transactions; }
        }

        public string FunctionId { get; internal set; }

        internal TransactionResponse FindTransaction(string transactionId)
        {
            return this.transactions.FirstOrDefault(t => t.TransactionId == transactionId);
        }
    }
}