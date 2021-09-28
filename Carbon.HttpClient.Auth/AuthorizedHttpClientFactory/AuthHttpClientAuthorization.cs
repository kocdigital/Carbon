using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using static Carbon.HttpClient.Auth.IdentityServerSupport.AuthHttpClientFactory;

namespace Carbon.HttpClient.Auth.IdentityServerSupport
{
    public class AuthHttpClientAuthorization
    {
        public AuthHttpClientAuthorization()
        {
            Authentications = new ConcurrentDictionary<string, AuthenticationInfo>();
        }
        public ConcurrentDictionary<string, AuthenticationInfo> Authentications { get; set; }

    }
}
