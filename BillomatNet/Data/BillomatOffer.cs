using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BillomatNet.Data
{
    /// <summary>
    /// Represents the status of a Billomat offer
    /// </summary>
    public enum BillomatOfferStatus
    {
        Draft,
        Open,
        Won,
        Lost,
        Canceled
    }

    /// <summary>
    /// Represents a Billomat offer
    /// </summary>
    [BillomatResource("offers", "offer", "offers")]
    public class BillomatOffer : BillomatTransaction<BillomatOffer>
    {
        /// <summary>
        /// Finds all offers that match the specified criteria
        /// </summary>
        /// <param name="client">search for client ID</param>
        /// <param name="offerNumber">search for offer number</param>
        /// <param name="status">search for offer status</param>
        /// <param name="fromDate">search for date</param>
        /// <param name="toDate">search for date</param>
        /// <param name="intro">search for intro text</param>
        /// <param name="note">search for note text</param>
        /// <returns></returns>
        public static List<BillomatOffer> FindAll(int? client = null, string offerNumber = null,
            BillomatOfferStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null,
            string intro = null, string note = null)
        {
            return FindAll(client, offerNumber, status.ToString().ToUpper(), fromDate, toDate, intro, note);
        }

        /// <summary>
        /// Returns a dictionary containing all offers: (offer ID => offer object)
        /// </summary>
        /// <returns></returns>
        public new static Dictionary<int, BillomatOffer> GetList()
        {
            return BillomatObject<BillomatOffer>.GetList();
        }

        /// <summary>
        /// Returns all invoice items that belong to this offer
        /// </summary>
        /// <returns></returns>
        public List<BillomatOfferItem> GetItems()
        {
            return BillomatOfferItem.FindAll(this.Id);
        }

        /// <summary>
        /// Returns all comments that belong to this offer
        /// </summary>
        /// <returns></returns>
        public List<BillomatOfferComment> GetComments()
        {
            return BillomatOfferComment.FindAll(this.Id);
        }

        [BillomatField("offer_number")]
        [BillomatReadOnly]
        public string OfferNumber { get; internal set; }

        [BillomatField("status")]
        [BillomatReadOnly]
        internal string _status { get; set; }

        public BillomatOfferStatus Status
        {
            get
            {
                return (BillomatOfferStatus)Enum.Parse(typeof(BillomatOfferStatus), _status, true);
            }
            internal set
            {
                _status = value.ToString().ToUpperInvariant();
            }
        }

        public override string ToString()
        {
            return OfferNumber;
        }
    }
}
