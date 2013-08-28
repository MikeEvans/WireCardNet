using WireCardNet.Processing.Transactions;

namespace WireCardNet.Processing.Functions
{
    public class FncCCRefund : Function
    {
        protected override string GetXmlName()
        {
            return "CC_REFUND";
        }

        protected override bool IsTransactionAcceptable(Transaction tx)
        {
            return (tx is CCTransaction);
        }
    }
}