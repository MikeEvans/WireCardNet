using System;
using System.Globalization;
using System.Xml;

namespace WireCardNet.Processing
{
    public class ProcessingStatus
    {
        internal ProcessingStatus(XmlNode n)
        {
            GuWID = n.SelectSingleNode("GuWID").InnerText;
            AuthorizationCode = n.SelectSingleNode("AuthorizationCode").InnerText;
            Result = (FunctionResult) Enum.Parse(typeof (FunctionResult), n.SelectSingleNode("FunctionResult").InnerText, true);

            var error = n.SelectSingleNode("ERROR");

            if (Result == FunctionResult.NOK && error != null)
            {
                Error = new ResponseError(error);
            }

            var timestamp = n.SelectSingleNode("TimeStamp").InnerText;
            TimeStamp = DateTime.ParseExact(timestamp, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        public string GuWID { get; internal set; }
        public string AuthorizationCode { get; internal set; }
        public DateTime TimeStamp { get; internal set; }
        public FunctionResult Result { get; internal set; }
        public ResponseError Error { get; internal set; }
    }
}