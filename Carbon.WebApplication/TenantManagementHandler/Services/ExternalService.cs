using Carbon.Common.TenantManagementHandler.Classes;
using Carbon.WebApplication.TenantManagementHandler.Dtos;
using Carbon.WebApplication.TenantManagementHandler.Dtos.ErrorHandling;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.WebApplication.TenantManagementHandler.Services
{
    /// <inheritdoc cref="IExternalService"/>
    
    public class ExternalService : IExternalService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;
        private readonly string _policyv2;
        private readonly string _errorHandling;

        public ExternalService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _policyv2 = _config.GetValue<string>("PolicyServerUrlv2");
            _errorHandling = _config.GetValue<string>("ErrorHandling:Url");
            _clientFactory = httpClientFactory;
        }

		public async Task<List<PermissionDetailedDto>> ExecuteInPolicyApi_GetRoles(PermissionDetailedFilterDto permissionDetailedFilterDto, string token)
        {

            var clientForPolicy = _clientFactory.CreateClient();
            clientForPolicy.DefaultRequestHeaders.Accept.Clear();
            clientForPolicy.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage httpRequestMessageToPolicyv2 = new HttpRequestMessage();

            if (!String.IsNullOrEmpty(token))
                httpRequestMessageToPolicyv2.SetBearerToken(token);


            var uribuilder = new UriBuilder(_policyv2 + "policies/endpoint-item-permissions-with-policies/" + permissionDetailedFilterDto.UserId);
            string daUrlRuleLog = uribuilder.ToString();

            httpRequestMessageToPolicyv2.RequestUri = new Uri(daUrlRuleLog);
            var jStr = JsonConvert.SerializeObject(permissionDetailedFilterDto);
            httpRequestMessageToPolicyv2.Content = new StringContent(jStr, Encoding.UTF8, "application/json");
            httpRequestMessageToPolicyv2.Method = HttpMethod.Get;
            var resp = await clientForPolicy.SendAsync(httpRequestMessageToPolicyv2);
            var respStr = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode)
            {
                JObject respJObj = JObject.Parse(respStr);
                var respObj = respJObj["Data"].ToObject<List<PermissionDetailedDto>>();
                return respObj;
            }
            return null;
        }

        public async Task<ErrorResponse> GetErrorDescription(ApplicationErrorRequest request, string token = null)
        {
            var clientForPolicy = _clientFactory.CreateClient();
            clientForPolicy.DefaultRequestHeaders.Accept.Clear();
            clientForPolicy.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage httpRequest = new HttpRequestMessage();

            if (!String.IsNullOrEmpty(token))
                httpRequest.SetBearerToken(token);


            var uribuilder = new UriBuilder(_errorHandling + "api/v1/error/get-error-detail");
            string url = uribuilder.ToString();

            httpRequest.RequestUri = new Uri(url);
            var jStr = JsonConvert.SerializeObject(request);
            httpRequest.Content = new StringContent(jStr, Encoding.UTF8, "application/json");
            httpRequest.Method = HttpMethod.Post;
            var resp = await clientForPolicy.SendAsync(httpRequest);
            var respStr = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode)
            {
                if (resp.StatusCode == System.Net.HttpStatusCode.NoContent) return null;
                JObject respJObj = JObject.Parse(respStr);
                var respObj = respJObj.ToObject<ErrorResponse>();
                return respObj;
            }
            return null;
        }

		public async Task<bool> RegisterApplicationError(ApplicationErrorRegisterRequest request, string token = null)
        {
            var clientForPolicy = _clientFactory.CreateClient();
            clientForPolicy.DefaultRequestHeaders.Accept.Clear();
            clientForPolicy.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpRequestMessage httpRequest = new HttpRequestMessage();

            if (!String.IsNullOrEmpty(token))
                httpRequest.SetBearerToken(token);


            var uribuilder = new UriBuilder(_errorHandling + "api/v1/error/register-application-error");
            string url = uribuilder.ToString();

            httpRequest.RequestUri = new Uri(url);
            var jStr = JsonConvert.SerializeObject(request);
            httpRequest.Content = new StringContent(jStr, Encoding.UTF8, "application/json");
            httpRequest.Method = HttpMethod.Post;
            var resp = await clientForPolicy.SendAsync(httpRequest);
            if (resp.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
    }
}
