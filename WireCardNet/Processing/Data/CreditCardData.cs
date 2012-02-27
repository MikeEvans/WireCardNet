using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WireCardNet.Processing.Data
{
    public class CreditCardData : AbstractData
    {
        public CreditCardData() : base("CREDIT_CARD_DATA") { }

        public string CreditCardNumber { get; set; }
        public string CVC2 { get; set; }

        public string ExpirationYear { get; set; }
        public string ExpirationMonth { get; set; }

        public string CardHolderName { get; set; }

        /// <summary>
        /// Optional: Required for Switch/Solo/Maestro card only
        /// </summary>
        public string CardStartYear { get; set; }

        /// <summary>
        /// Optional: Required for Switch/Solo/Maestro card only
        /// </summary>
        public string CardStartMonth { get; set; }

        /// <summary>
        /// Optional: Required for Switch/Solo/Maestro card only
        /// </summary>
        public string CardIssueNumber { get; set; }
    }
}
