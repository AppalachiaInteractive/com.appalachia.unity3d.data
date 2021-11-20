using System;
using Appalachia.Core.Scriptables;
using Appalachia.Data.Core.Collections;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Appalachia.Data.Core.Documents
{
    [Serializable]
    public abstract class AppaDocument<TD, TC> : AppaDocumentBase
        where TD : AppaDocument<TD, TC>
    where TC : AppaCollection<TD, TC>
    {
        public void Save()
        {
        }
    }
}
