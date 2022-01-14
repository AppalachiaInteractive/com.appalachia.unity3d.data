using System.Collections.Generic;
using Appalachia.Data.Core.Contracts;

namespace Appalachia.Data.Core.Collections
{
    public abstract class AppaCollectionBase<T> : DataObject<T>, IAppaCollection
        where T : AppaCollectionBase<T>
    {
        public abstract IReadOnlyList<IAppaDocument> BoxedDocuments { get; }
        
        public abstract string CollectionName { get; }
    }
}
