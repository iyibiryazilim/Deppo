using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface IPurchasePanelService
    {
        Task<DataResult<IEnumerable<dynamic>>> GetLastTransactionBySupplier(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<IEnumerable<dynamic>>> SupplierTransaction(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<dynamic>> TotalOrderCount(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<dynamic>> ShippedOrderCount(HttpClient httpClient, int firmNumber, int periodNumber);
    }
}
