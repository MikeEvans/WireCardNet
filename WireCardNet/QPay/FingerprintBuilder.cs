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
        private readonly bool autoAppendFingerprintOrder = true;
        private readonly NameValueCollection formValues = new NameValueCollection();
        private readonly List<string> order = new List<string>();
        private readonly StringBuilder seed = new StringBuilder();

        /// <summary>
        /// Creates a new Fingerprintbuilder
        /// </summary>
        /// <param name="secret">The customer secret</param>
        public FingerprintBuilder(string secret)
        {
            this.seed.Append(secret);
            this.order.Add("secret");
        }

        private FingerprintBuilder()
        {
            this.autoAppendFingerprintOrder = false;
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
            var itemArray = (items["responseFingerprintOrder"] ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var key in itemArray)
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

        protected string GetMD5(string seed)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(seed));
            return hash.Aggregate("", (current, b) => current + b.ToString("x02"));
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
                this.seed.Append(value);
                this.order.Add(name);
            }

            if (addToFormValues)
            {
                this.formValues.Add(name, value);
            }
        }

        /// <summary>
        /// Returns the fingerprint
        /// </summary>
        /// <returns></returns>
        public string GetFingerprint()
        {
            return GetMD5(this.seed + (this.autoAppendFingerprintOrder ? GetFingerprintOrder() : ""));
        }

        /// <summary>
        /// Returns a comma-separated list of form value keys
        /// </summary>
        /// <returns></returns>
        public string GetFingerprintOrder()
        {
            return string.Join(",", (this.autoAppendFingerprintOrder ? this.order.Concat(new[] { "requestFingerprintOrder" }) : this.order));
        }

        /// <summary>
        /// Returns all form values
        /// </summary>
        /// <returns></returns>
        public NameValueCollection GetFormValues()
        {
            return new NameValueCollection(this.formValues);
        }
    }
}