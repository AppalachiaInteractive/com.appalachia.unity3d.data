using System.Collections.Generic;
using System.Linq;
using Appalachia.Utility.Strings;

namespace LiteDB.Engine
{
    /// <summary>
    ///     Implement IN index operation. Value must be an array
    /// </summary>
    internal class IndexIn : Index
    {
        public IndexIn(string name, BsonArray values, int order) : base(name, order)
        {
            _values = values;
        }

        #region Fields and Autoproperties

        private BsonArray _values;

        #endregion

        /// <inheritdoc />
        public override IEnumerable<IndexNode> Execute(IndexService indexer, CollectionIndex index)
        {
            foreach (var value in _values.Distinct())
            {
                var idx = new IndexEquals(Name, value);

                foreach (var node in idx.Execute(indexer, index))
                {
                    yield return node;
                }
            }
        }

        /// <inheritdoc />
        public override uint GetCost(CollectionIndex index)
        {
            return index.Unique ? (uint)_values.Count * 1 : (uint)_values.Count * 10;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ZString.Format("INDEX SEEK({0} IN {1})", Name, JsonSerializer.Serialize(_values));
        }
    }
}
