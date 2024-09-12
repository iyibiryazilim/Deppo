using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Deppo.Core.DTOs.BaseTransaction;

namespace Deppo.Core.DTOs.ProductionTransaction
{
    public class ProductionTransactionInsert : BaseTransactionDto
    {
        public List<ProductionTransactionLineDto> Lines { get; set; }

        public ProductionTransactionInsert()
        {
            Lines = new List<ProductionTransactionLineDto>();
        }
    }
}
