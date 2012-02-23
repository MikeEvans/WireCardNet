using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.Specialized;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BillomatNet
{
    /// <summary>
    /// Static class for Billomat account data
    /// </summary>
    public static class Billomat
    {
        static Billomat()
        {
            _billomatId = null;
            _apiKey = null;
            UseHttps = false;
        }

        /// <summary>
        /// Sets up account data for Billomat
        /// </summary>
        /// <param name="billomatId">Your Billomat ID</param>
        /// <param name="apiKey">Your API key</param>
        /// <param name="useHttps">Whether to use HTTPS instead of HTTP (HTTPS is not available in Tarif S)</param>
        public static void SetAccount(string billomatId, string apiKey, bool useHttps)
        {
            BillomatId = billomatId;
            ApiKey = apiKey;
            UseHttps = useHttps;
        }

        private static string _billomatId;

        /// <summary>
        /// Gets or sets the BillomatId that identifies your account
        /// </summary>
        public static string BillomatId
        {
            get { return _billomatId; }
            set
            {
                if (!Regex.Match(value, "^[a-z0-9]+$").Success)
                {
                    throw new BillomatException("Invalid BillomatId! BillomatId may only contain lower-case alphanumeric characters!", new ArgumentException());
                }

                _billomatId = value;
            }
        }

        private static string _apiKey;

        /// <summary>
        /// Gets or sets the API key associated with your Billomat account
        /// </summary>
        public static string ApiKey
        {
            get { return _apiKey; }
            set
            {
                if (!Regex.Match(value, "^[a-f0-9]{32}$", RegexOptions.IgnoreCase).Success)
                {
                    throw new BillomatException("Invalid API key! API key must be a 32-digit hexadecimal number!", new ArgumentException());
                }

                _apiKey = value.ToLower();
            }
        }

        /// <summary>
        /// Gets or sets whether to use HTTPS instead of HTTP when sending a request
        /// </summary>
        public static bool UseHttps { get; set; }

        private static int _pageSize = 100;

        /// <summary>
        /// Gets or sets the page size for "FindAll" requests. Normally you don't need to change
        /// this value.
        /// </summary>
        public static int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (value < 1 || value > 1000)
                {
                    throw new ArgumentOutOfRangeException("PageSize must be between 1 and 1000!");
                }
                else
                {
                    _pageSize = value;
                }
            }
        }
    }
}
