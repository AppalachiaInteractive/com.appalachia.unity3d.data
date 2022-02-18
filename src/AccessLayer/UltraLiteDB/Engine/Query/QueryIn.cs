using System.Collections.Generic;
using System.Linq;
using Appalachia.Utility.Strings;

namespace UltraLiteDB
{
    internal class QueryIn : Query
    {
        public QueryIn(string field, IEnumerable<BsonValue> values) : base(field)
        {
            _values = values ?? Enumerable.Empty<BsonValue>();
        }

        #region Fields and Autoproperties

        private IEnumerable<BsonValue> _values;

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return ZString.Format(
                "{0}({1} in {2})",
                UseFilter
                    ? "Filter"
                    : UseIndex
                        ? "Seek"
                        : "",
                Field,
                string.Join(",", _values.Select(a => a != null ? a.ToString() : "Null").ToArray())
            );
        }

        /// <inheritdoc />
        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            foreach (var value in _values.Distinct())
            {
                foreach (var node in EQ(Field, value).ExecuteIndex(indexer, index))
                {
                    yield return node;
                }
            }
        }

        /// <inheritdoc />
        internal override bool FilterDocument(BsonDocument doc)
        {
            foreach (var val in Expression.Execute(doc))
            {
                foreach (var value in _values.Distinct())
                {
                    var diff = val.CompareTo(value);

                    if (diff == 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
