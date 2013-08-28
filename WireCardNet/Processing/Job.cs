using System.Collections.Generic;
using System.Xml;

namespace WireCardNet.Processing
{
    public class Job
    {
        private readonly List<Function> functions = new List<Function>();

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
            if (this.functions.Count == 10)
            {
                throw new WireCardException("Can't add more than 10 functions to a single job!");
            }

            this.functions.Add(fnc);
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

            foreach (var f in this.functions)
            {
                var fnc = f.GetXml(doc);
                job.AppendChild(fnc);
            }

            return job;
        }
    }
}