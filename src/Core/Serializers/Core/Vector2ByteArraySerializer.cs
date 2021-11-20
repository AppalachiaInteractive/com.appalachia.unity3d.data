using UnityEngine;

namespace Appalachia.Data.Core.Serializers.Core
{
    public class Vector2ByteArraySerializer : IDataSerializer<Vector2, byte[]>
    {
        #region IDataSerializer<Vector2,byte[]> Members

        public Vector2 Deserialize(byte[] input)
        {
            var floats = SerializerUtilities.BytesToFloats(input);

            Vector2 value;
            value.x = floats[0];
            value.y = floats[1];

            return value;
        }

        public byte[] Serialize(Vector2 value)
        {
            return SerializerUtilities.FloatsToBytes(value.x, value.y);
        }

        #endregion
    }
}
