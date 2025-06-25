namespace Dsw2025Tpi.Domain.Common;

public abstract class EntityBase
{
    protected EntityBase()
    {
        Id = Guid.NewGuid();
    }
    public Guid Id { get; }
}
