using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        /// <returns>A subclass of CheckoutResponse or null if no QPay response is found in the request</returns>
        public static CheckoutResponse FromRequest(HttpRequestBase request)
        {
            if (string.IsNullOrEmpty(WireCard.QPayCustomerId))
            {
                throw new WireCardException("Customer id is invalid. Please specify WireCard.CustomerId!");
            }

            if (string.IsNullOrEmpty(WireCard.QPayCustomerSecret))
            {
                throw new WireCardException("Customer secret is invalid. Please specify WireCard.CustomerSecret!");
            }

            CheckoutResponse result = null;

            if (request.Form["paymentState"] == "SUCCESS")
            {
                var success = new CheckoutSuccessResponse
                                  {
                                      PaymentState = PaymentState.Success,
                                      Amount = Decimal.Parse(request.Form["amount"], CultureInfo.InvariantCulture),
                                      Currency = request.Form["currency"],
                                      PaymentType = (PaymentType) Enum.Parse(typeof (PaymentType), request.Form["paymentType"].Replace('-', '_'), true),
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
                {
                    success.Authenticated = (request.Form["authenticated"].ToUpper() == "YES");
                }

                success.IsValid = FingerprintBuilder.VerifyFingerprint(WireCard.QPayCustomerSecret, request.Form);

                result = success;
            }
            else if (request.Form["paymentState"] == "FAILURE")
            {
                result = new CheckoutFailureResponse { PaymentState = PaymentState.Failure };

                (result as CheckoutFailureResponse).Message = request.Form["message"];
            }
            else if (request.Form["paymentState"] == "CANCEL")
            {
                result = new CheckoutCancelResponse { PaymentState = PaymentState.Cancel };
            }

            foreach (string key in request.Form.AllKeys)
            {
                if (!ReservedParameters.Contains(key))
                {
                    result.CustomParameters.Add(key, request.Form[key]);
                }
            }

            return result;
        }
    }
}