
namespace WireCardNet
{
    public class WireCard
    {
        /// <summary>
        /// Your customer id
        /// </summary>
        public static string QPayCustomerId { get; set; }

        /// <summary>
        /// Preshared key for signing requests
        /// </summary>
        public static string QPayCustomerSecret { get; set; }

        /// <summary>
        /// Shop id (in case you are using more than one license)
        /// </summary>
        public static string QPayShopId { get; set; }

        /// <summary>
        /// The WireCard Business Case Signature to use as default
        /// </summary>
        public static string WireCardDefaultBCI { get; set; }

        /// <summary>
        /// WireCard username for HTTPS requests
        /// </summary>
        public static string WireCardUsername { get; set; }

        /// <summary>
        /// WireCard password for HTTPS requests
        /// </summary>
        public static string WireCardPassword { get; set; }

        /// <summary>
        /// Initializes this library with the WireCard demo account - no real money will be transferred!
        /// </summary>
        public static void SetupDemoAccount()
        {
            QPayCustomerId = "D200411";
            QPayCustomerSecret = "CHCSH7UGHVVX2P7EHDHSY4T2S4CGYK4QBE4M5YUUG2ND5BEZWNRZW5EJYVJQ";
            WireCardDefaultBCI = "00000031629CAFD5";
            WireCardUsername = "00000031629CA9FA";
            WireCardPassword = "TestXAPTER";
        }
    }
}