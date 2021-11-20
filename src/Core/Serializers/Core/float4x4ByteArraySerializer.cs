using Unity.Mathematics;

namespace Appalachia.Data.Core.Serializers.Core
{
    public class float4x4ByteArraySerializer : IDataSerializer<float4x4, byte[]>
    {
        private const int SIZEX = 4;
        private const int SIZEY = 4;
        
        #region IDataSerializer<float4x4,byte[]> Members

        public float4x4 Deserialize(byte[] input)
        {
            var floats = SerializerUtilities.BytesToFloats(input);

            var value = float4x4.zero;

            for (var x = 0; x < SIZEX; x++)
            {
                var xOffset = SIZEX * x;
                
                for (var y = 0; y < SIZEY; y++)
                {
                    var index = xOffset + y;
                    
                    value[x][y] = floats[index];                    
                }
            }
            
            return value;
        }

        public byte[] Serialize(float4x4 value)
        {
            var bytes = new byte[SIZEX*SIZEY];
            
            for (var x = 0; x < SIZEX; x++)
            {
                var xOffset = SIZEX * x;
                
                for (var y = 0; y < SIZEY; y++)
                {
                    var index = xOffset + y;

                    var current = value[x][y];

                    bytes[index] = SerializerUtilities.ToByte(current);
                }
            }
            
            return bytes;
        }

        #endregion
    }
}
