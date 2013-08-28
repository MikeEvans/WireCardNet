
namespace WireCardNet.Processing.Data
{
    public class CorpTrustCenterData : AbstractData
    {
        public CorpTrustCenterData() : base("CORPTRUSTCENTER_DATA")
        {
        }

        public Address Address { get; set; }
    }
}