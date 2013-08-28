namespace WireCardNet.QPay
{
    /// <summary>
    /// Represents the state of a payment returned by QPay
    /// </summary>
    public enum PaymentState
    {
        /// <summary>
        /// The payment was successfully processed
        /// </summary>
        Success,

        /// <summary>
        /// The payment was cancelled by the user
        /// </summary>
        Cancel,

        /// <summary>
        /// The payment failed
        /// </summary>
        Failure
    }
}