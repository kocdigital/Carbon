using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using IdentityModel.Client;
using System.Security.Authentication;
using System.IO;

namespace Carbon.HttpClient.Auth.IdentityServerSupport
{
    public class AuthHttpClientFactory
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;
        private readonly IdentityServer _identityServer;
        private readonly object _locker;

        private readonly AuthHttpClientAuthorization _authHttpClientAuthorization;
        //Dictionary<string, AuthenticationInfo> Authentications;
        private Dictionary<string, string> _currentSessionHeaders;

        public Dictionary<string, string> CurrentSessionHeaders
        {
            get
            {
                return _currentSessionHeaders;
            }
            set
            {
                _currentSessionHeaders = value;
            }
        }
        public AuthHttpClientFactory()
        {

        }
        public AuthHttpClientFactory(IHttpClientFactory clientFactory, IConfiguration config, AuthHttpClientAuthorization authHttpClientAuthorization)
        {
            _authHttpClientAuthorization = authHttpClientAuthorization;
            _currentSessionHeaders = new Dictionary<string, string>();
            _clientFactory = clientFactory;
            _config = config;
            _identityServer = new IdentityServer()
            {
                AuthorizationUrl = _config.GetSection("JwtSettings")["Authority"] + "/connect/authorize",
                IdentityServerAuthority = _config.GetSection("JwtSettings")["Authority"],
                TokenUrl = _config.GetSection("JwtSettings")["Authority"] + "/connect/token"
            };
        }

        public async Task<AuthenticationInfo> ReAuthenticate(string Name)
        {
            DropAuthentication(Name);
            return await CreateAuthentication(Name);
        }

        public bool DropAuthentication(string Name)
        {
            if (_authHttpClientAuthorization.Authentications.ContainsKey(Name) && _authHttpClientAuthorization.Authentications[Name] != null)
            {
                AuthenticationInfo removal = null;
                _authHttpClientAuthorization.Authentications.TryRemove(Name, out removal);

                if (removal != null)
                    return true;
                else
                    return false;
            }
            return false;
        }
        public async Task<AuthenticationInfo> CreateAuthentication(string Name, bool reauth = false)
        {
            if (!reauth)
                if (_authHttpClientAuthorization.Authentications.ContainsKey(Name) && _authHttpClientAuthorization.Authentications[Name] != null && _authHttpClientAuthorization.Authentications[Name].PrimaryAccessIdExpireDate > DateTime.Now)
                {
                    return _authHttpClientAuthorization.Authentications[Name];
                }

            var relatedSection = _config.GetSection("Authorizations").GetSection(Name);
            if (relatedSection == default(IConfigurationSection) || String.IsNullOrEmpty(relatedSection.GetValue<string>("AuthenticationType")))
                return null;

            var authType = Convert.ToInt16(relatedSection["AuthenticationType"]);
            if (authType == (int)AuthenticationType.BasicAuth)
            {
                Credentials credentials = new Credentials()
                {
                    Password = relatedSection["Password"],
                    Scope = relatedSection["Scope"],
                    Secret = relatedSection["Secret"],
                    UserName = relatedSection["UserName"],
                    ClientId = relatedSection["ClientId"]
                };
                var autherClient = _clientFactory.CreateClient();
                autherClient.DefaultRequestHeaders.Accept.Clear();
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Method = HttpMethod.Post;
                Dictionary<string, string> bodyDict = new Dictionary<string, string>();
                bodyDict.Add("grant_type", "password");
                bodyDict.Add("username", credentials.UserName);
                bodyDict.Add("password", credentials.Password);
                bodyDict.Add("scope", credentials.Scope);
                bodyDict.Add("client_id", credentials.ClientId);
                bodyDict.Add("client_secret", credentials.Secret);
                httpRequestMessage.Content = new FormUrlEncodedContent(bodyDict);
                httpRequestMessage.RequestUri = new Uri(_identityServer.TokenUrl);
                var res = await autherClient.SendAsync(httpRequestMessage);


                if (res.IsSuccessStatusCode)
                {
                    var resStr = await res.Content.ReadAsStringAsync();
                    var daJObj = JObject.Parse(resStr);
                    var accToken = daJObj["access_token"].ToString();
                    var basicAuth = new BasicAuth(accToken, DateTime.Now.AddSeconds((double)daJObj["expires_in"]), Name);
                    _authHttpClientAuthorization.Authentications.AddOrUpdate(Name, basicAuth, (key, auth) => { auth = basicAuth; return basicAuth; });
                    return basicAuth;
                }
                else
                {
                    throw new Exception("Identity server authentication error: " + res.StatusCode);
                }
            }
            else if (authType == (int)AuthenticationType.OAuthClientCredentials)
            {
                var autherClient = _clientFactory.CreateClient();
                autherClient.DefaultRequestHeaders.Accept.Clear();
                var clientCredentials = new ClientCredentialsTokenRequest
                {
                    Address = _identityServer.TokenUrl,
                    ClientId = relatedSection["ClientId"],
                    ClientSecret = relatedSection["Secret"],
                    Scope = relatedSection["Scope"]
                };
                var tokenResponse = await autherClient.RequestClientCredentialsTokenAsync(clientCredentials);

                if (tokenResponse.IsError)
                {
                    throw new Exception("Identity server authentication error: " + tokenResponse.Error);
                }

                var authenticationInfo = new BasicAuth(tokenResponse.AccessToken, DateTime.Now.AddSeconds(tokenResponse.ExpiresIn), Name);

                _authHttpClientAuthorization.Authentications.AddOrUpdate(Name, authenticationInfo, (key, auth) => { auth = authenticationInfo; return authenticationInfo; });

                return authenticationInfo;

            }
            else if (authType == (int)AuthenticationType.OneM2MSpecific)
            {
                var aeid = relatedSection["ApplicationEntityId"];
                var onem2mAuth = new OneM2MAuth(aeid, DateTime.MaxValue, Name);
                onem2mAuth.FQDN = relatedSection["FQDN"];
                _authHttpClientAuthorization.Authentications.AddOrUpdate(Name, onem2mAuth, (key, auth) => { auth = onem2mAuth; return onem2mAuth; });
                return onem2mAuth;
            }
            else if (authType == (int)AuthenticationType.OneM2MConnector)
            {
                var clientId = relatedSection["ClientId"];
                var clientSecret = relatedSection["Secret"];
                var scope = relatedSection["Scope"];

                if (String.IsNullOrEmpty(clientId) || String.IsNullOrEmpty(clientSecret) || String.IsNullOrEmpty(scope))
                {
                    return null;
                }

                var autherClient = _clientFactory.CreateClient();
                autherClient.DefaultRequestHeaders.Accept.Clear();
                var clientCredentials = new ClientCredentialsTokenRequest
                {
                    Address = _identityServer.TokenUrl,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Scope = scope
                };
                var tokenResponse = await autherClient.RequestClientCredentialsTokenAsync(clientCredentials);

                if (tokenResponse.IsError)
                {
                    throw new Exception("Identity server authentication error: " + tokenResponse.Error);
                }
                var authenticationInfo = new BasicAuth(tokenResponse.AccessToken, DateTime.Now.AddSeconds(tokenResponse.ExpiresIn), Name);
                _authHttpClientAuthorization.Authentications.AddOrUpdate(Name, authenticationInfo, (key, auth) => { auth = authenticationInfo; return authenticationInfo; });

                //var aeid = relatedSection["ApplicationEntityId"];
                //var onem2mAuth = new OneM2MConAuth(aeid, DateTime.MaxValue, Name);
                //onem2mAuth.PrimaryAccessId = tokenResponse.AccessToken;
                //onem2mAuth.PrimaryAccessIdExpireDate = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn);

                //onem2mAuth.FQDN = relatedSection["FQDN"];
                //_authHttpClientAuthorization.Authentications.TryAdd(Name, onem2mAuth);
                return authenticationInfo;
            }
            return null;

        }

        public virtual AuthHttpClient CreateAuthClient(string Name)
        {
            string originheader = "";
            var clnt = _clientFactory.CreateClient();
            AuthenticationInfo relatedAuth = null;
            _authHttpClientAuthorization.Authentications.TryGetValue(Name, out relatedAuth);
            if (relatedAuth == null || String.IsNullOrEmpty(Name))
            {
                return new AuthHttpClient() { HttpClient = clnt };
            }

            if (relatedAuth.AuthType == AuthenticationType.BasicAuth)
            {
                clnt.SetBearerToken(relatedAuth.PrimaryAccessId);
                return new AuthHttpClient() { HttpClient = clnt, Name = Name };
            }
            else if (relatedAuth.AuthType == AuthenticationType.OneM2MSpecific)
            {
                originheader = "X-M2M-Origin";
                return SetAuthHttpClientForOneM2m<OneM2MAuth>(clnt, originheader, (IBaseOneM2MAuth)relatedAuth, Name);
            }
            else if (relatedAuth.AuthType == AuthenticationType.OneM2MConnector)
            {
                originheader = "x-from";
                return SetAuthHttpClientForOneM2m<OneM2MConAuth>(clnt, originheader, (IBaseOneM2MAuth)relatedAuth, Name);
            }

            return null;

        }

        private AuthHttpClient SetAuthHttpClientForOneM2m<T>(System.Net.Http.HttpClient clnt, string originheader, IBaseOneM2MAuth relatedAuth, string Name) where T : IBaseOneM2MAuth
        {
            if (!this._currentSessionHeaders.ContainsKey(originheader))
                clnt.DefaultRequestHeaders.TryAddWithoutValidation(originheader, ((AuthenticationInfo)relatedAuth).PrimaryAccessId);
            else
                clnt.DefaultRequestHeaders.TryAddWithoutValidation(originheader, this._currentSessionHeaders[originheader]);

            clnt.DefaultRequestHeaders.TryAddWithoutValidation("x-original-fqdn", ((T)relatedAuth).FQDN);

            foreach (var csh in this._currentSessionHeaders.Where(k => k.Key != originheader && k.Key != "x-original-fqdn").ToList())
            {
                clnt.DefaultRequestHeaders.TryAddWithoutValidation(csh.Key, csh.Value);
            }
            return new AuthHttpClient() { HttpClient = clnt, Name = Name };
        }

        public virtual async Task<HttpResponseMessage> SendAsync(AuthHttpClient client, HttpRequestMessage request)
        {
            var resp = await client.HttpClient.SendAsync(request);
            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                if (client.Name == null) throw new AuthenticationException("Client Name can not be null!");
                //DropAuthentication(client.Name);
                var isReAuthed = await CreateAuthentication(client.Name, true);
                if (isReAuthed != null)
                {
                    client = CreateAuthClient(client.Name);
                    var clonedRequest = await CloneHttpRequestMessageAsync(request);
                    var reResp = await client.HttpClient.SendAsync(clonedRequest);
                    return reResp;
                }
            }
            return resp;
        }

        private static async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage req)
        {
            HttpRequestMessage clone = new HttpRequestMessage(req.Method, req.RequestUri);

            // Copy the request's content (via a MemoryStream) into the cloned object
            var ms = new MemoryStream();
            if (req.Content != null)
            {
                await req.Content.CopyToAsync(ms).ConfigureAwait(false);
                ms.Position = 0;
                clone.Content = new StreamContent(ms);

                // Copy the content headers
                if (req.Content.Headers != null)
                    foreach (var h in req.Content.Headers)
                        clone.Content.Headers.Add(h.Key, h.Value);
            }


            clone.Version = req.Version;

            foreach (KeyValuePair<string, object> prop in req.Properties)
                clone.Properties.Add(prop);

            foreach (KeyValuePair<string, IEnumerable<string>> header in req.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }

        public async Task<AuthenticationInfo> GetAuthentication(string Name)
        {
            if (_authHttpClientAuthorization.Authentications.ContainsKey(Name) && _authHttpClientAuthorization.Authentications[Name] != null && _authHttpClientAuthorization.Authentications[Name].PrimaryAccessIdExpireDate > DateTime.Now)
            {
                return _authHttpClientAuthorization.Authentications[Name];
            }
            await ReAuthenticate(Name);
            return _authHttpClientAuthorization.Authentications[Name];
        }

        public List<string> GetAuthNames()
        {
            return _authHttpClientAuthorization.Authentications.Keys.ToList();
        }

        #region Classes

        public class AuthHttpClient
        {
            public string Name { get; set; }
            public System.Net.Http.HttpClient HttpClient { get; set; }

        }
        public enum AuthenticationType
        {
            BasicAuth = 0,
            OAuth2 = 1,
            OneM2MSpecific = 2,
            OneM2MConnector = 3,
            OAuthClientCredentials = 4
        }

        private class IdentityServer
        {
            public string IdentityServerAuthority { get; set; }
            public string AuthorizationUrl { get; set; }
            public string TokenUrl { get; set; }
        }

        public class AuthenticationInfo
        {
            public AuthenticationInfo()
            {

            }
            public AuthenticationInfo(string PrimaryAccessId, DateTime? PrimaryAccessIdExpireDate, string Name)
            {
                this.PrimaryAccessId = PrimaryAccessId;
                this.PrimaryAccessIdExpireDate = PrimaryAccessIdExpireDate;
                this.Name = Name;
            }
            public AuthenticationType AuthType { get; set; }
            public string PrimaryAccessId { get; set; }
            public DateTime? PrimaryAccessIdExpireDate { get; set; }
            public string Name { get; set; }
            public Guid? TenantId { get; set; }
            public bool IsGodUser { get; set; }
        }

        public class BasicAuth : AuthenticationInfo
        {
            public BasicAuth(string AccessToken, DateTime? AccessTokenExpiryDate, string Name) : base(AccessToken, AccessTokenExpiryDate, Name)
            {
                this.AccessToken = AccessToken;
                this.AccessTokenExpiryDate = AccessTokenExpiryDate;
                base.AuthType = AuthenticationType.BasicAuth;
            }
            public string AccessToken { get; set; }
            public DateTime? AccessTokenExpiryDate { get; set; }
            public string RefreshToken { get; set; }
            public DateTime? RefreshTokenExpiryDate { get; set; }
        }

        public interface IBaseOneM2MAuth
        {
            string ApplicationEntityId { get; set; }
            DateTime? ApplicationEntityIdExpiryDate { get; set; }
            string FQDN { get; set; }
        }

        public class OneM2MAuth : AuthenticationInfo, IBaseOneM2MAuth
        {
            public OneM2MAuth(string ApplicationEntityId, DateTime? ApplicationEntityIdExpiryDate, string Name) : base(ApplicationEntityId, ApplicationEntityIdExpiryDate, Name)
            {
                this.ApplicationEntityId = ApplicationEntityId;
                this.ApplicationEntityIdExpiryDate = ApplicationEntityIdExpiryDate;
                base.AuthType = AuthenticationType.OneM2MSpecific;
            }
            public string ApplicationEntityId { get; set; }
            public DateTime? ApplicationEntityIdExpiryDate { get; set; }
            public string FQDN { get; set; }

        }

        public class OneM2MConAuth : AuthenticationInfo, IBaseOneM2MAuth
        {
            public OneM2MConAuth(string ApplicationEntityId, DateTime? ApplicationEntityIdExpiryDate, string Name)
            {
                this.ApplicationEntityId = ApplicationEntityId;
                this.ApplicationEntityIdExpiryDate = ApplicationEntityIdExpiryDate;
                base.AuthType = AuthenticationType.OneM2MConnector;
                base.Name = Name;
            }
            public string ApplicationEntityId { get; set; }
            public DateTime? ApplicationEntityIdExpiryDate { get; set; }
            public string FQDN { get; set; }


        }

        public class Credentials
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Secret { get; set; }
            public string Scope { get; set; }
            public string ClientId { get; set; }
        }
        #endregion 


    }
}
