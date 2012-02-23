using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Globalization;

namespace BillomatNet.Data
{
    /// <summary>
    /// Represents one item on a Billomat invoice
    /// </summary>
    [BillomatResource("invoice-items", "invoice-item", "invoice-items")]
    public class BillomatInvoiceItem : BillomatTransactionItem<BillomatInvoiceItem>
    {
        /// <summary>
        /// Finds all items that belong to the specified invoice
        /// </summary>
        /// <param name="invoiceId">The invoice whose items should be found</param>
        /// <returns></returns>
        public static List<BillomatInvoiceItem> FindAll(int invoiceId)
        {
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("invoice_id", invoiceId.ToString(CultureInfo.InvariantCulture));
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

        [BillomatField("invoice_id")]
        public int Invoice { get; set; }
    }
}
