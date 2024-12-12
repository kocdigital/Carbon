using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using ProtoBuf;

namespace Carbon.Caching.Abstractions.Extensions
{
    public static class SerializationExtensions
    {
        public static byte[] ToByteArray(this object obj, CarbonContentSerializationType serializationType = CarbonContentSerializationType.Json)
        {
            if (serializationType == CarbonContentSerializationType.Json)
                return JsonBinarySerializer(obj);
            else if (serializationType == CarbonContentSerializationType.Protobuf)
                return ProtobufSerializer(obj);
            else
                throw new NotImplementedException();
        }
        public static T FromByteArray<T>(this byte[] byteArray, CarbonContentSerializationType serializationType = CarbonContentSerializationType.Json) where T : class
        {
            
            if (serializationType == CarbonContentSerializationType.Json)
                return JsonBinaryDeserializer<T>(byteArray);
            else if (serializationType == CarbonContentSerializationType.Protobuf)
                return ProtobufDeserializer<T>(byteArray);
            else
                throw new NotImplementedException();
        }

        private static T ProtobufDeserializer<T>(byte[] byteArray)
            where T : class
        {
            if (byteArray == null)
            {
                return default(T);
            }
            using (var memoryStream = new MemoryStream(byteArray))
            {
                return Serializer.Deserialize<T>(memoryStream);
            }
        }

        private static byte[] ProtobufSerializer(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            using (var memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        private static T JsonBinaryDeserializer<T>(byte[] byteArray)
            where T : class
        {
            if (byteArray == null)
            {
                return default(T);
            }
            var objAsJsonString = System.Text.Encoding.UTF8.GetString(byteArray);

            try
            {
                var objectReturn = JsonSerializer.Deserialize<T>(objAsJsonString);
                return objectReturn;
            }
            catch (JsonException)
            {
                return default(T);
            }
        }

        private static byte[] JsonBinarySerializer(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var serializedObj = JsonSerializer.Serialize(obj);
            return Encoding.UTF8.GetBytes(serializedObj);
        }
    }
}
