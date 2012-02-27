using System.Linq;
using System.Collections.Generic;
using System;
using JetBrains.Annotations;

namespace BillomatNet.Data
{
    /// <summary>
    /// Represents the status of a Billomat offer
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public enum BillomatOfferStatus
    {
        Draft,
        Open,
        Won,
        Lost,
        Canceled
    }
}