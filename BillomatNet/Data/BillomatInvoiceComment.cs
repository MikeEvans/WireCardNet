using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace BillomatNet.Data
{
    /// <summary>
    /// Represents one entry in the history (comments) of a Billomat invoice
    /// </summary>
    [BillomatResource("invoice-comments", "invoice-comment", "invoice-comments", Flags = BillomatResourceFlags.NoUpdate)]
    public class BillomatInvoiceComment : BillomatTransactionComment<BillomatInvoiceComment>
    {
        [BillomatField("invoice_id")]
        public int Invoice { get; set; }

        /// <summary>
        /// Finds all comments that belong to the specified invoice
        /// </summary>
        /// <param name="invoiceId">The invoice whose comments should be found</param>
        /// <returns></returns>
        public static List<BillomatInvoiceComment> FindAll(int invoiceId)
        {
            var parameters = new NameValueCollection
                                 {
                                     { "invoice_id", invoiceId.ToString(CultureInfo.InvariantCulture) }
                                 };
            return FindAll(parameters);
        }

        /// <summary>
        /// Creates a new invoice comment
        /// </summary>
        /// <returns></returns>
        public override BillomatInvoiceComment Create()
        {
            if (Invoice == 0)
            {
                throw new BillomatException("Missing mandatory field: Invoice");
            }

            return base.Create();
        }
    }
}