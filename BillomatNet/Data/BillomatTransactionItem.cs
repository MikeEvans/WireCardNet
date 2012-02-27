using System;
using System.Collections.Generic;
using System.Linq;

namespace BillomatNet.Data
{
    /// <summary>
    /// Base class for items in invoices and offers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BillomatTransactionItem<T> : BillomatObject<T> where T : BillomatTransactionItem<T>, new()
    {
        [BillomatField("position")]
        [BillomatReadOnly]
        public int? Position { get; set; }

        [BillomatField("unit")]
        public string Unit { get; set; }

        [BillomatField("quantity")]
        public float? Quantity { get; set; }

        [BillomatField("unit_price")]
        public float? UnitPrice { get; set; }

        [BillomatField("tax_name")]
        public string TaxName { get; set; }

        [BillomatField("tax_rate")]
        public float? TaxRate { get; set; }

        [BillomatField("title")]
        public string Title { get; set; }

        [BillomatField("description")]
        public string Description { get; set; }

        [BillomatField("total_gross")]
        [BillomatReadOnly]
        public float TotalGross { get; internal set; }

        [BillomatField("total_net")]
        [BillomatReadOnly]
        public float TotalNet { get; internal set; }
    }
}