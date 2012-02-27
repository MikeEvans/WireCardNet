using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace WireCardNet.Processing
{
    public class ProcessingRequest
    {
        public ProcessingRequest()
        {
            if (string.IsNullOrEmpty(WireCard.WireCardUsername))
            {
                throw new WireCardException("WireCard username is invalid. Please specify WireCard.WireCardUsername!");
            }

            if (string.IsNullOrEmpty(WireCard.WireCardPassword))
            {
                throw new WireCardException("WireCard password is invalid. Please specify WireCard.WireCardPassword!");
            }
        }

        private readonly List<Job> _jobs = new List<Job>();

        /// <summary>
        /// Adds a job to this request
        /// </summary>
        /// <param name="job">the job to add</param>
        public void AddJob(Job job)
        {
            _jobs.Add(job);
        }

#if DEBUG
        public string GetXml()
#else
        protected string GetXml()
#endif
        {
            var doc = new XmlDocument();
            XmlProcessingInstruction xpi = doc.CreateProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            doc.AppendChild(xpi);

            XmlElement root = doc.CreateElement("WIRECARD_BXML");
            root.SetAttribute("xmlns:xsi", "http://www.w3.org/1999/XMLSchema-instance");
            doc.AppendChild(root);

            XmlElement request = doc.CreateElement("W_REQUEST");
            root.AppendChild(request);

            foreach (Job job in _jobs)
            {
                XmlElement node = job.GetXml(doc);
                request.AppendChild(node);
            }

            return doc.OuterXml;
        }

#if DEBUG
        public string Send()
#else
        protected string Send()
#endif
        {
            var uri = new Uri("https://c3-test.wirecard.com/secure/ssl-gateway");

            var req = (HttpWebRequest) WebRequest.Create(uri);
            req.Credentials = new NetworkCredential(WireCard.WireCardUsername, WireCard.WireCardPassword);
            req.Method = "POST";

            var writer = new StreamWriter(req.GetRequestStream(), Encoding.UTF8);
            writer.Write(GetXml());
            writer.Flush();

            var resp = (HttpWebResponse) req.GetResponse();

            var reader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();
            reader.Dispose();

            return result;
        }

        /// <summary>
        /// Sends this request to WireCard and returns a response object
        /// </summary>
        /// <returns></returns>
        public ProcessingResponse GetResponse()
        {
            string xml = Send();
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            return new ProcessingResponse(doc.DocumentElement);
        }
    }
}