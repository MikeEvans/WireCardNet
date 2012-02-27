using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using WireCardNet.Processing.Transactions;

namespace WireCardNet.Processing.Functions
{
    public class FncCCAuthorization : Function
    {
        protected override string GetXmlName()
        {
            return "CC_AUTHORIZATION";
        }

        protected override bool IsTransactionAcceptable(Transaction tx)
        {
            return (tx is CCTransaction);
        }
    }
}
