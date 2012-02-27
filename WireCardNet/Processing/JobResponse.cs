using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace WireCardNet.Processing
{
    public class JobResponse
    {
        private List<FunctionResponse> functions = new List<FunctionResponse>();

        public string JobId { get; internal set; }

        internal JobResponse(XmlNode n)
        {
            XmlNode fid;

            if ((fid = n.SelectSingleNode("JobID")) != null)
            {
                JobId = fid.InnerText;
            }

            foreach (XmlNode child in n.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    if (child.Name.StartsWith("FNC_"))
                    {
                        functions.Add(new FunctionResponse(child));
                    }
                }
            }
        }

        internal FunctionResponse FindFunction(string functionId)
        {
            return functions.FirstOrDefault(f => f.FunctionId == functionId);
        }
    }
}
