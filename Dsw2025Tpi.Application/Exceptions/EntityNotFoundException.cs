namespace Dsw2025Tpi.Application.Exceptions;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string entity, object key)
        : base($"{entity} con identificador '{key}' no encontrado.")
    {
    }
}
