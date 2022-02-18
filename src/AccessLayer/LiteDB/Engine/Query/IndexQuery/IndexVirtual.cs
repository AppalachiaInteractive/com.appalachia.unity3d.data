using System;
using System.Collections.Generic;
using static LiteDB.Constants;

namespace LiteDB.Engine
{
    /// <summary>
    ///     Implement virtual index for system collections AND full data collection read
    /// </summary>
    internal class IndexVirtual : Index, IDocumentLookup
    {
        public IndexVirtual(IEnumerable<BsonDocument> source) : base(null, 0)
        {
            _source = source;
        }

        #region Fields and Autoproperties

        private readonly IEnumerable<BsonDocument> _source;

        private Dictionary<uint, BsonDocument> _cache = new Dictionary<uint, BsonDocument>();

        #endregion

        /// <inheritdoc />
        public override IEnumerable<IndexNode> Execute(IndexService indexer, CollectionIndex index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override uint GetCost(CollectionIndex index)
        {
            return 100; // virtual index is always full scan
        }

        /// <inheritdoc />
        public override IEnumerable<IndexNode> Run(CollectionPage col, IndexService indexer)
        {
            var rawId = 0u;

            foreach (var doc in _source)
            {
                rawId++;

                // create fake rawId for document source
                doc.RawId = new PageAddress(rawId, 0);

                // create cache until reach 1000 document - after this, delete cache and remove support
                if (_cache != null)
                {
                    _cache[rawId] = doc;

                    if (_cache.Count > VIRTUAL_INDEX_MAX_CACHE)
                    {
                        _cache = null;
                    }
                }

                // return an fake indexNode
                yield return new IndexNode(doc);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "FULL COLLECTION SCAN";
        }

        #region IDocumentLookup Members

        public BsonDocument Load(IndexNode node)
        {
            return node.Key as BsonDocument;
        }

        public BsonDocument Load(PageAddress rawId)
        {
            if (_cache == null)
            {
                throw new LiteException(
                    0,
                    string.Format(
                        "OrderBy/GroupBy operation are supported only in virtual collection with less than {0} documents",
                        VIRTUAL_INDEX_MAX_CACHE
                    )
                );
            }

            return _cache[rawId.PageID];
        }

        #endregion
    }
}
