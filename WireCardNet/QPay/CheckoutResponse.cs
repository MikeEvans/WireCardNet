using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WireCardNet.QPay
{
    /// <summary>
    /// Base class for all responses sent by the QPay page
    /// </summary>
    public abstract class CheckoutResponse
    {
        private static readonly string[] ReservedParameters = new[]
        {
            "paymentState", "amount", "currency", "financialInstitution", "language", "orderNumber",
            "anonymousPan", "authenticated", "message", "expiry", "cardholder", "maskedPan",
            "gatewayReferenceNumber", "gatewayContractNumber", "idealConsumerName", "idealConsumerCity",
            "idealConsumerAccountNumber", "paypalPayerID", "paypalPayerEmail", "paypalPayerLastName",
            "paypalPayerFirstName", "responseFingerprint", "responseFingerprintOrder"
        };

        protected CheckoutResponse()
        {
            CustomParameters = new NameValueCollection();
        }

        /// <summary>
        /// Custom parameters that were passed to the QPay page in the original request
        /// </summary>
        public NameValueCollection CustomParameters { get; internal set; }

        /// <summary>
        /// Payment state returned by QPay
        /// </summary>
        public PaymentState PaymentState { get; internal set; }

        /// <summary>
        /// Factory method that creates a checkout response from the specified HTTP request
        /// </summary>
        /// <param name="request">The request to create the response from</param>
        /// <param name="successCallback"></param>
        /// <param name="failureCallback"></param>
        /// <param name="cancelCallback"></param>
        /// <returns>A subclass of CheckoutResponse or null if no QPay response is found in the request</returns>
        public static CheckoutResponse FromRequest(HttpRequestBase request,
            Action<CheckoutSuccessResponse> successCallback = null,
            Action<CheckoutFailureResponse> failureCallback = null,
            Action<CheckoutCancelResponse> cancelCallback = null)
        {
            if (string.IsNullOrEmpty(WireCard.QPayCustomerId))
            {
                throw new WireCardException("Customer id is invalid. Please specify WireCard.CustomerId!");
            }

            if (string.IsNullOrEmpty(WireCard.QPayCustomerSecret))
            {
                throw new WireCardException("Customer secret is invalid. Please specify WireCard.CustomerSecret!");
            }

            CheckoutResponse checkoutResponse = null;

            Debug.WriteLine("checkout response: " + request.Form.ToString());

            var paymentState = request.Form["paymentState"];
            if (paymentState.Equals("SUCCESS", StringComparison.InvariantCultureIgnoreCase))
            {
                var successResponse = new CheckoutSuccessResponse
                {
                    PaymentState = PaymentState.Success,
                    Amount = Decimal.Parse(request.Form["amount"], CultureInfo.InvariantCulture),
                    Currency = request.Form["currency"],
                    PaymentType = (PaymentType)Enum.Parse(typeof(PaymentType), request.Form["paymentType"].Replace('-', '_'), true),
                    FinancialInstitution = request.Form["financialInstitution"],
                    Language = request.Form["language"],
                    OrderNumber = request.Form["orderNumber"],
                    AnonymousPan = request.Form["anonymousPan"],
                    Message = request.Form["message"],
                    Expiry = request.Form["expiry"],
                    Cardholder = request.Form["cardholder"],
                    MaskedPan = request.Form["maskedPan"],
                    GatewayReferenceNumber = request.Form["gatewayReferenceNumber"],
                    GatewayContractNumber = request.Form["gatewayContractNumber"],
                    IDealConsumerName = request.Form["idealConsumerName"],
                    IDealConsumerCity = request.Form["idealConsumerCity"],
                    IDealConsumerAccountNumber = request.Form["idealConsumerAccountNumber"],
                    PayPalPayerID = request.Form["paypalPayerID"],
                    PayPalPayerEMail = request.Form["paypalPayerEmail"],
                    PayPalPayerLastName = request.Form["paypalPayerLastName"],
                    PayPalPayerFirstName = request.Form["paypalPayerFirstName"]
                };

                if (request.Form["authenticated"] != null)
                    successResponse.Authenticated = request.Form["authenticated"].Equals("YES", StringComparison.InvariantCultureIgnoreCase);

                successResponse.IsValid = FingerprintBuilder.VerifyFingerprint(WireCard.QPayCustomerSecret, request.Form);

                checkoutResponse = successResponse;
            }
            else if (paymentState.Equals("FAILURE", StringComparison.InvariantCultureIgnoreCase))
            {
                var failureResponse = new CheckoutFailureResponse
                {
                    PaymentState = PaymentState.Failure,
                    Message = request.Form["message"]
                };

                checkoutResponse = failureResponse;
            }
            else if (paymentState.Equals("CANCEL", StringComparison.InvariantCultureIgnoreCase))
            {
                var cancelResponse = new CheckoutCancelResponse { PaymentState = PaymentState.Cancel };

                checkoutResponse = cancelResponse;
            }

            if (checkoutResponse != null)
            {
                foreach (var key in request.Form.AllKeys)
                {
                    if (!string.IsNullOrEmpty(key) && !ReservedParameters.Contains(key))
                    {
                        checkoutResponse.CustomParameters.Add(key, request.Form[key]);
                    }
                }
            }

            if (successCallback != null) 
                successCallback((CheckoutSuccessResponse)checkoutResponse);

            else if (failureCallback != null) 
                failureCallback((CheckoutFailureResponse)checkoutResponse);

            else if (cancelCallback != null) 
                cancelCallback((CheckoutCancelResponse)checkoutResponse);

            return checkoutResponse;
        }
    }
}