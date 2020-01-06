using System;

namespace Carbon.Domain.Abstractions.Entities
{
    public interface IUpdateAuditing
    {
        string UpdatedUser { get; set; }
        DateTime? UpdatedDate { get; set; }
    }

}
