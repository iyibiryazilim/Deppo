using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface ILocationTransactionService
{
	Task<DataResult<IEnumerable<dynamic>>> GetObjectsAsync(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int skip = 0, int take = 20, string search = "");

}
