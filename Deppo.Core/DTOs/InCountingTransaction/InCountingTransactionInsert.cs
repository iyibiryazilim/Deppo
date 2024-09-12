using Deppo.Core.BaseModels;
using Deppo.Core.DTOs.BaseTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs.InCountingTransaction
{
    public class InCountingTransactionInsert : BaseTransactionDto
    {
        public InCountingTransactionInsert()
        {
            Lines = new();
        }
        public List<InCountingTransactionLineDto> Lines { get; set; }
    }
}
