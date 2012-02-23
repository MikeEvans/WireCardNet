using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BillomatNet
{
    /// <summary>
    /// Exception that is thrown when an error in the Billomat library occurs
    /// </summary>
    [Serializable]
    public class BillomatException : ApplicationException
    {
        public BillomatException() : base() { }
        public BillomatException(string message) : base(message) { }
        public BillomatException(string message, Exception innerException) : base(message, innerException) { }
    }
}
