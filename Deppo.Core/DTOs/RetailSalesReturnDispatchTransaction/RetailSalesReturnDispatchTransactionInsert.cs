using Deppo.Core.DTOs.BaseTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs.RetailSalesReturnDispatchTransaction;

public class RetailSalesReturnDispatchTransactionInsert : BaseDispatchTransactionDto
{
    public RetailSalesReturnDispatchTransactionInsert()
    {
        Lines = new List<RetailSalesReturnDispatchTransactionLineInsert>();
    }
    public List<RetailSalesReturnDispatchTransactionLineInsert> Lines { get; set; }
}
