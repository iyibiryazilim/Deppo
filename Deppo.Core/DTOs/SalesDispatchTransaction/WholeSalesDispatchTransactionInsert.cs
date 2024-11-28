using Deppo.Core.DTOs.BaseTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs.SalesDispatchTransaction;

public class WholeSalesDispatchTransactionInsert : BaseDispatchTransactionDto
{
    public WholeSalesDispatchTransactionInsert()
    {
        Lines = new List<WholeSalesDispatchTransactionLineInsert>();
    }

    public List<WholeSalesDispatchTransactionLineInsert> Lines { get; set; }
}