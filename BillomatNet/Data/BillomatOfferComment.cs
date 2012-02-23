using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Globalization;

namespace BillomatNet.Data
{
    /// <summary>
    /// Represents one entry in the history (comments) of a Billomat offer
    /// </summary>
    [BillomatResource("offer-comments", "offer-comment", "offer-comments", Flags = BillomatResourceFlags.NoUpdate)]
    public class BillomatOfferComment : BillomatTransactionComment<BillomatOfferComment>
    {
        /// <summary>
        /// Finds all comments that belong to the specified offer
        /// </summary>
        /// <param name="offerId">The offer whose comments should be found</param>
        /// <returns></returns>
        public static List<BillomatOfferComment> FindAll(int offerId)
        {
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("offer_id", offerId.ToString(CultureInfo.InvariantCulture));
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

        [BillomatField("offer_id")]
        public int Offer { get; set; }
    }
}
