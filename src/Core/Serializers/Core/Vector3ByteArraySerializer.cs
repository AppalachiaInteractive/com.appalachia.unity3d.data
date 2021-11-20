using UnityEngine;

namespace Appalachia.Data.Core.Serializers.Core
{
    public class Vector3ByteArraySerializer : IDataSerializer<Vector3, byte[]>
    {
        #region IDataSerializer<Vector3,byte[]> Members

        public Vector3 Deserialize(byte[] input)
        {
            var floats = SerializerUtilities.BytesToFloats(input);

            Vector3 value;
            value.x = floats[0];
            value.y = floats[1];
            value.z = floats[2];

            return value;
        }

        public byte[] Serialize(Vector3 value)
        {
            return SerializerUtilities.FloatsToBytes(value.x, value.y, value.z);
        }

        #endregion
    }
}