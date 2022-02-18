using System;
using System.Collections.Generic;
using System.Linq;
using Appalachia.Utility.Strings;

namespace LiteDB.Engine
{
    /// <summary>
    ///     Execute an "index scan" passing a Func as where
    /// </summary>
    internal class IndexScan : Index
    {
        public IndexScan(string name, Func<BsonValue, bool> func, int order) : base(name, order)
        {
            _func = func;
        }

        #region Fields and Autoproperties

        private Func<BsonValue, bool> _func;

        #endregion

        /// <inheritdoc />
        public override IEnumerable<IndexNode> Execute(IndexService indexer, CollectionIndex index)
        {
            return indexer.FindAll(index, Order).Where(i => _func(i.Key));
        }

        /// <inheritdoc />
        public override uint GetCost(CollectionIndex index)
        {
            return 80;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ZString.Format("FULL INDEX SCAN({0})", Name);
        }
    }
}
