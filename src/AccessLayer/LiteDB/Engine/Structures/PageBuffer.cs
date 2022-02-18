using System.Threading;
using Appalachia.Utility.Strings;
using static LiteDB.Constants;

namespace LiteDB.Engine
{
    /// <summary>
    ///     Represent page buffer to be read/write using FileMemory
    /// </summary>
    internal class PageBuffer : BufferSlice
    {
        public PageBuffer(byte[] buffer, int offset, int uniqueID) : base(buffer, offset, PAGE_SIZE)
        {
            UniqueID = uniqueID;
            Position = long.MaxValue;
            Origin = FileOrigin.None;
            ShareCounter = 0;
            Timestamp = 0;
        }

#if DEBUG
        ~PageBuffer()
        {
            ENSURE(
                ShareCounter == 0,
                ZString.Format("share count must be 0 in destroy PageBuffer (current: {0})", ShareCounter)
            );
        }
#endif

        #region Fields and Autoproperties

        /// <summary>
        ///     Get, on initialize, a unique ID in all database instance for this PageBufer. Is a simple global incremented counter
        /// </summary>
        public readonly int UniqueID;

        /// <summary>
        ///     Get/Set page bytes origin (data/log)
        /// </summary>
        public FileOrigin Origin;

        /// <summary>
        ///     Get/Set how many read-share threads are using this page. -1 means 1 thread are using as writable
        /// </summary>
        public int ShareCounter;

        /// <summary>
        ///     Get/Set page position. If page are writable, this postion CAN be MaxValue (has not defined position yet)
        /// </summary>
        public long Position;

        /// <summary>
        ///     Get/Set timestamp from last request
        /// </summary>
        public long Timestamp;

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            var p = Position == long.MaxValue ? "<empty>" : Position.ToString();
            var s = ShareCounter == BUFFER_WRITABLE ? "<writable>" : ShareCounter.ToString();
            var pageID = this.ReadUInt32(0);
            var pageType = this[4];

            return ZString.Format(
                "ID: {0} - Position: {1}/{2} - Shared: {3} - ({4}) :: Content: [{5}/{6}]",
                UniqueID,
                p,
                Origin,
                s,
                base.ToString(),
                pageID.ToString("0:0000"),
                (PageType)pageType
            );
        }

        public unsafe bool IsBlank()
        {
            fixed (byte* arrayPtr = Array)
            {
                var ptr = (ulong*)(arrayPtr + Offset);

                return (*ptr == 0UL) && (*(ptr + 1) == 0UL);
            }
        }

        /// <summary>
        ///     Release this page - decrement ShareCounter
        /// </summary>
        public void Release()
        {
            ENSURE(ShareCounter > 0, "share counter must be > 0 in Release()");

            Interlocked.Decrement(ref ShareCounter);
        }
    }
}
