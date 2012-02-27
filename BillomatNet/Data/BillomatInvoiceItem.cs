using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace BillomatNet.Data
{
    /// <summary>
    /// Represents one item on a Billomat invoice
    /// </summary>
    [BillomatResource("invoice-items", "invoice-item", "invoice-items")]
    public class BillomatInvoiceItem : BillomatTransactionItem<BillomatInvoiceItem>
    {
        [BillomatField("invoice_id")]
        public int Invoice { get; set; }

        /// <summary>
        /// Finds all items that belong to the specified invoice
        /// </summary>
        /// <param name="invoiceId">The invoice whose items should be found</param>
        /// <returns></returns>
        public static List<BillomatInvoiceItem> FindAll(int invoiceId)
        {
            var parameters = new NameValueCollection
                                 {
                                     { "invoice_id", invoiceId.ToString(CultureInfo.InvariantCulture) }
                                 };
            return FindAll(parameters);
        }

        /// <summary>
        /// Creates a new invoice item
        /// </summary>
        /// <returns></returns>
        public override BillomatInvoiceItem Create()
        {
            if (Invoice == 0)
            {
                throw new BillomatException("Missing mandatory field: Invoice");
            }

            return base.Create();
        }
    }
}