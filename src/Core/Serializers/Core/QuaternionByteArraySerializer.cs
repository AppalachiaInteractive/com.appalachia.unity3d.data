using Unity.Mathematics;
using UnityEngine;

namespace Appalachia.Data.Core.Serializers.Core
{
    public class QuaternionByteArraySerializer : IDataSerializer<Quaternion, byte[]>
    {
        #region IDataSerializer<Quaternion,byte[]> Members

        public Quaternion Deserialize(byte[] input)
        {
            var floats = SerializerUtilities.BytesToFloats(input);

            Quaternion value;
            value.x = floats[0];
            value.y = floats[1];
            value.z = floats[2];
            value.w = floats[3];

            return value;
        }

        public byte[] Serialize(Quaternion value)
        {
            return SerializerUtilities.FloatsToBytes(value.x, value.y, value.z, value.w);
        }

        #endregion
    }

    public class quaternionByteArraySerializer : IDataSerializer<quaternion, byte[]>
    {
        #region IDataSerializer<quaternion,byte[]> Members

        public byte[] Serialize(quaternion value)
        {
            return SerializerUtilities.FloatsToBytes(
                value.value.x,
                value.value.y,
                value.value.z,
                value.value.w
            );
        }

        public quaternion Deserialize(byte[] input)
        {
            var floats = SerializerUtilities.BytesToFloats(input);

            quaternion value;
            value.value.x = floats[0];
            value.value.y = floats[1];
            value.value.z = floats[2];
            value.value.w = floats[3];

            return value;
        }

        #endregion
    }
}
