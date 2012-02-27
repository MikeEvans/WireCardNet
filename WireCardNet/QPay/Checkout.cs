using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WireCardNet.QPay
{
    /// <summary>
    /// Class for sending a request to the QPay payment page
    /// </summary>
    public class Checkout
    {
        #region Properties

        /// <summary>
        /// Payment amount
        /// </summary>
        public Decimal Amount { get; set; }

        /// <summary>
        /// After tax payment amount
        /// </summary>
        public Decimal? AmountNet { get; set; }

        /// <summary>
        /// ISO currency code
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Indicates the payment method used
        /// </summary>
        public PaymentType PaymentType { get; set; }

        /// <summary>
        /// When using EPS the supporting bank can be selected here
        /// </summary>
        public string FinancialInstitution { get; set; }

        /// <summary>
        /// Language used on the QPay page
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Description of the order (NO special characters, including umlaut, HTML entities and line breaks!)
        /// </summary>
        public string OrderDescription { get; set; }

        /// <summary>
        /// Text displayed to the consumer along with the order data (does not appear on the consumer's invoice)
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// Activates a double-booking check (enabled by default)
        /// </summary>
        public bool DuplicateRequestCheck { get; set; }

        /// <summary>
        /// URL connecting to a page indicating a successful purchase
        /// </summary>
        public string SuccessURL { get; set; }

        /// <summary>
        /// URL connecting to a page indicating a cancelled purchase
        /// </summary>
        public string CancelURL { get; set; }

        /// <summary>
        /// URL connecting to a page indicating a purchase attempt failed
        /// </summary>
        public string FailureURL { get; set; }

        /// <summary>
        /// URL connecting to a page providing service information by the merchant
        /// </summary>
        public string ServiceURL { get; set; }

        /// <summary>
        /// URL for the server-to-server confirmation (not visible)
        /// </summary>
        public string ConfirmURL { get; set; }

        /// <summary>
        /// link to your logo embedded on the QPay pages (gif, jpg, jpeg or png; no GET parameters,
        /// no authentication)
        /// </summary>
        public string ImageURL { get; set; }

        /// <summary>
        /// Text which is to appear on the consumer's statement
        /// </summary>
        public string CustomerStatement { get; set; }

        /// <summary>
        /// Unique transaction ID of the merchant
        /// </summary>
        public string OrderReference { get; set; }

        /// <summary>
        /// activates automatic settling of the payment
        /// </summary>
        public bool AutoDeposit { get; set; }

        /// <summary>
        /// maximum number of payment attempts
        /// </summary>
        public int? MaxRetries { get; set; }

        #endregion

        private readonly NameValueCollection _customParameters = new NameValueCollection();

        /// <summary>
        /// Creates a new checkout request
        /// </summary>
        public Checkout()
        {
            if (string.IsNullOrEmpty(WireCard.QPayCustomerId))
            {
                throw new WireCardException("QPay customer id is invalid. Please specify WireCard.QPayCustomerId!");
            }

            if (string.IsNullOrEmpty(WireCard.QPayCustomerSecret))
            {
                throw new WireCardException("QPay customer secret is invalid. Please specify WireCard.QPayCustomerSecret!");
            }

            DuplicateRequestCheck = true;
        }

        /// <summary>
        /// Adds a custom parameter to the request
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        public void SetCustomParameter(string name, string value)
        {
            _customParameters[name] = value;
        }

        /// <summary>
        /// Returns the value of a previously added custom parameter
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <returns>Value of the parameter or null if the name is invalid</returns>
        public string GetCustomParameter(string name)
        {
            return _customParameters[name];
        }

        /// <summary>
        /// Verifies that all parameters meet the QPay requirements and throws an exception if not
        /// </summary>
        /// <returns>true if all parameters are OK</returns>
        protected bool VerifyParameters()
        {
            #region Check for missing mandatory fields

            if (Amount <= 0)
            {
                throw new WireCardException("Amount must be greater than 0!", new ArgumentOutOfRangeException("Amount"));
            }

            if (string.IsNullOrEmpty(Currency))
            {
                throw new WireCardException("Missing mandatory field: Currency", new ArgumentNullException("Currency"));
            }

            if (string.IsNullOrEmpty(Language))
            {
                throw new WireCardException("Missing mandatory field: Language", new ArgumentNullException("Language"));
            }

            if (string.IsNullOrEmpty(OrderDescription))
            {
                throw new WireCardException("Missing mandatory field: OrderDescription", new ArgumentNullException("OrderDescription"));
            }

            if (string.IsNullOrEmpty(SuccessURL))
            {
                throw new WireCardException("Missing mandatory field: SuccessURL", new ArgumentNullException("SuccessURL"));
            }

            if (string.IsNullOrEmpty(CancelURL))
            {
                throw new WireCardException("Missing mandatory field: CancelURL", new ArgumentNullException("CancelURL"));
            }

            if (string.IsNullOrEmpty(FailureURL))
            {
                throw new WireCardException("Missing mandatory field: FailureURL", new ArgumentNullException("FailureURL"));
            }

            if (string.IsNullOrEmpty(ServiceURL))
            {
                throw new WireCardException("Missing mandatory field: ServiceURL", new ArgumentNullException("ServiceURL"));
            }

            #endregion

            #region Check for fields with character restrictions

            var rxAlphaNum = new Regex("^[A-Za-z0-9]+$");
            var rxNum = new Regex("^[0-9]+$");

            if (!rxAlphaNum.Match(OrderDescription).Success)
            {
                throw new WireCardException("OrderDescription contains invalid characters! Only alphanumeric characters are allowed!");
            }

            if (!string.IsNullOrEmpty(OrderReference) && !rxNum.Match(OrderReference).Success)
            {
                throw new WireCardException("OrderReference contains invalid characters! Only numeric characters are allowed!");
            }

            #endregion

            #region Check for field lengths

            if (OrderDescription.Length > 255)
            {
                throw new WireCardException("OrderDescription is too long! A maximum of 255 characters is allowed!");
            }

            if (!string.IsNullOrEmpty(OrderReference) && OrderReference.Length > 128)
            {
                throw new WireCardException("OrderReference is too long! A maximum of 128 digits is allowed!");
            }

            if (ServiceURL.Length > 255)
            {
                throw new WireCardException("ServiceURL is too long! A maximum of 255 characters is allowed!");
            }

            if (!string.IsNullOrEmpty(CustomerStatement) && CustomerStatement.Length > 254)
            {
                throw new WireCardException("CustomerStatement is too long! A maximum of 254 characters is allowed!");
            }

            #endregion

            #region Conditional checks

            if (PaymentType == PaymentType.Mia && !AmountNet.HasValue)
            {
                throw new WireCardException("AmountNet is required for payment type MIA!", new ArgumentNullException("AmountNet"));
            }

            #endregion

            return true;
        }

        /// <summary>
        /// Returns all form values that have to be POSTed to QPay
        /// </summary>
        /// <remarks>
        /// This method also verifies the form values and throws an exception, i.e. if a mandatory
        /// field is null.
        /// </remarks>
        /// <exception cref="WireCardNet.WireCardException">Thrown if a field violates the requirements
        /// by QPay</exception>
        /// <returns>A NameValueCollection containing form values</returns>
        public NameValueCollection GetFormValues()
        {
            var b = new FingerprintBuilder(WireCard.QPayCustomerSecret);

            foreach (string key in _customParameters.AllKeys)
            {
                b.AddValue(key, _customParameters[key]);
            }

            b.AddValue("customerId", WireCard.QPayCustomerId);

            if (!string.IsNullOrEmpty(WireCard.QPayShopId))
            {
                b.AddValue("shopId", WireCard.QPayShopId);
            }

            b.AddValue("amount", Amount.ToString("0.00", CultureInfo.InvariantCulture));

            if (AmountNet.HasValue)
            {
                b.AddValue("amount_net", Amount.ToString("0.00", CultureInfo.InvariantCulture));
            }

            b.AddValue("currency", Currency);

            if (PaymentType != PaymentType.Undefined)
            {
                b.AddValue("paymenttype", PaymentType.ToString().ToUpper().Replace('_', '-'));
            }

            if (!string.IsNullOrEmpty(FinancialInstitution))
            {
                b.AddValue("financialInstitution", FinancialInstitution);
            }

            b.AddValue("language", Language);
            b.AddValue("orderDescription", OrderDescription);

            if (!string.IsNullOrEmpty(DisplayText))
            {
                b.AddValue("displayText", DisplayText);
            }

            b.AddValue("successURL", SuccessURL);
            b.AddValue("cancelURL", CancelURL);
            b.AddValue("failureURL", FailureURL);
            b.AddValue("serviceURL", ServiceURL);

            if (!string.IsNullOrEmpty(ConfirmURL))
            {
                b.AddValue("confirmURL", ConfirmURL);
            }

            if (!string.IsNullOrEmpty(ImageURL))
            {
                b.AddValue("imageURL", ImageURL);
            }

            if (DuplicateRequestCheck)
            {
                b.AddValue("duplicateRequestCheck", "yes");
            }

            if (AutoDeposit)
            {
                b.AddValue("autoDeposit", "yes");
            }

            if (MaxRetries.HasValue)
            {
                b.AddValue("maxRetries", MaxRetries.Value.ToString(CultureInfo.InvariantCulture));
            }

            NameValueCollection form = b.GetFormValues();

            form.Add("requestFingerprintOrder", b.GetFingerprintOrder());
            form.Add("requestFingerprint", b.GetFingerprint());

            return form;
        }

        /// <summary>
        /// Returns an XHTML string containing hidden input fields with the form values that have to
        /// be POSTed to QPay.
        /// </summary>
        /// <remarks>
        /// This method also verifies the form values and throws an exception, i.e. if a mandatory
        /// field is null.
        /// </remarks>
        /// <exception cref="WireCardNet.WireCardException">Thrown if a field violates the requirements
        /// by QPay</exception>
        /// <returns>An XHTML string</returns>
        public string GetFormHtml()
        {
            NameValueCollection values = GetFormValues();
            var result = new StringBuilder();

            foreach (string key in values.AllKeys)
            {
                result.AppendLine(string.Format("<input type=\"hidden\" name=\"{0}\" value=\"{1}\" />", key, values[key]));
            }

            return result.ToString();
        }
    }
}