using System.Collections.Generic;
using Appalachia.Utility.Strings;

namespace UltraLiteDB
{
    /// <summary>
    /// All is an Index Scan operation
    /// </summary>
    internal class QueryAll : Query
    {
        private int _order;

        public QueryAll(string field, int order)
            : base(field)
        {
            _order = order;
        }

        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            return indexer.FindAll(index, _order);
        }

        internal override bool FilterDocument(BsonDocument doc)
        {
            return true;
        }

        public override string ToString()
        {
            return ZString.Format(
                "{0}({1})",
                this.UseFilter ? "Filter" : this.UseIndex ? "Scan" : "",
                this.Field);
        }
    }
}