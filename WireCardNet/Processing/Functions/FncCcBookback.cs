using WireCardNet.Processing.Transactions;

namespace WireCardNet.Processing.Functions
{
    public class FncCcBookback : Function
    {
        protected override string GetXmlName()
        {
            return "CC_BOOKBACK";
        }

        protected override bool IsTransactionAcceptable(Transaction tx)
        {
            return (tx is CCTransaction);
        }
    }
}