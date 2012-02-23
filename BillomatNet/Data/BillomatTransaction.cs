using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Xml;

namespace BillomatNet.Data
{
    /// <summary>
    /// Base class for transactions (i.e. offers and invoices)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BillomatTransaction<T> : BillomatNumberedObject<T> where T : BillomatTransaction<T>, new()
    {
        /// <summary>
        /// Finds all transactions that match the specified criteria
        /// </summary>
        /// <param name="client">search for client ID</param>
        /// <param name="number">search for number</param>
        /// <param name="fromDate">search for date</param>
        /// <param name="toDate">search for date</param>
        /// <param name="intro">search for intro text</param>
        /// <param name="note">search for note text</param>
        /// <returns></returns>
        protected static List<T> FindAll(int? client = null, string number = null,
            string status = null, DateTime? fromDate = null, DateTime? toDate = null,
            string intro = null, string note = null)
        {
            NameValueCollection parameters = new NameValueCollection();

            if (client.HasValue) { parameters.Add("client_id", client.Value.ToString(CultureInfo.InvariantCulture)); }
            if (!string.IsNullOrEmpty(number)) { parameters.Add(GetResource().XmlSingleName + "_number", number); }
            if (!string.IsNullOrEmpty(status)) { parameters.Add("status", status); }
            if (fromDate.HasValue) { parameters.Add("from", fromDate.Value.ToString("yyyy-MM-dd")); }
            if (toDate.HasValue) { parameters.Add("to", toDate.Value.ToString("yyyy-MM-dd")); }
            if (!string.IsNullOrEmpty(intro)) { parameters.Add("intro", intro); }
            if (!string.IsNullOrEmpty(note)) { parameters.Add("note", note); }

            return BillomatObject<T>.FindAll(parameters);
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

            var result = base.Create();

            //this.TotalGross = result.TotalGross;
            //this.TotalNet = result.TotalNet;

            return result;
        }

        /// <summary>
        /// Downloads the PDF document of this transaction
        /// </summary>
        /// <returns></returns>
        public Stream GetPdfDocument()
        {
            BillomatRequest req = new BillomatRequest();
            req.Resource = GetResource().ResourceName;
            req.Id = Id;
            req.Method = "pdf";
            
            req.Params.Add("format", "pdf");

            return new MemoryStream(req.GetResponse());
        }

        /// <summary>
        /// Completes the transaction
        /// </summary>
        /// <param name="templateId"></param>
        public void Complete(int? templateId = null)
        {
            BillomatRequest req = new BillomatRequest();
            req.Verb = "PUT";
            req.Resource = GetResource().ResourceName;
            req.Id = Id;
            req.Method = "complete";

            XmlDocument doc = new XmlDocument();
            XmlElement complete = doc.CreateElement("complete");
            doc.AppendChild(complete);

            if (templateId.HasValue)
            {
                XmlElement tpl = doc.CreateElement("template_id");
                tpl.InnerText = templateId.Value.ToString(CultureInfo.InvariantCulture);
                complete.AppendChild(tpl);
            }

            req.Body = doc.OuterXml;
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
        public void SendMail(string toEMail, string fromEMail = null, string ccEMail = null,
            string bccEMail = null, string subject = null, string body = null, string filename = null)
        {
            string[] tos = (toEMail ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] ccs = (ccEMail ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] bccs = (bccEMail ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (tos.Length == 0)
            {
                throw new BillomatException("No recipient specified!", new ArgumentException("toEMail must not be null or empty", "toEMail"));
            }

            BillomatRequest req = new BillomatRequest();
            req.Verb = "POST";
            req.Resource = GetResource().ResourceName;
            req.Id = Id;
            req.Method = "email";

            XmlDocument doc = new XmlDocument();
            XmlElement elEMail = doc.CreateElement("email");
            doc.AppendChild(elEMail);

            XmlElement elRecipients = doc.CreateElement("recipients");
            elEMail.AppendChild(elRecipients);

            foreach (string to in tos)
            {
                XmlElement elTo = doc.CreateElement("to");
                elTo.InnerText = to;
                elRecipients.AppendChild(elTo);
            }

            foreach(string cc in ccs)
            {
                XmlElement elCc = doc.CreateElement("cc");
                elCc.InnerText = cc;
                elRecipients.AppendChild(elCc);
            }

            foreach(string bcc in bccs)
            {
                XmlElement elBcc = doc.CreateElement("bcc");
                elBcc.InnerText = bcc;
                elRecipients.AppendChild(elBcc);
            }

            if (!string.IsNullOrEmpty(fromEMail))
            {
                XmlElement elFrom = doc.CreateElement("from");
                elFrom.InnerText = fromEMail;
                elEMail.AppendChild(elFrom);
            }

            if (!string.IsNullOrEmpty(subject))
            {
                XmlElement elSubject = doc.CreateElement("subject");
                elSubject.InnerText = subject;
                elEMail.AppendChild(elSubject);
            }

            if (!string.IsNullOrEmpty(body))
            {
                XmlElement elBody = doc.CreateElement("body");
                elBody.InnerText = body;
                elEMail.AppendChild(elBody);
            }

            if (!string.IsNullOrEmpty(filename))
            {
                XmlElement elFilename = doc.CreateElement("filename");
                elFilename.InnerText = filename;
                elEMail.AppendChild(elFilename);
            }

            req.Body = doc.OuterXml;
            req.GetResponse();
        }

        /// <summary>
        /// Cancels a transaction
        /// </summary>
        public void Cancel()
        {
            BillomatRequest req = new BillomatRequest();
            req.Verb = "PUT";
            req.Resource = GetResource().ResourceName;
            req.Id = Id;
            req.Method = "cancel";

            req.GetResponse();
        }

        /// <summary>
        /// Returns the client associated with this transaction
        /// </summary>
        /// <returns></returns>
        public BillomatClient GetClient()
        {
            return BillomatClient.Find(this.Client);
        }

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
    }
}
