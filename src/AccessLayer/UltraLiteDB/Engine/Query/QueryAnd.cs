using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Utility.Strings;

namespace UltraLiteDB
{
    internal class QueryAnd : Query
    {
        public QueryAnd(Query left, Query right) : base(null)
        {
            _left = left;
            _right = right;
        }

        #region Fields and Autoproperties

        private Query _left;
        private Query _right;

        #endregion

        /// <inheritdoc />
        internal override bool UseFilter
        {
            get =>

                // return true if any site use filter
                _left.UseFilter || _right.UseFilter;
            set
            {
                // set both sides with value
                _left.UseFilter = value;
                _right.UseFilter = value;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ZString.Format("({0} and {1})", _left, _right);
        }

        /// <inheritdoc />
        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        internal override bool FilterDocument(BsonDocument doc)
        {
            return _left.FilterDocument(doc) && _right.FilterDocument(doc);
        }

        /// <inheritdoc />
        internal override IEnumerable<IndexNode> Run(CollectionPage col, IndexService indexer)
        {
            // execute both run operation but not fetch any node yet
            var left = _left.Run(col, indexer);
            var right = _right.Run(col, indexer);

            // if left use index, force right use full scan (left has preference to use index)
            if (_left.UseIndex)
            {
                UseIndex = true;
                _right.UseFilter = true;
                return left;
            }

            // if right use index (and left no), force left use filter
            if (_right.UseIndex)
            {
                UseIndex = true;
                _left.UseFilter = true;
                return right;
            }

            // neither left and right uses index (both are full scan)
            UseIndex = false;
            UseFilter = true;

            return left.Intersect(right, new IndexNodeComparer());
        }
    }
}
