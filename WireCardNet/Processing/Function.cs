using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace WireCardNet.Processing
{
    public abstract class Function
    {
        private List<Transaction> transactions = new List<Transaction>();

        public string FunctionId { get; set; }

        public void AddTransaction(Transaction tx)
        {
            if (transactions.Count == 5)
            {
                throw new WireCardException("Can't add more than 5 transactions to a single function!");
            }

            if (transactions.Count > 0)
            {
                Type t = transactions.First().GetType();

                if (tx.GetType() != t)
                {
                    throw new WireCardException("A single function can only contain transactions of the same type!");
                }
            }

            if (!IsTransactionAcceptable(tx))
            {
                throw new WireCardException("The specified transaction is invalid for this function type!");
            }

            transactions.Add(tx);
        }

        internal XmlElement GetXml(XmlDocument doc)
        {
            var fnc = doc.CreateElement("FNC_" + GetXmlName());

            var fid = doc.CreateElement("FunctionID");
            fid.InnerText = FunctionId;
            fnc.AppendChild(fid);

            foreach (Transaction tx in transactions)
            {
                var transaction = tx.GetXml(doc);
                fnc.AppendChild(transaction);
            }

            return fnc;
        }

        protected virtual bool IsTransactionAcceptable(Transaction tx)
        {
            return true;
        }

        protected abstract string GetXmlName();
    }
}
