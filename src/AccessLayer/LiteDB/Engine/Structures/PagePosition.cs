namespace LiteDB.Engine
{
    /// <summary>
    ///     Represents a page position after save in disk. Used in WAL files where PageID do not match with PagePosition
    /// </summary>
    internal struct PagePosition
    {
        public PagePosition(uint pageID, long position)
        {
            PageID = pageID;
            Position = position;
        }

        #region Fields and Autoproperties

        /// <summary>
        ///     Position in disk
        /// </summary>
        public long Position;

        /// <summary>
        ///     PageID (4 bytes)
        /// </summary>
        public uint PageID;

        #endregion

        public static PagePosition Empty => new PagePosition(uint.MaxValue, long.MaxValue);

        /// <summary>
        ///     Checks if current PagePosition is empty value
        /// </summary>
        public bool IsEmpty => (PageID == uint.MaxValue) && (Position == long.MaxValue);

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            var other = (PagePosition)obj;

            return (PageID == other.PageID) && (Position == other.PageID);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 23) + (int)PageID;
                hash = (hash * 23) + (int)Position;
                return hash;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return IsEmpty
                ? "----:----"
                : (PageID == uint.MaxValue ? "----" : PageID.ToString()) + ":" + Position;
        }
    }
}
