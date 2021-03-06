using Appalachia.Utility.Strings;
using static LiteDB.Constants;

namespace LiteDB.Engine
{
    /// <summary>
    ///     Represent a index node inside a Index Page
    /// </summary>
    internal class IndexNode
    {
        #region Constants and Static Readonly

        /// <summary>
        ///     Fixed length of IndexNode (12 bytes)
        /// </summary>
        private const int INDEX_NODE_FIXED_SIZE = 1 +                // Slot [1 byte]
                                                  1 +                // Levels [1 byte]
                                                  PageAddress.SIZE + // DataBlock (5 bytes)
                                                  PageAddress.SIZE;  // NextNode (5 bytes)

        private const int P_DATA_BLOCK = 2; // 02-06 [PageAddress]
        private const int P_LEVEL = 1;      // 01-01 [byte]
        private const int P_NEXT_NODE = 7;  // 07-11 [PageAddress]
        private const int P_PREV_NEXT = 12; // 12-(_level * 5 [PageAddress] * 2 [prev-next])

        private const int P_SLOT = 0; // 00-00 [byte]

        #endregion

        /// <summary>
        ///     Read index node from page segment (lazy-load)
        /// </summary>
        public IndexNode(IndexPage page, byte index, BufferSlice segment)
        {
            _page = page;
            _segment = segment;

            Position = new PageAddress(page.PageID, index);
            Slot = segment.ReadByte(P_SLOT);
            Level = segment.ReadByte(P_LEVEL);
            DataBlock = segment.ReadPageAddress(P_DATA_BLOCK);
            NextNode = segment.ReadPageAddress(P_NEXT_NODE);

            Next = new PageAddress[Level];
            Prev = new PageAddress[Level];

            for (var i = 0; i < Level; i++)
            {
                Prev[i] = segment.ReadPageAddress(P_PREV_NEXT + (i * PageAddress.SIZE * 2));
                Next[i] = segment.ReadPageAddress(
                    P_PREV_NEXT + (i * PageAddress.SIZE * 2) + PageAddress.SIZE
                );
            }

            Key = segment.ReadIndexKey(P_KEY);
        }

        /// <summary>
        ///     Create new index node and persist into page segment
        /// </summary>
        public IndexNode(
            IndexPage page,
            byte index,
            BufferSlice segment,
            byte slot,
            byte level,
            BsonValue key,
            PageAddress dataBlock)
        {
            _page = page;
            _segment = segment;

            Position = new PageAddress(page.PageID, index);
            Slot = slot;
            Level = level;
            DataBlock = dataBlock;
            NextNode = PageAddress.Empty;
            Next = new PageAddress[level];
            Prev = new PageAddress[level];
            Key = key;

            // persist in buffer read only data
            segment.Write(slot,      P_SLOT);
            segment.Write(level,     P_LEVEL);
            segment.Write(dataBlock, P_DATA_BLOCK);
            segment.Write(NextNode,  P_NEXT_NODE);

            for (var i = 0; i < level; i++)
            {
                SetPrev((byte)i, PageAddress.Empty);
                SetNext((byte)i, PageAddress.Empty);
            }

            segment.WriteIndexKey(key, P_KEY);

            page.IsDirty = true;
        }

        /// <summary>
        ///     Create a fake index node used only in Virtual Index runner
        /// </summary>
        public IndexNode(BsonDocument doc)
        {
            _page = null;
            _segment = new BufferSlice(new byte[0], 0, 0);

            Position = new PageAddress(0, 0);
            Slot = 0;
            Level = 0;
            DataBlock = PageAddress.Empty;
            NextNode = PageAddress.Empty;
            Next = new PageAddress[0];
            Prev = new PageAddress[0];

            // index node key IS document
            Key = doc;
        }

        #region Fields and Autoproperties

        /// <summary>
        ///     The object value that was indexed (max 255 bytes value)
        /// </summary>
        public BsonValue Key { get; }

        /// <summary>
        ///     Skip-list level (0-31) - [1 byte]
        /// </summary>
        public byte Level { get; }

        /// <summary>
        ///     Index slot reference in CollectionIndex [1 byte]
        /// </summary>
        public byte Slot { get; }

        /// <summary>
        ///     Reference for a datablock address
        /// </summary>
        public PageAddress DataBlock { get; }

        /// <summary>
        ///     Position of this node inside a IndexPage (not persist)
        /// </summary>
        public PageAddress Position { get; }

        /// <summary>
        ///     Single linked-list for all nodes from a single document [5 bytes]
        /// </summary>
        public PageAddress NextNode { get; private set; }

        /// <summary>
        ///     Link to next value (used in skip lists - Prev.Length = Next.Length)
        /// </summary>
        public PageAddress[] Next { get; private set; }

        /// <summary>
        ///     Link to prev value (used in skip lists - Prev.Length = Next.Length) [5 bytes]
        /// </summary>
        public PageAddress[] Prev { get; private set; }

        private readonly BufferSlice _segment;

        private readonly IndexPage _page;

        #endregion

        /// <summary>
        ///     Get index page reference
        /// </summary>
        public IndexPage Page => _page;

        private int P_KEY => P_PREV_NEXT + (Level * PageAddress.SIZE * 2); // just after NEXT

        /// <summary>
        ///     Get how many bytes will be used to store this value. Must consider:
        ///     [1 byte] - BsonType
        ///     [1 byte] - KeyLength (used only in String|Byte[])
        ///     [N bytes] - BsonValue in bytes (0-254)
        /// </summary>
        public static int GetKeyLength(BsonValue key, bool recalc)
        {
            return 1 + (key.IsString || key.IsBinary ? 1 : 0) + key.GetBytesCount(recalc);
        }

        /// <summary>
        ///     Calculate how many bytes this node will need on page segment
        /// </summary>
        public static int GetNodeLength(byte level, BsonValue key, out int keyLength)
        {
            keyLength = GetKeyLength(key, true);

            return INDEX_NODE_FIXED_SIZE +
                   (level * 2 * PageAddress.SIZE) + // prev/next
                   keyLength;                       // key
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ZString.Format("Pos: [{0}] - Key: {1}", Position, Key);
        }

        /// <summary>
        ///     Returns Next (order == 1) OR Prev (order == -1)
        /// </summary>
        public PageAddress GetNextPrev(byte level, int order)
        {
            return order == Query.Ascending ? Next[level] : Prev[level];
        }

        /// <summary>
        ///     Update Next[index] pointer (update in buffer too). Also, set page as dirty
        /// </summary>
        public void SetNext(byte level, PageAddress value)
        {
            ENSURE(level <= Level, "out of index in level");

            Next[level] = value;

            _segment.Write(value, P_PREV_NEXT + (level * PageAddress.SIZE * 2) + PageAddress.SIZE);

            _page.IsDirty = true;
        }

        /// <summary>
        ///     Update NextNode pointer (update in buffer too). Also, set page as dirty
        /// </summary>
        public void SetNextNode(PageAddress value)
        {
            NextNode = value;

            _segment.Write(value, P_NEXT_NODE);

            _page.IsDirty = true;
        }

        /// <summary>
        ///     Update Prev[index] pointer (update in buffer too). Also, set page as dirty
        /// </summary>
        public void SetPrev(byte level, PageAddress value)
        {
            ENSURE(level <= Level, "out of index in level");

            Prev[level] = value;

            _segment.Write(value, P_PREV_NEXT + (level * PageAddress.SIZE * 2));

            _page.IsDirty = true;
        }
    }
}
