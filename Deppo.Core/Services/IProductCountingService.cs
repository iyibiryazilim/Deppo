using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
    public interface IProductCountingService
    {
        Task<DataResult<IEnumerable<dynamic>>> GetWarehouses(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId,string search = "", int skip = 0, int take = 20, string externalDb = "");
        Task<DataResult<IEnumerable<dynamic>>> GetWarehousesByVariant(HttpClient httpClient, int firmNumber, int periodNumber, int variantReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "");

    }
}
