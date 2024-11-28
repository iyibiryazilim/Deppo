using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Models
{
    public class ProcessRate
    {
        public Guid Oid { get; set; }
        public string ProcessType { get; set; } = string.Empty;
        public double Rate { get; set; } = default;
    }
}