using Deppo.Sys.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Services
{
    public interface IReasonsForRejectionService
    {
        public Task<IEnumerable<ReasonsForRejection>> GetAllAsync(HttpClient httpClient);

        public Task<IEnumerable<ReasonsForRejection>> GetAllAsync(HttpClient httpClient, string filter);
    }
}