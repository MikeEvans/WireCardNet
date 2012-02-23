using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Net;
using System.IO;

namespace BillomatNet
{
    /// <summary>
    /// Encapsulates an HTTP(S) REST request to a specified URL and processes the result
    /// </summary>
    internal class RestRequest
    {
        /// <summary>
        /// The HTTP content type of this request
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// HTTP verb to use (e.g. GET, POST, PUT, DELETE)
        /// </summary>
        public string Verb { get; set; }

        /// <summary>
        /// The request body (must be null for verbs GET and DELETE)
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Additional parameters for the URL (query string)
        /// </summary>
        public NameValueCollection Params { get; set; }

        /// <summary>
        /// Additional HTTP headers for the request
        /// </summary>
        public NameValueCollection RequestHeaders { get; set; }

        /// <summary>
        /// HTTP headers of the response
        /// </summary>
        public NameValueCollection ResponseHeaders { get; private set; }

        /// <summary>
        /// The URL to send the request to
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Sends the request and returns the resonse bytes
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GetResponse()
        {
            StringBuilder url = new StringBuilder();
            url.Append(Url);

            if (Params != null)
            {
                bool first = true;
                foreach (string key in Params.AllKeys)
                {
                    url.Append(first ? "?" : "&");
                    url.Append(key);
                    url.Append("=");
                    url.Append(Params[key]);
                    first = false;
                }
            }

            var request = (HttpWebRequest)WebRequest.Create(url.ToString());
            request.Headers.Add(RequestHeaders);
            request.Method = Verb;

            if (!string.IsNullOrEmpty(ContentType))
            {
                request.ContentType = ContentType;
            }

            if (!string.IsNullOrEmpty(Body))
            {
                Stream s = request.GetRequestStream();
                byte[] bytes = Encoding.UTF8.GetBytes(Body);
                s.Write(bytes, 0, bytes.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            if ((int)response.StatusCode / 100 != 2)
            {
                throw new Exception(string.Format("{0}: {1}", (int)response.StatusCode, response.StatusDescription));
            }

            ResponseHeaders = response.Headers;

            byte[] result;

            using (var ms = new MemoryStream())
            {
                int bytesRead = 0;
                do
                {
                    byte[] buf = new byte[32768];
                    bytesRead = response.GetResponseStream().Read(buf, 0, buf.Length);
                    ms.Write(buf, 0, bytesRead);
                } while (bytesRead > 0);

                result = ms.ToArray();
            }

            return result;
        }

        /// <summary>
        /// Sends the request, reads the response bytes as UTF8 string and returns the string
        /// </summary>
        /// <returns></returns>
        public virtual string GetString()
        {
            return Encoding.UTF8.GetString(GetResponse());
        }
    }
}
