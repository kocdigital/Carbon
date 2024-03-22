using Carbon.Domain.EntityFrameworkCore.CentralizedAudit.Models;


namespace Carbon.Domain.EntityFrameworkCore.CentralizedAudit.Contracts
{
    public interface IContextConfiguration
    {
        DataStoreType Type { get; }
    }
}
