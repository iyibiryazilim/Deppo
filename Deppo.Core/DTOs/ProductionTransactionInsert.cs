using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs
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
