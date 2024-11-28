using Deppo.Core.DTOs.BaseTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs.WholeSalesReturnDispatchTransaction;

public class WholeSalesReturnTransactionLineInsert : BaseDispatchTransactionLineDto
{
    public int? DispatchReferenceId { get; set; }

}
