using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace BillomatNet
{
    /// <summary>
    /// Marks a property to be included in Billomat requests
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    [MeansImplicitUse]
    internal class BillomatFieldAttribute : Attribute
    {
        /// <summary>
        /// Returns the XML attribute name for this field
        /// </summary>
        public string AttributeName { get; private set; }

        /// <summary>
        /// Creates a new instance of the BillomatFieldAttribute and sets the XML attribute name
        /// </summary>
        /// <param name="attributeName">The attribute name for this field</param>
        public BillomatFieldAttribute(string attributeName)
        {
            AttributeName = attributeName;
        }
    }
}
