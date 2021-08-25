

using Carbon.Common;
using HybridModelBinding;
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
        [HybridBindProperty(Source.Header)]
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

        /// <summary>
        /// Is God User Executing the request
        /// </summary>
        [JsonIgnore]
        [HybridBindProperty(Source.Header, "GodUser")]
        public bool IsGodUserExecuting { get; set; }
    }
}
