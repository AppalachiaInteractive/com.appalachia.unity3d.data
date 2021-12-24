using System;
using Appalachia.Core.Attributes;
using Appalachia.Data.Core.Collections;
using UnityEngine;

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
