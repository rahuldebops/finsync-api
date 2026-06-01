using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using finsyncapi.DAL.IRepositories;
using System.Diagnostics;

namespace finsyncapi.DAL.Repositories
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly ILogger<UnitOfWork<TContext>> _logger;

        public UnitOfWork(TContext context, ILogger<UnitOfWork<TContext>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task RunInTransactionAsync(Func<Task> action)
        {
            var stopwatch = Stopwatch.StartNew();

            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    await action();

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    stopwatch.Stop();

                    _logger.LogInformation(
                        "Transaction committed in {Duration}ms on context {Context}",
                        stopwatch.ElapsedMilliseconds,
                        typeof(TContext).Name);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();

                    try
                    {
                        await transaction.RollbackAsync();
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.LogError(
                            rollbackEx,
                            "Rollback failed on context {Context}",
                            typeof(TContext).Name);
                    }

                    _logger.LogError(
                        ex,
                        "Transaction failed after {Duration}ms on context {Context}",
                        stopwatch.ElapsedMilliseconds,
                        typeof(TContext).Name);

                    throw;
                }
            });
        }

        public async Task<T> RunInTransactionAsync<T>(Func<Task<T>> action)
        {
            var stopwatch = Stopwatch.StartNew();

            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction =
                    await _context.Database.BeginTransactionAsync();

                try
                {
                    var result = await action();

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    stopwatch.Stop();

                    _logger.LogInformation(
                        "Transaction committed in {Duration}ms on context {Context}",
                        stopwatch.ElapsedMilliseconds,
                        typeof(TContext).Name);

                    return result;
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();

                    try
                    {
                        await transaction.RollbackAsync();
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.LogError(
                            rollbackEx,
                            "Rollback failed on context {Context}",
                            typeof(TContext).Name);
                    }

                    _logger.LogError(
                        ex,
                        "Transaction failed after {Duration}ms on context {Context}",
                        stopwatch.ElapsedMilliseconds,
                        typeof(TContext).Name);

                    throw;
                }
            });
        }
    }
}