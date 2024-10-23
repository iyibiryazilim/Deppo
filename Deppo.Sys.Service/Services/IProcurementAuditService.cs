using Deppo.Sys.Service.Models;

namespace Deppo.Sys.Service.Services
{
	public interface IProcurementAuditService
    {
        public Task<IEnumerable<ProcurementAudit>> GetAllAsync(HttpClient httpClient);

        public Task<IEnumerable<ProcurementAudit>> GetAllAsync(HttpClient httpClient, string filter);
    }
}