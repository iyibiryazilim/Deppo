using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
    public interface IDriverService
    {
        Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient);
    }
}
