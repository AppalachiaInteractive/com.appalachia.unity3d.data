using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace UltraLiteDB
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
        internal virtual object RawValue { get; }

        #endregion

        public static bool operator ==(BsonValue lhs, BsonValue rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                return ReferenceEquals(rhs, null) || rhs.IsNull;
            }

            if (ReferenceEquals(rhs, null))
            {
                return ReferenceEquals(lhs, null) || lhs.IsNull;
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
            hash = (37 * hash) + RawValue.GetHashCode();
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

        public BsonValue(float value)
        {
            Type = BsonType.Double;
            RawValue = (double)value;
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

        public BsonValue(BsonValue value)
        {
            Type = value == null ? BsonType.Null : value.Type;
            RawValue = value.RawValue;
        }

        public static BsonValue FromObject(object value)
        {
            if (value == null)
            {
                return new BsonValue(BsonType.Null, null);
            }

            if (value is int)
            {
                return new BsonValue((int)value);
            }

            if (value is long)
            {
                return new BsonValue((long)value);
            }

            if (value is float)
            {
                return new BsonValue((double)value);
            }

            if (value is double)
            {
                return new BsonValue((double)value);
            }

            if (value is decimal)
            {
                return new BsonValue((decimal)value);
            }

            if (value is string)
            {
                return new BsonValue((string)value);
            }

            if (value is IDictionary<string, BsonValue>)
            {
                return new BsonDocument((IDictionary<string, BsonValue>)value);
            }

            if (value is IDictionary)
            {
                return new BsonDocument((IDictionary)value);
            }

            if (value is List<BsonValue>)
            {
                return new BsonArray((List<BsonValue>)value);
            }

            if (value is IEnumerable)
            {
                return new BsonArray((IEnumerable)value);
            }

            if (value is byte[])
            {
                return new BsonValue((byte[])value);
            }

            if (value is ObjectId)
            {
                return new BsonValue((ObjectId)value);
            }

            if (value is Guid)
            {
                return new BsonValue((Guid)value);
            }

            if (value is bool)
            {
                return new BsonValue((bool)value);
            }

            if (value is DateTime)
            {
                return new BsonValue((DateTime)value);
            }

            if (value is BsonValue)
            {
                return new BsonValue((BsonValue)value);
            }

            throw new InvalidCastException(
                "Value is not a valid BSON data type - Use Mapper.ToDocument for more complex types converts"
            );
        }

        protected BsonValue(BsonType type, object rawValue)
        {
            Type = type;
            RawValue = rawValue;
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
        public float AsSingle => Convert.ToSingle(RawValue);

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
        public bool IsSingle => Type == BsonType.Double;

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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsDefaultValue
        {
            get
            {
                switch (Type)
                {
                    case BsonType.Null:
                        return true;
                    case BsonType.MinValue:
                    case BsonType.MaxValue:
                        return false;
                    case BsonType.Int32:
                        return AsInt32 != 0;
                    case BsonType.Int64:
                        return AsInt64 != 0L;
                    case BsonType.Double:
                        return AsDouble != 0.0;
                    case BsonType.Decimal:
                        return AsDecimal != 0L;
                    case BsonType.String:
                        return AsString.IsNullOrEmpty();
                    case BsonType.Document:
                        return AsDocument.Count > 0;
                    case BsonType.Array:
                        return AsArray.Count > 0;
                    case BsonType.Binary:
                        return AsBinary.Length > 0;
                    case BsonType.ObjectId:
                        return AsObjectId == ObjectId.Empty;
                    case BsonType.Guid:
                        return AsGuid == Guid.Empty;
                    case BsonType.Boolean:
                        return AsBoolean == false;
                    case BsonType.DateTime:
                        return AsDateTime == DateTime.MinValue;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

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

        // Single
        public static implicit operator float(BsonValue value)
        {
            return (float)value.RawValue;
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

                return Type.CompareTo(other.Type);
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
        public virtual int GetBytesCount(bool recalc)
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
