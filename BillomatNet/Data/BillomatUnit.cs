using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BillomatNet.Data
{
    /// <summary>
    /// Represents a Billomat unit for an article
    /// </summary>
    [BillomatResource("units", "unit", "units")]
    public class BillomatUnit : BillomatObject<BillomatUnit>
    {
        /// <summary>
        /// Finds all units
        /// </summary>
        /// <returns></returns>
        public static List<BillomatUnit> FindAll()
        {
            return FindAll(null);
        }

        /// <summary>
        /// Returns a dictionary containing all units: (unit ID => unit object)
        /// </summary>
        /// <returns></returns>
        public new static Dictionary<int, BillomatUnit> GetList()
        {
            return BillomatObject<BillomatUnit>.GetList();
        }

        [BillomatField("name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
