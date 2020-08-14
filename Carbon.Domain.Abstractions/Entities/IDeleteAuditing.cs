using System;

namespace Carbon.Domain.Abstractions.Entities
{
    public interface IDeleteAuditing
    {
        string DeletedUser { get; set; }
        DateTime? DeletedDate { get; set; }
    }

}
