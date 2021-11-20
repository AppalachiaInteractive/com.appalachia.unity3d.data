using System;

namespace Appalachia.Data.Core.Attributes
{
    /// <summary>
    /// Indicate that field are not persisted inside this document but it's a reference for another document (DbRef)
    /// </summary>
    public class DataRefAttribute : Attribute
    {
        public string Collection { get; set; }

        public DataRefAttribute(string collection)
        {
            this.Collection = collection;
        }

        public DataRefAttribute()
        {
            this.Collection = null;
        }
    }
}