using System.Linq;
using System.Collections.Generic;
using System;
using JetBrains.Annotations;

namespace BillomatNet.Data
{
    /// <summary>
    /// File format of a template
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public enum BillomatTemplateFormat
    {
        Doc,
        Docx,
        Rtf
    }
}