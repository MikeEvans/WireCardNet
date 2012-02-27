using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Security.Cryptography;

namespace WireCardNet.QPay
{
    internal class FingerprintBuilder
    {
        /// <summary>
        /// Verifies the fingerprint returned by WireCard
        /// </summary>
        /// <param name="secret">The customer secret key</param>
        /// <param name="items">The form values</param>
        /// <returns></returns>
        public static bool VerifyFingerprint(string secret, NameValueCollection items)
        {
            var builder = new FingerprintBuilder();

            string[] itemArray = (items["responseFingerprintOrder"] ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

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

            var hash = builder.GetFingerprint();

            return hash.Equals(items["responseFingerprint"], StringComparison.OrdinalIgnoreCase);
        }

        private StringBuilder seed = new StringBuilder();
        private List<string> order = new List<string>();
        private NameValueCollection formValues = new NameValueCollection();

        private bool autoAppendFingerprintOrder = true;

        /// <summary>
        /// Creates a new Fingerprintbuilder
        /// </summary>
        /// <param name="secret">The customer secret</param>
        public FingerprintBuilder(string secret)
        {
            seed.Append(secret);
            order.Add("secret");
        }

        private FingerprintBuilder()
        {
            autoAppendFingerprintOrder = false;
        }

        protected string GetMD5(string seed)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(seed));
            var hashString = "";

            foreach (var b in hash)
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
                seed.Append(value);
                order.Add(name);
            }

            if (addToFormValues)
            {
                formValues.Add(name, value);
            }
        }

        /// <summary>
        /// Returns the fingerprint
        /// </summary>
        /// <returns></returns>
        public string GetFingerprint()
        {
            return GetMD5(seed.ToString() + (autoAppendFingerprintOrder ? GetFingerprintOrder() : ""));
        }

        /// <summary>
        /// Returns a comma-separated list of form value keys
        /// </summary>
        /// <returns></returns>
        public string GetFingerprintOrder()
        {
            return string.Join(",", (autoAppendFingerprintOrder ? order.Concat(new string[] { "requestFingerprintOrder" }) : order));
        }

        /// <summary>
        /// Returns all form values
        /// </summary>
        /// <returns></returns>
        public NameValueCollection GetFormValues()
        {
            return new NameValueCollection(formValues);
        }
    }
}
