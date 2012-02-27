using System.Linq;
using System.Collections.Generic;
using System;
using JetBrains.Annotations;

namespace BillomatNet.Data
{
    /// <summary>
    /// Transaction type for which a template was made
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public enum BillomatTemplateType
    {
        Invoice,
        Offer
    }
}