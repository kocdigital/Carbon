using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Carbon.Caching.Abstractions.Extensions
{
    public static class SerializationExtensions
    {
        [Obsolete("This conversion is deprecated as of dotnet 5 unless you demand especially in your csproj")]
        public static byte[] ToByteArray(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }
        [Obsolete("This conversion is deprecated as of dotnet 5 unless you demand especially in your csproj")]
        public static T FromByteArray<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
            {
                return default(T);
            }
            var binaryFormatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream(byteArray))
            {
                return binaryFormatter.Deserialize(memoryStream) as T;
            }
        }

    }
}
