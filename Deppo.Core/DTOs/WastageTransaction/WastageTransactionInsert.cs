using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deppo.Core.DTOs.BaseTransaction;

namespace Deppo.Core.DTOs.WastageTransaction
{
    public class WastageTransactionInsert : BaseTransactionDto
    {
        public WastageTransactionInsert()
        {
            Lines = new List<WastageTransactionLineDto>();
        }
        public List<WastageTransactionLineDto> Lines { get; set; }
    }
}
