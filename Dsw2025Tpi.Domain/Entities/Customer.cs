using Dsw2025Tpi.Domain.Common;

namespace Dsw2025Tpi.Domain.Entities;

public class Customer : EntityBase
{
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }

    public Customer() { }

    public static Customer Create(string email, string name, string phone)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email requerido");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nombre requerido");
        return new Customer { Email = email, Name = name, PhoneNumber = phone };
    }
}