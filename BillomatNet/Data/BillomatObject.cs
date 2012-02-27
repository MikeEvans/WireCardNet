using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using JetBrains.Annotations;

namespace BillomatNet.Data
{
    /// <summary>
    /// Base class for all Billomat objects
    /// </summary>
    /// <typeparam name="T">The specific Billomat object class</typeparam>
    public abstract class BillomatObject<T> where T : BillomatObject<T>, new()
    {
        #region Properties for every Billomat object

        /// <summary>
        /// Returns the ID of this Billomat object
        /// </summary>
        [BillomatField("id")]
        [BillomatReadOnly]
        public int Id { get; internal set; }

        /// <summary>
        /// Returns the creation date of this Billomat object
        /// </summary>
        [BillomatField("created")]
        [BillomatReadOnly]
        public DateTime Created { get; internal set; }

        #endregion

        /// <summary>
        /// Returns the ResourceAttribute associated with class T
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BillomatNet.BillomatException">Thrown if class T does not have a BillomatResourceAttribute</exception>
        protected static BillomatResourceAttribute GetResource()
        {
            Type t = typeof (T);

            var br = (BillomatResourceAttribute) Attribute.GetCustomAttribute(t, typeof (BillomatResourceAttribute));

            if (br == null)
            {
                throw new BillomatException(string.Format("Type '{0}' has no BillomatResourceAttribute!", t.FullName));
            }

            return br;
        }

        /// <summary>
        /// Finds the Billomat object with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PublicAPI]
        public static T Find(int id)
        {
            var req = new BillomatRequest
                          {
                              Verb = "GET",
                              Resource = GetResource().ResourceName,
                              Id = id
                          };

            return CreateFromXml(req.GetXmlResponse());
        }

        /// <summary>
        /// Finds all Billomat objects with the specified parameters. Sends multiple requests if
        /// necessary (due to page size).
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected static List<T> FindAll(NameValueCollection parameters = null)
        {
            BillomatResourceAttribute resource = GetResource();

            var req = new BillomatRequest
                          {
                              Verb = "GET",
                              Resource = resource.ResourceName
                          };

            if (parameters != null)
            {
                req.Params.Add(parameters);
            }

            req.Params["per_page"] = Billomat.PageSize.ToString(CultureInfo.InvariantCulture);
            int page = 0;
            int total = 0;

            T[] result = null;

            do
            {
                page++;

                req.Params["page"] = page.ToString(CultureInfo.InvariantCulture);

                XElement xml = req.GetXmlResponse();

                if (result == null)
                {
                    total = (int) xml.Attribute("total");
                    result = new T[total];
                }

                int i = 0;
                foreach (var obj in xml.XPathSelectElements(string.Format("/{0}/{1}", resource.XmlMultiName, resource.XmlSingleName)))
                {
                    result[(page - 1) * Billomat.PageSize + i] = CreateFromXml(obj);
                    i++;
                }
            } while (page * Billomat.PageSize < total);

            return result.ToList();
        }

        /// <summary>
        /// Returns a dictionary (ID => object) for all items of type T
        /// </summary>
        /// <returns></returns>
        protected static Dictionary<int, T> GetList()
        {
            return FindAll().ToDictionary(item => item.Id);
        }

        /// <summary>
        /// Copies the values from all Billomat properties from obj to this
        /// </summary>
        /// <param name="obj">The object to create a copy of</param>
        protected void ApplyFrom(BillomatObject<T> obj)
        {
            PropertyInfo[] pi = typeof (T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (PropertyInfo prop in pi)
            {
                var ba = (BillomatFieldAttribute) Attribute.GetCustomAttribute(prop, typeof (BillomatFieldAttribute));

                if (ba == null)
                {
                    continue;
                }

                try
                {
                    object value = prop.GetValue(obj, null);
                    prop.SetValue(this, value, null);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Creates a new Billomat object
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BillomatNet.BillomatException">Thrown if creating a new object is prohibited by the corresponding flag in the BillomatResourceAttribute</exception>
        [PublicAPI]
        public virtual T Create()
        {
            BillomatResourceAttribute resource = GetResource();

            if (resource.Flags.HasFlag(BillomatResourceFlags.NoCreate))
            {
                throw new BillomatException(string.Format("Creating new objects is not allowed for {0}!", GetType().Name), new NotSupportedException());
            }

            var xml = CreateXml(resource.XmlSingleName, this);

            var req = new BillomatRequest
                          {
                              Verb = "POST",
                              Resource = resource.ResourceName,
                              Body = xml.ToString(SaveOptions.DisableFormatting)
                          };

            T result = CreateFromXml(req.GetXmlResponse());

            // save the returned values (especially id)
            ApplyFrom(result);

            return result;
        }

        /// <summary>
        /// Updates an existing Billomat object
        /// </summary>
        /// <exception cref="BillomatNet.BillomatException">Thrown if updating the object is prohibited by the corresponding flag in the BillomatResourceAttribute</exception>
        [PublicAPI]
        public virtual void Update()
        {
            BillomatResourceAttribute resource = GetResource();

            if (resource.Flags.HasFlag(BillomatResourceFlags.NoUpdate))
            {
                throw new BillomatException(string.Format("Updating an object is not allowed for {0}!", GetType().Name), new NotSupportedException());
            }

            var xml = CreateXml(resource.XmlSingleName, this);

            var req = new BillomatRequest
                          {
                              Verb = "PUT",
                              Resource = resource.ResourceName,
                              Id = Id,
                              Body = xml.ToString(SaveOptions.DisableFormatting)
                          };

            req.GetXmlResponse();
        }

        /// <summary>
        /// Deletes a Billomat object
        /// </summary>
        /// <returns></returns>
        /// <exception cref="BillomatNet.BillomatException">Thrown if deleting the object is prohibited by the corresponding flag in the BillomatResourceAttribute</exception>
        [PublicAPI]
        public virtual bool Delete()
        {
            BillomatResourceAttribute resource = GetResource();

            if (resource.Flags.HasFlag(BillomatResourceFlags.NoDelete))
            {
                throw new BillomatException(string.Format("Deleting an object is not allowed for {0}!", GetType().Name), new NotSupportedException());
            }

            var req = new BillomatRequest
                          {
                              Verb = "DELETE",
                              Resource = resource.ResourceName,
                              Id = Id
                          };

            try
            {
                req.GetXmlResponse();
                return true;
            }
            catch (BillomatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates an XML element and sets its child nodes according to the properties of the
        /// specified Billomat object.
        /// </summary>
        /// <param name="tagName">The tag name of the new XML element</param>
        /// <param name="obj">The Billomat object to be converted to XML</param>
        /// <exception cref="BillomatNet.BillomatException">Thrown if an unsupported property type is encountered</exception>
        /// <returns></returns>
        internal static XElement CreateXml(string tagName, BillomatObject<T> obj)
        {
            Type t = typeof (T);

            PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            var root = new XElement(tagName);

            foreach (PropertyInfo prop in properties)
            {
                var ba = (BillomatFieldAttribute) Attribute.GetCustomAttribute(prop, typeof (BillomatFieldAttribute));

                if (ba == null)
                {
                    // only include properties with BillomatFieldAttribute
                    continue;
                }

                var bro = (BillomatReadOnlyAttribute) Attribute.GetCustomAttribute(prop, typeof (BillomatReadOnlyAttribute));

                if (bro != null)
                {
                    // exclude properties with BillomatReadOnlyAttribute
                    continue;
                }

                var elem = new XElement(ba.AttributeName);
                object value = prop.GetValue(obj, null);

                if (value == null)
                {
                    continue;
                }

                if (prop.PropertyType == typeof (int))
                {
                    elem.Add(new XAttribute("type", "integer"));
                    elem.Value = ((int) value).ToString(CultureInfo.InvariantCulture);
                }
                else if (prop.PropertyType == typeof (int?))
                {
                    elem.Add(new XAttribute("type", "integer"));
                    elem.Value = ((int?) value).Value.ToString(CultureInfo.InvariantCulture);
                }

                else if (prop.PropertyType == typeof (float))
                {
                    elem.Add(new XAttribute("type", "float"));
                    elem.Value = ((float) value).ToString(CultureInfo.InvariantCulture);
                }
                else if (prop.PropertyType == typeof (float?))
                {
                    elem.Add(new XAttribute("type", "float"));
                    elem.Value = ((float?) value).Value.ToString(CultureInfo.InvariantCulture);
                }

                else if (prop.PropertyType == typeof (DateTime))
                {
                    elem.Add(new XAttribute("type", "datetime"));
                    elem.Value = ((DateTime) value).ToString("s");
                }
                else if (prop.PropertyType == typeof (DateTime?))
                {
                    elem.Add(new XAttribute("type", "datetime"));
                    elem.Value = ((DateTime?) value).Value.ToString("s");
                }

                else if (prop.PropertyType == typeof (string))
                {
                    elem.Value = (string) value;
                }
                else if (prop.PropertyType == typeof (bool))
                {
                    elem.Value = ((bool) value) ? "1" : "0";
                }
                else
                {
                    throw new BillomatException(string.Format("Property type '{0}' is not supported as Billomat field", prop.PropertyType.FullName), new NotSupportedException());
                }

                root.Add(elem);
            }

            return root;
        }

        /// <summary>
        /// Creates a new instance of T and sets its properties according to the child nodes of
        /// the specified XML element
        /// </summary>
        /// <param name="node">An XML element representing a Billomat object of type T</param>
        /// <exception cref="BillomatNet.BillomatException">Thrown if a property type doesn't match the type specified in XML, or if the XML value could not be converted to the property type, or if the property type is not supported</exception>
        /// <returns></returns>
        internal static T CreateFromXml(XElement node)
        {
            Type t = typeof (T);

            PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var item = new T();

            foreach (PropertyInfo prop in properties)
            {
                var ba = (BillomatFieldAttribute) Attribute.GetCustomAttribute(prop, typeof (BillomatFieldAttribute));

                if (ba == null)
                {
                    continue;
                }

                var child = node.XPathSelectElement(ba.AttributeName);

                if (child == null)
                {
                    continue;
                }

                var value = (string) child;

                string xmlType = child.Attribute("type") != null ? (string) child.Attribute("type") : "string";

                if (prop.PropertyType == typeof (float) || prop.PropertyType == typeof (float?))
                {
                    if (xmlType != "float")
                    {
                        throw new BillomatException(string.Format("Type mismatch for '{1}'! Expected 'float', found '{0}'", xmlType, prop.Name));
                    }

                    try
                    {
                        float v = (value == "") ? 0 : float.Parse(value, CultureInfo.InvariantCulture);
                        prop.SetValue(item, v, null);
                    }
                    catch (FormatException ex)
                    {
                        throw new BillomatException(string.Format("Could not convert value '{0}' to float!", value), ex);
                    }
                }

                else if (prop.PropertyType == typeof (int) || prop.PropertyType == typeof (int?))
                {
                    if (xmlType != "integer")
                    {
                        throw new BillomatException(string.Format("Type mismatch for '{1}'! Expected 'integer', found '{0}'", xmlType, prop.Name));
                    }

                    try
                    {
                        int v = (value == "") ? 0 : int.Parse(value, CultureInfo.InvariantCulture);
                        prop.SetValue(item, v, null);
                    }
                    catch (FormatException ex)
                    {
                        throw new BillomatException(string.Format("Could not convert value '{0}' to int!", value), ex);
                    }
                }

                else if (prop.PropertyType == typeof (DateTime) || prop.PropertyType == typeof (DateTime?))
                {
                    if (xmlType == "datetime")
                    {
                        try
                        {
                            DateTime v = DateTime.Parse(value);
                            prop.SetValue(item, v, null);
                        }
                        catch (FormatException ex)
                        {
                            throw new BillomatException(string.Format("Could not convert value '{0}' to DateTime!", value), ex);
                        }
                    }
                    else if (xmlType == "date")
                    {
                        try
                        {
                            DateTime v = DateTime.ParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            prop.SetValue(item, v, null);
                        }
                        catch (FormatException ex)
                        {
                            throw new BillomatException(string.Format("Could not convert value '{0}' to Date!", value), ex);
                        }
                    }
                    else
                    {
                        throw new BillomatException(string.Format("Type mismatch for '{1}'! Expected 'date' or 'datetime', found '{0}'", xmlType, prop.Name));
                    }
                }

                else if (prop.PropertyType == typeof (string))
                {
                    if (xmlType != "string")
                    {
                        throw new BillomatException(string.Format("Type mismatch! Expected 'string' (or undefined), found '{0}'", xmlType));
                    }

                    prop.SetValue(item, value, null);
                }

                else if (prop.PropertyType == typeof (bool))
                {
                    if (xmlType != "bool")
                    {
                        throw new BillomatException(string.Format("Type mismatch for '{1}'! Expected 'bool', found '{0}'", xmlType, prop.Name));
                    }

                    try
                    {
                        int v = int.Parse(value, CultureInfo.InvariantCulture);
                        prop.SetValue(item, (v != 0), null);
                    }
                    catch (FormatException ex)
                    {
                        throw new BillomatException(string.Format("Could not convert value '{0}' to bool!", value), ex);
                    }
                }

                else
                {
                    throw new BillomatException(string.Format("Property type '{0}' for field '{1}' is not supported as Billomat field", prop.PropertyType.FullName, prop.Name),
                                                new NotSupportedException());
                }
            }

            return item;
        }
    }
}