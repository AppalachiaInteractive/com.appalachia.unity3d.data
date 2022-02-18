namespace UltraLiteDB
{
    /// <summary>
    ///     Represents a page address inside a page structure - index could be byte offset position OR index in a list (6 bytes)
    /// </summary>
    internal struct PageAddress
    {
        #region Constants and Static Readonly

        public const int SIZE = 6;

        #endregion

        public PageAddress(uint pageID, ushort index)
        {
            PageID = pageID;
            Index = index;
        }

        #region Static Fields and Autoproperties

        public static PageAddress Empty = new PageAddress(uint.MaxValue, ushort.MaxValue);

        #endregion

        #region Fields and Autoproperties

        /// <summary>
        ///     PageID (4 bytes)
        /// </summary>
        public uint PageID;

        /// <summary>
        ///     Index inside page (2 bytes)
        /// </summary>
        public ushort Index;

        #endregion

        public bool IsEmpty => PageID == uint.MaxValue;

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = (PageAddress)obj;

            return (PageID == other.PageID) && (Index == other.Index);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;

                // Maybe nullity checks, if these are objects not primitives!
                hash = (hash * 23) + (int)PageID;
                hash = (hash * 23) + Index;
                return hash;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return IsEmpty ? "----" : PageID + ":" + Index;
        }
    }
}
