using Appalachia.Data.Core.Serializers.Core;
using Unity.Mathematics;
using UnityEngine;

namespace Appalachia.Data.Core.Serializers
{
    public class ByteArrayDataSerializer : IDataSerializerSet<byte[]>
    {
        public ByteArrayDataSerializer()
        {
            Vector2 = new Vector2ByteArraySerializer();
            Vector3 = new Vector3ByteArraySerializer();
            Vector4 = new Vector4ByteArraySerializer();
            Quaternion = new QuaternionByteArraySerializer();
            float2 = new float2ByteArraySerializer();
            float3 = new float3ByteArraySerializer();
            float4 = new float4ByteArraySerializer();
            quaternion = new quaternionByteArraySerializer();
            Matrix4x4 = new Matrix4x4ByteArraySerializer();
            float4x4 = new float4x4ByteArraySerializer();
        }

        #region IDataSerializerSet<byte[]> Members

        public IDataSerializer<float4x4, byte[]> float4x4 { get; }
        public IDataSerializer<Matrix4x4, byte[]> Matrix4x4 { get; }

        public IDataSerializer<Vector2, byte[]> Vector2 { get; }
        public IDataSerializer<Vector3, byte[]> Vector3 { get; }
        public IDataSerializer<Vector4, byte[]> Vector4 { get; }
        public IDataSerializer<Quaternion, byte[]> Quaternion { get; }
        public IDataSerializer<float2, byte[]> float2 { get; }
        public IDataSerializer<float3, byte[]> float3 { get; }
        public IDataSerializer<float4, byte[]> float4 { get; }
        public IDataSerializer<quaternion, byte[]> quaternion { get; }

        #endregion
    }
}
