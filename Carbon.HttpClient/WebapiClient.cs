using System.Net.Http;

namespace Carbon.HttpClients
{
    /// <summary>
    /// Represents Http client of Carbon Framework.
    /// </summary>
    public class WebapiClient
    {
        /// <summary>
        /// Simple constructor of the WebapiClient
        /// </summary>
        /// <seealso cref = "HttpClient" />
        public WebapiClient(HttpClient client)
        {
            Client = client;
        }

        /// <summary>
        /// The client of the WebapiClient
        /// </summary>
        public HttpClient Client { get; }
    }
}
