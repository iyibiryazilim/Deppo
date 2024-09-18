using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
    public interface ICarrierService
    {
        Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber);
    }
}
