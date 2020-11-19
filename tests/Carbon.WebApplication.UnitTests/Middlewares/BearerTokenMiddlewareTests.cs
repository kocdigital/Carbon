using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Text;
using Carbon.WebApplication.Middlewares;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Carbon.WebApplication.UnitTests.Middlewares
{
     public class BearerTokenMiddlewareTests
    {
        private readonly Mock<RequestDelegate> mockRequestDelegate;

        public BearerTokenMiddlewareTests()
        {
            mockRequestDelegate = new Mock<RequestDelegate>();
        }

        [Fact]
        public void Invoke_NoAuthTokens_ReturnTask()
        {
            // Arrange
            var authTokens = new StringValues(new string[] { });

            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Headers.Add(HeaderNames.Authorization, authTokens);
            mockHttpContext.Request.Headers.Add("TenantId", "mockTenantId");
            mockHttpContext.Request.Headers.Add("ClientId", "mockClientId");

            mockRequestDelegate.Setup(_ => _(It.IsAny<HttpContext>())).Returns<HttpContext>(ctx => Task.FromResult(ctx));

            // Act
            var btm = new BearerTokenMiddleware(mockRequestDelegate.Object);
            var res = (Task<HttpContext>)btm.Invoke(mockHttpContext);
            var resHeaders = res.Result.Request.Headers;

            // Assert
            Assert.Equal(resHeaders, mockHttpContext.Request.Headers);
            mockRequestDelegate.Verify(_ => _(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public void Invoke_NoBearerToken_ReturnTask()
        {
            // Arrange
            var authTokens = new StringValues(new string[] { "authStr1", "authStr2" });

            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Headers.Add(HeaderNames.Authorization, authTokens);
            mockHttpContext.Request.Headers.Add("TenantId", "mockTenantId");
            mockHttpContext.Request.Headers.Add("ClientId", "mockClientId");

            mockRequestDelegate.Setup(_ => _(It.IsAny<HttpContext>())).Returns<HttpContext>(ctx => Task.FromResult(ctx));

            // Act
            var btm = new BearerTokenMiddleware(mockRequestDelegate.Object);
            var res = (Task<HttpContext>)btm.Invoke(mockHttpContext);
            var resHeaders = res.Result.Request.Headers;

            // Assert
            Assert.Equal(resHeaders, mockHttpContext.Request.Headers);
            mockRequestDelegate.Verify(_ => _(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public void Invoke_GodUser_ReturnTask()
        {
            // Arrange
            var testClaims = new List<Claim>()
            {
                new Claim("test claim", "test value"),
                new Claim("god-user", "true")
            };

            var authTokens = new StringValues(new string[] { "authStr1", "authStr2", "Bearer " + MockJwtTokens.GenerateJwtToken(testClaims)});
            
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Headers.Add(HeaderNames.Authorization, authTokens);
            mockHttpContext.Request.Headers.Add("TenantId", "mockTenantId");
            mockHttpContext.Request.Headers.Add("ClientId", "mockClientId");

            mockRequestDelegate.Setup(_ => _(It.IsAny<HttpContext>())).Returns<HttpContext>(ctx => Task.FromResult(ctx));

            // Act
            var btm = new BearerTokenMiddleware(mockRequestDelegate.Object);
            var res = (Task<HttpContext>)btm.Invoke(mockHttpContext);
            var resHeaders = res.Result.Request.Headers;

            // Assert
            Assert.True(resHeaders.ContainsKey("TenantId"));
            Assert.False(resHeaders.ContainsKey("ClientId"));
            mockRequestDelegate.Verify(_ => _(It.IsAny<HttpContext>()), Times.Once);

            string tmpStr = null;
            if(BearerTokenClaimMapper.TryGetValue("god-user", out tmpStr))
            {
                Assert.True(resHeaders.ContainsKey(tmpStr));
            }
        }

        [Fact]
        public void Invoke_StandardContext_ReturnTask()
        {
            // Arrange
            var testClaims = new List<Claim>()
            {
                new Claim("test claim", "test value"),
            };

            var authTokens = new StringValues(new string[] { "authStr1", "authStr2", "Bearer " + MockJwtTokens.GenerateJwtToken(testClaims) });

            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Request.Headers.Add(HeaderNames.Authorization, authTokens);
            mockHttpContext.Request.Headers.Add("TenantId", "mockTenantId");
            mockHttpContext.Request.Headers.Add("ClientId", "mockClientId");

            mockRequestDelegate.Setup(_ => _(It.IsAny<HttpContext>())).Returns<HttpContext>(ctx => Task.FromResult(ctx));

            // Act
            var btm = new BearerTokenMiddleware(mockRequestDelegate.Object);
            var res = (Task<HttpContext>)btm.Invoke(mockHttpContext);
            var resHeaders = res.Result.Request.Headers;

            // Assert
            Assert.False(resHeaders.ContainsKey("TenantId"));
            Assert.False(resHeaders.ContainsKey("ClientId"));
            mockRequestDelegate.Verify(_ => _(It.IsAny<HttpContext>()), Times.Once);
        }

        protected static class MockJwtTokens
        {
            public static string Issuer { get; } = Guid.NewGuid().ToString();
            public static SecurityKey SecurityKey { get; }
            public static SigningCredentials SigningCredentials { get; }

            private static readonly JwtSecurityTokenHandler s_tokenHandler = new JwtSecurityTokenHandler();
            private static readonly RandomNumberGenerator s_rng = RandomNumberGenerator.Create();
            private static readonly byte[] s_key = new byte[32];

            static MockJwtTokens()
            {
                s_rng.GetBytes(s_key);
                SecurityKey = new SymmetricSecurityKey(s_key) { KeyId = Guid.NewGuid().ToString() };
                SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            }

            public static string GenerateJwtToken(IEnumerable<Claim> claims)
            {
                return s_tokenHandler.WriteToken(new JwtSecurityToken(Issuer, null, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
            }
        }
    }
}
