using System.Collections.Generic;
using System.Linq;
using Appalachia.Utility.Strings;

namespace UltraLiteDB
{
    /// <summary>
    ///     Not is an Index Scan operation
    /// </summary>
    internal class QueryNotEquals : Query
    {
        public QueryNotEquals(string field, BsonValue value) : base(field)
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
                "{0}({1} != {2})",
                UseFilter
                    ? "Filter"
                    : UseIndex
                        ? "Scan"
                        : "",
                Field,
                _value
            );
        }

        /// <inheritdoc />
        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            return indexer.FindAll(index, Ascending).Where(x => x.Key.CompareTo(_value) != 0);
        }

        /// <inheritdoc />
        internal override bool FilterDocument(BsonDocument doc)
        {
            return Expression.Execute(doc).Any(x => x.CompareTo(_value) != 0);
        }
    }
}
