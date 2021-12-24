using System;
using Appalachia.Utility.Strings;

namespace LiteDB
{
    public class QueryAny
    {
        /// <summary>
        /// Returns all documents for which at least one value in arrayFields is equal to value
        /// </summary>
        public BsonExpression EQ(string arrayField, BsonValue value)
        {
            if (arrayField.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(arrayField));

            return BsonExpression.Create(
                ZString.Format("{0} ANY = {1}", arrayField, value ?? BsonValue.Null)
            );
        }

        /// <summary>
        /// Returns all documents for which at least one value in arrayFields are less tha to value (&lt;)
        /// </summary>
        public BsonExpression LT(string arrayField, BsonValue value)
        {
            if (arrayField.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(arrayField));

            return BsonExpression.Create(
                ZString.Format("{0} ANY < {1}", arrayField, value ?? BsonValue.Null)
            );
        }

        /// <summary>
        /// Returns all documents for which at least one value in arrayFields are less than or equals value (&lt;=)
        /// </summary>
        public BsonExpression LTE(string arrayField, BsonValue value)
        {
            if (arrayField.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(arrayField));

            return BsonExpression.Create(
                ZString.Format("{0} ANY <= {1}", arrayField, value ?? BsonValue.Null)
            );
        }

        /// <summary>
        /// Returns all documents for which at least one value in arrayFields are greater than value (&gt;)
        /// </summary>
        public BsonExpression GT(string arrayField, BsonValue value)
        {
            if (arrayField.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(arrayField));

            return BsonExpression.Create(
                ZString.Format("{0} ANY > {1}", arrayField, value ?? BsonValue.Null)
            );

        }

        /// <summary>
        /// Returns all documents for which at least one value in arrayFields are greater than or equals value (&gt;=)
        /// </summary>
        public BsonExpression GTE(string arrayField, BsonValue value)
        {
            if (arrayField.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(arrayField));

            return BsonExpression.Create(
                ZString.Format("{0} ANY >= {1}", arrayField, value ?? BsonValue.Null)
            );
        }

        /// <summary>
        /// Returns all documents for which at least one value in arrayFields are between "start" and "end" values (BETWEEN)
        /// </summary>
        public BsonExpression Between(string arrayField, BsonValue start, BsonValue end)
        {
            if (arrayField.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(arrayField));

            return BsonExpression.Create(
                ZString.Format(
                    "{0} ANY BETWEEN {1} AND {2}",
                    arrayField,
                    start ?? BsonValue.Null,
                    end ?? BsonValue.Null
                )
            );
        }

        /// <summary>
        /// Returns all documents for which at least one value in arrayFields starts with value (LIKE)
        /// </summary>
        public BsonExpression StartsWith(string arrayField, string value)
        {
            if (arrayField.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(arrayField));
            if (value.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(value));

            return BsonExpression.Create(
                ZString.Format("{0} ANY LIKE {1}", arrayField, new BsonValue(value + "%"))
            );
        }

        /// <summary>
        /// Returns all documents for which at least one value in arrayFields are not equals to value (not equals)
        /// </summary>
        public BsonExpression Not(string arrayField, BsonValue value)
        {
            if (arrayField.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(arrayField));

            return BsonExpression.Create(
                ZString.Format("{0} ANY != {1}", arrayField, value ?? BsonValue.Null)
            );
        }
    }
}