using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services
{
    public interface IWarehouseDetailService
    {
        public Task<DataResult<IEnumerable<dynamic>>> GetLastFiches(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseReferenceId);

        Task<DataResult<IEnumerable<dynamic>>> GetLastTransaction(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, int warehouseNumber);

        Task<DataResult<dynamic>> GetInputQuantity(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber);

        Task<DataResult<dynamic>> GetOutputQuantity(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber);

        Task<DataResult<IEnumerable<dynamic>>> ProductInputOutputReferences(HttpClient httpClient, int firmNumber, int periodNumber, DateTime dateTime, int warehouseNumber);

        Task<DataResult<dynamic>> WarehouseReferenceCount(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber);

        Task<DataResult<dynamic>> WarehouseLastTransactionDate(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber);



    }
}