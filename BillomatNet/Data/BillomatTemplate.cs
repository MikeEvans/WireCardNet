using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace BillomatNet.Data
{
    /// <summary>
    /// Represents a Billomat invoice/offer template
    /// </summary>
    [BillomatResource("templates", "template", "templates")]
    public class BillomatTemplate : BillomatObject<BillomatTemplate>
    {
        [BillomatField("type")]
        internal string _type { get; set; }

        public BillomatTemplateType Type
        {
            get { return (BillomatTemplateType) Enum.Parse(typeof (BillomatTemplateType), _type, true); }
            set { _type = value.ToString().ToUpperInvariant(); }
        }

        [BillomatField("format")]
        internal string _format { get; set; }

        public BillomatTemplateFormat Format
        {
            get { return (BillomatTemplateFormat) Enum.Parse(typeof (BillomatTemplateFormat), _format, true); }
            set { _format = value.ToString().ToUpperInvariant(); }
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

        /// <summary>
        /// Finds all templates or all templates that match the specified type
        /// </summary>
        /// <param name="type">Template type to search for</param>
        /// <returns></returns>
        public static List<BillomatTemplate> FindAll(BillomatTemplateType? type = null)
        {
            var parameters = new NameValueCollection();
            if (type != null)
            {
                parameters.Add("type", type.Value.ToString().ToUpper());
            }
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
            var req = new BillomatRequest
                          {
                              Resource = "templates",
                              Id = Id,
                              Method = "thumb"
                          };

            req.Params.Add("type", type.ToString().ToLower());

            return req.GetResponse();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}