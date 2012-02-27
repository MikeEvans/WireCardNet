using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace WireCardNet.Processing
{
    public class Job
    {
        private readonly List<Function> _functions = new List<Function>();

        public Job()
        {
            if (!string.IsNullOrEmpty(WireCard.WireCardDefaultBCI))
            {
                BusinessCaseSignature = WireCard.WireCardDefaultBCI;
            }
        }

        public string JobId { get; set; }
        public string BusinessCaseSignature { get; set; }

        public void AddFunction(Function fnc)
        {
            if (_functions.Count == 10)
            {
                throw new WireCardException("Can't add more than 10 functions to a single job!");
            }

            _functions.Add(fnc);
        }

        public XmlElement GetXml(XmlDocument doc)
        {
            XmlElement job = doc.CreateElement("W_JOB");

            XmlElement jobid = doc.CreateElement("JobID");
            jobid.InnerText = JobId;
            job.AppendChild(jobid);

            XmlElement bcs = doc.CreateElement("BusinessCaseSignature");
            bcs.InnerText = BusinessCaseSignature;
            job.AppendChild(bcs);

            foreach (Function f in _functions)
            {
                XmlElement fnc = f.GetXml(doc);
                job.AppendChild(fnc);
            }

            return job;
        }
    }
}