using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace WireCardNet.Processing.Data
{
    public abstract class AbstractData
    {
        private readonly string _xmlName;

        protected AbstractData(string xmlName)
        {
            _xmlName = xmlName;
        }

        internal XmlElement GetXml(XmlDocument doc)
        {
            XmlElement root = doc.CreateElement(_xmlName);

            Type t = GetType();

            PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo pi in properties)
            {
                if (pi.PropertyType.IsSubclassOf(typeof (AbstractData)))
                {
                    var value = (AbstractData) pi.GetValue(this, null);

                    if (value != null)
                    {
                        XmlElement e = value.GetXml(doc);
                        root.AppendChild(e);
                    }
                }
                else
                {
                    XmlElement e = doc.CreateElement(pi.Name);
                    e.InnerText = string.Format(CultureInfo.InvariantCulture, "{0}", pi.GetValue(this, null));
                    root.AppendChild(e);
                }
            }

            return root;
        }
    }
}