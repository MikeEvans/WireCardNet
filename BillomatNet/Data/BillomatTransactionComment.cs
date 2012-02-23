using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BillomatNet.Data
{
    /// <summary>
    /// Base class for comments on invoices or offers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BillomatTransactionComment<T> : BillomatObject<T> where T : BillomatTransactionComment<T>, new()
    {
        /// <summary>
        /// Creates a new transaction comment
        /// </summary>
        /// <returns></returns>
        public override T Create()
        {
            if (string.IsNullOrEmpty(Comment))
            {
                throw new BillomatException("Missing mandatory field: Comment");
            }

            return base.Create();
        }

        [BillomatField("user_id")]
        [BillomatReadOnly]
        public int? User { get; set; }

        [BillomatField("comment")]
        public string Comment { get; set; }
    }
}
