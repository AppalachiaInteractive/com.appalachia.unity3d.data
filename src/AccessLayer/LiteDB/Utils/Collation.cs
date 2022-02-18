using System;
using System.Collections.Generic;
using System.Globalization;

namespace LiteDB
{
    /// <summary>
    ///     Implement how database will compare to order by/find strings according defined culture/compare options
    ///     If not set, default is CurrentCulture with IgnoreCase
    /// </summary>
    public class Collation : IComparer<BsonValue>, IComparer<string>, IEqualityComparer<BsonValue>
    {
        public Collation(string collation)
        {
            var parts = collation.Split('/');
            var culture = parts[0];
            var sortOptions = parts.Length > 1
                ? (CompareOptions)Enum.Parse(typeof(CompareOptions), parts[1])
                : CompareOptions.None;

            LCID = LiteDB.LCID.GetLCID(culture);
            SortOptions = sortOptions;
            Culture = new CultureInfo(culture);

            _compareInfo = Culture.CompareInfo;
        }

        public Collation(int lcid, CompareOptions sortOptions)
        {
            LCID = lcid;
            SortOptions = sortOptions;
            Culture = LiteDB.LCID.GetCulture(lcid);

            _compareInfo = Culture.CompareInfo;
        }

        #region Static Fields and Autoproperties

        public static Collation Binary = new Collation(127 /* Invariant */, CompareOptions.Ordinal);

        public static Collation Default = new Collation(LiteDB.LCID.Current, CompareOptions.IgnoreCase);

        #endregion

        #region Fields and Autoproperties

        /// <summary>
        ///     Get options to how string should be compared in sort
        /// </summary>
        public CompareOptions SortOptions { get; }

        /// <summary>
        ///     Get database language culture
        /// </summary>
        public CultureInfo Culture { get; }

        /// <summary>
        ///     Get LCID code from culture
        /// </summary>
        public int LCID { get; }

        private readonly CompareInfo _compareInfo;

        #endregion

        /// <inheritdoc />
        public override string ToString()
        {
            return Culture.Name + "/" + SortOptions;
        }

        /// <summary>
        ///     Compare 2 chars values using current culture/compare options
        /// </summary>
        public int Compare(char left, char right)
        {
            //TODO implementar o compare corretamente
            return char.ToUpper(left) == char.ToUpper(right) ? 0 : 1;
        }

        #region IComparer<BsonValue> Members

        public int Compare(BsonValue left, BsonValue rigth)
        {
            return left.CompareTo(rigth, this);
        }

        #endregion

        #region IComparer<string> Members

        /// <summary>
        ///     Compare 2 string values using current culture/compare options
        /// </summary>
        public int Compare(string left, string right)
        {
            var result = _compareInfo.Compare(left, right, SortOptions);

            return result < 0
                ? -1
                : result > 0
                    ? +1
                    : 0;
        }

        #endregion

        #region IEqualityComparer<BsonValue> Members

        public bool Equals(BsonValue x, BsonValue y)
        {
            return Compare(x, y) == 0;
        }

        public int GetHashCode(BsonValue obj)
        {
            return obj.GetHashCode();
        }

        #endregion
    }
}
