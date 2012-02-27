using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace BillomatNet
{
    /// <summary>
    /// Specialized RestRequest for requests to the Billomat API
    /// </summary>
    internal class BillomatRequest : RestRequest
    {
        /// <summary>
        /// The resource being targeted (e.g. articles, invoices)
        /// </summary>
        public string Resource { get; set; }

        /// <summary>
        /// Optional resource identifier
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Optional method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Creates a new Billomat request. Make sure the Billomat class is initialized with some
        /// valid credentials.
        /// </summary>
        public BillomatRequest()
        {
            if (string.IsNullOrEmpty(Billomat.BillomatId) || string.IsNullOrEmpty(Billomat.ApiKey))
            {
                throw new BillomatException("BillomatId and/or API key missing! Please set up Billomat class with valid credentials!");
            }

            Params = new NameValueCollection();
            RequestHeaders = new NameValueCollection();
            Verb = "GET";
        }

        /// <summary>
        /// Sends the request to the Billomat API and returns the response bytes
        /// </summary>
        /// <returns></returns>
        public override byte[] GetResponse()
        {
            if (string.IsNullOrEmpty(Resource))
            {
                throw new BillomatException("Resource not set!");
            }

            var url = new StringBuilder();

            url.Append(Billomat.UseHttps ? "https://" : "http://");

            url.Append(Billomat.BillomatId);
            url.Append(".billomat.net/api/");
            url.Append(Resource);

            if (Id.HasValue)
            {
                url.Append("/");
                url.Append(Id.Value);

                if (!string.IsNullOrEmpty(Method))
                {
                    url.Append("/");
                    url.Append(Method);
                }
            }

            Url = url.ToString();

            ContentType = "application/xml"; // request format
            Params["format"] = "xml";        // response format

            RequestHeaders["X-BillomatApiKey"] = Billomat.ApiKey;

            try
            {
                return base.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var resp = ex.Response as HttpWebResponse;

                    // try to read Billomat's custom error message first
                    var sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
                    string body = sr.ReadToEnd();

                    var doc = XDocument.Parse(body);

                    var err = doc.XPathSelectElement("/errors/error");
                    if (err != null)
                    {
                        throw new BillomatException((string) err, ex);
                    }

                    // if Billomat didn't send a custom error message
                    if (resp.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new BillomatException("Not authorized to access this resource!", ex);
                    }
                    if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new BillomatException("Resource not found!", ex);
                    }
                }
                
                throw new BillomatException("Request failed", ex);
            }
            catch (Exception ex)
            {
                throw new BillomatException("Request failed", ex);
            }
        }

        /// <summary>
        /// Sends the request and tries to interprete the response as XML
        /// </summary>
        /// <returns>The root element of the XML response</returns>
        public XElement GetXmlResponse()
        {
            try
            {
                var doc = XDocument.Parse(GetString());
                return doc.Root;
            }
            catch (XmlException ex)
            {
                throw new BillomatException("Could not parse response!", ex);
            }
        }
    }
}
