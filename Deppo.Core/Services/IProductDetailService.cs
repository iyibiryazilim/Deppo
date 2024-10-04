using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface IProductDetailService
    {
        public Task<DataResult<IEnumerable<dynamic>>> GetLastFichesByProduct(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId);

        Task<DataResult<IEnumerable<dynamic>>> GetLastTransaction(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, int productReferenceId);
    }
}