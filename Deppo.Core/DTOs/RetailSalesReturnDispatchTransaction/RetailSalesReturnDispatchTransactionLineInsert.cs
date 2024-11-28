using Deppo.Core.DTOs.BaseTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs.RetailSalesReturnDispatchTransaction;

public class RetailSalesReturnDispatchTransactionLineInsert : BaseDispatchTransactionLineDto
{
    public int? DispatchReferenceId { get; set; }
}
