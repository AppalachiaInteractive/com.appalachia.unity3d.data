using System.Collections.Generic;
using Appalachia.Utility.Strings;

namespace UltraLiteDB
{
    /// <summary>
    ///     All is an Index Scan operation
    /// </summary>
    internal class QueryAll : Query
    {
        public QueryAll(string field, int order) : base(field)
        {
            _order = order;
        }

        #region Fields and Autoproperties

        private int _order;

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return ZString.Format(
                "{0}({1})",
                UseFilter
                    ? "Filter"
                    : UseIndex
                        ? "Scan"
                        : "",
                Field
            );
        }

        /// <inheritdoc />
        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            return indexer.FindAll(index, _order);
        }

        /// <inheritdoc />
        internal override bool FilterDocument(BsonDocument doc)
        {
            return true;
        }
    }
}
