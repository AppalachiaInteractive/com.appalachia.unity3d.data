using System.Collections.Generic;
using Appalachia.Data.Core.Documents;

namespace Appalachia.Data.Core.Collections
{
    public abstract class AppaCollectionBase : DataObject
    {
        public abstract IReadOnlyList<AppaDocumentBase> BoxedDocuments { get; }

        public abstract string CollectionName { get; }
    }
}
