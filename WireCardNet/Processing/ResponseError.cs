using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace WireCardNet.Processing
{
    public class ResponseError
    {
        internal ResponseError(XmlNode n)
        {
            string type = n.SelectSingleNode("Type").InnerText;

            if (type == "REJECTED")
            {
                Type = ResponseErrorType.Rejected;
            }
            else if (type == "DATA_ERROR")
            {
                Type = ResponseErrorType.DataError;
            }
            else if (type == "SYSTEM_ERROR")
            {
                Type = ResponseErrorType.SystemError;
            }
            else if (type == "CLIENT_ERROR")
            {
                Type = ResponseErrorType.ClientError;
            }

            Number = int.Parse(n.SelectSingleNode("Number").InnerText, CultureInfo.InvariantCulture);

            XmlNode el;

            if ((el = n.SelectSingleNode("Message")) != null)
            {
                Message = el.InnerText;
            }

            if ((el = n.SelectSingleNode("Advice")) != null)
            {
                Advice = el.InnerText;
            }
        }

        public ResponseErrorType Type { get; internal set; }
        public int Number { get; internal set; }
        public string Message { get; internal set; }
        public string Advice { get; internal set; }
    }
}