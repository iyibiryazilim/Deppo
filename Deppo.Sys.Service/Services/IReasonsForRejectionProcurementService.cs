using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Services
{
    public interface IReasonsForRejectionProcurementService
    {
        public Task<IEnumerable<ReasonsForRejectionProcurement>> GetAllAsync(HttpClient httpClient);

        public Task<IEnumerable<ReasonsForRejectionProcurement>> GetAllAsync(HttpClient httpClient, string filter);

		public Task<ReasonsForRejectionProcurement> CreateAsync(HttpClient httpClient, ReasonsForRejectionDto dto);
	}
}