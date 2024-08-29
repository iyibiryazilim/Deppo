using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface ILocationService
{
	Task<DataResult<IEnumerable<dynamic>>> GetObjectsAsync(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int skip, int take, string search);

}
