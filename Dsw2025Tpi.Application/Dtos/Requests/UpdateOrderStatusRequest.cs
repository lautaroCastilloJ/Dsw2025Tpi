using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos.Requests
{
    public class UpdateOrderStatusRequest
    {
        public string NewStatus { get; set; } = null!;
    }
}
