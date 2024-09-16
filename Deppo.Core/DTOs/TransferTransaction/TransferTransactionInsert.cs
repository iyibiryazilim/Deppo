using Deppo.Core.DTOs.BaseTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs.TransferTransaction
{
    public class TransferTransactionInsert : BaseTransactionDto
    {
        public TransferTransactionInsert()
        {
            Lines = new List<TransferTransactionLineDto>();
        }
        public int DestinationWarehouseNumber { get; set; }
        public string CurrentCode { get; set; } = string.Empty;
        public List<TransferTransactionLineDto> Lines { get; set; }
    }
}
