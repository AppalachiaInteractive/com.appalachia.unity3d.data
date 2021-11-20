using System;

namespace Appalachia.Data.Core.Attributes
{
    /// <summary>
    /// Set a name to this property in BsonDocument
    /// </summary>
    public class DataFieldAttribute : Attribute
    {
        public string Name { get; set; }

        public DataFieldAttribute(string name)
        {
            this.Name = name;
        }

        public DataFieldAttribute()
        {
        }
    }
}