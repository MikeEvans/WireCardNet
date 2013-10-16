using WireCardNet.Processing.Transactions;

namespace WireCardNet.Processing.Functions
{
    public class FncCcReversal : Function
    {
        protected override string GetXmlName()
        {
            return "CC_REVERSAL";
        }

        protected override bool IsTransactionAcceptable(Transaction tx)
        {
            return (tx is CCTransaction);
        }
    }
}