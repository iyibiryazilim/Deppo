using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface ICustomerDetailService
    {
        public Task<DataResult<IEnumerable<dynamic>>> GetLastFichesByCustomer(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId);

        Task<DataResult<IEnumerable<dynamic>>> GetLastTransaction(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId);
    }
}