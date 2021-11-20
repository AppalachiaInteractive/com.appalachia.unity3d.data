using System;

namespace Appalachia.Data.Core.Attributes
{
    /// <summary>
    /// Indicate that property will not be persist in Bson serialization
    /// </summary>
    public class DataIgnoreAttribute : Attribute
    {
    }
}