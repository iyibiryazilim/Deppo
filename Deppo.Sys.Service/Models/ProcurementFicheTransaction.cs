using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Models
{
    public class ProcurementFicheTransaction
    {
        public Product? Product { get; set; }
        public SubUnitset? SubUnitset { get; set; }

        public double Quantity { get; set; } = default;

        public Warehouse? Warehouse { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public int OrderReferenceId { get; set; }

        public ProcurementFiche? ProcurementFiche { get; set; }

    }
}
