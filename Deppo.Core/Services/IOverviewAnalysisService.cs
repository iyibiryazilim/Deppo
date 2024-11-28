using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
    public interface IOverviewAnalysisService
    {
        Task<DataResult<dynamic>> GetTotalProductCountAsync(HttpClient httpClient, int firmNumber);
        Task<DataResult<dynamic>> GetTotalInputProductCountAsync(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<dynamic>> GetTotalOutputProductCountAsync(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<dynamic>> GetInputTransactionCountAsync(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<dynamic>> GetOutputTransactionCountAsync(HttpClient httpClient, int firmNumber, int periodNumber);
        Task<DataResult<dynamic>> GetProductsWithNoTransactionsCountAsync(HttpClient httpClient, int firmNumber, int periodNumber, DateTime date);
        Task<DataResult<IEnumerable<dynamic>>> GetProductsWithNoTransactionsAsync(HttpClient httpClient, int firmNumber, int periodNumber, DateTime date, string search = "", int skip = 0, int take = 20);


    }
}
