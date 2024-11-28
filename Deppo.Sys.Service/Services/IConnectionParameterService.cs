using Deppo.Sys.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Services
{
    public interface IConnectionParameterService
    {
        public Task<IEnumerable<ConnectionParameter>> GetAllAsync(HttpClient httpClient);

        public Task<IEnumerable<ConnectionParameter>> GetAllAsync(HttpClient httpClient, string filter);
    }
}