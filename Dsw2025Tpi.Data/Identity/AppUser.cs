using Microsoft.AspNetCore.Identity;

namespace Dsw2025Tpi.Data.Identity;

public class AppUser : IdentityUser
{
    public string? DisplayName { get; set; }

    // Campos adicionales que se podrían sumar:

    // public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // public bool IsActive { get; set; } = true;
    // public ICollection<RefreshToken>? RefreshTokens { get; set; } = [];
    // public Guid CustomerId { get; set; }

}
