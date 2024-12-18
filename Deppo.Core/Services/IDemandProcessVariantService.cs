using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
    public interface IDemandProcessVariantService
    {
        Task<DataResult<IEnumerable<dynamic>>> GetVariants(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20, string externalDb = "");
    }
}
