using Microsoft.EntityFrameworkCore;

namespace finsyncapi.DAL.IRepositories
{
    public interface IUnitOfWork<TContext> where TContext : DbContext
    {
        Task RunInTransactionAsync(Func<Task> action);
        Task<T> RunInTransactionAsync<T>(Func<Task<T>> action);
    }

}
