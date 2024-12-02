using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Models
{
    public class SubUnitset
    {
        public Guid Oid { get; set; }
        public int ReferenceId { get; set; }

        public int UnitsetReferenceId { get; set; }

        public Unitset Unitset { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public double ConversionFactor { get; set; } = default;

        public double OtherConversionFactor { get; set; } = default;

        public int FirmNumber { get; set; }
        
    }
}
