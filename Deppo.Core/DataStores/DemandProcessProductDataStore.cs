using Deppo.Core.DataResultModel;
using Deppo.Core.Services;

namespace Deppo.Core.DataStores
{
    public class DemandProcessProductDataStore : IDemandProcessProductService
    {
        public Task<DataResult<IEnumerable<dynamic>>> GetProducts(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
        {
            throw new NotImplementedException();
        }
    }
}
