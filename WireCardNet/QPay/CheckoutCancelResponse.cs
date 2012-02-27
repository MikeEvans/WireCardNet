using System;
using System.Collections.Generic;
using System.Linq;

namespace WireCardNet.QPay
{
    /// <summary>
    /// This class represents a response from QPay if the user cancelled the payment
    /// </summary>
    public class CheckoutCancelResponse : CheckoutResponse
    {
        internal CheckoutCancelResponse()
        {
        }
    }
}