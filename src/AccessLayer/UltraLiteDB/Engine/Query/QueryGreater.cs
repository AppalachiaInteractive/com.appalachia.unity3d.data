using System.Collections.Generic;
using System.Linq;
using Appalachia.Utility.Strings;

namespace UltraLiteDB
{
    internal class QueryGreater : Query
    {
        public QueryGreater(string field, BsonValue value, bool equals) : base(field)
        {
            _value = value;
            _equals = equals;
        }

        #region Fields and Autoproperties

        private bool _equals;
        private BsonValue _value;

        #endregion

        public bool IsEquals => _equals;

        public BsonValue Value => _value;

        /// <inheritdoc />
        public override string ToString()
        {
            return ZString.Format(
                "{0}({1} >{2} {3})",
                UseFilter
                    ? "Filter"
                    : UseIndex
                        ? "Seek"
                        : "",
                Field,
                _equals ? "=" : "",
                _value
            );
        }

        /// <inheritdoc />
        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            // find first indexNode
            var node = indexer.Find(index, _value, true, Ascending);

            if (node == null)
            {
                yield break;
            }

            // move until next is last
            while (node != null)
            {
                // compares only with are same type
                if ((node.Key.Type == _value.Type) || (node.Key.IsNumber && _value.IsNumber))
                {
                    var diff = node.Key.CompareTo(_value);

                    if ((diff == 1) || (_equals && (diff == 0)))
                    {
                        if (node.IsHeadTail(index))
                        {
                            yield break;
                        }

                        yield return node;
                    }
                }

                node = indexer.GetNode(node.Next[0]);
            }
        }

        /// <inheritdoc />
        internal override bool FilterDocument(BsonDocument doc)
        {
            return Expression.Execute(doc)
                             .Where(x => (x.Type == _value.Type) || (x.IsNumber && _value.IsNumber))
                             .Any(x => x.CompareTo(_value) >= (_equals ? 0 : 1));
        }
    }
}
