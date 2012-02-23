using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace BillomatNet.Data
{
    /// <summary>
    /// Represents a Billomat client (customer)
    /// </summary>
    [BillomatResource("clients", "client", "clients")]
    public class BillomatClient : BillomatNumberedObject<BillomatClient>
    {
        /// <summary>
        /// Finds all clients that match the specified criteria
        /// </summary>
        /// <param name="name">search for name</param>
        /// <param name="clientNumber">search for client number</param>
        /// <param name="firstName">search for first name</param>
        /// <param name="lastName">search for last name</param>
        /// <param name="countryCode">search for country</param>
        /// <param name="note">search for note text</param>
        /// <returns></returns>
        public static List<BillomatClient> FindAll(string name = null, string clientNumber = null,
            string firstName = null, string lastName = null, string countryCode = null,
            string note = null)
        {
            NameValueCollection parameters = new NameValueCollection();

            if (!string.IsNullOrEmpty(name)) { parameters.Add("name", name); }
            if (!string.IsNullOrEmpty(clientNumber)) { parameters.Add("client_number", clientNumber); }
            if (!string.IsNullOrEmpty(firstName)) { parameters.Add("first_name", firstName); }
            if (!string.IsNullOrEmpty(lastName)) { parameters.Add("last_name", lastName); }
            if (!string.IsNullOrEmpty(countryCode)) { parameters.Add("country_code", countryCode); }
            if (!string.IsNullOrEmpty(note)) { parameters.Add("note", note); }

            return FindAll(parameters);
        }

        /// <summary>
        /// Returns a dictionary containing all clients: (client ID => client object)
        /// </summary>
        /// <returns></returns>
        public new static Dictionary<int, BillomatClient> GetList()
        {
            return BillomatObject<BillomatClient>.GetList();
        }

        /// <summary>
        /// Creates a new client
        /// </summary>
        /// <returns></returns>
        public override BillomatClient Create()
        {
            if (string.IsNullOrEmpty(Street))
            {
                throw new BillomatException("Missing mandatory field: Street");
            }

            if (string.IsNullOrEmpty(ZipCode))
            {
                throw new BillomatException("Missing mandatory field: ZipCode");
            }

            if (string.IsNullOrEmpty(City))
            {
                throw new BillomatException("Missing mandatory field: City");
            }

            return base.Create();
        }

        /// <summary>
        /// Returns all invoices that belong to this client
        /// </summary>
        /// <returns></returns>
        public List<BillomatInvoice> GetInvoices()
        {
            return BillomatInvoice.FindAll(client: this.Id);
        }

        /// <summary>
        /// Returns all offers that belong to this client
        /// </summary>
        /// <returns></returns>
        public List<BillomatOffer> GetOffers()
        {
            return BillomatOffer.FindAll(client: this.Id);
        }

        [BillomatField("client_number")]
        [BillomatReadOnly]
        public string ClientNumber { get; set; }

        [BillomatField("name")]
        public string Name { get; set; }

        [BillomatField("salutation")]
        public string Salutation { get; set; }

        [BillomatField("first_name")]
        public string FirstName { get; set; }

        [BillomatField("last_name")]
        public string LastName { get; set; }

        [BillomatField("street")]
        public string Street { get; set; }

        [BillomatField("zip")]
        public string ZipCode { get; set; }

        [BillomatField("city")]
        public string City { get; set; }

        [BillomatField("state")]
        public string State { get; set; }

        [BillomatField("country_code")]
        public string CountryCode { get; set; }

        [BillomatField("phone")]
        public string Phone { get; set; }

        [BillomatField("fax")]
        public string Fax { get; set; }

        [BillomatField("email")]
        public string EMail { get; set; }

        [BillomatField("www")]
        public string WebPage { get; set; }

        [BillomatField("tax_number")]
        public string TaxNumber { get; set; }

        [BillomatField("vat_number")]
        public string VatNumber { get; set; }

        [BillomatField("bank_account_owner")]
        public string BankAccountOwner { get; set; }

        [BillomatField("bank_number")]
        public string BankNumber { get; set; }

        [BillomatField("bank_name")]
        public string BankName { get; set; }

        [BillomatField("bank_account_number")]
        public string BankAccountNumber { get; set; }

        [BillomatField("bank_swift")]
        public string BankSwift { get; set; }

        [BillomatField("bank_iban")]
        public string BankIban { get; set; }

        [BillomatField("note")]
        public string Note { get; set; }

        [BillomatField("revenue_gross")]
        [BillomatReadOnly]
        public float? RevenueGross { get; internal set; }

        [BillomatField("revenue_net")]
        [BillomatReadOnly]
        public float? RevenueNet { get; internal set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
