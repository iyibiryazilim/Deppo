using Deppo.Sys.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.DTOs
{
    public class ProcurementFicheTransactionDto
    {
        public Guid Product { get; set; }

        public Guid SubUnitset { get; set; }

        public double Quantity { get; set; } = default;

        public Guid Warehouse { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public int  OrderReferenceId { get; set; }
    }
}
