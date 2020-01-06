using System.Threading.Tasks;

namespace Carbon.Domain.Abstractions.UOW
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
