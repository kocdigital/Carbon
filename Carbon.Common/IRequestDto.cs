using System;

namespace Carbon.Common
{
    public interface IRequestDto
    {
        public Guid CorrelationId { get; set; }
    }
}
