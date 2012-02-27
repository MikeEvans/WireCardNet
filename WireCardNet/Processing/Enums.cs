namespace WireCardNet.Processing
{
    public enum RecurringTransactionType
    {
        Single,
        Initial,
        Repeated
    }

    public enum TransactionMode
    {
        Demo,
        Live
    }

    public enum FunctionResult
    {
        ACK,
        NOK,
        Pending
    }

    public enum ResponseErrorType
    {
        Rejected,
        DataError,
        SystemError,
        ClientError
    }
}