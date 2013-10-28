namespace WireCardNet.QPay
{
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
        PayPal,

		/// <summary>
		/// SOFORTUEBERWEISUNG
		/// </summary>
		Sofortueberweisung
    }
}