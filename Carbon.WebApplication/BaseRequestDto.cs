

using Carbon.Common;
using HybridModelBinding;
using System;
using System.Text.Json.Serialization;

namespace Carbon.WebApplication
{
    public abstract class BaseRequestDto : IRequestDto
    {
        [JsonIgnore]
        [HybridBindProperty(Source.Header)]
        public Guid CorrelationId { get; set; }
    }
}
