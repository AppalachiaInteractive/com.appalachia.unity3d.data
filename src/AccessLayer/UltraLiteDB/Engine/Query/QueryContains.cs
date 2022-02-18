using System.Collections.Generic;
using System.Linq;
using Appalachia.Utility.Strings;

namespace UltraLiteDB
{
    /// <summary>
    ///     Contains query do not work with index, only full scan
    /// </summary>
    internal class QueryContains : Query
    {
        public QueryContains(string field, BsonValue value) : base(field)
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
                "{0}({1} contains {2})",
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
            return indexer.FindAll(index, Ascending)
                          .Where(x => x.Key.IsString && x.Key.AsString.Contains(_value));
        }

        /// <inheritdoc />
        internal override bool FilterDocument(BsonDocument doc)
        {
            return Expression.Execute(doc, false)
                             .Where(x => x.IsString)
                             .Any(x => x.AsString.Contains(_value));
        }
    }
}
