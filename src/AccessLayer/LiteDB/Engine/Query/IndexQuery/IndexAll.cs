using System.Collections.Generic;
using Appalachia.Utility.Strings;

namespace LiteDB.Engine
{
    /// <summary>
    ///     Return all index nodes
    /// </summary>
    internal class IndexAll : Index
    {
        public IndexAll(string name, int order) : base(name, order)
        {
        }

        /// <inheritdoc />
        public override IEnumerable<IndexNode> Execute(IndexService indexer, CollectionIndex index)
        {
            return indexer.FindAll(index, Order);
        }

        /// <inheritdoc />
        public override uint GetCost(CollectionIndex index)
        {
            return 100; // worst index cost
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ZString.Format("FULL INDEX SCAN({0})", Name);
        }
    }
}
