using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deppo.Core.DTOs.BaseTransaction;

namespace Deppo.Core.DTOs.ConsumableTransaction
{
    public class ConsumableTransactionInsert : BaseTransactionDto
    {
        public ConsumableTransactionInsert()
        {
            Lines = new();
        }
        public List<ConsumableTransactionLineDto> Lines { get; set; }
    }
}
