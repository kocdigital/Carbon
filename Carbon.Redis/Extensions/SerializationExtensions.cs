using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.InteropServices;
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

        internal static StructureType FromByteArraySafe<StructureType>(this byte[] byteArray)
    where StructureType : class
        {
            using (var memoryStream = new MemoryStream(byteArray))
            {
                int Length = Marshal.SizeOf(typeof(StructureType));
                byte[] Bytes = new byte[Length];
                memoryStream.Read(Bytes, 0, Length);
                IntPtr Handle = Marshal.AllocHGlobal(Length);
                Marshal.Copy(Bytes, 0, Handle, Length);
                StructureType Result = (StructureType)Marshal.PtrToStructure(Handle, typeof(StructureType));
                Marshal.FreeHGlobal(Handle);
                return Result;
            }
        }

        internal static byte[] ToByteArraySafe(this object Structure)
        {
            using (var memoryStream = new MemoryStream())
            {
                int Length = Marshal.SizeOf(Structure);
                byte[] Bytes = new byte[Length];
                IntPtr Handle = Marshal.AllocHGlobal(Length);
                Marshal.StructureToPtr(Structure, Handle, true);
                Marshal.Copy(Handle, Bytes, 0, Length);
                Marshal.FreeHGlobal(Handle);
                memoryStream.Write(Bytes, 0, Length);
                return Bytes;
            }
        }


        public static string ToJson(this object obj)
        {
            if (obj == null)
                return default(string);

            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJson<T>(this string jsonString) where T : class
        {
            if (jsonString == null)
                return default(T);

            return JsonConvert.DeserializeObject<T>(jsonString);
        }

    }
}
