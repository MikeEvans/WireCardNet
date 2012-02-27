using System;
using System.Collections.Generic;
using System.Linq;

namespace WireCardNet.QPay
{
    /// <summary>
    /// This class represents a response from QPay if the payment attempt failed
    /// </summary>
    public class CheckoutFailureResponse : CheckoutResponse
    {
        internal CheckoutFailureResponse()
        {
        }

        public string Message { get; internal set; }
    }
}