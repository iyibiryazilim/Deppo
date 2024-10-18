using Deppo.Sys.Module.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Services
{
    public interface IProcurementAuditService
    {
        public Task<IEnumerable<ProcurementAudit>> GetAllAsync(HttpClient httpClient);

        public Task<IEnumerable<ProcurementAudit>> GetAllAsync(HttpClient httpClient, string filter);
    }
}