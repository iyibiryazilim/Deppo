using Deppo.Core.DataResultModel;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.Services
{
    public interface IProductPanelService
    {
        public Task<DataResult<IEnumerable<dynamic>>> GetLastProducts(HttpClient httpClient, int firmNumber, int periodNumber);

        public Task<DataResult<IEnumerable<dynamic>>> GetLastTransactions(HttpClient httpClient, int firmNumber, int periodNumber);


        public Task<DataResult<IEnumerable<dynamic>>> GetLastWarehouses(HttpClient httpClient, int firmNumber, int periodNumber);

        public Task<DataResult<dynamic>> GetInputProductQuantity(HttpClient httpClient, int firmNumber, int periodNumber);


        public Task<DataResult<dynamic>> GetOutputProductQuantity(HttpClient httpClient, int firmNumber, int periodNumber);

    }
}
