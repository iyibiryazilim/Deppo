using Deppo.Sys.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Services
{
    public interface IProcurementAuditCustomerService
    {
        public Task<IEnumerable<ProcurementAuditCustomer>> GetAllAsync(HttpClient httpClient);

        public Task<IEnumerable<ProcurementAuditCustomer>> GetAllAsync(HttpClient httpClient, string filter);
    }
}