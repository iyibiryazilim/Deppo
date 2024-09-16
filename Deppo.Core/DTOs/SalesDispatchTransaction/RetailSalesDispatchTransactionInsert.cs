using Deppo.Core.DTOs.BaseTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs.SalesDispatchTransaction
{
    public class RetailSalesDispatchTransactionInsert : BaseDispatchTransactionDto
    {
        public string? CarrierCode { get; set; }
        public string? DriverFirstName { get; set; }
        public string? DriverLastName { get; set; }
        public string? IdentityNumber { get; set; }
        public string? Plaque { get; set; }
        public string? ShipInfoCode { get; set; }
        public RetailSalesDispatchTransactionInsert()
        {
            Lines = new List<RetailSalesDispatchTransactionLineInsert>();
        }
        public List<RetailSalesDispatchTransactionLineInsert> Lines { get; set; }

       
    }
}
