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
		public string? CarrierCode { get; set; } = string.Empty;
		public string? DriverFirstName { get; set; } = string.Empty;
		public string? DriverLastName { get; set; } = string.Empty;
		public string? IdentityNumber { get; set; } = string.Empty;
		public string? Plaque { get; set; } = string.Empty;
		public int? IsEDispatch { get; set; }
		public string? ShipInfoCode { get; set; } = string.Empty;
        public List<TransferTransactionLineDto> Lines { get; set; }
    }
}
