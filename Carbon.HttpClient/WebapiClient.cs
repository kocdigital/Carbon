using System.Net.Http;

namespace Carbon.HttpClients
{
    public class WebapiClient
    {
        public WebapiClient(HttpClient client)
        {
            Client = client;
        }

        public HttpClient Client { get; }
    }
}
