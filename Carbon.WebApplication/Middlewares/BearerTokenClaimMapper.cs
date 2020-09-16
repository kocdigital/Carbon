using System.Collections.Generic;

namespace Carbon.WebApplication.Middlewares
{
    public static class BearerTokenClaimMapper
    {
        /// <summary>
        /// 	A dictionary which keeps bearer token claims
        /// </summary>
        private static IDictionary<string, string> ValuePairs = new Dictionary<string, string>();

        /// <summary>
        /// 	Constructor that initializes the token claims/>.
        /// </summary>

        static BearerTokenClaimMapper()
        {
            ValuePairs.Add("sub", "ClientId");
            ValuePairs.Add("tenant-id", "TenantId");
            ValuePairs.Add("god-user", "GodUser");

        }

        /// <summary>
        /// Gets the value of the given token claim key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mappedKey"></param>
        /// <returns></returns>
        public static bool TryGetValue(string key, out string mappedKey)
        {
            return ValuePairs.TryGetValue(key, out mappedKey);
        }
    }
}
