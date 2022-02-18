using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Utility.Strings;

namespace UltraLiteDB
{
    /// <summary>
    ///     Execute an index scan passing a Func as where
    /// </summary>
    internal class QueryWhere : Query
    {
        public QueryWhere(string field, Func<BsonValue, bool> func, int order) : base(field)
        {
            _func = func;
            _order = order;
        }

        #region Fields and Autoproperties

        private Func<BsonValue, bool> _func;
        private int _order;

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return ZString.Format(
                "{0}({1}[{2}])",
                UseFilter
                    ? "Filter"
                    : UseIndex
                        ? "Scan"
                        : "",
                _func,
                Field
            );
        }

        /// <inheritdoc />
        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            return indexer.FindAll(index, _order).Where(i => _func(i.Key));
        }

        /// <inheritdoc />
        internal override bool FilterDocument(BsonDocument doc)
        {
            return Expression.Execute(doc).Any(x => _func(x));
        }
    }
}
