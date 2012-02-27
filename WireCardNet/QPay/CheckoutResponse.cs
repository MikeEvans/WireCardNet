using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Globalization;
using System.Collections.Specialized;

namespace WireCardNet.QPay
{
    /// <summary>
    /// Base class for all responses sent by the QPay page
    /// </summary>
    public abstract class CheckoutResponse
    {
        private static string[] reservedParameters = new string[] {
            "paymentState", "amount", "currency", "financialInstitution", "language", "orderNumber",
            "anonymousPan", "authenticated", "message", "expiry", "cardholder", "maskedPan",
            "gatewayReferenceNumber", "gatewayContractNumber", "idealConsumerName", "idealConsumerCity",
            "idealConsumerAccountNumber", "paypalPayerID", "paypalPayerEmail", "paypalPayerLastName",
            "paypalPayerFirstName", "responseFingerprint", "responseFingerprintOrder"
        };

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
                var success = new CheckoutSuccessResponse();
                
                success.PaymentState = PaymentState.Success;
                success.Amount = Decimal.Parse(request.Form["amount"], CultureInfo.InvariantCulture);
                success.Currency = request.Form["currency"];

                success.PaymentType = (PaymentType)Enum.Parse(typeof(PaymentType), request.Form["paymentType"].Replace('-', '_'), true);

                success.FinancialInstitution = request.Form["financialInstitution"];
                success.Language = request.Form["language"];
                success.OrderNumber = request.Form["orderNumber"];
                success.AnonymousPan = request.Form["anonymousPan"];

                if (request.Form["authenticated"] != null)
                {
                    success.Authenticated = (request.Form["authenticated"].ToUpper() == "YES");
                }

                success.Message = request.Form["message"];

                success.Expiry = request.Form["expiry"];
                success.Cardholder = request.Form["cardholder"];
                success.MaskedPan = request.Form["maskedPan"];
                success.GatewayReferenceNumber = request.Form["gatewayReferenceNumber"];
                success.GatewayContractNumber = request.Form["gatewayContractNumber"];

                success.IDealConsumerName = request.Form["idealConsumerName"];
                success.IDealConsumerCity = request.Form["idealConsumerCity"];
                success.IDealConsumerAccountNumber = request.Form["idealConsumerAccountNumber"];

                success.PayPalPayerID = request.Form["paypalPayerID"];
                success.PayPalPayerEMail = request.Form["paypalPayerEmail"];
                success.PayPalPayerLastName = request.Form["paypalPayerLastName"];
                success.PayPalPayerFirstName = request.Form["paypalPayerFirstName"];

                success.IsValid = FingerprintBuilder.VerifyFingerprint(WireCard.QPayCustomerSecret, request.Form);

                result = success;
            }
            else if (request.Form["paymentState"] == "FAILURE")
            {
                result = new CheckoutFailureResponse();
                result.PaymentState = PaymentState.Failure;

                (result as CheckoutFailureResponse).Message = request.Form["message"];
            }
            else if (request.Form["paymentState"] == "CANCEL")
            {
                result = new CheckoutCancelResponse();
                result.PaymentState = PaymentState.Cancel;
            }

            foreach (string key in request.Form.AllKeys)
            {
                if (!reservedParameters.Contains(key))
                {
                    result.CustomParameters.Add(key, request.Form[key]);
                }
            }

            return result;
        }

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
    }
}
