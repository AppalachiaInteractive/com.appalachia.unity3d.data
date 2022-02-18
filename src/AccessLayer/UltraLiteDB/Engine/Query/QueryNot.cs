using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Utility.Strings;

namespace UltraLiteDB
{
    /// <summary>
    ///     Not is an Index Scan operation
    /// </summary>
    internal class QueryNot : Query
    {
        public QueryNot(Query query, int order) : base("_id")
        {
            _query = query;
            _order = order;
        }

        #region Fields and Autoproperties

        private int _order;
        private Query _query;

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return ZString.Format("!({0})", _query);
        }

        /// <inheritdoc />
        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        internal override bool FilterDocument(BsonDocument doc)
        {
            return !_query.FilterDocument(doc);
        }

        /// <inheritdoc />
        internal override IEnumerable<IndexNode> Run(CollectionPage col, IndexService indexer)
        {
            // run base query
            var result = _query.Run(col, indexer);

            UseIndex = _query.UseIndex;
            UseFilter = _query.UseFilter;

            if (_query.UseIndex)
            {
                // if is by index, resolve here
                var all = new QueryAll("_id", _order).Run(col, indexer);

                return all.Except(result, new IndexNodeComparer());
            }

            // if is by document, must return all nodes to be ExecuteDocument after
            return result;
        }
    }
}
