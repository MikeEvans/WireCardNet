using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace BillomatNet.Data
{
    /// <summary>
    /// Represents one item on a Billomat offer
    /// </summary>
    [BillomatResource("offer-items", "offer-item", "offer-items")]
    public class BillomatOfferItem : BillomatTransactionItem<BillomatOfferItem>
    {
        [BillomatField("offer_id")]
        public int Offer { get; set; }

        /// <summary>
        /// Finds all items that belong to the specified offer
        /// </summary>
        /// <param name="offerId">The offer whose items should be found</param>
        /// <returns></returns>
        public static List<BillomatOfferItem> FindAll(int offerId)
        {
            var parameters = new NameValueCollection
                                 {
                                     { "offer_id", offerId.ToString(CultureInfo.InvariantCulture) }
                                 };
            return FindAll(parameters);
        }

        /// <summary>
        /// Creates a new offer item
        /// </summary>
        /// <returns></returns>
        public override BillomatOfferItem Create()
        {
            if (Offer == 0)
            {
                throw new BillomatException("Missing mandatory field: Offer");
            }

            return base.Create();
        }
    }
}