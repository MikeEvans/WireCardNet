using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace WireCardNet.Processing
{
    public class Job
    {
        protected List<Function> functions = new List<Function>();

        public string JobId { get; set; }
        public string BusinessCaseSignature { get; set; }

        public Job()
        {
            if (!string.IsNullOrEmpty(WireCard.WireCardDefaultBCI))
            {
                BusinessCaseSignature = WireCard.WireCardDefaultBCI;
            }
        }

        public void AddFunction(Function fnc)
        {
            if (functions.Count == 10)
            {
                throw new WireCardException("Can't add more than 10 functions to a single job!");
            }

            functions.Add(fnc);
        }

        public XmlElement GetXml(XmlDocument doc)
        {
            var job = doc.CreateElement("W_JOB");

            var jobid = doc.CreateElement("JobID");
            jobid.InnerText = JobId;
            job.AppendChild(jobid);

            var bcs = doc.CreateElement("BusinessCaseSignature");
            bcs.InnerText = BusinessCaseSignature;
            job.AppendChild(bcs);

            foreach (Function f in functions)
            {
                var fnc = f.GetXml(doc);
                job.AppendChild(fnc);
            }

            return job;
        }
    }
}
