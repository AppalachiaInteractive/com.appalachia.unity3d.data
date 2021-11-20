using Unity.Mathematics;

namespace Appalachia.Data.Core.Serializers.Core
{
    public class float4ByteArraySerializer : IDataSerializer<float4, byte[]>
    {
        #region IDataSerializer<float4,byte[]> Members

        public float4 Deserialize(byte[] input)
        {
            var floats = SerializerUtilities.BytesToFloats(input);

            float4 value;
            value.x = floats[0];
            value.y = floats[1];
            value.z = floats[2];
            value.w = floats[3];

            return value;
        }

        public byte[] Serialize(float4 value)
        {
            return SerializerUtilities.FloatsToBytes(value.x, value.y, value.z, value.w);
        }

        #endregion
    }
}