using System;
using Appalachia.Data.Core.Collections;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Appalachia.Data.Core.Documents
{
    [Serializable]
    public abstract class AppaDocument<TD, TC> : AppaDocumentBase<TD>
        where TD : AppaDocument<TD, TC>
        where TC : AppaCollection<TD, TC>
    {
        public void Save()
        {
        }
    }
}
