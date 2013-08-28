using System;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace WireCardNet.Processing.Data
{
    public abstract class AbstractData
    {
        private readonly string xmlName;

        protected AbstractData(string xmlName)
        {
            this.xmlName = xmlName;
        }

        internal XmlElement GetXml(XmlDocument doc)
        {
            var root = doc.CreateElement(this.xmlName);
            var t = GetType();
            var properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var pi in properties)
            {
                if (pi.PropertyType.IsSubclassOf(typeof (AbstractData)))
                {
                    var value = (AbstractData) pi.GetValue(this, null);

                    if (value != null)
                    {
                        var e = value.GetXml(doc);
                        root.AppendChild(e);
                    }
                }
                else
                {
                    var e = doc.CreateElement(pi.Name);
                    e.InnerText = string.Format(CultureInfo.InvariantCulture, "{0}", pi.GetValue(this, null));
                    root.AppendChild(e);
                }
            }

            return root;
        }
    }
}