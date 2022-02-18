using System.Collections.Generic;
using System.Linq;
using Appalachia.Utility.Strings;

namespace UltraLiteDB
{
    internal class QueryEquals : Query
    {
        public QueryEquals(string field, BsonValue value) : base(field)
        {
            _value = value;
        }

        #region Fields and Autoproperties

        private BsonValue _value;

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return ZString.Format(
                "{0}({1} = {2})",
                UseFilter
                    ? "Filter"
                    : UseIndex
                        ? "Seek"
                        : "",
                Field,
                _value
            );
        }

        /// <inheritdoc />
        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            var node = indexer.Find(index, _value, false, Ascending);

            if (node == null)
            {
                yield break;
            }

            yield return node;

            if (index.Unique == false)
            {
                // navigate using next[0] do next node - if equals, returns
                while (!node.Next[0].IsEmpty &&
                       ((node = indexer.GetNode(node.Next[0])).Key.CompareTo(_value) == 0))
                {
                    if (node.IsHeadTail(index))
                    {
                        yield break;
                    }

                    yield return node;
                }
            }
        }

        /// <inheritdoc />
        internal override bool FilterDocument(BsonDocument doc)
        {
            return Expression.Execute(doc).Any(x => x.CompareTo(_value) == 0);
        }
    }
}
