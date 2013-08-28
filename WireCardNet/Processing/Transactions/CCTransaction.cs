using System;
using System.Globalization;
using System.Xml;
using WireCardNet.Processing.Data;

namespace WireCardNet.Processing.Transactions
{
    public class CCTransaction : Transaction
    {
        public CCTransaction()
        {
            CurrencyMinorUnits = 2;
        }

        /// <summary>
        /// A reference payment id, used for recurring payments
        /// </summary>
        public string GuWID { get; set; }

        /// <summary>
        /// The amount of this transaction
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// The number of minor units in the specified currency (default is 2, for USD and EUR for example)
        /// </summary>
        public int CurrencyMinorUnits { get; set; }

        /// <summary>
        /// The currency in which the amount is specified
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// The country where the transaction takes place
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// This field will be shown on the customer's card statement (not supported by all acquirers)
        /// </summary>
        public string Usage { get; set; }

        /// <summary>
        /// Used to specify whether this transaction is a recurring one
        /// </summary>
        public RecurringTransactionType RecurringTransaction { get; set; }

        /// <summary>
        /// Credit card data (only needed with transaction type 'Initial' or 'Single')
        /// </summary>
        public CreditCardData CreditCardData { get; set; }

        public ContactData ContactData { get; set; }
        public CorpTrustCenterData CorpTrustCenterData { get; set; }

        internal override XmlElement GetXml(XmlDocument doc)
        {
            var root = doc.CreateElement("CC_TRANSACTION");
            root.SetAttribute("mode", this.Mode.ToString().ToLower());

            var tid = doc.CreateElement("TransactionID");
            tid.InnerText = TransactionId;
            root.AppendChild(tid);

            if (Amount > 0)
            {
                var e = doc.CreateElement("Amount");
                e.InnerText = Convert.ToInt32(Amount * Math.Pow(10, CurrencyMinorUnits)).ToString(CultureInfo.InvariantCulture);
                e.SetAttribute("minorunits", CurrencyMinorUnits.ToString(CultureInfo.InvariantCulture));
                e.SetAttribute("action", "convert");
                root.AppendChild(e);
            }

            if (!string.IsNullOrEmpty(Currency))
            {
                var e = doc.CreateElement("Currency");
                e.InnerText = Currency;
                root.AppendChild(e);
            }

            if (!string.IsNullOrEmpty(CountryCode))
            {
                var e = doc.CreateElement("CountryCode");
                e.InnerText = CountryCode;
                root.AppendChild(e);
            }

            if (!string.IsNullOrEmpty(Usage))
            {
                var e = doc.CreateElement("Usage");
                e.InnerText = Usage;
                root.AppendChild(e);
            }

            if (!string.IsNullOrEmpty(GuWID))
            {
                var e = doc.CreateElement("GuWID");
                e.InnerText = GuWID;
                root.AppendChild(e);
            }

            if (ContactData != null)
            {
                root.AppendChild(ContactData.GetXml(doc));
            }   

            if (CorpTrustCenterData != null)
            {
                root.AppendChild(CorpTrustCenterData.GetXml(doc));
            }

            if (CreditCardData != null)
            {
                root.AppendChild(CreditCardData.GetXml(doc));
            }

            if (RecurringTransaction != RecurringTransactionType.Single)
            {
                var e = doc.CreateElement("RECURRING_TRANSACTION");

                var type = doc.CreateElement("Type");
                type.InnerText = RecurringTransaction.ToString();
                e.AppendChild(type);

                root.AppendChild(e);
            }

            return root;
        }
    }
}