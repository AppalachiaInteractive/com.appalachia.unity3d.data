using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Utility.Strings;

namespace UltraLiteDB
{
    internal class QueryOr : Query
    {
        public QueryOr(Query left, Query right) : base(null)
        {
            _left = left;
            _right = right;
        }

        #region Fields and Autoproperties

        private Query _left;
        private Query _right;

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return ZString.Format("({0} or {1})", _left, _right);
        }

        /// <inheritdoc />
        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        internal override bool FilterDocument(BsonDocument doc)
        {
            return _left.FilterDocument(doc) || _right.FilterDocument(doc);
        }

        /// <inheritdoc />
        internal override IEnumerable<IndexNode> Run(CollectionPage col, IndexService indexer)
        {
            var left = _left.Run(col, indexer);
            var right = _right.Run(col, indexer);

            // if any query (left/right) is FullScan, this query is full scan too
            UseIndex = _left.UseIndex && _right.UseIndex;
            UseFilter = _left.UseFilter || _right.UseFilter;

            return left.Union(right, new IndexNodeComparer());
        }
    }
}
