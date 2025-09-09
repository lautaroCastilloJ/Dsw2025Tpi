using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos.Requests;

public record SetupAdminRequest(
    string UserName,
    string Email,
    string Password,
    string DisplayName
);

