namespace WireCardNet.QPay
{
    /// <summary>
    /// Represents the state of a payment returned by QPay
    /// </summary>
    public enum PaymentState
    {
        /// <summary>
        /// The payment was successfully processed
        /// </summary>
        Success,

        /// <summary>
        /// The payment was cancelled by the user
        /// </summary>
        Cancel,

        /// <summary>
        /// The payment failed
        /// </summary>
        Failure
    }

    /// <summary>
    /// Represents a payment method used by QPay
    /// </summary>
    public enum PaymentType
    {
        /// <summary>
        /// Payment type will not be included in fingerprint or form. You have to specify the
        /// form value by yourself. This is useful if you have a payment type selection field on
        /// your own page.
        /// </summary>
        Undefined,

        /// <summary>
        /// No pre-selection, payment type selection is done on the QPay page
        /// </summary>
        Select,

        /// <summary>
        /// Credit card, including "Verified by VISA" and "MasterCard Secure Code"
        /// </summary>
        CCard,

        /// <summary>
        /// Credit card, without "Verified by VISA" and "MasterCard Secure Code" (only with
        /// permission by WireCard)
        /// </summary>
        CCard_Moto,
        
        /// <summary>
        /// Maestro SecureCode
        /// </summary>
        Maestro,

        /// <summary>
        /// paybox
        /// </summary>
        PBX,
        
        /// <summary>
        /// paysafecard
        /// </summary>
        PSC,

        /// <summary>
        /// eps online transaction, sub-selection is done on the QPay page
        /// </summary>
        EPS,

        /// <summary>
        /// Direct debit ("Lastschrift")
        /// </summary>
        ELV,

        /// <summary>
        /// Electronic purse "@Quick"
        /// </summary>
        QUICK,

        /// <summary>
        /// Payment through mobile phone service provider, sub-selection is done on the QPay page
        /// </summary>
        Mia,

        /// <summary>
        /// iDeal
        /// </summary>
        IDL,

        /// <summary>
        /// Giropay
        /// </summary>
        Giropay,

        /// <summary>
        /// PayPal
        /// </summary>
        PayPal
    }

    /// <summary>
    /// Contains language definition constants
    /// </summary>
    public class Language
    {
        public const string Czech = "cz";
        public const string Danish = "da";
        public const string German = "de";
        public const string Greek = "el";
        public const string English = "en";
        public const string Spanish = "es";
        public const string Finnish = "fi";
        public const string French = "fr";
        public const string Hungarian = "hu";
        public const string Italian = "it";
        public const string Japanese = "jp";
        public const string Dutch = "nl";
        public const string Portuguese = "pg";
        public const string Polish = "pl";
        public const string Russian = "ru";
        public const string Swedish = "se";
        public const string Slowakian = "sk";
        public const string Slowenian = "sl";
        public const string Chinese = "zh";
    }
}