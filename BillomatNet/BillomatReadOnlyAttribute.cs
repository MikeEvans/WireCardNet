using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BillomatNet
{
    /// <summary>
    /// Marks a property as read-only, which means values are returned only by Billomat, but can
    /// not be saved and therefore will not be included in any XML request.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class BillomatReadOnlyAttribute : Attribute { }
}
