using Deppo.Core.DTOs.BaseTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs.PurchaseReturnDispatchTransaction;

public class PurchaseReturnDispatchTransactionInsert : BaseDispatchTransactionDto
{
    public string? CarrierCode { get; set; }
    public string? DriverFirstName { get; set; }
    public string? DriverLastName { get; set; }
    public string? IdentityNumber { get; set; }
    public string? Plaque { get; set; }
    public PurchaseReturnDispatchTransactionInsert()
    {
        Lines = new List<PurchaseReturnDispatchTransactionLineDto>();
    }
    public List<PurchaseReturnDispatchTransactionLineDto> Lines { get; set; }
}
