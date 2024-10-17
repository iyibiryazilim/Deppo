using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Models;

public class TransactionAudit
{
    public Guid Oid { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string TransactionNumber { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;

    public int? CurrentReferenceId { get; set; }
    public string CurrentName { get; set; } = string.Empty;
    public string CurrentCode { get; set; } = string.Empty;

    public int? ShipAddressReferenceId { get; set; }
    public string ShipAdressName { get; set; } = string.Empty;
    public string ShipAddressCode { get; set; } = string.Empty;

    public string WarehouseNumber { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;

    public int? ProductReferenceCount { get; set; }
}