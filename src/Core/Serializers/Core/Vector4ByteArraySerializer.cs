using UnityEngine;

namespace Appalachia.Data.Core.Serializers.Core
{
    public class Vector4ByteArraySerializer : IDataSerializer<Vector4, byte[]>
    {
        #region IDataSerializer<Vector4,byte[]> Members

        public Vector4 Deserialize(byte[] input)
        {
            var floats = SerializerUtilities.BytesToFloats(input);

            Vector4 value;
            value.x = floats[0];
            value.y = floats[1];
            value.z = floats[2];
            value.w = floats[3];

            return value;
        }

        public byte[] Serialize(Vector4 value)
        {
            return SerializerUtilities.FloatsToBytes(value.x, value.y, value.z, value.w);
        }

        #endregion
    }
}