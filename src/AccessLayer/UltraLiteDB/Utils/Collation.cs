using System.Collections.Generic;
using System.Globalization;

namespace UltraLiteDB
{
    /// <summary>
    ///     Implement how database will compare to order by/find strings according defined culture/compare options
    ///     If not set, default is CurrentCulture with IgnoreCase
    /// </summary>
    public class Collation : IComparer<BsonValue>, IComparer<string>, IEqualityComparer<BsonValue>
    {
        public Collation(CompareOptions sortOptions)
        {
            SortOptions = sortOptions;
            Culture = new CultureInfo("");

            _compareInfo = Culture.CompareInfo;
        }

        #region Static Fields and Autoproperties

        public static Collation Binary = new Collation(CompareOptions.Ordinal);

        public static Collation Default = new Collation(CompareOptions.IgnoreCase);

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
