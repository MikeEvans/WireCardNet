using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;

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

        private List<Job> jobs = new List<Job>();

        /// <summary>
        /// Adds a job to this request
        /// </summary>
        /// <param name="job">the job to add</param>
        public void AddJob(Job job)
        {
            jobs.Add(job);
        }

#if DEBUG
        public string GetXml()
#else
        protected string GetXml()
#endif
        {
            XmlDocument doc = new XmlDocument();
            var xpi = doc.CreateProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            doc.AppendChild(xpi);

            var root = doc.CreateElement("WIRECARD_BXML");
            root.SetAttribute("xmlns:xsi", "http://www.w3.org/1999/XMLSchema-instance");
            doc.AppendChild(root);

            var request = doc.CreateElement("W_REQUEST");
            root.AppendChild(request);

            foreach (Job job in jobs)
            {
                var node = job.GetXml(doc);
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

            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.Credentials = new NetworkCredential(WireCard.WireCardUsername, WireCard.WireCardPassword);
            req.Method = "POST";
            
            var writer = new StreamWriter(req.GetRequestStream(), Encoding.UTF8);
            writer.Write(GetXml());
            writer.Flush();

            var resp = (HttpWebResponse)req.GetResponse();

            var reader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
            var result = reader.ReadToEnd();
            reader.Dispose();

            return result;
        }

        /// <summary>
        /// Sends this request to WireCard and returns a response object
        /// </summary>
        /// <returns></returns>
        public ProcessingResponse GetResponse()
        {
            var xml = Send();
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            return new ProcessingResponse(doc.DocumentElement);
        }
    }
}
