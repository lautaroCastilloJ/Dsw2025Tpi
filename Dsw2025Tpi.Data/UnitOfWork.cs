using System.Transactions;
using Dsw2025Tpi.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Data.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly Dsw2025TpiContext _appDb;
    private readonly AuthenticateContext _authDb;

    public UnitOfWork(Dsw2025TpiContext appDb, AuthenticateContext authDb)
    {
        _appDb = appDb;
        _authDb = authDb;
    }

    public async Task ExecuteAsync(Func<Task> action, CancellationToken ct = default)
    {
        var options = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };

        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            options,
            TransactionScopeAsyncFlowOption.Enabled);

        await action();  // se ejecuta la operación completa

        // Guardamos cambios en ambos contextos
        await _authDb.SaveChangesAsync(ct);
        await _appDb.SaveChangesAsync(ct);

        scope.Complete();
    }
}
