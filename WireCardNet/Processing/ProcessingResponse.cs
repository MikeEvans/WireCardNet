using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace WireCardNet.Processing
{
    public class ProcessingResponse
    {
        private readonly List<JobResponse> jobs = new List<JobResponse>();

        internal ProcessingResponse(XmlNode n)
        {
            var xmlNodeList = n.SelectNodes("W_RESPONSE/W_JOB");
            if (xmlNodeList != null)
                foreach (XmlNode job in xmlNodeList)
                {
                    this.jobs.Add(new JobResponse(job));
                }
        }

        public List<JobResponse> Jobs
        {
            get { return this.jobs; }
        }

        /// <summary>
        /// Finds the status of a transaction in this response
        /// </summary>
        /// <param name="jobId">job id to look for</param>
        /// <param name="functionId">function id to look for</param>
        /// <param name="transactionId">transaction id to look for</param>
        /// <returns>A ProcessingStatus of the transaction or null if the transaction was not found</returns>
        public ProcessingStatus FindStatus(string jobId, string functionId, string transactionId)
        {
            var job = this.jobs.FirstOrDefault(j => j.JobId == jobId);

            if (job == null)
            {
                return null;
            }

            var function = job.FindFunction(functionId);

            if (function == null)
            {
                return null;
            }

            var transaction = function.FindTransaction(transactionId);

            if (transaction == null)
            {
                return null;
            }

            return transaction.ProcessingStatus;
        }
    }
}