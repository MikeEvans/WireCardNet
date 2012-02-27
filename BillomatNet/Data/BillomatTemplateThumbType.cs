using System.Linq;
using System.Collections.Generic;
using System;
using JetBrains.Annotations;

namespace BillomatNet.Data
{
    /// <summary>
    /// Image format of a template's thumbnail
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public enum BillomatTemplateThumbType
    {
        Png,
        Gif,
        Jpg
    }
}