using System.Collections.Generic;

namespace LiteDB.Engine
{
    public partial class LiteEngine
    {
        private IEnumerable<BsonDocument> SysSequences()
        {
            var values = _sequences.ToArray();

            foreach(var value in values)
            {
                yield return new BsonDocument
                {
                    ["collection"] = value.Key,
                    ["value"] = value.Value
                };
            }
        }
    }
}