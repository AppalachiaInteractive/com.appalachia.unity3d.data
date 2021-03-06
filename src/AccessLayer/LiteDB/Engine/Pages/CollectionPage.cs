using System.Collections.Generic;
using System.Linq;
using static LiteDB.Constants;

namespace LiteDB.Engine
{
    internal class CollectionPage : BasePage
    {
        public CollectionPage(PageBuffer buffer, uint pageID) : base(buffer, pageID, PageType.Collection)
        {
            for (var i = 0; i < PAGE_FREE_LIST_SLOTS; i++)
            {
                FreeDataPageList[i] = uint.MaxValue;
            }
        }

        public CollectionPage(PageBuffer buffer) : base(buffer)
        {
            ENSURE(PageType == PageType.Collection, "page type must be collection page");

            if (PageType != PageType.Collection)
            {
                throw LiteException.InvalidPageType(PageType.Collection, this);
            }

            // create new buffer area to store BsonDocument indexes
            var area = _buffer.Slice(PAGE_HEADER_SIZE, PAGE_SIZE - PAGE_HEADER_SIZE);

            using (var r = new BufferReader(new[] { area }))
            {
                // read position for FreeDataPage and FreeIndexPage
                for (var i = 0; i < PAGE_FREE_LIST_SLOTS; i++)
                {
                    FreeDataPageList[i] = r.ReadUInt32();
                }

                // skip reserved area
                r.Skip(P_INDEXES - PAGE_HEADER_SIZE - r.Position);

                // read indexes count (max 255 indexes per collection)
                var count = r.ReadByte(); // 1 byte

                for (var i = 0; i < count; i++)
                {
                    var index = new CollectionIndex(r);

                    _indexes[index.Name] = index;
                }
            }
        }

        #region Fields and Autoproperties

        /// <summary>
        ///     Free data page linked-list (N lists for different range of FreeBlocks)
        /// </summary>
        public uint[] FreeDataPageList { get; } = new uint[PAGE_FREE_LIST_SLOTS];

        /// <summary>
        ///     All indexes references for this collection
        /// </summary>
        private readonly Dictionary<string, CollectionIndex> _indexes =
            new Dictionary<string, CollectionIndex>();

        #endregion

        /// <summary>
        ///     Get PK index
        /// </summary>
        public CollectionIndex PK => _indexes["_id"];

        /// <inheritdoc />
        public override PageBuffer UpdateBuffer()
        {
            // if page was deleted, do not write in content area (must keep with 0 only)
            if (PageType == PageType.Empty)
            {
                return base.UpdateBuffer();
            }

            var area = _buffer.Slice(PAGE_HEADER_SIZE, PAGE_SIZE - PAGE_HEADER_SIZE);

            using (var w = new BufferWriter(area))
            {
                // read position for FreeDataPage and FreeIndexPage
                for (var i = 0; i < PAGE_FREE_LIST_SLOTS; i++)
                {
                    w.Write(FreeDataPageList[i]);
                }

                // skip reserved area (indexes starts at position 96)
                w.Skip(P_INDEXES - PAGE_HEADER_SIZE - w.Position);

                w.Write((byte)_indexes.Count); // 1 byte

                foreach (var index in _indexes.Values)
                {
                    index.UpdateBuffer(w);
                }
            }

            return base.UpdateBuffer();
        }

        /// <summary>
        ///     Remove index reference in this page
        /// </summary>
        public void DeleteCollectionIndex(string name)
        {
            _indexes.Remove(name);

            IsDirty = true;
        }

        /// <summary>
        ///     Get index from index name (index name is case sensitive) - returns null if not found
        /// </summary>
        public CollectionIndex GetCollectionIndex(string name)
        {
            if (_indexes.TryGetValue(name, out var index))
            {
                return index;
            }

            return null;
        }

        /// <summary>
        ///     Get all indexes in this collection page
        /// </summary>
        public ICollection<CollectionIndex> GetCollectionIndexes()
        {
            return _indexes.Values;
        }

        /// <summary>
        ///     Get all collections array based on slot number
        /// </summary>
        public CollectionIndex[] GetCollectionIndexesSlots()
        {
            var indexes = new CollectionIndex[_indexes.Max(x => x.Value.Slot) + 1];

            foreach (var index in _indexes.Values)
            {
                indexes[index.Slot] = index;
            }

            return indexes;
        }

        /// <summary>
        ///     Insert new index inside this collection page
        /// </summary>
        public CollectionIndex InsertCollectionIndex(string name, string expr, bool unique)
        {
            var totalLength = 1 +
                              _indexes.Sum(x => CollectionIndex.GetLength(x.Value)) +
                              CollectionIndex.GetLength(name, expr);

            // check if has space avaiable
            if ((_indexes.Count == 255) || (totalLength >= P_INDEXES_COUNT))
            {
                throw new LiteException(0, "This collection has no more space for new indexes");
            }

            var slot = (byte)(_indexes.Count == 0 ? 0 : _indexes.Max(x => x.Value.Slot) + 1);

            var index = new CollectionIndex(slot, 0, name, expr, unique);

            _indexes[name] = index;

            IsDirty = true;

            return index;
        }

        /// <summary>
        ///     Return index instance and mark as updatable
        /// </summary>
        public CollectionIndex UpdateCollectionIndex(string name)
        {
            IsDirty = true;

            return _indexes[name];
        }

        #region Buffer Field Positions

        private const int P_INDEXES = 96;                          // 96-8192 (64 + 32 header = 96)
        private const int P_INDEXES_COUNT = PAGE_SIZE - P_INDEXES; // 8096

        #endregion
    }
}
