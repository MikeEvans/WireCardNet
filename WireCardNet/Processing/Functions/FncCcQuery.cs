using WireCardNet.Processing.Transactions;

namespace WireCardNet.Processing.Functions
{
    public class FncCcQuery : Function
    {
        protected override string GetXmlName()
        {
            return "CC_QUERY";
        }

        protected override bool IsTransactionAcceptable(Transaction tx)
        {
            return (tx is CCTransaction);
        }
    }
}