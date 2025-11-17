using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos.Users;

public record RegisterRequest(
    string UserName, 
    string Password, 
    string Email, 
    string DisplayName, 
    string? Role
);
