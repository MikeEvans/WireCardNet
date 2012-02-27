using System.Linq;
using System.Collections.Generic;
using System;
using JetBrains.Annotations;

namespace BillomatNet.Data
{
    /// <summary>
    /// Represents the status of a Billomat invoice
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public enum BillomatInvoiceStatus
    {
        Draft,
        Open,
        Overdue,
        Paid,
        Canceled
    }
}