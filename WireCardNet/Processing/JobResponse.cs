using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace WireCardNet.Processing
{
    public class JobResponse
    {
        private readonly List<FunctionResponse> _functions = new List<FunctionResponse>();

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
                        _functions.Add(new FunctionResponse(child));
                    }
                }
            }
        }

        public string JobId { get; internal set; }

        internal FunctionResponse FindFunction(string functionId)
        {
            return _functions.FirstOrDefault(f => f.FunctionId == functionId);
        }
    }
}