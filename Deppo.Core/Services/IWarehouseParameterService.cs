using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
    public interface IWarehouseParameterService
    {
        Task<DataResult<dynamic>> GetObjectsByProduct(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int productReferenceId,string search = "", int skip = 0, int take = 20);
        Task<DataResult<dynamic>> GetObjectsByVariant(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int variantReferenceId, string search = "", int skip = 0, int take = 20);
    }
}
