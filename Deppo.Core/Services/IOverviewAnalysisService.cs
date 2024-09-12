using Deppo.Core.DataResultModel;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.Services
{
    public interface IOverviewAnalysisService
    {
        public Task<DataResult<dynamic>> GetTotalProductCountAsync(HttpClient httpClient, int firmNumber);

        public Task<DataResult<dynamic>> GetTotalInputProductCountAsync(HttpClient httpClient, int firmNumber, int periodNumber);

        public Task<DataResult<dynamic>> GetTotalOutputProductCountAsync(HttpClient httpClient, int firmNumber, int periodNumber);

        public Task<DataResult<dynamic>> GetInputTransactionCountAsync(HttpClient httpClient, int firmNumber, int periodNumber);

        public Task<DataResult<dynamic>> GetOutputTransactionCountAsync(HttpClient httpClient, int firmNumber, int periodNumber);


    }
}
