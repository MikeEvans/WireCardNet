using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BillomatNet
{
    [Flags]
    public enum BillomatResourceFlags
    {
        None = 0x0,
        NoUpdate = 0x1,
        NoDelete = 0x2,
        NoCreate = 0x4
    }

    /// <summary>
    /// Marks a class to be be Billomat resource. This attribute is necessary to provide additional
    /// information, such as the resource's name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BillomatResourceAttribute : Attribute
    {
        /// <summary>
        /// Returns the name of the resource for use in the request URL
        /// </summary>
        public string ResourceName { get; private set; }

        /// <summary>
        /// Returns the name of the XML element representing a single object
        /// </summary>
        public string XmlSingleName { get; private set; }

        /// <summary>
        /// Returns the name of the XML element containing an array of objects
        /// </summary>
        public string XmlMultiName { get; private set; }

        /// <summary>
        /// Gets or sets prohibited operations on this Billomat object class
        /// </summary>
        public BillomatResourceFlags Flags { get; set; }

        /// <summary>
        /// Creates a new instance of the BillomatResourceAttribute and specifies all necessary
        /// information.
        /// </summary>
        /// <param name="resourceName">name of the resource for use in the request URL</param>
        /// <param name="xmlSingleName">name of the XML element representing a single object</param>
        /// <param name="xmlMultiName">name of the XML element containing an array of objects</param>
        public BillomatResourceAttribute(string resourceName, string xmlSingleName, string xmlMultiName)
        {
            ResourceName = resourceName;
            XmlSingleName = xmlSingleName;
            XmlMultiName = xmlMultiName;
        }
    }
}
