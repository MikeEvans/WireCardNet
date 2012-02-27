using System;
using System.Collections.Generic;
using System.Linq;
using WireCardNet.Processing.Transactions;

namespace WireCardNet.Processing.Functions
{
    public class FncCCPurchase : Function
    {
        protected override string GetXmlName()
        {
            return "CC_PURCHASE";
        }

        protected override bool IsTransactionAcceptable(Transaction tx)
        {
            return (tx is CCTransaction);
        }
    }
}