using Unity.Mathematics;

namespace Appalachia.Data.Core.Serializers.Core
{
    public class float2ByteArraySerializer : IDataSerializer<float2, byte[]>
    {
        #region IDataSerializer<float2,byte[]> Members

        public float2 Deserialize(byte[] input)
        {
            var floats = SerializerUtilities.BytesToFloats(input);

            float2 value;
            value.x = floats[0];
            value.y = floats[1];

            return value;
        }

        public byte[] Serialize(float2 value)
        {
            return SerializerUtilities.FloatsToBytes(value.x, value.y);
        }

        #endregion
    }
}