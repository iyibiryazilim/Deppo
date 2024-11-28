using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Models;

public class ProcurementAudit
{
    public Guid ApplicationUser { get; set; }
    public DateTime CreatedOn { get; set; }

    public int ProductReferenceId { get; set; } = default;

    public string ProductName { get; set; } = string.Empty;

    public bool IsVariant { get; set; }

    public double Quantity { get; set; } = default;

    public ReasonsForRejectionProcurement? ReasonsForRejectionProcurement { get; set; }

    public double ProcurementQuantity { get; set; } = default;

    public int WarehouseNumber { get; set; } = default;

    public string WarehouseName { get; set; } = string.Empty;

    public int LocationReferenceId { get; set; } = default;

    public string LocationCode { get; set; } = string.Empty;

    public string LocationName { get; set; } = string.Empty;
}