using System.Collections.Generic;

namespace UltraLiteDB
{
    /// <summary>
    ///     Placeholder query for returning no values from a collection.
    /// </summary>
    internal class QueryEmpty : Query
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="QueryEmpty" /> class.
        /// </summary>
        public QueryEmpty() : base(null)
        {
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "(false)";
        }

        /// <inheritdoc />
        internal override IEnumerable<IndexNode> ExecuteIndex(IndexService indexer, CollectionIndex index)
        {
            yield break;
        }

        /// <inheritdoc />
        internal override bool FilterDocument(BsonDocument doc)
        {
            return false;
        }
    }
}
