using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace WireCardNet.Processing
{
    public class JobResponse
    {
        private readonly List<FunctionResponse> functions = new List<FunctionResponse>();

        internal JobResponse(XmlNode n)
        {
            XmlNode fid;

            if ((fid = n.SelectSingleNode("JobID")) != null)
            {
                JobId = fid.InnerText;
            }

            foreach (XmlNode child in n.ChildNodes)
            {
                if (child.NodeType != XmlNodeType.Element)
                    continue;

                if (child.Name.StartsWith("FNC_"))
                    this.functions.Add(new FunctionResponse(child));
            }
        }

        public List<FunctionResponse> Functions
        {
            get { return this.functions; }
        }

        public string JobId { get; internal set; }

        internal FunctionResponse FindFunction(string functionId)
        {
            return this.functions.FirstOrDefault(f => f.FunctionId == functionId);
        }
    }
}