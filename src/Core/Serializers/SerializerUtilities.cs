using System;

namespace Appalachia.Data.Core.Serializers
{
    public static class SerializerUtilities
    {
        public static float ToFloat(byte value)
        {
            return Convert.ToSingle(value);
        }
        
        public static byte ToByte(float value)
        {
            return Convert.ToByte(value);
        }
        
        public static byte[] FloatsToBytes(params float[] args)
        {
            var byteArray = new byte[args.Length * 4];

            Buffer.BlockCopy(args, 0, byteArray, 0, byteArray.Length);

            return byteArray;
        }

        public static float[] BytesToFloats(byte[] args)
        {
            var floatArray = new float[args.Length / 4];
            
            Buffer.BlockCopy(args, 0, floatArray, 0, args.Length);

            return floatArray;
        }
    }
}
