using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Globalization;

namespace BillomatNet.Data
{
    /// <summary>
    /// Represents one item on a Billomat offer
    /// </summary>
    [BillomatResource("offer-items", "offer-item", "offer-items")]
    public class BillomatOfferItem : BillomatTransactionItem<BillomatOfferItem>
    {
        /// <summary>
        /// Finds all items that belong to the specified offer
        /// </summary>
        /// <param name="offerId">The offer whose items should be found</param>
        /// <returns></returns>
        public static List<BillomatOfferItem> FindAll(int offerId)
        {
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("offer_id", offerId.ToString(CultureInfo.InvariantCulture));
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

        [BillomatField("offer_id")]
        public int Offer { get; set; }
    }
}
