using Dsw2025Tpi.Domain.Common;
using System;

namespace Dsw2025Tpi.Domain.Entities;

public class Customer : EntityBase
{
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }

    // Relacion: 1 cliente realiza muchas ordenes
    public ICollection<Order> Orders { get; set; }
}
