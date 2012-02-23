using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Globalization;

namespace BillomatNet.Data
{
    /// <summary>
    /// Transaction type for which a template was made
    /// </summary>
    public enum BillomatTemplateType { Invoice, Offer }

    /// <summary>
    /// File format of a template
    /// </summary>
    public enum BillomatTemplateFormat { Doc, Docx, Rtf }

    /// <summary>
    /// Image format of a template's thumbnail
    /// </summary>
    public enum BillomatTemplateThumbType { Png, Gif, Jpg }

    /// <summary>
    /// Represents a Billomat invoice/offer template
    /// </summary>
    [BillomatResource("templates", "template", "templates")]
    public class BillomatTemplate : BillomatObject<BillomatTemplate>
    {
        /// <summary>
        /// Finds all templates or all templates that match the specified type
        /// </summary>
        /// <param name="type">Template type to search for</param>
        /// <returns></returns>
        public static List<BillomatTemplate> FindAll(BillomatTemplateType? type = null)
        {
            var parameters = new NameValueCollection();
            if (type != null) { parameters.Add("type", type.Value.ToString().ToUpper()); }
            return FindAll(parameters);
        }

        /// <summary>
        /// Returns a dictionary containing all templates: (template ID => template object)
        /// </summary>
        /// <returns></returns>
        public new static Dictionary<int, BillomatTemplate> GetList()
        {
            return BillomatObject<BillomatTemplate>.GetList();
        }

        /// <summary>
        /// Creates a new template
        /// </summary>
        /// <returns></returns>
        public override BillomatTemplate Create()
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new BillomatException("Missing mandatory field: Name");
            }

            if (string.IsNullOrEmpty(_file))
            {
                throw new BillomatException("Missing mandatory field: File");
            }

            return base.Create();
        }

        /// <summary>
        /// Returns image date containing a thumbnail of the first page of the template
        /// </summary>
        /// <param name="type">Type of the image to be returned</param>
        /// <returns>Byte array containing image data of the thumbnail</returns>
        public byte[] Thumb(BillomatTemplateThumbType type = BillomatTemplateThumbType.Png)
        {
            BillomatRequest req = new BillomatRequest();
            req.Resource = "templates";
            req.Id = Id;
            req.Method = "thumb";

            req.Params.Add("type", type.ToString().ToLower());

            return req.GetResponse();
        }

        [BillomatField("type")]
        internal string _type { get; set; }

        public BillomatTemplateType Type
        {
            get
            {
                return (BillomatTemplateType)Enum.Parse(typeof(BillomatTemplateType), _type, true);
            }
            set
            {
                _type = value.ToString().ToUpperInvariant();
            }
        }

        [BillomatField("format")]
        internal string _format { get; set; }

        public BillomatTemplateFormat Format
        {
            get
            {
                return (BillomatTemplateFormat)Enum.Parse(typeof(BillomatTemplateFormat), _format, true);
            }
            set
            {
                _format = value.ToString().ToUpperInvariant();
            }
        }

        [BillomatField("name")]
        public string Name { get; set; }
        
        [BillomatField("base64file")]
        internal string _file { get; set; }

        public Stream File
        {
            get { return new MemoryStream(Convert.FromBase64String(_file)); }
            set { _file = BillomatHelper.Base64File(value); }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
