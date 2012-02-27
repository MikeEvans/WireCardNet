using System;
using System.Collections.Generic;
using System.Linq;

namespace BillomatNet.Data
{
    /// <summary>
    /// Base class for all Billomat objects that have a "Number" property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BillomatNumberedObject<T> : BillomatObject<T> where T : BillomatNumberedObject<T>, new()
    {
        [BillomatField("number")]
        public int? Number { get; set; }

        [BillomatField("number_pre")]
        public string NumberPrefix { get; set; }
    }
}