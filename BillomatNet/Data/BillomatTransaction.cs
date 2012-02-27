using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace BillomatNet.Data
{
    /// <summary>
    /// Base class for transactions (i.e. offers and invoices)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BillomatTransaction<T> : BillomatNumberedObject<T> where T : BillomatTransaction<T>, new()
    {
        [BillomatField("client_id")]
        public int Client { get; set; }

        [BillomatField("date")]
        public DateTime? Date { get; set; }

        [BillomatField("address")]
        public string Address { get; set; }

        [BillomatField("intro")]
        public string Intro { get; set; }

        [BillomatField("note")]
        public string Note { get; set; }

        [BillomatField("total_gross")]
        [BillomatReadOnly]
        public float TotalGross { get; internal set; }

        [BillomatField("total_net")]
        [BillomatReadOnly]
        public float TotalNet { get; internal set; }

        [BillomatField("currency_code")]
        public string CurrencyCode { get; set; }

        [BillomatField("quote")]
        public float? Quote { get; set; }

        /// <summary>
        /// Finds all transactions that match the specified criteria
        /// </summary>
        /// <param name="client">search for client ID</param>
        /// <param name="number">search for number</param>
        /// <param name="status">search for status</param>
        /// <param name="fromDate">search for date</param>
        /// <param name="toDate">search for date</param>
        /// <param name="intro">search for intro text</param>
        /// <param name="note">search for note text</param>
        /// <returns></returns>
        protected static List<T> FindAll(int? client = null, string number = null,
                                         string status = null, DateTime? fromDate = null, DateTime? toDate = null,
                                         string intro = null, string note = null)
        {
            var parameters = new NameValueCollection();

            if (client.HasValue)
            {
                parameters.Add("client_id", client.Value.ToString(CultureInfo.InvariantCulture));
            }
            if (!string.IsNullOrEmpty(number))
            {
                parameters.Add(GetResource().XmlSingleName + "_number", number);
            }
            if (!string.IsNullOrEmpty(status))
            {
                parameters.Add("status", status);
            }
            if (fromDate.HasValue)
            {
                parameters.Add("from", fromDate.Value.ToString("yyyy-MM-dd"));
            }
            if (toDate.HasValue)
            {
                parameters.Add("to", toDate.Value.ToString("yyyy-MM-dd"));
            }
            if (!string.IsNullOrEmpty(intro))
            {
                parameters.Add("intro", intro);
            }
            if (!string.IsNullOrEmpty(note))
            {
                parameters.Add("note", note);
            }

            return FindAll(parameters);
        }

        /// <summary>
        /// Creates a new transaction
        /// </summary>
        /// <returns></returns>
        public override T Create()
        {
            if (Client == 0)
            {
                throw new BillomatException("Missing mandatory field: Client");
            }

            T result = base.Create();

            //this.TotalGross = result.TotalGross;
            //this.TotalNet = result.TotalNet;

            return result;
        }

        /// <summary>
        /// Downloads the PDF document of this transaction
        /// </summary>
        /// <returns></returns>
        [PublicAPI]
        public Stream GetPdfDocument()
        {
            var req = new BillomatRequest
                          {
                              Resource = GetResource().ResourceName,
                              Id = Id,
                              Method = "pdf"
                          };

            req.Params.Add("format", "pdf");

            return new MemoryStream(req.GetResponse());
        }

        /// <summary>
        /// Completes the transaction
        /// </summary>
        /// <param name="templateId"></param>
        [PublicAPI]
        public void Complete(int? templateId = null)
        {
            var req = new BillomatRequest
                          {
                              Verb = "PUT",
                              Resource = GetResource().ResourceName,
                              Id = Id,
                              Method = "complete"
                          };

            var xml = new XElement("complete");

            if (templateId.HasValue)
            {
                xml.Add(new XElement("template_id", templateId.Value.ToString(CultureInfo.InvariantCulture)));
            }

            req.Body = xml.ToString(SaveOptions.DisableFormatting);
            req.GetResponse();
        }

        /// <summary>
        /// Sends the transaction in an e-mail to the specified recipient
        /// </summary>
        /// <param name="toEMail">recipient (separate multiple recipients with commas)</param>
        /// <param name="fromEMail">sender's e-mail address</param>
        /// <param name="ccEMail">CC recipient (separate multiple recipients with commas)</param>
        /// <param name="bccEMail">BCC recipient (separate multiple recipients with commas)</param>
        /// <param name="subject">subject of the e-mail</param>
        /// <param name="body">body of the e-mail</param>
        /// <param name="filename">filename of the invoice attachment (without .pdf)</param>
        /// <exception cref="BillomatNet.BillomatException">Thrown if no recipient is specified</exception>
        [PublicAPI]
        public void SendMail(string toEMail, string fromEMail = null, string ccEMail = null,
                             string bccEMail = null, string subject = null, string body = null, string filename = null)
        {
            string[] tos = (toEMail ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] ccs = (ccEMail ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] bccs = (bccEMail ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (tos.Length == 0)
            {
                throw new BillomatException("No recipient specified!", new ArgumentException("toEMail must not be null or empty", "toEMail"));
            }

            var req = new BillomatRequest
                          {
                              Verb = "POST",
                              Resource = GetResource().ResourceName,
                              Id = Id,
                              Method = "email"
                          };

            var elRecipients = new XElement("recipients");
            elRecipients.Add(tos.Select(to => new XElement("to", to)).ToArray());
            elRecipients.Add(ccs.Select(cc => new XElement("cc", cc)).ToArray());
            elRecipients.Add(bccs.Select(bcc => new XElement("bcc", bcc)).ToArray());

            var xml = new XElement("email", elRecipients);

            if (!string.IsNullOrEmpty(fromEMail))
            {
                xml.Add(new XElement("from", fromEMail));
            }

            if (!string.IsNullOrEmpty(subject))
            {
                xml.Add(new XElement("subject", subject));
            }

            if (!string.IsNullOrEmpty(body))
            {
                xml.Add(new XElement("body", body));
            }

            if (!string.IsNullOrEmpty(filename))
            {
                xml.Add(new XElement("filename", filename));
            }

            req.Body = xml.ToString(SaveOptions.DisableFormatting);
            req.GetResponse();
        }

        /// <summary>
        /// Cancels a transaction
        /// </summary>
        [PublicAPI]
        public void Cancel()
        {
            var req = new BillomatRequest
                          {
                              Verb = "PUT",
                              Resource = GetResource().ResourceName,
                              Id = Id,
                              Method = "cancel"
                          };

            req.GetResponse();
        }

        /// <summary>
        /// Returns the client associated with this transaction
        /// </summary>
        /// <returns></returns>
        [PublicAPI]
        public BillomatClient GetClient()
        {
            return BillomatClient.Find(Client);
        }
    }
}