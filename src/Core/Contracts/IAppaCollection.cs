using System.Collections.Generic;

namespace Appalachia.Data.Core.Contracts
{
    public interface IAppaCollection : IDataObject
    {
        string CollectionName { get; }

        IReadOnlyList<IAppaDocument> BoxedDocuments { get; }

        public void MarkAsModified();
    }
}
