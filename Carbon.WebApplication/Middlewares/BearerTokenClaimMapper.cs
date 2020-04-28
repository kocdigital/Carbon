using System.Collections.Generic;

namespace Carbon.WebApplication.Middlewares
{
    public static class BearerTokenClaimMapper
    {
        private static IDictionary<string, string> ValuePairs = new Dictionary<string, string>();

        static BearerTokenClaimMapper()
        {
            ValuePairs.Add("sub", "ClientId");
            ValuePairs.Add("tenant-id", "TenantId");
        }

        public static bool TryGetValue(string key, out string mappedKey)
        {
            return ValuePairs.TryGetValue(key, out mappedKey);
        }
    }
}
