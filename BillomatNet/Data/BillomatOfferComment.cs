using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace BillomatNet.Data
{
    /// <summary>
    /// Represents one entry in the history (comments) of a Billomat offer
    /// </summary>
    [BillomatResource("offer-comments", "offer-comment", "offer-comments", Flags = BillomatResourceFlags.NoUpdate)]
    public class BillomatOfferComment : BillomatTransactionComment<BillomatOfferComment>
    {
        [BillomatField("offer_id")]
        public int Offer { get; set; }

        /// <summary>
        /// Finds all comments that belong to the specified offer
        /// </summary>
        /// <param name="offerId">The offer whose comments should be found</param>
        /// <returns></returns>
        public static List<BillomatOfferComment> FindAll(int offerId)
        {
            var parameters = new NameValueCollection
                                 {
                                     { "offer_id", offerId.ToString(CultureInfo.InvariantCulture) }
                                 };
            return FindAll(parameters);
        }

        /// <summary>
        /// Creates a new offer comment
        /// </summary>
        /// <returns></returns>
        public override BillomatOfferComment Create()
        {
            if (Offer == 0)
            {
                throw new BillomatException("Missing mandatory field: Offer");
            }

            return base.Create();
        }
    }
}