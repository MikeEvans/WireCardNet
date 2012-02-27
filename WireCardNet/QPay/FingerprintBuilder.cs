using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WireCardNet.QPay
{
    internal class FingerprintBuilder
    {
        private readonly bool _autoAppendFingerprintOrder = true;
        private readonly NameValueCollection _formValues = new NameValueCollection();
        private readonly List<string> _order = new List<string>();
        private readonly StringBuilder _seed = new StringBuilder();

        /// <summary>
        /// Creates a new Fingerprintbuilder
        /// </summary>
        /// <param name="secret">The customer secret</param>
        public FingerprintBuilder(string secret)
        {
            _seed.Append(secret);
            _order.Add("secret");
        }

        private FingerprintBuilder()
        {
            _autoAppendFingerprintOrder = false;
        }

        /// <summary>
        /// Verifies the fingerprint returned by WireCard
        /// </summary>
        /// <param name="secret">The customer secret key</param>
        /// <param name="items">The form values</param>
        /// <returns></returns>
        public static bool VerifyFingerprint(string secret, NameValueCollection items)
        {
            var builder = new FingerprintBuilder();

            string[] itemArray = (items["responseFingerprintOrder"] ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string key in itemArray)
            {
                if (key == "secret")
                {
                    builder.AddValue("secret", secret);
                }
                else
                {
                    builder.AddValue(key, items[key]);
                }
            }

            if (builder.GetFingerprintOrder() != items["responseFingerprintOrder"])
            {
                throw new WireCardException("Fingerprint could not be checked!");
            }

            string hash = builder.GetFingerprint();

            return hash.Equals(items["responseFingerprint"], StringComparison.OrdinalIgnoreCase);
        }

        protected string GetMD5(string seed)
        {
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(seed));
            string hashString = "";

            foreach (byte b in hash)
            {
                hashString += b.ToString("x02");
            }

            return hashString;
        }

        /// <summary>
        /// Adds a value to be included in the fingerprint
        /// </summary>
        /// <param name="name">Key of the value</param>
        /// <param name="value">The value</param>
        /// <param name="addToSeed">True to add the value to the hash seed</param>
        /// <param name="addToFormValues">True to add the value to the form values returned by GetFormValues</param>
        public void AddValue(string name, string value, bool addToSeed = true, bool addToFormValues = true)
        {
            if (addToSeed)
            {
                _seed.Append(value);
                _order.Add(name);
            }

            if (addToFormValues)
            {
                _formValues.Add(name, value);
            }
        }

        /// <summary>
        /// Returns the fingerprint
        /// </summary>
        /// <returns></returns>
        public string GetFingerprint()
        {
            return GetMD5(_seed + (_autoAppendFingerprintOrder ? GetFingerprintOrder() : ""));
        }

        /// <summary>
        /// Returns a comma-separated list of form value keys
        /// </summary>
        /// <returns></returns>
        public string GetFingerprintOrder()
        {
            return string.Join(",", (_autoAppendFingerprintOrder ? _order.Concat(new[] { "requestFingerprintOrder" }) : _order));
        }

        /// <summary>
        /// Returns all form values
        /// </summary>
        /// <returns></returns>
        public NameValueCollection GetFormValues()
        {
            return new NameValueCollection(_formValues);
        }
    }
}