﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deppo.Core.DTOs.BaseTransaction;

namespace Deppo.Core.DTOs.OutCountingTransaction
{
    public class OutCountingTransactionInsert : BaseTransactionDto
    {
        public OutCountingTransactionInsert()
        {
            Lines = new();
        }
        public List<OutCountingTransactionLineDto> Lines { get; set; }
    }
}
