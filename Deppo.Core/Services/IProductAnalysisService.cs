using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
    public interface IProductAnalysisService
    {
        public Task<DataResult<dynamic>> GetTotalProductCountAsync(HttpClient httpClient, int firmNumber);
        public Task<DataResult<dynamic>> GetInStockProductCountAsync(HttpClient httpClient, int firmNumber, int periodNumber);
        public Task<DataResult<dynamic>> GetOutStockProductCountAsync(HttpClient httpClient, int firmNumber, int periodNumber);
        public Task<DataResult<dynamic>> GetInputTransactionCountAsync(HttpClient httpClient, int firmNumber, int periodNumber);

        public Task<DataResult<dynamic>> GetOutputTransactionCountAsync(HttpClient httpClient, int firmNumber, int periodNumber);

        public Task<DataResult<IEnumerable<dynamic>>> GetLastWarehousesAsync(HttpClient httpClient, int firmNumber, int periodNumber);
        public Task<DataResult<dynamic>> GetNegativeStockProductsCountAsync(HttpClient httpClient, int firmNumber, int periodNumber);


    }
}
