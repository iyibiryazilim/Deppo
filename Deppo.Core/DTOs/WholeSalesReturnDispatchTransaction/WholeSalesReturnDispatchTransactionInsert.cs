using Deppo.Core.DTOs.BaseTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs.WholeSalesReturnDispatchTransaction;

public class WholeSalesReturnDispatchTransactionInsert : BaseDispatchTransactionDto
{
    public WholeSalesReturnDispatchTransactionInsert()
    {
        Lines = new List<WholeSalesReturnTransactionLineInsert>();
    }

    public List<WholeSalesReturnTransactionLineInsert> Lines { get; set; }
}