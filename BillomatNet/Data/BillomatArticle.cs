using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;

namespace BillomatNet.Data
{
    /// <summary>
    /// Represents a Billomat article
    /// </summary>
    [BillomatResource("articles", "article", "articles")]
    public class BillomatArticle : BillomatNumberedObject<BillomatArticle>
    {
        [BillomatField("article_number")]
        [BillomatReadOnly]
        public string ArticleNumber { get; internal set; }

        [BillomatField("title")]
        public string Title { get; set; }

        [BillomatField("description")]
        public string Description { get; set; }

        [BillomatField("sales_price")]
        public float SalesPrice { get; set; }

        [BillomatField("currency_code")]
        public string CurrencyCode { get; set; }

        [BillomatField("unit_id")]
        public int Unit { get; set; }

        /// <summary>
        /// Finds all articles that match the specified criteria
        /// </summary>
        /// <param name="articleNumber">search for article number</param>
        /// <param name="title">search for title</param>
        /// <param name="description">search for description</param>
        /// <param name="currencyCode">search for currency code</param>
        /// <param name="unitId">search for unit</param>
        /// <returns></returns>
        public static List<BillomatArticle> FindAll(string articleNumber = null, string title = null,
                                                    string description = null, string currencyCode = null, int? unitId = null)
        {
            var parameters = new NameValueCollection();

            if (!string.IsNullOrEmpty(articleNumber))
            {
                parameters.Add("article_number", articleNumber);
            }
            if (!string.IsNullOrEmpty(title))
            {
                parameters.Add("title", title);
            }
            if (!string.IsNullOrEmpty(description))
            {
                parameters.Add("description", description);
            }
            if (!string.IsNullOrEmpty(currencyCode))
            {
                parameters.Add("currency_code", currencyCode);
            }
            if (unitId.HasValue)
            {
                parameters.Add("unit_id", unitId.Value.ToString(CultureInfo.InvariantCulture));
            }

            return FindAll(parameters);
        }

        /// <summary>
        /// Returns a dictionary containing all articles: (article ID => article object)
        /// </summary>
        /// <returns></returns>
        public new static Dictionary<int, BillomatArticle> GetList()
        {
            return BillomatObject<BillomatArticle>.GetList();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1:C02})", Title, SalesPrice);
        }
    }
}