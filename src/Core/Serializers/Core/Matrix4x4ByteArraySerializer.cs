using UnityEngine;

namespace Appalachia.Data.Core.Serializers.Core
{
    public class Matrix4x4ByteArraySerializer : IDataSerializer<Matrix4x4, byte[]>
    {
        private const int SIZE = 16;
        
        #region IDataSerializer<Matrix4x4,byte[]> Members

        public Matrix4x4 Deserialize(byte[] input)
        {
            var floats = SerializerUtilities.BytesToFloats(input);

            var value = Matrix4x4.zero;

            for (var i = 0; i < SIZE; i++)
            {
                value[0] = floats[0];
            }
            
            return value;
        }

        public byte[] Serialize(Matrix4x4 value)
        {
            var bytes = new byte[SIZE];
            
            for (var i = 0; i < SIZE; i++)
            {
                bytes[i] = SerializerUtilities.ToByte(value[i]);
            }

            return bytes;
        }

        #endregion
    }
}
