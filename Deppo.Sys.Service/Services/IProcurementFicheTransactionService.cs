using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Services
{
    public interface IProcurementFicheTransactionService
    {
        public Task<IEnumerable<ProcurementFicheTransaction>> GetAllAsync(HttpClient httpClient);

        public Task<IEnumerable<ProcurementFicheTransaction>> GetAllAsync(HttpClient httpClient, string filter);

        public Task<ProcurementFicheTransaction> CreateAsync(HttpClient httpClient, ProcurementFicheTransactionDto dto);

    }
}
