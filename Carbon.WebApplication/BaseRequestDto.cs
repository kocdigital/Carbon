

using Carbon.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json.Serialization;

namespace Carbon.WebApplication
{
    /// <summary>
    /// Base class of any Carbon request dto. 
    /// </summary>
    public abstract class BaseRequestDto : IRequestDto
    {
        /// <summary>
        /// Tenant Id of the request
        /// </summary>
        [JsonIgnore]
        [FromHeader]
        public Guid TenantId { get; set; }

        /// <summary>
        /// Client Id of the request
        /// </summary>
        [JsonIgnore]
        [FromHeader]
        public string ClientId { get; set; }

        /// <summary>
        /// Transaction Id of the request
        /// </summary>
        [JsonIgnore]
        [FromHeader]
        public Guid CorrelationId { get; set; }
    }
}
