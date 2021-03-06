using System.Collections.Generic;
using System.Linq;
using Appalachia.Utility.Strings;

namespace UltraLiteDB
{
    internal class QueryStartsWith : Query
    {
        public QueryStartsWith(string field, BsonValue value) : base(field)
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
                "{0}({1} startsWith {2})",
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
            // find first indexNode
            var node = indexer.Find(index, _value, true, Ascending);
            var str = _value.AsString;

            // navigate using next[0] do next node - if less or equals returns
            while (node != null)
            {
                var valueString = node.Key.AsString;

                // value will not be null because null occurs before string (bsontype sort order)
                if (valueString.StartsWith(str))
                {
                    if (!node.DataBlock.IsEmpty)
                    {
                        yield return node;
                    }
                }
                else
                {
                    break; // if no more starts with, stop scanning
                }

                node = indexer.GetNode(node.Next[0]);
            }
        }

        /// <inheritdoc />
        internal override bool FilterDocument(BsonDocument doc)
        {
            return Expression.Execute(doc, false)
                             .Where(x => x.IsString)
                             .Any(x => x.AsString.StartsWith(_value));
        }
    }
}
