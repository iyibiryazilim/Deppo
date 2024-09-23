using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
    public interface IWarehouseCountingService
    {
        Task<DataResult<IEnumerable<dynamic>>> GetNegativeProducts(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20);

        Task<DataResult<IEnumerable<dynamic>>> GetNegativeWarehousesByProductReferenceId(HttpClient httpClient, int firmNumber, int periodNumber,int productReferenceId);

    }
}
