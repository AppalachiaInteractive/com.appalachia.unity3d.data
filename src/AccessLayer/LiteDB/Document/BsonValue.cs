using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LiteDB
{
    /// <summary>
    ///     Represent a Bson Value used in BsonDocument
    /// </summary>
    public class BsonValue : IComparable<BsonValue>, IEquatable<BsonValue>
    {
        #region Constants and Static Readonly

        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Static Fields and Autoproperties

        /// <summary>
        ///     Represent a MaxValue bson type
        /// </summary>
        public static BsonValue MaxValue = new BsonValue(BsonType.MaxValue, "+oo");

        /// <summary>
        ///     Represent a MinValue bson type
        /// </summary>
        public static BsonValue MinValue = new BsonValue(BsonType.MinValue, "-oo");

        /// <summary>
        ///     Represent a Null bson type
        /// </summary>
        public static BsonValue Null = new BsonValue(BsonType.Null, null);

        #endregion

        #region Fields and Autoproperties

        /// <summary>
        ///     Indicate BsonType of this BsonValue
        /// </summary>
        public BsonType Type { get; }

        /// <summary>
        ///     Get internal .NET value object
        /// </summary>
        public virtual object RawValue { get; }

        #endregion

        /// <summary>
        ///     Create a new document used in DbRef => { $id: id, $ref: collection }
        /// </summary>
        public static BsonDocument DbRef(BsonValue id, string collection)
        {
            return new BsonDocument { ["$id"] = id, ["$ref"] = collection };
        }

        public static bool operator ==(BsonValue lhs, BsonValue rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                return ReferenceEquals(rhs, null);
            }

            if (ReferenceEquals(rhs, null))
            {
                return false; // don't check type because sometimes different types can be ==
            }

            return lhs.Equals(rhs);
        }

        public static bool operator >(BsonValue lhs, BsonValue rhs)
        {
            return lhs.CompareTo(rhs) > 0;
        }

        public static bool operator >=(BsonValue lhs, BsonValue rhs)
        {
            return lhs.CompareTo(rhs) >= 0;
        }

        public static bool operator !=(BsonValue lhs, BsonValue rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator <(BsonValue lhs, BsonValue rhs)
        {
            return lhs.CompareTo(rhs) < 0;
        }

        public static bool operator <=(BsonValue lhs, BsonValue rhs)
        {
            return lhs.CompareTo(rhs) <= 0;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is BsonValue other)
            {
                return Equals(other);
            }

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hash = 17;
            hash = (37 * hash) + Type.GetHashCode();
            hash = (37 * hash) + (RawValue?.GetHashCode() ?? 0);
            return hash;
        }

        #region Constructor

        public BsonValue()
        {
            Type = BsonType.Null;
            RawValue = null;
        }

        public BsonValue(int value)
        {
            Type = BsonType.Int32;
            RawValue = value;
        }

        public BsonValue(long value)
        {
            Type = BsonType.Int64;
            RawValue = value;
        }

        public BsonValue(double value)
        {
            Type = BsonType.Double;
            RawValue = value;
        }

        public BsonValue(decimal value)
        {
            Type = BsonType.Decimal;
            RawValue = value;
        }

        public BsonValue(string value)
        {
            Type = value == null ? BsonType.Null : BsonType.String;
            RawValue = value;
        }

        public BsonValue(byte[] value)
        {
            Type = value == null ? BsonType.Null : BsonType.Binary;
            RawValue = value;
        }

        public BsonValue(ObjectId value)
        {
            Type = value == null ? BsonType.Null : BsonType.ObjectId;
            RawValue = value;
        }

        public BsonValue(Guid value)
        {
            Type = BsonType.Guid;
            RawValue = value;
        }

        public BsonValue(bool value)
        {
            Type = BsonType.Boolean;
            RawValue = value;
        }

        public BsonValue(DateTime value)
        {
            Type = BsonType.DateTime;
            RawValue = value.Truncate();
        }

        protected BsonValue(BsonType type, object rawValue)
        {
            Type = type;
            RawValue = rawValue;
        }

        public BsonValue(object value)
        {
            RawValue = value;

            if (value == null)
            {
                Type = BsonType.Null;
            }
            else if (value is int)
            {
                Type = BsonType.Int32;
            }
            else if (value is long)
            {
                Type = BsonType.Int64;
            }
            else if (value is double)
            {
                Type = BsonType.Double;
            }
            else if (value is decimal)
            {
                Type = BsonType.Decimal;
            }
            else if (value is string)
            {
                Type = BsonType.String;
            }
            else if (value is IDictionary<string, BsonValue>)
            {
                Type = BsonType.Document;
            }
            else if (value is IList<BsonValue>)
            {
                Type = BsonType.Array;
            }
            else if (value is byte[])
            {
                Type = BsonType.Binary;
            }
            else if (value is ObjectId)
            {
                Type = BsonType.ObjectId;
            }
            else if (value is Guid)
            {
                Type = BsonType.Guid;
            }
            else if (value is bool)
            {
                Type = BsonType.Boolean;
            }
            else if (value is DateTime)
            {
                Type = BsonType.DateTime;
                RawValue = ((DateTime)value).Truncate();
            }
            else if (value is BsonValue)
            {
                var v = (BsonValue)value;
                Type = v.Type;
                RawValue = v.RawValue;
            }
            else
            {
                // test for array or dictionary (document)
                var enumerable = value as System.Collections.IEnumerable;
                var dictionary = value as System.Collections.IDictionary;

                // test first for dictionary (because IDictionary implements IEnumerable)
                if (dictionary != null)
                {
                    var dict = new Dictionary<string, BsonValue>();

                    foreach (var key in dictionary.Keys)
                    {
                        dict.Add(key.ToString(), new BsonValue(dictionary[key]));
                    }

                    Type = BsonType.Document;
                    RawValue = dict;
                }
                else if (enumerable != null)
                {
                    var list = new List<BsonValue>();

                    foreach (var x in enumerable)
                    {
                        list.Add(new BsonValue(x));
                    }

                    Type = BsonType.Array;
                    RawValue = list;
                }
                else
                {
                    throw new InvalidCastException(
                        "Value is not a valid BSON data type - Use Mapper.ToDocument for more complex types converts"
                    );
                }
            }
        }

        #endregion

        #region Index "this" property

        /// <summary>
        ///     Get/Set a field for document. Fields are case sensitive - Works only when value are document
        /// </summary>
        public virtual BsonValue this[string name]
        {
            get =>
                throw new InvalidOperationException("Cannot access non-document type value on " + RawValue);
            set =>
                throw new InvalidOperationException("Cannot access non-document type value on " + RawValue);
        }

        /// <summary>
        ///     Get/Set value in array position. Works only when value are array
        /// </summary>
        public virtual BsonValue this[int index]
        {
            get => throw new InvalidOperationException("Cannot access non-array type value on " + RawValue);
            set => throw new InvalidOperationException("Cannot access non-array type value on " + RawValue);
        }

        #endregion

        #region Convert types

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public BsonArray AsArray => this as BsonArray;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public BsonDocument AsDocument => this as BsonDocument;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public byte[] AsBinary => RawValue as byte[];

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool AsBoolean => (bool)RawValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string AsString => (string)RawValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int AsInt32 => Convert.ToInt32(RawValue);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public long AsInt64 => Convert.ToInt64(RawValue);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public double AsDouble => Convert.ToDouble(RawValue);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public decimal AsDecimal => Convert.ToDecimal(RawValue);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public DateTime AsDateTime => (DateTime)RawValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ObjectId AsObjectId => (ObjectId)RawValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public Guid AsGuid => (Guid)RawValue;

        #endregion

        #region IsTypes

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsNull => Type == BsonType.Null;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsArray => Type == BsonType.Array;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsDocument => Type == BsonType.Document;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsInt32 => Type == BsonType.Int32;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsInt64 => Type == BsonType.Int64;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsDouble => Type == BsonType.Double;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsDecimal => Type == BsonType.Decimal;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsNumber => IsInt32 || IsInt64 || IsDouble || IsDecimal;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsBinary => Type == BsonType.Binary;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsBoolean => Type == BsonType.Boolean;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsString => Type == BsonType.String;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsObjectId => Type == BsonType.ObjectId;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsGuid => Type == BsonType.Guid;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsDateTime => Type == BsonType.DateTime;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsMinValue => Type == BsonType.MinValue;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsMaxValue => Type == BsonType.MaxValue;

        #endregion

        #region Implicit Ctor

        // Int32
        public static implicit operator int(BsonValue value)
        {
            return (int)value.RawValue;
        }

        // Int32
        public static implicit operator BsonValue(int value)
        {
            return new BsonValue(value);
        }

        // Int64
        public static implicit operator long(BsonValue value)
        {
            return (long)value.RawValue;
        }

        // Int64
        public static implicit operator BsonValue(long value)
        {
            return new BsonValue(value);
        }

        // Double
        public static implicit operator double(BsonValue value)
        {
            return (double)value.RawValue;
        }

        // Double
        public static implicit operator BsonValue(double value)
        {
            return new BsonValue(value);
        }

        // Decimal
        public static implicit operator decimal(BsonValue value)
        {
            return (decimal)value.RawValue;
        }

        // Decimal
        public static implicit operator BsonValue(decimal value)
        {
            return new BsonValue(value);
        }

        // UInt64 (to avoid ambigous between Double-Decimal)
        public static implicit operator ulong(BsonValue value)
        {
            return (ulong)value.RawValue;
        }

        // Decimal
        public static implicit operator BsonValue(ulong value)
        {
            return new BsonValue((double)value);
        }

        // String
        public static implicit operator string(BsonValue value)
        {
            return (string)value.RawValue;
        }

        // String
        public static implicit operator BsonValue(string value)
        {
            return new BsonValue(value);
        }

        // Binary
        public static implicit operator byte[](BsonValue value)
        {
            return (byte[])value.RawValue;
        }

        // Binary
        public static implicit operator BsonValue(byte[] value)
        {
            return new BsonValue(value);
        }

        // ObjectId
        public static implicit operator ObjectId(BsonValue value)
        {
            return (ObjectId)value.RawValue;
        }

        // ObjectId
        public static implicit operator BsonValue(ObjectId value)
        {
            return new BsonValue(value);
        }

        // Guid
        public static implicit operator Guid(BsonValue value)
        {
            return (Guid)value.RawValue;
        }

        // Guid
        public static implicit operator BsonValue(Guid value)
        {
            return new BsonValue(value);
        }

        // Boolean
        public static implicit operator bool(BsonValue value)
        {
            return (bool)value.RawValue;
        }

        // Boolean
        public static implicit operator BsonValue(bool value)
        {
            return new BsonValue(value);
        }

        // DateTime
        public static implicit operator DateTime(BsonValue value)
        {
            return (DateTime)value.RawValue;
        }

        // DateTime
        public static implicit operator BsonValue(DateTime value)
        {
            return new BsonValue(value);
        }

        // +
        public static BsonValue operator +(BsonValue left, BsonValue right)
        {
            if (!left.IsNumber || !right.IsNumber)
            {
                return Null;
            }

            if (left.IsInt32 && right.IsInt32)
            {
                return left.AsInt32 + right.AsInt32;
            }

            if (left.IsInt64 && right.IsInt64)
            {
                return left.AsInt64 + right.AsInt64;
            }

            if (left.IsDouble && right.IsDouble)
            {
                return left.AsDouble + right.AsDouble;
            }

            if (left.IsDecimal && right.IsDecimal)
            {
                return left.AsDecimal + right.AsDecimal;
            }

            var result = left.AsDecimal + right.AsDecimal;
            var type = (BsonType)Math.Max((int)left.Type, (int)right.Type);

            return type == BsonType.Int64
                ? new BsonValue((long)result)
                : type == BsonType.Double
                    ? new BsonValue((double)result)
                    : new BsonValue(result);
        }

        // -
        public static BsonValue operator -(BsonValue left, BsonValue right)
        {
            if (!left.IsNumber || !right.IsNumber)
            {
                return Null;
            }

            if (left.IsInt32 && right.IsInt32)
            {
                return left.AsInt32 - right.AsInt32;
            }

            if (left.IsInt64 && right.IsInt64)
            {
                return left.AsInt64 - right.AsInt64;
            }

            if (left.IsDouble && right.IsDouble)
            {
                return left.AsDouble - right.AsDouble;
            }

            if (left.IsDecimal && right.IsDecimal)
            {
                return left.AsDecimal - right.AsDecimal;
            }

            var result = left.AsDecimal - right.AsDecimal;
            var type = (BsonType)Math.Max((int)left.Type, (int)right.Type);

            return type == BsonType.Int64
                ? new BsonValue((long)result)
                : type == BsonType.Double
                    ? new BsonValue((double)result)
                    : new BsonValue(result);
        }

        // *
        public static BsonValue operator *(BsonValue left, BsonValue right)
        {
            if (!left.IsNumber || !right.IsNumber)
            {
                return Null;
            }

            if (left.IsInt32 && right.IsInt32)
            {
                return left.AsInt32 * right.AsInt32;
            }

            if (left.IsInt64 && right.IsInt64)
            {
                return left.AsInt64 * right.AsInt64;
            }

            if (left.IsDouble && right.IsDouble)
            {
                return left.AsDouble * right.AsDouble;
            }

            if (left.IsDecimal && right.IsDecimal)
            {
                return left.AsDecimal * right.AsDecimal;
            }

            var result = left.AsDecimal * right.AsDecimal;
            var type = (BsonType)Math.Max((int)left.Type, (int)right.Type);

            return type == BsonType.Int64
                ? new BsonValue((long)result)
                : type == BsonType.Double
                    ? new BsonValue((double)result)
                    : new BsonValue(result);
        }

        // /
        public static BsonValue operator /(BsonValue left, BsonValue right)
        {
            if (!left.IsNumber || !right.IsNumber)
            {
                return Null;
            }

            if (left.IsDecimal || right.IsDecimal)
            {
                return left.AsDecimal / right.AsDecimal;
            }

            return left.AsDouble / right.AsDouble;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        #endregion

        #region IComparable<BsonValue>, IEquatable<BsonValue>

        public virtual int CompareTo(BsonValue other)
        {
            return CompareTo(other, Collation.Binary);
        }

        public virtual int CompareTo(BsonValue other, Collation collation)
        {
            // first, test if types are different
            if (Type != other.Type)
            {
                // if both values are number, convert them to Decimal (128 bits) to compare
                // it's the slowest way, but more secure
                if (IsNumber && other.IsNumber)
                {
                    return Convert.ToDecimal(RawValue).CompareTo(Convert.ToDecimal(other.RawValue));
                }

                // if not, order by sort type order

                var result = Type.CompareTo(other.Type);
                return result < 0
                    ? -1
                    : result > 0
                        ? +1
                        : 0;
            }

            // for both values with same data type just compare
            switch (Type)
            {
                case BsonType.Null:
                case BsonType.MinValue:
                case BsonType.MaxValue:
                    return 0;

                case BsonType.Int32:
                    return AsInt32.CompareTo(other.AsInt32);
                case BsonType.Int64:
                    return AsInt64.CompareTo(other.AsInt64);
                case BsonType.Double:
                    return AsDouble.CompareTo(other.AsDouble);
                case BsonType.Decimal:
                    return AsDecimal.CompareTo(other.AsDecimal);

                case BsonType.String:
                    return collation.Compare(AsString, other.AsString);

                case BsonType.Document:
                    return AsDocument.CompareTo(other);
                case BsonType.Array:
                    return AsArray.CompareTo(other);

                case BsonType.Binary:
                    return AsBinary.BinaryCompareTo(other.AsBinary);
                case BsonType.ObjectId:
                    return AsObjectId.CompareTo(other.AsObjectId);
                case BsonType.Guid:
                    return AsGuid.CompareTo(other.AsGuid);

                case BsonType.Boolean:
                    return AsBoolean.CompareTo(other.AsBoolean);
                case BsonType.DateTime:
                    var d0 = AsDateTime;
                    var d1 = other.AsDateTime;
                    if (d0.Kind != DateTimeKind.Utc)
                    {
                        d0 = d0.ToUniversalTime();
                    }

                    if (d1.Kind != DateTimeKind.Utc)
                    {
                        d1 = d1.ToUniversalTime();
                    }

                    return d0.CompareTo(d1);

                default:
                    throw new NotImplementedException();
            }
        }

        public bool Equals(BsonValue other)
        {
            return CompareTo(other) == 0;
        }

        #endregion

        #region GetBytesCount()

        /// <summary>
        ///     Returns how many bytes this BsonValue will consume when converted into binary BSON
        ///     If recalc = false, use cached length value (from Array/Document only)
        /// </summary>
        internal virtual int GetBytesCount(bool recalc)
        {
            switch (Type)
            {
                case BsonType.Null:
                case BsonType.MinValue:
                case BsonType.MaxValue:
                    return 0;

                case BsonType.Int32:
                    return 4;
                case BsonType.Int64:
                    return 8;
                case BsonType.Double:
                    return 8;
                case BsonType.Decimal:
                    return 16;

                case BsonType.String:
                    return Encoding.UTF8.GetByteCount(AsString);

                case BsonType.Binary:
                    return AsBinary.Length;
                case BsonType.ObjectId:
                    return 12;
                case BsonType.Guid:
                    return 16;

                case BsonType.Boolean:
                    return 1;
                case BsonType.DateTime:
                    return 8;

                case BsonType.Document:
                    return AsDocument.GetBytesCount(recalc);
                case BsonType.Array:
                    return AsArray.GetBytesCount(recalc);
            }

            throw new ArgumentException();
        }

        /// <summary>
        ///     Get how many bytes one single element will used in BSON format
        /// </summary>
        protected int GetBytesCountElement(string key, BsonValue value)
        {
            // check if data type is variant
            var variant = (value.Type == BsonType.String) ||
                          (value.Type == BsonType.Binary) ||
                          (value.Type == BsonType.Guid);

            return 1 +                               // element type
                   Encoding.UTF8.GetByteCount(key) + // CString
                   1 +                               // CString \0
                   value.GetBytesCount(true) +
                   (variant ? 5 : 0); // bytes.Length + 0x??
        }

        #endregion
    }
}
