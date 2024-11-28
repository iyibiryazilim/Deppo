﻿using Deppo.Core.DTOs.BaseTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs.PurchaseDispatchTransaction;

public class PurchaseDispatchTransactionInsert : BaseDispatchTransactionDto
{
    public PurchaseDispatchTransactionInsert()
    {
        Lines = new List<PurchaseDispatchTransactionLineDto>();
    }
    public IList<PurchaseDispatchTransactionLineDto> Lines { get; set; }
}
