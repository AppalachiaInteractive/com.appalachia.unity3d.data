using System.Collections.Generic;
using System.Linq;
using Appalachia.Utility.Strings;

namespace UltraLiteDB
{
    internal class QueryBetween : Query
    {
        public QueryBetween(string field, BsonValue start, BsonValue end, bool startEquals, bool endEquals)
            : base(field)
        {
            _start = start;
            _startEquals = startEquals;
            _end = end;
            _endEquals = endEquals;
        }

        #region Fields and Autoproperties

        private bool _endEquals;

        private bool _startEquals;
        private BsonValue _end;
        private BsonValue _start;

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return ZString.Format(
                "{0}({1} between {2}{3} and {4}{5})",
                UseFilter
                    ? "Filter"
                    : UseIndex
                        ? "IndexSeek"
                        : "",
                Field,
                _startEquals ? "[" : "(",
                _start,
                _end,
                _endEquals ? "]" : ")"
            );
        }

        /// <inheritdoc />
        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            // define order
            var order = _start.CompareTo(_end) <= 0 ? Ascending : Descending;

            // find first indexNode
            var node = indexer.Find(index, _start, true, order);

            // returns (or not) equals start value
            while (node != null)
            {
                var diff = node.Key.CompareTo(_start);

                // if current value are not equals start, go out this loop
                if (diff != 0)
                {
                    break;
                }

                if (_startEquals)
                {
                    yield return node;
                }

                node = indexer.GetNode(node.NextPrev(0, order));
            }

            // navigate using next[0] do next node - if less or equals returns
            while (node != null)
            {
                var diff = node.Key.CompareTo(_end);

                if (_endEquals && (diff == 0))
                {
                    yield return node;
                }
                else if (diff == -order)
                {
                    yield return node;
                }
                else
                {
                    break;
                }

                node = indexer.GetNode(node.NextPrev(0, order));
            }
        }

        /// <inheritdoc />
        internal override bool FilterDocument(BsonDocument doc)
        {
            return Expression.Execute(doc, false)
                             .Any(
                                  x =>
                                  {
                                      return (_startEquals
                                                 ? x.CompareTo(_start) >= 0
                                                 : x.CompareTo(_start) > 0) &&
                                             (_endEquals ? x.CompareTo(_end) <= 0 : x.CompareTo(_end) < 0);
                                  }
                              );
        }
    }
}
