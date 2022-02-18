﻿using System.Diagnostics;

namespace LiteDB.Engine
{
    /// <summary>
    ///     Represents a page address inside a page structure - index could be byte offset position OR index in a list (6 bytes)
    /// </summary>
    [DebuggerStepThrough]
    internal struct PageAddress
    {
        #region Constants and Static Readonly

        public const int SIZE = 5;

        #endregion

        public PageAddress(uint pageID, byte index)
        {
            PageID = pageID;
            Index = index;
        }

        #region Static Fields and Autoproperties

        public static PageAddress Empty = new PageAddress(uint.MaxValue, byte.MaxValue);

        #endregion

        #region Fields and Autoproperties

        /// <summary>
        ///     Page Segment index inside page (1 bytes)
        /// </summary>
        public readonly byte Index;

        /// <summary>
        ///     PageID (4 bytes)
        /// </summary>
        public readonly uint PageID;

        #endregion

        /// <summary>
        ///     Returns true if this PageAdress is empty value
        /// </summary>
        public bool IsEmpty => (PageID == uint.MaxValue) && (Index == byte.MaxValue);

        public static bool operator ==(PageAddress lhs, PageAddress rhs)
        {
            return (lhs.PageID == rhs.PageID) && (lhs.Index == rhs.Index);
        }

        public static bool operator !=(PageAddress lhs, PageAddress rhs)
        {
            return !(lhs == rhs);
        }

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
                hash = (hash * 23) + (int)PageID;
                hash = (hash * 23) + Index;
                return hash;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return IsEmpty
                ? "(empty)"
                : PageID.ToString().PadLeft(4, '0') + ":" + Index.ToString().PadLeft(2, '0');
        }

        public BsonValue ToBsonValue()
        {
            if (IsEmpty)
            {
                return BsonValue.Null;
            }

            return new BsonDocument { ["pageID"] = (int)PageID, ["index"] = (int)Index };
        }
    }
}
