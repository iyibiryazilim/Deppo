using Deppo.Core.DTOs.BaseTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs.PurchaseReturnDispatchTransaction;

public class PurchaseReturnDispatchTransactionInsert : BaseDispatchTransactionDto
{
    public PurchaseReturnDispatchTransactionInsert()
    {
        Lines = new List<PurchaseReturnDispatchTransactionLineDto>();
    }

    public List<PurchaseReturnDispatchTransactionLineDto> Lines { get; set; }
}