/*
using System;
using UltraLiteDB;
using Unity.Mathematics;
using UnityEngine;

namespace Appalachia.Prototype.KOC.Data.AccessLayer
{
    public static class LiteDBTypeRegistration
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

        private static BsonValue Serialize_Vector2(Vector2 value)
        {
            var bytes = FloatsToBytes(value.x, value.y);

            return new BsonValue(bytes);
        }

        private static BsonValue Serialize_Vector3(Vector3 value)
        {
            var bytes = FloatsToBytes(value.x, value.y, value.z);

            return new BsonValue(bytes);
        }

        private static BsonValue Serialize_Vector4(Vector4 value)
        {
            var bytes = FloatsToBytes(value.x, value.y, value.z, value.w);

            return new BsonValue(bytes);
        }

        private static BsonValue Serialize_Quaternion(Quaternion value)
        {
            var bytes = FloatsToBytes(value.x, value.y, value.z, value.w);

            return new BsonValue(bytes);
        }

        private static BsonValue Serialize_float2(float2 value)
        {
            var bytes = FloatsToBytes(value.x, value.y);

            return new BsonValue(bytes);
        }

        private static BsonValue Serialize_float3(float3 value)
        {
            var bytes = FloatsToBytes(value.x, value.y, value.z);

            return new BsonValue(bytes);
        }

        private static BsonValue Serialize_float4(float4 value)
        {
            var bytes = FloatsToBytes(value.x, value.y, value.z, value.w);

            return new BsonValue(bytes);
        }

        private static BsonValue Serialize_quaternion(quaternion value)
        {
            var bytes = FloatsToBytes(value.value.x, value.value.y, value.value.z, value.value.w);

            return new BsonValue(bytes);
        }

        private static Vector2 Deserialize_Vector2(BsonValue input)
        {
            var floats = BytesToFloats(input);
            
            Vector2 value;
            value.x = floats[0];
            value.y = floats[1];

            return value;
        }

        private static Vector3 Deserialize_Vector3(BsonValue input)
        {
            var floats = BytesToFloats(input);
            
            Vector3 value;
            value.x = floats[0];
            value.y = floats[1];
            value.z = floats[2];

            return value;
        }

        private static Vector4 Deserialize_Vector4(BsonValue input)
        {
            var floats = BytesToFloats(input);
            
            Vector4 value;
            value.x = floats[0];
            value.y = floats[1];
            value.z = floats[2];
            value.w = floats[3];

            return value;
        }

        private static Quaternion Deserialize_Quaternion(BsonValue input)
        {
            var floats = BytesToFloats(input);
            
            Quaternion value;
            value.x = floats[0];
            value.y = floats[1];
            value.z = floats[2];
            value.w = floats[3];

            return value;
        }

        private static float2 Deserialize_float2(BsonValue input)
        {
            var floats = BytesToFloats(input);
            
            float2 value;
            value.x = floats[0];
            value.y = floats[1];

            return value;
        }

        private static float3 Deserialize_float3(BsonValue input)
        {
            var floats = BytesToFloats(input);
            
            float3 value;
            value.x = floats[0];
            value.y = floats[1];
            value.z = floats[2];

            return value;
        }

        private static float4 Deserialize_float4(BsonValue input)
        {
            var floats = BytesToFloats(input);
            
            float4 value;
            value.x = floats[0];
            value.y = floats[1];
            value.z = floats[2];
            value.w = floats[3];

            return value;
        }

        private static quaternion Deserialize_quaternion(BsonValue input)
        {
            var floats = BytesToFloats(input);
            
            quaternion value;
            value.value.x = floats[0];
            value.value.y = floats[1];
            value.value.z = floats[2];
            value.value.w = floats[3];

            return value;
        }

        private static byte[] FloatsToBytes(params float[] args)
        {
            var byteArray = new byte[args.Length * 4];

            Buffer.BlockCopy(args, 0, byteArray, 0, byteArray.Length);

            return byteArray;
        }

        private static float[] BytesToFloats(byte[] args)
        {
            var floatArray = new float[args.Length / 4];
            
            Buffer.BlockCopy(args, 0, floatArray, 0, args.Length);

            return floatArray;
        }
    }
}
*/
