using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BillomatNet.Data
{
    [BillomatResource("settings", "settings", "settings", Flags = BillomatResourceFlags.NoCreate | BillomatResourceFlags.NoDelete)]
    public class BillomatSettings : BillomatObject<BillomatSettings>
    {
        /// <summary>
        /// This method must not be used. Please use BillomatSettings.Load() instead!
        /// </summary>
        public new static BillomatSettings Find(int id)
        {
            throw new BillomatException("BillomatSettings.Find(id) must not be used. Please use BillomatSettings.Load() instead!", new NotSupportedException());
        }

        /// <summary>
        /// Retrieves the Billomat settings for this account
        /// </summary>
        /// <returns></returns>
        public static BillomatSettings Load()
        {
            BillomatRequest req = new BillomatRequest();
            req.Verb = "GET";
            req.Resource = GetResource().ResourceName;

            var result = CreateFromXML(req.GetXmlResponse());
            result.Created = DateTime.Now;

            return result;
        } 

        [BillomatField("invoice_intro")]
        public string InvoiceIntro { get; set; }
        [BillomatField("invoice_note")]
        public string InvoiceNote { get; set; }

        [BillomatField("offer_intro")]
        public string OfferIntro { get; set; }
        [BillomatField("offer_note")]
        public string OfferNote { get; set; }

        [BillomatField("invoice_email_subject")]
        public string InvoiceEMailSubject { get; set; }
        [BillomatField("invoice_email_body")]
        public string InvoiceEMailBody { get; set; }

        [BillomatField("offer_email_subject")]
        public string OfferEMailSubject { get; set; }
        [BillomatField("offer_email_body")]
        public string OfferEMailBody { get; set; }

        [BillomatField("article_number_pre")]
        public string ArticleNumberPrefix { get; set; }
        [BillomatField("client_number_pre")]
        public string ClientNumberPrefix { get; set; }
        [BillomatField("invoice_number_pre")]
        public string InvoiceNumberPrefix { get; set; }
        [BillomatField("offer_number_pre")]
        public string OfferNumberPrefix { get; set; }

        [BillomatField("currency_code")]
        public string CurrencyCode { get; set; }
        [BillomatField("tax_rate")]
        public float TaxRate { get; set; }
        [BillomatField("tax_name")]
        public string TaxName { get; set; }

        [BillomatField("discount_rate")]
        public float DiscountRate { get; set; }
        [BillomatField("discount_days")]
        public int DiscountDays { get; set; }
        [BillomatField("due_days")]
        public int DueDays { get; set; }
    }
}
