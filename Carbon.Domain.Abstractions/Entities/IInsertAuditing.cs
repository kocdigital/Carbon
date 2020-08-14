using System;

namespace Carbon.Domain.Abstractions.Entities
{
    public interface IInsertAuditing
    {
        string InsertedUser { get; set; }
        DateTime? InsertedDate { get; set; }
    }

}
