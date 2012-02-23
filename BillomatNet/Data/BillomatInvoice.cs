using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;
using System.IO;

namespace BillomatNet.Data
{
    /// <summary>
    /// Represents the status of a Billomat invoice
    /// </summary>
    public enum BillomatInvoiceStatus
    {
        Draft,
        Open,
        Overdue,
        Paid,
        Canceled
    }

    /// <summary>
    /// Represents a Billomat invoice
    /// </summary>
    [BillomatResource("invoices", "invoice", "invoices")]
    public class BillomatInvoice : BillomatTransaction<BillomatInvoice>
    {
        /// <summary>
        /// Finds all invoices that match the specified criteria
        /// </summary>
        /// <param name="client">search for client ID</param>
        /// <param name="invoiceNumber">search for invoice number</param>
        /// <param name="status">search for invoice status</param>
        /// <param name="fromDate">search for date</param>
        /// <param name="toDate">search for date</param>
        /// <param name="intro">search for intro text</param>
        /// <param name="note">search for note text</param>
        /// <returns></returns>
        public static List<BillomatInvoice> FindAll(int? client = null, string invoiceNumber = null,
            BillomatInvoiceStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null,
            string intro = null, string note = null)
        {
            return FindAll(client, invoiceNumber, status.ToString().ToUpper(), fromDate, toDate, intro, note);
        }

        /// <summary>
        /// Returns a dictionary containing all invoices: (invoice ID => invoice object)
        /// </summary>
        /// <returns></returns>
        public new static Dictionary<int, BillomatInvoice> GetList()
        {
            return BillomatObject<BillomatInvoice>.GetList();
        }

        public override BillomatInvoice Create()
        {
            var result = base.Create();

            //this.InvoiceNumber = result.InvoiceNumber;
            //this.Status = result.Status;

            return result;
        }

        /// <summary>
        /// Downloads the PDF document of this invoice
        /// </summary>
        /// <param name="signed">Specify true to retrieve a signed PDF</param>
        /// <returns></returns>
        public MemoryStream GetPdfDocument(bool signed)
        {
            BillomatRequest req = new BillomatRequest();
            req.Resource = GetResource().ResourceName;
            req.Id = Id;
            req.Method = "pdf";

            req.Params.Add("format", "pdf");

            if (signed)
            {
                req.Params.Add("type", "signed");
            }

            return new MemoryStream(req.GetResponse());
        }

        /// <summary>
        /// Uploads a signed PDF file for this invoice
        /// </summary>
        /// <param name="file"></param>
        public void UploadSignature(byte[] file)
        {
            UploadSignature(new MemoryStream(file));
        }

        /// <summary>
        /// Uploads a signed PDF file for this invoice
        /// </summary>
        /// <param name="file"></param>
        public void UploadSignature(Stream file)
        {
            string base64file = BillomatHelper.Base64File(file);

            BillomatRequest req = new BillomatRequest();
            req.Verb = "PUT";
            req.Resource = GetResource().ResourceName;
            req.Id = Id;
            req.Method = "upload-signature";

            XmlDocument doc = new XmlDocument();
            XmlElement elSignature = doc.CreateElement("signature");
            doc.AppendChild(elSignature);

            XmlElement elBase64File = doc.CreateElement("base64file");
            elBase64File.InnerText = base64file;
            elSignature.AppendChild(elBase64File);

            req.Body = doc.OuterXml;
            req.GetResponse();
        }

        /// <summary>
        /// Returns all payments that belong to this invoice
        /// </summary>
        /// <returns></returns>
        public List<BillomatInvoicePayment> GetPayments()
        {
            return BillomatInvoicePayment.FindAll(this.Id);
        }

        /// <summary>
        /// Returns all invoice items that belong to this invoice
        /// </summary>
        /// <returns></returns>
        public List<BillomatInvoiceItem> GetItems()
        {
            return BillomatInvoiceItem.FindAll(this.Id);
        }

        /// <summary>
        /// Returns all comments that belong to this invoice
        /// </summary>
        /// <returns></returns>
        public List<BillomatInvoiceComment> GetComments()
        {
            return BillomatInvoiceComment.FindAll(this.Id);
        }

        [BillomatField("invoice_number")]
        [BillomatReadOnly]
        public string InvoiceNumber { get; internal set; }

        [BillomatField("due_date")]
        public DateTime? DueDate { get; set; }

        [BillomatField("due_days")]
        [BillomatReadOnly]
        public int DueDays { get; set; }

        [BillomatField("discount_rate")]
        public float? DiscountRate { get; set; }

        [BillomatField("discount_date")]
        public DateTime? DiscountDate { get; set; }

        [BillomatField("discount_days")]
        [BillomatReadOnly]
        public int DiscountDays { get; set; }

        [BillomatField("discount_amount")]
        public float? DiscountAmount { get; set; }

        [BillomatField("status")]
        [BillomatReadOnly]
        internal string _status { get; set; }

        public BillomatInvoiceStatus Status
        {
            get
            {
                return (BillomatInvoiceStatus)Enum.Parse(typeof(BillomatInvoiceStatus), _status, true);
            }
            internal set
            {
                _status = value.ToString().ToUpperInvariant();
            }
        }

        public override string ToString()
        {
            return InvoiceNumber;
        }
    }
}
