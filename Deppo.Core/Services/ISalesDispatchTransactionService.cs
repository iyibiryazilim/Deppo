using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface ISalesDispatchTransactionService
    {
        Task<DataResult<IEnumerable<dynamic>>> GetTransactionsByFicheReferenceId(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, string search = "", int skip = 0, int take = 20);
        Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int customerReferenceId, string search = "", int skip = 0, int take = 20);
    }
}
