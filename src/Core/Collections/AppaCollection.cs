using System;
using System.Collections.Generic;
using Appalachia.Core.Attributes;
using Appalachia.Data.Core.Documents;
using Appalachia.Utility.Logging;
using Appalachia.Utility.Reflection.Extensions;
using UnityEngine;

namespace Appalachia.Data.Core.Collections
{
    [Serializable]
    public abstract class AppaCollection<TD, TC> : AppaCollectionBase
        where TD : AppaDocument<TD, TC>
        where TC : AppaCollection<TD, TC>
    {
        #region Fields and Autoproperties

        public override string CollectionName { get; } = typeof(TD).GetSimpleReadableName();

        [SerializeField] private List<TD> _documents;

        #endregion

        public override IReadOnlyList<AppaDocumentBase> BoxedDocuments => _documents;

        public IReadOnlyList<TD> Documents => _documents;
    }
}
