using Unity.Mathematics;

namespace Appalachia.Data.Core.Serializers.Core
{
    public class float3ByteArraySerializer : IDataSerializer<float3, byte[]>
    {
        #region IDataSerializer<float3,byte[]> Members

        public float3 Deserialize(byte[] input)
        {
            var floats = SerializerUtilities.BytesToFloats(input);

            float3 value;
            value.x = floats[0];
            value.y = floats[1];
            value.z = floats[2];

            return value;
        }

        public byte[] Serialize(float3 value)
        {
            return SerializerUtilities.FloatsToBytes(value.x, value.y, value.z);
        }

        #endregion
    }
}