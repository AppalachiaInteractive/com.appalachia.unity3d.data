using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace UltraLiteDB
{
    public class BsonDocument : BsonValue, IDictionary<string, BsonValue>
    {
        public BsonDocument() : base(
            BsonType.Document,
            new Dictionary<string, BsonValue>(StringComparer.OrdinalIgnoreCase)
        )
        {
        }

        public BsonDocument(ConcurrentDictionary<string, BsonValue> dict) : this()
        {
            if (dict == null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            foreach (var element in dict)
            {
                Add(element);
            }
        }

        public BsonDocument(IDictionary<string, BsonValue> dict) : this()
        {
            if (dict == null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            foreach (var element in dict)
            {
                Add(element);
            }
        }

        public BsonDocument(IDictionary dict) : this()
        {
            if (dict == null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            foreach (var key in dict.Keys)
            {
                Add(key.ToString(), FromObject(dict[key]));
            }
        }

        #region Fields and Autoproperties

        /// <summary>
        ///     Get/Set position of this document inside database. It's filled when used in Find operation.
        /// </summary>
        internal PageAddress RawId { get; set; } = PageAddress.Empty;

        private int _length;

        #endregion

        internal new Dictionary<string, BsonValue> RawValue => base.RawValue as Dictionary<string, BsonValue>;

        #region CompareTo

        /// <inheritdoc />
        public override int CompareTo(BsonValue other)
        {
            // if types are different, returns sort type order
            if (other.Type != BsonType.Document)
            {
                return Type.CompareTo(other.Type);
            }

            var thisKeys = Keys.ToArray();
            var thisLength = thisKeys.Length;

            var otherDoc = other.AsDocument;
            var otherKeys = otherDoc.Keys.ToArray();
            var otherLength = otherKeys.Length;

            var result = 0;
            var i = 0;
            var stop = Math.Min(thisLength, otherLength);

            for (; (0 == result) && (i < stop); i++)
            {
                result = this[thisKeys[i]].CompareTo(otherDoc[thisKeys[i]]);
            }

            // are different
            if (result != 0)
            {
                return result;
            }

            // test keys length to check which is bigger
            if (i == thisLength)
            {
                return i == otherLength ? 0 : -1;
            }

            return 1;
        }

        #endregion

        /// <inheritdoc />
        public override int GetBytesCount(bool recalc)
        {
            if ((recalc == false) && (_length > 0))
            {
                return _length;
            }

            var length = 5;

            foreach (var element in RawValue)
            {
                length += GetBytesCountElement(element.Key, element.Value);
            }

            return _length = length;
        }

        public bool? GetBool(string key)
        {
            BsonValue value;
            if (RawValue.TryGetValue(key, out value))
            {
                if (value.IsBoolean)
                {
                    return value;
                }
            }

            return null;
        }

        public bool GetBoolOrDefault(string key, bool def)
        {
            BsonValue value;
            if (RawValue.TryGetValue(key, out value))
            {
                if (value.IsBoolean)
                {
                    return value;
                }
            }

            return def;
        }

        public double? GetDouble(string key)
        {
            BsonValue value;
            if (RawValue.TryGetValue(key, out value))
            {
                if (value.IsNumber)
                {
                    return value.AsDouble;
                }
            }

            return null;
        }

        public double GetDoubleOrDefault(string key, double def)
        {
            BsonValue value;
            if (RawValue.TryGetValue(key, out value))
            {
                if (value.IsNumber)
                {
                    return value.AsDouble;
                }
            }

            return def;
        }

        public int? GetInt32(string key)
        {
            BsonValue value;
            if (RawValue.TryGetValue(key, out value))
            {
                if (value.IsNumber)
                {
                    return value.AsInt32;
                }
            }

            return null;
        }

        public int GetInt32OrDefault(string key, int def)
        {
            BsonValue value;
            if (RawValue.TryGetValue(key, out value))
            {
                if (value.IsNumber)
                {
                    return value.AsInt32;
                }
            }

            return def;
        }

        public long? GetInt64(string key)
        {
            BsonValue value;
            if (RawValue.TryGetValue(key, out value))
            {
                if (value.IsNumber)
                {
                    return value.AsInt64;
                }
            }

            return null;
        }

        public long GetInt64OrDefault(string key, long def)
        {
            BsonValue value;
            if (RawValue.TryGetValue(key, out value))
            {
                if (value.IsNumber)
                {
                    return value.AsInt64;
                }
            }

            return def;
        }

        public float? GetSingle(string key)
        {
            BsonValue value;
            if (RawValue.TryGetValue(key, out value))
            {
                if (value.IsNumber)
                {
                    return value.AsSingle;
                }
            }

            return null;
        }

        public float GetSingleOrDefault(string key, float def)
        {
            BsonValue value;
            if (RawValue.TryGetValue(key, out value))
            {
                if (value.IsNumber)
                {
                    return value.AsSingle;
                }
            }

            return def;
        }

        public string GetString(string key)
        {
            BsonValue value;
            if (RawValue.TryGetValue(key, out value))
            {
                if (value.IsString)
                {
                    return value;
                }
            }

            return null;
        }

        public string GetStringOrDefault(string key, string def)
        {
            BsonValue value;
            if (RawValue.TryGetValue(key, out value))
            {
                if (value.IsString)
                {
                    return value;
                }
            }

            return def;
        }

        #region IDictionary<string,BsonValue> Members

        /// <summary>
        ///     Get/Set a field for document. Fields are case sensitive
        /// </summary>
        public override BsonValue this[string key]
        {
            get => RawValue.GetOrDefault(key, Null);
            set => RawValue[key] = value ?? Null;
        }

        #endregion

        #region IDictionary

        public ICollection<string> Keys => RawValue.Keys;

        public ICollection<BsonValue> Values => RawValue.Values;

        public int Count => RawValue.Count;

        public bool IsReadOnly => false;

        public bool ContainsKey(string key)
        {
            return RawValue.ContainsKey(key);
        }

        /// <summary>
        ///     Get all document elements - Return "_id" as first of all (if exists)
        /// </summary>
        public IEnumerable<KeyValuePair<string, BsonValue>> GetElements()
        {
            if (RawValue.TryGetValue("_id", out var id))
            {
                yield return new KeyValuePair<string, BsonValue>("_id", id);
            }

            foreach (var item in RawValue.Where(x => x.Key != "_id"))
            {
                yield return item;
            }
        }

        public void Add(string key, BsonValue value)
        {
            RawValue.Add(key, value ?? Null);
        }

        public bool Remove(string key)
        {
            return RawValue.Remove(key);
        }

        public void Clear()
        {
            RawValue.Clear();
        }

        public bool TryGetValue(string key, out BsonValue value)
        {
            return RawValue.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<string, BsonValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            Add(item.Key, FromObject(item.Value));
        }

        public bool Contains(KeyValuePair<string, BsonValue> item)
        {
            return RawValue.Contains(item);
        }

        public bool Remove(KeyValuePair<string, BsonValue> item)
        {
            return Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, BsonValue>> GetEnumerator()
        {
            return RawValue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return RawValue.GetEnumerator();
        }

        public void CopyTo(KeyValuePair<string, BsonValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, BsonValue>>)RawValue).CopyTo(array, arrayIndex);
        }

        public void CopyTo(BsonDocument other)
        {
            foreach (var element in this)
            {
                other[element.Key] = element.Value;
            }
        }

        #endregion
    }
}
