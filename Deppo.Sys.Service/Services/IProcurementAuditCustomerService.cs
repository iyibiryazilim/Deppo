using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Models;

namespace Deppo.Sys.Service.Services
{
    public interface IProcurementAuditCustomerService
    {
        public Task<IEnumerable<ProcurementAuditCustomer>> GetAllAsync(HttpClient httpClient);

        public Task<IEnumerable<ProcurementAuditCustomer>> GetAllAsync(HttpClient httpClient, string filter);

        public Task<ProcurementAuditCustomer> CreateAsync(HttpClient httpClient, ProcurementAuditCustomerDto dto);
    }
}