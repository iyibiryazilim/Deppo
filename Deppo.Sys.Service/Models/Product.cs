using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Models
{
    public class Product
    {
        public Guid Oid { get; set; }
        public int ReferenceId { get; set; }

        public string Code { get; set; }=string.Empty;

        public string Name { get; set; } = string.Empty;

        public Unitset? Unitset { get; set; }

        public int UnitsetReferenceId { get; set; }

        public double Vat { get; set; } = default;

        public string Group { get; set; } = string.Empty;

        public string SpeCode { get; set; } = string.Empty;

        public string SpeCOde2 { get; set; } = string.Empty;
        public string SpeCOde3 { get; set; } = string.Empty;
        public string SpeCOde4 { get; set; } = string.Empty;
        public string SpeCOde5 { get; set; } = string.Empty;
        public int FirmNumber { get; set; } 

        



    }
}
