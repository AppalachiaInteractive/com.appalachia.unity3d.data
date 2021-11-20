using System;
using UnityEngine;

namespace Appalachia.Data.AccessLayer
{
    public static class UltraLiteDBTypeRegistration
    {
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod]
        public static void RegisterTypes()
        {
            var mapper = BsonMapper.Global;
            
            mapper.RegisterType(Serialize_Vector2, Deserialize_Vector2);
            mapper.RegisterType(Serialize_Vector3, Deserialize_Vector3);
            mapper.RegisterType(Serialize_Vector4, Deserialize_Vector4);
            mapper.RegisterType(Serialize_Quaternion, Deserialize_Quaternion);
            mapper.RegisterType(Serialize_float2, Deserialize_float2);
            mapper.RegisterType(Serialize_float3, Deserialize_float3);
            mapper.RegisterType(Serialize_float4, Deserialize_float4);
            mapper.RegisterType(Serialize_quaternion, Deserialize_quaternion);
        }
    }
}
