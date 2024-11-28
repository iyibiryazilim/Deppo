using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Services
{
    public interface IProcurementFicheService
    {
        public Task<IEnumerable<ProcurementFiche>> GetAllAsync(HttpClient httpClient);

        public Task<IEnumerable<ProcurementFiche>> GetAllAsync(HttpClient httpClient, string filter);

        public Task<ProcurementFiche> CreateAsync(HttpClient httpClient, ProcurementFicheDto dto);

    }
}
