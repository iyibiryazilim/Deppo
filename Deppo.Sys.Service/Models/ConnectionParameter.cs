using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Models
{
    public class ConnectionParameter
    {
        public string GatewayUri { get; set; } = string.Empty;
        public string GatewayPort { get; set; } = string.Empty;
        public string ExternalDatabase { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}