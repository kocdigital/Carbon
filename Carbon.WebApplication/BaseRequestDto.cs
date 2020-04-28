

using Carbon.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json.Serialization;

namespace Carbon.WebApplication
{
    public abstract class BaseRequestDto : IRequestDto
    {
        [JsonIgnore]
        [FromHeader]
        public string TenantId { get; set; }

        [JsonIgnore]
        [FromHeader]
        public string ClientId { get; set; }

        [JsonIgnore]
        [FromHeader]
        public Guid CorrelationId { get; set; }
    }
}
