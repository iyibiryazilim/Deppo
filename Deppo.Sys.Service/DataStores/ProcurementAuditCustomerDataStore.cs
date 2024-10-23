using Deppo.Sys.Service.Models;
using Deppo.Sys.Service.Services;

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