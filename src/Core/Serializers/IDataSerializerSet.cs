using Unity.Mathematics;
using UnityEngine;

namespace Appalachia.Data.Core.Serializers
{
    public interface IDataSerializerSet<TOut>
    {
        public IDataSerializer<float2, TOut> float2 { get; }
        public IDataSerializer<float3, TOut> float3 { get; }
        public IDataSerializer<float4, TOut> float4 { get; }
        
        public IDataSerializer<float4x4, TOut> float4x4 { get; }
        public IDataSerializer<Matrix4x4, TOut> Matrix4x4 { get; }
        public IDataSerializer<quaternion, TOut> quaternion { get; }
        public IDataSerializer<Quaternion, TOut> Quaternion { get; }
        public IDataSerializer<Vector2, TOut> Vector2 { get; }
        public IDataSerializer<Vector3, TOut> Vector3 { get; }
        public IDataSerializer<Vector4, TOut> Vector4 { get; }
    }
}
