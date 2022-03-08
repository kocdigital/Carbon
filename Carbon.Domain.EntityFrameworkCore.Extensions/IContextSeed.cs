using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Carbon.Domain.EntityFrameworkCore
{
    public interface IContextSeed
    {
        public Task SeedAsync<TContextSeed>(DbContext context,
                                                            ILogger<TContextSeed> logger
                                                           ) where TContextSeed : IContextSeed;


    }
}
