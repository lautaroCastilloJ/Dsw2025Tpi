namespace Dsw2025Tpi.Application.Interfaces;

public interface IUnitOfWork
{
    Task ExecuteAsync(Func<Task> action, CancellationToken ct = default);
}
