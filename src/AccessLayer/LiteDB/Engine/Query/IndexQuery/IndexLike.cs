using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Utility.Strings;

namespace LiteDB.Engine
{
    internal class IndexLike : Index
    {
        public IndexLike(string name, BsonValue value, int order) : base(name, order)
        {
            _pattern = value.AsString;
            _startsWith = _pattern.SqlLikeStartsWith(out _testSqlLike);
            _equals = _pattern == _startsWith;
        }

        #region Fields and Autoproperties

        private readonly bool _equals;
        private readonly bool _testSqlLike;
        private readonly string _pattern;
        private readonly string _startsWith;

        #endregion

        /// <inheritdoc />
        public override IEnumerable<IndexNode> Execute(IndexService indexer, CollectionIndex index)
        {
            // if contains startsWith string, search using index Find
            // otherwise, use index full scan and test results
            return _startsWith.Length > 0 ? ExecuteStartsWith(indexer, index) : ExecuteLike(indexer, index);
        }

        /// <inheritdoc />
        public override uint GetCost(CollectionIndex index)
        {
            if (_startsWith.Length > 0)
            {
                return 10; // similar to equals non-unique
            }

            return 100; // index full scan
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ZString.Format(
                "{0}({1} LIKE \"{2}\")",
                _startsWith.Length > 0 ? "INDEX SEEK (+RANGE SCAN)" : "FULL INDEX SCAN",
                Name,
                _pattern
            );
        }

        private IEnumerable<IndexNode> ExecuteLike(IndexService indexer, CollectionIndex index)
        {
            return indexer.FindAll(index, Order)
                          .Where(x => x.Key.IsString && x.Key.AsString.SqlLike(_pattern, indexer.Collation));
        }

        private IEnumerable<IndexNode> ExecuteStartsWith(IndexService indexer, CollectionIndex index)
        {
            // find first indexNode
            var first = indexer.Find(index, _startsWith, true, Order);
            var node = first;

            // if collection exists but are empty
            if (first == null)
            {
                yield break;
            }

            // first, go backward to get all same values
            while (node != null)
            {
                // if current node are edges exit while
                if (node.Key.IsMinValue || node.Key.IsMaxValue)
                {
                    break;
                }

                var valueString = node.Key.IsString
                    ? node.Key.AsString
                    : node.Key.IsNull
                        ? ""
                        : node.Key.ToString();

                if (_equals
                        ? valueString.Equals(_startsWith, StringComparison.OrdinalIgnoreCase)
                        : valueString.StartsWith(_startsWith, StringComparison.OrdinalIgnoreCase))
                {
                    // must still testing SqlLike method for rest of pattern - only if exists more to test (avoid slow SqlLike test)
                    if ((_testSqlLike == false) ||
                        (_testSqlLike && valueString.SqlLike(_pattern, indexer.Collation)))
                    {
                        yield return node;
                    }
                }
                else
                {
                    break;
                }

                node = indexer.GetNode(node.GetNextPrev(0, -Order));
            }

            // move forward
            node = indexer.GetNode(first.GetNextPrev(0, Order));

            while (node != null)
            {
                // if current node are edges exit while
                if (node.Key.IsMinValue || node.Key.IsMaxValue)
                {
                    break;
                }

                var valueString = node.Key.IsString
                    ? node.Key.AsString
                    : node.Key.IsNull
                        ? ""
                        : node.Key.ToString();

                if (_equals
                        ? valueString.Equals(_pattern, StringComparison.OrdinalIgnoreCase)
                        : valueString.StartsWith(_startsWith, StringComparison.OrdinalIgnoreCase))
                {
                    // must still testing SqlLike method for rest of pattern - only if exists more to test (avoid slow SqlLike test)
                    if ((node.DataBlock.IsEmpty == false) &&
                        ((_testSqlLike == false) ||
                         (_testSqlLike && valueString.SqlLike(_pattern, indexer.Collation))))
                    {
                        yield return node;
                    }
                }
                else
                {
                    break;
                }

                // first, go backward to get all same values
                node = indexer.GetNode(node.GetNextPrev(0, Order));
            }
        }
    }
}
