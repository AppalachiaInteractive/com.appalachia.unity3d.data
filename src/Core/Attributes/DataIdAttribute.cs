using System;

namespace Appalachia.Data.Core.Attributes
{
    /// <summary>
    /// Indicate that property will be used as BsonDocument Id
    /// </summary>
    public class DataIdAttribute : Attribute
    {
        public bool AutoId { get; private set; }

        public DataIdAttribute()
        {
            this.AutoId = true;
        }

        public DataIdAttribute(bool autoId)
        {
            this.AutoId = autoId;
        }
    }
}