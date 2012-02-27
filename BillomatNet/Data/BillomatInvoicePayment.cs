using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace BillomatNet.Data
{
    /// <summary>
    /// Represents one payment for a Billomat invoice
    /// </summary>
    [BillomatResource("invoice-payments", "invoice-payment", "invoice-payments", Flags = BillomatResourceFlags.NoUpdate)]
    public class BillomatInvoicePayment : BillomatObject<BillomatInvoicePayment>
    {
        [BillomatField("invoice_id")]
        public int Invoice { get; set; }

        [BillomatField("date")]
        public DateTime? Date { get; set; }

        [BillomatField("amount")]
        public float Amount { get; set; }

        [BillomatField("comment")]
        public string Comment { get; set; }

        [BillomatField("mark_invoice_as_payed")]
        internal bool MarkInvoiceAsPaid { get; set; }

        /// <summary>
        /// Finds all payments that belong to the specified invoice
        /// </summary>
        /// <param name="invoiceId">The invoice whose payments should be found</param>
        /// <returns></returns>
        public static List<BillomatInvoicePayment> FindAll(int invoiceId)
        {
            var parameters = new NameValueCollection
                                 {
                                     { "invoice_id", invoiceId.ToString(CultureInfo.InvariantCulture) }
                                 };
            return FindAll(parameters);
        }

        /// <summary>
        /// Creates a new invoice payment without marking the invoice as paid
        /// </summary>
        /// <returns></returns>
        public override BillomatInvoicePayment Create()
        {
            return Create(false);
        }

        /// <summary>
        /// Creates a new invoice payment
        /// </summary>
        /// <param name="markInvoiceAsPaid">Specify true to mark the corresponding invoice as paid</param>
        /// <returns></returns>
        public BillomatInvoicePayment Create(bool markInvoiceAsPaid)
        {
            if (Invoice == 0)
            {
                throw new BillomatException("Missing mandatory field: Invoice");
            }

            if (Amount == 0)
            {
                throw new BillomatException("Missing mandatory field: Amount");
            }

            MarkInvoiceAsPaid = markInvoiceAsPaid;

            return base.Create();
        }
    }
}