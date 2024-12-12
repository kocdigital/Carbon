using System;

namespace Carbon.Caching.Abstractions
{
    public enum CarbonContentSerializationType
    {
        [Obsolete("BinaryFormatter is not supported in new .NET versions. Use JSON serialization instead.", true)]
        BinaryFormatter = 0,
        Json = 1,
        Protobuf = 2
    }
}
