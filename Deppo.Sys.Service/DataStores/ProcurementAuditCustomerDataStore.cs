using Deppo.Sys.Module.BusinessObjects;
using Deppo.Sys.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.DataStores
{
    public class ProcurementAuditCustomerDataStore : IProcurementAuditCustomerService
    {
        public Task<IEnumerable<ProcurementAuditCustomer>> GetAllAsync(HttpClient httpClient)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProcurementAuditCustomer>> GetAllAsync(HttpClient httpClient, string filter)
        {
            throw new NotImplementedException();
        }
    }
}